using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.ConnectionToKeil
{
    static class ConnectionToKeilClass
    {
        public static bool runningFlag = true;
        public static Stopwatch stopWatch = new Stopwatch();
        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern UVSC_STATUS UVSC_OpenConnection([MarshalAs(UnmanagedType.LPStr)] string name,
                                                    short* iConnHandle,
                                                    short* pPort,
                                                    [MarshalAs(UnmanagedType.LPStr)] string uvCmd,
                                                    UVSC_RUNMODE uvRunmode,
                                                    uvsc_cb callback,
                                                    void* cb_custom,
                                                    [MarshalAs(UnmanagedType.LPStr)] string logFileName,
                                                    bool logFileAppend,
                                                    log_cb logCallback);


        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern UVSC_STATUS UVSC_CloseConnection(int iConnHandle,
        bool terminate);

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UVSC_STATUS UVSC_Init(short uvMinPort,
                                      short uvMaxPort);

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UVSC_STATUS UVSC_UnInit(); 

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UVSC_STATUS UVSC_DBG_ENTER(short iConnHandle);

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UVSC_STATUS UVSC_DBG_EXIT(int iConnHandle);

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern UVSC_STATUS UVSC_DBG_CREATE_BP(
            int iConnHandle,
            [In, Out] BKPARM pBkptSet,
            int bkptSetLen,
            [In, Out] BKRSP[] pBkptRsp,
            ref int pBkptRspLen);

        [DllImport("UVSC64.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern UVSC_STATUS UVSC_DBG_START_EXECUTION(int iConnHandle);

        [MarshalAs(UnmanagedType.LPStr)]
        static string uvCmd = "";

        //Callback for 
        private static void keilCallback(IntPtr cb_custom, UVSC_CB_TYPE type, ref UVSC_CB_DATA data)
        {
            if ((data.msg.data.cmdRsp.cmd == UV_OPERATION.UV_DBG_STOP_EXECUTION) && (data.msg.data.cmdRsp.StopR.nBpNum != -1))
            {
                if(data.msg.data.cmdRsp.StopR.nBpNum == 0)
                {
                    stopWatch.Start();
                }
                else if (data.msg.data.cmdRsp.StopR.nBpNum == 1)
                {
                    stopWatch.Stop();
                    runningFlag = false;
                }
            }
            return; 
        }

        public static TimeSpan GetFunctionRunTime(int StartAddress, int EndAddress)
        {
            const int MAX_OP_TRIES = 3;
            int trynumber = 1;

            short connectionHndl = 0, port = 0;
            uvsc_cb newCallback = new uvsc_cb(keilCallback);
            UVSC_STATUS operationStatus = UVSC_STATUS.UVSC_STATUS_FAILED;
            UVSC_RUNMODE uvRunmode = UVSC_RUNMODE.UVSC_RUNMODE_NORMAL;
            String startAddress = (StartAddress + 1).ToString();
            String endAddress = (EndAddress - 1).ToString();
            BKRSP[] bkrsp = new BKRSP[128];

            BKPARM bk1params = new BKPARM();
            bk1params.type = BKTYPE.BRKTYPE_EXEC;
            bk1params.count = 1;
            bk1params.accSize = 0;
            bk1params.nCmdLen = 1;
            bk1params.szBuffer = startAddress;
            bk1params.nExpLen = (uint)startAddress.Length + 1;

            BKPARM bk2params = new BKPARM();
            bk2params.type = BKTYPE.BRKTYPE_EXEC;
            bk2params.count = 1;
            bk2params.accSize = 0;
            bk2params.nCmdLen = 1;
            bk2params.szBuffer = endAddress;
            bk2params.nExpLen = (uint)endAddress.Length + 1;


            stopWatch.Reset();
            unsafe
            {
                // Should to do something more flexible
                UVSC_Init(4823, 4832);

                while (operationStatus != UVSC_STATUS.UVSC_STATUS_SUCCESS && trynumber != MAX_OP_TRIES)
                {
                    operationStatus = UVSC_OpenConnection(null, &connectionHndl, &port, uvCmd, uvRunmode, newCallback, null, null, false, null);
                    trynumber++;
                }
               
                operationStatus = UVSC_DBG_ENTER(connectionHndl);
                if(operationStatus != UVSC_STATUS.UVSC_STATUS_SUCCESS)
                {
                    return new TimeSpan(0);
                }

                int bkRspSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BKRSP));
                int bkParamsSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BKPARM));

                operationStatus = UVSC_DBG_CREATE_BP(connectionHndl, bk1params, bkParamsSize, bkrsp, ref bkRspSize);
                if (operationStatus != UVSC_STATUS.UVSC_STATUS_SUCCESS)
                {
                    return new TimeSpan(0);
                }

                operationStatus = UVSC_DBG_CREATE_BP(connectionHndl, bk2params, bkParamsSize, bkrsp, ref bkRspSize);
                if (operationStatus != UVSC_STATUS.UVSC_STATUS_SUCCESS)
                {
                    return new TimeSpan(0);
                }

                operationStatus = UVSC_DBG_START_EXECUTION(connectionHndl);
                if (operationStatus != UVSC_STATUS.UVSC_STATUS_SUCCESS)
                {
                    return new TimeSpan(0);
                }

                while (runningFlag)
                {
                    Thread.Sleep(1);
                }

                UVSC_CloseConnection(connectionHndl, false);
                UVSC_UnInit();
            }

            return stopWatch.Elapsed;
        }

        // approximate algorithm of work with this lib 
        /*
        short connectionHndl = 0, port = 0;
            uvsc_cb newCallback = new uvsc_cb(callback2);
            UVSC_STATUS stat = UVSC_STATUS.UVSC_STATUS_FAILED;
            UVSC_RUNMODE uvRunmode = UVSC_RUNMODE.UVSC_RUNMODE_NORMAL;
            BKPARM bkparams = new BKPARM();
            bkparams.type = BKTYPE.BRKTYPE_EXEC;
            bkparams.count = 1; 
            bkparams.accSize = 0;
            bkparams.nExpLen = 10; // szBuffer lenght + 1
            bkparams.nCmdLen = 1;
            bkparams.szBuffer = "134220798";
            BKRSP[] bkrsp = new BKRSP[128];

            UVSC_Init(4823, 4832);

            unsafe
            {
                while (stat != UVSC_STATUS.UVSC_STATUS_SUCCESS)
                {
                    stat =  UVSC_OpenConnection(null, &connectionHndl, &port, uvCmd, uvRunmode, callback2, null, null, false, null);
                    Console.WriteLine(stat);
                }
                stat = UVSC_DBG_ENTER(connectionHndl);
                Console.WriteLine(stat);
                int bkRspSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BKRSP));
                int bkParamsSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BKPARM));
                stat = UVSC_DBG_CREATE_BP(connectionHndl, bkparams, bkParamsSize, bkrsp, ref bkRspSize);
                Console.WriteLine(stat);
                stat = UVSC_DBG_START_EXECUTION(connectionHndl);
                Console.WriteLine(stat);
                 while(runningFlag)
                 {
                     UVSC_DBG_START_EXECUTION(connectionHndl);
                    Thread.Sleep(1);
                 }

                Console.Read();
        */
    }
}

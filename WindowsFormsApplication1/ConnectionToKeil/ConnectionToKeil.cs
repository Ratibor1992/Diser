using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.ConnectionToKeil
{
    class ConnectionToKeil
    {
        public static bool runningFlag = true;
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
        public static extern UVSC_STATUS UVSC_Init(short uvMinPort,
                                      short uvMaxPort);



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

        //Callback for 
        public void keilCallback(IntPtr cb_custom, UVSC_CB_TYPE type, ref UVSC_CB_DATA data)
        {
            if ((data.msg.data.cmdRsp.cmd == UV_OPERATION.UV_DBG_STOP_EXECUTION) && (data.msg.data.cmdRsp.StopR.nBpNum != -1))
            {
                Console.WriteLine(data.msg.data.cmdRsp.StopR.eReason);
                Console.WriteLine(data.msg.data.cmdRsp.StopR.nBpNum);
                runningFlag = false;
            }
            return; 
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

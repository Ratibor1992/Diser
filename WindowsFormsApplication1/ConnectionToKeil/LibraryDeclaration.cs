using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.ConnectionToKeil
{
    static class LibraryDeclarations
    {
        static int UVSC_MAX_CLIENTS = 10;
        static int UVSC_MAX_API_STR_SIZE = 1024;
        static int UVSC_PORT_AUTO = 0;
        static int UVSC_MIN_AUTO_PORT = 1;
        static int UVSC_MAX_AUTO_PORT = 65535;
        static int SOCK_NDATA = 32768;
    }

    public enum UVSC_STATUS
    {
        UVSC_STATUS_SUCCESS = 0,  ///< Success
        UVSC_STATUS_FAILED = 1,  ///< General failure
        UVSC_STATUS_NOT_SUPPORTED = 2,  ///< Request for an unsupported operation
        UVSC_STATUS_NOT_INIT = 3,  ///< UVSC not initialised
        UVSC_STATUS_TIMEOUT = 4,  ///< Operation timed-out
        UVSC_STATUS_INVALID_CONTEXT = 5,  ///< Function called from an invalid context (most likely the callback function)
        UVSC_STATUS_INVALID_PARAM = 6,  ///< Function called with one or more invalid parameters
        UVSC_STATUS_BUFFER_TOO_SMALL = 7,  ///< Function called with a buffer that was not big enough to hold the result from uVision
        UVSC_STATUS_CALLBACK_IN_USE = 8,  ///< Function cannot be used when the callback is in use
        UVSC_STATUS_COMMAND_ERROR = 9,  ///< The command failed - call #UVSC_GetLastError to get more information on how the command failed
        UVSC_STATUS_END                       ///< Always at end
    };

    enum UVSC_RUNMODE
    {
        UVSC_RUNMODE_NORMAL = 0,  ///< Normal uVision operation
        UVSC_RUNMODE_LABVIEW = 1,  ///< LabVIEW operation
        UVSC_RUNMODE_END = 2,  ///< Always at end
    };

    /** Progress bar operations
      *
      * Progress bar operations as returned by #UVSC_ReadPBarQ.
      */
    enum UVSC_PBAR
    {
        UVSC_PBAR_INIT = 0,  ///< Initialise progress bar
        UVSC_PBAR_TEXT = 1,  ///< Set progress bar text
        UVSC_PBAR_POS = 2,  ///< Set progress bar position
        UVSC_PBAR_STOP = 3,  ///< Stop progress bar
    }

    enum UVSC_CB_TYPE
    {
        UVSC_CB_ERROR = 0,  ///< Error notification (not used)
        UVSC_CB_ASYNC_MSG = 1,  ///< Asynchronous message received (called from UVSC internal thread)  
        UVSC_CB_DISCONNECTED = 2,  ///< uVision has disconnected (called from UVSC internal thread)
        UVSC_CB_BUILD_OUTPUT_MSG = 3,  ///< Called from the #UVSC_PRJ_BUILD function - indicates a line of build output (called from API function callers own thread)  
        UVSC_CB_PROGRESS_BAR_MSG = 4,  ///< Called from function that cause a progress bar in uVision - indicates the progress bar state (called from API function callers own thread)  
        UVSC_CB_CMD_OUTPUT_MSG = 5,  ///< Called from the #UVSC_DBG_EXEC_CMD function - indicates a line of command output (called from API function callers own thread)  
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UVSOCK_CMD
    {
        [FieldOffset(0)]
        public UInt32 m_nTotalLen;    ///< Total message length (bytes)
        [FieldOffset(4)]
        public UV_OPERATION m_eCmd;    ///< Command code
        [FieldOffset(8)]
        public UInt32 m_nBufLen;    ///< Length of Data Section (bytes)
        [FieldOffset(12)]
        public UInt64 cycles;    ///< Cycle value (Simulation mode only)
        [FieldOffset(20)]
        public double tStamp;    ///< time-stamp (Simulation mode only)
        [FieldOffset(28)]
        public UInt32 m_Id;    ///< Reserved
        [FieldOffset(32)]
        public UVSOCK_CMD_DATA data;    ///< Data Section (Command code dependent data)
    }

    public struct PRJDATA
    {
        public UInt32 nLen;   ///< Length of @a szNames including NULL terminators
        public UInt32 nCode;   ///< Informational code
        public char[] szNames;   ///< Information ('string 1',0 [,'string 2',0] ... [,'string N',0])
    }

    public struct AMEM
    {
        public UInt64 nAddr;   ///< Address to read / write
        public UInt32 nBytes;   ///< Number of bytes read / write
        public UInt64 ErrAddr;   ///< Unused
        public UInt32 nErr;   ///< Unused
        public Byte[] aBytes;   ///< @a nBytes of data read or to be written
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SERIO
    {
        [FieldOffset(0)]
        public ushort nChannel;   ///< 0:=UART#1, 1:=UART#2, 2:=UART#3, 3:=Debug (printf) output 
        [FieldOffset(2)]
        public ushort itemMode;   ///< 0:=Bytes, 1:=WORD16
        [FieldOffset(4)]
        public ulong nMany;   ///< number of items (BYTE or WORD16)
        [FieldOffset(8)]
        byte aBytes;   ///< @a nMany Bytes follow here.
        [FieldOffset(9)]
        ushort aWords;   ///< @a nMany Word16 follow here.
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ITMOUT
    {
        [FieldOffset(0)]
        public ushort nChannel;   ///< 0:=UART#1, 1:=UART#2, 2:=UART#3, 3:=Debug (printf) output 
        [FieldOffset(2)]
        public ushort itemMode;   ///< 0:=Bytes, 1:=WORD16
        [FieldOffset(4)]
        public ulong nMany;   ///< number of items (BYTE or WORD16)
        [FieldOffset(8)]
        byte aBytes;   ///< @a nMany Bytes follow here.
        [FieldOffset(9)]
        ushort aWords;   ///< @a nMany Word16 follow here.
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct TVAL
    {
        [FieldOffset(0)]
        VTT_TYPE vType;   ///< Indicates the type of data in @a v
        [FieldOffset(4)]
        uint ul;   ///< #VTT_ulong
        [FieldOffset(4)]
        char sc;   ///< #VTT_char
        [FieldOffset(4)]
        byte uc;   ///< #VTT_uchar
        [FieldOffset(4)]
        short i16;   ///< #VTT_short
        [FieldOffset(4)]
        ushort u16;   ///< #VTT_ushort
        [FieldOffset(4)]
        int l;   ///< #VTT_long
        [FieldOffset(4)]
        short i;   ///< #VTT_int
        [FieldOffset(4)]
        long i64;   ///< #VTT_int64
        [FieldOffset(4)]
        ulong u64;   ///< #VTT_uint64
        [FieldOffset(4)]
        float f;   ///< #VTT_float
        [FieldOffset(4)]
        double d;   ///< #VTT_double
    }

    public unsafe struct SSTR
    {
        public int nLen;   ///< Length of name (including NULL terminator)
        public fixed char szSt[256];   ///< NULL terminated name string
    }

    public struct VSET
    {
        public TVAL val;   ///< Value of VTREG or register index
        public SSTR str;   ///< Name of VTREG or expression
    }

    public unsafe struct UVSOCK_ERROR_RESPONSE
    {
        public uint nRes1;    ///< Reserved
        public uint nRes2;    ///< Reserved
        public uint StrLen;    ///< Length of error string (including terminator) in bytes
        public fixed byte str[32768 - 20];
    }

    public unsafe struct BPREASON
    {
        public uint nRes1;   ///< Reserved
        public uint nRes2;   ///< Reserved
        public uint StrLen;   ///< Unused
        public STOPREASON eReason;   ///< Reason for stopping execution
        public ulong nPC;   ///< Address of PC when stopped
        public ulong nAdr;   ///< Address of break reason (i.e. memory access address, or breakpoint address)
        public int nBpNum;   ///< Breakpoint number (-1:=undefined)
        public uint nTickMark;   ///< Time of breakpoint creation, used to identify individual breakpoints (0 if @a nBpNum is undefined)
        public fixed uint nRes[4];   ///< Reserved
    }

    // TODO: SOME FIELDS ARE INCORRECT TYPE (SIMULATION)
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct UVSOCK_CMD_RESPONSE
    {
        [FieldOffset(0)]
        public UV_OPERATION cmd;    ///< Command or asynchronous operation to which this is a response
        [FieldOffset(4)]
        public UV_STATUS status;    ///< Status code indicating if the command was successful or not
        [FieldOffset(8)]
        public BPREASON StopR;
        /*
        [FieldOffset(8)]
        public UVSOCK_ERROR_RESPONSE err;    ///< Returned if status is not #UV_STATUS_SUCCESS or if from #UV_ASYNC_MSG
        [FieldOffset(8)]
        public uint nVal;    ///< Returned by #UV_PRJ_ACTIVE_FILES / #UV_GEN_GET_VERSION / #UV_DBG_STATUS
        [FieldOffset(8)]
        public AMEM time;    ///< Returned by #UV_DBG_TIME_INFO
        [FieldOffset(8)]
        public AMEM amem;    ///< Returned by #UV_DBG_MEM_READ / #UV_DBG_MEM_WRITE / #UV_DBG_DSM_READ
        [FieldOffset(8)]
        public SERIO serdat;    ///< Returned by #UV_DBG_SERIAL_OUTPUT / #UV_DBG_SERIAL_GET
        [FieldOffset(8)]
        public ITMOUT itmdat;    ///< Returned by #UV_DBG_ITM_OUTPUT
        [FieldOffset(8)]
        public VSET vset;    ///< Returned by #UV_DBG_VTR_GET / #UV_DBG_CALC_EXPRESSION
        [FieldOffset(8)]
        public BKRSP brk;    ///< Returned by #UV_DBG_BP_ENUMERATED / #UV_DBG_CHANGE_BP
        [FieldOffset(8)]
        public AMEM trnopt;    ///< Returned by #UV_PRJ_GET_OPTITEM
        [FieldOffset(8)]
        public SSTR str;    ///< Returned by #UV_PRJ_ENUM_GROUPS_ENU / #UV_PRJ_ENUM_FILES_ENU / #UV_PRJ_ENUM_TARGETS_ENU / #UV_PRJ_GET_CUR_TARGET / #UV_PRJ_GET_OUTPUTNAME/ #UV_PRJ_SET_OUTPUTNAME
        [FieldOffset(8)]
        public AMEM evers;    ///< Returned by #UV_GEN_GET_EXTVERSION
        [FieldOffset(8)]
        public AMEM tpm;    ///< Returned by #UV_DBG_ENUM_SYMTP_ENU
        [FieldOffset(8)]
        public AMEM aflm;    ///< Returned by #UV_DBG_ADR_TOFILELINE
        [FieldOffset(8)]
        public BPREASON StopR;    ///< Returned by #UV_DBG_STOP_EXECUTION
        [FieldOffset(8)]
        public BPREASON stack;    ///< Returned by #UV_DBG_ENUM_STACK_ENU
        [FieldOffset(8)]
        public BPREASON task;    ///< Returned by #UV_DBG_ENUM_TASKS
        [FieldOffset(8)]
        public BPREASON vtr;    ///< Returned by #UV_DBG_ENUM_VTR_ENU
        [FieldOffset(8)]
        public BPREASON licinfo;    ///< Returned by #UV_GEN_CHECK_LICENSE
        [FieldOffset(8)]
        public BPREASON dbgtgtopt;    ///< Returned by #UV_PRJ_GET_DEBUG_TARGET
        [FieldOffset(8)]
        public BPREASON powerScaleData;    ///< Returned by #UV_DBG_POWERSCALE_SHOWCODE / #UV_DBG_POWERSCALE_SHOWPOWER
        [FieldOffset(8)]
        public BPREASON regEnum;    ///< Returned by #UV_DBG_ENUM_REGISTERS
        [FieldOffset(8)]
        public BPREASON varInfo;    ///< Returned by #UV_DBG_EVAL_WATCH_EXPRESSION, #UV_DBG_ENUM_VARIABLES
        [FieldOffset(8)]
        public BPREASON viewInfo;   ///< Returned by #UV_DBG_ENUM_MENUS_ENU
        [FieldOffset(8)]
        public fixed char strbuf[1];    ///< Returned by #UV_DBG_READ_REGISTERS 
        */
    }

    // TODO: SOME FIELDS ARE INCORRECT TYPE (SIMULATION)
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct UVSOCK_CMD_DATA
    {
        [FieldOffset(0)]
        public fixed byte raw[32768];    ///< Command-dependent raw data
         /*
        [FieldOffset(0)]
        public PRJDATA prjdata;    ///< Sent in #UV_PRJ_LOAD / #UV_PRJ_ADD_GROUP / #UV_PRJ_SET_TARGET / #UV_PRJ_ADD_FILE / #UV_PRJ_DEL_GROUP / #UV_PRJ_DEL_FILE / #UV_PRJ_SET_OUTPUTNAME. Returned by #UV_PRJ_BUILD_OUTPUT / #UV_PRJ_BUILD_COMPLETE / #UV_RTA_MESSAGE
        [FieldOffset(0)]
        public AMEM amem;    ///< Sent in #UV_DBG_MEM_READ / #UV_DBG_MEM_WRITE / #UV_DBG_DSM_READ
        [FieldOffset(0)]
        public SERIO serdat;    ///< Sent in #UV_DBG_SERIAL_GET / #UV_DBG_SERIAL_PUT
        [FieldOffset(0)]
        public ITMOUT itmdat;    ///< Sent in #UV_DBG_ITM_REGISTER / #UV_DBG_ITM_UNREGISTER / #UV_DBG_ITM_OUTPUT
        [FieldOffset(0)]
        public VSET vset;    ///< Sent in #UV_DBG_VTR_GET / #UV_DBG_VTR_SET / #UV_DBG_CALC_EXPRESSION
        [FieldOffset(0)]
        public VSET trnopt;    ///< Sent in #UV_PRJ_GET_OPTITEM / #UV_PRJ_SET_OPTITEM
        [FieldOffset(0)]
        public SSTR sstr;    ///< Sent in #UV_PRJ_ENUM_FILES. Returned by #UV_DBG_CMD_OUTPUT
        [FieldOffset(0)]
        public BKPARM bkparm;    ///< Sent in #UV_DBG_CREATE_BP
        [FieldOffset(0)]
        public BKPARM bkchg;    ///< Sent in #UV_DBG_CHANGE_BP
        [FieldOffset(0)]
        public BKPARM dbgtgtopt;    ///< Sent in #UV_PRJ_SET_DEBUG_TARGET
        [FieldOffset(0)]
        public BKPARM adrmtfl;    ///< Sent in #UV_DBG_ADR_TOFILELINE
        [FieldOffset(0)]
        public BKPARM ishowsync;    ///< Sent in #UV_DBG_ADR_SHOWCODE
        [FieldOffset(0)]
        public BKPARM ivtrenum;    ///< Sent in #UV_DBG_ENUM_VTR
        [FieldOffset(0)]
        public BKPARM execcmd;    ///< Sent in #UV_DBG_EXEC_CMD
        [FieldOffset(0)]
        public BKPARM iPathReq;    ///< Sent in #UV_PRJ_GET_OUTPUTNAME / #UV_PRJ_GET_CUR_TARGET
        [FieldOffset(0)]
        public BKPARM powerScaleData;    ///< Sent in #UV_DBG_POWERSCALE_SHOWPOWER
        [FieldOffset(0)]
        public BKPARM iStkEnum;    ///< Sent in #UV_DBG_ENUM_STACK
        [FieldOffset(0)]
        public BKPARM pgress;    ///< Sent in #UV_PRJ_CMD_PROGRESS
        [FieldOffset(0)]
        public BKPARM enumtpm;    ///< Sent in #UV_DBG_ENUM_SYMTP
        [FieldOffset(0)]
        public BKPARM iInterval;    ///< Sent in #UV_DBG_WAKE
        [FieldOffset(0)]
        public BKPARM nVal;    ///< Sent in #UV_DBG_STEP_HLL_N, UV_DBG_STEP_INTO_N, UV_DBG_STEP_INSTRUCTION_N 
        [FieldOffset(0)]
        public BKPARM nAddress;    ///< Sent in #UV_DBG_RUN_TO_ADDRESS 
        [FieldOffset(0)]
        public BKPARM uvSockOpt;    ///< Sent in #UV_GEN_SET_OPTIONS  
            */
        // Command response, or new format asynchronous message data
        [FieldOffset(0)]
        public UVSOCK_CMD_RESPONSE cmdRsp;    ///< Command response formatted data
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UVSC_CB_DATA
    {
        [FieldOffset(0)]
        public UVSOCK_CMD msg;

        [FieldOffset(0)]
        public UVSC_STATUS err;

        [FieldOffset(0)]
        public int iConnHandle;
    }

    public enum BKTYPE
    {
        BRKTYPE_EXEC = 1,    ///< Execution Breakpoint
        BRKTYPE_READ = 2,    ///< Read Access Breakpoint
        BRKTYPE_WRITE = 3,    ///< Write Access Breakpoint
        BRKTYPE_READWRITE = 4,    ///< ReadWrite Access Breakpoint
        BRKTYPE_COMPLEX = 5,    ///< Complex Breakpoint (Expression Breakpoint)
        BRKTYPE_END               ///< Always at end
    }

    public struct BKPARM
    {
        public BKTYPE type;   ///< Type of breakpoint
        public UInt32 count;   ///< Number of occurrances before actually hit
        public UInt32 accSize;   ///< Access size for non-execution type breakpoints
        public UInt32 nExpLen;   ///< Length of breakpoint expression, including zero terminator
        public UInt32 nCmdLen;   ///< Length of breakpoint command, including zero terminator, or 0 if no breakpoint command is required
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string szBuffer;      ///< Breakpoint strings ('breakpoint expression',0 [,'breakpoint command',0])
    }

    public struct BKRSP
    {
        public BKTYPE type;    ///< Type of breakpoint
        public UInt32 count;    ///< Number of occurrances before actually hit
        public UInt32 enabled;    ///< 1:=Breakpoint is enabled, 0:=Breakpoint is disabled
        public UInt32 nTickMark;    ///< Time of breakpoint creation, used to identify individual breakpoints
        public ulong nAddress;    ///< Breakpoint address
        public UInt32 nExpLen;    ///< Length of breakpoint expression, including zero terminator
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string szBuffer;
        //public char[] szBuffer;    ///< Breakpoint expression ('breakpoint expression',0)
    }

    //delegate void uvsc_cb(IntPtr cb_custom, UVSC_CB_TYPE type, IntPtr data);
    delegate void uvsc_cb(IntPtr cb_custom, UVSC_CB_TYPE type, ref UVSC_CB_DATA data);
    //delegate void log_cb([MarshalAs(UnmanagedType.LPStr)] string msg, int msgLen);
    unsafe delegate void log_cb(char* msg, int msgLen);

    public enum UV_STATUS
    {
        UV_STATUS_SUCCESS = 0,  ///< Operation successful: No error
        UV_STATUS_FAILED = 1,  ///< Operation failed: Generic / unknown error
        UV_STATUS_NO_PROJECT = 2,  ///< Operation failed: No project is currently open
        UV_STATUS_WRITE_PROTECTED = 3,  ///< Operation failed: The current project is write protected
        UV_STATUS_NO_TARGET = 4,  ///< Operation failed: No target is selected for the current project
        UV_STATUS_NO_TOOLSET = 5,  ///< Operation failed: No toolset is selected for the current target
        UV_STATUS_NOT_DEBUGGING = 6,  ///< Operation failed: The debugger is not running, this operation is only possible in debug mode
        UV_STATUS_ALREADY_PRESENT = 7,  ///< Operation failed: The group / file is already present in the current project
        UV_STATUS_INVALID_NAME = 8,  ///< Operation failed: One of the specified group / file / project name(s) is invalid
        UV_STATUS_NOT_FOUND = 9,  ///< Operation failed: File / group not found in the current project
        UV_STATUS_DEBUGGING = 10, ///< Operation failed: The debugger is running, this operation is only possible when not in debug mode
        UV_STATUS_TARGET_EXECUTING = 11, ///< Operation failed: The target is executing, this operation is not possible when target is executing
        UV_STATUS_TARGET_STOPPED = 12, ///< Operation failed: The target is stopped, this operation is not possible when target is stopped
        UV_STATUS_PARSE_ERROR = 13, ///< Operation failed: Error parsing data in request
        UV_STATUS_OUT_OF_RANGE = 14, ///< Operation failed: Data in request is out of range

        UV_STATUS_BP_CANCELLED = 15, ///< Operation failed: Create new breakpoint has been cancelled
        UV_STATUS_BP_BADADDRESS = 16, ///< Operation failed: Invalid address in create breakpoint
        UV_STATUS_BP_NOTSUPPORTED = 17, ///< Operation failed: Type of breakpoint is not supported (by target)
        UV_STATUS_BP_FAILED = 18, ///< Operation failed: Breakpoint creation failed (syntax error, nested command etc.)
        UV_STATUS_BP_REDEFINED = 19, ///< Breakpoint Info: A breakpoint has been redefined
        UV_STATUS_BP_DISABLED = 20, ///< Breakpoint Info: A breakpoint has been disabled
        UV_STATUS_BP_ENABLED = 21, ///< Breakpoint Info: A breakpoint has been enabled
        UV_STATUS_BP_CREATED = 22, ///< Breakpoint Info: A breakpoint has been created
        UV_STATUS_BP_DELETED = 23, ///< Breakpoint Info: A breakpoint has been deleted
        UV_STATUS_BP_NOTFOUND = 24, ///< Operation failed: Breakpoint with @a nTickMark cookie not found.

        UV_STATUS_BUILD_OK_WARNINGS = 25, ///< Build Info: A build was successful, but with warnings
        UV_STATUS_BUILD_FAILED = 26, ///< Build Info: A build failed with errors
        UV_STATUS_BUILD_CANCELLED = 27, ///< Build Info: A build was cancelled

        UV_STATUS_NOT_SUPPORTED = 28, ///< Operation failed: Requested operation is not supported
        UV_STATUS_TIMEOUT = 29, ///< Operation failed: No response to the request occurred within the timeout period (UVSOCK Client DLL only)
        UV_STATUS_UNEXPECTED_MSG = 30, ///< Operation failed: An unexpected message type was returned (UVSOCK Client DLL only)

        UV_STATUS_VERIFY_FAILED = 31, ///< Operation failed: The code downloaded in the target differs from the current binary

        UV_STATUS_NO_ADRMAP = 32, ///< Operation failed: The specified code address does not map to a file / line
        UV_STATUS_INFO = 33, ///< General Info: This is an information only message. It may contain warning information pertinent to a later error condition.

        UV_STATUS_NO_MEM_ACCESS = 34, ///< Operation failed: Memory access is blocked (most likely target does not support memory access while running)
        UV_STATUS_FLASH_DOWNLOAD = 35, ///< Operation failed: The target is downloading FLASH, this operation is not possible when FLASH is downloading
        UV_STATUS_BUILDING = 36, ///< Operation failed: A build is in progress, this operation is not possible when build is in progress
        UV_STATUS_HARDWARE = 37, ///< Operation failed: The debugger is debugging hardware, this operation is not possible when debugging a hardware target
        UV_STATUS_SIMULATOR = 38, ///< Operation failed: The debugger is debugging a simulation, this operation not possible when debugging a simulated target

        UV_STATUS_BUFFER_TOO_SMALL = 39, ///< Operation failed: Return buffer was too small (UVSOCK Client DLL only)

        UV_STATUS_END                       ///<  Always at end
    }

    public enum UV_OPERATION
    {

        //---Command codes:
        //--- General functions:
        UV_NULL_CMD = 0x0000,   ///< Not a command. A message containing this code should be ignored
        UV_GEN_GET_VERSION = 0x0001,   ///< Get the UVSOCK interface version number
                                       ///< @li Request format  : no data
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> UINT / #UVSOCK_ERROR_RESPONSE
        UV_GEN_UI_UNLOCK = 0x0002,   ///< Enable message boxes and user input in uVision
                                     ///< @li Request format  : no data
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_UI_LOCK = 0x0003,   ///< Disable message boxes and user input in uVision
                                   ///< @li Request format  : no data
                                   ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_HIDE = 0x0004,   ///< Completely hide the uVision window
                                ///< @li Request format  : no data
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_SHOW = 0x0005,   ///< Show the uVision window (bringing it to the front if it is behind other windows)
                                ///< @li Request format  : no data
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_RESTORE = 0x0006,   ///< Restore the uVision window
                                   ///< @li Request format  : no data
                                   ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_MINIMIZE = 0x0007,   ///< Minimise the uVision window
                                    ///< @li Request format  : no data
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_MAXIMIZE = 0x0008,   ///< Maximise the uVision window
                                    ///< @li Request format  : no data
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_EXIT = 0x0009,   ///< Exit uVision
                                ///< @li Request format  : no data
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_GET_EXTVERSION = 0x000A,   ///< Get extended version number information for uVision (in ASCII format)
                                          ///< @li Request format  : no data
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #EXTVERS / #UVSOCK_ERROR_RESPONSE
        UV_GEN_CHECK_LICENSE = 0x000B,   ///< Check toolchain licensing
                                         ///< @li Request format  : no data
                                         ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #UVLICINFO / #UVSOCK_ERROR_RESPONSE
        UV_GEN_CPLX_COMPLETE = 0x000C,   ///< Complex command has completed
                                         ///< @li Request format  : no data
                                         ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_GEN_SET_OPTIONS = 0x000D,   ///< Sets UVSOCK options 
                                       ///< @li Request format  : #UVSOCK_OPTIONS
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE


        //--- Project functions:
        UV_PRJ_LOAD = 0x1000,   ///< Load a uVision project
                                ///< @li Request format  : #PRJDATA
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_CLOSE = 0x1001,   ///< Close the currently loaded uVision project
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                 ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ADD_GROUP = 0x1002,   ///< Add one or more groups to the current project
                                     ///< @li Request format  : #PRJDATA
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_DEL_GROUP = 0x1003,   ///< Remove one or more groups from the current project
                                     ///< @li Request format  : #PRJDATA
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ADD_FILE = 0x1004,   ///< Add one or more files to a group in the current project
                                    ///< @li Request format  : #PRJDATA
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_DEL_FILE = 0x1005,   ///< Remove one or more files from a group in the current project
                                    ///< @li Request format  : #PRJDATA
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_BUILD = 0x1006,   ///< Build the current project
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_REBUILD = 0x1007,   ///< Rebuild the current project
                                   ///< @li Request format  : no data
                                   ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_CLEAN = 0x1008,   ///< Clean current project
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_BUILD_CANCEL = 0x1009,   ///< Stop a currently progressing build / rebuild
                                        ///< @li Request format  : no data
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_FLASH_DOWNLOAD = 0x100A,   ///< Download the built binary to flash
                                          ///< @li Request format  : no data
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_GET_DEBUG_TARGET = 0x100B,   ///< Get the currently configured debug target
                                            ///< @li Request format  : no data
                                            ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #DBGTGTOPT / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_SET_DEBUG_TARGET = 0x100C,   ///< Set the currently configured debug target
                                            ///< @li Request format  : #DBGTGTOPT
                                            ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_GET_OPTITEM = 0x100D,   ///< Get an option item for the current project, see #optsel
                                       ///< @li Request format  : #TRNOPT
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #TRNOPT / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_SET_OPTITEM = 0x100E,   ///< Set an option item for the current project, see #optsel
                                       ///< @li Request format  : #TRNOPT
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_GROUPS = 0x100F,   ///< Enumerate the groups of the current project
                                       ///< @li Request format  : no data
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_FILES = 0x1010,   ///< Enumerate the files of a given group in the current project
                                      ///< @li Request format  : #SSTR
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_CMD_PROGRESS = 0x1011,   ///< Control the uVision UI progress bar
                                        ///< @li Request format  : #PGRESS
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ACTIVE_FILES = 0x1012,   ///< Get number of active files for the current project (i.e. how many files would be built on a rebuild)
                                        ///< @li Request format  : no data
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> UINT / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_FLASH_ERASE = 0x1013,   ///< Erase flash device
                                       ///< @li Request format  : no data
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                       ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_GET_OUTPUTNAME = 0x1014,   ///< Get the executable/library output object name for the current project
                                          ///< @li Request format  : #iPATHREQ
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_TARGETS = 0x1015,   ///< Enumerate the targets of the current project
                                        ///< @li Request format  : no data
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_SET_TARGET = 0x1016,   ///< Set a target active
                                      ///< @li Request format  : #PRJDATA
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_GET_CUR_TARGET = 0x1017,   ///< returns a string containing the current active target
                                          ///< @li Request format  : #iPATHREQ
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE

        UV_PRJ_SET_OUTPUTNAME = 0x1018,   ///< Set a target name
                                          ///< @li Request format  : #PRJDATA
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE


        //--- Debug functions:
        UV_DBG_ENTER = 0x2000,   ///< Start the debugger
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                 ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_EXIT = 0x2001,   ///< Stop the debugger
                                ///< @li Request format  : no data
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_START_EXECUTION = 0x2002,   ///< Start target execution
                                           ///< @li Request format  : no data
                                           ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                           ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_RUN_TO_ADDRESS = 0x2102,   ///< Start target execution and run to specified address
                                          ///< @li Request format  : xU64
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STOP_EXECUTION = 0x2003,   ///< Stop target execution
                                          ///< @li Request format  : no data
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #BPREASON / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STATUS = 0x2004,   ///< Check if the target / simulation is running
                                  ///< @li Request format  : no data
                                  ///< @li Response format : #UVSOCK_CMD_RESPONSE --> UINT / #UVSOCK_ERROR_RESPONSE
        UV_DBG_RESET = 0x2005,   ///< Reset the target / simulation
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                 ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_HLL = 0x2006,   ///< Step one line over HLL code(High Level Language code, eg C)
                                    ///< @li Request format  : no data
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_HLL_N = 0x2106,   ///< Step N lines over HLL code(High Level Language code, eg C)
                                      ///< @li Request format  : UINT
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_INTO = 0x2007,   ///< Step into HLL code(High Level Language code, eg C)
                                     ///< @li Request format  : no data
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_INTO_N = 0x2107,   ///< Perform N steps into HLL code(High Level Language code, eg C)
                                       ///< @li Request format  : no data
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_INSTRUCTION = 0x2008,   ///< Step one ASM Instruction
                                            ///< @li Request format  : no data
                                            ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_INSTRUCTION_N = 0x2108,   ///< Step N ASM Instruction
                                              ///< @li Request format  : UINT
                                              ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_STEP_OUT = 0x2009,   ///< Step out of the current function
                                    ///< @li Request format  : no data
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_CALC_EXPRESSION = 0x200A,   ///< Calculate the value of an expression
                                           ///< @li Request format  : #VSET
                                           ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VSET / #UVSOCK_ERROR_RESPONSE
        UV_DBG_MEM_READ = 0x200B,   ///< Read memory
                                    ///< @li Request format  : #AMEM
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #AMEM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_MEM_WRITE = 0x200C,   ///< Write memory
                                     ///< @li Request format  : #AMEM
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #AMEM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_TIME_INFO = 0x200D,   ///< Get the current simulation cycles and time-stamp (NOTE: This information is also available in every UVSOCK message) [SIMULATOR ONLY]
                                     ///< @li Request format  : no data
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #CYCTS / #UVSOCK_ERROR_RESPONSE
        UV_DBG_SET_CALLBACK = 0x200E,   ///< Set a time-interval for callback [SIMULATOR ONLY]
                                        ///< @li Request format  : float (callback time in seconds)
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_VTR_GET = 0x200F,   ///< Read a Virtual Register (VTR) value
                                   ///< @li Request format  : #VSET
                                   ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VSET / #UVSOCK_ERROR_RESPONSE
        UV_DBG_VTR_SET = 0x2010,   ///< Write a Virtual Register (VTR) value
                                   ///< @li Request format  : #VSET
                                   ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_SERIAL_GET = 0x2011,   ///< DEPRECATED use DBG_SERIAL_OUTPUT response. Read serial output from a uVision serial window 
                                      ///< @li Request format  : #SERIO
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #SERIO / #UVSOCK_ERROR_RESPONSE
        UV_DBG_SERIAL_PUT = 0x2012,   ///< Write serial output to a uVision serial window
                                      ///< @li Request format  : #SERIO
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_VERIFY_CODE = 0x2013,   ///< Verify the code in flash against built binary
                                       ///< @li Request format  : no data
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                       ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_CREATE_BP = 0x2014,   ///< Create a new breakpoint
                                     ///< @li Request format  : #BKPARM
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUMERATE_BP = 0x2015,   ///< Enumerate all currently defined breakpoints
                                        ///< @li Request format  : no data
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_CHANGE_BP = 0x2016,   ///< Enable, disable or delete an existing breakpoint
                                     ///< @li Request format  : #BKCHG
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                     ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #BKRSP
        UV_DBG_ENUM_SYMTP = 0x2017,   ///< Enumerate the struct members of a variable, i.e. the member size and packing
                                      ///< @li Request format  : #ENUMTPM
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ADR_TOFILELINE = 0x2018,   ///< Map an address to code file & linenumber
                                          ///< @li Request format  : #ADRMTFL
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #AFLMAP / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_STACK = 0x2019,   ///< Enumerate the call stack
                                      ///< @li Request format  : #iSTKENUM
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_VTR = 0x201A,   ///< Enumerate all virtual registers (VTRs)
                                    ///< @li Request format  : #iVTRENUM
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_UNUSED = 0x201B,   ///< Unused
        UV_DBG_ADR_SHOWCODE = 0x201C,   ///< Show disassembly and/or HLL (High Level Language) file for an address
                                        ///< @li Request format  : #iSHOWSYNC
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
        UV_DBG_WAKE = 0x201D,   ///< Set sleep callback and/or wake up simulation [SIMULATOR ONLY]
                                ///< @li Request format  : #iINTERVAL
                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_SLEEP = 0x201E,   ///< Sleep the simulation [SIMULATOR ONLY]
                                 ///< @li Request format  : no data
                                 ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                 ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_MSGBOX_MSG = 0x201F,   ///< Notification of a UV message box
                                  ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_DBG_EXEC_CMD = 0x2020,   ///< Execute a command (as if via the command line)
                                    ///< @li Request format  : #EXECCMD  
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE
                                    ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE

        UV_DBG_POWERSCALE_SHOWCODE = 0x2021, ///< Show disassembly / HLL and trace entry in uVision for timestamp
                                             ///< @li Request format  : #UVSC_PSTAMP
                                             ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #UVSC_PSTAMP / #UVSOCK_ERROR_RESPONSE

        UV_DBG_POWERSCALE_SHOWPOWER = 0x2022, ///< Show power in PowerScale for timestamp
                                              ///< @li Request format  : #UVSC_PSTAMP
                                              ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #UVSC_PSTAMP / #UVSOCK_ERROR_RESPONSE

        POWERSCALE_OPEN = 0x2023,   ///< Register PowerScale device

        UV_DBG_EVAL_EXPRESSION_TO_STR = 0x2024,   ///< Evaluate expression and return result as string 
                                                  ///< @li Request format  : #VSET - value field is used to submit stack frame pointer
                                                  ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VSET / #UVSOCK_ERROR_RESPONSE

        UV_DBG_FILELINE_TO_ADR = 0x2025,   ///< Map a file & line number to an address
                                           ///< @li Request format  : #AFLMAP
                                           ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VSET / #UVSOCK_ERROR_RESPONSE
                                           ///< @li Async format    : N/A

        //---Registers:
        UV_DBG_ENUM_REGISTER_GROUPS = 0x2026,   ///< Enumerate register groups 
                                                ///< @li Request format  : #VSET - value field is used to submit stack frame pointer
                                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE

        UV_DBG_ENUM_REGISTERS = 0x2027,   ///< Evaluate registers 
                                          ///< @li Request format  : #REGENUM
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #REGENUM / #UVSOCK_ERROR_RESPONSE

        UV_DBG_READ_REGISTERS = 0x2028,   ///< Get register values
                                          ///< @li Request format  : #SSTR 
                                          ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #char[] / #UVSOCK_ERROR_RESPONSE

        UV_DBG_REGISTER_SET = 0x2029,   ///< Set register value
                                        ///< @li Request format  : #VSET - value field is used to submit stack frame pointer
                                        ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE

        UV_DBG_DSM_READ = 0x202A,   ///< Read disassembly block
                                    ///< @li Request format  : #AMEM
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #AMEM / #UVSOCK_ERROR_RESPONSE

        UV_DBG_EVAL_WATCH_EXPRESSION = 0x202B,  ///< Add watch expression / evaluate existing
                                                ///< <b>Extended stack mode only</b> 
                                                ///< @li Request format  : #VSET
                                                ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VALAMEM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_REMOVE_WATCH_EXPRESSION = 0x202D,  ///< Remove watch expression 
                                                  ///< <b>Extended stack mode only</b> 
                                                  ///< @li Request format  : #VSET
                                                  ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VARINFO / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_VARIABLES = 0x202E,///< Get variables for given stack frame, globals or struct/array members of a variable/watch
                                       ///< <b>Extended stack mode only</b> 
                                       ///< @li Request format  : #IVARENUM
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VARINFO / #UVSOCK_ERROR_RESPONSE
        UV_DBG_VARIABLE_SET = 0x202F,  ///< Set variable value
                                       ///< <b>Extended stack mode only</b> 
                                       ///< @li Request format  : #VARINFO
                                       ///< @li Response format : #UVSOCK_CMD_RESPONSE --> #VARINFO / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_TASKS = 0x2030,  ///< Enumerate task list - in non - Rtx case returns main thread
                                     ///< <b>Extended stack mode only</b> 
                                     ///< @li Request format  : #iSTKENUM
                                     ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE

        UV_DBG_ENUM_MENUS = 0x2031,   ///< Enumerate available dynaic menus and peripheral dialogs 
                                      ///< @li Request format  : #iVIEWENUM
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE

        UV_DBG_MENU_EXEC = 0x2032,    ///< Execute menu entry (for example to show view or or peripheral dialog) 
                                      ///< @li Request format  : #iVIEWENUM
                                      ///< @li Response format : #UVSOCK_CMD_RESPONSE --> no data / #UVSOCK_ERROR_RESPONSE


        //---Answers/Error from uVision to Client:
        UV_CMD_RESPONSE = 0x3000,   ///< Response to a command from the client (the UVSOCK_CMD_RESPONSE structure will contain the command code to which this is a response)
                                    ///< @li Response format : #UVSOCK_CMD_RESPONSE --> XXXX

        //---Asynchronous messages:
        UV_ASYNC_MSG = 0x4000,   ///< Asynchronous message from uVision (the UVSOCK_CMD_RESPONSE structure will contain the relevant command code)
                                 ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> XXXX

        //--- Special Asynchronous messages:
        //--- Project functions:
        UV_PRJ_BUILD_COMPLETE = 0x5000,   ///< Notification of build completion
                                          ///< @li Async format    : #PRJDATA
        UV_PRJ_BUILD_OUTPUT = 0x5001,   ///< Notification of a line of build output
                                        ///< @li Async format    : #PRJDATA

        //--- Debug functions:
        UV_DBG_CALLBACK = 0x5002,   ///< Notification of expiration of the callback timeout set by UV_DBG_SET_CALLBACK
                                    ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE

        //--- Response to UV_DBG_ENUMERATE_BP:  
        UV_DBG_BP_ENUM_START = 0x5004,   ///< Start of breakpoint enumeration (no breakpoint info)
                                         ///< @li Async format    : no data
        UV_DBG_BP_ENUMERATED = 0x5005,   ///< Breakpoint enumeration; zero, one or more Response(s) with breakpoint info
                                         ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #BKRSP / #UVSOCK_ERROR_RESPONSE
        UV_DBG_BP_ENUM_END = 0x5006,   ///< End of breakpoint enumeration (no breakpoint info)
                                       ///< @li Async format    : no data

        //--- Response to UV_PRJ_ENUM_GROUPS:
        UV_PRJ_ENUM_GROUPS_START = 0x5007,   ///< Start of group enumeration
                                             ///< @li Async format    : no data
        UV_PRJ_ENUM_GROUPS_ENU = 0x5008,   ///< Group enumeration; zero, one or more Responses with group name
                                           ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_GROUPS_END = 0x5009,   ///< End of group enumeration
                                           ///< @li Async format    : no data

        //--- Response to UV_PRJ_ENUM_FILES:
        UV_PRJ_ENUM_FILES_START = 0x500A,   ///< Start of files enumeration
                                            ///< @li Async format    : no data
        UV_PRJ_ENUM_FILES_ENU = 0x500B,   ///< File enumeration; zero, one or more Response(s) with file name
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_FILES_END = 0x500C,   ///< End of files enumeration
                                          ///< @li Async format    : no data

        //--- Progress bar functions
        UV_PRJ_PBAR_INIT = 0x500D,   ///< Notification of progress bar initialisation
                                     ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_PBAR_STOP = 0x500E,   ///< Notification of progress bar stopping
                                     ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_PBAR_SET = 0x500F,   ///< Notification of progress bar position change
                                    ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE
        UV_PRJ_PBAR_TEXT = 0x5010,   ///< Notification of progress bar text change
                                     ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE

        //--- Response to UV_DBG_ENUM_SYMTP:
        UV_DBG_ENUM_SYMTP_START = 0x5011,   ///< Start of structure member enumeration
                                            ///< @li Async format    : no data
        UV_DBG_ENUM_SYMTP_ENU = 0x5012,   ///< Structure member enumeration; zero, one or more Responses with member information
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #ENUMTPM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_SYMTP_END = 0x5013,   ///< End of structure member enumeration
                                          ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_STACK:
        UV_DBG_ENUM_STACK_START = 0x5014,   ///< Start of stack enumeration
                                            ///< @li Async format    : no data
        UV_DBG_ENUM_STACK_ENU = 0x5015,   ///< Stack enumeration; one or more Response(s) with stack frame information
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #STACKENUM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_STACK_END = 0x5016,   ///< End of stack enumeration
                                          ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_VTR:
        UV_DBG_ENUM_VTR_START = 0x5017,   ///< Start of vtr enumeration
                                          ///< @li Async format    : no data
        UV_DBG_ENUM_VTR_ENU = 0x5018,   ///< Vtr enumeration; one or more Response(s) of structure
                                        ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #AVTR / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_VTR_END = 0x5019,   ///< End of vtr enumeration
                                        ///< @li Async format    : no data

        //--- Command Window output
        UV_DBG_CMD_OUTPUT = 0x5020,   ///< Notification of a line of command window output    
                                      ///< @li Async format    : #SSTR
        //--- Serial output
        UV_DBG_SERIAL_OUTPUT = 0x5120,   ///< Notification of a serial output (debug or UART 1, 2 or 3)
                                         ///< @li Request format  : #SERIO

        //--- Response to UV_PRJ_ENUM_TARGETS:
        UV_PRJ_ENUM_TARGETS_START = 0x5021,   ///< Start of targets enumeration
                                              ///< @li Async format    : no data
        UV_PRJ_ENUM_TARGETS_ENU = 0x5022,   ///< targets enumeration; zero, one or more Responses with group name
                                            ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE
        UV_PRJ_ENUM_TARGETS_END = 0x5023,   ///< End of targets enumeration
                                            ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_REGISTER_GROUPS:
        UV_DBG_ENUM_REGISTER_GROUPS_START = 0x5024,   ///< Start of register group enumeration
                                                      ///< @li Async format    : no data
        UV_DBG_ENUM_REGISTER_GROUPS_ENU = 0x5025,   ///< register group enumeration; one or more Response(s) of structure
                                                    ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #SSTR / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_REGISTER_GROUPS_END = 0x5026,   ///< End of register group enumeration
                                                    ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_REGISTERS:
        UV_DBG_ENUM_REGISTERS_START = 0x5027,  ///< Start of register enumeration
                                               ///< @li Async format    : no data
        UV_DBG_ENUM_REGISTERS_ENU = 0x5028,  ///< register enumeration; one or more Response(s) of structure
                                             ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #REGENUM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_REGISTERS_END = 0x5029,  ///< End of register enumeration
                                             ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_VARIABLES:
        UV_DBG_ENUM_VARIABLES_START = 0x5040,   ///< Start of variable enumeration
                                                ///< @li Async format    : no data
        UV_DBG_ENUM_VARIABLES_ENU = 0x5041,   ///< variable enumeration; one or more Response(s) of structure
                                              ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #VARINFO / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_VARIABLES_END = 0x5042,   ///< End of variable enumeration
                                              ///< @li Async format    : no data

        //--- Response to UV_DBG_ENUM_TASKS:
        UV_DBG_ENUM_TASKS_START = 0x5050,   ///< Start of task list enumeration
                                            ///< @li Async format    : no data
        UV_DBG_ENUM_TASKS_ENU = 0x5051,   ///< Task list enumeration; one or more Response(s) with stack frame information
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #STACKENUM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_TASKS_END = 0x5052,   ///< End of task list  enumeration
                                          ///< @li Async format    : no data


        //--- Response to UV_DBG_ENUM_MENUS:
        UV_DBG_ENUM_MENUS_START = 0x5060,   ///< Start of available view enumeration
                                            ///< @li Async format    : no data
        UV_DBG_ENUM_MENUS_ENU = 0x5061,   ///< View list enumeration; one or more Response(s) with view item information
                                          ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #VIEWENUM / #UVSOCK_ERROR_RESPONSE
        UV_DBG_ENUM_MENUS_END = 0x5062,   ///< End of task list  enumeration
                                          ///< @li Async format    : no data


        //--- Real-Time Agent:
        UV_RTA_MESSAGE = 0x6000,   ///< Notification of a Real-Time Agent message from the target
                                   ///< @li Async format    : #PRJDATA
        UV_RTA_INCOMPATIBLE = 0x6001,   ///< Notification of an incompatible Real-Time Agent in the current target
                                        ///< @li Async format    : #UVSOCK_CMD_RESPONSE --> #UVSOCK_ERROR_RESPONSE

        //--- Test definititions (for testing only):
        UV_TST_1 = 0xFF00,
        UV_TST_2 = 0xFF01,
        UV_TST_3 = 0xFF02,
        UV_TST_4 = 0xFF03,
        UV_TST_5 = 0xFF04,
        UV_TST_6 = 0xFF05,
        UV_TST_7 = 0xFF06,
        UV_TST_8 = 0xFF07,
        UV_TST_9 = 0xFF08,
        UV_TST_10 = 0xFF09
    }

    public enum VTT_TYPE
    {
        VTT_void = 0,         ///< val.u64
        VTT_bit = 1,         ///< val.ul & 1
        VTT_char = 2,         ///< val.sc
        VTT_uchar = 3,         ///< val.uc
        VTT_int = 4,         ///< val.i
        VTT_uint = 5,         ///< val.ul
        VTT_short = 6,         ///< val.i16
        VTT_ushort = 7,         ///< val.u16
        VTT_long = 8,         ///< val.l
        VTT_ulong = 9,         ///< val.ul
        VTT_float = 10,         ///< val.f
        VTT_double = 11,         ///< val.d
        VTT_ptr = 12,         ///< val.ul
        VTT_union = 13,         ///< Unused
        VTT_struct = 14,         ///< Unused
        VTT_func = 15,         ///< Unused
        VTT_string = 16,         ///< Unused
        VTT_enum = 17,         ///< Unused
        VTT_field = 18,         ///< Unused
        VTT_int64 = 19,         ///< val.i64
        VTT_uint64 = 20,         ///< val.u64
        VTT_end                   ///< Always at end
    }

    public enum STOPREASON
    {
        STOPREASON_UNDEFINED = 0x0000,   ///< Unknown / undefined stop reason
        STOPREASON_EXEC = 0x0001,   ///< Hit execution breakpoint
        STOPREASON_READ = 0x0002,   ///< Hit read access breakpoint
        STOPREASON_HIT_WRITE = 0x0004,   ///< Hit write access breakpoint
        STOPREASON_HIT_COND = 0x0008,   ///< Hit conditional breakpoint
        STOPREASON_HIT_ESC = 0x0010,   ///< ESCape key has been pressed
        STOPREASON_HIT_VIOLA = 0x0020,   ///< Memory access violation occurred (simulator only)
        STOPREASON_TIME_OVER = 0x0040,   ///< Interval time set by #UV_DBG_SET_CALLBACK or #UV_DBG_WAKE elapsed
        STOPREASON_UNDEFINS = 0x0080,   ///< Undefined instruction occurred
        STOPREASON_PABT = 0x0100,   ///< (Instruction) prefetch abort occurred
        STOPREASON_DABT = 0x0200,   ///< Data abort occurred
        STOPREASON_NONALIGN = 0x0400,   ///< Non-aligned access occurred (simulator only)
        STOPREASON_END                    ///< Always at end
    }
}

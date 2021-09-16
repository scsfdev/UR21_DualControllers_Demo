using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static UR21_DualControllers_Demo.Model.NativeMethods;

namespace UR21_DualControllers_Demo.Model
{
    unsafe public class NativeMethods
    {
        // Data size definition
        public const int PC_SIZE = 2;
        public const int UII_SIZE = 62;
        public const int PWD_SIZE = 4;
        public const int TAGDATA_SIZE = 256;

        // Structure definition
        // UII data structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct UiiData
        {
            public uint length;                 // Effective data length of UII information stored in uii
            public fixed byte pc[PC_SIZE];      // PC information on RF tag obtained
            public fixed byte uii[UII_SIZE];    // UII information on RF tag obtained. Stored from the head, 0x00 stored beyond the length specified by length
        };


        // UII data structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct UiiDataInfo
        {
            public uint length;                 // Effective data length of UII information stored in uii
            public fixed byte pc[PC_SIZE];      // PC information on RF tag obtained
            public fixed byte uii[UII_SIZE];    // UII information on RF tag obtained. Stored from the head, 0x00 stored beyond the length specified by length
            public short rssi;                  // Stores the electric field strength value that received the read tag
            public ushort antenna;              // Stores number of antenna that received the read tag
        };


        // Tag data structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct RESULTDATA
        {
            public ushort result;                   // Result of communication with RF tag
            public fixed byte reserved[2];          // Reserved (0x00,0x00 fixed)
            public uint uiilength;                  // Effective data length of UII information stored in uii
            public uint datalength;                 // Effective data length of data from memory stored in data
            public fixed byte pc[PC_SIZE];          // PC information on RF tag obtained
            public fixed byte uii[UII_SIZE];        // UII information on RF tag obtained. Stored from the head, 0x00 stored beyond the length specified by uiilength
            public fixed byte data[TAGDATA_SIZE];   // Data from memory on read RF tag. Stored from the head, 0x00 stored beyond the length specified by datalength
        };


        // Tag data structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct RESULTDATA2
        {
            public ushort result;                   // Result of communication with RF tag
            public fixed byte reserved[2];          // Reserved (0x00,0x00 fixed)
            public uint uiilength;                  // Effective data length of UII information stored in uii
            public uint datalength;                 // Effective data length of data from memory stored in data
            public fixed byte pc[PC_SIZE];          // PC information on RF tag obtained
            public fixed byte uii[UII_SIZE];        // UII information on RF tag obtained. Stored from the head, 0x00 stored beyond the length specified by uiilength
            public fixed byte data[TAGDATA_SIZE];   // Data from memory on read RF tag. Stored from the head, 0x00 stored beyond the length specified by datalength
            public short rssi;                      // Stores the electric field strength value that received the read tag
            public ushort antenna;                  // Stores number of antenna that received the read tag
        };


        //Tag communication Read from Memory structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct READPARAM
        {
            public byte bank;                           // Bank area to be read
            public byte reserved;                       // Reserved (0x00 fixed)
            public ushort size;                         // Reading size(2-256, only even number acceptable)
            public ushort ptr;                          // Pointer to the beginning of reading (only even number acceptable)
            public fixed byte accesspwd[PWD_SIZE];      // Password for authentication of RF tag (ALL 0x00: RF tag not authenticated)
        };


        //Tag communication Write to Memory structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct WRITEPARAM
        {
            public byte bank;                           // Bank area to be read
            public byte reserved;                       // Reserved (0x00 fixed)
            public ushort size;                         // Writing size(2-64, even number only)
            public ushort ptr;                          // Pointer to the beginning of reading (only even number acceptable)
            public fixed byte accesspwd[PWD_SIZE];      // Password for authentication of RF tag (ALL 0x00: RF tag not authenticated)
            public fixed byte data[TAGDATA_SIZE];       // Data written to RF tag. Stored from the head, set 0x00 stored beyond the length specified by size
        };


        // Tag communication Lock structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct LOCKPARAM
        {
            public byte target;                         // To be locked
            public byte locktype;                       // Locked state after change
            public fixed byte accesspwd[PWD_SIZE];      // Password for authentication of RF tag (ALL 0x00: RF tag not authenticated)
        };


        // Tag communication Kill structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct KILLPARAM
        {
            public fixed byte killpwd[PWD_SIZE];        // Password for killing RF tag (ALL 0x00: RF tag cannot be killed)
        };


        // TLV parameter structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct TLVPARAM
        {
            public ushort tag;              // Parameter tag number
            public ushort length;           // Value length (4 bytes)
            public uint value;              // Parameter setting
        };


        // Device list structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe public struct DEVLIST
        {
            public uint IPaddress;          // IP address
            public ushort TcpPort;          // Connection destination Tcp port number
            public ushort DevNo;            // Terminal control number
            public uint Status;             // Status of terminal 0x00000000: not used (before opening) 0x00000001:in use (while open)
        };


        #region Controller 1 

        // Interface to UtsOpen
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsOpen")]
        internal static extern uint Uts1Open(byte Port);


        // Interface to UtsClose
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsClose")]
        internal static extern uint Uts1Close(byte Port);


        // Interface to UtsAbort
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsAbort")]
        internal static extern uint Uts1Abort(byte Port);


        // Interface to UtsReadUii
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsReadUii")]
        internal static extern uint Uts1ReadUii(byte Port);


        // Interface to UtsGetUii
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetUii")]
        unsafe internal static extern uint Uts1GetUii(byte Port, void* UIIBUF, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsGetUiiInfo
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetUiiInfo")]
        unsafe internal static extern uint Uts1GetUiiInfo(byte Port, void* UIIBUFINFO, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsStartContinuousRead
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsStartContinuousRead")]
        internal static extern uint Uts1StartContinuousRead(byte Port);


        // Interface to UtsStartContinuousReadEx
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsStartContinuousReadEx")]
        internal static extern uint Uts1StartContinuousReadEx(byte Port);


        // Interface to UtsStopContinuousRead
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsStopContinuousRead")]
        internal static extern uint Uts1StopContinuousRead(byte Port);


        // Interface to UtsGetContinuousReadResult
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetContinuousReadResult")]
        unsafe internal static extern uint Uts1GetContinuousReadResult(byte Port, void* UIIBUF, uint BufCount, out uint GetCount);


        // Interface to UtsGetContinuousReadResultInfo
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetContinuousReadResultInfo")]
        unsafe internal static extern uint Uts1GetContinuousReadResultInfo(byte Port, void* UIIBUFINFO, uint BufCount, out uint GetCount);


        // Interface to UtsStartTagComm
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsStartTagComm")]
        unsafe internal static extern uint Uts1StartTagComm(byte Port, byte TagCmd, ushort Antenna, void* Param, byte ListEnable, ushort ListNum, void* UIIBUF);


        // Interface to UtsGetTagCommResult
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetTagCommResult")]
        unsafe internal static extern uint Uts1GetTagCommResult(byte Port, void* RESULTBUF, uint BufCount, out uint GetCount, out uint RestCount);

        // Interface to UtsGetTagCommResultInfo
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetTagCommResultInfo")]
        unsafe internal static extern uint Uts1GetTagCommResultInfo(byte Port, void* RESULTBUFINFO, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsSetTagList
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsSetTagList")]
        unsafe static extern uint Uts1SetTagList(byte Port, byte Type, ushort ListNum, void* UIIBUF);


        // Interface to UtsGetVersions
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetVersions")]
        internal static extern uint Uts1GetVersions(byte Port, out byte OSVer, out byte MainVer, out byte RFVer, out byte ChipVer, out byte OEMver);


        // Interface to UtsGetProductNo
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetProductNo")]
        internal static extern uint Uts1GetProductNo(byte Port, out byte MainNo, out byte RFNo);


        // Interface to UtsGetParameter
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetParameter")]
        internal static extern uint Uts1GetParameter(byte Port, ushort Tag, out TLVPARAM TLVDATA);


        // Interface to UtsSetParameter
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsSetParameter")]
        internal static extern uint Uts1SetParameter(byte Port, TLVPARAM TLVDATA);


        // Interface to UtsLoadParameter
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsLoadParameter")]
        internal static extern uint Uts1LoadParameter(byte Port, ref byte FilePath);


        // Interface to UtsUpdateDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsUpdateDevice")]
        unsafe static extern uint Uts1UpdateDevice(byte Port, ref byte FilePath);


        // Interface to UtsInitialReset
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsInitialReset")]
        internal static extern uint Uts1InitialReset(byte Port);


        // Interface to UtsCheckAlive
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsCheckAlive")]
        internal static extern uint Uts1CheckAlive(byte Port);


        // Interface to UtsGetNetworkConfig
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetNetworkConfig")]
        internal static extern uint Uts1GetNetworkConfig(byte Port, out uint IPaddress, out uint SubnetMask, out uint Gateway);


        // Interface to UtsSetNetworkConfig
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsSetNetworkConfig")]
        internal static extern uint Uts1SetNetworkConfig(byte Port, uint IPaddress, uint SubnetMask, uint Gateway);


        // Interface to UtsCreateLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsCreateLanDevice")]
        internal static extern uint Uts1CreateLanDevice(uint IPaddress, ushort TcpPort, out ushort DevNo);

        // Interface to UtsDeleteLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsDeleteLanDevice")]
        internal static extern uint Uts1DeleteLanDevice(ushort DevNo);


        // Interface to UtsSetCurrentLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsSetCurrentLanDevice")]
        internal static extern uint Uts1SetCurrentLanDevice(ushort DevNo);


        // Interface to UtsGetCurrentLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetCurrentLanDevice")]
        internal static extern uint Uts1GetCurrentLanDevice(out ushort DevNo);


        // Interface to UtsGetLanDeviceInfo
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsGetLanDeviceInfo")]
        internal static extern uint Uts1GetLanDeviceInfo(ushort DevNo, out uint IPaddress, out ushort TcpPort, out uint Status);


        // Interface to UtsListLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsListLanDevice")]
        unsafe internal static extern uint Uts1ListLanDevice(out ushort DevCount, void* DEVICELIST);


        // Interface to UtsSetLanDevice
        [DllImport("\\Controller1\\RfidTs.dll", EntryPoint ="UtsSetLanDevice")]
        unsafe internal static extern uint Uts1SetLanDevice(ushort DevCount, void* DEVICELIST);

        #endregion


        #region Controller 2

        // Interface to UtsOpen
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsOpen")]
        internal static extern uint Uts2Open(byte Port);


        // Interface to UtsClose
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsClose")]
        internal static extern uint Uts2Close(byte Port);


        // Interface to UtsAbort
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsAbort")]
        internal static extern uint Uts2Abort(byte Port);


        // Interface to UtsReadUii
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsReadUii")]
        internal static extern uint Uts2ReadUii(byte Port);


        // Interface to UtsGetUii
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsGetUii")]
        unsafe internal static extern uint Uts2GetUii(byte Port, void* UIIBUF, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsGetUiiInfo
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsGetUiiInfo")]
        unsafe internal static extern uint Uts2GetUiiInfo(byte Port, void* UIIBUFINFO, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsStartContinuousRead
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsStartContinuousRead")]
        internal static extern uint Uts2StartContinuousRead(byte Port);


        // Interface to UtsStartContinuousReadEx
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint ="UtsStartContinuousReadEx")]
        internal static extern uint Uts2StartContinuousReadEx(byte Port);


        // Interface to UtsStopContinuousRead
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsStopContinuousRead")]
        internal static extern uint Uts2StopContinuousRead(byte Port);


        // Interface to UtsGetContinuousReadResult
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetContinuousReadResult")]
        unsafe internal static extern uint Uts2GetContinuousReadResult(byte Port, void* UIIBUF, uint BufCount, out uint GetCount);


        // Interface to UtsGetContinuousReadResultInfo
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetContinuousReadResultInfo")]
        unsafe internal static extern uint Uts2GetContinuousReadResultInfo(byte Port, void* UIIBUFINFO, uint BufCount, out uint GetCount);


        // Interface to UtsStartTagComm
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsStartTagComm")]
        unsafe internal static extern uint Uts2StartTagComm(byte Port, byte TagCmd, ushort Antenna, void* Param, byte ListEnable, ushort ListNum, void* UIIBUF);


        // Interface to UtsGetTagCommResult
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetTagCommResult")]
        unsafe internal static extern uint Uts2GetTagCommResult(byte Port, void* RESULTBUF, uint BufCount, out uint GetCount, out uint RestCount);

        // Interface to UtsGetTagCommResultInfo
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetTagCommResultInfo")]
        unsafe internal static extern uint Uts2GetTagCommResultInfo(byte Port, void* RESULTBUFINFO, uint BufCount, out uint GetCount, out uint RestCount);


        // Interface to UtsSetTagList
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsSetTagList")]
        unsafe static extern uint Uts2SetTagList(byte Port, byte Type, ushort ListNum, void* UIIBUF);


        // Interface to UtsGetVersions
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetVersions")]
        internal static extern uint Uts2GetVersions(byte Port, out byte OSVer, out byte MainVer, out byte RFVer, out byte ChipVer, out byte OEMver);


        // Interface to UtsGetProductNo
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetProductNo")]
        internal static extern uint Uts2GetProductNo(byte Port, out byte MainNo, out byte RFNo);


        // Interface to UtsGetParameter
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetParameter")]
        internal static extern uint Uts2GetParameter(byte Port, ushort Tag, out TLVPARAM TLVDATA);


        // Interface to UtsSetParameter
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsSetParameter")]
        internal static extern uint Uts2SetParameter(byte Port, TLVPARAM TLVDATA);


        // Interface to UtsLoadParameter
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsLoadParameter")]
        internal static extern uint Uts2LoadParameter(byte Port, ref byte FilePath);


        // Interface to UtsUpdateDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsUpdateDevice")]
        unsafe static extern uint Uts2UpdateDevice(byte Port, ref byte FilePath);


        // Interface to UtsInitialReset
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsInitialReset")]
        internal static extern uint Uts2InitialReset(byte Port);


        // Interface to UtsCheckAlive
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsCheckAlive")]
        internal static extern uint Uts2CheckAlive(byte Port);


        // Interface to UtsGetNetworkConfig
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetNetworkConfig")]
        internal static extern uint Uts2GetNetworkConfig(byte Port, out uint IPaddress, out uint SubnetMask, out uint Gateway);


        // Interface to UtsSetNetworkConfig
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsSetNetworkConfig")]
        internal static extern uint Uts2SetNetworkConfig(byte Port, uint IPaddress, uint SubnetMask, uint Gateway);


        // Interface to UtsCreateLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsCreateLanDevice")]
        internal static extern uint Uts2CreateLanDevice(uint IPaddress, ushort TcpPort, out ushort DevNo);

        // Interface to UtsDeleteLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsDeleteLanDevice")]
        internal static extern uint Uts2DeleteLanDevice(ushort DevNo);


        // Interface to UtsSetCurrentLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsSetCurrentLanDevice")]
        internal static extern uint Uts2SetCurrentLanDevice(ushort DevNo);


        // Interface to UtsGetCurrentLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetCurrentLanDevice")]
        internal static extern uint Uts2GetCurrentLanDevice(out ushort DevNo);


        // Interface to UtsGetLanDeviceInfo
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsGetLanDeviceInfo")]
        internal static extern uint Uts2GetLanDeviceInfo(ushort DevNo, out uint IPaddress, out ushort TcpPort, out uint Status);


        // Interface to UtsListLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsListLanDevice")]
        unsafe internal static extern uint Uts2ListLanDevice(out ushort DevCount, void* DEVICELIST);


        // Interface to UtsSetLanDevice
        [DllImport("\\Controller2\\RfidTs.dll", EntryPoint = "UtsSetLanDevice")]
        unsafe internal static extern uint Uts2SetLanDevice(ushort DevCount, void* DEVICELIST);

        #endregion


    }

    unsafe public class Ur21
    {
        public static UiiData ud = new UiiData();

        // Create custom C# event >> https://www.codeproject.com/Articles/9355/Creating-advanced-C-custom-events
        public delegate void TagHandler(object sender, TagArgs e);
        public event TagHandler OnTagRead;
        
        Thread t1;
        byte bytePort = 0;
        int controllerNo = 1;

        bool bTrue = false;

        public Ur21()
        {
            
        }


        public bool StartRead(byte bytePort, int controllerNo)
        {
            this.bytePort = bytePort;
            this.controllerNo = controllerNo;

            t1 = new Thread(ReadTag);
            t1.Start();

            return true;
        }

        internal void StopReading()
        {
            bTrue = false;
            if (t1 != null && t1.IsAlive)
                t1.Abort();
        }


        public void ReadTag()
        {
            try
            {
                // int iCounter = 0;
                bTrue = true;
                uint iReturn;
                uint iReadCount;
                uint iRemainCount;
                uint iBufCount = 500;
                int i = 0;

                // More info on Marshal class >> https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal?view=netframework-4.5.1
                // More info on IntPtr >> https://docs.microsoft.com/en-us/dotnet/api/system.intptr?view=netframework-4.5.1

                // More info on what is void* in C# >> https://stackoverflow.com/questions/15527985/what-is-void-in-c
                IntPtr uiiBuf = Marshal.AllocHGlobal(sizeof(UiiData) * (int)iBufCount);

                iReturn = controllerNo == 1 ? Uts1Open(bytePort) : Uts2Open(bytePort);
                if (iReturn != 0)
                    throw new Exception("Open port error-" + iReturn.ToString("X2"));


                iReturn = controllerNo == 1 ? Uts1Abort(bytePort) : Uts2Abort(bytePort);
                if (iReturn != 0)
                    throw new Exception("Abort port error-" + iReturn.ToString("X2"));


                while (bTrue)
                {
                    iReturn = controllerNo == 1 ? Uts1ReadUii(bytePort) : Uts2ReadUii(bytePort);
                    if (iReturn != 0)        // There is an error, but keep going.
                        continue;

                    do
                    {
                        if(controllerNo == 1)
                            iReturn = Uts1GetUii(bytePort, (void*)uiiBuf, iBufCount, out iReadCount, out iRemainCount);
                        else
                            iReturn = Uts2GetUii(bytePort, (void*)uiiBuf, iBufCount, out iReadCount, out iRemainCount);

                        if (iReturn == 1)
                        {
                            iRemainCount = 1;
                            continue;
                        }

                        for (i = 0; i < iReadCount; i++)
                        {
                            TagArgs e = new TagArgs();

                            // IntPtr to Structure >> https://stackoverflow.com/a/27680642/770989
                            ud = (UiiData)Marshal.PtrToStructure(uiiBuf + (sizeof(UiiData) * i), typeof(UiiData));
                            //                            ud = (UiiData)Marshal.PtrToStructure((IntPtr)((uint)uiiBuf + (sizeof(UiiData) * i)), typeof(UiiData));

                            // More info on fixed >> https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/fixed-statement
                            // Reference from here >> https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs1666
                            // If we do not use fixed, it will throw CS1666 error.
                            fixed (UiiData* uf = &ud)
                            {
                                byte[] bUii = new byte[uf->length];


                                // How to get IntPtr from byte[] >> https://stackoverflow.com/questions/537573/how-to-get-intptr-from-byte-in-c-sharp
                                // Another example >> https://stackoverflow.com/a/27680642/770989
                                Marshal.Copy((IntPtr)uf->uii, bUii, 0, (int)uf->length);

                                e.Uii = BitConverter.ToString(bUii).Replace("-", "");
                            }

                            //if (!OnTagRead.Equals(null))
                            OnTagRead(this, e);
                            
                        }
                    }
                    while ((iRemainCount > 0) && bTrue);
                }

                iReturn = controllerNo == 1 ? Uts1Close(bytePort) : Uts2Close(bytePort);
                if (iReturn > 0)
                    throw new Exception("Close port error-" + iReturn.ToString("X2"));
            }
            catch (ThreadAbortException)
            {
                if(controllerNo == 1)
                    Uts1Close(bytePort);
                else
                    Uts2Close(bytePort);
            }
            catch (Exception e)
            {
                ErrMsg errMsg = new ErrMsg();
                errMsg.StatusMsg = "Error: An error occured while running UR21 Api.";
                errMsg.BoxMsg = "An error occured while running UR2x Api!" + Environment.NewLine + "Details: " + e.Message;
                Messenger.Default.Send(errMsg, MsgType.MAIN_VM);
            }
        }

    }
}

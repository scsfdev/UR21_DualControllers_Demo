using System;
using GalaSoft.MvvmLight;
using UR21_DualControllers_Demo.Model;
using GalaSoft.MvvmLight.Messaging;
using System.IO;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace UR21_DualControllers_Demo.ViewModel
{

    // TODO: 3. Need to check do I need to use 3 different Thread instead of just T1 based on Controller Number when more than one controller is used at the same time.
    // TODO: 4. Need to check will it affect starting, reading, displaying data, stopping of UR21 when more than one controller is used at the same time. 



    // Finished.
    // TODO: 1. Need to add functions to export scan data to CSV.
    //          Export - Finish.
    // TODO: 2. Need to add functions for Controller 1 + 2 --> Start, Stop, Clear.
    //          Start / Stop / Clear - Finish.
    // TODO: 5. Need to update Power and Antenna value to INI file for both Controller 1 and Controller 2.
    //          Now, it is using current defined power and antenan value - Finish.
    // TODO: 6. Need to check power value is being utilize when start reading. --> SaveUr21Setting
    //          Save ini file - Finish.

    // Current action
    // TODO: 7. Need to implements ReadUii, Continuous Read and Read Memory.
    //          ReadUii - Finish.
    //          ContinuousRead - Finish.
    //          ReadFromMemory - Ongoing.



    public class MainViewModel : ViewModelBase
    {
        DispatcherTimer msgTimer = new DispatcherTimer();

        List<ParcelSetting> parcels = new List<ParcelSetting>();

        private string statusMsg;
        public string StatusMsg
        {
            get { return statusMsg; }
            set { Set(ref statusMsg, value); }
        }


        private bool _c1Ready;
        public bool C1Ready
        {
            get { return _c1Ready; }
            set { Set(ref _c1Ready, value); }
        }

        private bool _c2Ready;
        public bool C2Ready
        {
            get { return _c2Ready; }
            set { Set(ref _c2Ready, value); }
        }

        private bool _c1n2Ready;
        public bool C1n2Ready
        {
            get { return _c1n2Ready; }
            set { Set(ref _c1n2Ready, value); }
        }

        private bool _c1Started;
        public bool C1Started
        {
            get { return _c1Started; }
            set { Set(ref _c1Started, value); }
        }

        private bool _c2Started;
        public bool C2Started
        {
            get { return _c2Started; }
            set { Set(ref _c2Started, value); }
        }

        private bool _c1n2Started;
        public bool C1n2Started
        {
            get { return _c1n2Started; }
            set { Set(ref _c1n2Started, value); }
        }

        private bool _c1ExportReady;
        public bool C1ExportReady
        {
            get { return _c1ExportReady; }
            set { Set(ref _c1ExportReady, value); }
        }


        private bool _c2ExportReady;
        public bool C2ExportReady
        {
            get { return _c2ExportReady; }
            set { Set(ref _c2ExportReady, value); }
        }


        private bool _c1n2ExportReady;
        public bool C1n2ExportReady
        {
            get { return _c1n2ExportReady; }
            set { Set(ref _c1n2ExportReady, value); }
        }


        private bool _readUii;
        public bool ReadUii
        {
            get { return _readUii; }
            set { Set(ref _readUii, value); }
        }


        private bool _readContinuous;
        public bool ReadContinuous
        {
            get { return _readContinuous; }
            set { Set(ref _readContinuous, value); }
        }


        private bool _readMemory;
        public bool ReadMemory
        {
            get { return _readMemory; }
            set { Set(ref _readMemory, value); }
        }



        public ICommand CmdRfidAction { get; private set; }

        public MainViewModel(IDataService dataService)
        {
            Messenger.Default.Register<string>(this, MsgType.MAIN_VM, ShowStatusMsg);
            Messenger.Default.Register<string>(this, MsgType.CON_STATUS, ControllerError);
            Messenger.Default.Register<ParcelSetting>(this, MsgType.MAIN_VM, SaveUr21Setting);
            Messenger.Default.Register<ErrMsg>(this, MsgType.MAIN_VM, ErrorMsgHandling);

            Messenger.Default.Register<string>(this, MsgType.MAIN_EXPORT, ExportReady);

            msgTimer.Interval = new TimeSpan(0, 0, 5);
            msgTimer.Tick += MsgTimer_Tick;

            LoadSetting1 = new SettingViewModel(1);
            LoadSetting2 = new SettingViewModel(2);
            // Load Setting.
            if (!LoadUr21Setting())
                Messenger.Default.Send(new NotificationMessage(this, MyConst.EXIT));

            CmdRfidAction = new RelayCommand<object>(RfidAction);

            ReadUii = true;
        }

        private void ExportReady(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            string[] infos = data.Split(',');
            if(infos[0] == "1")
            {
                if (infos[1] == "T")
                    C1ExportReady = true;
                else
                    C1ExportReady = false;
            }
            else if (infos[0] == "2")
            {
                if (infos[1] == "T")
                    C2ExportReady = true;
                else
                    C2ExportReady = false;
            }
            else
            {
                if (infos[1] == "T")
                    C1n2ExportReady = true;
                else
                    C1n2ExportReady = false;
            }
        }

        private void ErrorMsgHandling(ErrMsg errMsg)
        {
            StatusMsg = errMsg.StatusMsg;
            Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + errMsg.BoxMsg, MsgType.MAIN_V);
        }

        private void RfidAction(object cmdObj)
        {
            if (cmdObj == null)
            {
                return;
            }

            string cmdStr = cmdObj.ToString().Trim();

            switch (cmdStr)
            {
                case "C1START": StartUR21(1); break;
                case "C1STOP": StopUR21(1); break;
                case "C1CLEAR": ClearScanData(1); break;
                case "C1SAVE": ExportScanData(1); break;

                case "C2START": StartUR21(2); break;
                case "C2STOP": StopUR21(2); break;
                case "C2CLEAR": ClearScanData(2); break;
                case "C2SAVE": ExportScanData(2); break;

                case "C1n2START": StartUR21(3); break;
                case "C1n2STOP": StopUR21(3); break;
                case "C1n2CLEAR": ClearScanData(3); break;
                case "C1n2SAVE": ExportScanData(3); break;
            }
        }

        private void StartUR21(int controllerNo)
        {
            if (controllerNo == 1)
            {
                parcels[0].Start = true;
                parcels[0].ReadUii = _readUii;
                parcels[0].ReadContinuous = _readContinuous;
                parcels[0].ReadMemory = _readMemory;

                C1Ready = false;
                C1Started = true;
                Messenger.Default.Send(parcels[0], MsgType.CONTROLLER_VM + controllerNo.ToString());
                Messenger.Default.Send(false, MsgType.SETTING_VM + controllerNo.ToString());
            }
            else if (controllerNo == 2)
            {
                parcels[1].Start = true;
                parcels[0].ReadUii = _readUii;
                parcels[0].ReadContinuous = _readContinuous;
                parcels[0].ReadMemory = _readMemory;

                C2Ready = false;
                C2Started = true;
                
                Messenger.Default.Send(parcels[1], MsgType.CONTROLLER_VM + controllerNo.ToString());
                Messenger.Default.Send(false, MsgType.SETTING_VM + controllerNo.ToString());
            }
            // TODO: Need to add for 1 + 2 start reading.
        }

        private void StopUR21(int controllerNo)
        {
            if (controllerNo == 1)
            {
                C1Ready = true;
                C1Started = false;
                parcels[0].Start = false;
                Messenger.Default.Send(true, MsgType.SETTING_VM + controllerNo.ToString());
                Messenger.Default.Send(parcels[0], MsgType.CONTROLLER_VM + controllerNo.ToString());
            }
            else if (controllerNo == 2)
            {
                C2Ready = true;
                C2Started = false;
                parcels[1].Start = false;
                Messenger.Default.Send(true, MsgType.SETTING_VM + controllerNo.ToString());
                Messenger.Default.Send(parcels[1], MsgType.CONTROLLER_VM + controllerNo.ToString());
            }
            // TODO: Need to add for 1 + 2 stop reading.
        }

        private void ClearScanData(int controllerNo)
        {
            if (controllerNo == 1)
            {
                parcels[0].Start = true;
                C1Ready = true;
                C1Started = false;
                Messenger.Default.Send(true, MsgType.CONTROLLER_VM + controllerNo.ToString());
                ExportReady("1,F");
            }
            else if (controllerNo == 2)
            {
                parcels[1].Start = true;
                C2Ready = true;
                C2Started = false;
                Messenger.Default.Send(true, MsgType.CONTROLLER_VM + controllerNo.ToString());
                ExportReady("2,F");
            }
            else
            {
                // TODO: Need to add for 1 + 2 clearn reading.
                C1n2Ready = true;
                C1n2Started = false;
                ExportReady("1n2,F");
            }
            
        }

        private void ExportScanData(int controllerNo)
        {
            Messenger.Default.Send(controllerNo, MsgType.CONTROLLER_VM + controllerNo.ToString());
        }



        private void ControllerError(string status)
        {
            string[] data = status.Split(',');
            if (data[0].ToUpper() == "1")
            {
                // Controller 1.
                if (data[1].ToUpper() == "FALSE")
                {
                    // Error.
                    Controller1Status = false;
                }
                else
                    Controller1Status = true;
            }
            else
            {
                // Controller 2.
                if (data[1].ToUpper() == "FALSE")
                {
                    // Error
                    Controller2Status = false;
                }
                else
                    Controller2Status = true;
            }
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            StatusMsg = "";
            msgTimer.Stop();
        }

        private void ShowStatusMsg(string msg)
        {
            msgTimer.Start();
            StatusMsg = msg;
        }


        private void SaveUr21Setting(ParcelSetting parcel)
        {
            string xmlFile = "Setting.xml";
            try
            {
                bool result = new MyHelper().UpdateSettingFile(xmlFile, parcel);
                if (result)
                {
                    result = new MyHelper().UpdateIniFile(@"Controller" + parcel.No.ToString() + "\\RfidTsParam.ini", parcel);
                    // TODO: Save Ini file.
                    //foreach (ParcelSetting p in parcels)
                    //{
                    //    // Read controller Ini file.
                    //    if (!LoadIniFile(@"Controller" + p.No.ToString() + "\\RfidTsParam.ini", p))
                    //        return false;
                    //}

                    LoadUr21Setting(true);

                    if (parcel.No == 1)
                    {
                        Controller1Status = true;
                        C1Ready = true;
                    }
                    else if (parcel.No == 2)
                    {
                        Controller2Status = true;
                        C2Ready = true;
                    }

                    ShowStatusMsg("Info: Successfully updated Controller " + parcel.No.ToString() + " parameters.");
                }
            }
            catch (Exception e)
            {
                StatusMsg = "Error: Update Setting file failed.";
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + "Update Setting file failed!" + Environment.NewLine + "Details: " + e.Message, MsgType.MAIN_V);
            }
        }

       
        private bool LoadUr21Setting(bool reLoad = false)
        {
            parcels.Clear();
            string xmlFile = "Setting.xml";
            if(!File.Exists(xmlFile))
            {
                StatusMsg = "Error: Setting.xml file is missing.";
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + "Setting.xml file is missing!", MsgType.MAIN_V);
                return false;
            }

            // Load Setting.xml file.
            try
            {
                List<XmlData> xmlDatas = new MyHelper().LoadSettingFile(xmlFile);
                foreach(XmlData x in xmlDatas)
                {
                    ParcelSetting p = new ParcelSetting();
                    p.No = MyConverter.ToInt32(x.No);
                    p.Com = MyConverter.ToInt32(x.Com);
                    parcels.Add(p);
                }
            }
            catch(Exception e)
            {
                StatusMsg = "Error: Load Setting file failed.";
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + "Load Setting file failed!" + Environment.NewLine + "Details: " + e.Message, MsgType.MAIN_V);
                return false;
            }

            try
            {
                // Load Ini file.
                foreach (ParcelSetting p in parcels)
                {
                    // Read controller Ini file.
                    if (!LoadIniFile(@"Controller" + p.No.ToString() + "\\RfidTsParam.ini", p))
                        return false;
                }
            }
            catch (Exception e)
            {
                StatusMsg = "Error: Load UR21 Ini file failed.";
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + "Load UR21 Ini file failed!" + Environment.NewLine + "Details: " + e.Message, MsgType.MAIN_V);
                return false;
            }

            if (!reLoad)
            {
                // Loop through each Parcel and notify the Setting.
                foreach (ParcelSetting p in parcels)
                {
                    Messenger.Default.Send(p, MsgType.SETTING_VM + p.No.ToString());
                    Messenger.Default.Send(true, MsgType.SETTING_VM + p.No.ToString());
                }
            }

            return true;
        }

        private SettingViewModel _loadSetting1;
        public SettingViewModel LoadSetting1
        {
            get { return _loadSetting1; }
            set { Set(ref _loadSetting1, value); }
        }

        private SettingViewModel _loadSetting2;
        public SettingViewModel LoadSetting2
        {
            get { return _loadSetting2; }
            set { Set(ref _loadSetting2, value); }
        }


        private bool LoadIniFile(string iniFile, ParcelSetting p)
        {
            if (!File.Exists(iniFile))
            {
                StatusMsg = "Error: Controller setting file is missing.";
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine + "Controller setting file is missing!", MsgType.MAIN_V);
                return false;
            }

            try
            {
                string[] fileContents = File.ReadAllLines(iniFile);
                foreach (string line in fileContents)
                {
                    if (line.ToUpper().Contains("CARRIER_POWER_DBM"))
                        p.Power = MyConverter.ToInt32(line.ToUpper().Replace("CARRIER_POWER_DBM", "").Replace("=", "").Trim());

                    if (line.ToUpper().Contains("ANTENNA_PORT"))
                    {
                        string antenna = line.ToUpper().Replace("ANTENNA_PORT", "").Replace("=", "").Trim();
                        antenna = antenna.Substring(antenna.Length - 1, 1);
                        p.Antenna = MyConverter.ToInt32(antenna);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }            

            return true;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}



        private bool _controller1Status;
        public bool Controller1Status
        {
            get { return _controller1Status; }
            set { Set(ref _controller1Status, value); }
        }


        private bool _controller2Status;
        public bool Controller2Status
        {
            get { return _controller2Status; }
            set { Set(ref _controller2Status, value); }
        }
    }
}
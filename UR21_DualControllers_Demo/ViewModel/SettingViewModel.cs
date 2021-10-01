using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using UR21_DualControllers_Demo.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;

namespace UR21_DualControllers_Demo.ViewModel
{
    
    public class SettingViewModel : ViewModelBase, IDataErrorInfo
    {
        public ICommand CmdUpdate { get; private set; }

        public SettingViewModel(int controllerNo)
        {
            Init_View();
            Messenger.Default.Register<ParcelSetting>(this, MsgType.SETTING_VM + controllerNo.ToString(), LoadSetting);
            Messenger.Default.Register<bool>(this, MsgType.SETTING_VM + controllerNo.ToString(), RfidinAction);


            ControllerNo = "Controller " + controllerNo + " Setting";

            CmdUpdate = new RelayCommand(UpdateSetting);
        }

        private void RfidinAction(bool isReady)
        {
            UpdateReady = isReady;
        }

        private string GetComPort()
        {
            try
            {
                List<int> ports = new List<int>();

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort"))
                {
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["Caption"] != null && queryObj["Caption"].ToString().Trim().ToUpper().Contains("DENSO WAVE"))
                        {
                            if (!queryObj["Caption"].ToString().Trim().ToUpper().Contains("DISCONNECTED") && queryObj["DeviceID"] != null)
                            {
                                ports.Add(MyConverter.ToInt32(queryObj["DeviceID"].ToString().ToUpper().Replace("COM", "")));
                            }
                        }
                    }
                }


                if(ports.Count <=0)
                    Messenger.Default.Send("Error: Fail to auto retrieve COM port for " + ControllerNo, MsgType.MAIN_VM);

                return string.Join(",", ports.ToArray());
            }
            catch (Exception e)
            {
                Messenger.Default.Send(ControllerNo.ToUpper().Replace("CONTROLLER ", "") + ",FALSE", MsgType.CON_STATUS);
                Messenger.Default.Send("Error: Fail to auto retrieve COM port!", MsgType.MAIN_VM);
                Messenger.Default.Send(MyConst.ERROR + Environment.NewLine +
                                        Environment.NewLine + "Fail to auto retrieve COM port!", MsgType.MAIN_V);
                return "-";
            }

        }


        private bool _updateReady;

        public bool UpdateReady
        {
            get { return _updateReady; }
            set { Set(ref _updateReady, value); }
        }



        private void UpdateSetting()
        {
            List<string> lstErr = new List<string>();

            if (_controllerComPort < 0 || _controllerComPort > 50)
                lstErr.Add("Invalid COM Port.");

            if (!GetComPort().Contains(_controllerComPort.ToString()))
                lstErr.Add("No devie detected at given COM port.");

            if (_controllerPower < 50 || _controllerPower > 230)
                lstErr.Add("Invalid Power (50 to 230 only).");

            if (_ant1Checked == false && _ant2Checked == false && _ant1n2Checked == false)
                lstErr.Add("Please select the Antenna");

            if (lstErr.Count > 0)        // error.
            {
                string strErrMsg = MyConst.ERROR + Environment.NewLine +
                                    Environment.NewLine + 
                                    "Please check for " + _controllerNo + " setting." +
                                    Environment.NewLine + Environment.NewLine +
                                    string.Join(Environment.NewLine, lstErr.ToArray());

                Messenger.Default.Send(ControllerNo.ToUpper().Replace("CONTROLLER ", "") + ",FALSE", MsgType.CON_STATUS);
                Messenger.Default.Send("Error: Updating " + _controllerNo + " setting fail!", MsgType.MAIN_VM);
                Messenger.Default.Send(strErrMsg, MsgType.MAIN_V);
                
                return;
            }

            ParcelSetting parcel = new ParcelSetting();
            parcel.No = MyConverter.ToInt32(_controllerNo.ToUpper().Replace("CONTROLLER ", ""));
            parcel.Com = _controllerComPort;
            parcel.Power = _controllerPower;
            if (_ant1Checked)
                parcel.Antenna = 1;
            else if (_ant2Checked)
                parcel.Antenna = 2;
            else
                parcel.Antenna = 3;

            Messenger.Default.Send(parcel, MsgType.MAIN_VM);
        }

        private void LoadSetting(ParcelSetting parcel)
        {
            ControllerNo = "Controller " + parcel.No;
            ControllerComPort = parcel.Com;
            ControllerPower = parcel.Power;
            if (parcel.Antenna == 1)
                Ant1Checked = true;
            else if (parcel.Antenna == 2)
                Ant2Checked = true;
            else
                Ant1n2Checked = true;

            if (GetComPort().Trim() == "")
                Messenger.Default.Send(ControllerNo.ToUpper().Replace("CONTROLLER ", "") + ",FALSE", MsgType.CON_STATUS);
        }

        public SettingViewModel()
        {
            Init_View();
            ControllerNo = "Controller 0";
        }

        private void Init_View()
        {
            ControllerComPort = 1;
            ControllerPower = 50;
            Ant1Checked = true;
            Ant2Checked = false;
            Ant1n2Checked = false;
        }

        private string _controllerNo;
        public string ControllerNo
        {
            get { return _controllerNo; }
            set { Set(ref _controllerNo, value); }
        }

        private int _controllerComPort;
        public int ControllerComPort
        {
            get { return _controllerComPort; }
            set { Set(ref _controllerComPort, value); }
        }

        private int _controllerPower;
        public int ControllerPower
        {
            get { return _controllerPower; }
            set { Set(ref _controllerPower, value); }
        }


        private bool _ant1Checked;
        public bool Ant1Checked
        {
            get { return _ant1Checked; }
            set { Set(ref _ant1Checked, value); }
        }

        private bool _ant2Checked;
        public bool Ant2Checked
        {
            get { return _ant2Checked; }
            set { Set(ref _ant2Checked, value); }
        }

        private bool _ant1n2Checked;
        public bool Ant1n2Checked
        {
            get { return _ant1n2Checked; }
            set { Set(ref _ant1n2Checked, value); }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ControllerPower":
                        if (_controllerPower < 50 || _controllerPower > 230)
                            return "Power must be between 50 and 230!!";
                        break;
                }

                return string.Empty;
            }
        }

        //public string Error
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        //public string this[string columnName]
        //{
        //    get
        //    {
        //        switch (columnName)
        //        {
        //            case "ControllerPower":
        //                if (_controllerPower < 50 || _controllerPower > 230)
        //                    return "Power must be between 50 and 230";
        //                break;
        //        }

        //        return string.Empty;
        //    }
        //}
    }
}
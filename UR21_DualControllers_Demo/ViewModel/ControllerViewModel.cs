using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using UR21_DualControllers_Demo.Model;

namespace UR21_DualControllers_Demo.ViewModel
{
    public class ControllerViewModel : ViewModelBase
    {
        int controllerNo = 0;
        bool allowDuplicate = false;

        Ur21 ur;

        /// <summary>
        /// Initializes a new instance of the ControllerViewModel class.
        /// </summary>
        public ControllerViewModel(int controllerNo)
        {
            Messenger.Default.Register<ParcelSetting>(this, MsgType.CONTROLLER_VM + controllerNo.ToString(), RfidAction);
            Messenger.Default.Register<bool>(this, MsgType.CONTROLLER_VM + controllerNo.ToString(), ClearList);
            

            ur = new Ur21();
            ur.OnTagRead += Ur_OnTagRead;

            ControllerNo = "Controller " + controllerNo + " scan data";
            this.controllerNo = controllerNo;
        }

      

        private void ExportList(int controllerNo)
        {
            string parentDir = @"C:\Temp\UR21Dual_Demo";
            if (!Directory.Exists(parentDir))
                Directory.CreateDirectory(parentDir);

            string todayDir = Path.Combine(parentDir, DateTime.Now.ToString("yyMMdd"));
            if (!Directory.Exists(todayDir))
                Directory.CreateDirectory(todayDir);

            string fileName = Path.Combine(todayDir, "C" + controllerNo + "_" + DateTime.Now.ToString("HHmmss") + ".csv");

            try
            {
                new MyHelper().ExportToCSV(fileName, tagList.ToList());
                // TODO: Send success status message only. No need to send message box. This is to avoid msgbox pop up while Controller 1 or 2 is still running at the same time.
                Messenger.Default.Send("Info: Successfully exported out Controller " + controllerNo + " data to " + todayDir, MsgType.MAIN_VM);
            }
            catch (Exception e)
            {
                ErrMsg errMsg = new ErrMsg();
                errMsg.StatusMsg = "Error: Controller " + controllerNo + " failed to export data.";
                errMsg.BoxMsg = "Controller: " + controllerNo + " failed to export data." + Environment.NewLine + "Details: " + e.Message;
                Messenger.Default.Send(errMsg, MsgType.MAIN_VM);
            }            
        }

        private void ClearList(bool clearList)
        {
            TagList = new ObservableCollection<Tag>(); 
        }

        private void Ur_OnTagRead(object sender, TagArgs e)
        {
            if (e != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    int iCount = 0;
                    int iQty = 1;

                    if (TagList == null)
                        TagList = new ObservableCollection<Tag>();
                    else
                        iCount = TagList.Count;

                    bool bExist = false;

                    if (TagList.Count > 0)
                    {
                        List<Tag> lst = TagList.ToList();

                        foreach (Tag ta in lst)
                        {
                            if (ta.Uii == e.Uii)
                            {
                                bExist = true;

                                if (allowDuplicate)
                                {
                                    // Need to increase Qty.
                                    ta.Qty += 1;
                                    ta.ReadDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    ta.ReadTime = DateTime.Now.ToString("hh:mm:ss tt");
                                }
                            }
                        }

                        if (bExist)
                        {
                            TagList.Clear();
                            TagList = new ObservableCollection<Tag>(lst);
                        }
                    }

                    if (!bExist)
                    {
                        iCount++;

                        Tag t = new Tag();
                        t.Uii = e.Uii;
                        t.Memory = e.Memory;
                        t.No = iCount;
                        t.Qty = iQty;
                        t.ReadDate = DateTime.Now.ToString("yyyy-MM-dd");
                        t.ReadTime = DateTime.Now.ToString("hh:mm:ss tt");

                        TagList.Add(t);
                    }
                });
            }
        }


        private string _controllerNo;
        public string ControllerNo
        {
            get { return _controllerNo; }
            set { Set(ref _controllerNo, value); }
        }




        private ObservableCollection<Tag> tagList;
        public ObservableCollection<Tag> TagList
        {
            get { return tagList; }
            set { Set(ref tagList, value); }
        }

        public void RfidAction(ParcelSetting parcel)
        {
            try
            {
                if (parcel.Start)
                {
                    TagList = new ObservableCollection<Tag>();

                    if (parcel.ReadUii)
                        ur.StartReadUii(byte.Parse(parcel.Com.ToString()), parcel.No);
                    else if (parcel.ReadContinuous)
                        ur.StartReadContinuous(byte.Parse(parcel.Com.ToString()), parcel.No);
                    else
                        ur.StartReadMemory(byte.Parse(parcel.Com.ToString()), parcel.No);
                }
                else
                {
                    ur.StopReading(parcel.No);
                    if (tagList != null && tagList.Count > 0)
                        Messenger.Default.Send(parcel.No + ",T", MsgType.MAIN_EXPORT);
                    else
                        Messenger.Default.Send(parcel.No + ",F", MsgType.MAIN_EXPORT);
                }                    
            }
            catch (Exception e)
            {
                ErrMsg errMsg = new ErrMsg();
                errMsg.StatusMsg = "Error: Controller " + parcel.No.ToString() + " failed to " + (parcel.Start ? "start" : "stop") + " reading.";
                errMsg.BoxMsg = "Controller: " + parcel.No.ToString() + " failed to " + (parcel.Start ? "start" : "stop") + " reading." + Environment.NewLine + "Details: " + e.Message;
                Messenger.Default.Send(errMsg, MsgType.MAIN_VM);
            }
        }

    }
}
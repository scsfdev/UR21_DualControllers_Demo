using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                    ur.StartRead(byte.Parse(parcel.Com.ToString()), parcel.No);
                }
                else
                {
                    ur.StopReading();
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
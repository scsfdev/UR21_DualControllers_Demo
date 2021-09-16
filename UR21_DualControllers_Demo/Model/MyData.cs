using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UR21_DualControllers_Demo.Model
{
    internal static class MyConst
    {
        internal const string ERROR = "<< ERROR >>";
        internal const string WARNING = "<< WARNING >>";
        internal const string INFO = "<< INFO >>";

        internal const string OK = "OK";
        internal const string CANCEL = "CANCEL";

        internal const string EXIT = "EXIT";

        internal const string START = "START";
        internal const string STOP = "STOP";

        internal const string TITLE = "UR21 DUAL CONTROLLER DEMO";
    }

    public enum MsgType
    {
        MAIN_V,
        SETTING_VM,
        SETTING_VM1,
        SETTING_VM2,
        CONTROLLER_VM,
        CONTROLLER_VM1,
        CONTROLLER_VM2,
        MAIN_V_CONFIRM,
        MAIN_VM,
        CON_STATUS,
        TERMINATOR,
        MAIN_EXPORT
    }

    public enum ErrCode
    {
        Err_Null = 0
    }

    internal static class General
    {
        internal static string ReplyMsg(ErrCode errCode, string strMainmsg, bool? bErr = null)
        {
            string strTitle = "";
            string strErrCode = "";

            if (bErr == null)
                strTitle = MyConst.INFO;
            else if (bErr == true)
                strTitle = MyConst.ERROR;
            else
                strTitle = MyConst.WARNING;

            strErrCode = ((int)errCode).ToString();

            if (strErrCode == "0")
                return strTitle + Environment.NewLine + Environment.NewLine + strMainmsg;
            else if (bErr == true)
                return strTitle + Environment.NewLine + Environment.NewLine +
                       "Error Code: " + strErrCode + Environment.NewLine +
                       "Message: " + strMainmsg;
            else
                return strTitle + Environment.NewLine + Environment.NewLine +
                       "Message: " + strMainmsg;
        }

        internal static string GetVersion()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
            return asm.GetName().Version.Major.ToString() + "." + asm.GetName().Version.Minor.ToString() + "." + asm.GetName().Version.Revision.ToString();
        }

        internal static string GetExeLocation()
        {
            string strCompany = ((System.Reflection.AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                                System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyCompanyAttribute), false)).Company;
            string strPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), strCompany);
            strPath = Path.Combine(strPath, System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

            return strPath;
        }


        internal static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        internal static string HexToString(string strHex)
        {
            string strData = "";
            while (strHex.Length > 0)
            {
                strData += Convert.ToChar(Convert.ToInt32(strHex.Substring(0, 2), 16)).ToString();
                strHex = strHex.Substring(2, strHex.Length - 2);
            }
            return strData;

        }
    }

    public class XmlData
    {
        private string _no;
        public string No
        {
            get { return _no; }
            set { _no = value; }
        }


        private string _com;
        public string Com
        {
            get { return _com; }
            set { _com = value; }
        }
    }

    public class ErrMsg
    {
        public string StatusMsg { get; set; }
        public string BoxMsg { get; set; }
    }

    public class ParcelSetting
    {
        public int No { get; set; }
        public int Com { get; set; }

        public int Power { get; set; }

        public int Antenna { get; set; }

        public bool Start { get; set; }
    }


    public class Tag
    {
        public string Uii { get; set; }
        public int No { get; set; }
        public int Qty { get; set; }

        public string ReadDate { get; set; }
        public string ReadTime { get; set; }

        public Tag() { }

        public Tag(Tag t)
        {
            Uii = t.Uii;
            No = t.No;
            Qty = t.Qty;
            ReadDate = t.ReadDate;
            ReadTime = t.ReadTime;
        }
    }

    public class TagArgs : EventArgs
    {
        public TagArgs()
        {
            Uii = null;
            Qty = 0;
        }

        public int qty;
        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }


        private string uii;
        public string Uii
        {
            get { return uii; }
            set { uii = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Management;
using UR21_DualControllers_Demo.Model;
using System.Globalization;

namespace UR21_DualControllers_Demo.Design
{
    public class PowerValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
            {
                if (i < 50 || i > 230)
                    return new ValidationResult(false, "Power must be between 50 and 230.");
            }
            else
                return new ValidationResult(false, "Invalid Power value.");

            return ValidationResult.ValidResult;
        }
    }


    public class ComPortValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
            {
                if(!GetComPort().Contains(i.ToString()))
                    return new ValidationResult(false, "Invalid COM Port.");
            }
            else
                return new ValidationResult(false, "Invalid COM Port.");

            return ValidationResult.ValidResult;
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

                return string.Join(",", ports.ToArray());
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }

    class Obj2Enable : IValueConverter
    {
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value == null ? "0" : value.ToString().Trim();

            bool bReturn = false;

            switch (strValue)
            {
                case "":
                case "0":
                    bReturn = false;
                    break;
                default:
                    bReturn = true;
                    break;
            }

            if (Reverse)
                return !bReturn;
            else
                return bReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "1";
                else
                    return "0";
            }
            return "0";
        }
    }

    public class IsEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if ((value is bool) && !(bool)value)
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


}

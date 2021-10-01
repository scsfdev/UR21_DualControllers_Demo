using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UR21_DualControllers_Demo.Model
{
    class MyHelper
    {
        public List<XmlData> LoadSettingFile(string xmlFile)
        {
            try
            {
                XDocument xD = XDocument.Load(xmlFile);

                List<XmlData> myDelis = (from s in xD.Descendants("controller")
                                        select new XmlData
                                        {
                                            No = s.Element("no").Value,
                                            Com = s.Element("com").Value
                                        }).ToList();

                return myDelis;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool UpdateIniFile(string iniFile, ParcelSetting p)
        {
            if (!File.Exists(iniFile))
            {
                throw new Exception("Controller ini setting file is missing");
            }

            try
            {
                string[] fileContents = File.ReadAllLines(iniFile);
                for (int i = 0; i < fileContents.Length; i++)
                {
                    if (fileContents[i].ToUpper().Contains("CARRIER_POWER_DBM"))
                        fileContents[i] = fileContents[i].Replace(fileContents[i].Substring(fileContents[i].IndexOf("=") + 1), p.Power.ToString());
                    else if (fileContents[i].ToUpper().Contains("ANTENNA_PORT"))
                        fileContents[i] = fileContents[i].Replace(fileContents[i].Substring(fileContents[i].IndexOf("=") + 1), "0x0000000" + p.Antenna.ToString());
                }

                File.WriteAllLines(iniFile, fileContents);
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        public bool UpdateSettingFile(string xmlFile, ParcelSetting parcel)
        {
            try
            {
                XDocument xD = XDocument.Load(xmlFile);

                var xE = from e in xD.Descendants("controller")
                         where e.Element("no").Value == parcel.No.ToString()
                         select e;

                foreach (var item in xE)
                {
                    item.Element("com").Value = parcel.Com.ToString();
                }

                xD.Save(xmlFile);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void ExportToCSV(string strFileName, List<Tag> lst)
        {
            try
            {
                using (StreamWriter objWriter = new StreamWriter(strFileName, false))
                {
                    objWriter.AutoFlush = true;

                    objWriter.WriteLine("No,UII");

                    foreach (Tag t in lst)
                        objWriter.WriteLine(t.No + "," + t.Uii);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                GC.Collect();
            }
        }
    }

    internal static class MyConverter
    {
        internal static int ToInt32(object obj)
        {
            int iOut = 0;
            if (obj is DBNull || obj == null)
                return 0;

            if (int.TryParse(obj.ToString().Trim(), out iOut))
                return iOut;
            else
                return 0;
        }

        internal static decimal ToDecimal(object obj)
        {
            decimal dOut = 0;
            if (obj is DBNull || obj == null)
                return 0.0M;

            if (decimal.TryParse(obj.ToString().Trim(), out dOut))
                return dOut;
            else
                return 0.0M;
        }

        internal static byte[] StringToByteAry(object obj)
        {
            if (obj is DBNull || obj == null)
                return new byte[0];
            else
                return Encoding.ASCII.GetBytes(obj.ToString().Trim());
        }

        internal static string ByteToString(object obj)
        {
            if (obj is DBNull || obj == null)
                return "";
            else
            {
                if (obj is byte[])
                    return Encoding.ASCII.GetString((obj as byte[]));
                else
                    return "";
            }
        }

        internal static string ToString(object obj)
        {
            if (obj is DBNull || obj == null)
                return "";
            else
                return obj.ToString().Trim();
        }

    }
}

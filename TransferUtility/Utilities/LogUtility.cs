using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HttpBridge.Extension
{
    public class LogUtility
    {
        private static object ERROR = new object();
        private static object MESSAGE = new object();
        public static void LogRecord(string msg, string type = "d", string filePath = null)
        {
            lock (MESSAGE)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                string fileDirectory = string.Format("{0}\\SysLog\\YLog", filePath);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                string fileName = null; DateTime time = DateTime.Now;
                switch (type)
                {
                    case "d": fileName = time.ToString("yyyy-MM-dd"); break;//天
                    case "h": fileName = time.ToString("yyyy-MM-dd HH"); break;//时
                    case "m": fileName = time.ToString("yyyy-MM-dd HH:mm"); break;//分
                    case "s": fileName = time.ToString("yyyy-MM-dd HH:mm:ss"); break;//秒
                    default: break;
                }
                string fullPath = string.Format("{0}\\{1}.log", fileDirectory, fileName);
                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath).Close();
                }
                StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8);
                string text = string.Format("msg: {0}  {1}", msg, time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine(text); sw.Flush(); sw.Close();
            }
        }

        public static void LogException(Exception en, string type = "d", string filePath = null)
        {
            string detailMsg = string.Format("{0}{1}{2}{3}", en.Source, en.Message, en.StackTrace, en.TargetSite);
            LogError(detailMsg, filePath, type);
        }

        public static void LogError(string msg, string type = "d", string filePath = null)
        {
            lock (ERROR)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                string fileDirectory = string.Format("{0}\\SysLog\\NLog", filePath);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                string fileName = null; DateTime time = DateTime.Now;
                switch (type)
                {
                    case "d": fileName = time.ToString("yyyy-MM-dd"); break;//天
                    case "h": fileName = time.ToString("yyyy-MM-dd HH"); break;//时
                    case "m": fileName = time.ToString("yyyy-MM-dd HH:mm"); break;//分
                    case "s": fileName = time.ToString("yyyy-MM-dd HH:mm:ss"); break;//秒
                    default: break;
                }
                string fullPath = string.Format("{0}\\{1}.log", fileDirectory, fileName);
                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath).Close();
                }
                StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8);
                string text = string.Format("errors: {0}  {1}", msg, time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine(text); sw.Flush(); sw.Close();
            }
        }
    }
}

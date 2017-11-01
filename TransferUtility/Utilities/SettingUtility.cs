using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TransferUtility
{
    public class SettingUtility
    {
        #region 字段
        private static string fileName = "Client.sc";//配置文件
        #endregion

        private static Dictionary<string, string> SetDic = null;//配置字典

        /// <summary>
        /// 设置配置
        /// </summary>
        public static bool SetSetting(string key, string value)
        {
            bool b = false;
            if (!string.IsNullOrEmpty(key))
            {
                if (SetDic.ContainsKey(key))
                {
                    SetDic[key] = value;
                }
                else
                {
                    SetDic.Add(key, value);
                }
                b = true;
            }
            return b;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static string GetSetting(string key)
        {
            if (!string.IsNullOrEmpty(key) && SetDic.ContainsKey(key))
            {
                return SetDic[key];
            }
            return null;
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        public static bool LoadSetting()
        {
            bool b = false; string filePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (SetDic == null) { SetDic = new Dictionary<string, string>(); }//初始化
            if (File.Exists(filePath))
            {
                string text = SecureUtility.AESDecrypt(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                if (!string.IsNullOrEmpty(text))
                {
                    SetDic = new Dictionary<string, string>();//实例化
                    string[] array = text.Trim().Split(';');
                    if (array != null && array.Length > 0)
                    {
                        for (int a = 0; a < array.Length; a++)
                        {
                            string single = array[a];
                            if (!string.IsNullOrEmpty(single.Trim()))
                            {
                                string[] childArray = single.Split('=');
                                string key = childArray[0], val = childArray[1];
                                if (!string.IsNullOrEmpty(key))
                                {
                                    if (val == null) { val = string.Empty; }
                                    if (!SetDic.ContainsKey(key))
                                    {
                                        SetDic.Add(key, val);
                                    }
                                    else
                                    {
                                        SetDic[key] = val;
                                    }
                                }
                            }
                        }
                        b = true;//加载成功
                    }
                }
            }
            return b;
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public static void SaveSetting()
        {
            string filePath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(filePath)) { File.Create(filePath).Close(); }//不存在则创建
            if (SetDic != null && SetDic.Count > 0)
            {
                StringBuilder sbr = new StringBuilder();
                foreach (string k in SetDic.Keys)
                {
                    sbr.AppendFormat("{0}={1};", k, SetDic[k]);
                }
                if (sbr.Length > 0)
                {
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    string pwdString = SecureUtility.AESEncrypt(sbr.ToString());
                    fs.Write(Encoding.UTF8.GetBytes(pwdString), 0, pwdString.Length);
                    fs.Flush(); fs.Close();
                    LoadSetting();//重新加载
                }
            }
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        public static bool DelSetting(string key)
        {
            bool b = false;
            if (!string.IsNullOrEmpty(key) && SetDic.ContainsKey(key))
            {
                SetDic.Remove(key);
            }
            return b;
        }
    }
}

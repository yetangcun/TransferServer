using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TransferUtility
{
    public class ConfigUtility
    {
        #region 字段
        private static string fileName = "Client.sc";//配置文件

        private static Dictionary<string, string> ConfigDic = null;//配置字典
        #endregion

        /// <summary>
        /// 设置配置
        /// </summary>
        public static bool SetSetting(string key, string value)
        {
            bool b = false;
            if (!string.IsNullOrEmpty(key))
            {
                if (ConfigDic.ContainsKey(key))
                {
                    ConfigDic[key] = value;
                }
                else
                {
                    ConfigDic.Add(key, value);
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
            if (!string.IsNullOrEmpty(key) && ConfigDic.ContainsKey(key))
            {
                return ConfigDic[key];
            }
            return null;
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        public static bool LoadSetting()
        {
            bool b = false; string filePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (ConfigDic == null) { ConfigDic = new Dictionary<string, string>(); }//初始化
            if (File.Exists(filePath))
            {
                string text = AESDecrypt(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                if (!string.IsNullOrEmpty(text))
                {
                    ConfigDic = new Dictionary<string, string>();//实例化
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
                                if (!string.IsNullOrEmpty(key) && !ConfigDic.ContainsKey(key))
                                {
                                    if (val == null) { val = string.Empty; }
                                    ConfigDic.Add(key, val);
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
            if (ConfigDic != null && ConfigDic.Count > 0)
            {
                StringBuilder sbr = new StringBuilder();
                foreach (string k in ConfigDic.Keys)
                {
                    sbr.AppendFormat("{0}={1};", k, ConfigDic[k]);
                }
                if (sbr.Length > 0)
                {
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    string pwdString = AESEncrypt(sbr.ToString());
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
            if (!string.IsNullOrEmpty(key) && ConfigDic.ContainsKey(key))
            {
                ConfigDic.Remove(key);
            }
            return b;
        }

        #region 加密解密
        private static readonly string aesKey = "xingyao.xiaolao.xiaoyao.xiaoxiao";//加密所需32位密匙
        public static string AESEncrypt(string str)
        {
            Byte[] keys = Encoding.UTF8.GetBytes(aesKey);
            Byte[] encrypts = Encoding.UTF8.GetBytes(str);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keys;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] result = cTransform.TransformFinalBlock(encrypts, 0, encrypts.Length);
            return Convert.ToBase64String(result, 0, result.Length);
        }
        public static string AESDecrypt(string sourceStr)
        {
            byte[] keys = Encoding.UTF8.GetBytes(aesKey);
            byte[] decrypts = Convert.FromBase64String(sourceStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keys;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] result = cTransform.TransformFinalBlock(decrypts, 0, decrypts.Length);
            return Encoding.UTF8.GetString(result);
        }
        #endregion
    }
}

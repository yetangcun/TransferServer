using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace TransferUtility
{
    public class SecureUtility
    {
        #region  MD5
        public static string MD5Encrypt16(string sourceStr)//16位小写
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] array = Encoding.Default.GetBytes(sourceStr);
            string result = BitConverter.ToString(md5.ComputeHash(array)).Replace("-", "");
            return result.ToLower();
        }
        public static string Encrypt32MD5(string sourceStr)//32位
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypts = md5.ComputeHash(Encoding.ASCII.GetBytes(sourceStr));
            StringBuilder sb = new StringBuilder();
            int len = encrypts.Length;
            for (int i = 0; i < len; i++)
            {
                sb.AppendFormat("{0:x2}", encrypts[i]);
            }
            return sb.ToString();
        }
        static string MD5Encrypt32(string sourceStr)
        {
            MD5 md5 = MD5.Create();//实例化一个md5对像
            StringBuilder sr = new StringBuilder();
            //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(sourceStr));
            int len = s.Length; //字节数组
            //通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < len; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                string str = s[i].ToString("X");
                sr.AppendFormat("{0}", str);

            }
            return sr.ToString();
        }
        #endregion

        #region  AES
        private static readonly string aesKey = "xingyun.xiaojun.xiaoyao.xiaoxiao";//加密所需32位密匙
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

        private static string AES_IV = "AES_XIAOXIAO_YAO";
        private static string AES_KEY = "AES_XIAOXIAO_YAO";
        //对文件进行加密
        public static void AESEncryptFile(string filePath)
        {
            if (!File.Exists(filePath)) { throw new FileNotFoundException("文件不存在!", filePath); }
            byte[] arrays = File.ReadAllBytes(filePath);
            if (arrays != null && arrays.Length > 0)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    if (arrays != null)
                    {
                        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                        byte[] ivs = Encoding.Default.GetBytes(AES_IV), keys = Encoding.Default.GetBytes(AES_KEY);
                        using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(keys, ivs), CryptoStreamMode.Write))
                        {
                            cs.Write(arrays, 0, arrays.Length);
                            cs.Flush();
                            cs.Close();
                        }
                    }
                }
            }
        }
        //对文件进行解密
        public static void AESDecryptFile(string filePath)
        {
            if (!File.Exists(filePath)) { throw new FileNotFoundException("文件不存在!", filePath); }
            byte[] arrays = File.ReadAllBytes(filePath);
            if (arrays != null && arrays.Length > 0)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                    byte[] ivs = Encoding.Default.GetBytes(AES_IV), keys = Encoding.Default.GetBytes(AES_KEY);
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(keys, ivs), CryptoStreamMode.Write))
                    {
                        cs.Write(arrays, 0, arrays.Length);
                        cs.Flush();
                        cs.Close();
                    }
                }
            }
        }
        #endregion

        #region DES
        private static string DES_IV = "juncreated"; // 加密偏移量 必须大于等于8位
        private static string DES_KEY = "xiaoxiao";  // 加密密钥 必须为8位
        public static string Encrypt(string sourceStr) //加密原始串
        {
            byte[] ivArray = Encoding.Default.GetBytes(DES_IV);
            byte[] keyArray = Encoding.Default.GetBytes(DES_KEY);
            using (MemoryStream ms = new MemoryStream())
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] sourceBytes = Encoding.Default.GetBytes(sourceStr);
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(keyArray, ivArray), CryptoStreamMode.Write))
                {
                    cs.Write(sourceBytes, 0, sourceBytes.Length); cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptStr) // 解密加密串
        {
            if (string.IsNullOrWhiteSpace(encryptStr)) return null;
            byte[] ivArray = Encoding.Default.GetBytes(DES_IV);
            byte[] keyArray = Encoding.Default.GetBytes(DES_KEY);
            using (MemoryStream ms = new MemoryStream())
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider(); byte[] decryptBytes = Convert.FromBase64String(encryptStr);
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(keyArray, ivArray), CryptoStreamMode.Write))
                {
                    cs.Write(decryptBytes, 0, decryptBytes.Length); cs.FlushFinalBlock();
                    return Encoding.Default.GetString(ms.ToArray());
                }
            }
        }

        //对文件进行加密
        public static void EncryptFile(string originFile, string encryptFile)  //加密文件 originFile是待加密的文件, encryptFile是加密后保存的文件
        {
            if (!File.Exists(originFile)) { throw new FileNotFoundException("给定路径的文件不存在!", originFile); }
            using (FileStream fs = new FileStream(encryptFile, FileMode.Create, FileAccess.Write))
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] ivArray = Encoding.Default.GetBytes(DES_IV), keyArray = Encoding.Default.GetBytes(DES_KEY);
                using (CryptoStream cs = new CryptoStream(fs, des.CreateEncryptor(keyArray, ivArray), CryptoStreamMode.Write))
                {
                    byte[] originBytes = File.ReadAllBytes(originFile); //源文件内容字节数组
                    cs.Write(originBytes, 0, originBytes.Length);
                    cs.FlushFinalBlock();
                }
            }
        }

        //对文件进行解密
        public static void DecryptFile(string encryptFile, string originFile)  //解密文件 encryptFile是已被加密的文件, originFile是解密后保存的文件
        {
            if (!File.Exists(encryptFile)) { throw new FileNotFoundException("给定路径的文件不存在!", encryptFile); }
            using (FileStream fs = new FileStream(originFile, FileMode.Create, FileAccess.Write))
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] ivArray = Encoding.Default.GetBytes(DES_IV), keyArray = Encoding.Default.GetBytes(DES_KEY);
                using (CryptoStream cs = new CryptoStream(fs, des.CreateDecryptor(keyArray, ivArray), CryptoStreamMode.Write))
                {
                    byte[] encryptBytes = File.ReadAllBytes(encryptFile);
                    cs.Write(encryptBytes, 0, encryptBytes.Length);
                    cs.FlushFinalBlock();
                }
            }
        }
        #endregion

        #region Base64
        public static string ConvertToBase64(string originStr)
        {
            byte[] strBytes = Encoding.Default.GetBytes(originStr);
            string baseStr = Convert.ToBase64String(strBytes); return baseStr;
        }

        public static string ConvertFromBase64(string baseStr)
        {
            byte[] strBytes = Convert.FromBase64String(baseStr);
            string originStr = Encoding.Default.GetString(strBytes); return originStr;
        }
        #endregion
    }
}

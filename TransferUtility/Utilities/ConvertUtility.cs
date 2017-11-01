using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace TransferUtility
{
    public class ConvertUtility
    {
        /// <summary>
        /// 转化成String类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertToString(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return obj.ToString().Trim();
        }

        /// <summary>
        /// 转化成Int类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ConvertToInt(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return -1;
            }
            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// 转化成Bool类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ConvertToBool(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return false;
            }
            bool b = false;
            bool.TryParse(obj.ToString(), out b);

            return b;
        }

        /// <summary>
        /// 转化成DateTime类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return DateTime.Parse("1900-01-01");
            }
            return DateTime.Parse(obj.ToString());
        }

        /// <summary>
        /// 将 DataRow 中指定列的数值转换成相应类型的值
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dr">DataRow</param>
        /// <param name="columnName">指定列列名</param>
        /// <param name="defaultValue">默认值(可选)</param>
        /// <returns></returns>
        public static T ConvertToDataRowValue<T>(DataRow dr, string columnName, T defaultValue = default(T))
        {
            if (dr.Table.Columns.Contains(columnName))
            {
                if (dr[columnName] != null && dr[columnName] != DBNull.Value)
                { defaultValue = (T)dr[columnName]; }
            }

            return defaultValue;
        }

        /// <summary>
        /// 将 DataRow 中指定列的数值转换成字符串
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dr">DataRow</param>
        /// <param name="columnName">指定列列名</param>
        /// <param name="formatString">格式化字符串(可选)</param>
        /// <returns></returns>
        public static string ConvertToDataRowString<T>(DataRow dr, string columnName, string formatString = null, string defaultValue = null) where T : struct
        {
            T convertValue = ConvertToDataRowValue<T>(dr, columnName);

            string returnValue = null;
            if (convertValue.ToString() == default(T).ToString())
            { returnValue = defaultValue; }
            else
            {
                if (string.IsNullOrEmpty(formatString))
                { returnValue = convertValue.ToString(); }
                else
                {
                    returnValue = ConvertToValueString<T>(convertValue, formatString);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// 将数值转换成字符串
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dr">DataRow</param>
        /// <param name="columnName">指定列列名</param>
        /// <param name="formatString">格式化字符串(可选)</param>
        /// <returns></returns>
        public static string ConvertToValueString<T>(T convertValue, string formatString = null) where T : struct
        {
            string convertedValue = null;
            try
            {
                if (string.IsNullOrEmpty(formatString))
                { convertedValue = convertValue.ToString(); }
                else
                {
                    var typeInstance = convertValue.GetType();
                    var methodInfo = typeInstance.GetMethods().FirstOrDefault(method =>
                    {
                        var parameters = method.GetParameters();

                        return method.Name == "ToString"
                        && parameters.Length > 0
                        && parameters[0].ParameterType.Name == "String";
                    });

                    if (methodInfo != null)
                    { convertedValue = methodInfo.Invoke(convertValue, new object[] { formatString }).ToString(); }
                }
            }
            catch (Exception ex)
            {
                //CommMessage.ShowMessage(ex.Message);
            }

            return convertedValue;
        }

        /// <summary>
        /// 将DataRow,中指定列colName的值转化成String类型
        /// </summary>
        public static string ConvertToDataRowString(DataRow dr, string colName)
        {
            string str = "";
            try
            {
                if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName))
                {
                    str = dr[colName].ToString().Trim();
                }
            }
            catch { }

            return str;
        }

        /// <summary>
        /// 将DataRow,中指定列colName的值转化成String类型
        /// </summary>
        public static byte[] ConvertToDataRowBytes(DataRow dr, string colName)
        {
            byte[] str = null;
            try
            {
                if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName) && dr[colName] != null)
                {
                    str = (byte[])dr[colName];
                }
            }
            catch { }
            return str;
        }


        /// <summary>
        /// 将DataRow,中指定列colName的值转化成DateTime类型
        /// </summary>
        public static object ConvertToDataRowDateTime(DataRow dr, string colName)
        {
            object obj = new object();
            DateTime _DateTime = new DateTime();
            try
            {
                if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName))
                {
                    if (DateTime.TryParse(ConvertToDataRowString(dr, colName), out _DateTime))
                    {
                        obj = _DateTime;
                    }
                    else
                    {
                        obj = null;
                    }
                }
            }
            catch { }
            return obj;
        }

        /// <summary>
        ///  将DataRow,中指定列colName的值转化成Int类型
        /// </summary>
        public static int ConvertToDataRowInt(DataRow dr, string colName)
        {
            int r = -1;
            if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName))
            {
                int.TryParse(dr[colName].ToString(), out r);
            }
            return r;
        }
        /// <summary>
        ///  将DataRow,中指定列colName的值转化成double类型
        /// </summary>
        public static double ConvertToDataRowDouble(DataRow dr, string colName)
        {
            double r = 0.0;
            if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName))
            {
                double.TryParse(dr[colName].ToString(), out r);
            }
            return r;
        }
        public static bool ConvertToDataRowBool(DataRow dr, string colName)
        {
            bool r = false;
            if (dr != null && !string.IsNullOrEmpty(colName) && dr.Table.Columns.Contains(colName))
            {
                bool.TryParse(dr[colName].ToString(), out r);
            }
            return r;
        }



        #region 可序列化对象到byte数组的相互转换

        /// <summary>
        /// 将可序列化对象转成Byte数组
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>返回相关数组</returns>
        public static byte[] ObjectToByteArray(object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            ms.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 将可序列化对象转成的byte数组还原为对象
        /// </summary>
        /// <param name="b">byte数组</param>
        /// <returns>相关对象</returns>
        public static object ByteArrayToObject(byte[] b)
        {
            MemoryStream ms = new MemoryStream(b, 0, b.Length);
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(ms);
        }

        #endregion 可序列化对象到byte数组的相互转换

        #region 采用.net系统自带Gzip压缩类进行流压缩

        /// <summary>
        /// 压缩数据
        /// 采用.net系统自带Gzip压缩类进行流压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GzipCompress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            GZipStream stream = new GZipStream(ms, CompressionMode.Compress, true);
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            //并且解压缩方法stream.Read(buffer, 0, buffer.Length)时会返回0
            bData = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return bData;
        }

        /// <summary>
        /// 解压数据
        /// 采用.net系统自带Gzip压缩类进行流压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GzipDecompress(byte[] data)
        {
            byte[] bData;
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            GZipStream stream = new GZipStream(ms, CompressionMode.Decompress, true);
            byte[] buffer = new byte[1024];
            MemoryStream temp = new MemoryStream();
            int read = stream.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                temp.Write(buffer, 0, read);
                read = stream.Read(buffer, 0, buffer.Length);
            }
            //必须把stream流关闭才能返回ms流数据,不然数据会不完整
            stream.Close();
            stream.Dispose();
            ms.Close();
            ms.Dispose();
            bData = temp.ToArray();
            temp.Close();
            temp.Dispose();
            return bData;
        }

        #endregion 采用.net系统自带Gzip压缩类进行流压缩

        #region 字节单位转换

        /// <summary>
        /// 字节单位转换
        /// </summary>
        /// <param name="byteslength">字节长度</param>
        /// <param name="si">si参数为True则是以国际单位制1000为单位，为False则是以二进制的1024为单位进位。</param>
        /// <returns>返回结果1024字节就显示1KB。1024*1024字节则显示1MB</returns>
        public static String ConvertByteCount(long byteslength, bool si = true)
        {
            int unit = si ? 1000 : 1024;

            if (byteslength < unit) return byteslength + " B";

            int exp = (int)(Math.Log(byteslength) / Math.Log(unit));

            String pre = (si ? "kMGTPE" : "KMGTPE")[exp - 1] + (si ? "" : "i");

            return String.Format("{0:F1} {1}B", byteslength / Math.Pow(unit, exp), pre);
        }

        #endregion 字节单位转换

        private static System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
        /// <summary>
        /// 字符串转UTF-8 byte
        /// </summary>
        /// <param name="StringMessage"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToByte(string StringMessage)
        {
            Byte[] BytesMessage = UTF8.GetBytes(StringMessage);
            return BytesMessage;
        }

        /// <summary>
        /// UTF-8 byte 转字符串 
        /// </summary>
        /// <param name="StringMessage"></param>
        /// <returns></returns>
        public static string ConvertByteToString(byte[] BytesMessage)
        {
            //UTF-8 byte 转字符串 
            String StringMessage = UTF8.GetString(BytesMessage);
            return StringMessage;
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>内存流</returns>
        public static MemoryStream ObjectSerialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();  //序列化二进制类
            bformatter.Serialize(ms, obj);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}

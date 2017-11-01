using Newtonsoft.Json;
using System.Collections.Generic;

namespace TransferUtility
{
    public class JsonUtility
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">序列化对象</param>
        /// <returns>json串</returns>
        public static string Serializer<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="json">反序列化串</param>
        /// <returns>反序列化对象</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="json">反序列化串</param>
        /// <returns>反序列化对象列表</returns>
        public static List<T> Deserializes<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}

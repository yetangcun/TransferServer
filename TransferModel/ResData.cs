using System;
using System.Data;
using System.Runtime.Serialization;

namespace TransferModel
{
    [Serializable]
    public class ResData
    {
        /// <summary>
        /// 执行状态:成功或失败
        /// </summary>
        public bool ExecuteResult { get; set; }

        /// <summary>
        /// 返回提示信息,可以是正常的提示,也可以是异常信息提示
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// 返回字符串类型的执行结果
        /// </summary>
        public string ResultString { get; set; }

        /// <summary>
        /// 返回字符串数组类型的结果
        /// </summary>
        public string[] ResultStrings { get; set; }

        /// <summary>
        /// 返回字节数组类型的结果
        /// </summary>
        public byte[] ResultByte { get; set; }

        public byte[][] ParamBytes { get; set; }//请求参数

        /// <summary>
        /// 返回DataTable类型的结果
        /// </summary>
        public DataTable ResultDataTable { get; set; }

        /// <summary>
        /// 返回DataSet类型结果
        /// </summary>
        public DataSet ResultDataSet { get; set; }

        /// <summary>
        /// 通讯加密标识
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// 是否启用压缩
        /// </summary>
        public bool IsZip { get; set; }

        /// <summary>
        /// 压缩数据的类型
        /// </summary>
        public string ZipType { get; set; }
    }
}

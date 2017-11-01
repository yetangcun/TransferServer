using System;

namespace TransferCommon
{
    [Serializable]
    public class ReqData
    {
        public string[] ParamString { get; set; }//请求参数

        public byte[][] ParamBytes { get; set; }//字节数组

        public byte[] ParamByte { get; set; }//字节数

        public string ServiceName { get; set; }//服务名称
        
        public string RouteName { get; set; }//路由名称
        
        public string DBMarker { get; set; }//数据库标识
        
        public string Marker { get; set; }//通信标识
        
        public bool IsUseZip { get; set; }//是否压缩
    }
}

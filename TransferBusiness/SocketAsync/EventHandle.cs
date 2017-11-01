using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace TransferBusiness.SocketAsync
{
    /// <summary>
    /// 数据接收
    /// </summary>
    public class DataReceiveHandle
    {
        public string ConnMarker { get; set; }//连接标识

        public byte[] Data { get; set; }//数据

        public DataReceiveHandle(string connMarker, byte[] data)
        {
            ConnMarker = connMarker;
            Data = data;
        }
    }
    
    /// <summary>
    /// 客户端连接状态
    /// </summary>
    public class ConnStatusChangedHandle
    {
        public string ConnMarker { get; set; }//连接标识

        public bool IsConnected { get; set; }//是否连接

        public ConnStatusChangedHandle(string connMarker,bool isConnected)
        {
            ConnMarker = connMarker;
            IsConnected = isConnected;
        }
    }

}

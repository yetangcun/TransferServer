using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TransferModel.Client
{
    public class OutClient
    {
        public long Port;//侦听端口
        public string OutIp;//远程客户端IP
        public long OutPort;//远程客户端端口
        public int ConnectId;//标识该连接的ID
    }
}

using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using System.Net;

namespace TransferModel
{
    public class SysCache
    {
        public static string IP
        {
            get;set;
        }
        public static int Port = -1;

        public static List<int> Ports
        {
            get;set;
        }

        public static Dictionary<EndPoint, int> ConnecttingDic = new Dictionary<EndPoint, int>();
        public static List<Socket> SocList = new List<Socket>();


        private static int _StoreWay = -1; //存储方式 
        public static int StoreWay  //0:本地1:数据库
        {
            get
            {
                if (_StoreWay == -1)
                {
                    int.TryParse(ConfigurationManager.AppSettings["StoreWay"], out _StoreWay);
                }
                if (_StoreWay < 0)
                {
                    _StoreWay = 0;
                }
                return _StoreWay;
            }
            set
            {
                _StoreWay = value;
            }
        }
    }
}

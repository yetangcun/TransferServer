using System;
using System.IO;
using TransferCommon;

namespace TransferBusiness
{
    public class TransRouter
    {
        public static ResData TransData(ReqData req)
        {
            ResData res = new ResData()
            {
                ExecuteResult = true,
                ResultByte = File.ReadAllBytes("sms.txt"),
                ResultString = "Hey , Good Boy, Best wish to you !"
            };
            try
            {
                switch (req.ServiceName)
                {
                    default:break;
                }
            }
            catch (Exception e)
            {
                res = new ResData()
                {
                    ExecuteResult = false,
                    ResultMessage = e.Message
                };
            }
            return res;
        }
    }
}

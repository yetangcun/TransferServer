using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TransferModel
{
    public class UserInfo
    {
        public int UserID { get; set; }//用户编号
        public int UserType { get; set; }//用户类型
        public string Phone { get; set; }//手机号
        public string UserTypeCH { get; set; }//用户类型名
        public string UserRealName { get; set; }//真实姓名
        public string UserLoginName { get; set; }//用户名
        public string PermitString { get; set; }//权限
        public string InsertTime { get; set; }//添加日期
    }
}

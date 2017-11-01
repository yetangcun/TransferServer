using System.Windows.Controls;

namespace TransferUtility
{
    public delegate void ShowMsgDelegate(string msg, ListView lv = null);

    public delegate void ShowVedioDelegate(object msg,int size, Image img =null);
    public class MsgUtility
    {
        public static event ShowMsgDelegate ShowMsgHandle = null;

        public static void ShowMsg(string msg, object lv = null)
        {
            ListView lView = null;
            if (lv != null)
            {
                lView = lv as ListView;
                if (ShowMsgHandle != null)
                {
                    ShowMsgHandle(msg, lView);
                }
            }
        }

        public static event ShowVedioDelegate ShowVedioHandle = null;
        public static void ShowVedio(object bytes,int size,object vd=null)
        {
            Image vedio = (Image)vd;
            if (vd != null)
            {
                if (ShowVedioHandle != null)
                {
                    ShowVedioHandle(bytes, size, vedio);
                }
            }
        }
    }
}

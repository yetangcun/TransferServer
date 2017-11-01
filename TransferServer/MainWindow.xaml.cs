using System;
using System.IO;
using System.Windows;
using TransferUtility;
using HttpBridge.Extension;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TransferServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitLoad();
        }

        private void InitLoad()
        {
            try
            {
                MsgUtility.ShowMsgHandle += ShowMsg;

                WinNotifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/TransferServer;component/wireless.ico")).Stream);
                WinNotifyIcon.Visible = true; WinNotifyIcon.MouseDoubleClick += WinNotifyIcon_MouseDoubleClick;
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }
        
        private void ShowMsg(string msg, ListView lv = null)
        {
            try
            {
                if (lv == null) return;
                if (!string.IsNullOrEmpty(msg))
                {
                    MsgInfo msgInfo = new MsgInfo() { Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"), Msg = msg };
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)delegate ()
                    {
                        lv.Items.Add(msgInfo);
                        if (lv.Items.Count > 100)
                        {
                            lv.Items.RemoveAt(0);
                        }
                        lv.ScrollIntoView(msgInfo);
                    });
                }
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }

        private void ShowVedio(object obj,int size,Image img)
        {
            try
            {
                byte[] bts = (byte[])obj;
                if (bts != null)
                {
                    BitmapImage bti = new BitmapImage();
                    bti.BeginInit();
                    bti.StreamSource = new MemoryStream(bts, 0, size);
                    bti.EndInit();
                    img.Source = bti;
                }
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定退出系统吗?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    socketSrever.DisposeObj();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                else e.Cancel = true;
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }

        #region 显示到托盘
        private System.Windows.Forms.NotifyIcon WinNotifyIcon = new System.Windows.Forms.NotifyIcon();
        private void WinNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }
            catch (Exception en) { MessageBox.Show(en.Message); }
        }
        #endregion

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }
    }

    public class MsgInfo
    {
        public string Time { get; set; }
        public string Msg { get; set; }
    }
}

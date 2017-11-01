using System;
using System.Windows;
using TransferCommon;
using TransferUtility;
using System.Windows.Controls;
using TransferBusiness.SocketAsync;
using TransferModels;

namespace TransferServer.Views
{
    /// <summary>
    /// PortResend.xaml 的交互逻辑
    /// </summary>
    public partial class SocketUI : UserControl
    {
        public SocketUI()
        {
            InitializeComponent();

            InitLoad();
        }
        private SocketAsync socketAsync = null;

        private void InitLoad()
        {
            try
            {
                SettingUtility.LoadSetting();
                SysCache.IP = SettingUtility.GetSetting("ServerListenIp");
                int.TryParse(SettingUtility.GetSetting("ServerListenPort"), out SysCache.Port);
                txtPort.Text = SysCache.Port.ToString(); txtIp.Text = SysCache.IP;
                socketAsync = new SocketAsync(SysCache.IP, SysCache.Port);
                socketAsync.Lv = msgView;
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, msgView);
            }
        }

        private void btn_Start_Listen_Handle(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                socketAsync.Start();
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, msgView);
            }
        }

        private void btn_Stop_Listen_Handle(object sender, RoutedEventArgs e)
        {
            DisposeObj();
        }

        public void DisposeObj()
        {
            try
            {
                socketAsync.Stop();
                //socketAsync.AsyncStop();
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, msgView);
            }
        }

        private void btn_Clear_Record_Handle(object sender, RoutedEventArgs e)
        {
            msgView.Items.Clear();
        }

        private void btn_Edit_Handle(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn.Content.Equals("编辑"))
                {
                    btn.Content = "保存";
                    txtIp.IsEnabled = true;
                }
                else if (btn.Content.Equals("保存"))
                {
                    btn.Content = "编辑";
                    txtIp.IsEnabled = false;
                    SysCache.IP = txtIp.Text.Trim();
                    SettingUtility.SetSetting("ServerListenIp", txtIp.Text.Trim());
                    SettingUtility.SetSetting("ServerListenPort", txtPort.Text.Trim());

                    SettingUtility.SaveSetting();
                    SettingUtility.LoadSetting();
                }
            }
            catch (Exception en)
            {
                MessageBox.Show(en.Message);
            }
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransferUtility;
using TransferBusiness;
using TransferCommon;
using TransferModels;

namespace TransferServer.Views
{
    /// <summary>
    /// PortMap.xaml 的交互逻辑
    /// </summary>
    public partial class PortMap : UserControl
    {
        private SocketServer serverSocket = null;
        public PortMap()
        {
            InitializeComponent();

            InitLoad();
        }

        private void InitLoad()
        {
            try
            {
                serverSocket = new SocketServer()
                {
                    Lv = msgView,
                    Vd = null
                };

                SettingUtility.LoadSetting();
                SysCache.IP = SettingUtility.GetSetting("ServerListenIp");
                int.TryParse(SettingUtility.GetSetting("ServerListenPort"), out SysCache.Port);
                txtPort.Text = SysCache.Port.ToString();
                txtIp.Text = SysCache.IP;
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message);
            }
        }

        private void btn_Start_Listen_Handle(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                serverSocket.InitSocket(out msg);
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message);
            }
        }

        private void btn_Stop_Listen_Handle(object sender, RoutedEventArgs e)
        {
            try
            {
                serverSocket.DisposeObj();
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message);
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

using System;
using System.IO;
using System.Net;
using System.Text;
using TransferCommon;
using TransferModels;
using TransferUtility;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace TransferBusiness
{
    public class SocketServer
    {
        public object Lv = null;
        public object Vd = null;
        public bool IsListen = false;
        public void InitSocket(out string msg)
        {
            msg = null;
            try
            {
                if (IsListen)
                {
                    MsgUtility.ShowMsg("已开启侦听...", Lv);
                    return;
                }
                if (string.IsNullOrEmpty(SysCache.IP))
                {
                    SysCache.IP = "192.168.31.27";
                }
                if (SysCache.Ports == null || SysCache.Ports.Count <= 0)
                {
                    SysCache.Ports = new List<int>() { 3699, 9866 };
                }
                if (SysCache.Ports != null)
                {
                    Socket server = null;
                    foreach (int p in SysCache.Ports)
                    {
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        EndPoint ep = new IPEndPoint(IPAddress.Parse(SysCache.IP), p);
                        server.Bind(ep); server.Listen(30);
                        SysCache.SocList.Add(server);

                        MsgUtility.ShowMsg(string.Format("{0}:{1}开启侦听!", SysCache.IP, p), Lv);
                        server.BeginAccept(AsyncAccept, server);
                    }
                    IsListen = true;
                }
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, Lv);
            }
        }
        
        public void AsyncAccept(IAsyncResult ar)
        {
            Socket server = ar.AsyncState as Socket;
            try
            {
                Socket work = server.EndAccept(ar);//接收连接
                IPEndPoint sIpe = (IPEndPoint)work.LocalEndPoint;
                MsgUtility.ShowMsg(string.Format("{0}连接到{1}", work.RemoteEndPoint.ToString(), sIpe.Port.ToString()), Lv);
                StateObject state = new StateObject()
                {
                    WorkSocket = work
                };
                work.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceive), state);

                server.BeginAccept(AsyncAccept, server);//继续接收
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, Lv);
            }
        }

        public void AsyncReceive(IAsyncResult iar)
        {
            StateObject state = (StateObject)iar.AsyncState;
            MemoryStream mStream = new MemoryStream();
            mStream.Position = 0;
            Socket work = state.WorkSocket;
            try
            {
                int bytesRead = work.EndReceive(iar);//接收数据
                iar.AsyncWaitHandle.Close();
                ResData res = null;
                if (bytesRead > 0)
                {
                    mStream.Write(state.buffer, 0, bytesRead);
                    mStream.Flush();
                    // mStream.Position = 0;
                    if (mStream.Length > 0)
                    {
                        BinaryFormatter bFormatter = new BinaryFormatter();
                        ReqData req = (ReqData)bFormatter.Deserialize(mStream);

                        res = TransRouter.TransData(req);
                        AsyncSend(work, res);
                    }
                    mStream.Position = 0;
                    mStream.Close();
                }
                work.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(AsyncReceive), state);
            }
            catch (Exception en)
            {
                work.Shutdown(SocketShutdown.Both);
                work.Close();
                work = null;
                iar.AsyncWaitHandle.Close();
                MsgUtility.ShowMsg(en.Message, Lv);
            }
        }

        public void AsyncSend(Socket handle, ResData res)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();
                BinaryFormatter bformatter = new BinaryFormatter();  //二进制序列化类  
                bformatter.Serialize(mStream, res); //将消息类转换为内存流
                mStream.Flush();
                mStream.Position = 0;

                byte[] data = new byte[mStream.Length];
                mStream.Read(data, 0, data.Length);
                handle.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), handle);
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, Lv);
            }
        }

        private void SendCallback(IAsyncResult iar)
        {
            try
            {
                Socket work = (Socket)iar.AsyncState;
                int bytesSent = work.EndSend(iar);
                iar.AsyncWaitHandle.Close();
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message, Lv);
            }
        }

        public void DisposeObj()
        {
            if (SysCache.SocList != null)
            {
                foreach (Socket s in SysCache.SocList)
                {
                    s.Close();
                }
                IsListen = false;
                SysCache.SocList.Clear();
                MsgUtility.ShowMsg(string.Format("已关闭所有侦听!"), Lv);
            }
        }
    }

    public class StateObject
    {  
        public Socket WorkSocket = null; // Client Socket

        public const int BufferSize = 4194304; // Size of receive buffer 4M
             
        public byte[] buffer = new byte[BufferSize]; // Receive buffer.
             
        public StringBuilder sb = new StringBuilder(); // Received data string.
    }
}

using System;
using System.IO;
using System.Net;
using TransferCommon;
using TransferUtility;
using System.Threading;
using System.Net.Sockets;
using HttpBridge.Extension;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace TransferBusiness.SocketAsync
{
    public class SocketAsync
    {
        public object Lv = null;   //显示消息载体
        int connectedSockets = 0;  //客户端连接数 Semaphore maxAcceptedClients = null;
        private int initSize = 4096; //默认缓冲区 不能设置太大 太大在Receive的时候会发生延迟 甚至出现异常或连接断开
        private Socket serverListen = null;  //侦听Socket
        private EndPoint serverPoint = null; //套接字
        //private BufferManager bufferManager; //缓冲区
        private SocketAsyncEventArgs acceptArgs = null; //侦听异步连接
        private SocketAsyncEventArgsPool saeaPool = null; //客户端连接池

        private List<SocketAsyncEventArgs> ClientSockets = null; //所有已连接对象
        private string ipStr = null; //服务IP
        private int iPort = -1; //侦听端口

        public SocketAsync(string ipString, int port)
        {
            ipStr = ipString;
            iPort = port;
        }

        public void Start() //初始化
        {
            if (serverListen == null)
            {
                IPAddress addr = IPAddress.Parse(ipStr);
                serverPoint = new IPEndPoint(addr, iPort);
                saeaPool = new SocketAsyncEventArgsPool(MaxClientNums);

                acceptArgs = new SocketAsyncEventArgs();
                ClientSockets = new List<SocketAsyncEventArgs>();
                acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

                SocketAsyncEventArgs saeaArgs;
                for (int i = 0; i < MaxClientNums; i++)
                {
                    saeaArgs = new SocketAsyncEventArgs();
                    saeaArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIoCompleted);
                    saeaArgs.SetBuffer(new byte[initSize], 0, initSize); //bufferManager.SetBuffer(saeaArgs); 
                    saeaPool.Push(saeaArgs);
                }

                serverListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverListen.Bind(serverPoint); serverListen.Listen(MaxClientNums); //开始侦听

                MsgUtility.ShowMsg(string.Format("服务 {0} 启动成功,开始侦听...", serverListen.LocalEndPoint.ToString()), Lv);
                AsyncAccept(); //等待连接
            }
            else
            {
                MsgUtility.ShowMsg(string.Format("服务 {0} 正在侦听...", serverListen.LocalEndPoint.ToString()), Lv);
            }
        }

        private void AsyncAccept() //建立连接
        {
            acceptArgs.AcceptSocket = null;
            if (serverListen != null && !serverListen.AcceptAsync(acceptArgs))
            {
                AcceptHandle();
            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            AcceptHandle();
        }

        private void AcceptHandle() //建立连接
        {
            if (acceptArgs.SocketError == SocketError.Success)
            {
                Socket client = acceptArgs.AcceptSocket;
                if (client != null && client.Connected)
                {
                    try
                    {
                        Interlocked.Increment(ref connectedSockets);
                        SocketAsyncEventArgs args = saeaPool.Pop();
                        if (args != null)
                        {
                            args.UserToken = new AsyncUserToken()
                            {
                                Socket = client,
                                ConnectTime = DateTime.Now
                            };
                            ClientSockets.Add(args);//已连接的客户端
                            MsgUtility.ShowMsg(string.Format(" {0} 已建立连接...", client.RemoteEndPoint.ToString()), Lv);//打印连接客户端信息
                            if (!client.ReceiveAsync(args))
                            {
                                AsyncReceive(args);
                            }
                        }
                        else
                        {
                            MsgUtility.ShowMsg(string.Format("客户端数已达极限:{0}...", saeaPool.Count), Lv);//打印连接客户端信息
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogException(ex);
                    }
                    AsyncAccept();//继续接受连接请求
                }
            }
        }

        private void OnIoCompleted(object sender, SocketAsyncEventArgs e) //操作回调
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    AsyncReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    AsyncSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        private void AsyncReceive(SocketAsyncEventArgs e) //接收数据
        {
            try
            {
                //判断连接是否正常
                int transSize = e.BytesTransferred;
                if (e.SocketError == SocketError.Success && transSize > 0)
                {
                    AsyncUserToken token = (AsyncUserToken)e.UserToken;
                    BufferManager.WriteBuffer(token.Token, e.Buffer, e.Offset, transSize);
                    List<byte[]> datas = BufferManager.PacketsHandle(token.Token);
                    if (datas != null && datas.Count > 0)
                    {
                        MemoryStream memeory = null;
                        foreach (var data in datas)
                        {
                            memeory = new MemoryStream();
                            memeory.Write(data, 0, data.Length);
                            memeory.Flush(); memeory.Position = 0;
                            if (memeory.Length > 0)
                            {
                                BinaryFormatter bFormatter = new BinaryFormatter(); //序列化
                                ReqData req = (ReqData)bFormatter.Deserialize(memeory);
                                MsgUtility.ShowMsg(req.ParamString[0], Lv);
                                ResData res = TransRouter.TransData(req); //处理请求
                                res.Marker = req.Marker; //客户端需要用到
                                memeory.Close();

                                using (memeory = new MemoryStream())
                                {
                                    bFormatter.Serialize(memeory, res); //反序列化
                                    memeory.Flush(); memeory.Position = 0;
                                    int len = (int)memeory.Length;
                                    byte[] ress = new byte[len];
                                    memeory.Read(ress, 0, len);
                                    using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                                    {
                                        var list = new List<byte>(BitConverter.GetBytes(len)); //头4个字节标识数据大小
                                        list.AddRange(ress); //完整包含数据内容和数据包头
                                        args.SetBuffer(list.ToArray(), 0, list.Count); //bool willRaiseEvent = token.Socket.??(e);
                                        token.Socket.SendAsync(args); //本次流程结束 
                                    }
                                }
                            }
                        }
                    }

                    if (e.SocketError == SocketError.Success) //连接正常
                    {
                        if (!token.Socket.ReceiveAsync(e))
                        {
                            AsyncReceive(e); //继续异步接收数据
                        }
                    }
                }
                else if (e.SocketError == SocketError.ConnectionReset || e.SocketError == SocketError.ConnectionAborted)//连接断开
                {
                    if (connectedSockets > 0)
                    {
                        Interlocked.Decrement(ref connectedSockets);
                    }

                    ClientSockets.Remove(e); //移除断开的客户端
                    AsyncUserToken aut = (AsyncUserToken)e.UserToken; //异步连接信息
                    BufferManager.DisposeObj(aut.Token); //释放缓冲区对象

                    MsgUtility.ShowMsg(string.Format("客户端 {0} 已断开连接...", aut.Socket.RemoteEndPoint.ToString()), Lv); //打印运行消息
                    aut = null; e.UserToken = null; e.SocketError = SocketError.NotConnected; //资源释放

                    //e = new SocketAsyncEventArgs(); e.Completed += new EventHandler<SocketAsyncEventArgs>(OnIoCompleted); //重新实例化
                    saeaPool.Push(e); //重复使用, 提升效率和资源利用率
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogException(ex);
            }
        }

        public ResData AsyncSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)//连接正常
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                if (!token.Socket.ReceiveAsync(e))
                {
                    AsyncReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
            return null;
        }

        #region 主动发送
        public ResData Send(SocketAsyncEventArgs e, byte[] data)
        {
            ResData res = null;
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    AsyncUserToken aut = (AsyncUserToken)e.UserToken;
                    var list = new List<byte>(BitConverter.GetBytes(data.Length)); //头4个字节(数据包头)标识数据大小
                    list.AddRange(data);   //完整包:数据内容和数据包头

                    e.SetBuffer(list.ToArray(), 0, list.Count);
                    aut.Socket.SendAsync(e);
                }
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message);
            }
            return res;
        }
        public void Send(Socket sk, byte[] data)
        {
            try
            {
                if (sk!=null)
                {
                    var list = new List<byte>(BitConverter.GetBytes(data.Length)); //头4个字节标识数据大小(数据包头)
                    list.AddRange(data);   //完整包:数据内容和数据包头
                    sk.Send(list.ToArray());
                }
            }
            catch (Exception en)
            {
                MsgUtility.ShowMsg(en.Message);
            }
        }
        #endregion

        private void CloseClientSocket(SocketAsyncEventArgs e) //关闭连接
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
                BufferManager.DisposeObj(token.Token); //释放缓冲区对象
            }
            catch (Exception)
            {
                // throws if client process has already closed
            }
            token.Socket.Close();

            Interlocked.Decrement(ref connectedSockets);//更新连接的客户端
            saeaPool.Push(e); // Free the SocketAsyncEventArg so they can be reused by next client
        }

        public void Stop()//停止侦听
        {
            try
            {
                if (serverListen != null)
                {
                    ResData res = new ResData()
                    {
                        ExecuteResult = false,
                        ResultMessage = "shutdown"
                    };
                    byte[] bits = GetSendBuffers(res);
                    var list = new List<byte>(BitConverter.GetBytes(bits.Length)); //头4个字节标识数据大小(数据包头)
                    list.AddRange(bits); //完整包含数据内容和数据包头
                    foreach (SocketAsyncEventArgs args in ClientSockets)
                    {
                        AsyncUserToken aut = (AsyncUserToken)args.UserToken; //连接信息
                        if (aut.Socket.Send(list.ToArray()) > 0)
                        {
                            aut.Socket.Shutdown(SocketShutdown.Send);
                            BufferManager.DisposeObj(aut.Token); //释放缓冲区对象
                            aut.Socket.Close();
                            aut.Socket = null;
                            aut = null; args.Dispose();
                        }
                    }
                    ClientSockets.Clear(); ClientSockets = null;

                    MsgUtility.ShowMsg(string.Format("服务端 {0} 停止侦听...", serverListen.LocalEndPoint.ToString()), Lv); //打印客户端断开连接信息
                    serverListen.Close();
                    serverListen = null;
                }
                
                serverPoint = null; // bufferManager = null;
                saeaPool = null;
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }

        public void AsyncStop()//停止侦听
        {
            try
            {
                if (serverListen != null)
                {
                    ResData res = new ResData()
                    {
                        ExecuteResult = false,
                        ResultMessage = "shutdown"
                    };
                    SocketAsyncEventArgs saea = GetSocketAsyncEventArgs(res);
                    AsyncUserToken aut = null;
                    foreach (SocketAsyncEventArgs args in ClientSockets)
                    {
                        aut = (AsyncUserToken)args.UserToken; //该连接信息
                        aut.Socket.SendAsync(saea); Thread.Sleep(50);//不暂停会不正常
                        aut.Socket.Shutdown(SocketShutdown.Send);
                        BufferManager.DisposeObj(aut.Token); //释放缓冲区对象
                        aut.Socket.Close();
                        aut.Socket = null;
                        aut = null;
                        args.Dispose(); // bufferManager.FreeBuffer(args);
                    }
                    ClientSockets.Clear(); ClientSockets = null;

                    MsgUtility.ShowMsg(string.Format("服务端 {0} 停止侦听...", serverListen.LocalEndPoint.ToString()), Lv); //打印客户端断开连接信息
                    serverListen.Close();
                    serverListen = null;
                }
                serverPoint = null; //bufferManager = null;
                saeaPool = null;
            }
            catch (Exception en)
            {
                LogUtility.LogException(en);
            }
        }

        private SocketAsyncEventArgs GetSocketAsyncEventArgs(ResData res)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, res); ms.Flush();
                ms.Position = 0;
                byte[] msg = new byte[ms.Length];
                ms.Read(msg, 0, msg.Length);

                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(msg, 0, msg.Length);
                return saea;
            }
        }

        private byte[] GetSendBuffers(ResData res)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, res); ms.Flush();
                ms.Position = 0;
                byte[] msg = new byte[ms.Length];
                ms.Read(msg, 0, msg.Length);
                return msg;
            }
        }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxClientNums = 10000;

        /// <summary>
        /// 数据接收
        /// </summary>
        public Action<DataReceiveHandle> OnDataReceive = null;

        /// <summary>
        /// 客户端状态改变
        /// </summary>
        public Action<ConnStatusChangedHandle> OnConnStatusChanged = null;
    }

    #region 辅助类
    class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> saeaPool;

        // Initializes the object pool to the specified size
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold
        public SocketAsyncEventArgsPool(int capacity)
        {
            saeaPool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // The "item" parameter is the SocketAsyncEventArgs instance 
        // Add a SocketAsyncEventArg instance to the pool
        // to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (saeaPool)
            {
                saeaPool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Pop()
        {
            lock (saeaPool)
            {
                return saeaPool.Pop();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get { return saeaPool.Count; }
        }
    }

    class AsyncUserToken
    {
        /// <summary>  
        /// 通信SOKET  
        /// </summary>  
        public Socket Socket { get; set; }

        /// <summary>  
        /// 连接时间  
        /// </summary>  
        public DateTime ConnectTime { get; set; }

        /// <summary>  
        /// 所属用户信息  
        /// </summary>  
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 连接唯一标识
        /// </summary>
        public string Token { get; set; }

        public AsyncUserToken()
        {
            Token = Guid.NewGuid().ToString();
        }
    }
    #endregion
}

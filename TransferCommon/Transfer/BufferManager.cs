using System;
using System.Collections;
using System.Collections.Generic;

namespace TransferCommon
{
    public class BufferManager : IDisposable
    {
        private int len = 0; //大小
        private int offset = 0; //位置
        private byte[] buffer = null; //缓冲区
        private object obj = new object(); //锁

        public static Hashtable hashTable = new Hashtable(); //缓存
        private int size = 1024 * 8; //默认8k

        private int MaxSize = 545259520;//520M 标识大小
        private int MaxBuffer = 512000; //500KB
        private int LeftSize //可用空间
        {
            get { return buffer.Length - offset - len; }
        }

        /// <summary>
        /// 是否把网络字节顺序转为本地字节顺序
        /// </summary>
        public static bool NetByteOrder { get; set; }

        public BufferManager()
        {
            buffer = new byte[size];
        }

        /// <summary>
        /// 把数据写入动态缓存
        /// </summary>
        public static void WriteBuffer(string marker, byte[] buffer, int offset, int count)
        {
            BufferManager buffManager = null;
            if (hashTable.ContainsKey(marker) == false)
            {
                buffManager = new BufferManager();
                hashTable.Add(marker, buffManager);
            }
            else buffManager = hashTable[marker] as BufferManager;
            buffManager.WriteBuffer(buffer, offset, count);
        }

        /// <summary>
        /// 写入数据到动态缓存中
        /// </summary>
        private void WriteBuffer(byte[] data, int offSet, int count)
        {
            lock (obj)
            {
                if (len <= 0 && buffer.Length >= MaxBuffer) //重置缓冲区
                {
                    buffer = new byte[size];
                    offset = 0; len = 0;
                }
                if (LeftSize >= count) //缓冲区空间足够
                {
                    Array.Copy(data, offSet, buffer, offset + len, count); //复制数据
                }
                else //缓冲区空间不够，需要申请更大的内存，并进行移位
                {
                    int totalSize = buffer.Length * 8; //新容量是原来的8倍
                    //若总大小还是不够则需要重新设置大小
                    if (totalSize < len + count) totalSize = len + count;

                    byte[] tmpBuffer = new byte[totalSize];
                    Array.Copy(buffer, offset, tmpBuffer, 0, len); //复制以前的数据
                    Array.Copy(data, offSet, tmpBuffer, len, count); //复制新写入的数据
                    offset = 0; buffer = tmpBuffer; //替换
                }
                len += count;
            }
        }

        /// <summary>
        /// 处理完整数据包
        /// </summary>
        public static List<byte[]> PacketsHandle(string marker)
        {
            if (hashTable.ContainsKey(marker))
            {
                BufferManager buffManager = hashTable[marker] as BufferManager;
                return buffManager.PacketsAnalyze(marker);
            }
            return null;
        }

        /// <summary>
        /// 数据包解析
        /// </summary>
        private List<byte[]> PacketsAnalyze(string marker)
        {
            if (len < 4) return null; //无数据或数据格式错误

            var list = new List<byte[]>(); //数据包集
            while (true)
            {
                var packLen = BitConverter.ToInt32(buffer, offset); //数据包头(数据大小)
                if (NetByteOrder) packLen = System.Net.IPAddress.NetworkToHostOrder(packLen); //把网络字节顺序转为本地字节顺序

                if (packLen <= 0 || packLen >= MaxSize) //标识异常
                {
                    //重置有效数据前处理所有内存中的数据
                    var data = new byte[len]; Array.Copy(buffer, offset, data, 0, len);
                    if (data.Length > 0) { list.Add(data); }
                    offset = 0; len = 0; //重置有效数据游标
                    return list;
                }

                if (packLen <= len - 4) //标识正常
                {
                    var data = new byte[packLen];
                    Array.Copy(buffer, offset + 4, data, 0, packLen);
                    if (data.Length > 0) { list.Add(data); }

                    offset += packLen + 4; //数据长度和包头长度(包头长度是标识数据大小的头四个字节)
                    len -= (packLen + 4);

                    if (len <= 0) //数据有效长度为0时
                    {
                        if (size >= MaxBuffer) //超过阈值 则重置缓冲区
                        {
                            buffer = new byte[size];
                            offset = 0;
                        }
                    }
                }
                else break;
            }
            return list;
        }

        public static void DisposeObj(string marker)
        {
            if (hashTable.ContainsKey(marker))
            {
                BufferManager bm = (BufferManager)hashTable[marker];
                hashTable.Remove(marker);
                bm.Dispose(); bm = null;
            }
        }

        public void Dispose() { }
    }
}

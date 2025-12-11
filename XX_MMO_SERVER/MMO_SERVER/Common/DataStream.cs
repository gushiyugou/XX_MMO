using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    /// <summary>
    /// 数据流，采用大端模式存储
    /// </summary>
    public class DataStream : MemoryStream    
    {
        private static Queue<DataStream> pool = new Queue<DataStream>();
        public static int poolMaxCount = 200;
        


        public static DataStream Allocate()
        {
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    return pool.Dequeue();
                }
            }
            return new DataStream();
        }


        public static DataStream Allocate(byte[] bytes)
        {
            DataStream stream = Allocate();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }

        public static bool IsBigEndianSystem()
        {
            return !BitConverter.IsLittleEndian;
        }

        public static byte[] ToNetworkOrder(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian && bytes != null)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static byte[] FromNetworkOrder(byte[] bytes)
        {
            // 从网络序（大端序）转换到本机序
            if (BitConverter.IsLittleEndian && bytes != null)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public ushort ReadUShort()
        {
            byte[] bytes = new byte[2];
            this.Read(bytes,0,2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes);
        }

        public short ReadShort()
        {
            byte[] bytes = new byte[2];
            this.Read(bytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes);
        }

        public uint ReadUInt()
        {
            byte[] bytes = new byte[4];
            this.Read(bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes);
        }

        public int ReadInt()
        {
            byte[] bytes = new byte[4];
            this.Read(bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes);
        }

        public ulong ReadULong()
        {
            byte[] bytes = new byte[8];
            this.Read(bytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes);
        }

        public long ReadLong()
        {
            byte[] bytes = new byte[8];
            this.Read(bytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes);
        }

        public ushort ReadUShortBE()
        {
            return (ushort)((ReadByte() << 8) | ReadByte());
        }


        public void WriteUShort(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes,0,2);
        }

        public void WriteShort(short value)
        {

            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 2);
        }


        public void WriteUInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 4);
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 4);
        }


        public void WriteULong(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 8);
        }

        public void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 8);
        }

        protected override void Dispose(bool disposing)
        {
            //Log.Information("DataStream自动释放");
            lock (pool)
            {
                if(pool.Count < poolMaxCount)
                {
                    this.Position = 0;
                    this.SetLength(0);
                    pool.Enqueue(this);
                    //Console.WriteLine("DataStream池子长度：" + pool.Count);
                }
                else
                {
                    //递归
                    this.Dispose(disposing);
                    this.Close();
                }
            }
        }
    }
}

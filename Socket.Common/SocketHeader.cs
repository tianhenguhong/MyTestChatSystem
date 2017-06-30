using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class SocketHeader
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MessageType;
        /// <summary>
        /// 单次发送长度
        /// </summary>
        public long MessageLength;
        /// <summary>
        /// 发送包数量
        /// </summary>
        public long DataLength;
        /// <summary>
        /// 结束ID
        /// </summary>
        public Guid EndMessageID;

        /// <summary>
        /// DestinationID
        /// </summary>
        public Guid DestinationID;

        public SocketHeader(int messageType, long messageLength, long dataLength, Guid destinationID)
        {
            MessageType = messageType;
            MessageLength = messageLength;
            DataLength = dataLength;
            EndMessageID = Guid.NewGuid();
            DestinationID = destinationID;
        }

        public SocketHeader(byte[] headerBytes)
        {
            List<byte> result = headerBytes.ToList();
            MessageType = (int)BitConverter.ToUInt32(result.GetRange(0, 4).ToArray(), 0);
            MessageLength = BitConverter.ToUInt32(result.GetRange(4, 8).ToArray(), 0);
            DataLength = BitConverter.ToUInt32(result.GetRange(12, 8).ToArray(), 0);
            EndMessageID = new Guid(result.GetRange(20, 16).ToArray());
            DestinationID = new Guid(result.GetRange(36, 16).ToArray());
        }

        /// <summary>
        /// 返回36位数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(System.BitConverter.GetBytes(MessageType));
            result.AddRange(System.BitConverter.GetBytes(MessageLength));
            result.AddRange(System.BitConverter.GetBytes(DataLength));
            result.AddRange(EndMessageID.ToByteArray());
            result.AddRange(DestinationID.ToByteArray());
            return result.ToArray();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class DataMessage : BaseMessage
    {
        public List<byte> DataContainer = new List<byte>();
        public Guid MessageID = new Guid();
        public Guid DestID = new Guid();

        public override byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(base.ToBytes());
            result.AddRange(MessageID.ToByteArray());
            result.AddRange(DestID.ToByteArray());
            result.AddRange(System.BitConverter.GetBytes(DataContainer.Count));
            result.AddRange(DataContainer);
            return result.ToArray();
        }

        public override void ConvertFromBytes(byte[] datas)
        {
            List<byte> result = new List<byte>();
            result.AddRange(datas);
            base.ConvertFromBytes(datas);
            MessageID = new Guid(result.GetRange(17, 16).ToArray());
            DestID = new Guid(result.GetRange(33, 16).ToArray());
            if (MessageType == MTMessageType.DataPackage)
            {
                int dataLength = (int)BitConverter.ToUInt32(result.GetRange(49, 4).ToArray(), 0);
                DataContainer = result.GetRange(53, dataLength);
            }
        }
    }
}

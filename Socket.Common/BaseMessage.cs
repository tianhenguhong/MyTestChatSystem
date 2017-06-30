using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class BaseMessage
    {
        public Guid LogonID { get; set; }
        public MTMessageType MessageType { get; set; }

        public virtual byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            result.Add((byte)MessageType);
            result.AddRange(LogonID.ToByteArray());
            return result.ToArray();
        }

        public virtual void ConvertFromBytes(byte[] datas)
        {
            List<byte> result = new List<byte> ();
            result.AddRange(datas);
            MessageType = (MTMessageType)result[0];
            LogonID = new Guid(result.GetRange(1, 16).ToArray());
        }
    }
}

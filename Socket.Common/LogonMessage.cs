using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class LogonMessage : BaseMessage
    {
        public string Password;
        public string LogonName;
        public override byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(base.ToBytes());
            result.AddRange(MTUtil.Convert(LogonName));
            return result.ToArray();
        }

        public override void ConvertFromBytes(byte[] datas)
        {
            List<byte> result = new List<byte>();
            result.AddRange(datas);
            base.ConvertFromBytes(datas);

            if (MessageType == MTMessageType.Logon)
            {
                int logonNameLength = 0;
                LogonName = MTUtil.ReadText(result.GetRange(17, result.Count - 17), out logonNameLength);
                logonNameLength = logonNameLength + 17;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class MTUtil
    {
        public static List<byte> Convert(string text)
        {
            List<byte> result = new List<byte>();
            byte[] data = Encoding.UTF8.GetBytes(text ?? string.Empty);
            uint length = (uint)(data == null ? 0 : data.Length);
            byte[] lengthdata = System.BitConverter.GetBytes(length);
            result.AddRange(lengthdata);
            result.AddRange(data);
            return result;
        }

        public static string ReadText(List<byte> textBirray, out int maxLength)
        {
            maxLength = (int)BitConverter.ToUInt32(textBirray.GetRange(0, 4).ToArray(), 0);
            byte[] data = new byte[maxLength];            
            return Encoding.UTF8.GetString(textBirray.ToArray(), 1, maxLength).Trim();
        }
    }
}

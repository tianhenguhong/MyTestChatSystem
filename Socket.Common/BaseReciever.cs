using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTSocket
{
    public class BaseReciever
    {
        private List<byte> messageData = new List<byte>();
        private Guid id = new Guid();
        private double messageLength = 0;

        public void Copy(BaseReciever sourceReciever)
        {
            Type sourceType = sourceReciever.GetType();//获得该类的Type  
            Type thisType = this.GetType();
            FieldInfo[] sourceProperties = sourceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in sourceProperties)
            {
                object fieldValue = fieldInfo.GetValue(sourceReciever);
                string name = fieldInfo.Name;
                FieldInfo sourceInfo = thisType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if(sourceInfo != null)
                {
                    sourceInfo.SetValue(this, fieldValue);
                }
            }
        }

        public List<byte> MessageData
        {
            get
            {
                return messageData;
            }
        }

        public Guid ID
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
            }
        }

        public double MessageLength
        {
            set
            {
                messageLength = value;
            }
            get
            {
                return messageLength;
            }
        }

        public virtual object GetResult()
        {
            string result = string.Empty;
            result = Encoding.UTF8.GetString(this.messageData.ToArray(), 0, (int)this.messageLength).Trim();
            return result;
        }
    }
}

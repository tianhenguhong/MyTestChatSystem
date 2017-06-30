using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace MTSocket
{
    /// <summary>
    /// 待修改：前2位为标记位转义short,1->header 2->data 3->end
    /// </summary>
    public class ClientSocket : Socket
    {
        public Dictionary<Guid, ServerSocket> Clients = new Dictionary<Guid, ServerSocket>();
        const int FAILED = 1;
        const int CONNECTED = 2;
        const int PASSED = 3;
        const int ENDED = 4;
        public RecieveDataComplete RecieveDataCompleteAction;
        public ClientSocket(SocketInformation socketInformation) : base(socketInformation)
        { }

        public ClientSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        { }

        public ClientSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        { }
        public void RecieveServerMessage()
        {
            while (Connected)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    BeginReceive(bytes, 0, 1024, SocketFlags.None, (result) => { RecieveData(bytes, result); }, null);
                }
                catch (SocketException ex)
                {

                }

                Thread.Sleep(100);
            }
        }

        private void RecieveData(byte[] result, IAsyncResult asyResult)
        {
            BaseMessage message = null;
            switch (result[0])
            {
                case (byte)MTMessageType.Logon:
                case (byte)MTMessageType.Logout:
                    message = new LogonMessage();
                    break;
                case (byte)MTMessageType.DataPackage:
                case (byte)MTMessageType.EndMessage:
                    message = new DataMessage();
                    break;
                case (byte)MTMessageType.KeepAlive:
                    break;
                default:
                    break;
            }

            message?.ConvertFromBytes(result);
            RecieveDataCompleteAction?.Invoke(message);
        }

        public bool Send(string ipStr, int port, BaseMessage meesage)
        {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            if (!this.Connected)
            {
                this.Connect(ipe);
                Console.WriteLine("1");
            }
            Send(meesage.ToBytes());
            Console.WriteLine(meesage.MessageType.ToString());
            return true;
        }

        private bool GetSendResult()
        {
            byte[] stateBytes = new byte[1024];
            int receiveNumber = Receive(stateBytes);
            return Encoding.ASCII.GetString(stateBytes, 0, receiveNumber) == "3";
        }
    }
}

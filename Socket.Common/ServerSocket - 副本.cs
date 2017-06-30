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
    public delegate bool RecieveDataComplete(BaseMessage meesage);
    /// <summary>
    /// 待修改：前2位为标记位转义short,1->header 2->data 3->end
    /// </summary>
    public class ServerSocket : Socket
    {
        const int FAILED = 1;
        const int CONNECTED = 2;
        const int PASSED = 3;
        const int ENDED = 4;
        public RecieveDataComplete RecieveDataCompleteAction;
        public ServerSocket(SocketInformation socketInformation) : base(socketInformation)
        { }

        public ServerSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        { }

        public ServerSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        { }

        public void ListenClientConnect()
        {
            while (true)
            {
                //this.
                Socket clientSocket = Accept();
                //clientSocket.RemoteEndPoint.ToString();
                if (clientSocket.Connected)
                {
                    clientSocket.Send(Encoding.UTF8.GetBytes(CONNECTED.ToString()));
                    Thread receiveThread = new Thread(delegate () { ReceiveMessage(clientSocket); });
                    receiveThread.Start();
                }
            }
        }

        private void ReceiveMessage(Socket clientSocket)
        {
            int receiveNumber = 0;

            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    receiveNumber = clientSocket.Receive(result);
                    BaseMessage message = RecieveData(result);
                    RecieveDataCompleteAction?.Invoke(message);
                    if (message.MessageType  == MTMessageType.Logout)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private BaseMessage RecieveData(byte[] result)
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
            message.ConvertFromBytes(result);
            return message;
        }

        public bool Send(string ipStr, int port, BaseMessage meesage)
        {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            bool result = false;
            if (!this.Connected)
            {
                this.Connect(ipe);
            }
            Send(meesage.ToBytes());
            return GetSendResult();
        }

        private bool GetSendResult()
        {
            byte[] stateBytes = new byte[1024];
            int receiveNumber = Receive(stateBytes);
            Encoding.ASCII.GetString(stateBytes, 0, receiveNumber);
        }
    }
}

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
        public Dictionary<Guid, Socket> Clients = new Dictionary<Guid, Socket>();
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

        private void ReceiveMessage(Socket clientSocket)
        {
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    //clientSocket.Receive(bytes, 0, 1024, SocketFlags.None);
                    //RecieveData(bytes);
                    clientSocket.BeginReceive(bytes, 0, 1024, SocketFlags.None, (result) => { RecieveData(bytes, result, clientSocket); }, this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
                Thread.Sleep(1000);
            }
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private void RecieveData(byte[] result)
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
            Console.WriteLine(message?.MessageType.ToString());
            RecieveDataCompleteAction?.Invoke(message);
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
            Console.WriteLine(message?.MessageType.ToString());
            RecieveDataCompleteAction?.Invoke(message);
        }

        private void RecieveData(byte[] result, IAsyncResult asyResult, Socket clientSocket)
        {
            int readCount = 0;
            try
            {
                readCount = clientSocket.EndReceive(asyResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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
            if (message is LogonMessage)
            {
                Clients[(message as LogonMessage).LogonID] = clientSocket;
            }
            Console.WriteLine(message?.MessageType.ToString());
            RecieveDataCompleteAction?.Invoke(message);
            //clientSocket.Send(Encoding.UTF8.GetBytes(PASSED.ToString()));
            if (message != null && message.MessageType == MTMessageType.Logout)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        //private BaseMessage RecieveData(byte[] result, Socket client)
        //{
        //    BaseMessage message = RecieveData(result);
        //    if (message.MessageType == MTMessageType.Logon)
        //    {
        //        Clients[message.LogonID] = client as ServerSocket;
        //    }
        //    else if (message.MessageType == MTMessageType.Logout)
        //    {
        //        if (Clients.ContainsKey(message.LogonID))
        //        {
        //            Clients.Remove(message.LogonID);
        //        }
        //    }
        //    return message;
        //}

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

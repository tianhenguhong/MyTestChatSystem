using MTSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemServer.Common;

namespace FileServer
{
    class Program
    {
        static bool waiting = true;
        static void Main(string[] args)
        {
            string host = "127.0.0.1";//服务器端ip地址

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, 14004);
            RecieveService service = new RecieveService();
            service.CompleteRecieverAction = Recieve;
            ServerSocket clientSocket = new ServerSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(ipe, ConnectedCallback, clientSocket);
            while (waiting)
            {
                Thread.Sleep(5000);
            }
            clientSocket.RecieveDataCompleteAction = service.Recieve;
            Thread myThread = new Thread(clientSocket.RecieveServerMessage);
            myThread.Start();


            string sendStr = "send to server : hello,ni hao";
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            LogonMessage logonMessage = new LogonMessage();
            logonMessage.MessageType = MTMessageType.Logon;
            logonMessage.LogonID = Guid.NewGuid();
            logonMessage.LogonName = "test001";

            clientSocket.Send(host, 14004, logonMessage);


            List<byte> datas = new List<byte>();

            datas.AddRange(sendBytes);
            DataMessage dataMessage = new DataMessage();
            dataMessage.MessageType = MTMessageType.DataPackage;
            dataMessage.LogonID = Guid.NewGuid();
            dataMessage.DataContainer.AddRange(datas.GetRange(0,1));
            dataMessage.MessageID = Guid.NewGuid();
            Thread.Sleep(3000);
            clientSocket.Send(host, 14004, dataMessage);
            Thread.Sleep(3000);
            dataMessage.DataContainer = new List<byte>();
            dataMessage.DataContainer.AddRange(datas.GetRange(1, datas.Count - 1));
            clientSocket.Send(host, 14004, dataMessage);
            Thread.Sleep(3000);
            dataMessage.MessageType = MTMessageType.EndMessage;
            dataMessage.DataContainer = new List<byte>();
            clientSocket.Send(host, 14004, dataMessage);

            Console.WriteLine("quit结束");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.Equals("quit"))
                {
                    break;
                }

            }
        }

        private static void ConnectedCallback(IAsyncResult ar)
        {
            waiting = false;
        }

        internal static void Recieve(BaseReciever reciever)
        {
            Console.WriteLine(reciever.GetResult());
        }
    }
}

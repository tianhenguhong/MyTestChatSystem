using MTSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemServer.Common
{
    public class ServiceManager
    {
        List<BaseService> services = new List<BaseService>();
        Dictionary<Guid, string> logonUsers = new Dictionary<Guid, string>();

        public void Init()
        {

            //RecieveService recieveService = new RecieveService() { CompleteRecieverAction = TestRecieve };

            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, 14004);
            ServerSocket serverSocket = new ServerSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipe);

            SendService sendService = new SendService(serverSocket);
            RecieveService recieveService = new RecieveService(serverSocket)
            {
                CompleteRecieverAction = TestRecieve,
                CompleteLogonAction = TestLogon,
                CompleteRecieveMessageAction = sendService.SendMessage,
            };



            services.Add(recieveService);
            services.ForEach(service =>
            {
                try
                {
                    service.Init();
                }
                catch (Exception e)
                {
                }
            });
        }

        private void TestRecieve(BaseReciever test)
        {
            Console.WriteLine(test.GetResult());
        }

        private void TestLogon(LogonMessage logonMessge)
        {
            if (logonMessge.MessageType == MTMessageType.Logon)
            {
                logonUsers[logonMessge.LogonID] = logonMessge.LogonName;
            }
            if (logonMessge.MessageType == MTMessageType.Logout)
            {
                if (logonUsers.ContainsKey(logonMessge.LogonID))
                {
                    logonUsers.Remove(logonMessge.LogonID);
                }
            }
        }

    }
}

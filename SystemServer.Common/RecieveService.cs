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
    public delegate void CompleteReciever(BaseReciever reciever);
    public delegate void CompleteLogon(LogonMessage message);
    public delegate void CompleteRecieveMessage(DataMessage message);

    public class RecieveService : BaseService
    {
        private ServerSocket serverSocket = null;
        private RecieverCollection recvieverCollection = new RecieverCollection();

        public CompleteReciever CompleteRecieverAction;
        public CompleteLogon CompleteLogonAction;
        public CompleteRecieveMessage CompleteRecieveMessageAction;

        public RecieveService()
        { }

        public RecieveService(ServerSocket serverSocket)
        {
            this.serverSocket = serverSocket;
        }

        public override void Init()
        {
            serverSocket.RecieveDataCompleteAction += Recieve;
            serverSocket.Listen(0);
            Console.WriteLine("监听已经打开，请等待");
            Thread myThread = new Thread(serverSocket.ListenClientConnect);
            myThread.Start();

        }

        public bool Recieve(BaseMessage message)
        {
            if (message is LogonMessage)
            {
                CompleteLogonAction?.Invoke(message as LogonMessage);
            }
            else if (message is DataMessage)
            {
                DataMessage dataMessage = message as DataMessage;
                if (!recvieverCollection.Contains(dataMessage.MessageID))
                {
                    recvieverCollection.Add(new BaseReciever { ID = dataMessage.MessageID });
                }
                BaseReciever currentReciever = recvieverCollection[dataMessage.MessageID];
                if (message.MessageType == MTMessageType.DataPackage)
                {
                    CompleteRecieveMessageAction?.Invoke(message as DataMessage);
                    currentReciever.MessageData.AddRange(dataMessage.DataContainer);
                    currentReciever.MessageLength += dataMessage.DataContainer.Count;
                }
                else if (message.MessageType == MTMessageType.EndMessage)
                {
                    //写服务器记录
                    CompleteRecieveMessageAction?.Invoke(message as DataMessage);
                    currentReciever.GetResult();
                    CompleteRecieverAction?.Invoke(currentReciever);
                    //recvieverCollection.Remove(currentReciever);
                }
            }
            return true;
        }

        public override void Cloes()
        {
            try
            {
                if (serverSocket != null)
                {
                    serverSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    serverSocket.Close();
                }
            }
            catch (Exception e)
            { }
        }

    }
}

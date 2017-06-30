using MTSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SystemServer.Common
{
    public class SendService : BaseService
    {
        private ServerSocket serverSocket = null;

        public SendService(ServerSocket serverSocket)
        {
            this.serverSocket = serverSocket;
        }
        internal void SendMessage(BaseMessage message)
        {
            foreach(Socket socket in serverSocket.Clients.Values)
            {
                socket.Send(message.ToBytes());
            }
        }
    }
}

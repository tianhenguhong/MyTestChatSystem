using System;
using System.Net.Sockets;
using System.Threading;
using SystemServer.Common;

namespace SystemServer
{
    class SystemServerProgram
    {
        const int PASSED = 1;
        const int HEADERLENGTH = 36;
        static void Main(string[] args)
        {
            try
            {
                ServiceManager manager = new ServiceManager();
                manager.Init();
                System.Console.WriteLine("Input 'QUIT' to exit...");
                char s = (char)0;
                while (s != 'Q' && s != 'q')
                {
                    if (System.Console.KeyAvailable)
                        s = (char)System.Console.Read();
                    else
                        Thread.Sleep(1000);
                }

            }
            catch (Exception e)
            { }
            finally
            {

            }
        }

        private static void receiveRequest(object linstener)
        {
            TcpListener currentListener = linstener as TcpListener;
            while (true)
            {
                TcpClient cl = null;
                try
                {
                    cl = currentListener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(
                      state =>
                      {

                      }
                    );
                }
                catch (SocketException ex)
                {

                    break;
                }
                catch (Exception ex)
                {
                    if (cl != null)
                        cl.Close();
                    break;
                }
            }
        }
    }


}



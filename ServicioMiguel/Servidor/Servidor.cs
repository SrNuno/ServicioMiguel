using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    internal class Servidor
    {
        private const int port = 5005;
        public int Port { get { return port; } }
        private IPEndPoint ipep;
        public IPEndPoint IPEP
        {
            set { ipep = value; }
            get { return ipep; }
        }

        private Socket socketServer;
        public Socket ServerSocket
        {
            set { socketServer = value; }
            get { return socketServer; }
        }
        private Socket socketCliente;
        public Socket SockeCliente
        {
            set { socketCliente = value; }
            get { return socketCliente; }
        }

        private bool flag = false;

        private void Server()
        {
            IPEP = new IPEndPoint(IPAddress.Any, Port);
            Console.WriteLine("Start server");
            using (ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                while (!flag)
                {
                    try
                    {
                        ServerSocket.Bind(IPEP);
                        flag = true;
                    }
                    catch (SocketException)
                    {
                        Debug.WriteLine($"Port {IPEP.Port} in use!");
                        IPEP.Port++;
                    }
                }
            }
        }

        private void Connection()
        {
            string command;
            
        }

        static void Main(string[] args)
        {
            Servidor s = new Servidor();
            s.Server();
        }
    }
}

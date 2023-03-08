using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

namespace ServicioMiguel
{
    internal class Server
    {
        private int port;
        public int Port
        {
            set { port = value; }
            get { return port; }
        }

        private Socket socketServer;
        public Socket SocketServer
        {
            set { socketServer = value; }
            get { return socketServer; }
        }


        public IPEndPoint ie;
        Socket socketClient;
        bool hilo = true;
        private ServicioMiguel servicio;

        public Server()
        {
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void servidor()
        {
            ie = new IPEndPoint(IPAddress.Any, GetPort());

            try
            {
                SocketServer.Bind(ie);
                SocketServer.Listen(1000);
                while (hilo)
                {
                    socketClient = SocketServer.Accept();
                    Thread hilo = new Thread(conection);
                    hilo.Start(socketClient);
                    hilo.IsBackground = true;
                }
            }
            catch (SocketException)
            {
                Debug.WriteLine($"Port {ie.Port} in use!");
            }
        }

        public void conection(object socket)
        {
            string command;
            bool passwordValid = false;
            Socket cliente = (Socket)socket;
            IPEndPoint ieCliente = (IPEndPoint)cliente.RemoteEndPoint;

            using (NetworkStream ns = new NetworkStream(cliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                Console.WriteLine("New Connection");
                sw.WriteLine("Connected");
                sw.Flush();

                while (!passwordValid)
                {
                    try
                    {
                        command = sr.ReadLine();
                        if (string.IsNullOrEmpty(command))
                        {
                            Console.WriteLine("Command invalid");
                            passwordValid = true;
                        }
                        else
                        {
                            string[] text = command.Split(' ');
                            switch (text[0].Trim())
                            {
                                case "time":
                                    sw.WriteLine(DateTime.UtcNow.ToString("H:mm"));
                                    break;

                                case "date":
                                    sw.WriteLine(DateTime.UtcNow.ToString("dd-MM-yyyy"));
                                    break;

                                case "all":
                                    sw.WriteLine(DateTime.Now);
                                    break;

                                case "close":
                                    string pwdSave;
                                    try
                                    {
                                        StreamReader pwd;
                                        using (pwd = new StreamReader(Environment.GetEnvironmentVariable("PROGRAMDATA") + "\\password.txt"))
                                        {
                                            pwdSave = pwd.ReadToEnd();
                                        }
                                        if (text.Length > 1 && text[1] == pwdSave)
                                        {
                                            sw.WriteLine("\nClose operation");
                                            passwordValid = true;
                                        }
                                        else
                                        {
                                            sw.WriteLine("Password invalid");
                                        }
                                    }
                                    catch (FileNotFoundException fnfe)
                                    {
                                        Debug.Write(fnfe.Message);
                                    }
                                    break;

                                default:
                                    sw.WriteLine("Command invalid");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception");
                    }
                    sw.Flush();
                }
            }
            if (passwordValid)
            {
                cliente.Close();
            }
        }

        public int GetPort()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetEnvironmentVariable("PROGRAMDATA") + "\\config.ini"))
                {
                    bool valid = Int32.TryParse(sr.ReadLine(), out int valPort);
                    if (valPort >= 0 && valPort < 65536)
                    {
                        Port = valPort;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                servicio.writeEvent($"The file was not found: '{e}'");
                Port = 31416;
            }
            catch (DirectoryNotFoundException e)
            {
                servicio.writeEvent($"The directory was not found: '{e}'");
                Port = 31416;
            }
            catch (IOException e)
            {
                servicio.writeEvent($"The file could not be opened: '{e}'");
                Port = 31416;
            }
            return Port;
        }

        public void endSocket()
        {
            hilo = false;
            SocketServer.Close();
        }
    }
}

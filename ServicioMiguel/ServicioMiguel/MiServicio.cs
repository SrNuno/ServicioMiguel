using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioMiguel
{
    public partial class ServicioMiguel : ServiceBase
    {
        Server server;
        public ServicioMiguel()
        {
            InitializeComponent();
            server = new Server();
        }

        public void writeEvent(string mensaje)
        {
            string nombre = "ServicioMiguel";
            string logDestino = "Application";
            if (!EventLog.SourceExists(nombre))
            {
                EventLog.CreateEventSource(nombre, logDestino);
            }
            EventLog.WriteEntry(nombre, mensaje);
        }

        protected override void OnStart(string[] args)
        {
            
            Thread thread = new Thread(server.servidor);
            thread.Start();
        }

        protected override void OnStop()
        {
            writeEvent("Used port: "+server.ie.Port);
            server.endSocket();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLayer;
using WorkService;
using System.ServiceModel;

namespace ClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var callback = new callback2();
            InstanceContext context = new InstanceContext(callback);

            ServiceReference1.DocumentServiceClient c = new ServiceReference1.DocumentServiceClient(context);
            c.GetSessionId();


            //ServiceCaller.Execute<IDocumentService>(context, svc =>
            //{
                
            //    svc.GetSessionId();
            //});

            Console.ReadKey();
            c.Close();
        }
    }

    public class callback2 : ServiceReference1.IDocumentServiceCallback
    {

        public void CallbackAction(string sessionId)
        {
            Console.WriteLine(sessionId);
        }
    }

    public class ClientCallback : IDocumentCallback
    {
        public void CallbackAction(string sessionId)
        {
            Console.WriteLine(sessionId);
        }
    }
}

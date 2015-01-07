using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLayer;
using WorkService;
using System.ServiceModel;
using ServiceContract;


namespace ClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var callback = new ClientCallback();
            InstanceContext context = new InstanceContext(callback);

            ServiceCaller.Execute<IDocumentService>(context, svc =>
            {
                svc.GetUserInfo("042");
            });

          
            WSDualHttpBinding binding = new WSDualHttpBinding();
            //the Guid ensure the client call back address is unique if there are more than one client using the same base callback address
            binding.ClientBaseAddress = new Uri(string.Format("{0}{1}", "http://localhost:7799/Callback/", Guid.NewGuid().ToString()));
            using (DuplexChannelFactory<IDocumentService> channel = new DuplexChannelFactory<IDocumentService>(context, binding, new EndpointAddress("http://localhost:8008/DocumentService.svc")))
            {
                IDocumentService proxy = channel.CreateChannel();
                proxy.GetUserInfo("042");
                //Console.WriteLine("output is :" + );
                //Console.Read();
            }

            Console.ReadKey();
        }
    }
    public class ClientCallback : IDocumentCallback
    {
        public void ReturnUserInfo(Model.codeUsers user)
        {
            Console.WriteLine(user.name);
        }

        public void ReturnUserDiarys(IEnumerable<Model.viewUserDiarys> diarys)
        {
            
        }
    }
}

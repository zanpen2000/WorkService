using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLayer;
using WorkService;
using System.ServiceModel;
using ServiceContract;
using DBModel;


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


            NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Mode = SecurityMode.None;
            using (DuplexChannelFactory<IDocumentService> channel = 
                new DuplexChannelFactory<IDocumentService>(context, binding, new EndpointAddress("net.tcp://localhost:8008/DocumentService.svc")))
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
        public void ReturnUserInfo(codeUsers user)
        {
            Console.WriteLine(user.name);
        }

        public void ReturnUserDiarys(IEnumerable<viewUserDiarys> diarys)
        {
            
        }

        public event EventHandler<UserInfo> OnGetUserInfo;
    }
}

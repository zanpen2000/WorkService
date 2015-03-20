using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace AppLayer
{
    public class ServiceCaller
    {

        public static void Execute<ISvc>(InstanceContext instanceContext, Action<ISvc> ac)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string addr = __getAddress<ISvc>();
                Execute<ISvc>(addr, instanceContext, ac);
            });
        }

        public static void Execute<ISvc>(string addr, InstanceContext instanceContext, Action<ISvc> ac)
        {
            using (var factory = ChannelFactory<ISvc>(addr, instanceContext))
            {
                var proxy = factory.CreateChannel();

                try
                {
                    ac(proxy);

                }
                catch (EndpointNotFoundException ex)
                {
                    (proxy as ICommunicationObject).Abort();
                    throw ex;
                }
                catch (CommunicationException ex)
                {
                    (proxy as ICommunicationObject).Abort();
                    throw ex;
                }
                catch (TimeoutException ex)
                {
                    (proxy as ICommunicationObject).Abort();
                    throw ex;
                }
                catch (Exception ex)
                {
                    (proxy as ICommunicationObject).Close();
                    throw ex;
                }
                finally
                {
                    (proxy as ICommunicationObject).Close();
                }

            }
        }

        private static DuplexChannelFactory<ISvc> ChannelFactory<ISvc>(string addr, InstanceContext context)
        {
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.Security.Mode = WSDualHttpSecurityMode.None;
            binding.ClientBaseAddress = new Uri(string.Format("{0}{1}", "http://localhost:7799/Callback/", Guid.NewGuid().ToString()));
            return new DuplexChannelFactory<ISvc>(context, binding, new EndpointAddress(addr));
        }

        private static string __getAddress<ISvc>()
        {
            string addr = AppSettings.Get("ServiceAddress");
       

            if (string.IsNullOrEmpty(addr))
            {
                throw new ArgumentNullException("未能获取Web Service地址");
            }
            return addr;
        }

    }
}

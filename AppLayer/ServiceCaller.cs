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
        //是否使用IIS，使用IIS与否决定addr的拼接方式
        public static bool USEIIS = true;

        public static void Execute<ISvc>(InstanceContext instanceContext, Action<ISvc> ac)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string addr = __getAddress<ISvc>();
                Execute<ISvc>(addr, instanceContext, ac);
            });
        }

        public static void Execute<ISvc>(Action<ISvc> ac)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string addr = __getAddress<ISvc>();
                Execute<ISvc>(addr,  ac);
            });
        }

        private static void Execute<ISvc>(string addr, Action<ISvc> ac)
        {
            using (var factory = ChannelFactory<ISvc>(addr))
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

        private static ChannelFactory<ISvc> ChannelFactory<ISvc>(string addr)
        {
            NetTcpBinding binding = new NetTcpBinding();
            return new ChannelFactory<ISvc>(binding, new EndpointAddress(addr));
        }


        private static DuplexChannelFactory<ISvc> ChannelFactory<ISvc>(string addr, InstanceContext context)
        {
            NetTcpBinding binding = new NetTcpBinding();
            return new DuplexChannelFactory<ISvc>(context, binding, new EndpointAddress(addr));
        }

        private static string __getAddress<ISvc>()
        {
            string addr = AppSettings.Get("ServiceAddress");
            string svrName = typeof(ISvc).ToString().Split('.')[1].Substring(1);

            if (USEIIS)
                addr = "net.tcp://" + addr + "/" + svrName + ".svc";
            else
                addr = "net.tcp://" + addr + "/" + svrName + "";

            if (string.IsNullOrEmpty(addr))
            {
                throw new ArgumentNullException("未能获取Web Service地址");
            }
            return addr;
        }

    }
}

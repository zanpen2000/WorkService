using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WorkService;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost sh = new ServiceHost(typeof(WorkService.ServiceImpl));
            sh.Opened += sh_Opened;
            sh.Open();

            Console.Read();
        }

        static void sh_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("listening...");
        }
    }
}

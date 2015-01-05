using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            docsvr.DocumentServiceClient client = new docsvr.DocumentServiceClient();
            client.Open();
            
            Console.WriteLine(client.GetSessionId());

            Console.WriteLine(client.GetSessionId());
            Console.WriteLine(client.GetSessionId());

            Console.ReadKey();
            client.Close();
        }
    }
}

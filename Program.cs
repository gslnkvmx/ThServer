using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace ThServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var udpServer = new UdpClient(2004);
            var handler = new CommunicationHandler(udpServer);

            Task.Run(async () =>
            {
                while (true)
                {
                    var received = await handler.ReciveRequestAsync();
                    Console.WriteLine(received.Item1);
                    await handler.HandleRequest(received.Item1, received.Item2);
                }
            });

            Console.ReadLine();
        }
    }
}

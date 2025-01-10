using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace ThServer
{
    internal class Program
    {
        public const int SIO_UDP_CONNRESET = -1744830452;
        static void Main(string[] args)
        {

            var udpServer = new UdpClient(2004);
            udpServer.Client.IOControl(
                (IOControlCode)SIO_UDP_CONNRESET,
                new byte[] { 0, 0, 0, 0 },
                null
                );
            var handler = new CommunicationHandler(udpServer);

            Task.Run(async () =>
            {
                while (true)
                {
                    var received = await handler.ReciveRequestAsync();
                    var clientEndPoint = received.Item2;

                    Console.WriteLine(received.Item1);
                    await handler.HandleRequest(received.Item1, received.Item2);
                }
            });

            Console.ReadLine();
        }
    }
}

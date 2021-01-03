using System;
using System.Collections.Generic;
using App.Repository;
using Grpc.Core;
using Lotresult;

namespace Server
{
    internal class Server
    {
        static void Main(string[] args)
        {
            var lot = LotDB.Lot;
            var lotResultService = new LotResultService();
            var server = new Grpc.Core.Server(new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxSendMessageLength, 104857600 * 8),
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 104857600 * 8)
            })
            {
                Services = { LotService.BindService(lotResultService) },
                Ports = { new ServerPort("localhost", 50053, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Lot server listening ");
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

        }
    }
}

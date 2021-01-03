using System;
using Lotresult;
using Grpc.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            var channel = new Channel("localhost:50053", ChannelCredentials.Insecure,
                new List<ChannelOption>
                {
                    new ChannelOption(ChannelOptions.MaxSendMessageLength, 104857600 * 8),
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 104857600 * 8)
                });
            var client = new LotService.LotServiceClient(channel);
            var lotClient = new LotClient(client);
            var sw = new Stopwatch();
            sw.Start();
            var defects = lotClient.GetDefectsBulk();
            sw.Stop();
            Console.WriteLine($"Overall GetDefectsBulk call got {defects.Count} in {sw.ElapsedMilliseconds} ms");
            sw.Restart();
            var result = lotClient.GetDefectStream();
            result.Wait();
            Console.WriteLine($"Overall GetDefectStream call got {result.Result.Count} in {sw.ElapsedMilliseconds} ms");
        }
    }
}

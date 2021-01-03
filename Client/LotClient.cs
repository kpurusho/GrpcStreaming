using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lotresult;

namespace Client
{
    class LotClient
    {
        private readonly LotService.LotServiceClient _client;
        public LotClient(LotService.LotServiceClient client)
        {
            _client = client;
        }
        public List<ClientDefect> GetDefectsBulk()
        {
            var sw = new Stopwatch();
            sw.Start();
            var defects = _client.GetDefects(new DefectsRequest { WaferId = 2 });
            sw.Stop();
            Console.WriteLine($"client.GetDefectsBulk took {sw.ElapsedMilliseconds} ms");
            return defects.Defects.Select(CreateDefect).ToList();
        }

        public async Task<List<ClientDefect>> GetDefectStream()
        {
            using (var buffer = new BlockingCollection<DefectsResponse>())
            {
                using (var consumer = Task.Factory.StartNew(() => ConsumeDefects(buffer)))
                {
                    using (var defectStream = _client.GetDefectsStream(new DefectsRequest {WaferId = 2}))
                    {
                        while (await defectStream.ResponseStream.MoveNext(CancellationToken.None))
                            buffer.Add(defectStream.ResponseStream.Current);
                    }

                    buffer.CompleteAdding();
                    consumer.Wait();
                    return consumer.Result;
                }
            }
        }

        private static List<ClientDefect> ConsumeDefects(BlockingCollection<DefectsResponse> sink)
        {
            var defects = new List<ClientDefect>();
            foreach (var r in sink.GetConsumingEnumerable())
                defects.AddRange(r.Defects.Select(CreateDefect));
            return defects;
        }

        private static ClientDefect CreateDefect(Defect d)
        {
            var rand = new Random();
            var cd = new ClientDefect
            {
                Id = rand.Next(),
                X = MuToNm(d.X),
                Y = MuToNm(d.Y),
                Classcode = d.Classcode,
                Classcode1 = d.Classcode1,
                Classcode2 = d.Classcode2,
                Classcode3 = d.Classcode3,
                Classcode4 = d.Classcode4,
                Classcode5 = d.Classcode5,
                Classcode6 = d.Classcode6,
                Classcode7 = d.Classcode7,
                Classcode8 = d.Classcode8,
                Classcode9 = d.Classcode9,
                Roughcode = d.Roughcode,
            };
            cd.DynamicValues.AddRange(d.DynamicValues);
            return cd;
        }

        private static int MuToNm(int v)
        {
            return v * 1000;
        }

    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.Query;
using Grpc.Core;
using Lotresult;

namespace Server
{
    public class LotResultService : LotService.LotServiceBase
    {
        public override Task<LotResponse> GetLot(LotRequest request, ServerCallContext context)
        {
            var lot = new Query().GetLot();
            var lotResponse = new LotResponse { Id = lot.Id, Name = lot.Name };
            lotResponse.WaferIds.AddRange(lot.Wafers.Select(w => w.Id));
            return Task.FromResult(lotResponse);
        }
        public override Task<WaferResponse> GetWafer(WaferRequest request, ServerCallContext context)
        {
            var wafer = new Query().GetWafer(request.Id);
            var waferResponse = new WaferResponse { Id = wafer.Id, Name = wafer.Name };
            waferResponse.DefectIds.AddRange(wafer.Defects.Select(d => d.Id));
            return Task.FromResult(waferResponse);
        }

        public override Task<DefectsResponse> GetDefects(DefectsRequest request, ServerCallContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            var wafer = new Query().GetWafer(request.WaferId);
            var defsResponse = new DefectsResponse();
            var defCount = wafer.Defects.Count;
            for (var idx = 0; idx < defCount; idx++)
                defsResponse.Defects.Add(CreateDefect(wafer.Defects[idx]));
            sw.Stop();
            Console.WriteLine($"Data preparation time {sw.ElapsedMilliseconds} ms");
            return Task.FromResult(defsResponse);
        }

        public override async Task GetDefectsStream(DefectsRequest request,
            IServerStreamWriter<DefectsResponse> responseStream, ServerCallContext context)
        {
            using (var buffer = new BlockingCollection<DefectsResponse>())
            {
                using (var producer = Task.Factory.StartNew(() => { ProduceDefects(request.WaferId, buffer); }))
                {
                    foreach (var defectsResponse in buffer.GetConsumingEnumerable())
                        await responseStream.WriteAsync(defectsResponse);
                    producer.Wait();
                }
            }
        }
        private void ProduceDefects(int waferId, BlockingCollection<DefectsResponse> sink)
        {
            var blockSize = 100000;
            var wafer = new Query().GetWafer(waferId);
            var defsResponse = new DefectsResponse();
            var defCount = wafer.Defects.Count;
            for (var idx = 0; idx < defCount; idx++)
            {
                if (idx % blockSize == 0)
                {
                    sink.Add(defsResponse);
                    Console.WriteLine($"added {defsResponse.Defects.Count} to sink");
                    defsResponse = new DefectsResponse();
                }
                defsResponse.Defects.Add(CreateDefect(wafer.Defects[idx]));
            }
            sink.Add(defsResponse);
            Console.WriteLine($"added {defsResponse.Defects.Count} to sink");
            sink.CompleteAdding();
        }

        private static Lotresult.Defect CreateDefect(App.Data.Defect def)
        {
            var _rand = new Random();

            var defect = new Lotresult.Defect
            {
                Id = _rand.Next(), 
                X = def.X, 
                Y = def.Y, 
                Classcode = def.Classcode, 
                Classcode1 = def.Classcode1, 
                Classcode2 = def.Classcode2, 
                Classcode3 = def.Classcode2, 
                Classcode4 = def.Classcode3, 
                Classcode5 = def.Classcode4, 
                Classcode6 = def.Classcode5,
                Classcode7 = def.Classcode6,
                Classcode8 = def.Classcode7,
                Classcode9 = def.Classcode8, 
                Roughcode = def.Roughcode
            };
            defect.DynamicValues.AddRange(new List<int>{1,2,3,4});
            return defect;
        }
    }
}
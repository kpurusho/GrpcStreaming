using System.Collections.Generic;
using App.Data;

namespace App.Repository
{
    public static class LotDB
    {
        static LotDB()
        {
            var wafer1 = new Wafer
            {
                Id = 1,
                Name = "W01",
                Defects = new List<Defect>
                {
                    new Defect {Id = 1, X = 10, Y = 10, Classcode = 1, Roughcode = 2},
                    new Defect {Id = 2, X = 10, Y = 10, Classcode = 1, Roughcode = 2}
                }
            };
            var wafer2 = new Wafer
            {
                Id = 2,
                Name = "W02",
                Defects = new List<Defect>()
            };
            for (var i = 0; i < 1000000; i++)
            {
                wafer2.Defects.Add(new Defect{Id = i});
            }
            Lot = new Lot
            {
                Id = 1,
                Name = "LotusLot",
                Wafers = new List<Wafer>
                {
                    wafer1,
                    wafer2
                }
            };
        }

        public static Lot Lot { get; }
    }
}
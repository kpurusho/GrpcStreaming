using System.Collections.Generic;
using System.Linq;
using App.Data;
using App.Repository;

namespace App.Query
{
    public class Query
    {
        public Lot GetLot()
        {
            return LotDB.Lot;
        }

        public IEnumerable<Wafer> GetWafers()
        {
            return LotDB.Lot.Wafers;
        }

        public Wafer GetWafer(int id)
        {
            return LotDB.Lot.Wafers.FirstOrDefault(w => w.Id == id);
        }

        public IEnumerable<Defect> GetDefects(int waferid)
        {
            return LotDB.Lot.Wafers.FirstOrDefault(w => w.Id == waferid)?.Defects;
        }

        public Defect GetDefect(int waferid, int id)
        {
            return LotDB.Lot.Wafers.FirstOrDefault(w => w.Id == waferid)?.Defects.FirstOrDefault(d => d.Id == id);
        }
    }
}
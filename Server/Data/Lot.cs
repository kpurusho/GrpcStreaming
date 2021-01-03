using System.Collections.Generic;

namespace App.Data
{
    public class Lot
    {
        public int Id { get; set; }
        public List<Wafer> Wafers { get; set; }
        public string Name { get; set; }
    }
}
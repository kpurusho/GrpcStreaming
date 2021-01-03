using System.Collections.Generic;

namespace App.Data
{
    public class Wafer
    {
        public int Id { get; set; }
        public List<Defect> Defects { get; set; }
        public string Name { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceChannel.DTO
{
    public class InfectionsDTO
    {
        public string Date { get; set; }
        public int TotalCases { get; set; }
        public int NewCases { get; set; }
    }
}

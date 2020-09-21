using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceChannel.DTO
{
    public class RateOfChangeDTO : BaseDTO
    {
        public int RateOfChangeInCases { get; set; }
        public double PercentIncrease { get; set; }
    }
}

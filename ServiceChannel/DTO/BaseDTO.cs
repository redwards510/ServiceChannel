using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceChannel.DTO
{
    public interface IBaseDTO
    {
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double AverageDailyCases { get; set; }
        public int MinimumNumberOfCases { get; set; }
        public string MinimumNumberOfCasesDate { get; set; }
        public int MaximumNumberOfCases { get; set; }
        public string MaximumNumberOfCasesDate { get; set; }
    }
    public class BaseDTO : IBaseDTO
    {
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double AverageDailyCases { get; set; }
        public int MinimumNumberOfCases { get; set; }
        public string MinimumNumberOfCasesDate { get; set; }
        public int MaximumNumberOfCases { get; set; }
        public string MaximumNumberOfCasesDate { get; set; }
    }
}

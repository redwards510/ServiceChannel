using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceChannel.Model
{
    public class County
    {
        public County()
        {
            Infections = new List<Infections>();
        }
        [Key]
        public int ID { get; set; }
        public int UID { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Code3 { get; set; }
        public string FIPS { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Combined_Key { get; set; }
        public IEnumerable<Infections> Infections { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceChannel.Model
{
    public class Infections
    {
        public Infections()
        {
        }

        [Key]
        public int ID { get; set; }

        public int CountyID { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public int NewCases { get; set; }
    }
}

using System.Collections.Generic;

namespace ServiceChannel.DTO
{
    public class DailyBreakDownDTO : BaseDTO
    {
        public List<InfectionsDTO> DailyInfections { get; set; }
    }
}

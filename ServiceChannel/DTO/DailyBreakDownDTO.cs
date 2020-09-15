using System.Collections.Generic;

namespace ServiceChannel.DTO
{
    public class DailyBreakDownDTO : BaseDTO, IBaseDTO
    {
        public List<InfectionsDTO> DailyInfections { get; set; }
    }
}

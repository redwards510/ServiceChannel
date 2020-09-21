using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActionFilters.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceChannel.Model;
using ServiceChannel.Services;

namespace ServiceChannel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyBreakdownController : ControllerBase
    {
        private readonly ICountyService _countyService;
        private readonly IGeocodingService _geocodingService;

        public DailyBreakdownController(ICountyService countyService, IGeocodingService geocodingService)
        {
            _countyService = countyService;
            _geocodingService = geocodingService;
        }

        /// <summary>
        /// Show a daily breakdown of the number of cases per day for the given location
        /// and date range, both total cases AND new cases each day
        /// </summary>
        /// <param name="address">Address fragment that will be geocoded. </param>
        /// <param name="county">US county, minus the word "County"</param>
        /// <param name="state">US state full name</param>
        /// <param name="startDate">Beginning Date. May be omitted to search all dates</param>
        /// <param name="endDate">End Date. May be omitted to search all dates</param>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpGet]
        public ActionResult Get(string address, string county, string state, string startDate, string endDate)
        {            
            var result = _countyService.GetDailyBreakDownDto(county, state, (DateTime)HttpContext.Items["sDate"], (DateTime)HttpContext.Items["eDate"]);
            return Ok(result);
        }
    }
}

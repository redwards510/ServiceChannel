using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ActionFilters.ActionFilters;
using GuigleApi;
using GuigleApi.Models.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceChannel.DTO;
using ServiceChannel.Model;
using ServiceChannel.Services;

namespace ServiceChannel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Covid19Controller : ControllerBase
    {
        private readonly ICountyService _countyService;
        private readonly IGeocodingService _geocodingService;

        public Covid19Controller(ICountyService countyService, IGeocodingService geocodingService)
        {
            _countyService = countyService;
            _geocodingService = geocodingService;
        }
        /// <summary>
        /// Get basic COVID19 statistics about a location
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
        public IActionResult Get(string address, string county, string state, string startDate, string endDate)
        {
            
            var result = _countyService.GetBaseResultDto(county, state, (DateTime)HttpContext.Items["sDate"], (DateTime)HttpContext.Items["eDate"]);
            return Ok(result);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpGet("DailyBreakdown")]
        public ActionResult GetDailyBreakdown(string address, string county, string state, string startDate, string endDate)
        {
            var result = _countyService.GetDailyBreakDownDto(county, state, (DateTime)HttpContext.Items["sDate"], (DateTime)HttpContext.Items["eDate"]);
            return Ok(result);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpGet("RateOfChange")]
        public ActionResult GetRateOfChange(string address, string county, string state, string startDate, string endDate)
        {
            var result = _countyService.GetRateOfChangeDto(county, state, (DateTime)HttpContext.Items["sDate"], (DateTime)HttpContext.Items["eDate"]);
            return Ok(result);
        }

    }
}

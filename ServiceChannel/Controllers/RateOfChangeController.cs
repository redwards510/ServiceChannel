using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceChannel.Model;
using ServiceChannel.Services;

namespace ServiceChannel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateOfChangeController : ControllerBase
    {
        private readonly ICountyService _countyService;
        private readonly IGeocodingService _geocodingService;

        public RateOfChangeController(ICountyService countyService, IGeocodingService geocodingService)
        {
            _countyService = countyService;
            _geocodingService = geocodingService;
        }

        /// <summary>
        /// Show the rate of change (both value and percentage) for the given location and
        /// date range
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
        [HttpGet]
        public async Task<ActionResult> Get(string address, string county, string state, string startDate, string endDate)
        {
            if (!String.IsNullOrWhiteSpace(address))
            {
                var geoAddress = await _geocodingService.GeocodeAddress(address);
                county = geoAddress.County;
                state = geoAddress.State;
                if (String.IsNullOrWhiteSpace(state))
                {
                    return UnprocessableEntity("Address not found");
                }
            }
            // check for input paramters
            if ((String.IsNullOrEmpty(county) && String.IsNullOrEmpty(state)) || String.IsNullOrEmpty(state))
                return BadRequest("Please provide location parameters 'county' and 'state', or 'state'");

            // check db for valid location name
            if (!String.IsNullOrEmpty(county) && !_countyService.IsValidCounty(county, state))
            {
                return UnprocessableEntity("Please provide a valid county and state.");
            }
            else if (!_countyService.IsValidState(state))
            {
                return UnprocessableEntity("Please provide a valid state name.");
            }
            
            var sDate = DateTime.MinValue;
            var eDate = DateTime.MaxValue;
            if ((!String.IsNullOrEmpty(startDate) && !DateTime.TryParse(startDate, out sDate)) ||
                 (!String.IsNullOrEmpty(endDate) && !DateTime.TryParse(endDate, out eDate)))
            {
                return UnprocessableEntity("Invalid Date");
            }
            if (sDate > eDate)
            {
                return BadRequest("Start Date cannot be after End Date");
            }

            var result = _countyService.GetRateOfChangeDto(county, state, sDate, eDate);

            return Ok(result);
        }
    }
}

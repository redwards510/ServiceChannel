using GuigleApi;
using GuigleApi.Models.Address;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ServiceChannel.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ActionFilters.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        private readonly ICountyService _countyService;
        private readonly IGeocodingService _geocodingService;

        public ValidationFilterAttribute(ICountyService countyService, IGeocodingService geocodingService)
        {
            _countyService = countyService;
            _geocodingService = geocodingService;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            string state = String.Empty, address = String.Empty, county = String.Empty, startDate = String.Empty, endDate = String.Empty;
            if (context.ActionArguments.ContainsKey("state"))
                state = context.ActionArguments["state"] as string;
            if (context.ActionArguments.ContainsKey("address"))
                address = context.ActionArguments["address"] as string;
            if (context.ActionArguments.ContainsKey("county"))
                county = context.ActionArguments["county"] as string;
            if (context.ActionArguments.ContainsKey("endDate"))
                endDate = context.ActionArguments["endDate"] as string;
            if (context.ActionArguments.ContainsKey("startDate"))
                startDate = context.ActionArguments["startDate"] as string;            

            if (!String.IsNullOrWhiteSpace(address))
            {
                var geoAddress = await _geocodingService.GeocodeAddress(address);
                county = geoAddress.County;
                state = geoAddress.State;
                if (String.IsNullOrWhiteSpace(state))
                {
                    context.Result = new UnprocessableEntityObjectResult("Address not found");
                    return;
                }
            }
            // check for input paramters
            if ((String.IsNullOrEmpty(county) && String.IsNullOrEmpty(state)) || String.IsNullOrEmpty(state))
            {
                context.Result = new BadRequestObjectResult("Please provide location parameters 'county' and 'state', or 'state'");
                return;
            }
            // check db for valid location name
            if (!String.IsNullOrEmpty(county) && !_countyService.IsValidCounty(county, state))
            {
                context.Result = new UnprocessableEntityObjectResult("Please provide a valid county and state.");
                return;
            }
            else if (!_countyService.IsValidState(state))
            {
                context.Result = new UnprocessableEntityObjectResult("Please provide a valid state name.");
                return;
            }

            var sDate = DateTime.MinValue;
            var eDate = DateTime.MaxValue;
            if ((!String.IsNullOrEmpty(startDate) && !DateTime.TryParse(startDate, out sDate)) ||
                 (!String.IsNullOrEmpty(endDate) && !DateTime.TryParse(endDate, out eDate)))
            {
                context.Result = new UnprocessableEntityObjectResult("Invalid Date");
                return;
            }
            if (sDate > eDate)
            {
                context.Result = new BadRequestObjectResult("Start Date cannot be after End Date");
                return;
            }

            context.HttpContext.Items.Add("sDate", sDate);
            context.HttpContext.Items.Add("eDate", eDate);

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}

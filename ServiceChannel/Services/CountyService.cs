using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceChannel.DTO;
using ServiceChannel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceChannel.Services
{
    public interface ICountyService
    {
        BaseDTO GetBaseResultDto(string county, string state, DateTime sDate, DateTime eDate);
        DailyBreakDownDTO GetDailyBreakDownDto(string county, string state, DateTime sDate, DateTime eDate);
        RateOfChangeDTO GetRateOfChangeDto(string county, string state, DateTime sDate, DateTime eDate);
        bool IsValidState(string state);
        bool IsValidCounty(string county, string state);
    }

    public class CountyService : ICountyService
    {
        private readonly Covid19Context _context;
        public CountyService(Covid19Context context)
        {
            _context = context;
        }
        public BaseDTO GetBaseResultDto(string county, string state, DateTime sDate, DateTime eDate)
        {
            IEnumerable<Infections> infections = new List<Infections>();
            County theCounty = null;            

            if (!String.IsNullOrEmpty(county))
            {
                infections = GetInfections(county, state, sDate, eDate);
                theCounty = GetCounty(county, state);
            }
            else
            {
                infections = GetInfectionsByState(state, sDate, eDate);
            }

            var result = new BaseDTO();
            PopulateBaseDto(result, theCounty, infections, state);
            return result;
        }

        public DailyBreakDownDTO GetDailyBreakDownDto(string county, string state, DateTime sDate, DateTime eDate)
        {

            IEnumerable<Infections> infections = new List<Infections>();
            County theCounty = null;
            IEnumerable<InfectionsDTO> infectionsDtos = new List<InfectionsDTO>();

            if (!String.IsNullOrEmpty(county))
            {
                infections = GetInfections(county, state, sDate, eDate);
                theCounty = GetCounty(county, state);
                infectionsDtos = infections.Select(i => new InfectionsDTO { 
                                                    Date = i.Date.ToString("MM/dd/yyyy"), 
                                                    NewCases = i.NewCases, 
                                                    TotalCases = i.Count 
                                                });
            }
            else
            {
                infections = GetInfectionsByState(state, sDate, eDate);
                infectionsDtos = infections.GroupBy(d => d.Date, (key, g) =>
                                    new InfectionsDTO
                                    {
                                        Date = key.ToString("MM/dd/yyyy"),
                                        NewCases = g.Sum(n => n.NewCases),
                                        TotalCases = g.Sum(t => t.Count)
                                    });
            }

            var result = new DailyBreakDownDTO();
            PopulateBaseDto(result, theCounty, infections, state);
            result.DailyInfections = infectionsDtos.ToList();
            return result;
        }

        public RateOfChangeDTO GetRateOfChangeDto(string county, string state, DateTime sDate, DateTime eDate)
        {
            IEnumerable<Infections> infections = new List<Infections>();
            County theCounty = null;

            if (!String.IsNullOrEmpty(county))
            {
                infections = GetInfections(county, state, sDate, eDate);
                theCounty = GetCounty(county, state);
            }
            else
            {
                infections = GetInfectionsByState(state, sDate, eDate);
                infections = infections.GroupBy(d => d.Date, (key, g) =>
                                    new Infections
                                    {
                                        Date = key,
                                        NewCases = g.Sum(n => n.NewCases),
                                        Count = g.Sum(t => t.Count)
                                    });
            }

            // most recent day count - earliest day count
            var differenceBetweenDates = infections.OrderByDescending(d => d.Date).FirstOrDefault().Count - infections.OrderBy(d => d.Date).FirstOrDefault().Count;
            // difference / earliest day count * 100
            var percentageIncrease = Math.Round(Convert.ToDouble(differenceBetweenDates) / Convert.ToDouble(infections.OrderBy(d => d.Date).FirstOrDefault().Count) * 100, 2);            
            var daysTotal = infections.Select(s => s.Date).Distinct().Count();
            var rateOfChangeInCases = differenceBetweenDates / daysTotal;

            var result = new RateOfChangeDTO();
            PopulateBaseDto(result, theCounty, infections, state);
            result.RateOfChangeInCases = rateOfChangeInCases;
            result.PercentIncrease = percentageIncrease;
            return result;
        }

        /// <summary>
        /// Populate the common fields returned by all API requests
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="theCounty"></param>
        /// <param name="infections"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private void PopulateBaseDto(BaseDTO dto, County theCounty, IEnumerable<Infections> infections, string state)
        {            
            // order by date ensures we display the earliest possible date if there is a match on multiple days
            var maxInfectionDay = infections.Where(x => x.NewCases == infections.Max(i => i.NewCases)).OrderBy(d => d.Date).FirstOrDefault();
            var minInfectionDay = infections.Where(x => x.NewCases == infections.Min(i => i.NewCases)).OrderBy(d => d.Date).FirstOrDefault();

            dto.AverageDailyCases = Math.Round(infections.Average(i => Convert.ToDouble(i.Count)), 1);

            dto.Latitude = theCounty?.Lat;
            dto.Longitude = theCounty?.Long;
            dto.Location = theCounty?.Name ?? state ?? "";
            dto.MaximumNumberOfCases = maxInfectionDay.NewCases;
            dto.MaximumNumberOfCasesDate = maxInfectionDay.Date.ToString("MM/dd/yyyy");
            dto.MinimumNumberOfCases = minInfectionDay.NewCases;
            dto.MinimumNumberOfCasesDate = minInfectionDay.Date.ToString("MM/dd/yyyy");            
        }

        /// <summary>
        /// Get all infections for a state for a provided date span
        /// </summary>
        /// <param name="state"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        private IEnumerable<Infections> GetInfectionsByState(string state, DateTime sDate, DateTime eDate)
        {
            // only grab infection data from relevant dates to avoid bringing back unnecessary data
            // flatten all counties into one list of infection data because we are going to aggregate. 
            return _context.Set<County>()
                        .Include(i => i.Infections)
                        .Where(c => c.State.ToLower() == state.ToLower())
                        .SelectMany(x => x.Infections).Where(r => r.Date >= sDate && r.Date <= eDate);
        }

        /// <summary>
        /// Get all infections for a county for a provided date span
        /// </summary>
        /// <param name="county"></param>
        /// <param name="state"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        private IEnumerable<Infections> GetInfections(string county, string state, DateTime sDate, DateTime eDate)
        {
            // only grab infection data from relevant dates to avoid bringing back unnecessary data
            return _context.Set<County>()
                        .Include(i => i.Infections)
                        .FirstOrDefault(c => c.Name.ToLower() == county.ToLower() && c.State.ToLower() == state.ToLower())
                        .Infections.Where(r => r.Date >= sDate && r.Date <= eDate);
        }

        private County GetCounty(string county, string state)
        {
            return _context.Set<County>()
                        .FirstOrDefault(c => c.Name.ToLower() == county.ToLower() && c.State.ToLower() == state.ToLower());
        }

        public bool IsValidCounty(string county, string state)
        {
            return _context.Set<County>()
                    .Any(c => c.Name.ToLower().Equals(county.ToLower())
                           && c.State.ToLower().Equals(state.ToLower()));
        }

        public bool IsValidState(string state)
        {
            return _context.Set<County>()
                    .Any(c => c.State.ToLower().Equals(state.ToLower()));
        }
    }


    
}

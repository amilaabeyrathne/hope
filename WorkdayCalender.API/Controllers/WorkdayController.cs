using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WorkdayCalendar.API.Models;
using WorkdayCalendar.API.Services;

namespace WorkdayCalendar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class WorkdayController : ControllerBase
    {
        private readonly IHolidayRegistryService _holidayRegistryService;
        private readonly IWorkdaySettingsService _workdaySettingsService;
        private readonly IWorkDayCalculatorService _workdayCalculatorService;

        public WorkdayController(IHolidayRegistryService holidayRegistryService, IWorkdaySettingsService workdaySettingsService, IWorkDayCalculatorService workdayCalculatorService)
        {
            _holidayRegistryService = holidayRegistryService;
            _workdaySettingsService = workdaySettingsService;
            _workdayCalculatorService = workdayCalculatorService;
        }

        /// <summary>
        /// Registers single-date holidays. These dates will not count as working days.
        /// </summary>
        /// <remarks>
        /// **Input format (JSON body):**  
        /// - **holidays**: Array of dates in ISO 8601 format. E.g. `["2004-05-27T00:00:00"]` or `["2004-05-27"]`. Must be valid calendar dates.
        /// </remarks>
        [HttpPost("holiday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult AddHoliday([FromBody] HolidaysRequest request)
        {
            var dates = request.Holidays.Select(DateOnly.FromDateTime);
            _holidayRegistryService.AddHolidays(dates);
            return Ok();
        }

        /// <summary>
        /// Registers recurring holidays. The given month/day will be a holiday every year.
        /// </summary>
        /// <remarks>
        /// **Input format (JSON body):**  
        /// - **recurringHolidays**: Array of objects with **month** (1-12) and **day** (1 to max days in month). E.g. `[{ "month": 5, "day": 17 }]`. February accepts day 29 for leap years.
        /// </remarks>
        [HttpPost("recurring-holiday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult AddRecurringHoliday([FromBody] RecurringHolidaysRequest request)
        {
            var holidays = request.RecurringHolidays.Select(h => (h.Month, h.Day));
            _holidayRegistryService.AddRecurringHolidays(holidays);
            return Ok();
        }

        /// <summary>
        /// Sets the workday start and end times. Working hours are used for fractional day calculations.
        /// </summary>
        /// <remarks>
        /// **Input format (JSON body):**  
        /// - **start**: Workday start time as `HH:mm:ss`. E.g. `"08:00:00"`.  
        /// - **end**: Workday end time as `HH:mm:ss`. E.g. `"16:00:00"`. Must be after start.
        /// </remarks>
        [HttpPost("workday-boundary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult SetWorkdayHours([FromBody] WorkdayHoursRequest request)
        {
            _workdaySettingsService.SetWorkdayHours(request.Start, request.End);
            return Ok();
        }

        /// <summary>
        /// Calculates the resulting datetime when adding or subtracting working days from a start time.
        /// </summary>
        /// <remarks>
        /// **Input formats:**  
        /// - **start**: ISO 8601 date-time (e.g. `2004-05-24T18:05:00` or `2004-05-24T12:00:00`). Date and time required.  
        /// - **days**: Decimal number. Positive = add workdays; negative = subtract. Supports fractional workdays (e.g. `-5.5`, `8.276628`).
        /// </remarks>
        [HttpGet("calculate")]
        [ProducesResponseType(typeof(CalculateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public ActionResult<DateTime> Calculate(
            [FromQuery] [SwaggerParameter("Start date and time (ISO 8601, e.g. 2004-05-24T18:05:00)", Required = true)] DateTime start,
            [FromQuery] [SwaggerParameter("Workdays to add (decimal). Negative to subtract. E.g. -5.5, 1, 8.276628", Required = true)] decimal days)
        {
            var result = _workdayCalculatorService.CalculateWorkday(start, days);

            return Ok(new CalculateResult { Result =result});
        }
    }
}

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
        [HttpPost("holiday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult AddHoliday([FromBody] HolidaysRequest request)
        {
            _holidayRegistryService.AddHoliday(request);
            return Ok();
        }

        /// <summary>
        /// Registers recurring holidays. The given month/day will be a holiday every year.
        /// </summary>
        [HttpPost("recurring-holiday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult AddRecurringHoliday([FromBody] RecurringHolidaysRequest request)
        {
            _holidayRegistryService.AddRecurringHoliday(request);
            return Ok();
        }

        /// <summary>
        /// Sets the workday start and end times. Working hours are used for fractional day calculations.
        /// </summary>
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
        [HttpGet("calculate")]
        [ProducesResponseType(typeof(CalculateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public ActionResult<DateTime> Calculate(
            [FromQuery, SwaggerParameter("Start date and time")] DateTime start,
            [FromQuery, SwaggerParameter("Number of working days to add (negative to subtract)")] double days)
        {
            var result = _workdayCalculatorService.CalculateWorkday(start, days);

            return Ok(new CalculateResult { Result =result});
        }
    }
}

# Workday Calendar API

A .NET 9 Web API that calculates resulting datetimes when adding or subtracting working days (including fractional) from a start time. Working days exclude weekends and configurable holidays; work hours (e.g. 08:00–16:00) are configurable.

## Features

- **Add or subtract workdays** – Supports fractional workdays (e.g. `-5.5`, `8.276628`)
- **Configurable work hours** – Define start and end of the workday (e.g. 08:00–16:00)
- **Single-date holidays** – Exclude specific dates (e.g. 27 May 2004)
- **Recurring holidays** – Exclude dates that repeat every year (e.g. 17 May)
- **Seconds-based calculation** – Uses work-seconds for precise fractional-day results
- **Directional rounding** – Conservative rounding (floor when adding, ceiling when subtracting)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/workday/holiday` | Register single-date holidays |
| POST | `/api/workday/recurring-holiday` | Register recurring holidays (month/day, every year) |
| POST | `/api/workday/workday-boundary` | Set workday start and end times |
| GET | `/api/workday/calculate?start={dateTime}&days={decimal}` | Calculate result date/time |

## Example Usage

1. **Set work hours** (e.g. 08:00–16:00):

   ```http
   POST /api/workday/workday-boundary
   Content-Type: application/json

   { "start": "08:00:00", "end": "16:00:00" }
   ```

2. **Register holidays** (e.g. 17 May recurring, 27 May 2004 single):

   ```http
   POST /api/workday/recurring-holiday
   Content-Type: application/json

   { "recurringHolidays": [{ "month": 5, "day": 17 }] }
   ```

   ```http
   POST /api/workday/holiday
   Content-Type: application/json

   { "holidays": ["2004-05-27T00:00:00"] }
   ```

3. **Calculate** (e.g. 24 May 2004 18:05 minus 5.5 workdays):

   ```http
   GET /api/workday/calculate?start=2004-05-24T18:05:00&days=-5.5
   ```

   Response:

   ```json
   { "result": "2004-05-14T12:00:00" }
   ```

## Document Example Test Cases

The API is validated against the following reference examples (work hours 08:00–16:00, recurring holiday 17 May, single holiday 27 May 2004):

| Start (24 May 2004) | Workdays   | Result            |
|---------------------|------------|-------------------|
| 18:05               | -5.5       | 14 May 2004 12:00 |
| 19:03               | 44.723656  | 27 Jul 2004 13:47 |
| 18:03               | -6.7470217 | 13 May 2004 10:02 |
| 08:03               | 12.782709  | 10 Jun 2004 14:18 |
| 07:03               | 8.276628   | 4 Jun 2004 10:12  |

## Technical Notes

### Seconds-Based Calculation

Fractional workdays are converted to work-seconds to avoid minute-level rounding errors:

- 1 workday = `(StopTime - StartTime)` in seconds (e.g. 8h = 28,800 s)
- `totalWorkSeconds = startSecondsIntoDay + (workdays × workSecondsPerDay)`
- Calculation stays in seconds until the final result, which is rounded to the nearest minute for display

### Directional Rounding

For fractional minutes in the result:

- **Adding workdays**: round *down* (floor) — conservative, don’t overcount time
- **Subtracting workdays**: round *up* (ceiling) — conservative, don’t undercount time

### Negative Workdays

For negative `totalWorkSeconds`, `Math.Floor` is used so the correct number of whole days is applied (e.g. -4.5 workdays → jump 5 days back, not 4).

## Project Structure

```
WorkdayCalendar/
├── WorkdayCalender.API/          # Web API
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   │   ├── WorkDayCalculatorService.cs
│   │   ├── HolidayRegistryService.cs
│   │   └── WorkdaySettingsService.cs
│   └── Program.cs
├── WorkdayCalendar.Test/         # Unit tests (xUnit, Moq)
└── WorkdayCalendar.sln
```
## Assumptions

- **Singleton service lifetime** – `IHolidayRegistryService`, `IWorkdaySettingsService`, and `IWorkDayCalculatorService` are registered as singletons in `Program.cs`. Holiday and workday configuration is shared across all requests. For multi-tenant or per-request configuration, these would need to be scoped or transient (If a DB is used in the future).

- **In-memory configuration** - Holidays and workday settings are stored only in memory. They are lost on restart and are not persisted.

- **Configuration order** - Workday hours must be set before any calculation. There is no default; unconfigured hours throw.

- **Max consecutive non-working days** - If more than 365 consecutive non-working days are configured, the API throws.

- **Start outside work hours** – If the start time is before work starts, it is treated as the workday start; if after work ends, as the workday end. The “closest valid point” interpretation is assumed.

- **No validation of date range** – Very large days values are accepted; extreme results may be unexpected but are not explicitly rejected.

- **Directional rounding of fractional minutes** – The final result is rounded to the nearest minute using *asymmetric* rounding based on direction: **floor** when adding workdays, **ceiling** when subtracting. This avoids accidentally jumping to the next day when the computed time is very close to the workday boundary (e.g. 15:59:59.99). The assignment did not specify this behavior explicitly; it was inferred by reverse engineering the provided reference examples to match their expected outputs.




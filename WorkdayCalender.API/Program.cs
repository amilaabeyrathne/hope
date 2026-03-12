using WorkdayCalendar.API.Middleware;
using WorkdayCalendar.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHolidayRegistryService, HolidayRegistryService>();
builder.Services.AddSingleton<IWorkdaySettingsService, WorkdaySettingsService>();
builder.Services.AddSingleton<IWorkDayCalculatorService, WorkDayCalculatorService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

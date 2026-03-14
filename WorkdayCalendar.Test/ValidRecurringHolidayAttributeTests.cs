using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.Test
{
    public class ValidRecurringHolidayAttributeTests
    {
        private static bool IsValid(RecurringHoliday holiday, out ValidationResult? error)
        {
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(holiday, new ValidationContext(holiday), results, true);
            error = results.FirstOrDefault();
            return valid;
        }

        [Fact]
        public void IsValid_March28_ReturnsSuccess()
        {
            var holiday = new RecurringHoliday { Month = 3, Day = 28 };

            var valid = IsValid(holiday, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_April1_ReturnsSuccess()
        {
            var holiday = new RecurringHoliday { Month = 4, Day = 1 };

            var valid = IsValid(holiday, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_February29_ReturnsSuccess()
        {
            var holiday = new RecurringHoliday { Month = 2, Day = 29 };

            var valid = IsValid(holiday, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_February30_ReturnsError()
        {
            var holiday = new RecurringHoliday { Month = 2, Day = 30 };

            var valid = IsValid(holiday, out var error);

            Assert.False(valid);
            Assert.NotNull(error);
            Assert.Contains("29", error!.ErrorMessage!);
        }

        [Fact]
        public void IsValid_April31_ReturnsError()
        {
            var holiday = new RecurringHoliday { Month = 4, Day = 31 };

            var valid = IsValid(holiday, out var error);

            Assert.False(valid);
            Assert.NotNull(error);
            Assert.Contains("30", error!.ErrorMessage!);
        }
    }
}

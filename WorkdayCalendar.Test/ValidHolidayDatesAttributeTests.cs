using System.ComponentModel.DataAnnotations;
using WorkdayCalendar.API.Models;

namespace WorkdayCalendar.Test
{
    public class ValidHolidayDatesAttributeTests
    {
        private static bool IsValid(HolidaysRequest request, out ValidationResult? error)
        {
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);
            error = results.FirstOrDefault();
            return valid;
        }

        [Fact]
        public void IsValid_March28_ReturnsSuccess()
        {
            var request = new HolidaysRequest { Holidays = [new DateTime(2004, 3, 28)] };

            var valid = IsValid(request, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_April1_ReturnsSuccess()
        {
            var request = new HolidaysRequest { Holidays = [new DateTime(2004, 4, 1)] };

            var valid = IsValid(request, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_February29LeapYear_ReturnsSuccess()
        {
            var request = new HolidaysRequest { Holidays = [new DateTime(2024, 2, 29)] };

            var valid = IsValid(request, out _);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_March31_ReturnsSuccess()
        {
            var request = new HolidaysRequest { Holidays = [new DateTime(2004, 3, 31)] };

            var valid = IsValid(request, out _);

            Assert.True(valid);
        }
    }
}

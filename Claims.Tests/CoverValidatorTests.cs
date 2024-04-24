using Claims.Models;
using Claims.Validators;
using Xunit;

namespace Claims.Tests
{
    public class CoverValidatorTests
    {
        private readonly CoverValidator _validator = new();

        [Fact]
        public void CoverValidator_StartDateIsFuture_ShoulNotFail()
        {
            var cover = new Cover
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1)
            };

            var result = _validator.Validate(cover);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CoverValidator_StartDateIsPast_ShouldFail()
        {
            var cover = new Cover
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-1)
            };

            var result = _validator.Validate(cover);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void CoverValidator_PeriodIsWithin1Year_ShouldSucceed()
        {
            var cover = new Cover
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(300)
            };

            var result = _validator.Validate(cover);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CoverValidator_PeriodExeeds1Year_ShouldFail()
        {
            var cover = new Cover
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(400)
            };

            var result = _validator.Validate(cover);

            Assert.False(result.IsValid);
        }
    }
}

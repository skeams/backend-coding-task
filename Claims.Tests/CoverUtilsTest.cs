using Claims.Models;
using Claims.Utils;
using Xunit;

namespace Claims.Tests
{
    public class CoverUtilsTest
    {
        private const int BaseRate = 1250;
        private const decimal YachtMultiplier = 1.1m;
        private const decimal TankerMultiplier = 1.5m;

        [Fact]
        public void ComputePremium_WithinFirst30Days_NoDiscount()
        {
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(28);

            var result = CoverUtils.ComputePremium(start, end, CoverType.Yacht);
            Assert.Equal(28 * BaseRate * YachtMultiplier, result);

            var result2 = CoverUtils.ComputePremium(start, end, CoverType.Tanker);
            Assert.Equal(28 * BaseRate * TankerMultiplier, result2);
        }

        [Fact]
        public void ComputePremium_YachtWithinNext150Days_5percentDiscount()
        {
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(60);

            var result = CoverUtils.ComputePremium(start, end, CoverType.Yacht);

            const decimal yachtFirst30days = 30 * BaseRate * YachtMultiplier; // 41 250
            const decimal yacthNext30days = 30 * BaseRate * (YachtMultiplier - 0.05m); // 39 375

            Assert.Equal(yachtFirst30days + yacthNext30days, result);
        }

        [Fact]
        public void ComputePremium_TankerWithinNext150Days_3percentDiscount()
        {
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(60);

            var result = CoverUtils.ComputePremium(start, end, CoverType.Tanker);

            const decimal tankerFirst30days = 30 * BaseRate * TankerMultiplier;
            const decimal tankerNext30days = 30 * BaseRate * (TankerMultiplier - 0.02m);

            Assert.Equal(tankerFirst30days + tankerNext30days, result);
        }

        [Fact]
        public void ComputePremium_YachtAfter180Days_8percentDiscount()
        {
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(200);

            var result = CoverUtils.ComputePremium(start, end, CoverType.Yacht);

            const decimal yachtFirst30days = 30 * BaseRate * YachtMultiplier;
            const decimal yacthNext150days = 150 * BaseRate * (YachtMultiplier - 0.05m);
            const decimal yacthFinalDays = 20 * BaseRate * (YachtMultiplier - 0.08m);

            Assert.Equal(yachtFirst30days + yacthNext150days + yacthFinalDays, result);
        }

        [Fact]
        public void ComputePremium_OtherVesselAfter180Days_3percentDiscount()
        {
            var start = DateOnly.FromDateTime(DateTime.Today);
            var end = start.AddDays(200);

            var result = CoverUtils.ComputePremium(start, end, CoverType.PassengerShip);

            const decimal first30days = 30 * BaseRate * 1.2m;
            const decimal next150days = 150 * BaseRate * (1.2m - 0.02m);
            const decimal finalDays = 20 * BaseRate * (1.2m - 0.03m);

            Assert.Equal(first30days + next150days + finalDays, result);
        }
    }
}

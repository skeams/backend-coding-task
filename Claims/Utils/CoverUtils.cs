using Claims.Models;

namespace Claims.Utils
{
    public static class CoverUtils
    {
        public static decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
        {
            var premiumMultiplier = coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m
            };

            var baseRate = 1250;
            var insuranceLength = endDate.DayNumber - startDate.DayNumber;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                var discountMultiplier = 0m;

                switch (i)
                {
                    case < 30:
                        break;
                    case < 180:
                        discountMultiplier = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                        break;
                    default:
                        discountMultiplier = coverType == CoverType.Yacht ? 0.08m : 0.03m;
                        break;
                }

                totalPremium += baseRate * (premiumMultiplier - discountMultiplier);
            }

            return totalPremium;
        }
    }
}

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

            var insuranceLength = endDate.DayNumber - startDate.DayNumber;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                var discountMultiplier = 0m;

                switch (i)
                {
                    case < 30: // No discount for the first 30 days
                        break;
                    case < 180: // Some discount the next 150 days
                    {
                        discountMultiplier = 0.02m;
                        if (coverType == CoverType.Yacht)
                        {
                            discountMultiplier = 0.05m;
                        }

                        break;
                    }
                    default: // Max discount achieved after 180 days
                        {
                        discountMultiplier = 0.03m;
                        if (coverType == CoverType.Yacht)
                        {
                            discountMultiplier = 0.08m;
                        }

                        break;
                    }
                }

                totalPremium += 1250 * (premiumMultiplier - discountMultiplier);
            }

            return totalPremium;
        }
    }
}

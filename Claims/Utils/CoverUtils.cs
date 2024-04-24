using Claims.Models;

namespace Claims.Utils
{
    public static class CoverUtils
    {
        /**
         * Computes Premium based on duration and type
         *
         * BaseRate of 1250 per day with an additional percentage on top based on type:
         *  - Yacht: +10%
         *  - PassengerShip: +20%
         *  - Tanker: +50%
         *  - Other: +30%
         *
         * Premium will accumulate as described above for the first 30 days.
         * Then for the next 150, a 2% discount is applied. (or 5% in case of Yacht)
         * After day 180 the discount is increased to 3% (8% for Yacht)
         */
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Domain.Services
{
    public class PricingService : IPricingService
    {
        public decimal CalculateStayTotal(DateTime checkIn, DateTime checkOut, decimal nightlyRate)
        {
            var days = (checkOut.Date - checkIn.Date).Days;
            // Even a 1-hour stay counts as 1 day in most clinics
            int stayDuration = days <= 0 ? 1 : days;

            return stayDuration * nightlyRate;
        }

        public decimal ApplyLoyaltyDiscount(decimal originalAmount, int previousStaysCount)
        {
            if (previousStaysCount > 10) // "Gold Member" rule
                return originalAmount * 0.90m;

            return originalAmount;
        }
    }
}

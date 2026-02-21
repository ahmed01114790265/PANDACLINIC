using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.Domain.Services
{
    public interface IPricingService
    {
        // Calculates the total cost based on stay duration and the clinic's nightly rate
        decimal CalculateStayTotal(DateTime checkIn, DateTime checkOut, decimal nightlyRate);

        // Applies discounts for "Gold Members" or repeat visitors
        decimal ApplyLoyaltyDiscount(decimal originalAmount, int previousStaysCount);
    }
}

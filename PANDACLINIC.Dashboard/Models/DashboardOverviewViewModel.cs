using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Dashboard.Models
{
    public class DashboardOverviewViewModel
    {
        public int TotalAnimals { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int ScheduledAppointments { get; set; }

        public int TotalHostings { get; set; }
        public int OngoingHostings { get; set; }

        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProducts { get; set; }

        public int TotalUsers { get; set; }

        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int NewPayments { get; set; }

        public decimal TotalRevenue { get; set; }
        public int CompletedOrders { get; set; }

        public string GreetingName { get; set; } = "Admin";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}

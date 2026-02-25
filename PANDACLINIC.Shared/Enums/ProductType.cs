using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Shared.Enums
{
    public enum ProductType
    {
        DryFood= 1,
        WetFood= 2,
        Cosmetics = 3,
        Accessories = 4,
        Vaccines = 5,
        Supplements = 6,
    }
    public enum HostingStayStatus
    {
        Scheduled = 1,
        Ongoing = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum AppointmentStatus
    {
        Scheduled = 1,
        Completed = 2,
        Cancelled = 3
    }
     
    public enum AppointmentType
    {
        ClinicalExamination = 1,
        HomeExamination = 2,
        Surgery = 3,
        Grooming = 4,
        Bording = 5
    }
    public enum OrderStatus
    {
        Pending = 1,
        Completed = 2,
        Cancelled = 3
    }
    public enum PaymentStatus
    {
        New = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4
    }

    public enum UserRole
    {
        Admin = 1,
        Staff = 2,
        Customer = 3

    }
}

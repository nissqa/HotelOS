
using System;

namespace HotelOS.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Position { get; set; }

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public string Status { get; set; } = "Aktif";

        public DateTime? LeaveStartDate { get; set; }

        public DateTime? LeaveEndDate { get; set; }

        public string? LeaveType { get; set; }

        public DateTime? TerminationDate { get; set; }

        public string? TerminationReason { get; set; }

        public ICollection<CleaningTask> CleaningTasks { get; set; }
            = new List<CleaningTask>();

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
            = new List<MaintenanceRequest>();
    }
}


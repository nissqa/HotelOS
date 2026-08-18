using System;

namespace HotelOS.Models
{
    public class PoolMaintenance
    {
        public int Id { get; set; }

        public int PoolId { get; set; }

        public Pool? Pool { get; set; }

        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public string MaintenanceType { get; set; } = string.Empty;

        public string Priority { get; set; } = "Normal";

        public string? Description { get; set; }

        public string Status { get; set; } = "Bekliyor";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
    }
}
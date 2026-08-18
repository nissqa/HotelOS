using System;

namespace HotelOS.Models
{
    public class Pool
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Açık";

        public int? AssignedEmployeeId { get; set; }

        public Employee? AssignedEmployee { get; set; }

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public ICollection<PoolMaintenance> PoolMaintenances { get; set; }
            = new List<PoolMaintenance>();
    }
}
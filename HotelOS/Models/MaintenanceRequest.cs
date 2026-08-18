namespace HotelOS.Models
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int? TechnicalEquipmentId { get; set; }

        public int? EmployeeId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Priority { get; set; }

        public string Status { get; set; } = "Bekliyor";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Room? Room { get; set; }

        public TechnicalEquipment? TechnicalEquipment { get; set; }

        public Employee? Employee { get; set; }
    }
}
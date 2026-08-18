namespace HotelOS.DTOs
{
    public class MaintenancePanelDto
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int? EmployeeId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Priority { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
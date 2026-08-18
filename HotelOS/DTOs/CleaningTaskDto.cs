namespace HotelOS.DTOs
{
    public class CleaningTaskDto
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime TaskDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}
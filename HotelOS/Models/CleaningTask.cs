namespace HotelOS.Models
{
    public class CleaningTask
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime TaskDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public Room? Room { get; set; }

        public Employee? Employee { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace HotelOS.Models
{
    public class RoomOutOfService
    {
        public int Id { get; set; }

    public int RoomId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? ExpectedEndDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public Room? Room { get; set; }
    }

}

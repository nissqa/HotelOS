using System.ComponentModel.DataAnnotations;

namespace HotelOS.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public DateTime ActivityDate { get; set; }

        [Required]
        public string StartTime { get; set; } = "";

        [Required]
        public string EndTime { get; set; } = "";

        [Required]
        public string Type { get; set; } = "";

        [Required]
        public string Area { get; set; } = "";

        public string? Responsible { get; set; }

        public string Status { get; set; } = "Planlandı";

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
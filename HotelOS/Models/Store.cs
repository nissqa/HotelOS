using System.ComponentModel.DataAnnotations;

namespace HotelOS.Models
{
    public class Store
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Type { get; set; } = "";

        [Required]
        public string Status { get; set; } = "Açık";

        public decimal Rent { get; set; }

        public string Responsible { get; set; } = "";

        public string Phone { get; set; } = "";

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
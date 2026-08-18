using System;
using System.ComponentModel.DataAnnotations;

namespace HotelOS.Models
{
    public class CommonArea
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Type { get; set; } = "";

        public string Location { get; set; } = "";

        [Required]
        public string Status { get; set; } = "Temiz";

        public string Description { get; set; } = "";

        public DateTime? LastCleaningDate { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
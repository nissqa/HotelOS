using System.ComponentModel.DataAnnotations;

namespace HotelOS.Models
{
    public class TechnicalEquipment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Category { get; set; } = "";

        public string Location { get; set; } = "";

        public int? Floor { get; set; }

        [Required]
        public string Status { get; set; } = "Aktif";

        public DateTime? LastInspectionDate { get; set; }

        public DateTime? NextInspectionDate { get; set; }

        public string Description { get; set; } = "";

        public bool IsActive { get; set; } = true;

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
            = new List<MaintenanceRequest>();
    }
}
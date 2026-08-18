using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HotelOS.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad girilmesi zorunludur.")]
        public string CustomerName { get; set; } = string.Empty;

        public string? CustomerPhone { get; set; }

        [Required(ErrorMessage = "Plaka girilmesi zorunludur.")]
        [RegularExpression(
            @"^[0-9]{2} [A-ZÇĞİÖŞÜ]{1,3} [0-9]{2,4}$",
            ErrorMessage = "Plaka 34 ABC 123 formatında girilmelidir."
        )]
        public string PlateNumber { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Color { get; set; }

        public int ParkingFloor { get; set; }

        public string? ParkingLocation { get; set; }

        public DateTime ArrivalDate { get; set; } = DateTime.UtcNow;

        public DateTime? DeliveryDate { get; set; }

        public string Status { get; set; } = "Otoparkta";

        public bool CleaningRequested { get; set; } = false;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }
        public DateTime? DepartureDate { get; set; }

        public bool IsManuallyDelivered { get; set; }

        public string ComputeCurrentStatus()
        {
            if (IsManuallyDelivered)
                return "Teslim Edildi";

            if (DepartureDate.HasValue)
            {
                var today = DateTime.Today;
                var departureDay = DepartureDate.Value.Date;

                if (departureDay == today)
                    return "Teslim Bekliyor";

                if (departureDay < today)
                    return "Teslim Edildi";
            }

            return Status ?? "Otoparkta";
        }
    }
}
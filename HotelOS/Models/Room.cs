
using HotelOS.Models.Enums;

namespace HotelOS.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public int RoomTypeId { get; set; }

        public int Floor { get; set; }

        public bool IsActive { get; set; } = true;

        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public string? Description { get; set; }

        public RoomType? RoomType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
            = new List<Reservation>();

        public ICollection<CleaningTask> CleaningTasks { get; set; }
            = new List<CleaningTask>();

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
            = new List<MaintenanceRequest>();

        public ICollection<RoomOutOfService> OutOfServiceRecords { get; set; }
            = new List<RoomOutOfService>();
    }
}

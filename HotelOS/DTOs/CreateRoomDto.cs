using HotelOS.Models.Enums;

namespace HotelOS.DTOs
{
    public class CreateRoomDto
    {
        public string RoomNumber { get; set; } = string.Empty;

        public int RoomTypeId { get; set; }

        public int Floor { get; set; }

        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public string? Description { get; set; }
    }
}
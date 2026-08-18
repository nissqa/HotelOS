namespace HotelOS.DTOs
{
    public class CreateRoomTypeDto
    {
        public string TypeName { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public decimal PricePerNight { get; set; }

        public string? Description { get; set; }
    }
}
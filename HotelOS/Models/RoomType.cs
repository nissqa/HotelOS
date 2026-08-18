namespace HotelOS.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public decimal PricePerNight { get; set; }

        public string? Description { get; set; }

        // Navigation Property
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
namespace HotelOS.DTOs
{
    public class CreateReservationDto
    {
        public int CustomerId { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int AdultCount { get; set; }

        public int ChildCount { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
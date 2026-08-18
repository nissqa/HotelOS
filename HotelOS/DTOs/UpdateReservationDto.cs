namespace HotelOS.DTOs
{
    public class UpdateReservationDto
    {
        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int AdultCount { get; set; }

        public int ChildCount { get; set; }
    }
}
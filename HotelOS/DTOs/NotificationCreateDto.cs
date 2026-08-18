namespace HotelOS.DTOs
{
    public class NotificationCreateDto
    {
        public int UserId { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }
    }
}
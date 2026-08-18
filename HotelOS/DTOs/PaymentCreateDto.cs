namespace HotelOS.DTOs
{
    public class PaymentCreateDto
    {
        public int ReservationId { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }
    }
}
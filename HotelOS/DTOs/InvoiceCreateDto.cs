namespace HotelOS.DTOs
{
    public class InvoiceCreateDto
    {
        public int ReservationId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Tax { get; set; }
    }
}
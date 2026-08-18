namespace HotelOS.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }

        public string? InvoiceNo { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Tax { get; set; }

        public DateTime IssueDate { get; set; }
        public Reservation? Reservation { get; set; }
    }
}
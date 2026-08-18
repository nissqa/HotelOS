namespace HotelOS.Models
{
    public class Payment
    {
        public int Id { get; set; }

    public int ReservationId { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime DueDate { get; set; }

        public string? Status { get; set; }

        public string? TransactionNo { get; set; }

        public Reservation? Reservation { get; set; }
    }


}

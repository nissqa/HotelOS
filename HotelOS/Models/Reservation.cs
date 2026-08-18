namespace HotelOS.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int AdultCount { get; set; }

        public int ChildCount { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

    
        public Customer? Customer { get; set; }

        public Room? Room { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
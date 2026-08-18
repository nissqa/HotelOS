namespace HotelOS.DTOs
{
    public class CreateCustomerDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? IdentityNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Nationality { get; set; }

        public string? Address { get; set; }
    }
}
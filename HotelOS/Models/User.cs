namespace HotelOS.Models
{
    public class User
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public Role? Role { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
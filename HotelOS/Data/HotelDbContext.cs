using Microsoft.EntityFrameworkCore;
using HotelOS.Models;

namespace HotelOS.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RoomType> RoomTypes { get; set; }

        public DbSet<RoomOutOfService> RoomOutOfServices { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Pool> Pools { get; set; }

        public DbSet<PoolMaintenance> PoolMaintenances { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<TechnicalEquipment> TechnicalEquipments { get; set; }

        public DbSet<CommonArea> CommonAreas { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<CleaningTask> CleaningTasks { get; set; }

        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
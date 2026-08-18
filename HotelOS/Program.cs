using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        CookieAuthenticationDefaults.AuthenticationScheme;

    options.DefaultSignInScheme =
        CookieAuthenticationDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login";
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HotelOS API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "reception",
    pattern: "Reception/{action=Index}/{id?}",
    defaults: new
    {
        controller = "Reception"
    }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.MapControllers();



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<HotelDbContext>();

    context.Database.Migrate();



    var existingReceptionUser = context.Users
        .FirstOrDefault(x => x.Email == "resep@hotelos.com");

    if (existingReceptionUser != null)
    {
        existingReceptionUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingReceptionUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine("Reception þifresi BCrypt ile güncellendi.");
    }


    if (existingReceptionUser == null)
    {
        var receptionUser = new User
        {
            RoleId = 6,
            Username = "Receptionist",
            Email = "resep@hotelos.com",
            FirstName = "Resepsiyon",
            LastName = "Personeli",
            Phone = "05330000000",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("123456")
        };

        context.Users.Add(receptionUser);
        context.SaveChanges();

        Console.WriteLine(
            "Reception kullanýcýsý oluþturuldu."
        );
    }
    else
    {
   
        existingReceptionUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingReceptionUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine(
            "Reception þifresi BCrypt ile güncellendi."
        );
    }



    var existingCleaningUser = context.Users
        .FirstOrDefault(x => x.Email == "cleaning@hotelos.com");

    if (existingCleaningUser != null)
    {
        existingCleaningUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingCleaningUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine("Cleaning þifresi BCrypt ile güncellendi.");
    }


    if (existingCleaningUser == null)
    {
        var cleaningUser = new User
        {
            RoleId = 7,
            Username = "Cleaning",
            Email = "cleaning@hotelos.com",
            FirstName = "Temizlik",
            LastName = "Personeli",
            Phone = "05330000001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("123456")
        };

        context.Users.Add(cleaningUser);
        context.SaveChanges();

        Console.WriteLine(
            "Cleaning kullanýcýsý oluþturuldu."
        );
    }
    else
    {

        existingCleaningUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingCleaningUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine(
            "Cleaning þifresi BCrypt ile güncellendi."
        );
    }


    var existingCleaningEmployee = context.Employees
        .FirstOrDefault(x => x.Email == "cleaning@hotelos.com");

    if (existingCleaningEmployee == null)
    {
        var cleaningEmployee = new Employee
        {
            FirstName = "Temizlik",
            LastName = "Personeli",
            Email = "cleaning@hotelos.com",
            Phone = "05330000001",
            Position = "Cleaning",
            Salary = 25000,
            HireDate = DateTime.UtcNow,
            Status = "Aktif"
        };

        context.Employees.Add(cleaningEmployee);
        context.SaveChanges();

        Console.WriteLine(
            "Cleaning Employee kaydý oluþturuldu."
        );
    }


    var existingMaintenanceUser = context.Users
        .FirstOrDefault(x => x.Email == "maintenance@hotelos.com");

    if (existingMaintenanceUser != null)
    {
        existingMaintenanceUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingMaintenanceUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine("Maintenance þifresi BCrypt ile güncellendi.");
    }


    if (existingMaintenanceUser == null)
    {
        var maintenanceUser = new User
        {
            RoleId = 8,
            Username = "Maintenance",
            Email = "maintenance@hotelos.com",
            FirstName = "Bakým",
            LastName = "Personeli",
            Phone = "05330000002",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("123456")
        };

        context.Users.Add(maintenanceUser);
        context.SaveChanges();

        Console.WriteLine(
            "Maintenance kullanýcýsý oluþturuldu."
        );
    }
    else
    {
        
        existingMaintenanceUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingMaintenanceUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine(
            "Maintenance þifresi BCrypt ile güncellendi."
        );
    }



    var existingMaintenanceEmployee = context.Employees
        .FirstOrDefault(x => x.Email == "maintenance@hotelos.com");

    if (existingMaintenanceEmployee == null)
    {
        var maintenanceEmployee = new Employee
        {
            FirstName = "Bakým",
            LastName = "Personeli",
            Email = "maintenance@hotelos.com",
            Phone = "05330000002",
            Position = "Maintenance",
            Salary = 25000,
            HireDate = DateTime.UtcNow,
            Status = "Aktif"
        };

        context.Employees.Add(maintenanceEmployee);
        context.SaveChanges();

        Console.WriteLine(
            "Maintenance Employee kaydý oluþturuldu."
        );
    }



    var existingBoiUser = context.Users
        .FirstOrDefault(x => x.Email == "boimudur@hotelos.com");

    if (existingBoiUser != null)
    {
        existingBoiUser.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        existingBoiUser.IsActive = true;

        context.SaveChanges();

        Console.WriteLine(
            "BOI Müdür þifresi BCrypt ile güncellendi."
        );
    }
    else
    {
        var boiUser = new User
        {
            RoleId = 9,
            Username = "boimudur",
            Email = "boimudur@hotelos.com",
            FirstName = "BOI",
            LastName = "Müdür",
            Phone = "05330000003",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword("123456")
        };

        context.Users.Add(boiUser);
        context.SaveChanges();

        Console.WriteLine(
            "BOI Müdür kullanýcýsý oluþturuldu."
        );
    }




    var poolEmployees = context.Employees
        .Where(x => x.Position == "Havuz Görevlisi")
        .OrderBy(x => x.Id)
        .ToList();

    if (!context.Pools.Any())
    {
        var bigPool = new Pool
        {
            Name = "Büyük Havuz",
            Description = "Otelin ana açýk yüzme havuzu.",
            Status = "Açýk",

            AssignedEmployeeId =
                poolEmployees.Count > 0
                    ? poolEmployees[0].Id
                    : null,

            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0),

            LastMaintenanceDate =
                new DateTime(
                    2026,
                    8,
                    8,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc
                )
        };

        var childPool = new Pool
        {
            Name = "Çocuk Havuzu",
            Description = "Çocuk misafirler için ayrýlmýþ havuz.",
            Status = "Açýk",

            AssignedEmployeeId =
                poolEmployees.Count > 1
                    ? poolEmployees[1].Id
                    : null,

            OpeningTime = new TimeSpan(9, 0, 0),
            ClosingTime = new TimeSpan(20, 0, 0),

            LastMaintenanceDate =
                new DateTime(
                    2026,
                    8,
                    8,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc
                )
        };

        var indoorPool = new Pool
        {
            Name = "Kapalý Havuz",
            Description = "Kapalý alanda bulunan yüzme havuzu.",
            Status = "Bakýmda",

            AssignedEmployeeId =
                poolEmployees.Count > 2
                    ? poolEmployees[2].Id
                    : null,

            OpeningTime = new TimeSpan(10, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0),

            LastMaintenanceDate =
                new DateTime(
                    2026,
                    8,
                    10,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc
                )
        };

        context.Pools.AddRange(
            bigPool,
            childPool,
            indoorPool
        );

        context.SaveChanges();

        Console.WriteLine(
            "3 havuz kaydý oluþturuldu."
        );
    }
}

app.Run();
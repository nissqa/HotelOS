using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Security.Claims;


namespace HotelOS.Controllers
{
    public class ValePanelController : Controller
    {
        private readonly HotelDbContext _context;

        public ValePanelController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
    .Include(x => x.CreatedByUser)
    .ThenInclude(u => u.Role)
    .Where(x =>
        x.CreatedByUser != null &&
        x.CreatedByUser.Role != null &&
        x.CreatedByUser.Role.RoleName == "Vale"
    )
    .OrderByDescending(x => x.CreatedAt)
    .ToListAsync();

            bool changed = false;

            foreach (var vehicle in vehicles)
            {
                var currentStatus = vehicle.ComputeCurrentStatus();

                if (vehicle.Status != currentStatus)
                {
                    vehicle.Status = currentStatus;
                    changed = true;
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }

            await LoadTodayCustomers();

            return View(vehicles);
        }



        private async Task LoadTodayCustomers()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var todayCustomers = await _context.Reservations
                .Include(r => r.Customer)
                .Where(r =>
                    r.Customer != null &&
                    r.CheckIn >= today &&
                    r.CheckIn < tomorrow &&
                    r.Status != "Cancelled" &&
                    r.Status != "İptal"
                )
                .Select(r => r.Customer!)
                .Distinct()
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();

            ViewBag.TodayCustomers = todayCustomers;
        }



        [HttpPost]
        public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        {
      

            if (string.IsNullOrWhiteSpace(vehicle.CustomerName))
            {
                ModelState.AddModelError(
                    "CustomerName",
                    "Ad Soyad girilmesi zorunludur."
                );
            }
            else
            {
                vehicle.CustomerName = vehicle.CustomerName.Trim();

                var nameParts = vehicle.CustomerName
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                if (nameParts.Length < 2)
                {
                    ModelState.AddModelError(
                        "CustomerName",
                        "Lütfen ad ve soyad birlikte giriniz."
                    );
                }
            }



            if (string.IsNullOrWhiteSpace(vehicle.PlateNumber))
            {
                ModelState.AddModelError(
                    "PlateNumber",
                    "Plaka girilmesi zorunludur."
                );
            }
            else
            {
                vehicle.PlateNumber = vehicle.PlateNumber
                    .Trim()
                    .ToUpper();

                var platePattern =
                    @"^[0-9]{2} [A-ZÇĞİÖŞÜ]{1,3} [0-9]{2,4}$";

                if (!Regex.IsMatch(
                    vehicle.PlateNumber,
                    platePattern))
                {
                    ModelState.AddModelError(
                        "PlateNumber",
                        "Plaka 34 ABC 123 formatında girilmelidir."
                    );
                }
            }



            if (!ModelState.IsValid)
            {
                var vehicles = await _context.Vehicles
    .Include(x => x.CreatedByUser)
    .ThenInclude(u => u.Role)
    .Where(x =>
        x.CreatedByUser != null &&
        x.CreatedByUser.Role != null &&
        x.CreatedByUser.Role.RoleName == "Vale"
    )
    .OrderByDescending(x => x.CreatedAt)
    .ToListAsync();

                await LoadTodayCustomers();

                return View("Index", vehicles);
            }



            var now = DateTime.UtcNow;

            vehicle.CreatedAt = now;
            vehicle.ArrivalDate = now;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                vehicle.CreatedByUserId = userId;
            }

            if (vehicle.DepartureDate.HasValue)
            {
                vehicle.DepartureDate =
                    DateTime.SpecifyKind(
                        vehicle.DepartureDate.Value,
                        DateTimeKind.Utc
                    );
            }



            vehicle.Status = "Otoparkta";

            vehicle.IsManuallyDelivered = false;

            vehicle.DeliveryDate = null;



            _context.Vehicles.Add(vehicle);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == vehicle.Id);

            if (existingVehicle == null)
            {
                return NotFound();
            }



            if (string.IsNullOrWhiteSpace(vehicle.CustomerName))
            {
                ModelState.AddModelError(
                    "CustomerName",
                    "Ad Soyad girilmesi zorunludur."
                );
            }
            else
            {
                vehicle.CustomerName =
                    vehicle.CustomerName.Trim();

                var nameParts = vehicle.CustomerName
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                if (nameParts.Length < 2)
                {
                    ModelState.AddModelError(
                        "CustomerName",
                        "Lütfen ad ve soyad birlikte giriniz."
                    );
                }
            }



            if (string.IsNullOrWhiteSpace(vehicle.PlateNumber))
            {
                ModelState.AddModelError(
                    "PlateNumber",
                    "Plaka girilmesi zorunludur."
                );
            }
            else
            {
                vehicle.PlateNumber =
                    vehicle.PlateNumber
                        .Trim()
                        .ToUpper();

                var platePattern =
                    @"^[0-9]{2} [A-ZÇĞİÖŞÜ]{1,3} [0-9]{2,4}$";

                if (!Regex.IsMatch(
                    vehicle.PlateNumber,
                    platePattern))
                {
                    ModelState.AddModelError(
                        "PlateNumber",
                        "Plaka 34 ABC 123 formatında girilmelidir."
                    );
                }
            }


            if (!ModelState.IsValid)
            {
                var vehicles = await _context.Vehicles
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                await LoadTodayCustomers();

                return View("Index", vehicles);
            }

            existingVehicle.CustomerName =
                vehicle.CustomerName;

            existingVehicle.CustomerPhone =
                vehicle.CustomerPhone;

            existingVehicle.PlateNumber =
                vehicle.PlateNumber;

            existingVehicle.Brand =
                vehicle.Brand;

            existingVehicle.Model =
                vehicle.Model;

            existingVehicle.Color =
                vehicle.Color;

            existingVehicle.ParkingFloor =
                vehicle.ParkingFloor;

            existingVehicle.ParkingLocation =
                vehicle.ParkingLocation;

            existingVehicle.Notes =
                vehicle.Notes;



            if (vehicle.DepartureDate.HasValue)
            {
                existingVehicle.DepartureDate =
                    DateTime.SpecifyKind(
                        vehicle.DepartureDate.Value,
                        DateTimeKind.Utc
                    );
            }
            else
            {
                existingVehicle.DepartureDate = null;
            }



            existingVehicle.IsManuallyDelivered =
                vehicle.IsManuallyDelivered;


            if (vehicle.IsManuallyDelivered)
            {
                existingVehicle.Status =
                    "Teslim Edildi";

                existingVehicle.DeliveryDate =
                    DateTime.UtcNow;
            }
            else
            {
                existingVehicle.DeliveryDate =
                    null;

                existingVehicle.Status =
                    existingVehicle.ComputeCurrentStatus();
            }


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Deliver(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.IsManuallyDelivered = true;

            vehicle.Status = "Teslim Edildi";

            vehicle.DeliveryDate =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> RequestCleaning(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.CleaningRequested = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
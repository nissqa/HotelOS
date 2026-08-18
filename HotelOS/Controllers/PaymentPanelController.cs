using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Admin,Reception,Receptionist")]
    [Route("PaymentPanel")]
    public class PaymentPanelController : Controller
    {
        private readonly HotelDbContext _context;

        public PaymentPanelController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Customer)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var today = DateTime.Today;

            ViewBag.TotalPayments = payments.Sum(p => p.Amount);

          
            ViewBag.TodayPayments = payments
                .Where(p => p.PaymentDate.Date == today)
                .Sum(p => p.Amount);

        
            ViewBag.PaidPayments = payments.Count(p =>
                p.Status == "Ödendi");

       
            ViewBag.PendingPayments = payments.Count(p =>
                p.Status != "Ödendi");

            return View(payments);
        }



        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .Where(r => r.Status == "CheckedIn")
                .OrderBy(r => r.Room.RoomNumber)
                .ToListAsync();

            return View(reservations);
        }



        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int reservationId,
            decimal amount,
            string paymentMethod)
        {
       
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            
            if (reservation == null)
            {
                TempData["Error"] =
                    "Rezervasyon bulunamadı.";

                return RedirectToAction(nameof(Create));
            }


            if (reservation.Status != "CheckedIn")
            {
                TempData["Error"] =
                    "Sadece aktif konaklayan müşterilerden ödeme alınabilir.";

                return RedirectToAction(nameof(Create));
            }


            if (amount <= 0)
            {
                TempData["Error"] =
                    "Ödeme tutarı 0'dan büyük olmalıdır.";

                return RedirectToAction(nameof(Create));
            }


            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                TempData["Error"] =
                    "Lütfen ödeme yöntemi seçiniz.";

                return RedirectToAction(nameof(Create));
            }


       
            var payment = new Payment
            {
                ReservationId = reservationId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.UtcNow,
                Status = "Ödendi",


                TransactionNo =
                    Guid.NewGuid()
                        .ToString("N")
                        .ToUpper()
            };


            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();


            TempData["Success"] =
                $"{reservation.Customer?.FirstName} " +
                $"{reservation.Customer?.LastName} " +
                "için ödeme başarıyla alındı.";


            return RedirectToAction(nameof(Index));
        }


        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Customer)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (payment == null)
            {
                return NotFound();
            }

            return View("Details", payment);
        }
    }
}
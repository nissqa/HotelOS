using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class ReservationCreateController : Controller
    {
        private readonly HotelDbContext _context;

    public ReservationCreateController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadData();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRooms(
            int roomTypeId,
            DateTime? checkIn,
            DateTime? checkOut)
        {
            try
            {
                var roomsQuery = _context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r =>
                        r.IsActive &&
                        r.RoomTypeId == roomTypeId);

                if (checkIn.HasValue && checkOut.HasValue)
                {
                    var start = DateTime.SpecifyKind(
                        checkIn.Value,
                        DateTimeKind.Utc);

                    var end = DateTime.SpecifyKind(
                        checkOut.Value,
                        DateTimeKind.Utc);

                    if (end <= start)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Çıkış tarihi giriş tarihinden sonra olmalıdır."
                        });
                    }

                    var reservedRoomIds = await _context.Reservations
                        .Where(r =>
                            r.CheckIn < end &&
                            r.CheckOut > start)
                        .Select(r => r.RoomId)
                        .Distinct()
                        .ToListAsync();

                    roomsQuery = roomsQuery
                        .Where(r => !reservedRoomIds.Contains(r.Id));
                }

                var rooms = await roomsQuery
                    .OrderBy(r => r.RoomNumber)
                    .Select(r => new
                    {
                        id = r.Id,
                        roomNumber = r.RoomNumber,
                        typeName = r.RoomType!.TypeName,
                        capacity = r.RoomType.Capacity,
                        price = r.RoomType.PricePerNight
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    rooms = rooms
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "Odalar yüklenirken bir hata oluştu."
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(
            int customerId,
            int roomTypeId,
            int roomId,
            DateTime checkIn,
            DateTime checkOut,
            int adultCount,
            int childCount,
            string paymentMethod,
            int installmentCount)

        { 
            var now = DateTime.UtcNow;

            checkIn = DateTime.SpecifyKind(
                checkIn,
                DateTimeKind.Utc);

            checkOut = DateTime.SpecifyKind(
                checkOut,
                DateTimeKind.Utc);

            if (checkIn <= now)
            {
                ViewBag.Error =
                    "Giriş tarihi bugünden sonra olmalıdır.";

                await LoadData();
                return View();
            }

            if (checkOut <= checkIn)
            {
                ViewBag.Error =
                    "Çıkış tarihi giriş tarihinden sonra olmalıdır.";

                await LoadData();
                return View();
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                ViewBag.Error =
                    "Lütfen ödeme yöntemini seçiniz.";

                await LoadData();
                return View();
            }



            if (adultCount <= 0)
            {
                ViewBag.Error =
                    "En az 1 yetişkin olmalıdır.";

                await LoadData();
                return View();
            }

            if (childCount < 0)
            {
                ViewBag.Error =
                    "Çocuk sayısı negatif olamaz.";

                await LoadData();
                return View();
            }

            var customer = await _context.Customers
                .FindAsync(customerId);

            if (customer == null)
            {
                ViewBag.Error =
                    "Müşteri bulunamadı.";

                await LoadData();
                return View();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r =>
                    r.Id == roomId &&
                    r.RoomTypeId == roomTypeId &&
                    r.IsActive);

            if (room == null)
            {
                ViewBag.Error =
                    "Seçilen oda bulunamadı.";

                await LoadData();
                return View();
            }

            int totalPeople =
                adultCount + childCount;

            if (totalPeople > room.RoomType!.Capacity)
            {
                ViewBag.Error =
                    $"Bu oda en fazla {room.RoomType.Capacity} kişi alabilir.";

                await LoadData();
                return View();
            }

            bool conflict = await _context.Reservations
                .AnyAsync(r =>
                    r.RoomId == roomId &&
                    r.CheckIn < checkOut &&
                    r.CheckOut > checkIn);

            if (conflict)
            {
                ViewBag.Error =
                    "Bu oda seçilen tarihler arasında zaten rezerve edilmiş. Lütfen başka bir oda seçiniz.";

                await LoadData();
                return View();
            }

            int nightCount =
                (checkOut - checkIn).Days;

            if (nightCount <= 0)
            {
                ViewBag.Error =
                    "Rezervasyon en az 1 gece olmalıdır.";

                await LoadData();
                return View();
            }

            decimal totalPrice =
                nightCount * room.RoomType.PricePerNight;

            if (totalPrice <= 0)
            {
                ViewBag.Error =
                    "Rezervasyon toplam tutarı geçerli değil.";

                await LoadData();
                return View();
            }

            bool cashOrTransfer =
                paymentMethod == "Nakit" ||
                paymentMethod == "Havale/EFT";

            if (cashOrTransfer && installmentCount != 1)
            {
                ViewBag.Error =
                    $"{paymentMethod} ödemelerinde taksit yapılamaz. Lütfen Tek Çekim seçiniz.";

                await LoadData();
                return View();
            }

            int maxInstallments =
                CalculateMaximumInstallments(
                    now,
                    checkIn,
                    paymentMethod);

            if (installmentCount < 1 ||
                installmentCount > maxInstallments)
            {
                ViewBag.Error =
                    $"Bu rezervasyon için en fazla {maxInstallments} taksit yapılabilir.";

                await LoadData();
                return View();
            }

            var reservation = new Reservation
            {
                CustomerId = customerId,
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                AdultCount = adultCount,
                ChildCount = childCount,
                Status = "Beklemede",
                TotalPrice = totalPrice,
                CreatedAt = now
            };

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();

            var dueDates =
                CreatePaymentDates(
                    now,
                    checkIn,
                    installmentCount);

            decimal baseInstallment =
                Math.Round(
                    totalPrice / installmentCount,
                    2,
                    MidpointRounding.AwayFromZero);

            for (int i = 1; i <= installmentCount; i++)
            {
                decimal amount;

                if (i == installmentCount)
                {
                    amount =
                        totalPrice -
                        (baseInstallment * (installmentCount - 1));
                }
                else
                {
                    amount = baseInstallment;
                }

                DateTime dueDate =
                    dueDates[i - 1];

                bool firstPayment =
                    i == 1;

                string methodText;

                if (installmentCount == 1)
                {
                    methodText = paymentMethod;
                }
                else
                {
                    methodText =
                        $"{paymentMethod} - {i}. Taksit";
                }

                var payment = new Payment
                {
                    ReservationId = reservation.Id,

                    PaymentMethod = methodText,

                    Amount = amount,

                    PaymentDate = firstPayment
                        ? now
                        : dueDate,

                    DueDate = dueDate,

                    Status = firstPayment
                        ? "Ödendi"
                        : "Bekliyor",

                    TransactionNo =
                        "PAY-" +
                        now.ToString("yyyyMMddHHmmssfff") +
                        "-" +
                        i
                };

                _context.Payments.Add(payment);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Index",
                "ReservationsView");
        }

        private int CalculateMaximumInstallments(
            DateTime reservationDate,
            DateTime checkIn,
            string paymentMethod)
        {
            if (paymentMethod == "Nakit" ||
                paymentMethod == "Havale/EFT")
            {
                return 1;
            }

            if (checkIn <= reservationDate)
            {
                return 1;
            }

            int months = 0;

            DateTime temp =
                reservationDate;

            while (temp.AddMonths(1) <= checkIn)
            {
                months++;
                temp = temp.AddMonths(1);
            }

            int maximum;

            if (months <= 0)
            {
                maximum = 2;
            }
            else
            {
                maximum = months + 1;
            }

            if (maximum > 5)
            {
                maximum = 5;
            }

            return maximum;
        }

        private List<DateTime> CreatePaymentDates(
            DateTime reservationDate,
            DateTime checkIn,
            int installmentCount)
        {
            var dates =
                new List<DateTime>();

            if (installmentCount == 1)
            {
                dates.Add(reservationDate);
                return dates;
            }

            dates.Add(reservationDate);

            double totalDays =
                (checkIn - reservationDate).TotalDays;

            for (int i = 1; i < installmentCount; i++)
            {
                double ratio =
                    (double)i /
                    (installmentCount - 1);

                DateTime paymentDate =
                    reservationDate.AddDays(
                        totalDays * ratio);

                if (i == installmentCount - 1)
                {
                    paymentDate = checkIn;
                }

                dates.Add(paymentDate);
            }

            return dates;
        }

        private async Task LoadData()
        {
            ViewBag.Customers =
                await _context.Customers
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToListAsync();

            ViewBag.RoomTypes =
                await _context.RoomTypes
                    .OrderBy(r => r.TypeName)
                    .ToListAsync();

            ViewBag.Rooms =
                await _context.Rooms
                    .Include(r => r.RoomType)
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();
        }
    }


}

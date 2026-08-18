using HotelOS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Admin,Reception,Receptionist")]
    [ApiController]
    [Route("api/payments")]
    public class PaymentApiController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public PaymentApiController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Customer)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Ödeme bulunamadı."
                });
            }

            return Ok(new
            {
                success = true,
                id = payment.Id,
                customer = payment.Reservation?.Customer != null
                    ? $"{payment.Reservation.Customer.FirstName} {payment.Reservation.Customer.LastName}"
                    : "Bilinmiyor",
                room = payment.Reservation?.Room?.RoomNumber ?? "-",
                amount = payment.Amount,
                paymentMethod = payment.PaymentMethod,
                paymentDate = payment.PaymentDate.ToString("dd.MM.yyyy HH:mm"),
                transactionNo = payment.TransactionNo,
                status = payment.Status,
                reservationId = payment.ReservationId
            });
        }



        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelPayment(int id)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Ödeme bulunamadı."
                });
            }

            if (payment.Status == "İptal")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Bu ödeme zaten iptal edilmiş."
                });
            }

            payment.Status = "İptal";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Ödeme başarıyla iptal edildi."
            });
        }
    }
}
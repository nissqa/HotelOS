using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public PaymentsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    ReservationId = p.ReservationId,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Status = p.Status,
                    TransactionNo = p.TransactionNo
                })
                .ToListAsync();

            return Ok(payments);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    ReservationId = p.ReservationId,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Status = p.Status,
                    TransactionNo = p.TransactionNo
                })
                .FirstOrDefaultAsync();


            if (payment == null)
                return NotFound();


            return Ok(payment);
        }


        [HttpPost]
        public async Task<ActionResult> CreatePayment(PaymentCreateDto dto)
        {

            var reservationExists = await _context.Reservations
                .AnyAsync(r => r.Id == dto.ReservationId);


            if (!reservationExists)
                return BadRequest("Reservation bulunamadı.");


            var payment = new Payment
            {
                ReservationId = dto.ReservationId,
                PaymentMethod = dto.PaymentMethod,
                Amount = dto.Amount,
                Status = dto.Status,
                PaymentDate = DateTime.UtcNow,
                TransactionNo = Guid.NewGuid().ToString()
            };


            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Ödeme başarıyla oluşturuldu.",
                paymentId = payment.Id
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);


            if (payment == null)
                return NotFound();


            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();


            return NoContent();
        }
    }
}
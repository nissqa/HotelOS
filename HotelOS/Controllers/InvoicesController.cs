using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public InvoicesController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    ReservationId = i.ReservationId,
                    InvoiceNo = i.InvoiceNo,
                    TotalAmount = i.TotalAmount,
                    Tax = i.Tax,
                    IssueDate = i.IssueDate
                })
                .ToListAsync();

            return Ok(invoices);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Where(i => i.Id == id)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    ReservationId = i.ReservationId,
                    InvoiceNo = i.InvoiceNo,
                    TotalAmount = i.TotalAmount,
                    Tax = i.Tax,
                    IssueDate = i.IssueDate
                })
                .FirstOrDefaultAsync();


            if (invoice == null)
                return NotFound();


            return Ok(invoice);
        }



        [HttpPost]
        public async Task<IActionResult> CreateInvoice(InvoiceCreateDto dto)
        {
            var reservationExists = await _context.Reservations
                .AnyAsync(r => r.Id == dto.ReservationId);


            if (!reservationExists)
                return BadRequest("Rezervasyon bulunamadı.");



            var invoice = new Invoice
            {
                ReservationId = dto.ReservationId,
                TotalAmount = dto.TotalAmount,
                Tax = dto.Tax,
                IssueDate = DateTime.Now,
                InvoiceNo = "INV-" + Guid.NewGuid().ToString()[..8].ToUpper()
            };


            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Fatura oluşturuldu.",
                invoiceId = invoice.Id,
                invoiceNo = invoice.InvoiceNo
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices
                .FindAsync(id);


            if (invoice == null)
                return NotFound();


            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();


            return NoContent();
        }
    }
}
using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Receptionist")]
    public class CustomersController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public CustomersController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return customer;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return BadRequest("Ad boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest("Soyad boş bırakılamaz.");

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _context.Customers.AnyAsync(x => x.Email == dto.Email))
                    return BadRequest("Bu e-posta adresi zaten kayıtlı.");
            }

            if (!string.IsNullOrWhiteSpace(dto.IdentityNumber))
            {
                if (await _context.Customers.AnyAsync(x => x.IdentityNumber == dto.IdentityNumber))
                    return BadRequest("Bu kimlik numarası zaten kayıtlı.");
            }

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                IdentityNumber = dto.IdentityNumber,
                BirthDate = dto.BirthDate,
                Nationality = dto.Nationality,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
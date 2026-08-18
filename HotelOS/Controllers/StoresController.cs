using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class StoresController : Controller
    {
        private readonly HotelDbContext _context;

        public StoresController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stores = await _context.Stores
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View("~/Views/BOI/Stores.cshtml", stores);
        }

        [HttpPost]
    
        public async Task<IActionResult> Create([FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            store.CreatedAt = DateTime.UtcNow;

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return Ok(store);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] Store store)

        {
            if (id != store.Id)
            {
                return BadRequest();
            }

            var existingStore = await _context.Stores.FindAsync(id);

            if (existingStore == null)
            {
                return NotFound();
            }

            existingStore.Name = store.Name;
            existingStore.Type = store.Type;
            existingStore.Status = store.Status;
            existingStore.Rent = store.Rent;
            existingStore.Responsible = store.Responsible;
            existingStore.Phone = store.Phone;
            existingStore.OpeningTime = store.OpeningTime;
            existingStore.ClosingTime = store.ClosingTime;
            existingStore.Description = store.Description;

            await _context.SaveChangesAsync();

            return Ok(existingStore);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
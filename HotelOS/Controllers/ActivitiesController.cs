using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly HotelDbContext _context;

        public ActivitiesController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activities = await _context.Activities
                .OrderBy(a => a.ActivityDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View("~/Views/BOI/Activities.cshtml", activities);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Activity activity)
        {
            try
            {
                activity.ActivityDate = DateTime.SpecifyKind(
                    activity.ActivityDate,
                    DateTimeKind.Utc
                );

                activity.CreatedAt = DateTime.UtcNow;

                _context.Activities.Add(activity);

                await _context.SaveChangesAsync();

                return Ok(activity);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] Activity activity)
        {
            try
            {
                var existingActivity = await _context.Activities.FindAsync(id);

                if (existingActivity == null)
                {
                    return NotFound();
                }

                existingActivity.Name = activity.Name;
                existingActivity.ActivityDate = activity.ActivityDate;
                existingActivity.StartTime = activity.StartTime;
                existingActivity.EndTime = activity.EndTime;
                existingActivity.Type = activity.Type;
                existingActivity.Area = activity.Area;
                existingActivity.Responsible = activity.Responsible;
                existingActivity.Status = activity.Status;
                existingActivity.Description = activity.Description;

                await _context.SaveChangesAsync();

                return Ok(existingActivity);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            _context.Activities.Remove(activity);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
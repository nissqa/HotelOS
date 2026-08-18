using HotelOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class BOIReportsController : Controller
    {
        private readonly HotelDbContext _context;

        public BOIReportsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? department)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                employees = employees
                    .Where(e =>
                        e.FirstName.ToLower().Contains(search) ||
                        e.LastName.ToLower().Contains(search) ||
                        (e.FirstName + " " + e.LastName)
                            .ToLower()
                            .Contains(search))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(department) &&
                department != "all")
            {
                employees = employees
                    .Where(e => e.Position == department)
                    .ToList();
            }

            ViewBag.TotalEmployees =
                await _context.Employees.CountAsync();

            ViewBag.CleaningEmployees =
                await _context.Employees.CountAsync(e =>
                    e.Position == "Housekeeper" ||
                    e.Position == "Cleaning" ||
                    e.Position == "Temizlik Personeli" ||
                    e.Position == "Temizlikçi");

            ViewBag.MaintenanceEmployees =
                await _context.Employees.CountAsync(e =>
                    e.Position == "Maintenance" ||
                    e.Position == "Teknik Personel" ||
                    e.Position == "Bakım Görevlisi");

            ViewBag.PoolEmployees =
                await _context.Employees.CountAsync(e =>
                    e.Position == "Pool" ||
                    e.Position == "Havuz Görevlisi");

            ViewBag.Positions = await _context.Employees
                .Where(e => !string.IsNullOrWhiteSpace(e.Position))
                .Select(e => e.Position!)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.SelectedDepartment = department;

            return View(
                "~/Views/BOI/Reports.cshtml",
                employees
            );
        }



        [HttpGet]
        public async Task<IActionResult> EmployeeTasks(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }


            var cleaningTasks = await _context.CleaningTasks
                .Where(t => t.EmployeeId == id)
                .Select(t => new
                {
                    id = t.Id,
                    type = "Temizlik",
                    title = "Oda temizliği",
                    room = "Oda " + t.RoomId,
                    status = t.Status,
                    date = t.TaskDate,
                    notes = t.Notes
                })
                .ToListAsync();


            var maintenanceTasks = await _context.MaintenanceRequests
                .Where(m => m.EmployeeId == id)
                .Select(m => new
                {
                    id = m.Id,
                    type = "Bakım",
                    title = m.Title,
                    room = "Oda " + m.RoomId,
                    status = m.Status,
                    date = m.CreatedAt,
                    notes = m.Description
                })
                .ToListAsync();


            var tasks = cleaningTasks
                .Select(t => new
                {
                    t.id,
                    t.type,
                    t.title,
                    t.room,
                    t.status,
                    t.date,
                    t.notes
                })
                .Concat(
                    maintenanceTasks.Select(t => new
                    {
                        t.id,
                        t.type,
                        t.title,
                        t.room,
                        t.status,
                        t.date,
                        t.notes
                    })
                )
                .OrderByDescending(t => t.date)
                .ToList();


            var totalTasks = tasks.Count;

            var completedTasks = tasks.Count(t =>
                !string.IsNullOrWhiteSpace(t.status) &&
                (
                    t.status.ToLower() == "completed" ||
                    t.status.ToLower() == "tamamlandı" ||
                    t.status.ToLower() == "tamamlanan" ||
                    t.status.ToLower() == "completed"
                )
            );

            var today = DateTime.Today;

            var todayTasks = tasks.Count(t =>
                t.date.Date == today
            );


            return Json(new
            {
                totalTasks,
                completedTasks,
                todayTasks,
                tasks
            });
        }
    }
}
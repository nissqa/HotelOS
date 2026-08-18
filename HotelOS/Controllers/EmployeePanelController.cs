using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class EmployeePanelController : Controller
    {
        private readonly HotelDbContext _context;

        public EmployeePanelController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.CleaningTasks)
                    .ThenInclude(t => t.Room)
                .ToListAsync();

            employees = employees
                .OrderBy(e =>
                    e.Status == "Aktif" ? 1 :
                    e.Status == "İzinli" ? 2 :
                    e.Status == "İşten Ayrıldı" ? 3 : 4)
                .ThenBy(e => e.FirstName)
                .ToList();

            ViewBag.TotalEmployees = employees.Count;

            ViewBag.ActiveEmployees = employees.Count(e =>
                e.Status == "Aktif");

            ViewBag.HousekeepingEmployees = employees.Count(e =>
                !string.IsNullOrWhiteSpace(e.Position) &&
                e.Position.Contains("temizlik", StringComparison.OrdinalIgnoreCase));

            ViewBag.OnLeaveEmployees = employees.Count(e =>
                e.Status == "İzinli");

            ViewBag.TerminatedEmployees = employees.Count(e =>
                e.Status == "İşten Ayrıldı");

            return View(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(employee.Status))
            {
                employee.Status = "Aktif";
            }

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SetLeave(
       int id,
       DateTime leaveStartDate,
       DateTime leaveEndDate,
       string? leaveType)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            leaveStartDate = DateTime.SpecifyKind(
                leaveStartDate,
                DateTimeKind.Utc);

            leaveEndDate = DateTime.SpecifyKind(
                leaveEndDate,
                DateTimeKind.Utc);

            employee.Status = "İzinli";
            employee.LeaveStartDate = leaveStartDate;
            employee.LeaveEndDate = leaveEndDate;
            employee.LeaveType = leaveType;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EndLeave(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Status = "Aktif";
            employee.LeaveStartDate = null;
            employee.LeaveEndDate = null;
            employee.LeaveType = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Terminate(
    int id,
    DateTime terminationDate,
    string? terminationReason)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            terminationDate = DateTime.SpecifyKind(
                terminationDate,
                DateTimeKind.Utc);

            employee.Status = "İşten Ayrıldı";
            employee.TerminationDate = terminationDate;
            employee.TerminationReason = terminationReason;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.CleaningTasks)
                    .ThenInclude(t => t.Room)
                        .ThenInclude(r => r.Reservations)
                            .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
    }
}
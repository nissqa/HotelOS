using System.Collections.Generic;

namespace HotelOS.Models
{
    public class CommonAreasViewModel
    {
        public List<CommonArea> Areas { get; set; } = new();

        public List<Employee> CleaningEmployees { get; set; } = new();

        public List<Employee> MaintenanceEmployees { get; set; } = new();
    }
}
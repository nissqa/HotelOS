using HotelOS.Models;

namespace HotelOS.Models.ViewModels
{
    public class CheckInViewModel
    {
        public List<Reservation> TodayReservations { get; set; } = new();

        public List<Reservation> PendingCheckIns { get; set; } = new();

        public List<Reservation> CompletedCheckIns { get; set; } = new();

        public int TotalGuests =>
            TodayReservations.Sum(x => x.AdultCount + x.ChildCount);
    }
}
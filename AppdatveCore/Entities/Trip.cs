using AppdatveCore.Enums;

namespace AppdatveCore.Entities;

public class Trip
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public virtual Bus Bus { get; set; } = null!;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string BookedSeats { get; set; } = ""; //"A1,A2"
}
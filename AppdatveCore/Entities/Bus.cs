using AppdatveCore.Enums;

namespace AppdatveCore.Entities;

public class Bus
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public BusType Type { get; set; }
    public int TotalSeats { get; set; }
    public int BusCompanyId { get; set; }
    public virtual BusCompany BusCompany { get; set; } = null!;
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppdatveInfrastructure.Data;
using AppdatveCore.Enums;

[ApiController]
[Route("api/[controller]")]
public class TripsController(AppDbContext context) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime date, [FromQuery] int? companyId = null)
    {
        // 1. Tạo query cơ bản
        var query = context.Trips
            .Include(t => t.Bus)
            .ThenInclude(b => b.BusCompany) // Lấy tên hãng xe
            .Where(t => t.From == from 
                     && t.To == to 
                     && t.DepartureTime.Date == date.Date);

        // 2. Nếu người dùng chọn lọc theo hãng xe thì mới lọc
        if (companyId.HasValue)
        {
            query = query.Where(t => t.Bus.BusCompanyId == companyId);
        }

        // 3. Chọn lọc các trường cần thiết (Tránh trả về thừa dữ liệu)
        var trips = await query
            .Select(t => new {
                t.Id,
                CompanyName = t.Bus.BusCompany.Name,
                FromLocation = t.From,
                ToLocation = t.To,
                StartTime = t.DepartureTime,
                Price = t.Price,
                AvailableSeats = t.AvailableSeats,
                BusType = t.Bus.Type.ToString()// Enum chuyển sang String (vd: Limousine)
            })
            .ToListAsync();

        return Ok(trips);
    }
}
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
        // 1. Chuẩn hóa dữ liệu đầu vào: Loại bỏ khoảng trắng thừa
        string fromLocation = from?.Trim() ?? "";
        string toLocation = to?.Trim() ?? "";

        // 2. Tạo query cơ bản
        var query = context.Trips
            .Include(t => t.Bus)
            .ThenInclude(b => b.BusCompany)
            .AsQueryable();

        // 3. Lọc theo điểm đi/đến (Sử dụng ToLower để so sánh không phân biệt hoa thường)
        // Lưu ý: Cột From và To trong DB của bạn là nvarchar(MAX) nên hỗ trợ tốt tiếng Việt.
        query = query.Where(t => t.From.ToLower() == fromLocation.ToLower() 
                              && t.To.ToLower() == toLocation.ToLower());

        // 4. Lọc theo ngày (Bỏ qua giờ phút giây để khớp với tìm kiếm 25/12/2025)
        query = query.Where(t => t.DepartureTime.Date == date.Date);

        // 5. Nếu có companyId (lớn hơn 0) thì mới lọc theo hãng
        if (companyId.HasValue && companyId > 0)
        {
            query = query.Where(t => t.Bus.BusCompanyId == companyId);
        }

        // 6. Chuyển đổi dữ liệu sang DTO để trả về Flutter
        var trips = await query
            .Select(t => new {
                t.Id,
                CompanyName = t.Bus.BusCompany.Name,
                FromLocation = t.From,
                ToLocation = t.To,
                // Flutter dùng DateTime.parse nên trả về Full ISO String
                StartTime = t.DepartureTime, 
                Price = t.Price,
                AvailableSeats = t.AvailableSeats,
                // Lấy TotalSeats từ bảng Buses để dùng cho SeatSelection
                TotalSeats = t.Bus.TotalSeats, 
                // Chuyển Enum sang chuỗi mô tả
                BusType = t.Bus.Type.ToString() 
            })
            .ToListAsync();

        return Ok(trips);
    }
}
using System.Security.Claims;
using AppdatveCore.Entities;
using AppdatveInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppdatveAPI.Controllers;

[Authorize] // Bắt buộc phải đăng nhập mới được đặt vé
[ApiController]
[Route("api/[controller]")]
public class BookingsController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Lấy lịch sử đặt vé của người dùng hiện tại
    /// </summary>
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var history = await context.Bookings
            .Include(b => b.Trip)
                .ThenInclude(t => t.Bus)
                    .ThenInclude(bus => bus.BusCompany)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(history);
    }

    /// <summary>
    /// Logic đặt vé chính: Kiểm tra ghế trống và khóa giao dịch
    /// </summary>
    [HttpPost("book")]
    public async Task<IActionResult> BookTicket([FromBody] BookingRequestDto req)
    {
        if (string.IsNullOrEmpty(req.SelectedSeats))
            return BadRequest("Vui lòng chọn ít nhất một ghế.");

        // 1. Khởi tạo Transaction để đảm bảo an toàn dữ liệu khi có nhiều người đặt cùng lúc
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 2. Tìm chuyến xe
            var trip = await context.Trips
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.Id == req.TripId);

            if (trip == null) return NotFound("Chuyến xe không tồn tại.");

            // 3. Xử lý chuỗi ghế: Tách danh sách ghế đã đặt và ghế đang chọn
            var alreadyBooked = trip.BookedSeats
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            var requestedSeats = req.SelectedSeats
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            // 4. KIỂM TRA TRÙNG LẶP (Race Condition Check)
            var overlap = requestedSeats.Intersect(alreadyBooked).Any();
            if (overlap)
            {
                return BadRequest("Một hoặc nhiều ghế bạn chọn vừa có người khác đặt. Vui lòng chọn ghế khác.");
            }

            // 5. Cập nhật dữ liệu chuyến xe
            alreadyBooked.AddRange(requestedSeats);
            trip.BookedSeats = string.Join(",", alreadyBooked);
            trip.AvailableSeats -= requestedSeats.Count;

            // 6. Tạo đơn đặt vé mới
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = new Booking
            {
                TripId = req.TripId,
                UserId = userId!,
                SelectedSeats = string.Join(",", requestedSeats),
                TotalAmount = trip.Price * requestedSeats.Count,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);
            
            // 7. Lưu tất cả thay đổi
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { 
                Message = "Đặt vé thành công!", 
                BookingId = booking.Id,
                Seats = booking.SelectedSeats,
                Total = booking.TotalAmount 
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO hứng dữ liệu từ Flutter gửi lên
/// </summary>
public class BookingRequestDto
{
    public int TripId { get; set; }
    public string SelectedSeats { get; set; } = string.Empty; // VD: "A1, A2"
}
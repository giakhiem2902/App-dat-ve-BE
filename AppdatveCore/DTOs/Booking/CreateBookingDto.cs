public class CreateBookingDto
{
    public int TripId { get; set; }
    public string SelectedSeats { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    // Thêm thông tin liên hệ nếu bạn muốn lưu vào bảng Booking
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
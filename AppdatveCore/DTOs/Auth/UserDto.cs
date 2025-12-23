namespace AppdatveCore.DTOs.Auth;

// DTO dùng cho Đăng ký
public class RegisterDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
}

// DTO dùng để trả về thông tin User sau khi đăng nhập (Không trả về Password)
public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
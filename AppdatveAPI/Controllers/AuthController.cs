using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppdatveCore.Entities;
using AppdatveInfrastructure.Services;
using AppdatveCore.DTOs.Auth;

[ApiController]
[Route("api/[controller]")]
// Sử dụng Primary Constructor để inject các dịch vụ cần thiết
public class AuthController(
    UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    TokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var user = new ApplicationUser { 
            UserName = req.Email, 
            Email = req.Email, 
            FullName = req.FullName, 
            PhoneNumber = req.PhoneNumber 
        };

        var result = await userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // Kiểm tra và gán quyền mặc định là "User"
        // Lưu ý: Tên Role phải khớp chính xác với bản ghi trong AspNetRoles
        if (await roleManager.RoleExistsAsync("User"))
        {
            await userManager.AddToRoleAsync(user, "User");
        }

        return Ok(new { message = "Đăng ký thành công" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // Sử dụng biến trực tiếp từ Primary Constructor (không dùng dấu gạch dưới)
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null) return Unauthorized("Email hoặc mật khẩu không chính xác.");

        // Kiểm tra mật khẩu thông qua SignInManager
        var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        
        if (!result.Succeeded) return Unauthorized("Email hoặc mật khẩu không chính xác.");

        // Lấy danh sách Role để tích hợp vào JWT Token
        var roles = await userManager.GetRolesAsync(user);

        // Tạo Token chứa Claims về Email và Roles
        var token = tokenService.CreateToken(user, roles);

        return Ok(new { 
            token = token, 
            email = user.Email,
            roles = roles 
        });
    }
}

// DTO used by the Login endpoint (defined here to resolve missing type error)
public record LoginDto
{
    public string Email { get; init; }
    public string Password { get; init; }
}
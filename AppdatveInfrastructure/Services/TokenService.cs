using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppdatveCore.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AppdatveInfrastructure.Services;

// Sử dụng Primary Constructor giúp code ngắn gọn
public class TokenService(IConfiguration config)
{
    public string CreateToken(ApplicationUser user, IList<string> roles)
    {
        // 1. Tạo danh sách Claims (Thông tin chứa trong Token)
        var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id), // Dùng NameId chuẩn JWT
            new Claim(ClaimTypes.Name, user.FullName)
        };

        // 2. Duyệt qua danh sách Roles để thêm vào Claim
        // Giúp [Authorize(Roles = "Admin")] trong Controller hoạt động được
        foreach (var role in roles) 
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 3. Lấy Secret Key từ appsettings.json
        // Đảm bảo Key này đủ độ dài (ít nhất 32-64 ký tự)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
        
        // Sử dụng thuật toán HmacSha512 để bảo mật tốt hơn
       var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Thiết kế cấu trúc Token
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), // Hạn dùng 7 ngày
            SigningCredentials = creds,
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"]
        };

        // 5. Khởi tạo và trả về chuỗi Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}
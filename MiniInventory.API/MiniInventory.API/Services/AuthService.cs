using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniInventory.API.Data;
using MiniInventory.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MiniInventory.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Tiêm cả Kho dữ liệu và File cấu hình (để lấy key bí mật) vào đây
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(User loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginModel.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.PasswordHash, user.PasswordHash))
            {
                throw new Exception("Sai tài khoản hoặc mật khẩu!"); // Ném lỗi ra ngoài
            }

            // Tạo Token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Trả về chuỗi Token
        }

        public async Task<string> RegisterAsync(User registerModel)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerModel.Username))
            {
                throw new Exception("Tên tài khoản này đã tồn tại trong hệ thống!");
            }

            registerModel.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.PasswordHash);

            if (string.IsNullOrEmpty(registerModel.Role))
            {
                registerModel.Role = "Admin";
            }

            _context.Users.Add(registerModel);
            await _context.SaveChangesAsync();

            return "Đăng ký thành công! Hãy quay lại API Login để thử lại với tài khoản này.";
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using MiniInventory.API.Models;
using MiniInventory.API.Services;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // CHỈ GỌI AUTH SERVICE
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginModel)
        {
            try
            {
                var token = await _authService.LoginAsync(loginModel);
                return Ok(new { token = token }); // Trả về Token cho Angular
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message); // Sai pass thì báo lỗi 401
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User registerModel)
        {
            try
            {
                var message = await _authService.RegisterAsync(registerModel);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trùng tên thì báo lỗi 400
            }
        }
    }
}
using MiniInventory.API.Models;

namespace MiniInventory.API.Services
{
    public interface IAuthService
    {
        // Nhận vào thông tin User, trả về chuỗi Token (string)
        Task<string> LoginAsync(User loginModel);

        // Nhận vào thông tin đăng ký, trả về câu thông báo thành công (string)
        Task<string> RegisterAsync(User registerModel);
    }
}
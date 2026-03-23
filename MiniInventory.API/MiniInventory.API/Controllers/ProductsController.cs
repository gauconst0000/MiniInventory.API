using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniInventory.API.Models;
using MiniInventory.API.Services;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // 🚀 Lệnh chuẩn Enterprise: Cấm mọi trình duyệt lưu Cache API này
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProductsController : ControllerBase
    {
        // CHỈ GỌI SERVICE, TUYỆT ĐỐI KHÔNG GỌI DBCONTEXT
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm!");
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                var newProduct = await _productService.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Gọi thẳng vào Service hoàn hảo của em
            await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Thao tác thành công, dữ liệu lịch sử được bảo toàn." });
        }
    }
}
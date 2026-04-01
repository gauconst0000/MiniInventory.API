using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniInventory.API.Models;
using MiniInventory.API.Services;
// using ClosedXML.Excel; // ĐÃ XÓA: Lễ tân không cần dùng mực in nữa!
using System.IO;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // ==============================================================
        // KHU VỰC 1: CÁC API CŨ ĐƯỢC BẢO VỆ NGUYÊN VẸN 100%
        // ==============================================================
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
            await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Thao tác thành công, dữ liệu lịch sử được bảo toàn." });
        }

        // ==============================================================
        // KHU VỰC 2: CÁC TÍNH NĂNG EXCEL ĐÃ ĐƯỢC CHUẨN HÓA VỀ SERVICE
        // ==============================================================

        [HttpGet("export")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            // Lễ tân chỉ việc nhờ Bếp trưởng làm file, nhận lại mảng byte và trả cho khách
            var content = await _productService.ExportAllProductsToExcelAsync();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(content, contentType, "DanhSachSanPham.xlsx");
        }

        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            // Tương tự, nhờ Bếp trưởng làm template
            var content = await _productService.GenerateExcelTemplateAsync();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(content, contentType, "Template_NhapSanPham.xlsx");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn một file Excel hợp lệ!");

            try
            {
                // Mở luồng dữ liệu (Stream) từ file tải lên
                using (var stream = file.OpenReadStream())
                {
                    // Ném luồng dữ liệu thô này xuống cho Bếp trưởng xử lý Upsert
                    var resultMessage = await _productService.ImportProductsFromExcelAsync(stream);
                    return Ok(resultMessage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống khi đọc file Excel: {ex.Message}");
            }
        }
    }
}
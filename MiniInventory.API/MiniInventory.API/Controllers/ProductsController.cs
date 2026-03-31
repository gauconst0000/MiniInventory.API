using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniInventory.API.Models;
using MiniInventory.API.Services;
using ClosedXML.Excel;

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
        [HttpGet("export")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            // 1. Lấy toàn bộ hàng hóa trong kho (Khầy đang giả sử em gọi qua Service hoặc Context tùy code cũ của em)
            // Nếu em đang dùng _context thì đổi thành _context.Products.ToList();
            // Nếu em đang dùng _productService thì đổi thành _productService.GetAllProducts();
            var products = await _productService.GetProductsAsync();

            // 2. Tạo một cuốn sổ Excel mới tinh
            using (var workbook = new XLWorkbook())
            {
                // 3. Tạo một trang giấy tên là "DanhSachSanPham"
                var worksheet = workbook.Worksheets.Add("DanhSachSanPham");

                // 4. In tiêu đề cho các cột (Dòng số 1)
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Mã Sản Phẩm";
                worksheet.Cell(1, 3).Value = "Tên Sản Phẩm";
                worksheet.Cell(1, 4).Value = "Giá Bán";
                worksheet.Cell(1, 5).Value = "Số Lượng Tồn";

                // 5. Vòng lặp: Đổ từng sản phẩm vào các dòng tiếp theo
                int currentRow = 2; // Bắt đầu in từ dòng số 2
                foreach (var p in products)
                {
                    worksheet.Cell(currentRow, 1).Value = p.Id;
                    worksheet.Cell(currentRow, 2).Value = p.ProductCode;
                    worksheet.Cell(currentRow, 3).Value = p.Name;
                    worksheet.Cell(currentRow, 4).Value = p.SellingPrice;
                    worksheet.Cell(currentRow, 5).Value = p.StockQuantity;
                    currentRow++;
                }

                // 6. Đóng gói cuốn sổ lại thành luồng dữ liệu (Stream)
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // 7. Trả về file Excel cho người dùng tải xuống
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachSanPham.xlsx");
                }
            }
        }
    }
}
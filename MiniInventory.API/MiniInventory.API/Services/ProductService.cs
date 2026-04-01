using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;
using ClosedXML.Excel;
using System.IO;

namespace MiniInventory.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // 2. Lấy chi tiết
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        // 3. Thêm mới
        public async Task<Product> AddProductAsync(Product product)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                throw new Exception("Loại sản phẩm (CategoryId) không tồn tại!");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // 4. Cập nhật
        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // 5. Xóa
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            // 1. Kiểm tra xem sản phẩm đã có lịch sử giao dịch chưa
            var hasTransactions = await _context.InventoryTransactionDetails
                                                .AnyAsync(t => t.ProductId == id);

            if (hasTransactions)
            {
                // 2. Nếu ĐÃ CÓ giao dịch -> Chuyển trạng thái
                product.Status = "INACTIVE";
                _context.Entry(product).State = EntityState.Modified;
            }
            else
            {
                // 3. Nếu CHƯA CÓ giao dịch -> Cho phép xóa vật lý
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<byte[]> GenerateExcelTemplateAsync()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Template_SanPham");

                worksheet.Cell(1, 1).Value = "Mã Sản Phẩm (*)";
                worksheet.Cell(1, 2).Value = "Tên Sản Phẩm (*)";
                worksheet.Cell(1, 3).Value = "Mã Danh Mục";
                worksheet.Cell(1, 4).Value = "Số Lượng";
                worksheet.Cell(1, 5).Value = "Giá";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray(); // Trả về mảng byte dữ liệu thô
                }
            }
        }

        public async Task<string> ImportProductsFromExcelAsync(Stream excelStream)
        {
            using (var workbook = new XLWorkbook(excelStream))
            {
                var worksheet = workbook.Worksheet(1);
                var usedRange = worksheet.RangeUsed();

                // Dựng khiên phòng thủ: Nếu file trống trơn không có dữ liệu, chặn ngay từ cửa!
                if (usedRange == null)
                {
                    return "File Excel trống không, vui lòng điền dữ liệu trước khi tải lên!";
                }

                var rows = usedRange.RowsUsed().Skip(1);

                // Lấy danh sách sản phẩm hiện tại bằng context trong Service
                var existingProducts = _context.Products.ToList();

                foreach (var row in rows)
                {
                    var productCode = row.Cell(1).Value.ToString().Trim();
                    var productName = row.Cell(2).Value.ToString().Trim();

                    if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(productName))
                        continue;

                    var existingProduct = existingProducts.FirstOrDefault(p => p.ProductCode == productCode);

                    if (existingProduct != null)
                    {
                        // Cập nhật
                        existingProduct.Name = productName;
                        if (int.TryParse(row.Cell(4).Value.ToString(), out int qty))
                            existingProduct.StockQuantity = qty;
                        if (decimal.TryParse(row.Cell(5).Value.ToString(), out decimal price))
                            existingProduct.SellingPrice = price;

                        _context.Products.Update(existingProduct);
                    }
                    else
                    {
                        // Thêm mới
                        var newProduct = new Product
                        {
                            ProductCode = productCode,
                            Name = productName
                        };
                        if (int.TryParse(row.Cell(4).Value.ToString(), out int qty))
                            newProduct.StockQuantity = qty;
                        if (decimal.TryParse(row.Cell(5).Value.ToString(), out decimal price))
                            newProduct.SellingPrice = price;

                        _context.Products.Add(newProduct);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return "Đã nhập dữ liệu từ Excel thành công rực rỡ!";
        }
        public async Task<byte[]> ExportAllProductsToExcelAsync()
        {
            // 1. Gọi hàm lấy danh sách sản phẩm (Hàm này đã có sẵn trong ProductService của em)
            var products = await GetProductsAsync();

            // 2. Tạo một cuốn sổ Excel mới tinh
            using (var workbook = new XLWorkbook())
            {
                // 3. Tạo một trang giấy
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

                // 6. Đóng gói cuốn sổ lại thành mảng byte để trả về cho Lễ tân
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
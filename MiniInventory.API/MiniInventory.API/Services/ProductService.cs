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

        // ==============================================================
        // 1. CÁC NGHIỆP VỤ LÕI (BẢO TOÀN NGUYÊN VẸN 100%)
        // ==============================================================
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

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

        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

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

        // ==============================================================
        // 2. NGHIỆP VỤ EXCEL (ĐÃ CẬP NHẬT UPSERT KHÓA NGOẠI)
        // ==============================================================
        public async Task<byte[]> GenerateExcelTemplateAsync()
        {
            // 1. Kéo toàn bộ Danh Mục từ Database lên (Đây chính là dữ liệu cho cái Dropdown)
            var categories = await _context.Categories.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                // =========================================================
                // BƯỚC 1: TẠO SHEET ẨN CHỨA DỮ LIỆU DANH MỤC
                // =========================================================
                var hiddenSheet = workbook.Worksheets.Add("Data_DanhMuc");

                // Đổ danh sách tên danh mục vào cột A của Sheet ẩn
                for (int i = 0; i < categories.Count; i++)
                {
                    hiddenSheet.Cell(i + 1, 1).Value = categories[i].Name;
                }

                // MA THUẬT: Giấu cái Sheet này đi để user không thấy
                hiddenSheet.Hide();

                // =========================================================
                // BƯỚC 2: TẠO SHEET CHÍNH (GIAO DIỆN NGƯỜI DÙNG)
                // =========================================================
                var mainSheet = workbook.Worksheets.Add("Template_SanPham");

                // Đổ mực in Header
                mainSheet.Cell(1, 1).Value = "Mã Sản Phẩm (*)";
                mainSheet.Cell(1, 2).Value = "Tên Sản Phẩm (*)";
                mainSheet.Cell(1, 3).Value = "Mã Danh Mục (*)"; // Cột C (Cột số 3)
                mainSheet.Cell(1, 4).Value = "Số Lượng";
                mainSheet.Cell(1, 5).Value = "Giá";

                var headerRow = mainSheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // =========================================================
                // BƯỚC 3: MÓC NỐI DROPDOWN VÀO CỘT MÃ DANH MỤC
                // =========================================================
                if (categories.Any())
                {
                    // Thiết lập Dropdown cho phép chọn từ dòng 2 đến dòng 1000 ở Cột C
                    var dropdownRange = mainSheet.Range("C2:C1000");

                    // KHẮC PHỤC LỖI PHIÊN BẢN: Gọi hàm CreateDataValidation() thay vì gọi thuộc tính
                    var validation = dropdownRange.CreateDataValidation();

                    // Lấy dữ liệu từ cột A của sheet ẩn để làm danh sách xổ xuống
                    var dataListRange = hiddenSheet.Range(1, 1, categories.Count, 1);
                    validation.List(dataListRange);

                    // Cảnh báo nếu cố tình gõ bậy bạ không có trong danh sách
                    validation.ErrorTitle = "Lỗi nhập liệu";
                    validation.ErrorMessage = "Vui lòng chỉ chọn danh mục có sẵn từ danh sách thả xuống!";
                }

                mainSheet.Columns().AdjustToContents();

                // Đóng gói trả về
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<string> ImportProductsFromExcelAsync(Stream excelStream)
        {
            using (var workbook = new XLWorkbook(excelStream))
            {
                var worksheet = workbook.Worksheet(1);
                var usedRange = worksheet.RangeUsed();

                if (usedRange == null)
                {
                    return "File Excel trống không, vui lòng điền dữ liệu trước khi tải lên!";
                }

                var rows = usedRange.RowsUsed().Skip(1);

                // Lấy trước danh sách SP và Danh mục lên bộ nhớ để dò tìm siêu tốc
                var existingProducts = _context.Products.ToList();
                var categories = _context.Categories.ToList(); // BỔ SUNG: Lấy kho Danh mục

                foreach (var row in rows)
                {
                    var productCode = row.Cell(1).Value.ToString().Trim();
                    var productName = row.Cell(2).Value.ToString().Trim();
                    var categoryName = row.Cell(3).Value.ToString().Trim(); // BỔ SUNG: Đọc cột 3

                    if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(productName))
                        continue;

                    // BỔ SUNG: Tìm ID của Danh Mục
                    // (Lưu ý: Nếu bảng Categories của em dùng cột khác để chứa chữ "Iphone", hãy đổi c.Name thành cột đó)
                    var category = categories.FirstOrDefault(c => c.Name != null && c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                    // Nếu người ta điền mã danh mục bậy bạ không có trong DB -> Bỏ qua để tránh văng lỗi 500
                    if (category == null)
                        continue;

                    var existingProduct = existingProducts.FirstOrDefault(p => p.ProductCode == productCode);

                    if (existingProduct != null)
                    {
                        // TH1: Cập nhật
                        existingProduct.Name = productName;
                        existingProduct.CategoryId = category.Id; // BỔ SUNG: Gán khóa ngoại mới

                        if (int.TryParse(row.Cell(4).Value.ToString(), out int qty))
                            existingProduct.StockQuantity = qty;
                        if (decimal.TryParse(row.Cell(5).Value.ToString(), out decimal price))
                            existingProduct.SellingPrice = price;

                        _context.Products.Update(existingProduct);
                    }
                    else
                    {
                        // TH2: Thêm mới
                        var newProduct = new Product
                        {
                            ProductCode = productCode,
                            Name = productName,
                            CategoryId = category.Id // BỔ SUNG: Gán khóa ngoại bắt buộc
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
            var products = await GetProductsAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachSanPham");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Mã Sản Phẩm";
                worksheet.Cell(1, 3).Value = "Tên Sản Phẩm";
                worksheet.Cell(1, 4).Value = "Giá Bán";
                worksheet.Cell(1, 5).Value = "Số Lượng Tồn";

                int currentRow = 2;
                foreach (var p in products)
                {
                    worksheet.Cell(currentRow, 1).Value = p.Id;
                    worksheet.Cell(currentRow, 2).Value = p.ProductCode;
                    worksheet.Cell(currentRow, 3).Value = p.Name;
                    worksheet.Cell(currentRow, 4).Value = p.SellingPrice;
                    worksheet.Cell(currentRow, 5).Value = p.StockQuantity;
                    currentRow++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
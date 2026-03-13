import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule], 
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  danhSachSanPham: any[] = [];
  danhSachLoaiSP: any[] = [];
  
  spMoi: any = { id: 0, productCode: '', name: '', purchasePrice: 0, sellingPrice: 0, unit: '', stockQuantity: 0, categoryId: 1 };

  hienThiForm: boolean = false;
  hangSapHet: any[] = []; // Chứa danh sách hàng sắp hết
  tongSoMatHang: number = 0; // Đếm xem có bao nhiêu loại sản phẩm
  tongSoTonKho: number = 0; // Cộng dồn tất cả số lượng hàng đang có
  lichSuGiaoDich: any[] = [];

  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router); // <-- Thêm "Người dẫn đường" vào đây 
  
  ngOnInit() {
    this.layDanhSachHangHoa();
    this.layDanhSachLoaiSP();
    this.layBaoCaoSapHet(); // THÊM DÒNG NÀY
    this.layLichSuGiaoDich();
  }

  layDanhSachHangHoa() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('https://localhost:7089/api/Products', { headers: headers })
      .subscribe({
        next: (data: any) => {
          this.danhSachSanPham = data.$values || data;
          // TÍNH TOÁN KPI NGAY KHI LẤY ĐƯỢC DỮ LIỆU
          this.tongSoMatHang = this.danhSachSanPham.length;
          // Dùng hàm reduce để cộng dồn toàn bộ cột stockQuantity
          this.tongSoTonKho = this.danhSachSanPham.reduce((tong, sp) => tong + sp.stockQuantity, 0);
          console.log('Kho hàng đã cập nhật: ', this.danhSachSanPham);
          
          // 2. THỔI CÒI! Ép Angular phải vẽ lại bảng giao diện ngay lập tức
          this.cdr.detectChanges(); 
        },
        error: (err) => console.error('Lỗi lấy hàng:', err)
      });
  }

  layDanhSachLoaiSP() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('https://localhost:7089/api/Categories', { headers: headers })
      .subscribe({
        next: (data: any) => {
          // Gắn dữ liệu lấy được vào biến danhSachLoaiSP
          this.danhSachLoaiSP = data.$values || data;
          console.log('Danh mục Loại SP đã tải: ', this.danhSachLoaiSP);
        },
        error: (err) => console.error('Lỗi lấy danh mục:', err)
      });
  }
  layBaoCaoSapHet() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('https://localhost:7089/api/Reports/low-stock', { headers: headers })
      .subscribe({
        next: (data: any) => {
          this.hangSapHet = data.$values || data;
          this.cdr.detectChanges(); // Thổi còi
        },
        error: (err) => console.error('Lỗi lấy báo cáo:', err)
      });
  }

  layLichSuGiaoDich() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('https://localhost:7089/api/InventoryTransactions', { headers: headers })
      .subscribe({
        next: (data: any) => {
          let tatCaGiaoDich = data.$values || data;
          
          // Tuyệt chiêu Javascript: Sắp xếp ngày mới nhất lên đầu, sau đó cắt lấy đúng 10 cái đầu tiên
          this.lichSuGiaoDich = tatCaGiaoDich
            .sort((a: any, b: any) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime())
            .slice(0, 10);
            
          this.cdr.detectChanges(); // Thổi còi
        },
        error: (err) => console.error('Lỗi lấy lịch sử giao dịch:', err)
      });
  }
  // 1. Hàm dọn dẹp form cho sạch sẽ khi bấm "+ Nhập thêm hàng mới"
  lamMoiForm() {
    this.spMoi = { id: 0, productCode: '', name: '', purchasePrice: 0, sellingPrice: 0, unit: '', stockQuantity: 0, categoryId: 0 };
    this.hienThiForm = true;
  }

  // 2. Hàm copy thông tin cũ đưa lên form khi bấm nút "Sửa"
  suaSanPham(spCu: any) {
    // Dấu 3 chấm (...) giúp copy dữ liệu an toàn, để em gõ thử vào form cũng không làm biến dạng cái bảng ở ngoài
    this.spMoi = { ...spCu }; 
    this.hienThiForm = true;
  }

  dongForm() {
    this.hienThiForm = false; // TẮT popup đi
  }
  // ... (Các hàm themSanPham và xoaSanPham bên dưới em cứ giữ nguyên nhé)
  // Đổi tên hàm một chút cho chuẩn nghĩa, hoặc giữ nguyên tên cũ cũng được. Ở đây thầy giữ nguyên để em đỡ phải sửa HTML.
  themSanPham() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    // KIỂM TRA NHANH: Nếu chưa chọn loại SP (id = 0) thì báo lỗi luôn không cho gửi
    if (this.spMoi.categoryId === 0) {
      alert('Vui lòng chọn Loại sản phẩm trước khi lưu!');
      return;
    }

    if (this.spMoi.id === 0) {
      // Logic THÊM MỚI
      this.http.post('https://localhost:7089/api/Products', this.spMoi, { headers: headers })
        .subscribe({
          next: () => { 
            alert('🎉 Thêm thành công!'); 
            this.dongForm();
            this.layDanhSachHangHoa(); 
            this.lamMoiForm();
          },
          error: (err) => {
            console.error('Chi tiết lỗi Add:', err); // Xem lỗi thật ở F12
            alert('Lỗi: Không thể thêm hàng!');
          }
        });
    } else {
      // Logic CẬP NHẬT
      this.http.put('https://localhost:7089/api/Products/' + this.spMoi.id, this.spMoi, { headers: headers })
        .subscribe({
          next: () => { 
            alert('🎉 Cập nhật thành công!'); 
            this.layDanhSachHangHoa();
            this.lamMoiForm();
          },
          error: (err) => {
            console.error('Chi tiết lỗi Update:', err);
            alert('Lỗi: Không thể cập nhật hàng!');
          }
        });
    }
  }

  // Hàm Xóa sản phẩm siêu ngầu
  xoaSanPham(id: number) {
    const xacNhan = confirm('⚠️ Em có chắc chắn muốn xóa sản phẩm này khỏi kho không?');
    if (xacNhan) {
      const token = localStorage.getItem('token');
      const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

      this.http.delete(`https://localhost:7089/api/Products/${id}`, { headers: headers })
        .subscribe({
          next: () => {
            alert('✅ Đã dọn dẹp sản phẩm thành công!');
            this.layDanhSachHangHoa(); // Tự động load lại bảng cho sạch sẽ
          },
          error: (err) => {
            console.error('Lỗi khi xóa:', err);
            alert('Lỗi: C# không cho phép xóa món hàng này!');
          }
        });
    }
  }
  dangXuat() {
    // 1. Xóa thẻ VIP
    localStorage.removeItem('token');

    // 2. Gọi lệnh chuyển trang và giăng bẫy bắt lỗi
    this.router.navigate(['/login']).then(success => {
      if (success) {
        console.log("🎉 Mở cửa thành công!");
      } else {
        // Nếu rơi vào đây: 99% là do AuthGuard chặn
        console.warn("⚠️ Lệnh chuyển trang bị từ chối (Khả năng cao do AuthGuard)."); 
      }
    }).catch(err => {
      // Nếu rơi vào đây: 99% là do sai đường dẫn trong app.routes.ts
      console.error("❌ Lỗi trật khớp đường dẫn. Chi tiết:", err); 
    });
  }
}
import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ProductForm } from '../shared/components/product-form/product-form';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule, ProductForm], 
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  danhSachSanPham: any[] = [];
  danhSachLoaiSP: any[] = [];
  
  spMoi: any = { id: 0, productCode: '', name: '', purchasePrice: 0, sellingPrice: 0, unit: '', stockQuantity: 0, categoryId: 1 };

  hienThiForm: boolean = false;
  hangSapHet: any[] = [];
  tongSoMatHang: number = 0;
  tongSoTonKho: number = 0;
  lichSuGiaoDich: any[] = [];

  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  
  ngOnInit() {
    this.layDanhSachHangHoa();
    this.layDanhSachLoaiSP();
    this.layBaoCaoSapHet();
    this.layLichSuGiaoDich();
  }

  layDanhSachHangHoa() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    // 🚀 BÙA CHỐNG CACHE: Thêm timestamp vào đuôi URL để ép Chrome tải dữ liệu mới 100%
    const urlChongCache = environment.apiUrl + '/Products?t=' + new Date().getTime();

    this.http.get(urlChongCache, { headers: headers })
      .subscribe({
        next: (data: any) => {
          let tatCaSanPham = data.$values || data;
          
          this.danhSachSanPham = tatCaSanPham.filter((sp: any) => {
            const statusLower = (sp.Status || sp.status || 'ACTIVE').toLowerCase();
            return statusLower !== 'inactive';
          });
          
          this.tongSoMatHang = this.danhSachSanPham.length;
          this.tongSoTonKho = this.danhSachSanPham.reduce((tong: any, sp: any) => tong + sp.stockQuantity, 0);
          
          this.cdr.detectChanges(); 
        },
        error: (err) => console.error('Lỗi lấy hàng:', err)
      });
  }

  layDanhSachLoaiSP() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get(environment.apiUrl + '/Categories', { headers: headers })
      .subscribe({
        next: (data: any) => {
          this.danhSachLoaiSP = data.$values || data;
        },
        error: (err) => console.error('Lỗi lấy danh mục:', err)
      });
  }

  layBaoCaoSapHet() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    // 🚀 BÙA CHỐNG CACHE: Ép tải lại báo cáo mới nhất
    const urlChongCache = environment.apiUrl + '/Reports/low-stock?t=' + new Date().getTime();

    this.http.get(urlChongCache, { headers: headers })
      .subscribe({
        next: (data: any) => {
          this.hangSapHet = data.$values || data;
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Lỗi lấy báo cáo:', err)
      });
  }

  layLichSuGiaoDich() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    // 🚀 BÙA CHỐNG CACHE: Ép tải lại lịch sử (Giải quyết lỗi SP Không xác định)
    const urlChongCache = environment.apiUrl + '/InventoryTransactions?t=' + new Date().getTime();

    this.http.get(urlChongCache, { headers: headers })
      .subscribe({
        next: (data: any) => {
          let tatCaGiaoDich = data.$values || data;
          this.lichSuGiaoDich = tatCaGiaoDich
            .sort((a: any, b: any) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime())
            .slice(0, 10);
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Lỗi lấy lịch sử giao dịch:', err)
      });
  }

  lamMoiForm() {
    this.spMoi = { id: 0, productCode: '', name: '', purchasePrice: 0, sellingPrice: 0, unit: '', stockQuantity: 0, categoryId: 0 };
    this.hienThiForm = true;
  }

  suaSanPham(spCu: any) {
    this.spMoi = { ...spCu }; 
    this.hienThiForm = true;
  }

  dongForm() {
    this.hienThiForm = false;
  }

  themSanPham() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    if (this.spMoi.categoryId === 0) {
      alert('Vui lòng chọn Loại sản phẩm trước khi lưu!');
      return;
    }

    if (this.spMoi.id === 0) {
      this.http.post(environment.apiUrl + '/Products', this.spMoi, { headers: headers })
        .subscribe({
          next: () => { 
            alert('🎉 Thêm thành công!'); 
            this.dongForm();
            this.layDanhSachHangHoa(); 
            this.lamMoiForm();
          },
          error: (err) => {
            console.error('Chi tiết lỗi Add:', err); 
            alert('Lỗi: Không thể thêm hàng!');
          }
        });
    } else {
      this.http.put(environment.apiUrl + '/Products/' + this.spMoi.id, this.spMoi, { headers: headers })
        .subscribe({
          next: () => { 
            alert('🎉 Cập nhật thành công!'); 
            this.layDanhSachHangHoa();
            this.lamMoiForm();
            this.dongForm();
          },
          error: (err) => {
            console.error('Chi tiết lỗi Update:', err);
            alert('Lỗi: Không thể cập nhật hàng!');
          }
        });
    }
  }

  xoaSanPham(id: any) { 
    const xacNhan = confirm('⚠️ Em có chắc chắn muốn xóa sản phẩm này khỏi kho không?');
    if (xacNhan) {
      const token = localStorage.getItem('token');
      const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

      this.http.delete(`${environment.apiUrl}/Products/${id}`, { headers: headers })
        .subscribe({
          next: () => {
            alert('✅ Đã dọn dẹp sản phẩm thành công!');
            
            // 1. Lọc sản phẩm ra khỏi bảng danh sách chính
            this.danhSachSanPham = this.danhSachSanPham.filter((sp: any) => (sp.Id || sp.id) != id); 
            
            // 🚀 2. MA THUẬT Ở ĐÂY: Xóa luôn nó khỏi bảng Cảnh báo bên trên
            this.hangSapHet = this.hangSapHet.filter((sp: any) => (sp.Id || sp.id) != id);

            // 3. Tính toán lại tổng số
            this.tongSoMatHang = this.danhSachSanPham.length;
            this.tongSoTonKho = this.danhSachSanPham.reduce((tong: any, sp: any) => tong + sp.stockQuantity, 0);

            // 4. Ép Angular vẽ lại màn hình ngay lập tức
            this.cdr.detectChanges(); 
          },
          error: (err) => {
            console.error('Lỗi khi xóa:', err);
            alert('Lỗi: C# không cho phép xóa món hàng này!');
          }
        });
    }
  }

  dangXuat() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']).then(success => {
      if (success) {
        console.log("🎉 Mở cửa thành công!");
      }
    }).catch(err => {
      console.error("❌ Lỗi trật khớp đường dẫn. Chi tiết:", err); 
    });
  }
}
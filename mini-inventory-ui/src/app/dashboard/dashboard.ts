import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ProductForm } from '../shared/components/product-form/product-form';



@Component({
  selector: 'app-dashboard',
  standalone: true,
  // Đã nạp ProductFormComponent vào mảng imports hợp lệ
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

    this.http.get('https://localhost:7089/api/Products', { headers: headers })
      .subscribe({
        next: (data: any) => {
          this.danhSachSanPham = data.$values || data;
          this.tongSoMatHang = this.danhSachSanPham.length;
          this.tongSoTonKho = this.danhSachSanPham.reduce((tong, sp) => tong + sp.stockQuantity, 0);
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
          this.danhSachLoaiSP = data.$values || data;
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
          this.cdr.detectChanges();
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
      this.http.post('https://localhost:7089/api/Products', this.spMoi, { headers: headers })
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
      this.http.put('https://localhost:7089/api/Products/' + this.spMoi.id, this.spMoi, { headers: headers })
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

  xoaSanPham(id: number) {
    const xacNhan = confirm('⚠️ Em có chắc chắn muốn xóa sản phẩm này khỏi kho không?');
    if (xacNhan) {
      const token = localStorage.getItem('token');
      const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

      this.http.delete(`https://localhost:7089/api/Products/${id}`, { headers: headers })
        .subscribe({
          next: () => {
            alert('✅ Đã dọn dẹp sản phẩm thành công!');
            this.layDanhSachHangHoa();
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
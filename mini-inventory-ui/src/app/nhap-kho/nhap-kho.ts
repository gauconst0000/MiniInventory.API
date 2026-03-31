import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-nhap-kho',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // Cần CommonModule để dùng *ngFor
  templateUrl: './nhap-kho.html',
  styleUrl: './nhap-kho.css'
})
export class NhapKhoComponent implements OnInit {
  // Các biến để hứng dữ liệu trên màn hình
  danhSachSanPham: any[] = [];
  sanPhamChonId: number = 0;
  soLuongNhap: number = 0;

  private http = inject(HttpClient);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.layDanhSachSanPham();
  }

  layDanhSachSanPham() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
  'Authorization': 'Bearer ' + token
});

    this.http.get(environment.apiUrl + '/Products', { headers }).subscribe({
      next: (data: any) => {
        // CHÚ Ý CHỖ NÀY: Phải có .$values để mở hộp C# nhé em
        this.danhSachSanPham = data.$values || data; 
        this.cdr.detectChanges();
        
        console.log("🎉 Đã đổ thành công danh sách vào Dropdown!", this.danhSachSanPham);
      },
      error: (err) => {
        console.error("❌ Lỗi lấy danh sách sản phẩm:", err);
      }
    });
  }

  luuPhieuNhap() {
    if (this.sanPhamChonId == 0) { alert("⚠️ Chưa chọn mặt hàng!"); return; }
    if (this.soLuongNhap <= 0 || !this.soLuongNhap) { alert("⚠️ Số lượng phải > 0!"); return; }

    const sanPhamCu = this.danhSachSanPham.find(sp => sp.id == this.sanPhamChonId);
    if (!sanPhamCu) return;

    // ĐÓNG GÓI CHUẨN XÁC 100%
    const phieuNhap = {
      transactionDate: new Date().toISOString(),
      transactionType: "Inbound", // CHỈ CẦN SỬA ĐÚNG CHỮ NÀY TỪ "Nhập" THÀNH "Inbound"
      contactPerson: "Thủ Kho Admin",
      notes: "Nhập kho chuẩn API", 
      transactionDetails: [
        {
          quantity: this.soLuongNhap,
          unitPrice: sanPhamCu.purchasePrice, 
          productId: Number(this.sanPhamChonId) 
        }
      ]
    };

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
  'Authorization': 'Bearer ' + token
});

    // GỌI ĐÚNG 1 LẦN DUY NHẤT LÀ XONG VIỆC
    this.http.post(environment.apiUrl + '/InventoryTransactions', phieuNhap, { headers })
      .subscribe({
        next: () => {
          alert(`🎉 Thành công! Đã nhập ${this.soLuongNhap} sản phẩm chuẩn nghiệp vụ!`);
          this.router.navigate(['/dashboard']); 
        },
        error: (err) => {
          console.error("Lỗi:", err);
          alert("❌ Lỗi lập phiếu!");
        }
      });
  }
}
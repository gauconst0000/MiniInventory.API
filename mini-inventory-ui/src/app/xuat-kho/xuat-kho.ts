import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-xuat-kho',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './xuat-kho.html'
})
export class XuatKhoComponent implements OnInit {
  danhSachSanPham: any[] = [];
  sanPhamChonId: number = 0;
  soLuongXuat: number = 0;

  private http = inject(HttpClient);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.layDanhSachSanPham();
  }

  // 1. Lấy danh sách sản phẩm để đưa vào Dropdown (Giống hệt Nhập kho)
  layDanhSachSanPham() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.get('https://localhost:7089/api/Products', { headers }).subscribe({
      next: (data: any) => {
        this.danhSachSanPham = data.$values || data;
        this.cdr.detectChanges(); // Thổi còi gọi Angular vẽ giao diện
      },
      error: (err) => console.error("Lỗi lấy danh sách sản phẩm:", err)
    });
  }

  // 2. Logic lưu phiếu xuất
  luuPhieuXuat() {
    if (this.sanPhamChonId == 0) { alert("⚠️ Em chưa chọn mặt hàng cần xuất!"); return; }
    if (this.soLuongXuat <= 0 || !this.soLuongXuat) { alert("⚠️ Số lượng xuất phải lớn hơn 0!"); return; }

    const sanPhamCu = this.danhSachSanPham.find(sp => sp.id == this.sanPhamChonId);
    if (!sanPhamCu) return;

    // ⛔ CHỐT CHẶN QUAN TRỌNG: Kiểm tra tồn kho
    if (this.soLuongXuat > sanPhamCu.stockQuantity) {
      alert(`❌ Không thể xuất! Trong kho hiện chỉ còn ${sanPhamCu.stockQuantity} sản phẩm. Em không thể xuất ${this.soLuongXuat} được!`);
      return;
    }

    // 3. Đóng gói kiện hàng gửi đi
    const phieuXuat = {
      transactionDate: new Date().toISOString(),
      transactionType: "Outbound", // TỪ KHÓA QUAN TRỌNG
      contactPerson: "Thủ Kho Admin",
      notes: "Xuất kho giao cho khách", 
      transactionDetails: [
        {
          quantity: this.soLuongXuat,
          unitPrice: sanPhamCu.sellingPrice, // Xuất kho lấy giá bán
          productId: Number(this.sanPhamChonId)
        }
      ]
    };

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.post('https://localhost:7089/api/InventoryTransactions', phieuXuat, { headers })
      .subscribe({
        next: () => {
          alert(`🎉 Thành công! Đã xuất ${this.soLuongXuat} sản phẩm khỏi kho!`);
          this.router.navigate(['/dashboard']); 
        },
        error: (err) => {
          console.error("Lỗi:", err);
          alert("❌ Lỗi lập phiếu xuất! Em kiểm tra F12 nhé.");
        }
      });
  }
}
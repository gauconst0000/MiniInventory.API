import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-danh-muc',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './danh-muc.html'
})
export class DanhMucComponent implements OnInit {
  danhSachDanhMuc: any[] = [];
  // Đối tượng dùng chung cho cả Thêm và Sửa
  dmMoi: any = { id: 0, name: '', description: '' };
  hienThiForm: boolean = false;

  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.layDanhSach();
  }

  // 1. Lấy danh sách Category từ Backend
  layDanhSach() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });
    this.http.get('https://localhost:7089/api/Categories', { headers }).subscribe({
      next: (data: any) => {
        this.danhSachDanhMuc = data.$values || data;
        this.cdr.detectChanges(); // Gọi Angular vẽ lại giao diện
      },
      error: (err) => console.error("Lỗi lấy danh mục:", err)
    });
  }

  // 2. Lưu (Thêm mới hoặc Cập nhật)
  luuDanhMuc() {
    if (!this.dmMoi.name) { alert("⚠️ Tên loại không được để trống!"); return; }

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    if (this.dmMoi.id === 0) {
      // THÊM MỚI
      this.http.post('https://localhost:7089/api/Categories', this.dmMoi, { headers }).subscribe({
        next: () => { 
          alert("✨ Thêm danh mục mới thành công!"); 
          this.dongForm(); 
          this.layDanhSach(); 
        },
        error: () => alert("❌ Lỗi khi thêm mới!")
      });
    } else {
      // CẬP NHẬT
      this.http.put(`https://localhost:7089/api/Categories/${this.dmMoi.id}`, this.dmMoi, { headers }).subscribe({
        next: () => { 
          alert("✏️ Cập nhật danh mục thành công!"); 
          this.dongForm(); 
          this.layDanhSach(); 
        },
        error: () => alert("❌ Lỗi khi cập nhật!")
      });
    }
  }

  // 3. Xóa danh mục
  xoaDanhMuc(id: number) {
    const xacNhan = confirm("⚠️ Nếu xóa danh mục này, các sản phẩm thuộc loại này có thể bị ảnh hưởng. Em chắc chắn chứ?");
    if (xacNhan) {
      const token = localStorage.getItem('token');
      const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });
      this.http.delete(`https://localhost:7089/api/Categories/${id}`, { headers }).subscribe({
        next: () => { 
          alert("✅ Đã xóa danh mục thành công!"); 
          this.layDanhSach(); 
        },
        error: () => alert("❌ Không thể xóa! Có thể đang có sản phẩm thuộc danh mục này.")
      });
    }
  }

  // Điều khiển Popup
  moForm(dm: any = null) {
    // Nếu có dm (sửa), copy dữ liệu vào dmMoi. Nếu không (thêm), reset về 0.
    this.dmMoi = dm ? { ...dm } : { id: 0, name: '', description: '' };
    this.hienThiForm = true;
  }

  dongForm() {
    this.hienThiForm = false;
  }
}
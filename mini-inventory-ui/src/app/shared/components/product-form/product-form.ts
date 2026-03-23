import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-form',
  imports: [CommonModule, FormsModule], // Cần 2 cái này để chạy *ngIf, *ngFor và [(ngModel)]
  templateUrl: './product-form.html',
  styleUrl: './product-form.css'
})
export class ProductForm {
  // --- ĐẦU VÀO (@Input): Nhận dữ liệu từ trang cha ---
  @Input() hienThiForm: boolean = false; 
  @Input() spMoi: any = { id: 0, productCode: '', name: '', purchasePrice: 0, sellingPrice: 0, unit: '', stockQuantity: 0, categoryId: 0 };
  @Input() danhSachLoaiSP: any[] = [];

  // --- ĐẦU RA (@Output): Báo cáo kết quả về cho trang cha ---
  @Output() dongFormEvent = new EventEmitter<void>(); 
  @Output() luuSanPhamEvent = new EventEmitter<any>(); 

  dongForm() {
    this.dongFormEvent.emit(); 
  }

  xacNhanLuu() {
    if (this.spMoi.categoryId === 0 || this.spMoi.categoryId === '0') {
      alert('⚠️ Vui lòng chọn Loại sản phẩm trước khi lưu!');
      return;
    }
    this.luuSanPhamEvent.emit(this.spMoi); 
  }
}
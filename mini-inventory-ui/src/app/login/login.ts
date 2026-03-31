import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule], // Cần cái này để lấy dữ liệu từ form HTML
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  // Biến lưu thông tin người dùng nhập
  tenDangNhap = '';
  matKhau = '';

  // Khai báo các công cụ cần thiết
  private http = inject(HttpClient);
  private router = inject(Router);

  // Hàm xử lý khi bấm nút Đăng nhập
  dangNhap() {
    const duLieu = { username: this.tenDangNhap, passwordHash: this.matKhau };
    
    // 1. In ra xem Angular có lấy được đúng chữ 'admin' và '123456' không
    console.log("Dữ liệu chuẩn bị gửi đi:", duLieu); 

    this.http.post(environment.apiUrl + '/Auth/login', duLieu).subscribe({
      next: (res: any) => {
        console.log("Đăng nhập thành công!", res);
        localStorage.setItem('token', res.token); 
        this.router.navigate(['/dashboard']); 
      },
      error: (err) => {
        // 2. In ra lỗi thực sự từ C# để thầy trò mình cùng xem
        console.error("Lý do C# từ chối:", err);
        alert('Đăng nhập thất bại! Em mở F12 xem chi tiết lỗi nhé.');
      }
    });
  }
}
import { Routes } from '@angular/router';
import { LoginComponent } from './login/login'; 
import { DashboardComponent } from './dashboard/dashboard';
import { NhapKhoComponent } from './nhap-kho/nhap-kho';
import { XuatKhoComponent } from './xuat-kho/xuat-kho'; // Đường dẫn có thể thay đổi tùy cách em đặt tên file
import { DanhMucComponent } from './danh-muc/danh-muc';

export const routes: Routes = [
  // 1. Nếu người dùng vào trang web mà không gõ gì, tự động đưa về trang Login
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  
  // 2. Định nghĩa đường dẫn cho trang Login
  { path: 'login', component: LoginComponent },
  
  // 3. Định nghĩa đường dẫn cho trang Dashboard
  { path: 'dashboard', component: DashboardComponent },
  { path: 'nhap-kho', component: NhapKhoComponent },
  { path: 'xuat-kho', component: XuatKhoComponent },
  { path: 'danh-muc', component: DanhMucComponent }
];
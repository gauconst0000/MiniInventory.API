import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule], // Chỉ cần một chữ imports này thôi, chứa RouterModule để khung tranh hoạt động
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {
  // File này giờ chỉ làm nhiệm vụ chứa <router-outlet> nên không cần code logic gì thêm ở đây cả.
  title = 'mini-inventory-ui';
}
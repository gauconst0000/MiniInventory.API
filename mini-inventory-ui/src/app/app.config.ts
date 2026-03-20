import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
// <-- Thêm dòng nhập thư viện này ở trên cùng:
import { provideHttpClient } from '@angular/common/http'; 

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient() // <-- Thêm cái "điện thoại" vào danh sách công cụ
  ]
};
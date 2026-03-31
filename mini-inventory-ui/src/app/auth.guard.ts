import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('token'); // Lục túi quần tìm vé

  if (token) {
    // Nếu có vé (token) -> Cho phép đi tiếp vào trong
    return true; 
  } else {
    // Nếu không có vé -> Lập tức đá văng ra trang Login
    router.navigate(['/login']); 
    return false;
  }
};
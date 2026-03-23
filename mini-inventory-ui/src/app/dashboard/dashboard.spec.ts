import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard'; // 🚀 Đã sửa tên đúng

describe('DashboardComponent', () => {
  let component: DashboardComponent; // 🚀 Đã sửa tên đúng
  let fixture: ComponentFixture<DashboardComponent>; // 🚀 Đã sửa tên đúng

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent], // 🚀 Đã sửa tên đúng
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent); // 🚀 Đã sửa tên đúng
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
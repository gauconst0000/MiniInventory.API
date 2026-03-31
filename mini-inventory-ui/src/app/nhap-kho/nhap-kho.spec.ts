import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NhapKhoComponent } from './nhap-kho';

describe('NhapKho', () => {
  let component: NhapKhoComponent;
  let fixture: ComponentFixture<NhapKhoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NhapKhoComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(NhapKhoComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

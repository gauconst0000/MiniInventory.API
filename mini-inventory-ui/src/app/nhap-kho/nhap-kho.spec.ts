import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NhapKho } from './nhap-kho';

describe('NhapKho', () => {
  let component: NhapKho;
  let fixture: ComponentFixture<NhapKho>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NhapKho],
    }).compileComponents();

    fixture = TestBed.createComponent(NhapKho);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorMDashboardComponent } from './vendor-m-dashboard.component';

describe('VendorMDashboardComponent', () => {
  let component: VendorMDashboardComponent;
  let fixture: ComponentFixture<VendorMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VendorMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VendorMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

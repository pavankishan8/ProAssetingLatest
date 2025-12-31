import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseMDashboardComponent } from './purchase-m-dashboard.component';

describe('PurchaseMDashboardComponent', () => {
  let component: PurchaseMDashboardComponent;
  let fixture: ComponentFixture<PurchaseMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PurchaseMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurchaseMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

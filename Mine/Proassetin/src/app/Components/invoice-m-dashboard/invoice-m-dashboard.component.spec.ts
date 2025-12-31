import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceMDashboardComponent } from './invoice-m-dashboard.component';

describe('InvoiceMDashboardComponent', () => {
  let component: InvoiceMDashboardComponent;
  let fixture: ComponentFixture<InvoiceMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InvoiceMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoiceMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

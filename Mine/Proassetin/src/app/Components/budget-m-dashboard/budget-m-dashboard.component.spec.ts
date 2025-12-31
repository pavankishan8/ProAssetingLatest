import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetMDashboardComponent } from './budget-m-dashboard.component';

describe('BudgetMDashboardComponent', () => {
  let component: BudgetMDashboardComponent;
  let fixture: ComponentFixture<BudgetMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BudgetMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BudgetMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

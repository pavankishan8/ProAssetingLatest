import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecurityMDashboardComponent } from './security-m-dashboard.component';

describe('SecurityMDashboardComponent', () => {
  let component: SecurityMDashboardComponent;
  let fixture: ComponentFixture<SecurityMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SecurityMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SecurityMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

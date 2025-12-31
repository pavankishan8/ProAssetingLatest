import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VaptMDashboardComponent } from './vapt-m-dashboard.component';

describe('VaptMDashboardComponent', () => {
  let component: VaptMDashboardComponent;
  let fixture: ComponentFixture<VaptMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VaptMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VaptMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

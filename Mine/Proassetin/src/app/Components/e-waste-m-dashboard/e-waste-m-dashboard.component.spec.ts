import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EWasteMDashboardComponent } from './e-waste-m-dashboard.component';

describe('EWasteMDashboardComponent', () => {
  let component: EWasteMDashboardComponent;
  let fixture: ComponentFixture<EWasteMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EWasteMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EWasteMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

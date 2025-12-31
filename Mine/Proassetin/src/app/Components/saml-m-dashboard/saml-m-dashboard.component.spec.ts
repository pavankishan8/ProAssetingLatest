import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SamlMDashboardComponent } from './saml-m-dashboard.component';

describe('SamlMDashboardComponent', () => {
  let component: SamlMDashboardComponent;
  let fixture: ComponentFixture<SamlMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SamlMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SamlMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

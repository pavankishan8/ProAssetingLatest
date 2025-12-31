import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetMDashboardComponent } from './asset-m-dashboard.component';

describe('AssetMDashboardComponent', () => {
  let component: AssetMDashboardComponent;
  let fixture: ComponentFixture<AssetMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssetMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssetMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

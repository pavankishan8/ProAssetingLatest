import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetsReportsComponent } from './assets-reports.component';

describe('AssetsReportsComponent', () => {
  let component: AssetsReportsComponent;
  let fixture: ComponentFixture<AssetsReportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssetsReportsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssetsReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

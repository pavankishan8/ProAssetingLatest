import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyTimeZoneComponent } from './currency-time-zone.component';

describe('CurrencyTimeZoneComponent', () => {
  let component: CurrencyTimeZoneComponent;
  let fixture: ComponentFixture<CurrencyTimeZoneComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CurrencyTimeZoneComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrencyTimeZoneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

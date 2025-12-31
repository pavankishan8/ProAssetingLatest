import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScanInventoryComponent } from './scan-inventory.component';

describe('ScanInventoryComponent', () => {
  let component: ScanInventoryComponent;
  let fixture: ComponentFixture<ScanInventoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScanInventoryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScanInventoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

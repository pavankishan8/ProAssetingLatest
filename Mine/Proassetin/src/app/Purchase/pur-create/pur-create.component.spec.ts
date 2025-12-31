import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurCreateComponent } from './pur-create.component';

describe('PurCreateComponent', () => {
  let component: PurCreateComponent;
  let fixture: ComponentFixture<PurCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PurCreateComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

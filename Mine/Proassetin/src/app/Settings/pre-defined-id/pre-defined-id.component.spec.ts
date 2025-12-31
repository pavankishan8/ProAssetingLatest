import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreDefinedIDComponent } from './pre-defined-id.component';

describe('PreDefinedIDComponent', () => {
  let component: PreDefinedIDComponent;
  let fixture: ComponentFixture<PreDefinedIDComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PreDefinedIDComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PreDefinedIDComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

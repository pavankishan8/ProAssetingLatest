import { ComponentFixture, TestBed } from '@angular/core/testing';

import { POComponent } from './po.component';

describe('POComponent', () => {
  let component: POComponent;
  let fixture: ComponentFixture<POComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ POComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(POComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

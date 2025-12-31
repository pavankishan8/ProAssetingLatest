import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SMTPConfigComponent } from './smtp-config.component';

describe('SMTPConfigComponent', () => {
  let component: SMTPConfigComponent;
  let fixture: ComponentFixture<SMTPConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SMTPConfigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SMTPConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

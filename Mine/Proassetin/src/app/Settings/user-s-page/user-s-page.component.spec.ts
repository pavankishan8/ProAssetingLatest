import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSPageComponent } from './user-s-page.component';

describe('UserSPageComponent', () => {
  let component: UserSPageComponent;
  let fixture: ComponentFixture<UserSPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserSPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserSPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

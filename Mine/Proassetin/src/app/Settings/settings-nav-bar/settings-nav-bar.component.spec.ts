import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsNavBarComponent } from './settings-nav-bar.component';

describe('SettingsNavBarComponent', () => {
  let component: SettingsNavBarComponent;
  let fixture: ComponentFixture<SettingsNavBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SettingsNavBarComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SettingsNavBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

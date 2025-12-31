import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketToolDashboardComponent } from './ticket-tool-dashboard.component';

describe('TicketToolDashboardComponent', () => {
  let component: TicketToolDashboardComponent;
  let fixture: ComponentFixture<TicketToolDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketToolDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TicketToolDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

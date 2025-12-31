import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectMDashboardComponent } from './project-m-dashboard.component';

describe('ProjectMDashboardComponent', () => {
  let component: ProjectMDashboardComponent;
  let fixture: ComponentFixture<ProjectMDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectMDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectMDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ProjectListComponent } from './components/project-list/project-list.component';
import { AddProjectComponent } from './components/add-project/add-project.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: ProjectListComponent },
  { path: 'add', component: AddProjectComponent },
  { path: 'edit/:id', component: AddProjectComponent }
];

@NgModule({
  declarations: [ProjectListComponent, AddProjectComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class ProjectsModule {}

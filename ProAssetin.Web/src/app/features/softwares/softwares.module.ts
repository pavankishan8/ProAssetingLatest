import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { SoftwareListComponent } from './components/software-list/software-list.component';
import { AddSoftwareComponent } from './components/add-software/add-software.component';

const routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full' as const
  },
  {
    path: 'list',
    component: SoftwareListComponent
  },
  {
    path: 'add',
    component: AddSoftwareComponent
  },
  {
    path: 'edit/:id',
    component: AddSoftwareComponent
  }
];

@NgModule({
  declarations: [
    SoftwareListComponent,
    AddSoftwareComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class SoftwaresModule { }


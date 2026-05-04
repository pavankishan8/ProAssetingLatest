import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { SecurityListComponent } from './components/security-list/security-list.component';
import { AddSecurityComponent } from './components/add-security/add-security.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: SecurityListComponent },
  { path: 'add', component: AddSecurityComponent },
  { path: 'edit/:id', component: AddSecurityComponent }
];

@NgModule({
  declarations: [SecurityListComponent, AddSecurityComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class SecurityModule {}

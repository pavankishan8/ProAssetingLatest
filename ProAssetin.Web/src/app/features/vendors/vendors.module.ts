import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { VendorListComponent } from './components/vendor-list/vendor-list.component';
import { AddVendorComponent } from './components/add-vendor/add-vendor.component';

const routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full' as const
  },
  {
    path: 'list',
    component: VendorListComponent
  },
  {
    path: 'add',
    component: AddVendorComponent
  },
  {
    path: 'edit/:id',
    component: AddVendorComponent
  }
];

@NgModule({
  declarations: [
    VendorListComponent,
    AddVendorComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class VendorsModule { }


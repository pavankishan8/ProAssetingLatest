import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { InvoiceListComponent } from './components/invoice-list/invoice-list.component';
import { AddInvoiceComponent } from './components/add-invoice/add-invoice.component';

const routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full' as const
  },
  {
    path: 'list',
    component: InvoiceListComponent
  },
  {
    path: 'add',
    component: AddInvoiceComponent
  },
  {
    path: 'edit/:id',
    component: AddInvoiceComponent
  }
];

@NgModule({
  declarations: [
    InvoiceListComponent,
    AddInvoiceComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class InvoicesModule { }


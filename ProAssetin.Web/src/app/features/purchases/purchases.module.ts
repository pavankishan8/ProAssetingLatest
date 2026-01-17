import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { POListComponent } from './components/po-list/po-list.component';
import { AddPOComponent } from './components/add-po/add-po.component';

const routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full' as const
  },
  {
    path: 'list',
    component: POListComponent
  },
  {
    path: 'add',
    component: AddPOComponent
  },
  {
    path: 'edit/:id',
    component: AddPOComponent
  }
];

@NgModule({
  declarations: [
    POListComponent,
    AddPOComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class PurchasesModule { }


import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ContractListComponent } from './components/contract-list/contract-list.component';
import { AddContractComponent } from './components/add-contract/add-contract.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: ContractListComponent },
  { path: 'add', component: AddContractComponent },
  { path: 'edit/:id', component: AddContractComponent }
];

@NgModule({
  declarations: [ContractListComponent, AddContractComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class ContractsModule {}

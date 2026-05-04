import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { BudgetListComponent } from './components/budget-list/budget-list.component';
import { AddBudgetComponent } from './components/add-budget/add-budget.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: BudgetListComponent },
  { path: 'add', component: AddBudgetComponent },
  { path: 'edit/:id', component: AddBudgetComponent }
];

@NgModule({
  declarations: [
    BudgetListComponent,
    AddBudgetComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class BudgetsModule { }

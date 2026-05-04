import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { TicketListComponent } from './components/ticket-list/ticket-list.component';
import { AddTicketComponent } from './components/add-ticket/add-ticket.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: TicketListComponent },
  { path: 'add', component: AddTicketComponent },
  { path: 'edit/:id', component: AddTicketComponent }
];

@NgModule({
  declarations: [TicketListComponent, AddTicketComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class TicketsModule {}

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TicketService, Ticket, TicketQuery } from '../../../../core/services/ticket.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-ticket-list',
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss']
})
export class TicketListComponent implements OnInit {
  rows: Ticket[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  searchTerm = '';
  stateFilter: string | undefined = undefined;
  priorityFilter: string | undefined = undefined;
  states: string[] = [];
  priorities: string[] = [];

  constructor(
    private ticketService: TicketService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.ticketService.getTaskStates().subscribe({ next: (s) => (this.states = s), error: () => {} });
    this.ticketService.getPriorities().subscribe({ next: (p) => (this.priorities = p), error: () => {} });
    this.load();
  }

  load(): void {
    this.loading = true;
    const q: TicketQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      taskState: this.stateFilter,
      priority: this.priorityFilter,
      sortBy: 'createdAt',
      sortDescending: true
    };
    this.ticketService.getTickets(q).subscribe({
      next: (res) => {
        this.rows = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load tickets', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
      }
    });
  }

  apply(): void {
    this.pageNumber = 1;
    this.load();
  }

  clear(): void {
    this.searchTerm = '';
    this.stateFilter = undefined;
    this.priorityFilter = undefined;
    this.pageNumber = 1;
    this.load();
  }

  page(p: number): void {
    this.pageNumber = p;
    this.load();
  }

  edit(row: Ticket): void {
    this.router.navigate(['/tickets/edit', row.id]);
  }

  remove(row: Ticket): void {
    if (!confirm(`Delete ticket #${row.id} "${row.taskTitle}"?`)) return;
    this.ticketService.deleteTicket(row.id).subscribe({
      next: () => {
        this.snackBar.open('Ticket deleted', 'Close', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
        this.load();
      },
      error: () => this.snackBar.open('Delete failed', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' })
    });
  }

  d(v: string | null | undefined): string {
    if (!v) return '—';
    return new Date(v).toLocaleString();
  }

  /** Second token for .st.* chip styles */
  stateSlug(s: string): string {
    return (s || 'Open').toLowerCase().replace(/\s+/g, '-');
  }
}

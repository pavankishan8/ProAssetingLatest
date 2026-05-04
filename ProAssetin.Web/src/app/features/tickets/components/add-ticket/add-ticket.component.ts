import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TicketService, CreateTicket, UpdateTicket } from '../../../../core/services/ticket.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-ticket',
  templateUrl: './add-ticket.component.html',
  styleUrls: ['./add-ticket.component.scss']
})
export class AddTicketComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEdit = false;
  id: number | null = null;
  states: string[] = [];
  priorities: string[] = [];

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.build();
    this.ticketService.getTaskStates().subscribe({ next: (s) => (this.states = s), error: () => {} });
    this.ticketService.getPriorities().subscribe({ next: (p) => (this.priorities = p), error: () => {} });

    const rid = this.route.snapshot.paramMap.get('id');
    if (rid) {
      this.isEdit = true;
      this.id = +rid;
      this.load(this.id);
    }
  }

  build(): void {
    this.form = this.fb.group({
      taskTitle: ['', [Validators.required, Validators.maxLength(500)]],
      taskAssignedToName: ['', Validators.maxLength(200)],
      taskState: ['Open', Validators.required],
      priority: ['Medium', Validators.required],
      description: ['', Validators.maxLength(2000)],
      resolution: ['', Validators.maxLength(2000)]
    });
  }

  load(id: number): void {
    this.loading = true;
    this.ticketService.getTicket(id).subscribe({
      next: (row) => {
        this.form.patchValue({
          taskTitle: row.taskTitle,
          taskAssignedToName: row.taskAssignedToName || '',
          taskState: row.taskState,
          priority: row.priority || 'Medium',
          description: row.description || '',
          resolution: row.resolution || ''
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load ticket', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/tickets/list']);
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const v = this.form.value;

    if (this.isEdit && this.id) {
      const body: UpdateTicket = {
        taskTitle: v.taskTitle,
        taskAssignedToName: v.taskAssignedToName || undefined,
        taskState: v.taskState,
        priority: v.priority,
        description: v.description || undefined,
        resolution: v.resolution || undefined
      };
      this.ticketService.updateTicket(this.id, body).subscribe({
        next: () => {
          this.snackBar.open('Ticket updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/tickets/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    } else {
      const body: CreateTicket = {
        taskTitle: v.taskTitle,
        taskAssignedToName: v.taskAssignedToName || undefined,
        taskState: v.taskState,
        priority: v.priority,
        description: v.description || undefined
      };
      this.ticketService.createTicket(body).subscribe({
        next: () => {
          this.snackBar.open('Ticket created', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/tickets/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    }
  }

  private err(e: HttpErrorResponse): void {
    this.snackBar.open(e.error?.message || e.message || 'Request failed', 'Close', {
      duration: 5000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
    this.loading = false;
  }

  cancel(): void {
    this.router.navigate(['/tickets/list']);
  }
}

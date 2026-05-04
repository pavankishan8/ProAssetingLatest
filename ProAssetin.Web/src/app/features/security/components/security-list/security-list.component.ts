import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SecurityIncidentService, SecurityIncident, SecurityIncidentQuery } from '../../../../core/services/security-incident.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-security-list',
  templateUrl: './security-list.component.html',
  styleUrls: ['./security-list.component.scss']
})
export class SecurityListComponent implements OnInit {
  rows: SecurityIncident[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  searchTerm = '';
  statusFilter: string | undefined = undefined;
  severityFilter: string | undefined = undefined;
  statuses: string[] = [];
  severities: string[] = [];

  constructor(
    private securityService: SecurityIncidentService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.securityService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });
    this.securityService.getSeverities().subscribe({ next: (s) => { this.severities = s; }, error: () => {} });
    this.load();
  }

  load(): void {
    this.loading = true;
    const q: SecurityIncidentQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      severity: this.severityFilter,
      sortBy: 'reportedDate',
      sortDescending: true
    };
    this.securityService.getIncidents(q).subscribe({
      next: (res) => {
        this.rows = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load security incidents', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
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
    this.statusFilter = undefined;
    this.severityFilter = undefined;
    this.pageNumber = 1;
    this.load();
  }

  page(p: number): void {
    this.pageNumber = p;
    this.load();
  }

  edit(row: SecurityIncident): void {
    this.router.navigate(['/security/edit', row.id]);
  }

  remove(row: SecurityIncident): void {
    if (!confirm(`Delete incident "${row.incidentReference}"?`)) return;
    this.securityService.deleteIncident(row.id).subscribe({
      next: () => {
        this.snackBar.open('Deleted', 'Close', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
        this.load();
      },
      error: () => this.snackBar.open('Delete failed', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' })
    });
  }

  dateOnly(v: string | null | undefined): string {
    if (!v) return '—';
    return v.split('T')[0];
  }

  sevClass(s: string): string {
    const x = (s || '').toLowerCase();
    if (x === 'critical') return 'crit';
    if (x === 'high') return 'high';
    if (x === 'medium') return 'med';
    return 'low';
  }

  statClass(s: string): string {
    const x = (s || '').toLowerCase();
    if (x === 'closed') return 'closed';
    if (x === 'mitigated') return 'mitigated';
    if (x === 'investigating') return 'inv';
    return 'open';
  }
}

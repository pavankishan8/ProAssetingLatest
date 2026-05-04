import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EWasteService, EWasteDisposal, EWasteQuery } from '../../../../core/services/ewaste.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-ewaste-list',
  templateUrl: './ewaste-list.component.html',
  styleUrls: ['./ewaste-list.component.scss']
})
export class EWasteListComponent implements OnInit {
  rows: EWasteDisposal[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  searchTerm = '';
  statusFilter: string | undefined = undefined;
  statuses: string[] = ['Scheduled', 'Collected', 'Disposed', 'Cancelled'];

  constructor(
    private ewasteService: EWasteService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.load();
    this.ewasteService.getStatuses().subscribe({
      next: (s) => { this.statuses = s; },
      error: () => {}
    });
  }

  load(): void {
    this.loading = true;
    const q: EWasteQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      sortBy: 'createdAt',
      sortDescending: true
    };
    this.ewasteService.getDisposals(q).subscribe({
      next: (res) => {
        this.rows = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load e-waste records', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(p: number): void {
    this.pageNumber = p;
    this.load();
  }

  edit(row: EWasteDisposal): void {
    this.router.navigate(['/ewaste/edit', row.id]);
  }

  remove(row: EWasteDisposal): void {
    if (!confirm(`Delete disposal "${row.disposalReference}"?`)) return;
    this.ewasteService.deleteDisposal(row.id).subscribe({
      next: () => {
        this.snackBar.open('Record deleted', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.load();
      },
      error: () => {
        this.snackBar.open('Delete failed', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
      }
    });
  }

  formatDate(v: string | null | undefined): string {
    if (!v) return '—';
    return v.split('T')[0];
  }

  statusClass(s: string): string {
    const x = (s || '').toLowerCase();
    if (x === 'disposed') return 'disposed';
    if (x === 'cancelled') return 'cancelled';
    if (x === 'collected') return 'collected';
    return 'scheduled';
  }
}

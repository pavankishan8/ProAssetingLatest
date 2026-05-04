import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ContractService, Contract, ContractQuery } from '../../../../core/services/contract.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-contract-list',
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.scss']
})
export class ContractListComponent implements OnInit {
  rows: Contract[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  searchTerm = '';
  statusFilter: string | undefined = undefined;
  typeFilter = '';
  statuses: string[] = [];

  constructor(
    private contractService: ContractService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.contractService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });
    this.load();
  }

  load(): void {
    this.loading = true;
    const q: ContractQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      contractType: this.typeFilter.trim() || undefined,
      sortBy: 'endDate',
      sortDescending: true
    };
    this.contractService.getContracts(q).subscribe({
      next: (res) => {
        this.rows = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load contracts', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
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
    this.typeFilter = '';
    this.pageNumber = 1;
    this.load();
  }

  page(p: number): void {
    this.pageNumber = p;
    this.load();
  }

  edit(row: Contract): void {
    this.router.navigate(['/contracts/edit', row.id]);
  }

  remove(row: Contract): void {
    if (!confirm(`Delete contract "${row.contractReference}"?`)) return;
    this.contractService.deleteContract(row.id).subscribe({
      next: () => {
        this.snackBar.open('Contract deleted', 'Close', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
        this.load();
      },
      error: () => this.snackBar.open('Delete failed', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' })
    });
  }

  money(v: number | null | undefined): string {
    if (v == null) return '—';
    return this.currencyService.formatCurrency(v);
  }

  d(v: string | null | undefined): string {
    if (!v) return '—';
    return v.split('T')[0];
  }

  stClass(s: string): string {
    const x = (s || '').toLowerCase().replace(/\s+/g, '');
    if (x === 'renewalpending') return 'ren';
    if (x === 'expired') return 'exp';
    if (x === 'terminated') return 'term';
    if (x === 'active') return 'act';
    return 'draft';
  }
}

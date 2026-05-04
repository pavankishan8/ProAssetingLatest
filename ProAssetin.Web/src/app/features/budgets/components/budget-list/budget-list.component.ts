import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BudgetService, Budget, BudgetQuery } from '../../../../core/services/budget.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-budget-list',
  templateUrl: './budget-list.component.html',
  styleUrls: ['./budget-list.component.scss']
})
export class BudgetListComponent implements OnInit {
  budgets: Budget[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;

  searchTerm = '';
  statusFilter: string | undefined = undefined;
  fiscalYearFilter: number | undefined = undefined;

  statuses: string[] = ['Active', 'Closed'];

  constructor(
    private budgetService: BudgetService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadBudgets();
    this.budgetService.getStatuses().subscribe({
      next: (s) => { this.statuses = s; },
      error: () => { }
    });
  }

  loadBudgets(): void {
    this.loading = true;
    const query: BudgetQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      fiscalYear: this.fiscalYearFilter,
      sortBy: 'fiscalYear',
      sortDescending: true
    };

    this.budgetService.getBudgets(query).subscribe({
      next: (response) => {
        this.budgets = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load budgets', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadBudgets();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.fiscalYearFilter = undefined;
    this.pageNumber = 1;
    this.loadBudgets();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadBudgets();
  }

  editBudget(b: Budget): void {
    this.router.navigate(['/budgets/edit', b.id]);
  }

  deleteBudget(b: Budget): void {
    if (confirm(`Delete budget "${b.name}"?`)) {
      this.budgetService.deleteBudget(b.id).subscribe({
        next: () => {
          this.snackBar.open('Budget deleted', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.loadBudgets();
        },
        error: () => {
          this.snackBar.open('Failed to delete budget', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        }
      });
    }
  }

  formatCurrency(amount: number): string {
    return this.currencyService.formatCurrency(amount);
  }

  getStatusClass(status: string): string {
    return status?.toLowerCase() === 'closed' ? 'closed' : 'active';
  }
}

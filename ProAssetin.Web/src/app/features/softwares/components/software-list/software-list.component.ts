import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SoftwareService, Software, SoftwareQuery } from '../../../../core/services/software.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-software-list',
  templateUrl: './software-list.component.html',
  styleUrls: ['./software-list.component.scss']
})
export class SoftwareListComponent implements OnInit {
  softwareList: Software[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;

  searchTerm = '';
  statusFilter: string | undefined = undefined;
  categoryFilter: string | undefined = undefined;
  licenseTypeFilter: string | undefined = undefined;

  statuses: string[] = ['Active', 'Expired', 'Inactive', 'Renewal Pending', 'Trial', 'Suspended'];
  categories: string[] = [];
  licenseTypes: string[] = ['Perpetual', 'Subscription', 'OEM', 'Open Source', 'Freeware', 'Trial', 'Volume License', 'Academic'];

  constructor(
    private softwareService: SoftwareService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadSoftware();
    this.loadCategories();
  }

  loadSoftware(): void {
    this.loading = true;
    
    const query: SoftwareQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      category: this.categoryFilter,
      licenseType: this.licenseTypeFilter,
      sortBy: 'softwareName',
      sortDescending: false
    };

    this.softwareService.getSoftware(query).subscribe({
      next: (response) => {
        this.softwareList = response.software;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading software:', error);
        this.snackBar.open('Failed to load software', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.softwareService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadSoftware();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.categoryFilter = undefined;
    this.licenseTypeFilter = undefined;
    this.pageNumber = 1;
    this.loadSoftware();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadSoftware();
  }

  editSoftware(software: Software): void {
    this.router.navigate(['/softwares/edit', software.id]);
  }

  deleteSoftware(software: Software): void {
    if (confirm(`Are you sure you want to delete "${software.softwareName}"?`)) {
      this.softwareService.deleteSoftware(software.id).subscribe({
        next: () => {
          this.snackBar.open('Software deleted successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadSoftware();
        },
        error: (error) => {
          console.error('Error deleting software:', error);
          this.snackBar.open('Failed to delete software', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
        }
      });
    }
  }

  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Active': 'active',
      'Expired': 'expired',
      'Inactive': 'inactive',
      'Renewal Pending': 'renewal',
      'Trial': 'trial',
      'Suspended': 'suspended'
    };
    return statusMap[status] || 'active';
  }

  isExpiringSoon(expiryDate?: string): boolean {
    if (!expiryDate) return false;
    const expiry = new Date(expiryDate);
    const today = new Date();
    const daysUntilExpiry = Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
    return daysUntilExpiry <= 30 && daysUntilExpiry > 0;
  }

  formatDate(dateString?: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  formatCurrency(amount?: number): string {
    return this.currencyService.formatCurrency(amount);
  }
}


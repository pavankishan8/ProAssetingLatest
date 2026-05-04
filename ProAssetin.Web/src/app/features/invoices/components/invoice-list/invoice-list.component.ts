import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InvoiceService, Invoice, InvoiceQuery } from '../../../../core/services/invoice.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-invoice-list',
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss']
})
export class InvoiceListComponent implements OnInit {
  invoices: Invoice[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;

  searchTerm = '';
  statusFilter: string | undefined = undefined;
  vendorNameFilter: string | undefined = undefined;

  statuses: string[] = ['Pending', 'Paid', 'Overdue', 'Cancelled'];

  constructor(
    private invoiceService: InvoiceService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadInvoices();
    this.loadStatuses();
  }

  loadStatuses(): void {
    this.invoiceService.getStatuses().subscribe({
      next: (statuses) => {
        this.statuses = statuses;
      },
      error: (error) => {
        console.error('Error loading statuses:', error);
      }
    });
  }

  loadInvoices(): void {
    this.loading = true;
    
    const query: InvoiceQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      vendorName: this.vendorNameFilter,
      sortBy: 'invoiceDate',
      sortDescending: true
    };

    this.invoiceService.getInvoices(query).subscribe({
      next: (response) => {
        this.invoices = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading invoices:', error);
        this.snackBar.open('Failed to load invoices', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadInvoices();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.vendorNameFilter = undefined;
    this.pageNumber = 1;
    this.loadInvoices();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadInvoices();
  }

  editInvoice(invoice: Invoice): void {
    this.router.navigate(['/invoices/edit', invoice.id]);
  }

  deleteInvoice(invoice: Invoice): void {
    if (confirm(`Are you sure you want to delete Invoice "${invoice.invoiceNumber}"?`)) {
      this.invoiceService.deleteInvoice(invoice.id).subscribe({
        next: () => {
          this.snackBar.open('Invoice deleted successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadInvoices();
        },
        error: (error) => {
          console.error('Error deleting invoice:', error);
          this.snackBar.open('Failed to delete invoice', 'Close', {
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
      'Pending': 'pending',
      'Paid': 'paid',
      'Overdue': 'overdue',
      'Cancelled': 'cancelled'
    };
    return statusMap[status] || 'pending';
  }

  formatCurrency(amount: number): string {
    return this.currencyService.formatCurrency(amount);
  }

  formatDate(dateString: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}


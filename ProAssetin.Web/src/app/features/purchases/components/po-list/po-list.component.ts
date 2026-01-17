import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PurchaseOrderService, PurchaseOrder, PurchaseOrderQuery } from '../../../../core/services/purchase-order.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-po-list',
  templateUrl: './po-list.component.html',
  styleUrls: ['./po-list.component.scss']
})
export class POListComponent implements OnInit {
  purchaseOrders: PurchaseOrder[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;

  searchTerm = '';
  statusFilter: string | undefined = undefined;
  vendorIdFilter: number | undefined = undefined;

  statuses: string[] = ['Draft', 'Submitted', 'Approved', 'Received', 'Cancelled'];

  constructor(
    private poService: PurchaseOrderService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadPurchaseOrders();
  }

  loadPurchaseOrders(): void {
    this.loading = true;
    
    const query: PurchaseOrderQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      vendorId: this.vendorIdFilter,
      sortBy: 'poDate',
      sortDescending: true
    };

    this.poService.getPurchaseOrders(query).subscribe({
      next: (response) => {
        this.purchaseOrders = response.purchaseOrders;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading purchase orders:', error);
        this.snackBar.open('Failed to load purchase orders', 'Close', {
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
    this.loadPurchaseOrders();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.vendorIdFilter = undefined;
    this.pageNumber = 1;
    this.loadPurchaseOrders();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadPurchaseOrders();
  }

  editPO(po: PurchaseOrder): void {
    this.router.navigate(['/purchases/edit', po.id]);
  }

  deletePO(po: PurchaseOrder): void {
    if (confirm(`Are you sure you want to delete Purchase Order "${po.poNumber}"?`)) {
      this.poService.deletePurchaseOrder(po.id).subscribe({
        next: () => {
          this.snackBar.open('Purchase Order deleted successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadPurchaseOrders();
        },
        error: (error) => {
          console.error('Error deleting purchase order:', error);
          this.snackBar.open('Failed to delete purchase order', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
        }
      });
    }
  }

  approvePO(po: PurchaseOrder): void {
    if (confirm(`Are you sure you want to approve Purchase Order "${po.poNumber}"?`)) {
      this.poService.approvePurchaseOrder(po.id).subscribe({
        next: () => {
          this.snackBar.open('Purchase Order approved successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadPurchaseOrders();
        },
        error: (error) => {
          console.error('Error approving purchase order:', error);
          this.snackBar.open('Failed to approve purchase order', 'Close', {
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
      'Draft': 'draft',
      'Submitted': 'submitted',
      'Approved': 'approved',
      'Received': 'received',
      'Cancelled': 'cancelled'
    };
    return statusMap[status] || 'draft';
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


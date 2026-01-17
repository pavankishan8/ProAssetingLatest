import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { VendorService, Vendor, VendorQuery } from '../../../../core/services/vendor.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-vendor-list',
  templateUrl: './vendor-list.component.html',
  styleUrls: ['./vendor-list.component.scss']
})
export class VendorListComponent implements OnInit {
  vendors: Vendor[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;

  searchTerm = '';
  isActiveFilter: boolean | undefined = undefined;

  constructor(
    private vendorService: VendorService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadVendors();
  }

  loadVendors(): void {
    this.loading = true;
    
    const query: VendorQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      isActive: this.isActiveFilter,
      sortBy: 'vendorName',
      sortDescending: false
    };

    this.vendorService.getVendors(query).subscribe({
      next: (response) => {
        this.vendors = response.vendors;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading vendors:', error);
        this.snackBar.open('Failed to load vendors', 'Close', {
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
    this.loadVendors();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.isActiveFilter = undefined;
    this.pageNumber = 1;
    this.loadVendors();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadVendors();
  }

  editVendor(vendor: Vendor): void {
    this.router.navigate(['/vendors/edit', vendor.id]);
  }

  deleteVendor(vendor: Vendor): void {
    if (confirm(`Are you sure you want to delete "${vendor.vendorName}"?`)) {
      this.vendorService.deleteVendor(vendor.id).subscribe({
        next: () => {
          this.snackBar.open('Vendor deleted successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadVendors();
        },
        error: (error) => {
          console.error('Error deleting vendor:', error);
          this.snackBar.open('Failed to delete vendor', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
        }
      });
    }
  }

  toggleVendorStatus(vendor: Vendor): void {
    const update: { isActive: boolean } = {
      isActive: !vendor.isActive
    };

    this.vendorService.updateVendor(vendor.id, update).subscribe({
      next: (updated) => {
        vendor.isActive = updated.isActive;
        this.snackBar.open(
          `Vendor ${updated.isActive ? 'activated' : 'deactivated'} successfully`,
          'Close',
          {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          }
        );
      },
      error: (error) => {
        console.error('Error updating vendor status:', error);
        this.snackBar.open('Failed to update vendor status', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
      }
    });
  }
}


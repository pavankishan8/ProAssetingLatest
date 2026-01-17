import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AssetService } from '../../../../core/services/asset.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-asset-list',
  templateUrl: './asset-list.component.html',
  styleUrls: ['./asset-list.component.scss']
})
export class AssetListComponent implements OnInit {
  dataSource: { data: any[] } = { data: [] };
  loading = false;
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  searchTerm = '';
  selectedCategory = '';
  selectedStatus = '';
  categories: string[] = [];
  statuses: string[] = ['In-Stock', 'Repair', 'Sold', 'Damaged', 'E-Waste'];

  // Allocation dialog
  showAllocateDialog = false;
  selectedAsset: any = null;
  userSearchTerm = '';
  selectedUser: any = null;
  invalidUser = false;
  searchingUser = false;
  allocating = false;

  Math = Math;

  constructor(
    private assetService: AssetService,
    private router: Router,
    private http: HttpClient,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadAssets();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  loadCategories(): void {
    this.assetService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories || [];
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadAssets(): void {
    this.loading = true;
    const query = {
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      category: this.selectedCategory || undefined,
      status: this.selectedStatus || undefined
    };

    this.assetService.getAssets(query).subscribe({
      next: (response) => {
        console.log('Assets API Response:', response);
        this.dataSource.data = response.data || [];
        this.totalCount = response.totalCount || 0;
        console.log('Loaded assets:', this.dataSource.data.length, 'Total:', this.totalCount);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading assets:', error);
        console.error('Full error:', JSON.stringify(error, null, 2));
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.pageIndex = 0;
    this.loadAssets();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = '';
    this.selectedStatus = '';
    this.pageIndex = 0;
    this.loadAssets();
  }

  onPageChange(event: any): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadAssets();
  }

  previousPage(): void {
    if (this.pageIndex > 0) {
      this.pageIndex--;
      this.loadAssets();
    }
  }

  nextPage(): void {
    if (this.pageIndex < this.totalPages - 1) {
      this.pageIndex++;
      this.loadAssets();
    }
  }

  deleteAsset(id: number): void {
    if (confirm('Are you sure you want to delete this asset?')) {
      this.assetService.deleteAsset(id).subscribe({
        next: () => {
          this.snackBar.open('Asset deleted successfully', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loadAssets();
        },
        error: (error) => {
          console.error('Error deleting asset:', error);
          this.snackBar.open('Failed to delete asset', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
        }
      });
    }
  }

  editAsset(id: number): void {
    // Navigate to edit page or open edit dialog
    this.router.navigate(['/assets/edit', id]);
  }

  // Allocation methods
  openAllocateDialog(asset: any): void {
    this.selectedAsset = asset;
    this.showAllocateDialog = true;
    this.userSearchTerm = '';
    this.selectedUser = null;
    this.invalidUser = false;
  }

  closeAllocateDialog(): void {
    this.showAllocateDialog = false;
    this.selectedAsset = null;
    this.userSearchTerm = '';
    this.selectedUser = null;
    this.invalidUser = false;
  }

  searchUser(): void {
    if (!this.userSearchTerm.trim()) {
      this.invalidUser = true;
      return;
    }

    this.searchingUser = true;
    this.invalidUser = false;

    this.http.get<any>(`${environment.apiUrl}/auth/search?searchTerm=${encodeURIComponent(this.userSearchTerm.trim())}`).subscribe({
      next: (user) => {
        this.selectedUser = user;
        this.invalidUser = false;
        this.searchingUser = false;
      },
      error: (error) => {
        console.error('Error searching user:', error);
        this.invalidUser = true;
        this.selectedUser = null;
        this.searchingUser = false;
      }
    });
  }

  allocateAsset(): void {
    if (!this.selectedUser || !this.selectedAsset) {
      return;
    }

    this.allocating = true;

    // Update asset with assigned user
    const updateData = {
      assignedToUserId: this.selectedUser.id,
      status: 'In-Stock' // Keep status as In-Stock when allocated
    };

    this.assetService.updateAsset(this.selectedAsset.id, updateData).subscribe({
      next: () => {
        this.snackBar.open('Asset allocated successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.closeAllocateDialog();
        this.loadAssets();
        this.allocating = false;
      },
      error: (error) => {
        console.error('Error allocating asset:', error);
        this.snackBar.open(
          error.error?.message || 'Failed to allocate asset',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          }
        );
        this.allocating = false;
      }
    });
  }
}

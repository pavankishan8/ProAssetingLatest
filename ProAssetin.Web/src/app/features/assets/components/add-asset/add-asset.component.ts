import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AssetService } from '../../../../core/services/asset.service';
import { VendorService, Vendor } from '../../../../core/services/vendor.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-asset',
  templateUrl: './add-asset.component.html',
  styleUrls: ['./add-asset.component.scss']
})
export class AddAssetComponent implements OnInit {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  
  assetForm!: FormGroup;
  loading = false;
  uploading = false;
  uploadProgress = 0;
  selectedFile: File | null = null;
  isDragOver = false;
  selectedTab = 0;
  categories: string[] = ['Laptop', 'Desktop', 'Monitor', 'Keyboard', 'Mouse', 'Phone', 'Tablet', 'Other'];
  statuses: string[] = ['In-Stock', 'Repair', 'Sold', 'Damaged', 'E-Waste'];
  vendors: Vendor[] = [];

  currencySymbol = '$';

  constructor(
    private formBuilder: FormBuilder,
    private assetService: AssetService,
    private vendorService: VendorService,
    private currencyService: CurrencyService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.currencyService.getCurrency().subscribe(currency => {
      this.currencySymbol = this.currencyService.getCurrencySymbol(currency);
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.loadCategories();
    this.loadVendors();
  }

  initializeForm(): void {
    this.assetForm = this.formBuilder.group({
      assetId: ['', [Validators.required, Validators.maxLength(100)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      category: ['', Validators.required],
      location: ['', Validators.maxLength(50)],
      status: ['In-Stock', Validators.required],
      make: ['', Validators.maxLength(100)],
      model: ['', Validators.maxLength(100)],
      serialNumber: ['', Validators.maxLength(100)],
      purchasePrice: [null],
      purchaseDate: [null],
      warrantyExpiryDate: [null],
      description: ['', Validators.maxLength(200)],
      vendorId: [null]
    });
  }

  loadCategories(): void {
    this.assetService.getCategories().subscribe({
      next: (categories) => {
        if (categories && categories.length > 0) {
          this.categories = categories;
        }
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadVendors(): void {
    this.vendorService.getActiveVendors().subscribe({
      next: (vendors) => {
        this.vendors = vendors;
      },
      error: (error) => {
        console.error('Error loading vendors:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.assetForm.invalid) {
      this.markFormGroupTouched(this.assetForm);
      return;
    }

    this.loading = true;
    const formData = this.assetForm.value;

    this.assetService.createAsset(formData).subscribe({
      next: () => {
        this.snackBar.open('Asset added successfully!', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.router.navigate(['/assets/dashboard']);
      },
      error: (error) => {
        console.error('Error adding asset:', error);
        this.snackBar.open(
          error.error?.message || 'Failed to add asset. Please try again.',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          }
        );
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/assets/dashboard']);
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  get f() {
    return this.assetForm.controls;
  }

  // Excel Upload Methods
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.validateAndSetFile(file);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.validateAndSetFile(files[0]);
    }
  }

  validateAndSetFile(file: File): void {
    const validExtensions = ['.xlsx', '.xls'];
    const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();
    
    if (!validExtensions.includes(fileExtension)) {
      this.snackBar.open('Please select a valid Excel file (.xlsx or .xls)', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });
      return;
    }

    if (file.size > 10 * 1024 * 1024) { // 10MB limit
      this.snackBar.open('File size must be less than 10MB', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });
      return;
    }

    this.selectedFile = file;
  }

  clearFile(): void {
    this.selectedFile = null;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  onExcelUpload(): void {
    if (!this.selectedFile) {
      this.snackBar.open('Please select a file to upload', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });
      return;
    }

    this.uploading = true;
    this.uploadProgress = 0;

    this.assetService.uploadExcelFile(this.selectedFile).subscribe({
      next: (response) => {
        this.uploadProgress = 100;
        this.uploading = false;
        this.snackBar.open(
          response.message || 'Assets imported successfully!',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top',
            panelClass: ['success-snackbar']
          }
        );
        setTimeout(() => {
          this.clearFile();
          this.router.navigate(['/assets/dashboard']);
        }, 1500);
      },
      error: (error) => {
        console.error('Error uploading file:', error);
        this.uploadProgress = 0;
        this.uploading = false;
        this.snackBar.open(
          error.error?.message || 'Failed to upload file. Please check the format and try again.',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top',
            panelClass: ['error-snackbar']
          }
        );
      }
    });
  }

  downloadTemplate(): void {
    // Create Excel template structure
    // Column order: AssetId, Name, Category, Status, Location, Make, Model, SerialNumber, PurchasePrice, PurchaseDate, WarrantyExpiryDate, Description
    const templateData = [
      ['AssetId', 'Name', 'Category', 'Status', 'Location', 'Make', 'Model', 'SerialNumber', 'PurchasePrice', 'PurchaseDate', 'WarrantyExpiryDate', 'Description'],
      ['LAP-001', 'Dell Laptop', 'Laptop', 'In-Stock', 'Building A', 'Dell', 'Latitude 5520', 'SN123456', '1200', '01/15/2024', '01/15/2027', 'Office laptop'],
      ['MON-001', 'HP Monitor', 'Monitor', 'In-Stock', 'Building B', 'HP', 'EliteDisplay', 'SN789012', '300', '02/01/2024', '02/01/2027', '27 inch monitor']
    ];

    // Convert to CSV format (simpler than Excel, but works)
    const csvContent = templateData.map(row => row.join(',')).join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', 'Asset_Import_Template.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    this.snackBar.open('Template downloaded! Save as .xlsx format in Excel.', 'Close', {
      duration: 4000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }
}


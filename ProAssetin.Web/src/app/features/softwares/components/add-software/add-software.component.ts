import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { SoftwareService, CreateSoftware, UpdateSoftware, Software } from '../../../../core/services/software.service';
import { VendorService, Vendor } from '../../../../core/services/vendor.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-software',
  templateUrl: './add-software.component.html',
  styleUrls: ['./add-software.component.scss']
})
export class AddSoftwareComponent implements OnInit {
  softwareForm!: FormGroup;
  loading = false;
  isEditMode = false;
  softwareId: number | null = null;
  vendors: Vendor[] = [];
  categories: string[] = [];
  licenseTypes: string[] = ['Perpetual', 'Subscription', 'OEM', 'Open Source', 'Freeware', 'Trial', 'Volume License', 'Academic'];
  statuses: string[] = ['Active', 'Expired', 'Inactive', 'Renewal Pending', 'Trial', 'Suspended'];

  defaultCategories: string[] = [
    'Operating System',
    'Office Suite',
    'Development Tools',
    'Security',
    'Database',
    'Graphics & Design',
    'Communication',
    'Productivity',
    'Business Applications',
    'Other'
  ];

  currencySymbol = '$';

  constructor(
    private formBuilder: FormBuilder,
    private softwareService: SoftwareService,
    private vendorService: VendorService,
    private currencyService: CurrencyService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.currencyService.getCurrency().subscribe(currency => {
      this.currencySymbol = this.currencyService.getCurrencySymbol(currency);
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.loadVendors();
    this.loadCategories();
    
    // Check if in edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.softwareId = +id;
      this.loadSoftware(this.softwareId);
    }
  }

  initializeForm(): void {
    this.softwareForm = this.formBuilder.group({
      softwareName: ['', [Validators.required, Validators.maxLength(200)]],
      version: ['', Validators.maxLength(50)],
      licenseType: ['', Validators.maxLength(100)],
      licenseKey: ['', Validators.maxLength(100)],
      vendorId: [null],
      purchasePrice: [null],
      purchaseDate: [null],
      licenseExpiryDate: [null],
      totalLicenses: [null],
      usedLicenses: [null],
      description: ['', Validators.maxLength(500)],
      installationPath: ['', Validators.maxLength(200)],
      category: ['', Validators.maxLength(100)],
      status: ['Active', Validators.required]
    });

    // Calculate available licenses when total or used licenses change
    this.softwareForm.get('totalLicenses')?.valueChanges.subscribe(() => {
      this.calculateAvailableLicenses();
    });

    this.softwareForm.get('usedLicenses')?.valueChanges.subscribe(() => {
      this.calculateAvailableLicenses();
    });
  }

  calculateAvailableLicenses(): void {
    const total = this.softwareForm.get('totalLicenses')?.value || 0;
    const used = this.softwareForm.get('usedLicenses')?.value || 0;
    const available = total > 0 ? total - used : null;
    // Note: Available licenses will be calculated on the backend
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

  loadCategories(): void {
    this.softwareService.getCategories().subscribe({
      next: (categories) => {
        // Combine existing categories with default categories
        const allCategories = [...new Set([...this.defaultCategories, ...categories])];
        this.categories = allCategories.sort();
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        // Use default categories if API fails
        this.categories = this.defaultCategories;
      }
    });
  }

  loadSoftware(id: number): void {
    this.loading = true;
    this.softwareService.getSoftwareById(id).subscribe({
      next: (software) => {
        this.softwareForm.patchValue({
          softwareName: software.softwareName,
          version: software.version || '',
          licenseType: software.licenseType || '',
          licenseKey: software.licenseKey || '',
          vendorId: software.vendorId || null,
          purchasePrice: software.purchasePrice || null,
          purchaseDate: software.purchaseDate ? software.purchaseDate.split('T')[0] : null,
          licenseExpiryDate: software.licenseExpiryDate ? software.licenseExpiryDate.split('T')[0] : null,
          totalLicenses: software.totalLicenses || null,
          usedLicenses: software.usedLicenses || null,
          description: software.description || '',
          installationPath: software.installationPath || '',
          category: software.category || '',
          status: software.status
        });
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
        this.router.navigate(['/softwares/list']);
      }
    });
  }

  get f() {
    return this.softwareForm.controls;
  }

  onSubmit(): void {
    if (this.softwareForm.invalid) {
      this.markFormGroupTouched(this.softwareForm);
      return;
    }

    this.loading = true;
    const formData = this.softwareForm.value;

    if (this.isEditMode && this.softwareId) {
      const updateData: UpdateSoftware = {
        softwareName: formData.softwareName,
        version: formData.version || undefined,
        licenseType: formData.licenseType || undefined,
        licenseKey: formData.licenseKey || undefined,
        vendorId: formData.vendorId || undefined,
        purchasePrice: formData.purchasePrice || undefined,
        purchaseDate: formData.purchaseDate ? new Date(formData.purchaseDate + 'T00:00:00').toISOString() : undefined,
        licenseExpiryDate: formData.licenseExpiryDate ? new Date(formData.licenseExpiryDate + 'T00:00:00').toISOString() : undefined,
        totalLicenses: formData.totalLicenses || undefined,
        usedLicenses: formData.usedLicenses || undefined,
        description: formData.description || undefined,
        installationPath: formData.installationPath || undefined,
        category: formData.category || undefined,
        status: formData.status
      };

      this.softwareService.updateSoftware(this.softwareId, updateData).subscribe({
        next: () => {
          this.snackBar.open('Software updated successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/softwares/list']);
        },
        error: (error) => {
          console.error('Error updating software:', error);
          this.snackBar.open('Failed to update software', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loading = false;
        }
      });
    } else {
      const createData: CreateSoftware = {
        softwareName: formData.softwareName,
        version: formData.version || undefined,
        licenseType: formData.licenseType || undefined,
        licenseKey: formData.licenseKey || undefined,
        vendorId: formData.vendorId || undefined,
        purchasePrice: formData.purchasePrice || undefined,
        purchaseDate: formData.purchaseDate ? new Date(formData.purchaseDate + 'T00:00:00').toISOString() : undefined,
        licenseExpiryDate: formData.licenseExpiryDate ? new Date(formData.licenseExpiryDate + 'T00:00:00').toISOString() : undefined,
        totalLicenses: formData.totalLicenses || undefined,
        usedLicenses: formData.usedLicenses || undefined,
        description: formData.description || undefined,
        installationPath: formData.installationPath || undefined,
        category: formData.category || undefined,
        status: formData.status
      };

      this.softwareService.createSoftware(createData).subscribe({
        next: () => {
          this.snackBar.open('Software added successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/softwares/list']);
        },
        error: (error) => {
          console.error('Error creating software:', error);
          this.snackBar.open('Failed to add software', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loading = false;
        }
      });
    }
  }

  markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/softwares/list']);
  }
}


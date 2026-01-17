import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { VendorService, CreateVendor, UpdateVendor } from '../../../../core/services/vendor.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-vendor',
  templateUrl: './add-vendor.component.html',
  styleUrls: ['./add-vendor.component.scss']
})
export class AddVendorComponent implements OnInit {
  vendorForm!: FormGroup;
  loading = false;
  isEditMode = false;
  vendorId: number | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private vendorService: VendorService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.initializeForm();
    
    // Check if in edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.vendorId = +id;
      this.loadVendor(this.vendorId);
    }
  }

  initializeForm(): void {
    this.vendorForm = this.formBuilder.group({
      vendorName: ['', [Validators.required, Validators.maxLength(200)]],
      contactPerson: ['', Validators.maxLength(200)],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      phoneNumber: ['', Validators.maxLength(50)],
      address: ['', Validators.maxLength(500)],
      city: ['', Validators.maxLength(100)],
      state: ['', Validators.maxLength(50)],
      country: ['', Validators.maxLength(50)],
      gstNumber: ['', Validators.maxLength(50)],
      taxID: ['', Validators.maxLength(50)],
      isActive: [true]
    });
  }

  loadVendor(id: number): void {
    this.loading = true;
    this.vendorService.getVendor(id).subscribe({
      next: (vendor) => {
        this.vendorForm.patchValue({
          vendorName: vendor.vendorName,
          contactPerson: vendor.contactPerson || '',
          email: vendor.email || '',
          phoneNumber: vendor.phoneNumber || '',
          address: vendor.address || '',
          city: vendor.city || '',
          state: vendor.state || '',
          country: vendor.country || '',
          gstNumber: vendor.gstNumber || '',
          taxID: vendor.taxID || '',
          isActive: vendor.isActive
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading vendor:', error);
        this.snackBar.open('Failed to load vendor', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
        this.router.navigate(['/vendors/list']);
      }
    });
  }

  get f() {
    return this.vendorForm.controls;
  }

  onSubmit(): void {
    if (this.vendorForm.invalid) {
      this.markFormGroupTouched(this.vendorForm);
      return;
    }

    this.loading = true;
    const formData = this.vendorForm.value;

    if (this.isEditMode && this.vendorId) {
      const updateData: UpdateVendor = {
        vendorName: formData.vendorName,
        contactPerson: formData.contactPerson || undefined,
        email: formData.email || undefined,
        phoneNumber: formData.phoneNumber || undefined,
        address: formData.address || undefined,
        city: formData.city || undefined,
        state: formData.state || undefined,
        country: formData.country || undefined,
        gstNumber: formData.gstNumber || undefined,
        taxID: formData.taxID || undefined,
        isActive: formData.isActive
      };

      this.vendorService.updateVendor(this.vendorId, updateData).subscribe({
        next: () => {
          this.snackBar.open('Vendor updated successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/vendors/list']);
        },
        error: (error) => {
          console.error('Error updating vendor:', error);
          this.snackBar.open('Failed to update vendor', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loading = false;
        }
      });
    } else {
      const createData: CreateVendor = {
        vendorName: formData.vendorName,
        contactPerson: formData.contactPerson || undefined,
        email: formData.email || undefined,
        phoneNumber: formData.phoneNumber || undefined,
        address: formData.address || undefined,
        city: formData.city || undefined,
        state: formData.state || undefined,
        country: formData.country || undefined,
        gstNumber: formData.gstNumber || undefined,
        taxID: formData.taxID || undefined,
        isActive: formData.isActive
      };

      this.vendorService.createVendor(createData).subscribe({
        next: () => {
          this.snackBar.open('Vendor added successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/vendors/list']);
        },
        error: (error) => {
          console.error('Error creating vendor:', error);
          this.snackBar.open('Failed to add vendor', 'Close', {
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
    this.router.navigate(['/vendors/list']);
  }
}


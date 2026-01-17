import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { PurchaseOrderService, CreatePurchaseOrder, UpdatePurchaseOrder } from '../../../../core/services/purchase-order.service';
import { VendorService, Vendor } from '../../../../core/services/vendor.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-po',
  templateUrl: './add-po.component.html',
  styleUrls: ['./add-po.component.scss']
})
export class AddPOComponent implements OnInit {
  poForm!: FormGroup;
  loading = false;
  isEditMode = false;
  poId: number | null = null;
  vendors: Vendor[] = [];
  statuses: string[] = ['Draft', 'Submitted', 'Approved', 'Received', 'Cancelled'];

  currencySymbol = '$';

  constructor(
    private formBuilder: FormBuilder,
    private poService: PurchaseOrderService,
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
    
    // Check if in edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.poId = +id;
      this.loadPurchaseOrder(this.poId);
    } else {
      // Generate PO Number automatically for new POs
      this.generatePONumber();
    }
  }

  initializeForm(): void {
    this.poForm = this.formBuilder.group({
      poNumber: ['', [Validators.required, Validators.maxLength(100)]],
      vendorId: [null],
      totalAmount: [0, [Validators.required, Validators.min(0.01)]],
      poDate: [new Date().toISOString().split('T')[0], Validators.required],
      expectedDeliveryDate: [null],
      status: ['Draft', Validators.required],
      description: ['', Validators.maxLength(500)]
    });
  }

  generatePONumber(): void {
    const today = new Date();
    const prefix = 'PO';
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
    const poNumber = `${prefix}-${year}${month}${day}-${random}`;
    this.poForm.patchValue({ poNumber });
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

  loadPurchaseOrder(id: number): void {
    this.loading = true;
    this.poService.getPurchaseOrder(id).subscribe({
      next: (po) => {
        this.poForm.patchValue({
          poNumber: po.poNumber,
          vendorId: po.vendorId || null,
          totalAmount: po.totalAmount,
          poDate: po.poDate.split('T')[0],
          expectedDeliveryDate: po.expectedDeliveryDate ? po.expectedDeliveryDate.split('T')[0] : null,
          status: po.status,
          description: po.description || ''
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading purchase order:', error);
        this.snackBar.open('Failed to load purchase order', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
        this.router.navigate(['/purchases/list']);
      }
    });
  }

  get f() {
    return this.poForm.controls;
  }

  onSubmit(): void {
    if (this.poForm.invalid) {
      this.markFormGroupTouched(this.poForm);
      return;
    }

    this.loading = true;
    const formData = this.poForm.value;

    if (this.isEditMode && this.poId) {
      const updateData: UpdatePurchaseOrder = {
        poNumber: formData.poNumber,
        vendorId: formData.vendorId || undefined,
        totalAmount: formData.totalAmount,
        poDate: formData.poDate ? new Date(formData.poDate + 'T00:00:00').toISOString() : undefined,
        expectedDeliveryDate: formData.expectedDeliveryDate ? new Date(formData.expectedDeliveryDate + 'T00:00:00').toISOString() : undefined,
        status: formData.status,
        description: formData.description || undefined
      };

      this.poService.updatePurchaseOrder(this.poId, updateData).subscribe({
        next: () => {
          this.snackBar.open('Purchase Order updated successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/purchases/list']);
        },
        error: (error) => {
          console.error('Error updating purchase order:', error);
          this.snackBar.open('Failed to update purchase order', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loading = false;
        }
      });
    } else {
      const createData: CreatePurchaseOrder = {
        poNumber: formData.poNumber,
        vendorId: formData.vendorId || undefined,
        totalAmount: formData.totalAmount,
        poDate: new Date(formData.poDate + 'T00:00:00').toISOString(),
        expectedDeliveryDate: formData.expectedDeliveryDate ? new Date(formData.expectedDeliveryDate + 'T00:00:00').toISOString() : undefined,
        status: formData.status,
        description: formData.description || undefined
      };

      this.poService.createPurchaseOrder(createData).subscribe({
        next: () => {
          this.snackBar.open('Purchase Order created successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/purchases/list']);
        },
        error: (error) => {
          console.error('Error creating purchase order:', error);
          this.snackBar.open('Failed to create purchase order', 'Close', {
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
    this.router.navigate(['/purchases/list']);
  }
}


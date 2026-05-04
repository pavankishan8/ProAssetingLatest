import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { InvoiceService, CreateInvoice, UpdateInvoice } from '../../../../core/services/invoice.service';
import { PurchaseOrderService } from '../../../../core/services/purchase-order.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-invoice',
  templateUrl: './add-invoice.component.html',
  styleUrls: ['./add-invoice.component.scss']
})
export class AddInvoiceComponent implements OnInit {
  invoiceForm!: FormGroup;
  loading = false;
  isEditMode = false;
  invoiceId: number | null = null;
  purchaseOrders: any[] = [];
  statuses: string[] = ['Pending', 'Paid', 'Overdue', 'Cancelled'];

  currencySymbol = '$';

  constructor(
    private formBuilder: FormBuilder,
    private invoiceService: InvoiceService,
    private purchaseOrderService: PurchaseOrderService,
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
    this.loadStatuses();
    
    // Check if in edit mode
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.invoiceId = +id;
      this.loadInvoice(this.invoiceId);
    } else {
      // Generate Invoice Number automatically for new invoices
      this.generateInvoiceNumber();
    }
  }

  initializeForm(): void {
    this.invoiceForm = this.formBuilder.group({
      invoiceNumber: ['', [Validators.required, Validators.maxLength(100)]],
      vendorName: ['', Validators.maxLength(200)],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      invoiceDate: [new Date().toISOString().split('T')[0], Validators.required],
      dueDate: [null],
      status: ['Pending', Validators.required],
      description: ['', Validators.maxLength(500)],
      purchaseOrderNumber: ['', Validators.maxLength(100)]
    });
  }

  generateInvoiceNumber(): void {
    const today = new Date();
    const prefix = 'INV';
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
    const invoiceNumber = `${prefix}-${year}${month}${day}-${random}`;
    this.invoiceForm.patchValue({ invoiceNumber });
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

  loadInvoice(id: number): void {
    this.loading = true;
    this.invoiceService.getInvoice(id).subscribe({
      next: (invoice) => {
        this.invoiceForm.patchValue({
          invoiceNumber: invoice.invoiceNumber,
          vendorName: invoice.vendorName || '',
          amount: invoice.amount,
          invoiceDate: invoice.invoiceDate.split('T')[0],
          dueDate: invoice.dueDate ? invoice.dueDate.split('T')[0] : null,
          status: invoice.status,
          description: invoice.description || '',
          purchaseOrderNumber: invoice.purchaseOrderNumber || ''
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading invoice:', error);
        this.snackBar.open('Failed to load invoice', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
        this.router.navigate(['/invoices/list']);
      }
    });
  }

  get f() {
    return this.invoiceForm.controls;
  }

  onSubmit(): void {
    if (this.invoiceForm.invalid) {
      this.markFormGroupTouched(this.invoiceForm);
      return;
    }

    this.loading = true;
    const formData = this.invoiceForm.value;

    if (this.isEditMode && this.invoiceId) {
      const updateData: UpdateInvoice = {
        invoiceNumber: formData.invoiceNumber,
        vendorName: formData.vendorName || undefined,
        amount: formData.amount,
        invoiceDate: formData.invoiceDate ? new Date(formData.invoiceDate + 'T00:00:00').toISOString() : undefined,
        dueDate: formData.dueDate ? new Date(formData.dueDate + 'T00:00:00').toISOString() : undefined,
        status: formData.status,
        description: formData.description || undefined,
        purchaseOrderNumber: formData.purchaseOrderNumber || undefined
      };

      this.invoiceService.updateInvoice(this.invoiceId, updateData).subscribe({
        next: () => {
          this.snackBar.open('Invoice updated successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/invoices/list']);
        },
        error: (error) => {
          console.error('Error updating invoice:', error);
          this.snackBar.open('Failed to update invoice', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.loading = false;
        }
      });
    } else {
      const createData: CreateInvoice = {
        invoiceNumber: formData.invoiceNumber,
        vendorName: formData.vendorName || undefined,
        amount: formData.amount,
        invoiceDate: new Date(formData.invoiceDate + 'T00:00:00').toISOString(),
        dueDate: formData.dueDate ? new Date(formData.dueDate + 'T00:00:00').toISOString() : undefined,
        status: formData.status,
        description: formData.description || undefined,
        purchaseOrderNumber: formData.purchaseOrderNumber || undefined
      };

      this.invoiceService.createInvoice(createData).subscribe({
        next: () => {
          this.snackBar.open('Invoice created successfully!', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          });
          this.router.navigate(['/invoices/list']);
        },
        error: (error) => {
          console.error('Error creating invoice:', error);
          this.snackBar.open('Failed to create invoice', 'Close', {
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
    this.router.navigate(['/invoices/list']);
  }
}


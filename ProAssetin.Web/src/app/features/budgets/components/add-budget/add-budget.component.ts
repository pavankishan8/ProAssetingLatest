import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BudgetService, CreateBudget, UpdateBudget } from '../../../../core/services/budget.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-budget',
  templateUrl: './add-budget.component.html',
  styleUrls: ['./add-budget.component.scss']
})
export class AddBudgetComponent implements OnInit {
  budgetForm!: FormGroup;
  loading = false;
  isEditMode = false;
  budgetId: number | null = null;
  statuses: string[] = ['Active', 'Closed'];
  currencySymbol = '$';

  constructor(
    private formBuilder: FormBuilder,
    private budgetService: BudgetService,
    private currencyService: CurrencyService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.currencyService.getCurrency().subscribe(() => {
      this.currencySymbol = this.currencyService.getCurrencySymbol();
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    this.budgetService.getStatuses().subscribe({
      next: (s) => { this.statuses = s; },
      error: () => { }
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.budgetId = +id;
      this.loadBudget(this.budgetId);
    } else {
      const y = new Date().getFullYear();
      this.budgetForm.patchValue({ fiscalYear: y });
    }
  }

  initializeForm(): void {
    this.budgetForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(500)],
      fiscalYear: [new Date().getFullYear(), [Validators.required, Validators.min(2000), Validators.max(2100)]],
      category: ['', Validators.maxLength(100)],
      allocatedAmount: [0, [Validators.required, Validators.min(0.01)]],
      spentAmount: [0, [Validators.required, Validators.min(0)]],
      status: ['Active', Validators.required]
    });
  }

  loadBudget(id: number): void {
    this.loading = true;
    this.budgetService.getBudget(id).subscribe({
      next: (b) => {
        this.budgetForm.patchValue({
          name: b.name,
          description: b.description || '',
          fiscalYear: b.fiscalYear,
          category: b.category || '',
          allocatedAmount: b.allocatedAmount,
          spentAmount: b.spentAmount,
          status: b.status
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load budget', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/budgets/list']);
      }
    });
  }

  get f() {
    return this.budgetForm.controls;
  }

  onSubmit(): void {
    if (this.budgetForm.invalid) {
      this.markFormGroupTouched(this.budgetForm);
      return;
    }

    this.loading = true;
    const v = this.budgetForm.value;

    if (this.isEditMode && this.budgetId) {
      const updateData: UpdateBudget = {
        name: v.name,
        description: v.description || undefined,
        fiscalYear: v.fiscalYear,
        category: v.category || undefined,
        allocatedAmount: v.allocatedAmount,
        spentAmount: v.spentAmount,
        status: v.status
      };

      this.budgetService.updateBudget(this.budgetId, updateData).subscribe({
        next: () => {
          this.snackBar.open('Budget updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/budgets/list']);
        },
        error: () => {
          this.snackBar.open('Failed to update budget', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.loading = false;
        }
      });
    } else {
      const createData: CreateBudget = {
        name: v.name,
        description: v.description || undefined,
        fiscalYear: v.fiscalYear,
        category: v.category || undefined,
        allocatedAmount: v.allocatedAmount,
        spentAmount: v.spentAmount ?? 0,
        status: v.status
      };

      this.budgetService.createBudget(createData).subscribe({
        next: () => {
          this.snackBar.open('Budget created', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/budgets/list']);
        },
        error: () => {
          this.snackBar.open('Failed to create budget', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
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
    this.router.navigate(['/budgets/list']);
  }
}

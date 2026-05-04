import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ContractService, CreateContract, UpdateContract } from '../../../../core/services/contract.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-contract',
  templateUrl: './add-contract.component.html',
  styleUrls: ['./add-contract.component.scss']
})
export class AddContractComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEdit = false;
  id: number | null = null;
  statuses: string[] = [];
  currencySymbol = '$';

  constructor(
    private fb: FormBuilder,
    private contractService: ContractService,
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
    this.build();
    this.contractService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });

    const rid = this.route.snapshot.paramMap.get('id');
    if (rid) {
      this.isEdit = true;
      this.id = +rid;
      this.load(this.id);
    } else {
      this.genRef();
    }
  }

  build(): void {
    this.form = this.fb.group({
      contractReference: ['', [Validators.required, Validators.maxLength(100)]],
      title: ['', [Validators.required, Validators.maxLength(200)]],
      counterpartyName: ['', Validators.maxLength(200)],
      contractType: ['', Validators.maxLength(100)],
      startDate: [''],
      endDate: [''],
      renewalReminderDate: [''],
      contractValue: [null as number | null],
      status: ['Draft', Validators.required],
      notes: ['', Validators.maxLength(2000)]
    });
  }

  genRef(): void {
    const d = new Date();
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const r = String(Math.floor(Math.random() * 10000)).padStart(4, '0');
    this.form.patchValue({ contractReference: `CT-${y}${m}${day}-${r}` });
  }

  load(id: number): void {
    this.loading = true;
    this.contractService.getContract(id).subscribe({
      next: (row) => {
        this.form.patchValue({
          contractReference: row.contractReference,
          title: row.title,
          counterpartyName: row.counterpartyName || '',
          contractType: row.contractType || '',
          startDate: row.startDate ? row.startDate.split('T')[0] : '',
          endDate: row.endDate ? row.endDate.split('T')[0] : '',
          renewalReminderDate: row.renewalReminderDate ? row.renewalReminderDate.split('T')[0] : '',
          contractValue: row.contractValue ?? null,
          status: row.status,
          notes: row.notes || ''
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load contract', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/contracts/list']);
      }
    });
  }

  get f() {
    return this.form.controls;
  }

  private iso(d: string | null | undefined): string | undefined {
    if (!d) return undefined;
    return new Date(d + 'T00:00:00').toISOString();
  }

  private parseMoney(v: unknown): number | undefined {
    if (v === null || v === undefined || v === '') return undefined;
    const n = Number(v);
    return Number.isFinite(n) && n >= 0 ? n : undefined;
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const v = this.form.value;
    const val = this.parseMoney(v.contractValue);

    if (this.isEdit && this.id) {
      const body: UpdateContract = {
        contractReference: v.contractReference,
        title: v.title,
        counterpartyName: v.counterpartyName || undefined,
        contractType: v.contractType || undefined,
        startDate: this.iso(v.startDate),
        endDate: this.iso(v.endDate),
        renewalReminderDate: this.iso(v.renewalReminderDate),
        contractValue: val,
        status: v.status,
        notes: v.notes || undefined
      };
      this.contractService.updateContract(this.id, body).subscribe({
        next: () => {
          this.snackBar.open('Contract updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/contracts/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    } else {
      const body: CreateContract = {
        contractReference: v.contractReference,
        title: v.title,
        counterpartyName: v.counterpartyName || undefined,
        contractType: v.contractType || undefined,
        startDate: this.iso(v.startDate),
        endDate: this.iso(v.endDate),
        renewalReminderDate: this.iso(v.renewalReminderDate),
        contractValue: val,
        status: v.status,
        notes: v.notes || undefined
      };
      this.contractService.createContract(body).subscribe({
        next: () => {
          this.snackBar.open('Contract created', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/contracts/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    }
  }

  private err(e: HttpErrorResponse): void {
    this.snackBar.open(e.error?.message || e.message || 'Request failed', 'Close', {
      duration: 5000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
    this.loading = false;
  }

  cancel(): void {
    this.router.navigate(['/contracts/list']);
  }
}

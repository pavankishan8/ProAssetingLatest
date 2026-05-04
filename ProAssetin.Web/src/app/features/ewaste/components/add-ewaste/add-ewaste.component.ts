import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EWasteService, CreateEWasteDisposal, UpdateEWasteDisposal } from '../../../../core/services/ewaste.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-ewaste',
  templateUrl: './add-ewaste.component.html',
  styleUrls: ['./add-ewaste.component.scss']
})
export class AddEwasteComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEditMode = false;
  id: number | null = null;
  statuses: string[] = ['Scheduled', 'Collected', 'Disposed', 'Cancelled'];

  constructor(
    private fb: FormBuilder,
    private ewasteService: EWasteService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.ewasteService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });

    const rid = this.route.snapshot.paramMap.get('id');
    if (rid) {
      this.isEditMode = true;
      this.id = +rid;
      this.load(this.id);
    } else {
      this.generateReference();
    }
  }

  buildForm(): void {
    this.form = this.fb.group({
      disposalReference: ['', [Validators.required, Validators.maxLength(100)]],
      assetId: [null as number | null],
      itemDescription: ['', [Validators.required, Validators.maxLength(500)]],
      category: ['', Validators.maxLength(100)],
      quantity: [1, [Validators.required, Validators.min(1)]],
      estimatedWeightKg: [null as number | null],
      recyclerName: ['', Validators.maxLength(200)],
      pickupDate: [''],
      disposalDate: [''],
      certificateReference: ['', Validators.maxLength(200)],
      status: ['Scheduled', Validators.required],
      notes: ['', Validators.maxLength(1000)]
    });
  }

  generateReference(): void {
    const d = new Date();
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const r = String(Math.floor(Math.random() * 10000)).padStart(4, '0');
    this.form.patchValue({ disposalReference: `EW-${y}${m}${day}-${r}` });
  }

  load(id: number): void {
    this.loading = true;
    this.ewasteService.getDisposal(id).subscribe({
      next: (row) => {
        this.form.patchValue({
          disposalReference: row.disposalReference,
          assetId: row.assetId ?? null,
          itemDescription: row.itemDescription,
          category: row.category || '',
          quantity: row.quantity,
          estimatedWeightKg: row.estimatedWeightKg ?? null,
          recyclerName: row.recyclerName || '',
          pickupDate: row.pickupDate ? row.pickupDate.split('T')[0] : '',
          disposalDate: row.disposalDate ? row.disposalDate.split('T')[0] : '',
          certificateReference: row.certificateReference || '',
          status: row.status,
          notes: row.notes || ''
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load record', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/ewaste/list']);
      }
    });
  }

  get f() {
    return this.form.controls;
  }

  private toIsoDate(d: string | null | undefined): string | undefined {
    if (!d) return undefined;
    return new Date(d + 'T00:00:00').toISOString();
  }

  private parseAssetId(v: unknown): number | undefined {
    if (v === null || v === undefined || v === '') return undefined;
    const n = Number(v);
    return Number.isFinite(n) && n > 0 ? n : undefined;
  }

  private parseOptionalWeight(v: unknown): number | undefined {
    if (v === null || v === undefined || v === '') return undefined;
    const n = Number(v);
    return Number.isFinite(n) && n >= 0 ? n : undefined;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const v = this.form.value;
    const assetId = this.parseAssetId(v.assetId);
    const estKg = this.parseOptionalWeight(v.estimatedWeightKg);

    if (this.isEditMode && this.id) {
      const body: UpdateEWasteDisposal = {
        disposalReference: v.disposalReference,
        assetId,
        itemDescription: v.itemDescription,
        category: v.category || undefined,
        quantity: v.quantity,
        estimatedWeightKg: estKg,
        recyclerName: v.recyclerName || undefined,
        pickupDate: this.toIsoDate(v.pickupDate),
        disposalDate: this.toIsoDate(v.disposalDate),
        certificateReference: v.certificateReference || undefined,
        status: v.status,
        notes: v.notes || undefined
      };
      this.ewasteService.updateDisposal(this.id, body).subscribe({
        next: () => {
          this.snackBar.open('Record updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/ewaste/list']);
        },
        error: (e: HttpErrorResponse) => this.handleErr(e)
      });
    } else {
      const body: CreateEWasteDisposal = {
        disposalReference: v.disposalReference,
        assetId,
        itemDescription: v.itemDescription,
        category: v.category || undefined,
        quantity: v.quantity,
        estimatedWeightKg: estKg,
        recyclerName: v.recyclerName || undefined,
        pickupDate: this.toIsoDate(v.pickupDate),
        disposalDate: this.toIsoDate(v.disposalDate),
        certificateReference: v.certificateReference || undefined,
        status: v.status,
        notes: v.notes || undefined
      };
      this.ewasteService.createDisposal(body).subscribe({
        next: () => {
          this.snackBar.open('Record created', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/ewaste/list']);
        },
        error: (e: HttpErrorResponse) => this.handleErr(e)
      });
    }
  }

  private handleErr(e: HttpErrorResponse): void {
    const msg = e.error?.message || e.message || 'Request failed';
    this.snackBar.open(msg, 'Close', { duration: 5000, horizontalPosition: 'end', verticalPosition: 'top' });
    this.loading = false;
  }

  cancel(): void {
    this.router.navigate(['/ewaste/list']);
  }
}

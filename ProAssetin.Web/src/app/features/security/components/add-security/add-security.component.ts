import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import {
  SecurityIncidentService,
  CreateSecurityIncident,
  UpdateSecurityIncident
} from '../../../../core/services/security-incident.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-security',
  templateUrl: './add-security.component.html',
  styleUrls: ['./add-security.component.scss']
})
export class AddSecurityComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEdit = false;
  id: number | null = null;
  statuses: string[] = [];
  severities: string[] = [];

  constructor(
    private fb: FormBuilder,
    private securityService: SecurityIncidentService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.build();
    this.securityService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });
    this.securityService.getSeverities().subscribe({ next: (s) => { this.severities = s; }, error: () => {} });

    const rid = this.route.snapshot.paramMap.get('id');
    if (rid) {
      this.isEdit = true;
      this.id = +rid;
      this.load(this.id);
    } else {
      this.genRef();
      this.form.patchValue({ reportedDate: new Date().toISOString().split('T')[0] });
    }
  }

  build(): void {
    this.form = this.fb.group({
      incidentReference: ['', [Validators.required, Validators.maxLength(100)]],
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      category: ['', Validators.maxLength(100)],
      severity: ['Medium', Validators.required],
      status: ['Open', Validators.required],
      reportedDate: ['', Validators.required],
      resolvedDate: [''],
      affectedSystem: ['', Validators.maxLength(300)],
      assignedToName: ['', Validators.maxLength(200)],
      notes: ['', Validators.maxLength(1000)]
    });
  }

  genRef(): void {
    const d = new Date();
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const r = String(Math.floor(Math.random() * 10000)).padStart(4, '0');
    this.form.patchValue({ incidentReference: `SEC-${y}${m}${day}-${r}` });
  }

  load(id: number): void {
    this.loading = true;
    this.securityService.getIncident(id).subscribe({
      next: (row) => {
        this.form.patchValue({
          incidentReference: row.incidentReference,
          title: row.title,
          description: row.description || '',
          category: row.category || '',
          severity: row.severity,
          status: row.status,
          reportedDate: row.reportedDate.split('T')[0],
          resolvedDate: row.resolvedDate ? row.resolvedDate.split('T')[0] : '',
          affectedSystem: row.affectedSystem || '',
          assignedToName: row.assignedToName || '',
          notes: row.notes || ''
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load incident', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/security/list']);
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

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const v = this.form.value;

    if (this.isEdit && this.id) {
      const body: UpdateSecurityIncident = {
        incidentReference: v.incidentReference,
        title: v.title,
        description: v.description || undefined,
        category: v.category || undefined,
        severity: v.severity,
        status: v.status,
        reportedDate: this.iso(v.reportedDate),
        resolvedDate: this.iso(v.resolvedDate),
        affectedSystem: v.affectedSystem || undefined,
        assignedToName: v.assignedToName || undefined,
        notes: v.notes || undefined
      };
      this.securityService.updateIncident(this.id, body).subscribe({
        next: () => {
          this.snackBar.open('Incident updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/security/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    } else {
      const body: CreateSecurityIncident = {
        incidentReference: v.incidentReference,
        title: v.title,
        description: v.description || undefined,
        category: v.category || undefined,
        severity: v.severity,
        status: v.status,
        reportedDate: this.iso(v.reportedDate)!,
        resolvedDate: this.iso(v.resolvedDate),
        affectedSystem: v.affectedSystem || undefined,
        assignedToName: v.assignedToName || undefined,
        notes: v.notes || undefined
      };
      this.securityService.createIncident(body).subscribe({
        next: () => {
          this.snackBar.open('Incident logged', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/security/list']);
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
    this.router.navigate(['/security/list']);
  }
}

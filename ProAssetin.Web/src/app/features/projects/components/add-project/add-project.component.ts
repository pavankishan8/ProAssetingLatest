import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectService, CreateProject, UpdateProject } from '../../../../core/services/project.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-project',
  templateUrl: './add-project.component.html',
  styleUrls: ['./add-project.component.scss']
})
export class AddProjectComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  isEdit = false;
  id: number | null = null;
  statuses: string[] = [];
  priorities: string[] = [];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.build();
    this.projectService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });
    this.projectService.getPriorities().subscribe({ next: (s) => { this.priorities = s; }, error: () => {} });

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
      projectReference: ['', [Validators.required, Validators.maxLength(100)]],
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      status: ['Planning', Validators.required],
      priority: ['Medium', Validators.required],
      startDate: [''],
      endDate: [''],
      projectManagerName: ['', Validators.maxLength(200)],
      departmentOrClient: ['', Validators.maxLength(200)],
      notes: ['', Validators.maxLength(2000)]
    });
  }

  genRef(): void {
    const d = new Date();
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const r = String(Math.floor(Math.random() * 10000)).padStart(4, '0');
    this.form.patchValue({ projectReference: `PRJ-${y}${m}${day}-${r}` });
  }

  load(id: number): void {
    this.loading = true;
    this.projectService.getProject(id).subscribe({
      next: (row) => {
        this.form.patchValue({
          projectReference: row.projectReference,
          name: row.name,
          description: row.description || '',
          status: row.status,
          priority: row.priority,
          startDate: row.startDate ? row.startDate.split('T')[0] : '',
          endDate: row.endDate ? row.endDate.split('T')[0] : '',
          projectManagerName: row.projectManagerName || '',
          departmentOrClient: row.departmentOrClient || '',
          notes: row.notes || ''
        });
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load project', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
        this.router.navigate(['/projects/list']);
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
      const body: UpdateProject = {
        projectReference: v.projectReference,
        name: v.name,
        description: v.description || undefined,
        status: v.status,
        priority: v.priority,
        startDate: this.iso(v.startDate),
        endDate: this.iso(v.endDate),
        projectManagerName: v.projectManagerName || undefined,
        departmentOrClient: v.departmentOrClient || undefined,
        notes: v.notes || undefined
      };
      this.projectService.updateProject(this.id, body).subscribe({
        next: () => {
          this.snackBar.open('Project updated', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/projects/list']);
        },
        error: (e: HttpErrorResponse) => this.err(e)
      });
    } else {
      const body: CreateProject = {
        projectReference: v.projectReference,
        name: v.name,
        description: v.description || undefined,
        status: v.status,
        priority: v.priority,
        startDate: this.iso(v.startDate),
        endDate: this.iso(v.endDate),
        projectManagerName: v.projectManagerName || undefined,
        departmentOrClient: v.departmentOrClient || undefined,
        notes: v.notes || undefined
      };
      this.projectService.createProject(body).subscribe({
        next: () => {
          this.snackBar.open('Project created', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
          this.router.navigate(['/projects/list']);
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
    this.router.navigate(['/projects/list']);
  }
}

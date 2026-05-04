import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService, Project, ProjectQuery } from '../../../../core/services/project.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss']
})
export class ProjectListComponent implements OnInit {
  rows: Project[] = [];
  loading = false;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  searchTerm = '';
  statusFilter: string | undefined = undefined;
  priorityFilter: string | undefined = undefined;
  statuses: string[] = [];
  priorities: string[] = [];

  constructor(
    private projectService: ProjectService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.projectService.getStatuses().subscribe({ next: (s) => { this.statuses = s; }, error: () => {} });
    this.projectService.getPriorities().subscribe({ next: (s) => { this.priorities = s; }, error: () => {} });
    this.load();
  }

  load(): void {
    this.loading = true;
    const q: ProjectQuery = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter,
      priority: this.priorityFilter,
      sortBy: 'createdAt',
      sortDescending: true
    };
    this.projectService.getProjects(q).subscribe({
      next: (res) => {
        this.rows = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load projects', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' });
        this.loading = false;
      }
    });
  }

  apply(): void {
    this.pageNumber = 1;
    this.load();
  }

  clear(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.priorityFilter = undefined;
    this.pageNumber = 1;
    this.load();
  }

  page(p: number): void {
    this.pageNumber = p;
    this.load();
  }

  edit(row: Project): void {
    this.router.navigate(['/projects/edit', row.id]);
  }

  remove(row: Project): void {
    if (!confirm(`Delete project "${row.projectReference}"?`)) return;
    this.projectService.deleteProject(row.id).subscribe({
      next: () => {
        this.snackBar.open('Project deleted', 'Close', { duration: 2500, horizontalPosition: 'end', verticalPosition: 'top' });
        this.load();
      },
      error: () => this.snackBar.open('Delete failed', 'Close', { duration: 3000, horizontalPosition: 'end', verticalPosition: 'top' })
    });
  }

  d(v: string | null | undefined): string {
    if (!v) return '—';
    return v.split('T')[0];
  }

  priClass(p: string): string {
    const x = (p || '').toLowerCase();
    if (x === 'high') return 'hi';
    if (x === 'low') return 'lo';
    return 'med';
  }

  stClass(s: string): string {
    const x = (s || '').toLowerCase().replace(/\s+/g, '');
    if (x === 'completed') return 'done';
    if (x === 'cancelled') return 'can';
    if (x === 'onhold') return 'hold';
    if (x === 'active') return 'act';
    return 'plan';
  }
}

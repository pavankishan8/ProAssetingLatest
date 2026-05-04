import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Project {
  id: number;
  projectReference: string;
  name: string;
  description?: string | null;
  status: string;
  priority: string;
  startDate?: string | null;
  endDate?: string | null;
  projectManagerName?: string | null;
  departmentOrClient?: string | null;
  notes?: string | null;
  createdByUserName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateProject {
  projectReference: string;
  name: string;
  description?: string;
  status?: string;
  priority?: string;
  startDate?: string | null;
  endDate?: string | null;
  projectManagerName?: string;
  departmentOrClient?: string;
  notes?: string;
}

export interface UpdateProject {
  projectReference?: string;
  name?: string;
  description?: string;
  status?: string;
  priority?: string;
  startDate?: string | null;
  endDate?: string | null;
  projectManagerName?: string;
  departmentOrClient?: string;
  notes?: string;
}

export interface ProjectQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  priority?: string;
  startDateFrom?: string;
  startDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ProjectListResponse {
  data: Project[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private base = `${environment.apiUrl}/projects`;

  constructor(private http: HttpClient) {}

  getProjects(q: ProjectQuery = {}): Observable<ProjectListResponse> {
    let p = new HttpParams();
    if (q.pageNumber) p = p.set('pageNumber', String(q.pageNumber));
    if (q.pageSize) p = p.set('pageSize', String(q.pageSize));
    if (q.searchTerm) p = p.set('searchTerm', q.searchTerm);
    if (q.status) p = p.set('status', q.status);
    if (q.priority) p = p.set('priority', q.priority);
    if (q.startDateFrom) p = p.set('startDateFrom', q.startDateFrom);
    if (q.startDateTo) p = p.set('startDateTo', q.startDateTo);
    if (q.sortBy) p = p.set('sortBy', q.sortBy);
    if (q.sortDescending !== undefined) p = p.set('sortDescending', String(q.sortDescending));
    return this.http.get<ProjectListResponse>(this.base, { params: p });
  }

  getProject(id: number): Observable<Project> {
    return this.http.get<Project>(`${this.base}/${id}`);
  }

  createProject(body: CreateProject): Observable<Project> {
    return this.http.post<Project>(this.base, body);
  }

  updateProject(id: number, body: UpdateProject): Observable<Project> {
    return this.http.put<Project>(`${this.base}/${id}`, body);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/statuses`);
  }

  getPriorities(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/priorities`);
  }
}

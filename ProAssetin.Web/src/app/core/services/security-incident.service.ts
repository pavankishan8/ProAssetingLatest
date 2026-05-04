import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SecurityIncident {
  id: number;
  incidentReference: string;
  title: string;
  description?: string | null;
  category?: string | null;
  severity: string;
  status: string;
  reportedDate: string;
  resolvedDate?: string | null;
  affectedSystem?: string | null;
  assignedToName?: string | null;
  notes?: string | null;
  createdByUserName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateSecurityIncident {
  incidentReference: string;
  title: string;
  description?: string;
  category?: string;
  severity?: string;
  status?: string;
  reportedDate: string;
  resolvedDate?: string | null;
  affectedSystem?: string;
  assignedToName?: string;
  notes?: string;
}

export interface UpdateSecurityIncident {
  incidentReference?: string;
  title?: string;
  description?: string;
  category?: string;
  severity?: string;
  status?: string;
  reportedDate?: string;
  resolvedDate?: string | null;
  affectedSystem?: string;
  assignedToName?: string;
  notes?: string;
}

export interface SecurityIncidentQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  severity?: string;
  reportedDateFrom?: string;
  reportedDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface SecurityIncidentListResponse {
  data: SecurityIncident[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class SecurityIncidentService {
  private base = `${environment.apiUrl}/security/incidents`;

  constructor(private http: HttpClient) {}

  getIncidents(query: SecurityIncidentQuery = {}): Observable<SecurityIncidentListResponse> {
    let p = new HttpParams();
    if (query.pageNumber) p = p.set('pageNumber', String(query.pageNumber));
    if (query.pageSize) p = p.set('pageSize', String(query.pageSize));
    if (query.searchTerm) p = p.set('searchTerm', query.searchTerm);
    if (query.status) p = p.set('status', query.status);
    if (query.severity) p = p.set('severity', query.severity);
    if (query.reportedDateFrom) p = p.set('reportedDateFrom', query.reportedDateFrom);
    if (query.reportedDateTo) p = p.set('reportedDateTo', query.reportedDateTo);
    if (query.sortBy) p = p.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) p = p.set('sortDescending', String(query.sortDescending));
    return this.http.get<SecurityIncidentListResponse>(this.base, { params: p });
  }

  getIncident(id: number): Observable<SecurityIncident> {
    return this.http.get<SecurityIncident>(`${this.base}/${id}`);
  }

  createIncident(body: CreateSecurityIncident): Observable<SecurityIncident> {
    return this.http.post<SecurityIncident>(this.base, body);
  }

  updateIncident(id: number, body: UpdateSecurityIncident): Observable<SecurityIncident> {
    return this.http.put<SecurityIncident>(`${this.base}/${id}`, body);
  }

  deleteIncident(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/statuses`);
  }

  getSeverities(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/severities`);
  }
}

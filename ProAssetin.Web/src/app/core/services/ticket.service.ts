import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Ticket {
  id: number;
  taskTitle: string;
  taskAssignedToName?: string | null;
  taskState: string;
  priority?: string | null;
  description?: string | null;
  resolution?: string | null;
  createdByUserName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  resolvedAt?: string | null;
}

export interface CreateTicket {
  taskTitle: string;
  taskAssignedToName?: string;
  taskState?: string;
  priority?: string;
  description?: string;
}

export interface UpdateTicket {
  taskTitle?: string;
  taskAssignedToName?: string;
  taskState?: string;
  priority?: string;
  description?: string;
  resolution?: string;
}

export interface TicketQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  taskState?: string;
  priority?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface TicketListResponse {
  data: Ticket[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class TicketService {
  private base = `${environment.apiUrl}/tickets`;

  constructor(private http: HttpClient) {}

  getTickets(q: TicketQuery = {}): Observable<TicketListResponse> {
    let p = new HttpParams();
    if (q.pageNumber) p = p.set('pageNumber', String(q.pageNumber));
    if (q.pageSize) p = p.set('pageSize', String(q.pageSize));
    if (q.searchTerm) p = p.set('searchTerm', q.searchTerm);
    if (q.taskState) p = p.set('taskState', q.taskState);
    if (q.priority) p = p.set('priority', q.priority);
    if (q.sortBy) p = p.set('sortBy', q.sortBy);
    if (q.sortDescending !== undefined) p = p.set('sortDescending', String(q.sortDescending));
    return this.http.get<TicketListResponse>(this.base, { params: p });
  }

  getTicket(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.base}/${id}`);
  }

  createTicket(body: CreateTicket): Observable<Ticket> {
    return this.http.post<Ticket>(this.base, body);
  }

  updateTicket(id: number, body: UpdateTicket): Observable<Ticket> {
    return this.http.put<Ticket>(`${this.base}/${id}`, body);
  }

  deleteTicket(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getTaskStates(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/states`);
  }

  getPriorities(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/priorities`);
  }
}

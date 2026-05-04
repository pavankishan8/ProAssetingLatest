import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Contract {
  id: number;
  contractReference: string;
  title: string;
  counterpartyName?: string | null;
  contractType?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  renewalReminderDate?: string | null;
  contractValue?: number | null;
  status: string;
  notes?: string | null;
  createdByUserName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateContract {
  contractReference: string;
  title: string;
  counterpartyName?: string;
  contractType?: string;
  startDate?: string | null;
  endDate?: string | null;
  renewalReminderDate?: string | null;
  contractValue?: number | null;
  status?: string;
  notes?: string;
}

export interface UpdateContract {
  contractReference?: string;
  title?: string;
  counterpartyName?: string;
  contractType?: string;
  startDate?: string | null;
  endDate?: string | null;
  renewalReminderDate?: string | null;
  contractValue?: number | null;
  status?: string;
  notes?: string;
}

export interface ContractQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  contractType?: string;
  endDateFrom?: string;
  endDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ContractListResponse {
  data: Contract[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class ContractService {
  private base = `${environment.apiUrl}/contracts`;

  constructor(private http: HttpClient) {}

  getContracts(q: ContractQuery = {}): Observable<ContractListResponse> {
    let p = new HttpParams();
    if (q.pageNumber) p = p.set('pageNumber', String(q.pageNumber));
    if (q.pageSize) p = p.set('pageSize', String(q.pageSize));
    if (q.searchTerm) p = p.set('searchTerm', q.searchTerm);
    if (q.status) p = p.set('status', q.status);
    if (q.contractType) p = p.set('contractType', q.contractType);
    if (q.endDateFrom) p = p.set('endDateFrom', q.endDateFrom);
    if (q.endDateTo) p = p.set('endDateTo', q.endDateTo);
    if (q.sortBy) p = p.set('sortBy', q.sortBy);
    if (q.sortDescending !== undefined) p = p.set('sortDescending', String(q.sortDescending));
    return this.http.get<ContractListResponse>(this.base, { params: p });
  }

  getContract(id: number): Observable<Contract> {
    return this.http.get<Contract>(`${this.base}/${id}`);
  }

  createContract(body: CreateContract): Observable<Contract> {
    return this.http.post<Contract>(this.base, body);
  }

  updateContract(id: number, body: UpdateContract): Observable<Contract> {
    return this.http.put<Contract>(`${this.base}/${id}`, body);
  }

  deleteContract(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/statuses`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Budget {
  id: number;
  name: string;
  description?: string;
  fiscalYear: number;
  category?: string;
  allocatedAmount: number;
  spentAmount: number;
  remainingAmount: number;
  status: string;
  createdByUserId?: string;
  createdByUserName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBudget {
  name: string;
  description?: string;
  fiscalYear: number;
  category?: string;
  allocatedAmount: number;
  spentAmount?: number;
  status?: string;
}

export interface UpdateBudget {
  name?: string;
  description?: string;
  fiscalYear?: number;
  category?: string;
  allocatedAmount?: number;
  spentAmount?: number;
  status?: string;
}

export interface BudgetQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  fiscalYear?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface BudgetListResponse {
  data: Budget[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = `${environment.apiUrl}/budgets`;

  constructor(private http: HttpClient) { }

  getBudgets(query: BudgetQuery = {}): Observable<BudgetListResponse> {
    let params = new HttpParams();
    if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
    if (query.status) params = params.set('status', query.status);
    if (query.fiscalYear != null) params = params.set('fiscalYear', query.fiscalYear.toString());
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', query.sortDescending.toString());

    return this.http.get<BudgetListResponse>(this.apiUrl, { params });
  }

  getBudget(id: number): Observable<Budget> {
    return this.http.get<Budget>(`${this.apiUrl}/${id}`);
  }

  createBudget(budget: CreateBudget): Observable<Budget> {
    return this.http.post<Budget>(this.apiUrl, budget);
  }

  updateBudget(id: number, budget: UpdateBudget): Observable<Budget> {
    return this.http.put<Budget>(`${this.apiUrl}/${id}`, budget);
  }

  deleteBudget(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/statuses`);
  }
}

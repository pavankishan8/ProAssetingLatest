import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Invoice {
  id: number;
  invoiceNumber: string;
  vendorName?: string;
  amount: number;
  invoiceDate: string;
  dueDate?: string;
  status: string;
  description?: string;
  purchaseOrderNumber?: string;
  createdByUserId?: string;
  createdByUserName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateInvoice {
  invoiceNumber: string;
  vendorName?: string;
  amount: number;
  invoiceDate: string;
  dueDate?: string;
  status?: string;
  description?: string;
  purchaseOrderNumber?: string;
}

export interface UpdateInvoice {
  invoiceNumber?: string;
  vendorName?: string;
  amount?: number;
  invoiceDate?: string;
  dueDate?: string;
  status?: string;
  description?: string;
  purchaseOrderNumber?: string;
}

export interface InvoiceQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  vendorName?: string;
  invoiceDateFrom?: string;
  invoiceDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface InvoiceListResponse {
  data: Invoice[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private apiUrl = `${environment.apiUrl}/invoices`;

  constructor(private http: HttpClient) { }

  getInvoices(query: InvoiceQuery = {}): Observable<InvoiceListResponse> {
    let params = new HttpParams();
    
    if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
    if (query.status) params = params.set('status', query.status);
    if (query.vendorName) params = params.set('vendorName', query.vendorName);
    if (query.invoiceDateFrom) params = params.set('invoiceDateFrom', query.invoiceDateFrom);
    if (query.invoiceDateTo) params = params.set('invoiceDateTo', query.invoiceDateTo);
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', query.sortDescending.toString());

    return this.http.get<InvoiceListResponse>(this.apiUrl, { params });
  }

  getInvoice(id: number): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  createInvoice(invoice: CreateInvoice): Observable<Invoice> {
    return this.http.post<Invoice>(this.apiUrl, invoice);
  }

  updateInvoice(id: number, invoice: UpdateInvoice): Observable<Invoice> {
    return this.http.put<Invoice>(`${this.apiUrl}/${id}`, invoice);
  }

  deleteInvoice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/statuses`);
  }
}


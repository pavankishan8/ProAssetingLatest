import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PurchaseOrder {
  id: number;
  poNumber: string;
  vendorId?: number;
  vendorName?: string;
  totalAmount: number;
  poDate: string;
  expectedDeliveryDate?: string;
  status: string;
  description?: string;
  createdByUserId?: string;
  createdByUserName?: string;
  approvedByUserId?: string;
  approvedByUserName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePurchaseOrder {
  poNumber: string;
  vendorId?: number;
  totalAmount: number;
  poDate: string;
  expectedDeliveryDate?: string;
  status?: string;
  description?: string;
}

export interface UpdatePurchaseOrder {
  poNumber?: string;
  vendorId?: number;
  totalAmount?: number;
  poDate?: string;
  expectedDeliveryDate?: string;
  status?: string;
  description?: string;
  approvedByUserId?: string;
}

export interface PurchaseOrderQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  vendorId?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface PurchaseOrderListResponse {
  purchaseOrders: PurchaseOrder[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private apiUrl = `${environment.apiUrl}/purchaseorders`;

  constructor(private http: HttpClient) { }

  getPurchaseOrders(query: PurchaseOrderQuery = {}): Observable<PurchaseOrderListResponse> {
    let params = new HttpParams();
    
    if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
    if (query.status) params = params.set('status', query.status);
    if (query.vendorId) params = params.set('vendorId', query.vendorId.toString());
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', query.sortDescending.toString());

    return this.http.get<PurchaseOrderListResponse>(this.apiUrl, { params });
  }

  getPurchaseOrder(id: number): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(`${this.apiUrl}/${id}`);
  }

  createPurchaseOrder(po: CreatePurchaseOrder): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(this.apiUrl, po);
  }

  updatePurchaseOrder(id: number, po: UpdatePurchaseOrder): Observable<PurchaseOrder> {
    return this.http.put<PurchaseOrder>(`${this.apiUrl}/${id}`, po);
  }

  deletePurchaseOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  approvePurchaseOrder(id: number): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(`${this.apiUrl}/${id}/approve`, {});
  }
}


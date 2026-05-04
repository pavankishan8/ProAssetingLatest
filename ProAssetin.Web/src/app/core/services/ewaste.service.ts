import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface EWasteDisposal {
  id: number;
  disposalReference: string;
  assetId?: number | null;
  assetTag?: string | null;
  itemDescription: string;
  category?: string | null;
  quantity: number;
  estimatedWeightKg?: number | null;
  recyclerName?: string | null;
  pickupDate?: string | null;
  disposalDate?: string | null;
  certificateReference?: string | null;
  status: string;
  notes?: string | null;
  createdByUserName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateEWasteDisposal {
  disposalReference: string;
  assetId?: number | null;
  itemDescription: string;
  category?: string;
  quantity?: number;
  estimatedWeightKg?: number | null;
  recyclerName?: string;
  pickupDate?: string | null;
  disposalDate?: string | null;
  certificateReference?: string;
  status?: string;
  notes?: string;
}

export interface UpdateEWasteDisposal {
  disposalReference?: string;
  assetId?: number | null;
  itemDescription?: string;
  category?: string;
  quantity?: number;
  estimatedWeightKg?: number | null;
  recyclerName?: string;
  pickupDate?: string | null;
  disposalDate?: string | null;
  certificateReference?: string;
  status?: string;
  notes?: string;
}

export interface EWasteQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  disposalDateFrom?: string;
  disposalDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface EWasteListResponse {
  data: EWasteDisposal[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class EWasteService {
  private apiUrl = `${environment.apiUrl}/ewaste`;

  constructor(private http: HttpClient) {}

  getDisposals(query: EWasteQuery = {}): Observable<EWasteListResponse> {
    let params = new HttpParams();
    if (query.pageNumber) params = params.set('pageNumber', String(query.pageNumber));
    if (query.pageSize) params = params.set('pageSize', String(query.pageSize));
    if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
    if (query.status) params = params.set('status', query.status);
    if (query.disposalDateFrom) params = params.set('disposalDateFrom', query.disposalDateFrom);
    if (query.disposalDateTo) params = params.set('disposalDateTo', query.disposalDateTo);
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', String(query.sortDescending));
    return this.http.get<EWasteListResponse>(this.apiUrl, { params });
  }

  getDisposal(id: number): Observable<EWasteDisposal> {
    return this.http.get<EWasteDisposal>(`${this.apiUrl}/${id}`);
  }

  createDisposal(body: CreateEWasteDisposal): Observable<EWasteDisposal> {
    return this.http.post<EWasteDisposal>(this.apiUrl, body);
  }

  updateDisposal(id: number, body: UpdateEWasteDisposal): Observable<EWasteDisposal> {
    return this.http.put<EWasteDisposal>(`${this.apiUrl}/${id}`, body);
  }

  deleteDisposal(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/statuses`);
  }
}

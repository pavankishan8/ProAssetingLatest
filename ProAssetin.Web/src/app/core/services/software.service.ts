import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Software {
  id: number;
  softwareName: string;
  version?: string;
  licenseType?: string;
  licenseKey?: string;
  vendorId?: number;
  vendorName?: string;
  purchasePrice?: number;
  purchaseDate?: string;
  licenseExpiryDate?: string;
  totalLicenses?: number;
  usedLicenses?: number;
  availableLicenses?: number;
  description?: string;
  installationPath?: string;
  category?: string;
  status: string;
  purchasedByUserId?: string;
  purchasedByUserName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateSoftware {
  softwareName: string;
  version?: string;
  licenseType?: string;
  licenseKey?: string;
  vendorId?: number;
  purchasePrice?: number;
  purchaseDate?: string;
  licenseExpiryDate?: string;
  totalLicenses?: number;
  usedLicenses?: number;
  description?: string;
  installationPath?: string;
  category?: string;
  status?: string;
}

export interface UpdateSoftware {
  softwareName?: string;
  version?: string;
  licenseType?: string;
  licenseKey?: string;
  vendorId?: number;
  purchasePrice?: number;
  purchaseDate?: string;
  licenseExpiryDate?: string;
  totalLicenses?: number;
  usedLicenses?: number;
  description?: string;
  installationPath?: string;
  category?: string;
  status?: string;
}

export interface SoftwareQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  category?: string;
  licenseType?: string;
  vendorId?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface SoftwareListResponse {
  software: Software[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class SoftwareService {
  private apiUrl = `${environment.apiUrl}/software`;

  constructor(private http: HttpClient) { }

  getSoftware(query: SoftwareQuery = {}): Observable<SoftwareListResponse> {
    let params = new HttpParams();
    
    if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
    if (query.status) params = params.set('status', query.status);
    if (query.category) params = params.set('category', query.category);
    if (query.licenseType) params = params.set('licenseType', query.licenseType);
    if (query.vendorId) params = params.set('vendorId', query.vendorId.toString());
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', query.sortDescending.toString());

    return this.http.get<SoftwareListResponse>(this.apiUrl, { params });
  }

  getSoftwareById(id: number): Observable<Software> {
    return this.http.get<Software>(`${this.apiUrl}/${id}`);
  }

  createSoftware(software: CreateSoftware): Observable<Software> {
    return this.http.post<Software>(this.apiUrl, software);
  }

  updateSoftware(id: number, software: UpdateSoftware): Observable<Software> {
    return this.http.put<Software>(`${this.apiUrl}/${id}`, software);
  }

  deleteSoftware(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categories`);
  }

  getLicenseTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/license-types`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/statuses`);
  }
}


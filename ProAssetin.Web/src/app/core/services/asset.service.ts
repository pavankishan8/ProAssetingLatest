import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AssetQuery {
  searchTerm?: string;
  category?: string;
  status?: string;
  location?: string;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  constructor(private http: HttpClient) {}

  getAssets(query: AssetQuery): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }
    if (query.category) {
      params = params.set('category', query.category);
    }
    if (query.status) {
      params = params.set('status', query.status);
    }
    if (query.location) {
      params = params.set('location', query.location);
    }

    return this.http.get(`${environment.apiUrl}/assets`, { params });
  }

  getAssetById(id: number): Observable<any> {
    return this.http.get(`${environment.apiUrl}/assets/${id}`);
  }

  createAsset(asset: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/assets`, asset);
  }

  updateAsset(id: number, asset: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/assets/${id}`, asset);
  }

  deleteAsset(id: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/assets/${id}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${environment.apiUrl}/assets/categories`);
  }

  getStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${environment.apiUrl}/assets/statuses`);
  }

  uploadExcelFile(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<any>(`${environment.apiUrl}/assets/import`, formData, {
      reportProgress: true
    });
  }
}


import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private http: HttpClient) {}

  getAssetSummary(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/reports/summary`);
  }

  getCategoryStats(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/reports/category-stats`);
  }

  getStatusStats(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/reports/status-stats`);
  }

  getWeeklyAdditions(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/reports/weekly-additions`);
  }

  getMonthlyAdditions(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/reports/monthly-additions`);
  }
}


import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CompanySettings {
  id: number;
  tenantId: string;
  companyLogo?: string; // Base64 encoded logo (data:image/png;base64,...)
  companyLogoMimeType?: string;
  companyName: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
  industry?: string;
  spocInformation?: string;
  gstNumber?: string;
  website?: string;
  currency?: string;
  timeZone?: string;
  dateFormat?: string;
  timeFormat?: string;
  defaultPageSize: number;
  enableEmailNotifications: boolean;
  enableSMSNotifications: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateCompanySettings {
  companyName?: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
  industry?: string;
  spocInformation?: string;
  gstNumber?: string;
  website?: string;
  currency?: string;
  timeZone?: string;
  dateFormat?: string;
  timeFormat?: string;
  defaultPageSize?: number;
  enableEmailNotifications?: boolean;
  enableSMSNotifications?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private apiUrl = `${environment.apiUrl}/settings`;

  constructor(private http: HttpClient) { }

  getCompanySettings(): Observable<CompanySettings> {
    return this.http.get<CompanySettings>(this.apiUrl);
  }

  updateCompanySettings(settings: UpdateCompanySettings): Observable<CompanySettings> {
    return this.http.put<CompanySettings>(this.apiUrl, settings);
  }

  uploadCompanyLogo(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('logo', file, file.name);

    return this.http.post(`${this.apiUrl}/logo`, formData, {
      reportProgress: true,
      observe: 'events'
    });
  }
}


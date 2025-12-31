import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../Environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public baseUrl: string = environment.baseUrl;

  constructor(private http: HttpClient) {}

  login(loginData: { email: string, password: string }): Observable<any> {
    return this.http.post(this.baseUrl + `api/login`, loginData);
  }

}

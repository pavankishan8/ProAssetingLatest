import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../Environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  public baseUrl: string = environment.baseUrl;

  constructor(private http: HttpClient) { }

  //To Add Asset Manually
  addAsset(assetData: any, employeeId: string) {
    const params = new HttpParams().set('userId', employeeId);
    return this.http.post(this.baseUrl + `api/assets/add`, assetData, { params });
  }

  //To Add Assets from XLSX
  uploadExcelFile(file: File, employeeId: string) {
    const formData = new FormData();
    formData.append('arquivo', file);

    const params = new HttpParams().set('userId', employeeId);
    return this.http.post(this.baseUrl + `api/assets/import`, formData, { params });
  }

  //To Get the Data from Tables
  getInventoryData(selectedValue: string) {  
    return this.http.get(this.baseUrl + `api/inventory/${selectedValue}`);
  }

  //To Search Asset by Asset ID
  searchAssetById(assetId: string): Observable<any> {
    return this.http.get(this.baseUrl + `api/asset/search?assetId=${assetId}`);
  }

  //To Get PreDefined-AssetID
  getPredefinedAssetID(employeeId: string) {
    return this.http.get<string>(this.baseUrl + `api/employee/predefined-asset?employeeId=${employeeId}`);
  }

  //To Update PreDefined-AssetID
  updateEmployeeConfiguration(model: any): Observable<any> {
    return this.http.post(this.baseUrl + `api/employee/config-update`, model);
  }

  //To Get the Image from DB
  getImage(employeeID: string) {
    return this.http.get(this.baseUrl + `api/employee/getImage?employeeID=${employeeID}`);
  }

  //To Get Assets Count
  getAssetCounts() {
    return this.http.get(this.baseUrl + `api/asset-counts`);
  }

  //To Get EmployeeID for Allocating
  getEmployeeById(employeeId: string): Observable<any> {
    return this.http.get(this.baseUrl + `api/employee/search?employeeId=${employeeId}`);
  }

  //To Allocate Asset
  allocateAsset(dialogData: any): Observable<any> {
    return this.http.post<any>(this.baseUrl + `api/allocate-asset`, dialogData);
  }

  //To Register Employee
  registerCompanyAndEmployee(data: any): Observable<any> {
    return this.http.post(this.baseUrl + `api/Register`, data);
  }

  //To Check Existing Users
  checkExistingUsers(data: { EEmail: string, UName: string }): Observable<any> {
    return this.http.post<any>(this.baseUrl + `api/check-existing-users`, data);
  }

  //To Get Employee Details
  getEmployeeDetails(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/GetEmployeeDetails`);
  }

  getAllEmployeeList(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/getAllEmployees`);
  }

  //To Get EndUser Details
  getEndUserDetails(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/GetEndUsers`);
  }

  //To Upload Image
  uploadImage(formData: FormData): Observable<any> {
    return this.http.post(this.baseUrl + `api/employee/uploadImage`, formData);
  }

  //To Update Employee Role
  updateEmployeeRole(data: { EmployeeID: string, Role: string }): Observable<any> {
    return this.http.put(this.baseUrl + `api/employee/updateRole`, data);
  }

  //To Get Asset by EmployeeID
  getAssetsByEmployeeId(employeeId: string): Observable<any[]> {
    const url = this.baseUrl + `api/searchByEmpID?employeeId=${employeeId}`;
    return this.http.get<any[]>(url);
  }

  //To Get EndUser Details
  getAllTickets(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/getTicketsCount`);
  }

  geTicketsByEmployeeId(employeeId: string): Observable<any[]> {
    const url = this.baseUrl + `api/getTicketsData?employeeId=${employeeId}`;
    return this.http.get<any[]>(url);
  }

  saveTicket(ticket: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/AddTicket`, ticket);
  }

  updateTicket(taskID: string, ticket: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/UpdateTicket?taskId=${taskID}`, ticket);
  }

  getAllTicketsGraph(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/TaskAssignedToNameCounts`);
  }

  geTicketsByTaskId(taskId: string): Observable<any[]> {
    const url = this.baseUrl + `api/tickets?taskId=${taskId}`;
    return this.http.get<any[]>(url);
  }

  getAllTicketsFull(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + `api/getAllTickets`);
  }

}

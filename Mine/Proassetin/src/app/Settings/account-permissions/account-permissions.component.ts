import { Component, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';

@Component({
  selector: 'app-account-permissions',
  templateUrl: './account-permissions.component.html',
  styleUrls: ['./account-permissions.component.scss']
})
export class AccountPermissionsComponent {

  @ViewChild('RoleChange') RoleChange!: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  
  employeeDetailsList: any[] = [];
  selectedRole: string | null = null;
  changeRole: string = "Select";
  selectData: any;
  selectedEmployee: any;
  userRole: string | null = null;
  pagedData: any[] = [];
  pageSize = 7;

  constructor(private service: NotificationService, public dialog: MatDialog, private apiserve: ApiService){}

  ngOnInit(): void {
    this.getEmployeeDetails();
    this.onPageChange({ pageIndex: 0, pageSize: this.pageSize, length: this.employeeDetailsList.length });

    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const userDataObject = JSON.parse(userData);
      this.userRole = userDataObject.Role;
    }
  }

  onPageChange(event: PageEvent) {
    const startIndex = event.pageIndex * event.pageSize;
    this.pagedData = this.employeeDetailsList.slice(startIndex, startIndex + event.pageSize);
  }

  getEmployeeDetails(): void {
    this.apiserve.getEndUserDetails().subscribe(
      (data) => {
        this.employeeDetailsList = data;
        this.onPageChange({ pageIndex: 0, pageSize: this.pageSize, length: this.employeeDetailsList.length });
        console.log(this.employeeDetailsList);
      },
      (error) => {
        console.error('Error getting employee details:', error);
      }
    );
  }

  // filterByRole(role: string | null): void {
  //   this.selectedRole = role === 'All' ? null : role;
  // }

  // get filteredEmployeeDetailsList(): any[] {
  //   if (!this.selectedRole) {
  //     return this.employeeDetailsList;
  //   } else {
  //     return this.employeeDetailsList.filter(employee => employee.Role === this.selectedRole);
  //   }
  // }

  getCellStyle(role: string): any {
    if (role === 'SuperAdmin') {
      return { 'background-color': '#007eff', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    } else if (role === 'Admin') {
      return { 'background-color': '#20c220', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    }  else if (role === 'Owner') {
      return { 'background-color': '#9b0000', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    } else if (role === 'Monitoring') {
      return { 'background-color': '#fdbf07', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    } else if (role === 'EndUser') {
      return { 'background-color': '#0fc3e8', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    } else if (role === 'QC') {
      return { 'background-color': '#fdbf07', 'color': 'white', 'border': 'none', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    } else if (role === '') {
      return { 'border': '0px', 'border-radius': '3px', 'width': '40%', 'height': '4vh' };
    }
  }

  openDialog(selectedEmployee){
    this.dialog.open(this.RoleChange, { panelClass: 'fullscreen-dialog', height: '22%', width: '30%', disableClose: true });

      const selectionData = {
        EmployeeID: selectedEmployee.EmployeeID,
        Role: selectedEmployee.Role,
        UserRole: this.userRole
      };

      this.selectData = selectionData;
  }

  saveChanges(){
    this.selectData.Role = this.changeRole;
    console.log(this.selectData);

    this.apiserve.updateEmployeeRole(this.selectData)
    .subscribe(
      response => {
        this.service.NotificationSuccess('Role updated successfully');
      },
      error => {
        this.service.NotificationFailure('Error updating role');
      }
    );
    this.dialog.closeAll();
  }

  onCancelClick(): void {
    this.dialog.closeAll();
    this.changeRole = "Select";
  }

  refresh(){
    this.getEmployeeDetails();
  }

  ngOnDestroy() {
    this.dialog.closeAll();
  }
  
}

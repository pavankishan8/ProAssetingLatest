import { Component, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import Chart from 'chart.js/auto';
import { ChartType } from 'chart.js';
import { MatDialog } from '@angular/material/dialog';
import { ApiService } from 'src/app/Services/api.service';
import { Observable, map, startWith } from 'rxjs';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { NotificationService } from 'src/app/Services/notification.service';
import { PageEvent } from '@angular/material/paginator';

export interface Ticket {
  title: string;
  assignedTo: string;
  state: string;
  description: string;
  discussion: string;
  priority: string;
  remainingWork: string;
}

export interface Employee {
  EmployeeID: string;
  Username: string;
}

@Component({
  selector: 'app-closed-tickets',
  templateUrl: './closed-tickets.component.html',
  styleUrls: ['./closed-tickets.component.scss']
})
export class ClosedTicketsComponent {

  @ViewChild('onTicketPopUp') onTicketPopUp!: TemplateRef<any>;
  
  pagedData: any[];
  pageSize = 7;

  employeeId: any;

  ticketData: any;
  filteredOptions: Observable<Employee[]>;
  
  ticketModel: Ticket = {
    title: '',
    assignedTo: '',
    state: '',
    description: '',
    discussion: '',
    priority: '',
    remainingWork: ''
  };

  panelOpenStateDes = true;
  panelOpenStateDis = true;
  panelOpenStateDet = true;

  myControl = new FormControl('');

  ticketDatabyID: any;
  employeeDetailsList: any[] = [];

  isSpinnerVisible= false;

  constructor(private service: NotificationService, private router: Router, public dialog: MatDialog, private apiserve: ApiService) {}

  ngOnInit(): void {
    this.isSpinnerVisible= true;
    const userData = sessionStorage.getItem('userData');
    this.employeeId = userData ? JSON.parse(userData).EmployeeID : null;
    this.fetchTickets();
    this.onPageChange({ pageIndex: 0, pageSize: this.pageSize, length: this.ticketData.length });
  }

  onPageChange(event: PageEvent) {
    const startIndex = event.pageIndex * event.pageSize;
    this.pagedData = this.ticketData.slice(startIndex, startIndex + event.pageSize);
  }

  fetchTickets(): void {
    this.apiserve.getAllTicketsFull().subscribe(
      (data) => {
        this.ticketData = data.filter(ticket => ticket.TaskState === "Done");
        this.onPageChange({ pageIndex: 0, pageSize: this.pageSize, length: this.ticketData.length });
        this.isSpinnerVisible= false;
      },
      (error) => {
        console.error('Error getting employee tickets:', error);
        this.isSpinnerVisible= false;
      }
    );
  }

  ticketView(taskID){
    console.log(taskID);
    this.apiserve.geTicketsByTaskId(taskID).subscribe(
      (data) => {
        this.ticketDatabyID = data;
        this.ticketModel.title = this.ticketDatabyID[0].TaskTitle;
        this.myControl.setValue(this.ticketDatabyID[0].TaskAssignedToName);
        this.ticketModel.state = this.ticketDatabyID[0].TaskState;
        this.ticketModel.description = this.ticketDatabyID[0].Description;
        this.ticketModel.discussion = this.ticketDatabyID[0].Discussion;
        this.ticketModel.priority = this.ticketDatabyID[0].Priority;
        this.ticketModel.remainingWork = this.ticketDatabyID[0].RemainingWork;
        console.log(this.ticketModel.state);
      },
      (error) => {
        console.error('Error getting employee tickets:', error);
      }
    );    
    this.dialog.open(this.onTicketPopUp, { panelClass: 'fullscreen-dialog', height: '80%', width: '70%' });
    this.getEmployeeDetails();
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedUsername = event.option.value;
    const selectedEmployee = this.employeeDetailsList.find(employee => employee.Username === selectedUsername);
    if (selectedEmployee) {
        this.ticketModel.assignedTo = selectedEmployee.EmployeeID;
    }
  }

  updateTicket(taskID){
    if (this.ticketModel.assignedTo === null || this.ticketModel.assignedTo === "") {
      this.ticketModel.assignedTo = this.ticketDatabyID[0]?.TaskAssignedToID;
    }
    
    this.apiserve.updateTicket(taskID, this.ticketModel).subscribe(
      (response) => {
  
        this.service.NotificationSuccess('Successfully updated a ticket');
      },
      (error) => {
        this.service.NotificationFailure('Failed to update a ticket');
        console.error('Error saving ticket:', error);
      }
    );
  }

  onTicketClose() {
    //this.dialog.open(this.childPopUp, { panelClass: 'fullscreen-dialog', height: '30%', width: '30%' });
    this.dialog.closeAll();
  }

  getEmployeeDetails(): void {
    this.apiserve.getAllEmployeeList().subscribe(
      (data) => {
        this.employeeDetailsList = data;
  
        this.filteredOptions = this.myControl.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value))
        );
      },
      (error) => {
        console.error('Error getting employee details:', error);
      }
    );
  }

  private _filter(value: string): Employee[] {
    const filterValue = value.toLowerCase();
    return this.employeeDetailsList.filter(option => option.Username.toLowerCase().includes(filterValue));
  }
}

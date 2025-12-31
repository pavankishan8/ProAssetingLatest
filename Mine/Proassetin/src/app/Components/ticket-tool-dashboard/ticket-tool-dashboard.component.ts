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

export interface Employee {
  EmployeeID: string;
  Username: string;
}

export interface Ticket {
  title: string;
  assignedTo: string;
  state: string;
  description: string;
  discussion: string;
  priority: string;
  remainingWork: string;
}
interface TicketsData {
  TotalRecords: number;
  StateCounts: {
    [key: string]: number;
  };
}
@Component({
  selector: 'app-ticket-tool-dashboard',
  templateUrl: './ticket-tool-dashboard.component.html',
  styleUrls: ['./ticket-tool-dashboard.component.scss']
})
export class TicketToolDashboardComponent {
  userRole: string | null = null;
  firstName: string | null = null;
  lastName: string | null = null;
  fullName: string | null = null;

  totalRecords: number = 0;
  closedCount: number = 0;
  inProgressCount: number = 0;
  openCount: number = 0;
  reopenedCount: number = 0;
  rejectedCount: number = 0;
  pendingCount: number = 0;
  ticketsCount: number = 0;

  employeeId: any;
  ticketData: any[] = [];
  ticketDatabyID: any;
  loading: boolean = false;

  graphData: any[] = [];

  unsavedChanges = false;

  @ViewChild('onGraphPopUp') onGraphPopUp!: TemplateRef<any>;
  @ViewChild('onTicketPopUp') onTicketPopUp!: TemplateRef<any>;
  @ViewChild('childPopUp') childPopUp!: TemplateRef<any>;


  @ViewChild('inputField') inputField: ElementRef;
  myControl = new FormControl('');
  options: string[] = ['One', 'Two', 'Three'];
  filteredOptions: Observable<Employee[]>;
  panelOpenStateDes = true;
  panelOpenStateDis = true;
  panelOpenStateDet = true;
  employeeDetailsList: any[] = [];

  ticketModel: Ticket = {
    title: '',
    assignedTo: '',
    state: '',
    description: '',
    discussion: '',
    priority: '',
    remainingWork: ''
  };

  constructor(private service: NotificationService, private router: Router, public dialog: MatDialog, private apiserve: ApiService) {}

  ngOnInit(): void {
    this.getAllTicketsGraph();
    this.getAllTicketsCount();
    this.initializeHorizontalBarChart();
    const userData = sessionStorage.getItem('userData');
    this.employeeId = userData ? JSON.parse(userData).EmployeeID : null;
    if (userData) {
      const userDataObject = JSON.parse(userData);
      this.userRole = userDataObject.Role;
      this.firstName = userDataObject.FirstName;
      this.lastName = userDataObject.LastName;
      this.fullName = this.firstName + ' ' + this.lastName;
    }
    this.fetchTickets(this.employeeId);
  }

  initializeHorizontalBarChart() {
    const ctx = document.getElementById('horizontalBarChart') as HTMLCanvasElement;
    if (!ctx) {
      console.error('Canvas element not found!');
      return;
    }

    this.apiserve.getAllTicketsGraph().subscribe(
      (data: any) => {
        this.graphData = data;

        const sortedData = this.graphData.slice().sort((a, b) => b.data - a.data);
        const sortedLabels = sortedData.map(item => item.label);

        const horizontalBarChart = new Chart(ctx, {
          type: 'bar' as ChartType,
          data: {
            labels: sortedLabels,
            datasets: [{
              label: 'Tickets',
              data: sortedData.map(item => item.data),
              backgroundColor: [
                '#19b5e5',
                '#f58b1f',
                '#fbbc3d',
                '#71338d',
                '#a8ce4b',
              ]
            }]
          },
          options: {
            indexAxis: 'y',
            aspectRatio: 1,
            scales: {
              y: {
                beginAtZero: true,
                grid: {
                  display: false,
                }
              },
              x: {
                grid: {
                  display: false,
                }
              }
            },
            plugins: {
              legend: {
                display: false,
                onClick: (e, legendItem) => {
                  e.native.stopPropagation();
                }
              }
            }
          }
        });
      },
      (error) => {
        console.error('Error getting task assigned to name counts:', error);
      }
    );
  }

  onGraphExp() {
    this.dialog.open(this.onGraphPopUp, { panelClass: 'fullscreen-dialog', height: '77%', width: '50%' });

    const ctx = document.getElementById('BarChart') as HTMLCanvasElement;
    if (!ctx) {
      console.error('Canvas element not found!');
      return;
    }

    this.apiserve.getAllTicketsGraph().subscribe(
      (data: any) => {
        this.graphData = data;

        const sortedData = this.graphData.slice().sort((a, b) => b.data - a.data);
        const sortedLabels = sortedData.map(item => item.label);

        const horizontalBarChart = new Chart(ctx, {
          type: 'bar' as ChartType,
          data: {
            labels: sortedLabels,
            datasets: [{
              label: 'Tickets',
              data: sortedData.map(item => item.data),
              backgroundColor: [
                '#19b5e5',
                '#f58b1f',
                '#fbbc3d',
                '#71338d',
                '#a8ce4b',
              ]
            }]
          },
          options: {
            indexAxis: 'y',
            aspectRatio: 1,
            scales: {
              y: {
                beginAtZero: true,
                grid: {
                  display: false,
                }
              },
              x: {
                grid: {
                  display: false,
                }
              }
            },
            plugins: {
              legend: {
                display: false,
                onClick: (e, legendItem) => {
                  e.native.stopPropagation();
                }
              }
            }
          }
        });
      },
      (error) => {
        console.error('Error getting task assigned to name counts:', error);
      }
    );
  }

  ticketView(taskID){
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

  onGraphClose() {
    this.dialog.closeAll();
  }

  onTicketClose() {
    //this.dialog.open(this.childPopUp, { panelClass: 'fullscreen-dialog', height: '30%', width: '30%' });
    this.dialog.closeAll();
  }

  showButton() {
    const button = document.querySelector('.expBtn') as HTMLElement;
    if (button) {
        button.style.display = 'block';
    }
}

openChildPopUp() {
  this.dialog.open(this.childPopUp, { panelClass: 'fullscreen-dialog', height: '80%', width: '70%' });
}


hideButton() {
    const button = document.querySelector('.expBtn') as HTMLElement;
    if (button) {
        button.style.display = 'none';
    }
}

getAllTicketsCount(): void {
  this.apiserve.getAllTickets().subscribe(
    (data: any) => {
      this.totalRecords = data.TotalRecords;
      this.closedCount = data.StateCounts['Done'] || 0;
      this.inProgressCount = data.StateCounts['In Progress'] || 0;
      this.openCount = data.StateCounts['Open'] || 0;
      this.reopenedCount = data.StateCounts['Reopened'] || 0;
      this.rejectedCount = data.StateCounts['Rejected'] || 0;
      this.pendingCount = data.StateCounts['Pending'] || 0;
      this.ticketsCount = data.StateCounts['Tickets'] || 0;
    },
    (error) => {
      console.error('Error getting employee details:', error);
    }
  );
}

getAllTicketsGraph(): void {
  this.apiserve.getAllTicketsGraph().subscribe(
    (data: any) => {
      debugger
      this.graphData = data.map((item: any) => ({
        label: item.TaskAssignedToName,
        data: item.TaskCount
      }));
    },
    (error) => {
      console.error('Error getting employee details:', error);
    }
  );
}

fetchTickets(employeeId: string): void {
  this.loading = true;
  this.apiserve.geTicketsByEmployeeId(employeeId).subscribe(
    (data) => {
      this.ticketData = data || [];
      this.loading = false;
    },
    (error) => {
      console.error('Error getting employee tickets:', error);
      this.ticketData = [];
      this.loading = false;
    }
  );
}

getTaskStateClass(taskState: string): string {
  switch (taskState) {
    case 'In Progress':
      return 'fas fa-circle cold_color';
    case 'Open':
      return 'fas fa-circle open_color';
    case 'To Do':
      return 'fas fa-circle open_color';
    case 'Done':
      return 'fas fa-circle done_color';
    case 'Closed':
      return 'fas fa-circle closed_color';
    case 'Rejected':
        return 'fas fa-circle closed_color';  
    case 'Pending':
      return 'fas fa-circle warning_color';
    default:
      return '';
  }
}

sortByState() {
  this.ticketData.sort((a, b) => {
    return a.TaskState.localeCompare(b.TaskState);
  });
}

sortByTitle() {
  this.ticketData.sort((a, b) => {
    return a.TaskTitle.localeCompare(b.TaskTitle);
  });
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

displayFn(employee: Employee): string {
  return employee && employee.Username ? employee.Username : '';
}

saveTicket(): void {
  if(this.ticketModel.assignedTo == null || this.ticketModel.assignedTo == ''){
    this.service.NotificationFailure('Ticket should be assigned');
    return;
  }

  this.apiserve.saveTicket(this.ticketModel).subscribe(
    (response) => {
      this.ticketModel.title = '';
      this.ticketModel.state = '';
      this.ticketModel.discussion= '';
      this.ticketModel.priority= '';
      this.ticketModel.remainingWork= '';
      this.ticketModel.assignedTo = '';
      this.ticketModel.description = '';
      this.inputField.nativeElement.value = '';
      this.service.NotificationSuccess('Successfully created a ticket');
    },
    (error) => {
      this.service.NotificationFailure('Failed to create a ticket');
      console.error('Error saving ticket:', error);
    }
  );
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
    this.ticketModel.assignedTo = this.ticketDatabyID[0].TaskAssignedToID;
  }

  this.apiserve.updateTicket(taskID, this.ticketModel).subscribe(
    (response) => {

      this.service.NotificationSuccess('Successfully updated a ticket');
    },
    (error) => {
      this.service.NotificationFailure('Failed to create a ticket');
      console.error('Error saving ticket:', error);
    }
  );
}

}

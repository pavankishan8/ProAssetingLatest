import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { Observable, map, startWith } from 'rxjs';
import { ApiService } from 'src/app/Services/api.service';
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

@Component({
  selector: 'app-create-ticket',
  templateUrl: './create-ticket.component.html',
  styleUrls: ['./create-ticket.component.scss']
})
export class CreateTicketComponent {
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

  constructor(private service: NotificationService, public dialog: MatDialog, private apiserve: ApiService){}

  ngOnInit() {
    this.getEmployeeDetails();
  }

  getEmployeeDetails(): void {
    this.apiserve.getAllEmployeeList().subscribe(
      (data) => {
        this.employeeDetailsList = data;
        console.log(this.employeeDetailsList);

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
}

import { Component, ElementRef, Inject, TemplateRef, ViewChild, OnInit, OnDestroy, HostListener } from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {NgFor, NgIf} from '@angular/common';
import {MatSelectModule} from '@angular/material/select';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDialog, MAT_DIALOG_DATA, MatDialogRef, MatDialogModule} from '@angular/material/dialog';
import { ApiService } from 'src/app/Services/api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedDataService } from 'src/app/Services/shared-data.service';
import { NotificationService } from 'src/app/Services/notification.service';

interface Item {
  value: string;
  viewValue: string;
}

export interface DialogData {
  animal: 'panda' | 'unicorn' | 'lion';
}
@Component({
  selector: 'app-assets-page',
  templateUrl: './assets-page.component.html',
  styleUrls: ['./assets-page.component.scss']
})
export class AssetsPageComponent implements OnInit, OnDestroy {
  items: Item[] = [
    {value: 'In-Stock', viewValue: 'In-Stock'},
    {value: 'Allocated', viewValue: 'Allocated'},
    {value: 'Repair', viewValue: 'Repair'},
    {value: 'Sold', viewValue: 'Sold'},
    {value: 'Damaged', viewValue: 'Damaged'},
    {value: 'E-Waste', viewValue: 'E-Waste'},
  ];

  invEmp: boolean = false;
  isDropdownOpen: boolean = false;

  @ViewChild('allocatePopUp') allocatePopUp!: TemplateRef<any>;
  @ViewChild('searchBtn') searchButton: ElementRef;
  @ViewChild('dropdownContainer') dropdownContainer: ElementRef;

  dialogData: any;
  empSearch: any;
  empData: any;
  
  assetData: any = { data: [] }

  selectedItem: any;
  isSpinnerVisible= false;

  constructor(public dialog: MatDialog, private apiserve: ApiService, private service: NotificationService, private route: ActivatedRoute, private shared: SharedDataService){}

  ngOnInit() {
    this.isSpinnerVisible = true;
    this.shared.selectedValue$.subscribe((selectedValue) => {
      this.selectedItem = selectedValue;
      if (selectedValue !== null) {
        this.handleSelectedValue(selectedValue);
        this.isSpinnerVisible = false;
      } else {
        this.isSpinnerVisible = false;
      }
    });

  }

  onItemSelected(selectedValue: string) {
    this.handleSelectedValue(selectedValue);
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
    console.log('Dropdown toggled:', this.isDropdownOpen);
    console.log('Items:', this.items);
  }

  closeDropdown() {
    this.isDropdownOpen = false;
  }

  selectItem(value: string) {
    console.log('Item selected:', value);
    this.selectedItem = value;
    this.isDropdownOpen = false;
    this.onItemSelected(value);
  }

  getSelectedLabel(): string {
    const selected = this.items.find(item => item.value === this.selectedItem);
    return selected ? selected.viewValue : 'Select Asset Status';
  }

  trackByValue(index: number, item: Item): string {
    return item.value;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.isDropdownOpen && this.dropdownContainer && !this.dropdownContainer.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  handleSelectedValue(selectedValue: string) {
    this.isSpinnerVisible = true;
    this.apiserve.getInventoryData(selectedValue).subscribe((response: any) => {
      this.assetData = response.data;
      console.log(response.data);
    });
    this.isSpinnerVisible = false;
  }
  
  getCellStyle(level: string): any {
    if (level === 'Completed') {
      return { 'background-color': '#20c220', 'color': 'white' };
    } else if (level === 'In Progress') {
      return { 'background-color': '#007eff', 'color': 'white' };
    } else if (level === 'Not Yet') {
      return { 'background-color': 'red', 'color': 'white' };
    }  else if (level === 'Allocated') {
      return { 'background-color': '#007eff', 'color': 'white' };
    }  else if (level === '') {
      return { 'border': '0px', 'border-radius': '3px' };
    }
  }

  openDialog(selectedItem) {
    this.dialog.open(this.allocatePopUp, { panelClass: 'fullscreen-dialog', height: '75%', width: '65%', disableClose: true });

    const dialogData = {
      AssetID: selectedItem.AssetID,
      Serial_Number: selectedItem.Serial_Number,
      AssetType: selectedItem.AssetType,
      Make: selectedItem.Make,
      Processor: selectedItem.Processor,
      RAM: selectedItem.RAM,
      Status: selectedItem.Status,
  };
      this.dialogData = dialogData;
  }

  onCancelClick(): void {
    this.dialog.closeAll();
    this.empSearch = "";
    this.empData = "";
  }

  triggerSearch() {
    this.searchButton.nativeElement.click();
  }

  clearInputData() {
    this.empSearch = "";
    this.empData = "";
  }

  searchEmp(): void{
    if (this.empSearch) {
      this.apiserve.getEmployeeById(this.empSearch).subscribe(
        (employeeData) => {
          console.log(employeeData);
          this.empData = employeeData;
          this.invEmp = false;

          Object.assign(this.dialogData, {
            AssignedToUserID: this.empSearch,
            Email: this.empData.Email,
            Username: this.empData.Username,
            PhoneNumber: this.empData.PhoneNumber,
            Location: this.empData.Location,
            ReportingManager: this.empData.ReportingManager,
          });
        },
        (error) => {
          this.invEmp = true;
          console.error(error);
        }
      );
    } else {
      this.invEmp = true;
      console.error('Invalid Employee ID');
    }
  }

  allocateAsset(){
    if(this.empSearch){
     
    this.apiserve.allocateAsset(this.dialogData).subscribe(
      (response) => {
        this.service.NotificationSuccess(`Asset Allocated Successfully`);
        this.dialog.closeAll();
        this.empSearch = "";
        this.empData = "";
      },
      (error) => {
        this.service.NotificationFailure('Error allocating asset');
      }
    );
     }
     else{
      this.service.NotificationFailure(`Employee ID is required`);
     }
  }

  ngOnDestroy() {
    this.dialog.closeAll();
  }
}

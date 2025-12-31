import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, ViewChild, HostListener, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Notification } from 'angular2-notifications';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
import { ToastrService } from 'ngx-toastr';
import { SharedDataService } from 'src/app/Services/shared-data.service';

interface Item {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-assets-create',
  templateUrl: './assets-create.component.html',
  styleUrls: ['./assets-create.component.scss']
})
export class AssetsCreateComponent implements OnInit {

  horizontalPosition: MatSnackBarHorizontalPosition = 'right';
  verticalPosition: MatSnackBarVerticalPosition = 'bottom';

  @ViewChild('arquivo', { read: ElementRef }) fileInput!: ElementRef;
  @ViewChild('statusDropdown') statusDropdown: ElementRef;

  formData: any = {
    AssetType: '',
    Make: '',
    Model: '',
    Status: '',
    RAM: '',
    Processor: '',
    OS: '',
    Serial_Number: '',
    IMEI_Number: '',
    Purchase_Cost: '',
    Purchase_Year: '',
    MonthsInUse: '',
    NextRecycleDate: '',
    AssignedToUserID: '',
    RecieveDate: ''
  };
  
  items: Item[] = [
    {value: 'In-Stock', viewValue: 'In-Stock'},
    {value: 'Repair', viewValue: 'Repair'},
    {value: 'Sold', viewValue: 'Sold'},
    {value: 'Damaged', viewValue: 'Damaged'},
    {value: 'E-Waste', viewValue: 'E-Waste'},
  ];
  
  assetData = { Assets: [this.formData] };
  ifManCre: boolean = false;
  ifUpload: boolean = false;
  employeeId: any;
  isStatusDropdownOpen: boolean = false;
  selectedFileName: string = '';


  initialFormData = {
    AssetType: '',
    Make: '',
    Model: '',
    RAM: '',
    Status: '',
    Processor: '',
    OS: '',
    Serial_Number: '',
    IMEI_Number: '',
    Purchase_Cost: '',
    Purchase_Year: '',
    MonthsInUse: '',
    NextRecycleDate: '',
    AssignedToUserID: '', // Add other fields for different statuses
  };

  constructor(private http: HttpClient, private assetService: ApiService, private snackBar: MatSnackBar, private toastr: ToastrService, private service: NotificationService, private shared: SharedDataService){}

  ngOnInit(): void {
    const isLoggedIn = sessionStorage.getItem('userData');
    this.employeeId = isLoggedIn ? JSON.parse(isLoggedIn).EmployeeID : null;
  }

  manCreate(){
    this.ifManCre = true;
    if(this.ifManCre == true){
      this.ifUpload = false;
    }
  }

  onUpload(){
    this.ifUpload = true;
    if(this.ifUpload == true){
      this.ifManCre = false;
    }
  }

  onStatusChange(newStatus: string) {
    this.formData.Status = newStatus;
    if (newStatus === 'In-Stock') {
      this.resetFormForInStock();
    } else if (newStatus === 'Repair') {
      this.resetFormForRepair();
    } else if (newStatus === 'Sold') {
      this.resetFormForSold();
    } else if (newStatus === 'Damaged') {
      this.resetFormForDamaged();
    } else if (newStatus === 'E-Waste') {
      this.resetFormForEWaste();
    }
  }

  toggleStatusDropdown() {
    this.isStatusDropdownOpen = !this.isStatusDropdownOpen;
  }

  selectStatus(value: string) {
    this.formData.Status = value;
    this.isStatusDropdownOpen = false;
    this.onStatusChange(value);
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFileName = file.name;
    }
  }

  clearFile() {
    this.selectedFileName = '';
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

resetFormForInStock() {
  this.formData.AssetType = '';
  this.formData.Make = '';
  this.formData.Model = '';
  this.formData.RAM = ''; 
  this.formData.Processor = ''; 
  this.formData.OS = ''; 
  this.formData.Serial_Number = ''; 
  this.formData.IMEI_Number = ''; 
  this.formData.Purchase_Cost = ''; 
  this.formData.Purchase_Year = ''; 
  this.formData.MonthsInUse = ''; 
  this.formData.NextRecycleDate = ''; 
  this.formData.AssignedToUserID = ''; 
}

resetFormForRepair() {
  this.formData.AssetType = '';
  this.formData.Make = '';
  this.formData.Model = '';
  this.formData.RAM = ''; 
  this.formData.Processor = ''; 
  this.formData.OS = ''; 
  this.formData.Serial_Number = ''; 
  this.formData.IMEI_Number = ''; 
  this.formData.Purchase_Cost = ''; 
  this.formData.Purchase_Year = ''; 
  this.formData.MonthsInUse = ''; 
  this.formData.NextRecycleDate = ''; 
  this.formData.Vendor = ''; 
  this.formData.SentDate = ''; 
  this.formData.ReceiveDate = ''; 
  this.formData.Repair_Cost = '';
  this.formData.RepairStatus = '';
  this.formData.Tracking = ''; 
  this.formData.RepairNotes = '';
  this.formData.DeliveredBy = ''; 
}

resetFormForSold() {
  this.formData.AssetType = '';
  this.formData.Make = '';
  this.formData.Model = '';
  this.formData.RAM = '';
  this.formData.Processor = '';
  this.formData.OS = '';
  this.formData.Serial_Number = '';
  this.formData.IMEI_Number = '';
  this.formData.MonthsInUse = '';
  this.formData.Purchase_Cost = '';
  this.formData.NextRecycleDate = '';
  this.formData.Sold_Cost = '';
  this.formData.Purchase_Year = '';
  this.formData.Sold_Year = '';
  this.formData.SoldNotes = '';
  this.formData.SoldTo = '';
  this.formData.Approvals = '';
}

resetFormForDamaged() {
  this.formData.AssetType = '';
  this.formData.Make = '';
  this.formData.Model = '';
  this.formData.RAM = ''; 
  this.formData.Processor = '';
  this.formData.OS = '';
  this.formData.Serial_Number = '';
  this.formData.IMEI_Number = '';
  this.formData.MonthsInUse = '';
  this.formData.NextRecycleDate = '';
  this.formData.Purchase_Cost = '';
  this.formData.Purchase_Year = '';
  this.formData.DamagedNotes = '';
}

resetFormForEWaste() {
  this.formData.AssetType = '';
  this.formData.Make = '';
  this.formData.Model = '';
  this.formData.RAM = '';
  this.formData.Processor = '';
  this.formData.OS = '';
  this.formData.Serial_Number = '';
  this.formData.IMEI_Number = '';
  this.formData.MonthsInUse = '';
  this.formData.NextRecycleDate = '';
  this.formData.Purchase_Year = '';
  this.formData.EWaste_Vendor = '';
  this.formData.EWasteNotes = '';
  this.formData.EWasteApprovals = '';
}

onSubmit() {
    switch (this.formData.Status) {
      case 'In-Stock':
        if (this.formData.AssignedToUserID === null) {
          this.service.NotificationWarning('AssignedToUserID cannot be null in In-Stock status');
          return;
        }
        break;
      default:
        if (this.formData.Status === 'In-Stock') {
        for (const key in this.formData) {
          if (key !== 'AssignedToUserID' && !this.formData[key]) {
            this.service.NotificationWarning(`All fields except AssignedToUserID are required in ${this.formData.Status} status`);
            return;
          }
        }
      }
        break;
    }

    switch (this.formData.Status) {
      case 'In-Stock':
        this.assetData = {
          Assets: [
            {
              AssetType: this.formData.AssetType,
              Make: this.formData.Make,
              Model: this.formData.Model,
              RAM: this.formData.RAM,
              Status: this.formData.Status,
              Processor: this.formData.Processor,
              OS: this.formData.OS,
              Serial_Number: this.formData.Serial_Number,
              IMEI_Number: this.formData.IMEI_Number,
              Purchase_Cost: this.formData.Purchase_Cost,
              Purchase_Year: this.formData.Purchase_Year,
              MonthsInUse: this.formData.MonthsInUse,
              NextRecycleDate: this.formData.NextRecycleDate,
              AssignedToUserID: this.formData.AssignedToUserID,
            },
          ],
        };
        break;
  
      case 'Repair':
        this.assetData = {
          Assets: [
            {
              AssetType: this.formData.AssetType,
              Make: this.formData.Make,
              Model: this.formData.Model,
              RAM: this.formData.RAM,
              Status: this.formData.Status,
              Processor: this.formData.Processor,
              OS: this.formData.OS,
              Serial_Number: this.formData.Serial_Number,
              IMEI_Number: this.formData.IMEI_Number,
              Purchase_Cost: this.formData.Purchase_Cost,
              Purchase_Year: this.formData.Purchase_Year,
              MonthsInUse: this.formData.MonthsInUse,
              NextRecycleDate: this.formData.NextRecycleDate,
              Vendor: this.formData.Vendor,
              SentDate: this.formData.SentDate,
              ReceiveDate: this.formData.ReceiveDate,
              Repair_Cost: this.formData.Repair_Cost,
              RepairStatus: this.formData.RepairStatus,
              Tracking: this.formData.Tracking,
              RepairNotes: this.formData.RepairNotes,
              DeliveredBy: this.formData.DeliveredBy,
            },
          ],
        };
        break;
  
      case 'Sold':
        this.assetData = {
          Assets: [
            {
              AssetType: this.formData.AssetType,
              Make: this.formData.Make,
              Model: this.formData.Model,
              RAM: this.formData.RAM,
              Processor: this.formData.Processor,
              OS: this.formData.OS,
              Status: this.formData.Status,
              Serial_Number: this.formData.Serial_Number,
              IMEI_Number: this.formData.IMEI_Number,
              MonthsInUse: this.formData.MonthsInUse,
              Purchase_Cost: this.formData.Purchase_Cost,
              NextRecycleDate: this.formData.NextRecycleDate,
              Sold_Cost: this.formData.Sold_Cost,
              Purchase_Year: this.formData.Purchase_Year,
              Sold_Year: this.formData.Sold_Year,
              SoldNotes: this.formData.SoldNotes,
              SoldTo: this.formData.SoldTo,
              Approvals: this.formData.Approvals
            },
          ],
        };
        break;
  
      case 'Damaged':
        this.assetData = {
          Assets: [
            {
              AssetType: this.formData.AssetType,
              Make: this.formData.Make,
              Model: this.formData.Model,
              RAM: this.formData.RAM,
              Processor: this.formData.Processor,
              OS: this.formData.OS,
              Status: this.formData.Status,
              Serial_Number: this.formData.Serial_Number,
              IMEI_Number: this.formData.IMEI_Number,
              MonthsInUse: this.formData.MonthsInUse,
              NextRecycleDate: this.formData.NextRecycleDate,
              Purchase_Cost: this.formData.Purchase_Cost,
              Purchase_Year: this.formData.Purchase_Year,
              DamagedNotes: this.formData.DamagedNotes,
            },
          ],
        };
        break;
  
      case 'E-Waste':
        this.assetData = {
          Assets: [
            {
              AssetType: this.formData.AssetType,
              Make: this.formData.Make,
              Model: this.formData.Model,
              RAM: this.formData.RAM,
              Processor: this.formData.Processor,
              OS: this.formData.OS,
              Status: this.formData.Status,
              Serial_Number: this.formData.Serial_Number,
              IMEI_Number: this.formData.IMEI_Number,
              MonthsInUse: this.formData.MonthsInUse,
              NextRecycleDate: this.formData.NextRecycleDate,
              Purchase_Year: this.formData.Purchase_Year,
              EWaste_Vendor: this.formData.EWaste_Vendor,
              EWasteNotes: this.formData.EWasteNotes,
              EWasteApprovals: this.formData.EWasteApprovals,
            },
          ],
        };
        break;
      
      default:
        this.assetData = { Assets: [this.formData] };
        break;
    }

    if (this.assetData === null) {
      return; // Handle this case as needed
    }

  this.assetService.addAsset(this.assetData, this.employeeId).subscribe(
    (response) => {
      console.log('Asset added successfully:', response);
      this.service.NotificationSuccess(`Asset added successfully to ${this.formData.Status}`);
    },
    (error) => {
      console.error('Failed to add asset:', error);
      this.service.NotificationFailure('Failed to add asset');
    }
  );
}

  openSnackBar(message: string, panelClass: string) {
    this.snackBar.open(message, '', {
      duration: 5000,
      horizontalPosition: this.horizontalPosition,
      verticalPosition: this.verticalPosition,
    });
  }
  
  onFileUpload() {
    const file = this.fileInput.nativeElement.files[0];

    if (!file) {
      this.service.NotificationFailure('Please select a file to upload');
      return;
    }

    if (file) {
      this.assetService.uploadExcelFile(file, this.employeeId)
        .subscribe(
          response => {
            console.log('File upload successful:', response);
            this.service.NotificationSuccess('Assets added successfully');
            this.clearFile();
          },
          error => {
            console.error('File upload error:', error);
            this.service.NotificationFailure('Failed to add assets');
          }
        );
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.isStatusDropdownOpen && this.statusDropdown && !this.statusDropdown.nativeElement.contains(event.target)) {
      this.isStatusDropdownOpen = false;
    }
  }
}

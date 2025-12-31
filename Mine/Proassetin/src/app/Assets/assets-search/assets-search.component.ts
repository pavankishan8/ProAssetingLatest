import { Component, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';

@Component({
  selector: 'app-assets-search',
  templateUrl: './assets-search.component.html',
  styleUrls: ['./assets-search.component.scss']
})
export class AssetsSearchComponent implements OnInit {
  searchResult: Asset = {} as Asset;
  searchAssetId: string = '';
  searchbyEmpId: string = '';
  showResDiv: boolean = false;
  showEmpDiv: boolean = false;

  pagedData: any[] = [];
  pageSize = 5;
  rowData: RowData[] = [];

  expandedStateMap: Map<string, boolean> = new Map();

  constructor(private apiserve: ApiService, private service: NotificationService) { 
  }

  ngOnInit(): void {
    this.onPageChange({ pageIndex: 0, pageSize: this.pageSize, length: this.rowData.length });
  }

  searchAsset(assetId: string): void {
    if (!assetId || assetId.trim() === '') {
      this.service.NotificationFailure('Please enter an Asset ID');
      return;
    }
    
    this.searchResult = {} as Asset;
    
    this.apiserve.searchAssetById(this.searchAssetId).subscribe(
      (result: Asset) => {
        this.searchResult = result;
        this.showResDiv = true;
        this.showEmpDiv = false;
        if (!result || !result.AssetID) {
          this.service.NotificationFailure('Asset not found');
          this.showResDiv = false;
        }
      },
      (error) => {
        console.error('Search failed:', error);
        this.service.NotificationFailure('Failed to search asset');
        this.searchResult = {} as Asset;
        this.showResDiv = false;
      }
    );
  }

  clearBtn(){
    this.searchResult = {} as Asset;
    this.showResDiv = false;
  }

  toggleExpansion(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    const isActionClick = target.tagName === 'BUTTON';
  
    if (!isActionClick) {
      const assetId = this.pagedData[index].AssetID;

    this.expandedStateMap.set(assetId, !this.expandedStateMap.get(assetId));

    this.pagedData[index].expanded = !this.pagedData[index].expanded;
    }
  }

  onPageChange(event: PageEvent): void {
    const startIndex = event.pageIndex * event.pageSize;
    this.pagedData = this.rowData.slice(startIndex, startIndex + event.pageSize);

    this.pagedData.forEach(item => item.expanded = false);
  }

  fetchData(employeeId: string): void {
    if (!employeeId || employeeId.trim() === '') {
      this.service.NotificationFailure('Please enter an Employee ID');
      return;
    }
    
    this.apiserve.getAssetsByEmployeeId(employeeId).subscribe(
      (data) => {
        this.rowData = data || [];
        
        if (this.rowData.length === 0) {
          this.service.NotificationFailure('No Assets found for the Employee');
          this.showEmpDiv = false;
          return;
        }
        
        this.rowData.forEach(item => {
          const assetId = item.AssetID;
          item.expanded = this.expandedStateMap.has(assetId) ? this.expandedStateMap.get(assetId) : false;
        });

        this.pagedData = this.rowData.slice(0, this.pageSize);
        this.showEmpDiv = true;
        this.showResDiv = false;
      },
      (error) => {
        console.error('Fetch failed:', error);
        this.service.NotificationFailure('No Assets found for the Employee');
        this.showEmpDiv = false;
      }
    );
  }
  
}

export class Asset {
  AssetID: string = '';
  AssetType: string = '';
  AssignedToUserIDString: string = '';
  Make: string = '';
  Model: string = '';
  Status: string = '';
  RAM: string = '';
  Processor: string = '';
  OS: string = '';
  Serial_Number: string = '';
  IMEI_Number: string = '';
  Purchase_Cost: string = '';
  Purchase_Year: string = '';
  MonthsInUse: string = '';
  NextRecycleDate: string = '';
  AssignedToUserID: string = '';
  Vendor: string = '';
  SentDate: string = '';
  ReceiveDate: string = '';
  Repair_Cost: string = '';
  RepairStatus: string = '';
  Tracking: string = '';
  RepairNotes: string = '';
  DeliveredBy: string = '';
  DamagedNotes: string = '';
  Sold_Cost: string = '';
  Sold_Year: string = '';
  SoldNotes: string = '';
  SoldTo: string = '';
  Approvals: string = '';
  EWaste_Vendor: string = '';
  EWasteNotes: string = '';
  EWasteApprovals: string = '';

  Email: string = '';
  Username: string = '';
  PhoneNumber: string = '';
  Location: string = '';
  ReportingManager: string = '';
}

interface RowData {
  AssetID: string;
  AssetType: string;
  Make: string;
  Processor: string;
  RAM: string;
  Status: string;
  Serial_Number: string;
  AssignedToUserID: string;
  Email: string;
  Username: string;
  PhoneNumber: string;
  Location: string;
  ReportingManager: string;
  expanded: boolean;
}
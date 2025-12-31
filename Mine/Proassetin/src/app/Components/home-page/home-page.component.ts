import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../Services/api.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {
  assetCounts: any = {
    Active: 0,
    InStock: 0,
    Repair: 0,
    Damaged: 0,
    Sold: 0,
    EWaste: 0,
    Allocated: 0
  };
  
  ticketCounts: any = {
    TotalRecords: 0,
    StateCounts: {}
  };
  
  loading = true;
  totalAssets = 0;

  constructor(
    private apiService: ApiService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    
    // Load asset counts
    this.apiService.getAssetCounts().subscribe(
      (data: any) => {
        this.assetCounts = data;
        this.totalAssets = (data.Active || 0) + (data.InStock || 0) + (data.Repair || 0) + 
                          (data.Damaged || 0) + (data.Sold || 0) + (data.EWaste || 0) + 
                          (data.Allocated || 0);
        this.loading = false;
      },
      (error) => {
        console.error('Error loading asset counts:', error);
        this.toastr.error('Failed to load asset data', 'Error');
        this.loading = false;
      }
    );

    // Load ticket counts
    this.apiService.getAllTickets().subscribe(
      (data: any) => {
        this.ticketCounts = data;
      },
      (error) => {
        console.error('Error loading ticket counts:', error);
      }
    );
  }

  getPendingTicketsCount(): number {
    return this.ticketCounts.StateCounts?.['Pending'] || 
           this.ticketCounts.StateCounts?.['In Progress'] || 0;
  }

  getTotalTicketsCount(): number {
    return this.ticketCounts.TotalRecords || 0;
  }
}

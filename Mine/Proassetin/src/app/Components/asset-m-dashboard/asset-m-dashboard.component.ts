import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/Services/api.service';
import { SharedDataService } from 'src/app/Services/shared-data.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-asset-m-dashboard',
  templateUrl: './asset-m-dashboard.component.html',
  styleUrls: ['./asset-m-dashboard.component.scss']
})
export class AssetMDashboardComponent implements OnInit {
  public count: any = {
    InStock: 0,
    Repair: 0,
    Damaged: 0,
    Allocated: 0
  };
  selectedValue: string | null = null;
  loading = false;

  constructor(
    private sharedServe: SharedDataService, 
    private router: Router, 
    private apiserve: ApiService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadAssetCounts();
  }

  loadAssetCounts() {
    this.loading = true;
    this.apiserve.getAssetCounts().subscribe(
      (data: any) => {
        this.count = data || this.count;
        this.loading = false;
      },
      (error) => {
        console.error('Error loading asset counts:', error);
        this.toastr.error('Failed to load asset data', 'Error');
        this.loading = false;
      }
    );
  }

  redirectToAssetsPage() {
    if (this.selectedValue) {
    this.sharedServe.updateSelectedValue(this.selectedValue);
    this.router.navigate(['/Home/AssetPage']);
    }
  }
}

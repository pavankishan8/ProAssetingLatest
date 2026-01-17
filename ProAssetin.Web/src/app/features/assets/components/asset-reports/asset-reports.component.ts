import { Component, OnInit } from '@angular/core';
import { AssetService } from '../../../../core/services/asset.service';
import { ReportService } from '../../../../core/services/report.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-asset-reports',
  templateUrl: './asset-reports.component.html',
  styleUrls: ['./asset-reports.component.scss']
})
export class AssetReportsComponent implements OnInit {
  reports: any[] = [];
  loading = false;
  filters = {
    startDate: null as Date | null,
    endDate: null as Date | null,
    category: '',
    status: ''
  };
  categories: string[] = [];
  statuses: string[] = ['In-Stock', 'Repair', 'Sold', 'Damaged', 'E-Waste'];

  constructor(
    private assetService: AssetService,
    private reportService: ReportService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadReports();
  }

  loadCategories(): void {
    this.assetService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories || [];
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadReports(): void {
    this.loading = true;
    // In a real app, this would call a report service
    // For now, we'll use asset data to generate reports
    this.assetService.getAssets({ pageNumber: 1, pageSize: 1000 }).subscribe({
      next: (response) => {
        const assets = response.data || [];
        this.reports = this.generateReports(assets);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading reports:', error);
        this.loading = false;
      }
    });
  }

  generateReports(assets: any[]): any[] {
    const reports = [
      {
        title: 'Total Assets Report',
        type: 'summary',
        data: {
          total: assets.length,
          inStock: assets.filter(a => a.status === 'In-Stock').length,
          repair: assets.filter(a => a.status === 'Repair').length,
          sold: assets.filter(a => a.status === 'Sold').length,
          damaged: assets.filter(a => a.status === 'Damaged').length,
          ewaste: assets.filter(a => a.status === 'E-Waste').length,
          // Keep old property names for backward compatibility
          available: assets.filter(a => a.status === 'In-Stock').length,
          allocated: assets.filter(a => a.status === 'In-Stock').length,
          maintenance: assets.filter(a => a.status === 'Repair').length,
          disposed: assets.filter(a => a.status === 'E-Waste').length
        }
      },
      {
        title: 'Assets by Category',
        type: 'category',
        data: this.groupByCategory(assets)
      },
      {
        title: 'Assets by Status',
        type: 'status',
        data: this.groupByStatus(assets)
      }
    ];
    return reports;
  }

  groupByCategory(assets: any[]): any {
    const grouped: { [key: string]: number } = {};
    assets.forEach(asset => {
      const cat = asset.category || 'Uncategorized';
      grouped[cat] = (grouped[cat] || 0) + 1;
    });
    return grouped;
  }

  groupByStatus(assets: any[]): any {
    const grouped: { [key: string]: number } = {};
    assets.forEach(asset => {
      const status = asset.status || 'Unknown';
      grouped[status] = (grouped[status] || 0) + 1;
    });
    return grouped;
  }

  exportReport(reportType: string): void {
    this.snackBar.open(`Exporting ${reportType} report...`, 'Close', {
      duration: 2000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
    // In a real app, this would trigger a PDF/CSV export
  }

  filterReports(): void {
    this.loadReports();
  }

  clearFilters(): void {
    this.filters = {
      startDate: null,
      endDate: null,
      category: '',
      status: ''
    };
    this.loadReports();
  }

  getReportIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'summary': 'assessment',
      'category': 'pie_chart',
      'status': 'bar_chart'
    };
    return icons[type] || 'description';
  }

  getObjectEntries(obj: any): Array<{ key: string; value: any }> {
    return Object.keys(obj).map(key => ({ key, value: obj[key] }));
  }
}


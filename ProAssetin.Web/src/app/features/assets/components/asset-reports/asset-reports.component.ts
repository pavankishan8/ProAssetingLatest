import { Component, OnInit, OnDestroy } from '@angular/core';
import { AssetService } from '../../../../core/services/asset.service';
import { ReportService } from '../../../../core/services/report.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription, forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-asset-reports',
  templateUrl: './asset-reports.component.html',
  styleUrls: ['./asset-reports.component.scss']
})
export class AssetReportsComponent implements OnInit, OnDestroy {
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

  private subscriptions = new Subscription();

  constructor(
    private assetService: AssetService,
    private reportService: ReportService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadReports();
  }

  ngOnDestroy(): void {
    // Clean up subscriptions to prevent memory leaks
    this.subscriptions.unsubscribe();
  }

  loadCategories(): void {
    const sub = this.assetService.getCategories().pipe(
      catchError(error => {
        console.error('Error loading categories:', error);
        return of([]);
      })
    ).subscribe({
      next: (categories) => {
        this.categories = categories || [];
      }
    });
    this.subscriptions.add(sub);
  }

  loadReports(): void {
    // Prevent multiple simultaneous loads
    if (this.loading) {
      return;
    }

    this.loading = true;
    
    // Use forkJoin to run all API calls in parallel instead of nesting
    const reportsSub = forkJoin({
      summary: this.reportService.getAssetSummary().pipe(
        catchError(error => {
          console.error('Error loading asset summary:', error);
          return of({});
        })
      ),
      categoryStats: this.reportService.getCategoryStats().pipe(
        catchError(error => {
          console.error('Error loading category stats:', error);
          return of([]);
        })
      ),
      statusStats: this.reportService.getStatusStats().pipe(
        catchError(error => {
          console.error('Error loading status stats:', error);
          return of([]);
        })
      )
    }).subscribe({
      next: (data) => {
        // Generate reports from aggregated data
        this.reports = this.generateReportsFromAggregated(
          data.summary || {},
          data.categoryStats || [],
          data.statusStats || []
        );
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading reports:', error);
        this.loading = false;
        // Show error message to user
        this.snackBar.open('Failed to load reports. Please try again.', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
      }
    });
    
    this.subscriptions.add(reportsSub);
  }


  generateReportsFromAggregated(summary: any, categoryStats: any[], statusStats: any[]): any[] {
    // Convert category stats array to object
    const categoryData: { [key: string]: number } = {};
    if (Array.isArray(categoryStats)) {
      categoryStats.forEach((item: any) => {
        const key = item.category || item.Category || 'Uncategorized';
        const value = item.count || item.Count || 0;
        categoryData[key] = value;
      });
    }

    // Convert status stats array to object
    const statusData: { [key: string]: number } = {};
    if (Array.isArray(statusStats)) {
      statusStats.forEach((item: any) => {
        const key = item.status || item.Status || 'Unknown';
        const value = item.count || item.Count || 0;
        statusData[key] = value;
      });
    }

    // Pre-compute entries arrays to avoid calling getObjectEntries in template
    const categoryEntries = this.getObjectEntries(categoryData);
    const statusEntries = this.getObjectEntries(statusData);

    return [
      {
        title: 'Total Assets Report',
        type: 'summary',
        data: {
          total: summary.totalAssets || 0,
          inStock: summary.inStockAssets || 0,
          repair: summary.repairAssets || 0,
          sold: summary.soldAssets || 0,
          damaged: summary.damagedAssets || 0,
          ewaste: summary.ewasteAssets || 0,
          available: summary.inStockAssets || 0,
          allocated: summary.inStockAssets || 0,
          maintenance: summary.repairAssets || 0,
          disposed: summary.ewasteAssets || 0
        }
      },
      {
        title: 'Assets by Category',
        type: 'category',
        data: categoryData,
        entries: categoryEntries  // Pre-computed entries
      },
      {
        title: 'Assets by Status',
        type: 'status',
        data: statusData,
        entries: statusEntries  // Pre-computed entries
      }
    ];
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
    if (!obj || typeof obj !== 'object') {
      return [];
    }
    // Return empty array if obj is null/undefined to prevent errors
    try {
      return Object.keys(obj).map(key => ({ key, value: obj[key] }));
    } catch (error) {
      console.error('Error in getObjectEntries:', error);
      return [];
    }
  }
}


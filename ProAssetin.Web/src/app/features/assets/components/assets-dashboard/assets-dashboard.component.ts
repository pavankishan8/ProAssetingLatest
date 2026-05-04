import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { AssetService } from '../../../../core/services/asset.service';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { ReportService } from '../../../../core/services/report.service';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-assets-dashboard',
  templateUrl: './assets-dashboard.component.html',
  styleUrls: ['./assets-dashboard.component.scss']
})
export class AssetsDashboardComponent implements OnInit, AfterViewInit {
  @ViewChild('weeklyChart', { static: false }) weeklyChartRef!: ElementRef;
  @ViewChild('monthlyChart', { static: false }) monthlyChartRef!: ElementRef;

  stats: any = {
    totalAssets: 0,
    inStock: 0,
    repair: 0,
    damaged: 0,
    sold: 0,
    ewaste: 0,
    byCategory: [],
    byStatus: [],
    recentAssets: []
  };
  loading = true;
  
  weeklyChart: Chart | null = null;
  monthlyChart: Chart | null = null;

  constructor(
    private assetService: AssetService,
    private dashboardService: DashboardService,
    private reportService: ReportService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  ngAfterViewInit(): void {
    // Wait a bit for view to be fully initialized before creating charts
    setTimeout(() => {
      this.loadChartData();
    }, 500);
  }

  loadDashboardData(): void {
    this.loading = true;
    
    // Load status stats
    this.reportService.getStatusStats().subscribe({
      next: (statusStats: any[]) => {
        this.stats.byStatus = statusStats;
        
        // Map status stats to individual properties
        statusStats.forEach((stat: any) => {
          const status = stat.status || stat.Status || '';
          const count = stat.count || stat.Count || 0;
          
          switch (status.toLowerCase()) {
            case 'in-stock':
              this.stats.inStock = count;
              break;
            case 'repair':
              this.stats.repair = count;
              break;
            case 'damaged':
              this.stats.damaged = count;
              break;
            case 'sold':
              this.stats.sold = count;
              break;
            case 'e-waste':
              this.stats.ewaste = count;
              break;
          }
        });
        
        // Calculate total
        this.stats.totalAssets = statusStats.reduce((sum, stat) => sum + (stat.count || stat.Count || 0), 0);
      },
      error: (error) => {
        console.error('Error loading status stats:', error);
      }
    });
    
    // Load summary for additional data
    this.reportService.getAssetSummary().subscribe({
      next: (summary: any) => {
        this.stats.totalAssets = summary.totalAssets || 0;
        if (!this.stats.inStock && summary.inStockAssets) {
          this.stats.inStock = summary.inStockAssets;
        }
        if (!this.stats.repair && summary.repairAssets) {
          this.stats.repair = summary.repairAssets;
        }
        if (!this.stats.damaged && summary.damagedAssets) {
          this.stats.damaged = summary.damagedAssets;
        }
        if (!this.stats.sold && summary.soldAssets) {
          this.stats.sold = summary.soldAssets;
        }
        if (!this.stats.ewaste && summary.ewasteAssets) {
          this.stats.ewaste = summary.ewasteAssets;
        }
      },
      error: (error) => {
        console.error('Error loading asset summary:', error);
      }
    });
    
    // Load category stats
    this.reportService.getCategoryStats().subscribe({
      next: (categoryStats: any[]) => {
        this.stats.byCategory = categoryStats.map((item: any) => ({
          category: item.category || item.Category || 'Uncategorized',
          count: item.count || item.Count || 0
        }));
      },
      error: (error) => {
        console.error('Error loading category stats:', error);
      }
    });
    
    // Load recent assets
    this.loadRecentAssets();
  }

  loadChartData(): void {
    // Load weekly additions
    this.reportService.getWeeklyAdditions().subscribe({
      next: (data: any[]) => {
        this.createWeeklyChart(data);
      },
      error: (error) => {
        console.error('Error loading weekly additions:', error);
      }
    });

    // Load monthly additions
    this.reportService.getMonthlyAdditions().subscribe({
      next: (data: any[]) => {
        this.createMonthlyChart(data);
      },
      error: (error) => {
        console.error('Error loading monthly additions:', error);
      }
    });
  }

  createWeeklyChart(data: any[]): void {
    if (!this.weeklyChartRef?.nativeElement) return;

    // Destroy existing chart if any
    if (this.weeklyChart) {
      this.weeklyChart.destroy();
    }

    const labels = data.map(d => d.dayName || d.DayName || '');
    const counts = data.map(d => d.count || d.Count || 0);

    const config: ChartConfiguration = {
      type: 'bar' as ChartType,
      data: {
        labels: labels,
        datasets: [{
          label: 'Assets Added',
          data: counts,
          backgroundColor: 'rgba(63, 81, 181, 0.6)',
          borderColor: 'rgba(63, 81, 181, 1)',
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 1
            }
          }
        }
      }
    };

    this.weeklyChart = new Chart(this.weeklyChartRef.nativeElement, config);
  }

  createMonthlyChart(data: any[]): void {
    if (!this.monthlyChartRef?.nativeElement) return;

    // Destroy existing chart if any
    if (this.monthlyChart) {
      this.monthlyChart.destroy();
    }

    const labels = data.map(d => d.monthName || d.MonthName || '');
    const counts = data.map(d => d.count || d.Count || 0);

    const config: ChartConfiguration = {
      type: 'line' as ChartType,
      data: {
        labels: labels,
        datasets: [{
          label: 'Assets Added',
          data: counts,
          backgroundColor: 'rgba(63, 81, 181, 0.1)',
          borderColor: 'rgba(63, 81, 181, 1)',
          borderWidth: 2,
          fill: true,
          tension: 0.4
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 1
            }
          }
        }
      }
    };

    this.monthlyChart = new Chart(this.monthlyChartRef.nativeElement, config);
  }
  
  private loadRecentAssets(): void {
    this.assetService.getAssets({ pageNumber: 1, pageSize: 5 }).subscribe({
      next: (response) => {
        const assets = response.data || [];
        // Sort by created date (most recent first)
        this.stats.recentAssets = assets
          .sort((a: any, b: any) => {
            const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;
            const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;
            return dateB - dateA;
          });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading recent assets:', error);
        this.stats.recentAssets = [];
        this.loading = false;
      }
    });
  }
  
  private loadAssetsDirectly(): void {
    // Fallback method if dashboard API fails
    this.assetService.getAssets({ pageNumber: 1, pageSize: 1000 }).subscribe({
      next: (response) => {
        const assets = response.data || [];
        const totalCount = response.totalCount || assets.length;
        
        this.stats.totalAssets = totalCount;
        this.stats.available = assets.filter((a: any) => a.status?.toLowerCase() === 'available').length;
        this.stats.allocated = assets.filter((a: any) => a.status?.toLowerCase() === 'allocated').length;
        this.stats.maintenance = assets.filter((a: any) => a.status?.toLowerCase() === 'maintenance').length;
        
        // Group by category
        const categoryMap = new Map<string, number>();
        assets.forEach((asset: any) => {
          const cat = asset.category || 'Uncategorized';
          categoryMap.set(cat, (categoryMap.get(cat) || 0) + 1);
        });
        this.stats.byCategory = Array.from(categoryMap.entries()).map(([category, count]) => ({
          category,
          count
        }));
        
        // Recent assets
        this.stats.recentAssets = assets
          .sort((a: any, b: any) => {
            const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;
            const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;
            return dateB - dateA;
          })
          .slice(0, 5);
        
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading assets:', error);
        this.loading = false;
      }
    });
  }

  getCategoryColor(category: string): string {
    const colors = ['#3f51b5', '#f44336', '#4caf50', '#ff9800', '#9c27b0', '#00bcd4'];
    const index = category ? category.charCodeAt(0) % colors.length : 0;
    return colors[index];
  }

  /** Normalizes API status for Recent Assets badge styling */
  statusKey(status: string | undefined | null): string {
    return (status || 'unknown').toLowerCase().replace(/\s+/g, '-');
  }
}


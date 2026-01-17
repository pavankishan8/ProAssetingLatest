import { Component, OnInit } from '@angular/core';
import { AssetService } from '../../../../core/services/asset.service';
import { DashboardService } from '../../../../core/services/dashboard.service';

@Component({
  selector: 'app-assets-dashboard',
  templateUrl: './assets-dashboard.component.html',
  styleUrls: ['./assets-dashboard.component.scss']
})
export class AssetsDashboardComponent implements OnInit {
  stats: any = {
    totalAssets: 0,
    available: 0,
    allocated: 0,
    maintenance: 0,
    byCategory: [],
    recentAssets: []
  };
  loading = true;

  constructor(
    private assetService: AssetService,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    
    // Load dashboard stats from the API (same as main dashboard)
    this.dashboardService.getDashboardData().subscribe({
      next: (dashboardData) => {
        // Use dashboard API data for stats
        if (dashboardData.summary) {
          this.stats.totalAssets = dashboardData.summary.totalAssets || 0;
          this.stats.available = dashboardData.summary.availableAssets || 0;
          this.stats.allocated = dashboardData.summary.allocatedAssets || 0;
          this.stats.maintenance = dashboardData.summary.maintenanceAssets || 0;
        }
        
        // Use category breakdown from dashboard
        if (dashboardData.categoryBreakdown) {
          this.stats.byCategory = Object.entries(dashboardData.categoryBreakdown).map(([category, count]) => ({
            category,
            count
          }));
        } else if (dashboardData.categoryStats && Array.isArray(dashboardData.categoryStats)) {
          this.stats.byCategory = dashboardData.categoryStats.map((item: any) => ({
            category: item.category || item.Category || 'Uncategorized',
            count: item.count || item.Count || 0
          }));
        }
        
        // Fetch recent assets separately
        this.loadRecentAssets();
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        // Fallback: fetch assets directly if dashboard API fails
        this.loadAssetsDirectly();
      }
    });
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
}


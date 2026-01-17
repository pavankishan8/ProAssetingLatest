import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { CurrencyService } from '../../../../core/services/currency.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  dashboardData: any = null;
  loading = true;
  statusChartData: any[] = [];
  categoryChartData: any[] = [];
  recentActivities: any[] = [];
  maxStatusCount = 0;

  constructor(
    private dashboardService: DashboardService,
    private currencyService: CurrencyService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.dashboardService.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData = data;
        this.processChartData(data);
        this.processRecentActivities(data);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        this.loading = false;
      }
    });
  }

  private processChartData(data: any): void {
    // Process status data
    if (data.statusBreakdown) {
      this.statusChartData = Object.keys(data.statusBreakdown).map(status => ({
        status: status,
        count: data.statusBreakdown[status]
      }));
      this.maxStatusCount = Math.max(...this.statusChartData.map(item => item.count), 1);
    }

    // Process category data
    if (data.categoryBreakdown) {
      this.categoryChartData = Object.keys(data.categoryBreakdown).map(category => ({
        category: category || 'Uncategorized',
        count: data.categoryBreakdown[category]
      }));
    }
  }

  private processRecentActivities(data: any): void {
    // Mock recent activities if not provided by API
    if (data.recentActivities) {
      this.recentActivities = data.recentActivities;
    } else {
      // Generate sample activities based on data
      this.recentActivities = [
        {
          action: 'added',
          description: `Added ${data.summary?.totalAssets || 0} total assets`,
          timestamp: new Date()
        },
        {
          action: 'allocated',
          description: `${data.summary?.allocatedAssets || 0} assets allocated`,
          timestamp: new Date(Date.now() - 3600000)
        }
      ];
    }
  }

  formatCurrency(value: number): string {
    return this.currencyService.formatCurrency(value);
  }

  formatTime(date: string | Date): string {
    const d = new Date(date);
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - d.getTime()) / 1000);

    if (diffInSeconds < 60) return 'Just now';
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minutes ago`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hours ago`;
    return `${Math.floor(diffInSeconds / 86400)} days ago`;
  }

  getBarPercentage(count: number, max: number): number {
    if (max === 0) return 0;
    return (count / max) * 100;
  }

  getCategoryColor(category: string): string {
    const colors = [
      '#64b5f6', '#81c784', '#ffb74d', '#90caf9',
      '#a5d6a7', '#4dd0e1', '#ce93d8', '#f48fb1'
    ];
    const index = category ? category.charCodeAt(0) % colors.length : 0;
    return colors[index];
  }

  getActivityIcon(action: string): string {
    const icons: { [key: string]: string } = {
      'added': 'add_circle',
      'updated': 'edit',
      'allocated': 'assignment_ind',
      'deleted': 'delete',
      'audited': 'check_circle'
    };
    return icons[action] || 'info';
  }
}

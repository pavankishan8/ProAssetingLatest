import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../core/services/dashboard.service';
import { CurrencyService } from '../../../../core/services/currency.service';

export interface DashboardFeatureLink {
  label: string;
  route: string;
  icon: string;
}

export interface DashboardFeatureModule {
  title: string;
  description: string;
  icon: string;
  color: string;
  links: DashboardFeatureLink[];
}

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

  /** All app areas — same coverage as the sidebar, surfaced on the home dashboard */
  readonly featureModules: DashboardFeatureModule[] = [
    {
      title: 'Assets',
      description: 'Inventory, allocation, and reports',
      icon: 'inventory_2',
      color: '#1976d2',
      links: [
        { label: 'Dashboard', route: '/assets/dashboard', icon: 'dashboard' },
        { label: 'List', route: '/assets/list', icon: 'list' },
        { label: 'Add', route: '/assets/add', icon: 'add' },
        { label: 'Reports', route: '/assets/reports', icon: 'assessment' }
      ]
    },
    {
      title: 'Vendors',
      description: 'Supplier records and contacts',
      icon: 'store',
      color: '#00897b',
      links: [
        { label: 'List', route: '/vendors/list', icon: 'list' },
        { label: 'Add', route: '/vendors/add', icon: 'person_add' }
      ]
    },
    {
      title: 'Purchases',
      description: 'Purchase orders and approvals',
      icon: 'shopping_cart',
      color: '#f57c00',
      links: [
        { label: 'Orders', route: '/purchases/list', icon: 'list' },
        { label: 'Create PO', route: '/purchases/add', icon: 'add' }
      ]
    },
    {
      title: 'Software',
      description: 'Licenses and compliance',
      icon: 'apps',
      color: '#7b1fa2',
      links: [
        { label: 'List', route: '/softwares/list', icon: 'list' },
        { label: 'Add', route: '/softwares/add', icon: 'add' }
      ]
    },
    {
      title: 'Invoices',
      description: 'AP tracking and status',
      icon: 'receipt_long',
      color: '#0288d1',
      links: [
        { label: 'List', route: '/invoices/list', icon: 'list' },
        { label: 'Add', route: '/invoices/add', icon: 'add' }
      ]
    },
    {
      title: 'Budgets',
      description: 'Fiscal planning and spend',
      icon: 'account_balance_wallet',
      color: '#388e3c',
      links: [
        { label: 'List', route: '/budgets/list', icon: 'list' },
        { label: 'Add', route: '/budgets/add', icon: 'add' }
      ]
    },
    {
      title: 'E-Waste',
      description: 'Disposal and chain-of-custody',
      icon: 'delete_sweep',
      color: '#5d4037',
      links: [
        { label: 'List', route: '/ewaste/list', icon: 'list' },
        { label: 'Add', route: '/ewaste/add', icon: 'add' }
      ]
    },
    {
      title: 'Security',
      description: 'Incidents and governance',
      icon: 'shield',
      color: '#c62828',
      links: [
        { label: 'Incidents', route: '/security/list', icon: 'list' },
        { label: 'Report', route: '/security/add', icon: 'report' }
      ]
    },
    {
      title: 'Projects',
      description: 'Initiatives and delivery',
      icon: 'assignment',
      color: '#00695c',
      links: [
        { label: 'List', route: '/projects/list', icon: 'list' },
        { label: 'New', route: '/projects/add', icon: 'add' }
      ]
    },
    {
      title: 'Contracts',
      description: 'Agreements and renewals',
      icon: 'description',
      color: '#455a64',
      links: [
        { label: 'List', route: '/contracts/list', icon: 'list' },
        { label: 'Add', route: '/contracts/add', icon: 'add' }
      ]
    },
    {
      title: 'Ticketing',
      description: 'Help desk and IT service requests',
      icon: 'confirmation_number',
      color: '#6d28d9',
      links: [
        { label: 'Queue', route: '/tickets/list', icon: 'list' },
        { label: 'New ticket', route: '/tickets/add', icon: 'add' }
      ]
    },
    {
      title: 'Settings',
      description: 'Company profile and preferences',
      icon: 'settings',
      color: '#546e7a',
      links: [{ label: 'Open', route: '/settings', icon: 'tune' }]
    }
  ];

  constructor(
    private dashboardService: DashboardService,
    private currencyService: CurrencyService
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

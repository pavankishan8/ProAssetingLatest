import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  menuItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/dashboard' },
    { 
      icon: 'inventory_2', 
      label: 'Assets', 
      route: '/assets',
      expanded: false,
      children: [
        { icon: 'dashboard', label: 'Assets Dashboard', route: '/assets/dashboard' },
        { icon: 'list', label: 'Asset List', route: '/assets/list' },
        { icon: 'add', label: 'Add Assets', route: '/assets/add' },
        { icon: 'assessment', label: 'Asset Reports', route: '/assets/reports' }
      ]
    },
    { 
      icon: 'store', 
      label: 'Vendors', 
      route: '/vendors',
      expanded: false,
      children: [
        { icon: 'list', label: 'Vendor List', route: '/vendors/list' },
        { icon: 'add', label: 'Add Vendor', route: '/vendors/add' }
      ]
    },
    { 
      icon: 'shopping_cart', 
      label: 'Purchases', 
      route: '/purchases',
      expanded: false,
      children: [
        { icon: 'list', label: 'Purchase Orders', route: '/purchases/list' },
        { icon: 'add', label: 'Create PO', route: '/purchases/add' }
      ]
    },
    { 
      icon: 'apps', 
      label: 'Softwares', 
      route: '/softwares',
      expanded: false,
      children: [
        { icon: 'list', label: 'Software List', route: '/softwares/list' },
        { icon: 'add', label: 'Add Software', route: '/softwares/add' }
      ]
    },
    { 
      icon: 'receipt', 
      label: 'Invoices', 
      route: '/invoices',
      expanded: false,
      children: [
        { icon: 'list', label: 'Invoice List', route: '/invoices/list' },
        { icon: 'add', label: 'Add Invoice', route: '/invoices/add' }
      ]
    },
    {
      icon: 'account_balance_wallet',
      label: 'Budgets',
      route: '/budgets',
      expanded: false,
      children: [
        { icon: 'list', label: 'Budget list', route: '/budgets/list' },
        { icon: 'add', label: 'Add budget', route: '/budgets/add' }
      ]
    },
    {
      icon: 'delete_sweep',
      label: 'E-Waste',
      route: '/ewaste',
      expanded: false,
      children: [
        { icon: 'list', label: 'Disposal list', route: '/ewaste/list' },
        { icon: 'add', label: 'Add disposal', route: '/ewaste/add' }
      ]
    },
    {
      icon: 'shield',
      label: 'Security',
      route: '/security',
      expanded: false,
      children: [
        { icon: 'list', label: 'Incident list', route: '/security/list' },
        { icon: 'add', label: 'Report incident', route: '/security/add' }
      ]
    },
    { icon: 'settings', label: 'Settings', route: '/settings' }
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Check current route on initialization
    this.updateExpandedState(this.router.url);

    // Listen to route changes to keep menu expanded
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.updateExpandedState(event.url);
      });
  }

  updateExpandedState(currentUrl: string): void {
    this.menuItems.forEach(item => {
      if (item.children) {
        const underParent = item.route && currentUrl.startsWith(item.route + '/');
        const isChildActive =
          !!underParent ||
          item.children.some(child => currentUrl.startsWith(child.route));
        item.expanded = isChildActive;
      }
    });
  }

  handleParentClick(event: Event, item: any): void {
    if (item.children) {
      // Toggle expansion
      item.expanded = !item.expanded;
      
      // Navigate to first child route (usually the dashboard)
      if (item.children.length > 0 && item.children[0].route) {
        this.router.navigate([item.children[0].route]);
      }
    }
  }

  toggleExpand(item: any): void {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}


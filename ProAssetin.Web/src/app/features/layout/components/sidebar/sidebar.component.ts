import { Component } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
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
    { icon: 'settings', label: 'Settings', route: '/settings' }
  ];

  toggleExpand(item: any): void {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}


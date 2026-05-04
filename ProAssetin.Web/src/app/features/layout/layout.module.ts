import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { MainLayoutComponent } from './components/main-layout/main-layout.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';

const routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full' as const
      },
      {
        path: 'dashboard',
        loadChildren: () => import('../dashboard/dashboard.module').then(m => m.DashboardModule)
      },
      {
        path: 'assets',
        loadChildren: () => import('../assets/assets.module').then(m => m.AssetsModule)
      },
      {
        path: 'vendors',
        loadChildren: () => import('../vendors/vendors.module').then(m => m.VendorsModule)
      },
      {
        path: 'purchases',
        loadChildren: () => import('../purchases/purchases.module').then(m => m.PurchasesModule)
      },
      {
        path: 'softwares',
        loadChildren: () => import('../softwares/softwares.module').then(m => m.SoftwaresModule)
      },
      {
        path: 'invoices',
        loadChildren: () => import('../invoices/invoices.module').then(m => m.InvoicesModule)
      },
      {
        path: 'budgets',
        loadChildren: () => import('../budgets/budgets.module').then(m => m.BudgetsModule)
      },
      {
        path: 'ewaste',
        loadChildren: () => import('../ewaste/ewaste.module').then(m => m.EWasteModule)
      },
      {
        path: 'security',
        loadChildren: () => import('../security/security.module').then(m => m.SecurityModule)
      },
      {
        path: 'settings',
        loadChildren: () => import('../settings/settings.module').then(m => m.SettingsModule)
      }
    ]
  }
];

@NgModule({
  declarations: [
    MainLayoutComponent,
    SidebarComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class LayoutModule { }


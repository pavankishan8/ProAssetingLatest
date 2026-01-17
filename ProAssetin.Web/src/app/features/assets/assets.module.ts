import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { AssetListComponent } from './components/asset-list/asset-list.component';
import { AssetsDashboardComponent } from './components/assets-dashboard/assets-dashboard.component';
import { AddAssetComponent } from './components/add-asset/add-asset.component';
import { AssetReportsComponent } from './components/asset-reports/asset-reports.component';

const routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full' as const
  },
  {
    path: 'dashboard',
    component: AssetsDashboardComponent
  },
  {
    path: 'add',
    component: AddAssetComponent
  },
  {
    path: 'reports',
    component: AssetReportsComponent
  },
  {
    path: 'list',
    component: AssetListComponent
  }
];

@NgModule({
  declarations: [
    AssetListComponent,
    AssetsDashboardComponent,
    AddAssetComponent,
    AssetReportsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class AssetsModule { }


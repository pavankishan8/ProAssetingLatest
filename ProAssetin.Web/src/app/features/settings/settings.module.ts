import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { CompanySettingsComponent } from './components/company-settings/company-settings.component';

const routes = [
  {
    path: '',
    component: CompanySettingsComponent
  }
];

@NgModule({
  declarations: [
    CompanySettingsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class SettingsModule { }


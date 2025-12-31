import { NgModule, APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './Components/header/header.component';
import { FooterComponent } from './Components/footer/footer.component';
import { SideNavBarComponent } from './Components/side-nav-bar/side-nav-bar.component';
import { HomeComponent } from './Components/home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { NotificationsService } from 'angular2-notifications';
import { ToastrModule } from 'ngx-toastr';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { HomePageComponent } from './Components/home-page/home-page.component';
import { LoginComponent } from './Components/login/login.component';
import { CreatePageComponent } from './Components/create-page/create-page.component';
import { ViewPageComponent } from './Components/view-page/view-page.component';
import { DeletePageComponent } from './Components/delete-page/delete-page.component';
import { ReportsPageComponent } from './Components/reports-page/reports-page.component';
import { ScanInventoryComponent } from './Components/scan-inventory/scan-inventory.component';
import { CreateTicketComponent } from './Components/create-ticket/create-ticket.component';
import { ClosedTicketsComponent } from './Components/closed-tickets/closed-tickets.component';
import { PendingTicketsComponent } from './Components/pending-tickets/pending-tickets.component';
import { AssetMDashboardComponent } from './Components/asset-m-dashboard/asset-m-dashboard.component';
import { InvoiceMDashboardComponent } from './Components/invoice-m-dashboard/invoice-m-dashboard.component';
import { PurchaseMDashboardComponent } from './Components/purchase-m-dashboard/purchase-m-dashboard.component';
import { SamlMDashboardComponent } from './Components/saml-m-dashboard/saml-m-dashboard.component';
import { BudgetMDashboardComponent } from './Components/budget-m-dashboard/budget-m-dashboard.component';
import { VendorMDashboardComponent } from './Components/vendor-m-dashboard/vendor-m-dashboard.component';
import { EWasteMDashboardComponent } from './Components/e-waste-m-dashboard/e-waste-m-dashboard.component';
import { SecurityMDashboardComponent } from './Components/security-m-dashboard/security-m-dashboard.component';
import { VaptMDashboardComponent } from './Components/vapt-m-dashboard/vapt-m-dashboard.component';
import { ProjectMDashboardComponent } from './Components/project-m-dashboard/project-m-dashboard.component';
import { TicketToolDashboardComponent } from './Components/ticket-tool-dashboard/ticket-tool-dashboard.component';
import { AssetsPageComponent } from './Assets/assets-page/assets-page.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HttpClientModule } from '@angular/common/http';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';

import { NgxPaginationModule } from 'ngx-pagination';
import { SignaturePadModule } from 'angular2-signaturepad';
import { RouterModule } from '@angular/router';

import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';

import { AssetsCreateComponent } from './Assets/assets-create/assets-create.component';
import { AssetsSearchComponent } from './Assets/assets-search/assets-search.component';
import { AssetsReportsComponent } from './Assets/assets-reports/assets-reports.component';
import { UserSettingsComponent } from './Settings/user-settings/user-settings.component';
import { SettingsNavBarComponent } from './Settings/settings-nav-bar/settings-nav-bar.component';
import { CurrencyTimeZoneComponent } from './Settings/currency-time-zone/currency-time-zone.component';
import { UserSPageComponent } from './Settings/user-s-page/user-s-page.component';
import { AccountCreationComponent } from './Settings/account-creation/account-creation.component';
import { AccountPermissionsComponent } from './Settings/account-permissions/account-permissions.component';
import { AccountRolesComponent } from './Settings/account-roles/account-roles.component';
import { SMTPConfigComponent } from './Settings/smtp-config/smtp-config.component';
import { PreDefinedIDComponent } from './Settings/pre-defined-id/pre-defined-id.component';
import { PurCreateComponent } from './Purchase/pur-create/pur-create.component';
import { POComponent } from './Purchase/po/po.component';


function initializeApp() {
  return () => new Promise<void>((resolve) => {
    // Perform any initialization tasks here
    resolve();
  });
}

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    SideNavBarComponent,
    HomeComponent,
    HomePageComponent,
    LoginComponent,
    CreatePageComponent,
    ViewPageComponent,
    DeletePageComponent,
    ReportsPageComponent,
    ScanInventoryComponent,
    CreateTicketComponent,
    ClosedTicketsComponent,
    PendingTicketsComponent,
    AssetMDashboardComponent,
    InvoiceMDashboardComponent,
    PurchaseMDashboardComponent,
    SamlMDashboardComponent,
    BudgetMDashboardComponent,
    VendorMDashboardComponent,
    EWasteMDashboardComponent,
    SecurityMDashboardComponent,
    VaptMDashboardComponent,
    ProjectMDashboardComponent,
    TicketToolDashboardComponent,
    AssetsPageComponent,
    AssetsCreateComponent,
    AssetsSearchComponent,
    AssetsReportsComponent,
    UserSettingsComponent,
    SettingsNavBarComponent,
    CurrencyTimeZoneComponent,
    UserSPageComponent,
    AccountCreationComponent,
    AccountPermissionsComponent,
    AccountRolesComponent,
    SMTPConfigComponent,
    PreDefinedIDComponent,
    PurCreateComponent,
    POComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    RouterModule,
    BrowserAnimationsModule,
    MatSidenavModule,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    ToastrModule.forRoot(),
    SimpleNotificationsModule.forRoot(),
    FormsModule,
    MatDialogModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    MatTooltipModule,
    HttpClientModule,
    MatSnackBarModule,
    MatCheckboxModule,
    MatDatepickerModule, 
    MatNativeDateModule,
    MatStepperModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatPaginatorModule,
    MatTabsModule,
    MatChipsModule,
    
    NgxPaginationModule,
    SignaturePadModule,
    MatExpansionModule
    
  ],
  providers: [
    DatePipe,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

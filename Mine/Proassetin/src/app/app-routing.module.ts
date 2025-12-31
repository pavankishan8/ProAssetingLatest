import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { HomeComponent } from './Components/home/home.component';
import { FooterComponent } from './Components/footer/footer.component';
import { HomePageComponent } from './Components/home-page/home-page.component';
import { CreatePageComponent } from './Components/create-page/create-page.component';
import { ViewPageComponent } from './Components/view-page/view-page.component';
import { DeletePageComponent } from './Components/delete-page/delete-page.component';
import { ReportsPageComponent } from './Components/reports-page/reports-page.component';
import { AuthGuard } from './auth.guard';
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
import { AssetsCreateComponent } from './Assets/assets-create/assets-create.component';
import { AssetsSearchComponent } from './Assets/assets-search/assets-search.component';
import { AssetsReportsComponent } from './Assets/assets-reports/assets-reports.component';
import { UserSettingsComponent } from './Settings/user-settings/user-settings.component';
import { CurrencyTimeZoneComponent } from './Settings/currency-time-zone/currency-time-zone.component';
import { UserSPageComponent } from './Settings/user-s-page/user-s-page.component';
import { AccountCreationComponent } from './Settings/account-creation/account-creation.component';
import { AccountPermissionsComponent } from './Settings/account-permissions/account-permissions.component';
import { AccountRolesComponent } from './Settings/account-roles/account-roles.component';
import { SMTPConfigComponent } from './Settings/smtp-config/smtp-config.component';
import { PreDefinedIDComponent } from './Settings/pre-defined-id/pre-defined-id.component';
import { PurCreateComponent } from './Purchase/pur-create/pur-create.component';
import { POComponent } from './Purchase/po/po.component';

const routes: Routes = [
  { path: 'Login', component: LoginComponent },
  { path: 'Login/Home', redirectTo: 'Login', pathMatch: 'full' },
  { path: '', redirectTo: '/Login', pathMatch: 'full' },
  { path: 'Home', component: HomeComponent, canActivate: [AuthGuard],
    children: [
    { path: '', redirectTo: 'Home-Page', pathMatch: 'full' },
    { path: 'Home-Page', component: HomePageComponent },
    { path: 'Create', component: CreatePageComponent},
    { path: 'View', component: ViewPageComponent },
    { path: 'Delete', component: DeletePageComponent },
    { path: 'Reports', component: ReportsPageComponent },
    { path: 'ScanInv', component: ScanInventoryComponent },
    { path: 'CreateTick', component: CreateTicketComponent },
    { path: 'ClosedTick', component: ClosedTicketsComponent },
    { path: 'PendingTick', component: PendingTicketsComponent },
    { path: 'AssetMDash', component: AssetMDashboardComponent },
    { path: 'InvoiceMDash', component: InvoiceMDashboardComponent },
    { path: 'PurchaseMDash', component: PurchaseMDashboardComponent },
    { path: 'samlMDash', component: SamlMDashboardComponent },
    { path: 'BudgetMDash', component: BudgetMDashboardComponent },
    { path: 'VendorMDash', component: VendorMDashboardComponent },
    { path: 'EwasteMDash', component: EWasteMDashboardComponent },
    { path: 'SecurityMDash', component: SecurityMDashboardComponent },
    { path: 'vaptMDash', component: VaptMDashboardComponent },
    { path: 'ProjectMDash', component: ProjectMDashboardComponent },
    { path: 'TTMDash', component: TicketToolDashboardComponent },
    { path: 'AssetPage', component: AssetsPageComponent },
    { path: 'AssetCreate', component: AssetsCreateComponent },
    { path: 'AssetSearch', component: AssetsSearchComponent },
    { path: 'AssetReports', component: AssetsReportsComponent },
    { path: 'PurCreate', component: PurCreateComponent },
    { path: 'PurO', component: POComponent },
  ] },
  { path: 'UserSettings', component: UserSettingsComponent,  canActivate: [AuthGuard], children: [
    { path: '', redirectTo: 'UserS', pathMatch: 'full' },
    { path: 'CurTime', component: CurrencyTimeZoneComponent },
    { path: 'UserS', component: UserSPageComponent },
    { path: 'AccCreation', component: AccountCreationComponent },
    { path: 'AccPermissions', component: AccountPermissionsComponent },
    { path: 'AccRoles', component: AccountRolesComponent },
    { path: 'SMTPCon', component: SMTPConfigComponent, children:[
      { path: 'SMTPCon', component: SMTPConfigComponent },
    ] },
    { path: 'PreID', component: PreDefinedIDComponent, children:[
      { path: 'PreID', component: PreDefinedIDComponent },
    ] }
  ] },
  { path: 'Foot', component: FooterComponent },
  { path: '**', redirectTo: '/Login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

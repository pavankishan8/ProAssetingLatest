import { Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, ActivatedRouteSnapshot } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-settings-nav-bar',
  templateUrl: './settings-nav-bar.component.html',
  styleUrls: ['./settings-nav-bar.component.scss']
})
export class SettingsNavBarComponent {
  userAcc: boolean = false;
  invoiceMan: boolean = false;
  assetIDMan: boolean = false;
  samlMan: boolean = false;
  budgetMan: boolean = false;
  curTime: boolean = false;
  ewasMan: boolean = false;
  securityMan: boolean = false;
  vaptMan: boolean = false;
  projectMan: boolean = false;
  ticketMan: boolean = false;
  ifCrete: boolean = false;

  constructor(private route: ActivatedRoute, private router: Router) { 
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        const currentRoute = this.router.url;

        if (currentRoute === '/UserSettings/UserS') {
          this.userAcc = true;
          this.resetOtherStates('userAcc');
        } else if (currentRoute === '/UserSettings/AccCreation') {
          this.userAcc = true;
          this.resetOtherStates('userAcc');
        } else if (currentRoute === '/UserSettings/AccPermissions') {
          this.userAcc = true;
          this.resetOtherStates('userAcc');
        } else if (currentRoute === '/UserSettings/AccRoles') {
          this.userAcc = true;
          this.resetOtherStates('userAcc');
        } else if (currentRoute === '/Home/AssetReports') {
          this.userAcc = true;
          this.resetOtherStates('userAcc');
        } else if (currentRoute === '/Home/InvoiceMDash') {
          this.invoiceMan = true;
          this.resetOtherStates('invoiceMan');
        } else if (currentRoute === '/UserSettings/PreID') {
          this.assetIDMan = true;
          this.resetOtherStates('assetIDMan');
        } else if (currentRoute === '/Home/samlMDash') {
          this.samlMan = true;
          this.resetOtherStates('samlMan');
        } else if (currentRoute === '/Home/BudgetMDash') {
          this.budgetMan = true;
          this.resetOtherStates('budgetMan');
        } else if (currentRoute === '/Home/VendorMDash') {
          this.curTime = true;
          this.resetOtherStates('vendorMan');
        } else if (currentRoute === '/Home/EwasteMDash') {
          this.ewasMan = true;
          this.resetOtherStates('ewasMan');
        } else if (currentRoute === '/Home/SecurityMDash') {
          this.securityMan = true;
          this.resetOtherStates('securityMan');
        } else if (currentRoute === '/Home/vaptMDash') {
          this.vaptMan = true;
          this.resetOtherStates('vaptMan');
        } else if (currentRoute === '/Home/ProjectMDash') {
          this.projectMan = true;
          this.resetOtherStates('projectMan');
        } else if (currentRoute === '/Home/TTMDash') {
          this.ticketMan = true;
          this.resetOtherStates('ticketMan');
        } 

      }
    });
  }
  
  ngOnInit(): void {
    debugger
    const userData = sessionStorage.getItem('userData');

    if (userData) {
      // Parse user data (assuming it's JSON)
      const user = JSON.parse(userData);

      // Check user roles or other criteria
      if (user && user.Role === 'SuperAdmin') {
        this.ifCrete = true;
      }

      if (user && user.Role === 'Admin') {
        this.ifCrete = false;
      }
    }
  }

  goBack() {
    //window.history.back();
  }

  resetOtherStates(currentProperty: string) {
    if (currentProperty !== 'userAcc') {
      this.userAcc = false;
    }
    if (currentProperty !== 'invoiceMan') {
      this.invoiceMan = false;
    }
    if (currentProperty !== 'assetIDMan') {
      this.assetIDMan = false;
    }
    if (currentProperty !== 'samlMan') {
      this.samlMan = false;
    }
    if (currentProperty !== 'budgetMan') {
      this.budgetMan = false;
    }
    if (currentProperty !== 'vendorMan') {
      this.curTime = false;
    }
    if (currentProperty !== 'ewasMan') {
      this.ewasMan = false;
    }
    if (currentProperty !== 'securityMan') {
      this.securityMan = false;
    }
    if (currentProperty !== 'vaptMan') {
      this.vaptMan = false;
    }
    if (currentProperty !== 'projectMan') {
      this.projectMan = false;
    }
    if (currentProperty !== 'ticketMan') {
      this.ticketMan = false;
    }
    
  }

  userAccDrop(){
    this.userAcc = !this.userAcc;
  }

  invoiceDrop(){
    this.invoiceMan = !this.invoiceMan;
  }

  assetIDDrop(){
    this.assetIDMan = !this.assetIDMan;
  }

  samlDrop(){
    this.samlMan = !this.samlMan;
  }

  budgetDrop(){
    this.budgetMan = !this.budgetMan;
  }

  curTimeDrop(){
    this.curTime = !this.curTime;
  }

  ewasDrop(){
    this.ewasMan = !this.ewasMan;
  }

  securityDrop(){
    this.securityMan = !this.securityMan;
  }

  vaptDrop(){
    this.vaptMan = !this.vaptMan;
  }

  projectDrop(){
    this.projectMan = !this.projectMan;
  }

  ticketDrop(){
    this.ticketMan = !this.ticketMan;
  }
}

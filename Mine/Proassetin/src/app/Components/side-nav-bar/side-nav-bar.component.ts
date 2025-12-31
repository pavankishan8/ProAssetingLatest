import { Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, ActivatedRouteSnapshot } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-side-nav-bar',
  templateUrl: './side-nav-bar.component.html',
  styleUrls: ['./side-nav-bar.component.scss']
})
export class SideNavBarComponent {

  assetsMan: boolean = false;
  invoiceMan: boolean = false;
  purchaseMan: boolean = false;
  samlMan: boolean = false;
  budgetMan: boolean = false;
  vendorMan: boolean = false;
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

        if (currentRoute === '/Home/AssetPage') {
          this.assetsMan = true;
          this.resetOtherStates('assetsMan');
        } else if (currentRoute === '/Home/AssetMDash') {
          this.assetsMan = true;
          this.resetOtherStates('assetsMan');
        } else if (currentRoute === '/Home/AssetSearch') {
          this.assetsMan = true;
          this.resetOtherStates('assetsMan');
        } else if (currentRoute === '/Home/AssetCreate') {
          this.assetsMan = true;
          this.resetOtherStates('assetsMan');
        } else if (currentRoute === '/Home/AssetReports') {
          this.assetsMan = true;
          this.resetOtherStates('assetsMan');
        } else if (currentRoute === '/Home/InvoiceMDash') {
          this.invoiceMan = true;
          this.resetOtherStates('invoiceMan');
        } else if (currentRoute === '/Home/PurchaseMDash') {
          this.purchaseMan = true;
          this.resetOtherStates('purchaseMan');
        } else if (currentRoute === '/Home/PurCreate') {
          this.purchaseMan = true;
          this.resetOtherStates('purchaseMan');
        }  else if (currentRoute === '/Home/PurO') {
          this.purchaseMan = true;
          this.resetOtherStates('purchaseMan');
        } else if (currentRoute === '/Home/samlMDash') {
          this.samlMan = true;
          this.resetOtherStates('samlMan');
        } else if (currentRoute === '/Home/BudgetMDash') {
          this.budgetMan = true;
          this.resetOtherStates('budgetMan');
        } else if (currentRoute === '/Home/VendorMDash') {
          this.vendorMan = true;
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
        } else if (currentRoute === '/Home/CreateTick') {
          this.ticketMan = true;
          this.resetOtherStates('ticketMan');
        } else if (currentRoute === '/Home/ClosedTick') {
          this.ticketMan = true;
          this.resetOtherStates('ticketMan');
        } else if (currentRoute === '/Home/PendingTick') {
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
        this.ifCrete = true;
      }
    }
  }

  resetOtherStates(currentProperty: string) {
    if (currentProperty !== 'assetsMan') {
      this.assetsMan = false;
    }
    if (currentProperty !== 'invoiceMan') {
      this.invoiceMan = false;
    }
    if (currentProperty !== 'purchaseMan') {
      this.purchaseMan = false;
    }
    if (currentProperty !== 'samlMan') {
      this.samlMan = false;
    }
    if (currentProperty !== 'budgetMan') {
      this.budgetMan = false;
    }
    if (currentProperty !== 'vendorMan') {
      this.vendorMan = false;
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

  assetDrop(){
    this.assetsMan = !this.assetsMan;
  }

  invoiceDrop(){
    this.invoiceMan = !this.invoiceMan;
  }

  purchaseDrop(){
    this.purchaseMan = !this.purchaseMan;
  }

  samlDrop(){
    this.samlMan = !this.samlMan;
  }

  budgetDrop(){
    this.budgetMan = !this.budgetMan;
  }

  vendorDrop(){
    this.vendorMan = !this.vendorMan;
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

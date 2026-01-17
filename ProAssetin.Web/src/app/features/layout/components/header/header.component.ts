import { Component, EventEmitter, Output, OnInit, OnDestroy } from '@angular/core';
import { AuthService, User } from '../../../../core/services/auth.service';
import { SettingsService } from '../../../../core/services/settings.service';
import { Router, NavigationEnd } from '@angular/router';
import { Observable, map, Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  @Output() menuClick = new EventEmitter<void>();
  currentUser$!: Observable<User | null>;
  showAppName = false; // Set to true if you want to show "ProAssetin" next to company name
  companyLogoUrl: string | null = null;
  private routerSubscription?: Subscription;

  constructor(
    private authService: AuthService,
    private settingsService: SettingsService,
    private router: Router
  ) {
    this.currentUser$ = this.authService.currentUser$;
  }

  ngOnInit(): void {
    this.loadCompanyLogo();
    
    // Refresh logo when navigating to/from settings page
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.loadCompanyLogo();
      });
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  loadCompanyLogo(): void {
    this.settingsService.getCompanySettings().subscribe({
      next: (settings) => {
        // Logo is already in Base64 data URI format
        if (settings.companyLogo) {
          this.companyLogoUrl = settings.companyLogo;
        } else {
          this.companyLogoUrl = null;
        }
      },
      error: (error) => {
        // Silently fail - will show icon instead
        this.companyLogoUrl = null;
      }
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  getCompanyName(): string {
    const user = this.authService.getCurrentUser();
    if (user?.tenantId) {
      // Capitalize first letter and make rest lowercase
      return user.tenantId.charAt(0).toUpperCase() + user.tenantId.slice(1).toLowerCase();
    }
    return 'Company';
  }

  getUserRole(): string {
    const user = this.authService.getCurrentUser();
    // You can enhance this to get actual role from JWT token or API call
    // For now, return a default or check token claims
    return 'User';
  }
}

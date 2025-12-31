import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { SessionService } from './Services/session.service';
import { SharedDataService } from './Services/shared-data.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private sessionService: SessionService, private router: Router, private shared: SharedDataService) {}

  canActivate(): boolean {
    const isLoggedIn = sessionStorage.getItem('isLoggedIn');

    if (isLoggedIn === 'true') {
      this.resetSessionTimer();
      return true;
    } else {
      this.router.navigate(['/Login']);
      return false;
    }
  }

  private resetSessionTimer() {
    this.sessionService.resetSessionTimer();
  }
}
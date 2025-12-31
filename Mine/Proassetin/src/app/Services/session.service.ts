import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private sessionTimeoutMinutes: number = 20;
  private sessionTimeout: any
  public sessionExpired: EventEmitter<void> = new EventEmitter<void>();

  constructor() {
    this.resetSession();
    this.attachActivityListeners();
  }

  private resetSession(): void {
    this.sessionTimeout = setTimeout(() => {

      // Clear userData from sessionStorage
      sessionStorage.removeItem('userData');
      this.sessionExpired.emit();
    }, this.sessionTimeoutMinutes * 60 * 1000);
  }

  public resetSessionTimer(): void {
    clearTimeout(this.sessionTimeout);
    this.resetSession();
  }

  private attachActivityListeners(): void {
    ['mousemove', 'keydown', 'mousedown', 'touchstart', 'scroll'].forEach((event) => {
      window.addEventListener(event, () => this.resetSessionTimer());
    });
  }
}
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, catchError, of } from 'rxjs';
import { SettingsService, CompanySettings } from './settings.service';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private currencySettings$ = new BehaviorSubject<string>('USD');
  private currencySymbols: { [key: string]: string } = {
    'USD': '$',
    'INR': '₹',
    'EUR': '€',
    'GBP': '£',
    'JPY': '¥',
    'AUD': 'A$',
    'CAD': 'C$',
    'CHF': 'CHF',
    'CNY': '¥',
    'SGD': 'S$'
  };

  constructor(private settingsService: SettingsService) {
    this.loadCurrencySettings();
  }

  private loadCurrencySettings(): void {
    this.settingsService.getCompanySettings().pipe(
      catchError((error) => {
        console.error('Error loading currency settings:', error);
        return of({ currency: 'USD' } as CompanySettings);
      })
    ).subscribe({
      next: (settings: CompanySettings) => {
        const currency = settings.currency || 'USD';
        this.currencySettings$.next(currency);
      }
    });
  }

  getCurrency(): Observable<string> {
    return this.currencySettings$.asObservable();
  }

  getCurrentCurrency(): string {
    return this.currencySettings$.value;
  }

  getCurrencySymbol(currency?: string): string {
    const curr = currency || this.currencySettings$.value;
    return this.currencySymbols[curr.toUpperCase()] || curr;
  }

  formatCurrency(amount: number | null | undefined, currency?: string): string {
    if (amount === null || amount === undefined) {
      return '-';
    }

    const curr = currency || this.currencySettings$.value;
    const symbol = this.getCurrencySymbol(curr);

    // Special handling for INR and other currencies
    if (curr.toUpperCase() === 'INR') {
      return this.formatINR(amount, symbol);
    }

    // Default formatting for other currencies
    return this.formatDefaultCurrency(amount, symbol, curr);
  }

  private formatINR(amount: number, symbol: string): string {
    // Indian numbering system: Lakhs and Crores
    const formattedAmount = new Intl.NumberFormat('en-IN', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 2
    }).format(amount);

    return `${symbol} ${formattedAmount}`;
  }

  private formatDefaultCurrency(amount: number, symbol: string, currency: string): string {
    // For JPY and similar currencies, no decimal places
    const useDecimals = !['JPY', 'KRW', 'CLP'].includes(currency.toUpperCase());

    const formattedAmount = new Intl.NumberFormat('en-US', {
      style: 'decimal',
      minimumFractionDigits: useDecimals ? 2 : 0,
      maximumFractionDigits: useDecimals ? 2 : 0
    }).format(amount);

    // Position symbol based on currency
    if (currency.toUpperCase() === 'EUR' || currency.toUpperCase() === 'GBP') {
      return `${formattedAmount} ${symbol}`;
    }

    return `${symbol}${formattedAmount}`;
  }

  refreshCurrency(): void {
    this.loadCurrencySettings();
  }
}


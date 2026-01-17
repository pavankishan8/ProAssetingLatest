import { Component, OnInit } from '@angular/core';
import { SettingsService, CompanySettings, UpdateCompanySettings } from '../../../../core/services/settings.service';
import { CurrencyService } from '../../../../core/services/currency.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-company-settings',
  templateUrl: './company-settings.component.html',
  styleUrls: ['./company-settings.component.scss']
})
export class CompanySettingsComponent implements OnInit {
  settings: CompanySettings | null = null;
  loading = false;
  uploadingLogo = false;
  selectedFile: File | null = null;
  logoPreview: string | null = null;

  // Form data
  formData: UpdateCompanySettings = {};

  constructor(
    private settingsService: SettingsService,
    private currencyService: CurrencyService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.loading = true;
    this.settingsService.getCompanySettings().subscribe({
      next: (settings) => {
        this.settings = settings;
        this.formData = {
          companyName: settings.companyName,
          address: settings.address,
          phoneNumber: settings.phoneNumber,
          email: settings.email,
          industry: settings.industry,
          spocInformation: settings.spocInformation,
          gstNumber: settings.gstNumber,
          website: settings.website,
          currency: settings.currency,
          timeZone: settings.timeZone,
          dateFormat: settings.dateFormat,
          timeFormat: settings.timeFormat,
          defaultPageSize: settings.defaultPageSize,
          enableEmailNotifications: settings.enableEmailNotifications,
          enableSMSNotifications: settings.enableSMSNotifications
        };

        // Load logo preview if available (Base64 string)
        if (settings.companyLogo) {
          this.logoPreview = settings.companyLogo; // Already in data URI format (data:image/png;base64,...)
        } else {
          this.logoPreview = null;
        }

        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading settings:', error);
        this.snackBar.open('Failed to load settings', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
      }
    });
  }

  onFileSelected(event: any): void {
    const file = event.target?.files?.[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];
      if (!allowedTypes.includes(file.type)) {
        this.snackBar.open('Invalid file type. Please select an image file (jpg, png, gif, bmp)', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        return;
      }

      // Validate file size (5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.snackBar.open('File size exceeds 5MB limit', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        return;
      }

      this.selectedFile = file;

      // Preview image
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.logoPreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  uploadLogo(): void {
    if (!this.selectedFile) {
      this.snackBar.open('Please select a file', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });
      return;
    }

    this.uploadingLogo = true;
    this.settingsService.uploadCompanyLogo(this.selectedFile).subscribe({
      next: (response: any) => {
        this.snackBar.open('Logo uploaded successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.uploadingLogo = false;
        this.selectedFile = null;
        
        // Reload settings to get updated logo (Base64)
        this.loadSettings();
      },
      error: (error) => {
        console.error('Error uploading logo:', error);
        this.snackBar.open(
          error.error?.message || 'Failed to upload logo',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          }
        );
        this.uploadingLogo = false;
      }
    });
  }

  saveSettings(): void {
    if (!this.formData) {
      return;
    }

    this.loading = true;
    this.settingsService.updateCompanySettings(this.formData).subscribe({
      next: (settings) => {
        this.settings = settings;
        // Refresh currency service when currency is updated
        if (this.formData.currency) {
          this.currencyService.refreshCurrency();
        }
        this.snackBar.open('Settings saved successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error saving settings:', error);
        this.snackBar.open(
          error.error?.message || 'Failed to save settings',
          'Close',
          {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'top'
          }
        );
        this.loading = false;
      }
    });
  }

  getLogoUrl(): string | null {
    // Logo is already in Base64 data URI format
    if (this.logoPreview) {
      return this.logoPreview;
    }
    if (this.settings?.companyLogo) {
      return this.settings.companyLogo;
    }
    return null;
  }
}


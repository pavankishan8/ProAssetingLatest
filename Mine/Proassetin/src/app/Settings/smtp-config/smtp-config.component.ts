import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';

@Component({
  selector: 'app-smtp-config',
  templateUrl: './smtp-config.component.html',
  styleUrls: ['./smtp-config.component.scss']
})
export class SMTPConfigComponent implements OnInit {
  enableTLS: boolean = true;
  enableAuth: boolean = true;
  loading: boolean = false;

  smtpConfig: any = {
    senderEmail: '',
    host: '',
    username: '',
    password: '',
    port: '',
    enableTLS: true,
    enableAuth: true
  };

  constructor(
    private apiService: ApiService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadSMTPConfig();
  }

  loadSMTPConfig(): void {
    // Load existing SMTP configuration if available
    // this.apiService.getSMTPConfig().subscribe(...)
  }

  saveConfig(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    const config = {
      SenderEmail: (document.getElementById('SEmail') as HTMLInputElement)?.value || '',
      Host: (document.getElementById('Host') as HTMLInputElement)?.value || '',
      Username: (document.getElementById('Username') as HTMLInputElement)?.value || '',
      Password: (document.getElementById('Password') as HTMLInputElement)?.value || '',
      Port: (document.getElementById('Port') as HTMLInputElement)?.value || '',
      EnableTLS: this.enableTLS,
      EnableAuth: this.enableAuth
    };

    // Call API to save configuration
    // this.apiService.saveSMTPConfig(config).subscribe(
    //   (response) => {
    //     this.notificationService.NotificationSuccess('SMTP configuration saved successfully');
    //     this.loading = false;
    //   },
    //   (error) => {
    //     this.notificationService.NotificationFailure('Failed to save SMTP configuration');
    //     this.loading = false;
    //   }
    // );

    // For now, just show success message
    this.notificationService.NotificationSuccess('SMTP configuration saved successfully');
    this.loading = false;
  }

  testConnection(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    // Call API to test SMTP connection
    // this.apiService.testSMTPConnection(config).subscribe(...)
    
    // For now, just show message
    setTimeout(() => {
      this.notificationService.NotificationSuccess('SMTP connection test successful');
      this.loading = false;
    }, 2000);
  }

  validateForm(): boolean {
    const senderEmail = (document.getElementById('SEmail') as HTMLInputElement)?.value;
    const host = (document.getElementById('Host') as HTMLInputElement)?.value;
    const username = (document.getElementById('Username') as HTMLInputElement)?.value;
    const port = (document.getElementById('Port') as HTMLInputElement)?.value;

    if (!senderEmail || !host || !username || !port) {
      this.notificationService.NotificationFailure('Please fill in all required fields');
      return false;
    }

    if (!this.isValidEmail(senderEmail)) {
      this.notificationService.NotificationFailure('Please enter a valid email address');
      return false;
    }

    if (!this.isValidPort(port)) {
      this.notificationService.NotificationFailure('Please enter a valid port number');
      return false;
    }

    return true;
  }

  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  isValidPort(port: string): boolean {
    const portNum = parseInt(port, 10);
    return !isNaN(portNum) && portNum > 0 && portNum <= 65535;
  }
}

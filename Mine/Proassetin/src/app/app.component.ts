import { Component } from '@angular/core';
import { SessionService } from './Services/session.service';
import { Router } from '@angular/router';
import { NotificationService } from './Services/notification.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Proassetin';

  constructor(private router: Router,private sessionService: SessionService) {}

  ngOnInit(): void {
    this.sessionService.sessionExpired.subscribe(() => {
      console.log('Session expired. Redirecting to login page.');
      this.router.navigate(['/Login']);
    });
  }
  

}

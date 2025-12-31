import { Component } from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  userRole: string | null = null;
  firstName: string | null = null;
  lastName: string | null = null;
  fullName: string | null = null;

  constructor(private router: Router) {}

  ngOnInit(): void {
    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const userDataObject = JSON.parse(userData);
      this.userRole = userDataObject.Role;
      this.firstName = userDataObject.FirstName;
      this.lastName = userDataObject.LastName;
      this.fullName = this.firstName + ' ' + this.lastName;
    }
  }

  logout(): void {
    debugger
    sessionStorage.clear();

    this.router.navigate(['/Login']);
  }
}

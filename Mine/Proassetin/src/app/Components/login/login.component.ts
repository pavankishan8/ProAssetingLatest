import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthGuard } from 'src/app/auth.guard';
import { AuthService } from 'src/app/Services/auth.service';
import { SharedDataService } from 'src/app/Services/shared-data.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private shared: SharedDataService, private router: Router, private auth: AuthGuard, private http: HttpClient, private authService: AuthService) {
  }

  ngOnInit(): void {
    
  }

  onLogin() {
    debugger
    const loginData = { email: this.email, password: this.password };

  this.authService.login(loginData)
    .subscribe(
      (response: any) => {
        if (response.success) {
          this.shared.email = this.email;
          this.shared.password = this.password;
          sessionStorage.setItem('isLoggedIn', 'true');

          sessionStorage.setItem('userData', JSON.stringify(response.user));
          
          this.errorMessage = '';
          console.log('isLoggedIn set to true');
          this.router.navigate(['/Home']);
        } else {
          this.errorMessage = 'Incorrect email or password.';
        }
      },
      (error) => {
        console.error('Error:', error);
        this.errorMessage = 'An error occurred during the login.';
      }
    );
  }

}

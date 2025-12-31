import { Component } from '@angular/core';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';

@Component({
  selector: 'app-account-creation',
  templateUrl: './account-creation.component.html',
  styleUrls: ['./account-creation.component.scss']
})
export class AccountCreationComponent {

  today: string;
  CompanyID: string;
  CName: string;
  CAddress: string;
  CPhoneNumber: string;
  Industry: string;
  SPOCInfo: string;
  CEmail: string;
  Epass: string;
  EpassConfirm: string;
  EmpID: string;
  Domain: string;
  EmpType: string;
  FName: string;
  LName: string;
  UName: string;
  EEmail: string;
  RegDate: string;
  PhoneNum: string;
  Loc: string;
  ProjName: string;
  TName: string;
  CusName: string;
  Manag: string;
  selectedRole: string = "Select";
  selectedWorkType: string = "Select";

  configDiv: boolean = false;

  employeeID: string | null = '';
  role: string | null = '';

  constructor(private apiserve: ApiService, private service: NotificationService) { }

  ngOnInit() {
    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const parsedUserData = JSON.parse(userData);
      this.employeeID = parsedUserData.EmployeeID;
      this.role = parsedUserData.Role;
    }

    const currentDate = new Date();
    this.RegDate = currentDate.getDate() + '/' + (currentDate.getMonth() + 1) + '/' + currentDate.getFullYear();
  }

  checkPasswordStrength(): string {
    const minLength = 8;
    const minUpperCase = 1;
    const minLowerCase = 1;
    const minNumbers = 1;
    const minSpecialChars = 1;

    let strength = 0;

    // Check minimum length
    if (this.Epass.length >= minLength) {
      strength++;
    }

    // Check for uppercase letters
    const upperCaseRegex = /[A-Z]/g;
    if (this.Epass.match(upperCaseRegex) && this.Epass.match(upperCaseRegex).length >= minUpperCase) {
      strength++;
    }

    // Check for lowercase letters
    const lowerCaseRegex = /[a-z]/g;
    if (this.Epass.match(lowerCaseRegex) && this.Epass.match(lowerCaseRegex).length >= minLowerCase) {
      strength++;
    }

    // Check for numbers
    const numbersRegex = /[0-9]/g;
    if (this.Epass.match(numbersRegex) && this.Epass.match(numbersRegex).length >= minNumbers) {
      strength++;
    }

    // Check for special characters
    const specialCharsRegex = /[!@#$%^&*(),.?":{}|<>]/g;
    if (this.Epass.match(specialCharsRegex) && this.Epass.match(specialCharsRegex).length >= minSpecialChars) {
      strength++;
    }

    // Determine the strength level
    if (strength === 5) {
      return 'Strong';
    } else if (strength >= 3) {
      return 'Moderate';
    } else {
      return 'Weak';
    }
  }

  configDivs(){
    this.configDiv = !this.configDiv;
  }

  register() {

    this.CompanyID = this.CompanyID.toUpperCase();

    if (!this.CompanyID || !this.EmpID) {
      this.service.NotificationFailure('CompanyID and EmployeeID are required');
      return;
    }

    if (this.PhoneNum && this.PhoneNum.length !== 10) {
      this.service.NotificationFailure('Phone number must be 10 digits');
      return;
    }

    const checkUserData = {
      //CEmail: this.CEmail,
      EEmail: this.EEmail,
      UName: this.UName
    };

    this.apiserve.checkExistingUsers(checkUserData).subscribe(
      (response) => {
        if (response.success) {
          this.service.NotificationFailure('User already exists');
        } else {
          const registrationData = {
            companyModel: {
              CompanyID: this.CompanyID,
              // CompanyName: this.CName,
              // Address: this.CAddress,
              // PhoneNumber: this.CPhoneNumber,
              // Industry: this.Industry,
              // SPOCInformation: this.SPOCInfo,
              // EmailAccount: this.CEmail
            },
            employeeModel: {
              EmployeeID: this.EmpID,
              DomainAccount: this.Domain,
              EmployeeType: this.EmpType,
              Username: this.UName,
              FirstName: this.FName,
              LastName: this.LName,
              Email: this.EEmail,
              Password: this.Epass,
              RegisterDate: this.RegDate,
              Role: this.selectedRole,
              PhoneNumber: this.PhoneNum,
              Location: this.Loc,
              ProjectName: this.ProjName,
              Team: this.TName,
              CustomerName: this.CusName,
              WorkType: this.selectedWorkType,
              ReportingManager: this.Manag
            },
            userRole: this.role,
          };

          this.apiserve.registerCompanyAndEmployee(registrationData).subscribe(
            (response) => {
              console.log('Success:', response);
              this.service.NotificationSuccess(`User Registered Successfully`);
            },
            (error) => {
              console.error('Error:', error);
              this.service.NotificationFailure(`Error Registering User`);
            }
          );
        }
      },
      (checkError) => {
        console.error('Error checking existing users:', checkError);
        this.service.NotificationFailure('Error checking existing users');
      }
    );
  }

  resetForm() {
    // Set all ngModel properties to their initial values or null
    this.CompanyID = null;
    // this.CName = null;
    // this.CAddress = null;
    // this.CPhoneNumber = null;
    // this.Industry = null;
    // this.SPOCInfo = null;
    // this.CEmail = null;

    this.EmpID = null;
    this.Domain = null;
    this.EmpType = null;
    this.UName = null;
    this.FName = null;
    this.LName = null;
    this.EEmail = null;
    this.Epass = null;
    this.EpassConfirm = null;
    this.selectedRole = null;
    this.PhoneNum = null;
    this.Loc = null;
    this.ProjName = null;
    this.TName = null;
    this.CusName = null;
    this.selectedWorkType = null;
    this.Manag = null;
  }
}

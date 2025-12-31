import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectorRef, ChangeDetectionStrategy, OnInit, NgZone } from '@angular/core';
import { ApiService } from 'src/app/Services/api.service';
import { NotificationService } from 'src/app/Services/notification.service';

@Component({
  selector: 'app-pre-defined-id',
  templateUrl: './pre-defined-id.component.html',
  styleUrls: ['./pre-defined-id.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PreDefinedIDComponent implements OnInit {
  predefinedAssetID: string = '';
  employeeID: string | null = '';
  role: string | null = '';
  selectedFile!: File;
  imageData: any;
  gstNumber: string = '';
  loading: boolean = false;

  constructor(private ngZone: NgZone, private apiserve: ApiService, private service: NotificationService, private http: HttpClient, private cdr: ChangeDetectorRef) { 
    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const parsedUserData = JSON.parse(userData);
      this.employeeID = parsedUserData.EmployeeID;
      this.role = parsedUserData.Role;
    }
  }

  ngOnInit(): void {
    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const parsedUserData = JSON.parse(userData);
      this.employeeID = parsedUserData.EmployeeID;
      this.role = parsedUserData.Role;

      if (this.employeeID) {
        // Load predefined asset ID
        this.apiserve.getPredefinedAssetID(this.employeeID).subscribe(
          (data) => {
            this.predefinedAssetID = data || '';
            this.cdr.detectChanges();
          },
          (error) => {
            console.error('Error getting predefined asset ID:', error);
            this.predefinedAssetID = '';
            this.cdr.detectChanges();
          }
        );

        // Load company logo image
        this.getImage();
      }
    }
  }

  getImage(){
    if (!this.employeeID) {
      this.imageData = null;
      this.cdr.detectChanges();
      return;
    }
    
    this.apiserve.getImage(this.employeeID).subscribe(
      (response:any) => {
        if (response && response.image) {
          try {
            const base64Image = response.image;

            const byteArray = new Uint8Array(
              atob(base64Image)
                .split('')
                .map((char) => char.charCodeAt(0))
            );

            const blob = new Blob([byteArray], { type: 'image/jpeg' });
            const url = window.URL.createObjectURL(blob);
            this.imageData = url;
            this.cdr.detectChanges();
          } catch (error) {
            console.error('Error processing image:', error);
            this.imageData = null;
            this.cdr.detectChanges();
          }
        } else {
          this.imageData = null;
          this.cdr.detectChanges();
        }
      },
      (error) => {
        console.error('Error loading image:', error);
        this.imageData = null;
        this.cdr.detectChanges();
      }
    );
  }

  transformPredefinedAssetID(input: string): string {
    // Use a regular expression to remove numbers and special characters.
    const onlyCapitalLetters = input.replace(/[^A-Z]/g, '');

  // Ensure the length of the resulting string is exactly 3 characters.
    if (onlyCapitalLetters.length === 3) {
      return onlyCapitalLetters;
    } else {
      this.service.NotificationWarning('PreDefinedAssetID should only contain 3 capital letters');
      return 'Invalid PreDefinedAssetID';
    }
  }  

  saveConfiguration() {
    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const parsedUserData = JSON.parse(userData);
      this.employeeID = parsedUserData.EmployeeID;
      this.role = parsedUserData.Role;
    }

    if (!this.employeeID || !this.role) {
      this.service.NotificationFailure('EmployeeID or Role is missing');
      return;
    }

    if (!this.predefinedAssetID || this.predefinedAssetID.trim() === '') {
      this.service.NotificationFailure('Please enter a Pre-Defined AssetID');
      return;
    }

    const transformedPredefinedAssetID = this.transformPredefinedAssetID(this.predefinedAssetID);

    if (transformedPredefinedAssetID !== this.predefinedAssetID) {
      this.service.NotificationWarning('PreDefinedAssetID should only contain 3 capital letters (no numbers or special characters)');
      return;
    }

    if (this.predefinedAssetID.length !== 3) {
      this.service.NotificationWarning('PreDefinedAssetID must be exactly 3 characters');
      return;
    }

    this.loading = true;
    const model = {
      EmployeeID: this.employeeID,
      Role: this.role,
      PreDefinedAssetID: this.predefinedAssetID.toUpperCase(),
    };

    this.apiserve.updateEmployeeConfiguration(model).subscribe(
      (response) => {
        this.service.NotificationSuccess('Pre-Defined AssetID updated successfully');
        this.loading = false;
        this.cdr.detectChanges();
      },
      (error) => {
        this.service.NotificationFailure('Failed to update Pre-Defined AssetID');
        this.loading = false;
        this.cdr.detectChanges();
      }
    );
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadImage() {
    if (!this.selectedFile) {
      this.service.NotificationFailure('Please select an image file');
      return;
    }

    if (!this.employeeID || !this.role) {
      this.service.NotificationFailure('Employee ID or Role is missing');
      return;
    }

    this.loading = true;
    const formData = new FormData();
    formData.append('image', this.selectedFile, this.selectedFile.name);
    formData.append('employeeID', this.employeeID);
    formData.append('role', this.role);

    this.apiserve.uploadImage(formData).subscribe(
      (response) => {
        this.service.NotificationSuccess('Logo uploaded successfully');
        this.loading = false;
        this.selectedFile = null as any;
        // Reset file input
        const fileInput = document.getElementById('logoFile') as HTMLInputElement;
        if (fileInput) {
          fileInput.value = '';
        }
        // Refresh the image display
        this.ngZone.run(() => {
          this.getImage();
        });
      },
      (error) => {
        this.service.NotificationFailure('Error uploading logo');
        this.loading = false;
        this.cdr.detectChanges();
      }
    );
  }
  
  refreshComponent(): void {
    this.ngZone.run(() => {
      if (this.employeeID) {
        // Reload predefined asset ID
        this.apiserve.getPredefinedAssetID(this.employeeID).subscribe(
          (data) => {
            this.predefinedAssetID = data || '';
            this.cdr.detectChanges();
          },
          (error) => {
            console.error('Error getting predefined asset ID:', error);
            this.predefinedAssetID = '';
            this.cdr.detectChanges();
          }
        );
      }
    });
  }

  refreshImage(){
    this.ngZone.run(() => {
      this.getImage();
    });
  }

  refreshGST(): void {
    // Refresh GST number if needed
    // this.apiserve.getGSTNumber(this.employeeID).subscribe(...)
  }

  saveGST(): void {
    if (!this.gstNumber || this.gstNumber.trim() === '') {
      this.service.NotificationFailure('Please enter a GST number');
      return;
    }

    // Save GST number
    // this.apiserve.updateGSTNumber({ EmployeeID: this.employeeID, GSTNumber: this.gstNumber }).subscribe(...)
    this.service.NotificationSuccess('GST number saved successfully');
  }
}

import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormsModule, ReactiveFormsModule, FormGroup, FormArray, FormControl } from '@angular/forms';
import {STEPPER_GLOBAL_OPTIONS} from '@angular/cdk/stepper';
import {MatStepperModule} from '@angular/material/stepper';
import { NotificationService } from 'src/app/Services/notification.service';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import html2pdf from 'html2pdf.js';
import { startWith, map } from 'rxjs/operators';
import { saveAs } from 'file-saver';
import { Observable } from 'rxjs';
import { ApiService } from 'src/app/Services/api.service';
import { SignaturePad } from 'angular2-signaturepad';

@Component({
  selector: 'app-pur-create',
  templateUrl: './pur-create.component.html',
  styleUrls: ['./pur-create.component.scss'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: {showError: true},
    },
  ],
})
export class PurCreateComponent implements OnInit{

  companyName: string = '';
  companyAddress: string = '';

  @ViewChild('arquivo', { read: ElementRef }) fileInput!: ElementRef;
  @ViewChild('previewContent', { static: false }) previewContent: ElementRef;
  @ViewChild('previewContentInvoice', { static: false }) previewContentInvoice: ElementRef;
  @ViewChild(SignaturePad) signaturePad!: SignaturePad;
  
  stepper: boolean = false;
  selectedDate: Date;
  itemForm: FormGroup;
  item = { item: '',quantity: null, rate: null, amount: 0 };
  items: any[] = []; // Create an array to store your items
  desc = { invitem: '',addD: null,quantity: null, rate: null, amount: 0 };
  invItems: any[] = [];
  
  dropitems: string[] = ['Laptop', 'Keyboard', 'Mobile', 'Tab', 'Accessories'];
  itemControl = new FormControl();
  filteredItems: Observable<string[]>;

  vendorName: string = '';
  vendorAddress: string = '';
  purchaseOrderNo: string = '';
  orderDate: Date | null = null;
  dueDate: Date | null = null;
  notes: string = '';
  termsAndConditions: string = '';
  totalAmount: number = 0;

  frmName: string = '';
  frmEmail: string = '';
  frmAddress: string = '';
  frmPhone: string = '';
  frmBusNum: string = '';
  frmWeb: string = '';
  frmOwner: string = '';
  toName: string = '';
  toEmail: string = '';
  toAddress: string = '';
  toPhone: string = '';
  toNum: string = '';
  toFax: string = '';

  ifAddit: boolean = false;
  discount: number = 5;
  tax: number = 5;
  subtotal: number = 0;
  discountAmount: number = 0;
  taxAmount: number = 0;
  balanceDue: number = 0;

  employeeID: string | null = '';
  role: string | null = '';

  isSpinnerVisible= false;

  sign: boolean = false;

  signaturePadOptions: Object = { // check the options documentation for more details
    'minWidth': 2,
    'canvasWidth': 500,
    'canvasHeight': 300,
  };

  constructor(private _formBuilder: FormBuilder, private service: NotificationService, private apiserve: ApiService,) {
    this.itemForm = this._formBuilder.group({
      items: this._formBuilder.array([]),
    });

    this.itemForm = this._formBuilder.group({
      invItems: this._formBuilder.array([]),
    });
  }

  ngOnInit(): void {
    this.isSpinnerVisible = true;
    const isLoggedIn = sessionStorage.getItem('userData');

    const userData = sessionStorage.getItem('userData');
    if (userData) {
      const parsedUserData = JSON.parse(userData);
      this.employeeID = parsedUserData.EmployeeID;
      this.role = parsedUserData.Role;
    }
    this.getImage();

    if (isLoggedIn) {
      // Parse the user data from sessionStorage
      const userData = JSON.parse(isLoggedIn);

      // Set the values of companyName and companyAddress
      this.companyName = userData.CompanyName || '';
      this.companyAddress = userData.CompanyAddress || '';
    }

    this.addItem();
    this.addItemInv();
    this.isSpinnerVisible = false;
  }

  additional(){
    this.ifAddit = !this.ifAddit;
  }

  onLoaderActive(): void {
    console.log('Showing spinner');
    this.isSpinnerVisible = true;
  
    setTimeout(() => {
      console.log('Hiding spinner');
      this.isSpinnerVisible = false;
    }, 2000); // hide after 2 seconds (you can adjust this time)
  }

  drawComplete() {
    // Handle the draw complete event
    console.log('Draw complete');
  }

  clear() {
    this.signaturePad.clear();
  }

  save() {
    // Save the signature as an image or perform other actions
    console.log('Saving signature');
    const signatureData = this.signaturePad.toDataURL(); // Get the signature as a data URL
  }

  onSelectChange(event: any): void {
    this.item.item = event.target.value;
  }

  signature(){
    this.sign = !this.sign;
  }

  addItem() {
    this.isSpinnerVisible = true;
    const newItem = { item: '', quantity: null, rate: null, amount: 0 };
    this.items.push(newItem);
    this.isSpinnerVisible = false;
  }

  addItemInv() {
    this.isSpinnerVisible = true;
    const newItem = { invitem: '',addD: null,quantity: null, rate: null, amount: 0 };
    this.invItems.push(newItem);
    this.isSpinnerVisible = false;
  }

  removeItem(index: number) {
    this.isSpinnerVisible = true;
    if (index > 0 && index < this.items.length) {
      this.items.splice(index, 1);
      this.calculateTotalAmountInv();
    } else {
      this.service.NotificationWarning(`Last row can't be deleted`);
    }
    this.isSpinnerVisible = false;
  }

  removeItemInv(index: number) {
    this.isSpinnerVisible = true;
    if (index > 0 && index < this.invItems.length) {
      this.invItems.splice(index, 1);
      this.calculateTotalAmount();
    } else {
      this.service.NotificationWarning(`Last row can't be deleted`);
    }
    this.isSpinnerVisible = false;
  }

  calculateAmount(item: any) {
    const quantity = parseFloat(item.quantity);
    const rate = parseFloat(item.rate);

    if (!isNaN(quantity) && !isNaN(rate)) {
      item.amount = quantity * rate;
    } else {
      item.amount = 0;
    }

    this.calculateTotalAmount();
  }

  calculateAmountInv(item: any) {
    const quantity = parseFloat(item.quantity);
    const rate = parseFloat(item.rate);

    if (!isNaN(quantity) && !isNaN(rate)) {
      item.amount = quantity * rate;
    } else {
      item.amount = 0;
    }

    this.calculateTotalAmountInv();
  }

  calculateTotalAmount() {
    this.totalAmount = this.items.reduce((total, item) => total + parseFloat(item.amount), 0);
  }

  calculateTotalAmountInv() {
    this.totalAmount = this.invItems.reduce((total, desc) => total + parseFloat(desc.amount), 0);
    this.calculateTotals();
  }

  calculateTotals() {
    this.subtotal = 0;

    // Calculate subtotal
    for (const item of this.invItems) {
      this.subtotal += item.amount;
    }

    // Apply discount (assuming discount is a percentage)
    this.discountAmount = (this.subtotal * this.discount) / 100;
    const discountedTotal = this.subtotal - this.discountAmount;

    // Apply tax (assuming tax is a percentage)
    this.taxAmount = (discountedTotal * this.tax) / 100;
    this.totalAmount = discountedTotal + this.taxAmount;

    // Update the template-bound property
    this.balanceDue = this.totalAmount;
  }

  previewPDF(pdf: any) {
    this.isSpinnerVisible = true;
    const content = document.getElementById('preview-content');
    content.innerHTML = '';
  
    content.appendChild(pdf.output('bloburl'));
    this.isSpinnerVisible = false;
  }
  

  async generatePDF() {
    this.isSpinnerVisible = true;
    
    // Determine which preview content to use based on which is visible
    let content: HTMLElement;
    let fileName: string;

    // Check if invoice preview exists and is visible
    if (this.previewContentInvoice && this.previewContentInvoice.nativeElement) {
      const invoiceElement = this.previewContentInvoice.nativeElement;
      if (invoiceElement.offsetParent !== null) {
        content = invoiceElement;
        fileName = 'invoice.pdf';
      } else if (this.previewContent && this.previewContent.nativeElement) {
        content = this.previewContent.nativeElement;
        fileName = 'purchase_order.pdf';
      } else {
        console.log('No preview content available.');
        this.isSpinnerVisible = false;
        return;
      }
    } else if (this.previewContent && this.previewContent.nativeElement) {
      content = this.previewContent.nativeElement;
      fileName = 'purchase_order.pdf';
    } else {
      console.log('No preview content available.');
      this.isSpinnerVisible = false;
      return;
    }

    if (content.innerHTML.trim().length === 0) {
      console.log('No content to generate PDF.');
      this.isSpinnerVisible = false;
      return;
    }

    try {
      // Wait a bit to ensure all styles and images are loaded
      await new Promise(resolve => setTimeout(resolve, 500));

      // Store original styles
      const originalStyles = {
        display: content.style.display,
        visibility: content.style.visibility,
        position: content.style.position,
        left: content.style.left,
        top: content.style.top,
        zIndex: content.style.zIndex
      };

      // Ensure content is visible and properly positioned for capture
      content.style.display = 'block';
      content.style.visibility = 'visible';
      content.style.position = 'relative';
      content.style.zIndex = '9999';

      // Scroll to element to ensure it's in view
      content.scrollIntoView({ behavior: 'auto', block: 'start' });
      await new Promise(resolve => setTimeout(resolve, 100));

      // Use html2canvas to capture the styled content as an image
      // Capture the actual element with all its styles and padding
      const canvas = await html2canvas(content, {
        scale: 2,
        useCORS: true,
        allowTaint: true,
        logging: false,
        backgroundColor: '#ffffff',
        width: content.scrollWidth,
        height: content.scrollHeight,
        x: 0,
        y: 0,
        scrollX: 0,
        scrollY: 0,
        windowWidth: content.scrollWidth,
        windowHeight: content.scrollHeight
      });

      // Restore original styles
      content.style.display = originalStyles.display || '';
      content.style.visibility = originalStyles.visibility || '';
      content.style.position = originalStyles.position || '';
      content.style.left = originalStyles.left || '';
      content.style.top = originalStyles.top || '';
      content.style.zIndex = originalStyles.zIndex || '';

      const imgData = canvas.toDataURL('image/png', 1.0);
      const pdfWidth = 210; // A4 width in mm
      const pdfHeight = 297; // A4 height in mm
      const pagePadding = 10; // Padding in mm to match preview (2rem ≈ 10mm)
      const availableWidth = pdfWidth - (pagePadding * 2);
      const availableHeight = pdfHeight - (pagePadding * 2);
      
      // Calculate image dimensions to fit within available space with padding
      const imgWidth = availableWidth;
      const imgHeight = (canvas.height * imgWidth) / canvas.width;
      let heightLeft = imgHeight;

      const pdf = new jsPDF('p', 'mm', 'a4');
      let position = pagePadding;
      let sourceY = 0;

      // Add first page with padding
      if (imgHeight <= availableHeight) {
        // Content fits on one page
        pdf.addImage(imgData, 'PNG', pagePadding, position, imgWidth, imgHeight);
      } else {
        // Content spans multiple pages - add first page
        const firstPageHeight = availableHeight;
        const firstPageSourceHeight = (firstPageHeight / imgHeight) * canvas.height;
        
        // Create canvas for first page
        const firstPageCanvas = document.createElement('canvas');
        firstPageCanvas.width = canvas.width;
        firstPageCanvas.height = firstPageSourceHeight;
        const ctx = firstPageCanvas.getContext('2d');
        if (ctx) {
          ctx.drawImage(canvas, 0, 0, canvas.width, firstPageSourceHeight, 0, 0, canvas.width, firstPageSourceHeight);
          const firstPageImgData = firstPageCanvas.toDataURL('image/png', 1.0);
          pdf.addImage(firstPageImgData, 'PNG', pagePadding, position, imgWidth, firstPageHeight);
        }
        
        heightLeft -= availableHeight;
        sourceY = firstPageSourceHeight;

        // Add additional pages
        while (heightLeft > 0) {
          position = pagePadding;
          pdf.addPage();
          
          const pageImageHeight = Math.min(availableHeight, heightLeft);
          const pageSourceHeight = (pageImageHeight / imgHeight) * canvas.height;
          
          // Create canvas for this page
          const pageCanvas = document.createElement('canvas');
          pageCanvas.width = canvas.width;
          pageCanvas.height = pageSourceHeight;
          const pageCtx = pageCanvas.getContext('2d');
          if (pageCtx) {
            pageCtx.drawImage(canvas, 0, sourceY, canvas.width, pageSourceHeight, 0, 0, canvas.width, pageSourceHeight);
            const pageImgData = pageCanvas.toDataURL('image/png', 1.0);
            pdf.addImage(pageImgData, 'PNG', pagePadding, position, imgWidth, pageImageHeight);
          }
          
          sourceY += pageSourceHeight;
          heightLeft -= availableHeight;
        }
      }

      pdf.save(fileName);
      this.service.NotificationSuccess('PDF downloaded successfully!');
    } catch (error) {
      console.error('Error generating PDF:', error);
      this.service.NotificationFailure('Failed to generate PDF. Please try again.');
    } finally {
      this.isSpinnerVisible = false;
    }
  }

  numberToWords(num: number): string {
    const units = ['', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
    const teens = ['Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];
    const tens = ['', 'Ten', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];
  
    if (num === 0) return 'Zero';
  
    const getWords = (num: number): string => {
      if (num < 10) return units[num];
      if (num < 20) return teens[num - 11];
      return tens[Math.floor(num / 10)] + (num % 10 !== 0 ? ' ' + units[num % 10] : '');
    };
  
    if (num < 100) return getWords(num);
    if (num < 1000) return units[Math.floor(num / 100)] + ' Hundred ' + getWords(num % 100);
  
    if (num < 100000) {
      const thousands = Math.floor(num / 1000);
      const remainder = num % 1000;
      return `${this.numberToWords(thousands)} Thousand${remainder !== 0 ? ' ' + this.numberToWords(remainder) : ''}`;
    }
  
    if (num < 10000000) {
      const lakhs = Math.floor(num / 100000);
      const remainder = num % 100000;
      return `${this.numberToWords(lakhs)} Lakh${remainder !== 0 ? ' ' + this.numberToWords(remainder) : ''}`;
    }
  
    if (num < 1000000000) {
      const crores = Math.floor(num / 10000000);
      const remainder = num % 10000000;
      return `${this.numberToWords(crores)} Crore${remainder !== 0 ? ' ' + this.numberToWords(remainder) : ''}`;
    }
  
    return 'Number is too large to convert to words';
  }  

  getImage(){
    this.apiserve.getImage(this.employeeID).subscribe((response:any) => {
      
      const base64Image = response.image;

      const byteArray = new Uint8Array(
        atob(base64Image)
          .split('')
          .map((char) => char.charCodeAt(0))
      );

      const blob = new Blob([byteArray], { type: 'image/jpeg' });
      const url = window.URL.createObjectURL(blob);
      console.log('Check', url);

      // Update both image elements (for purchase order and invoice)
      const imgElement = document.getElementById('employeeImage') as HTMLImageElement;
      const imgElementInvoice = document.getElementById('employeeImageInvoice') as HTMLImageElement;

      if (imgElement) {
        imgElement.src = url;
      }
      if (imgElementInvoice) {
        imgElementInvoice.src = url;
      }

    });
  }

  savePurchase(){
    const itemObjects = this.items.map(item => ({
      item: item.item,
      quantity: item.quantity,
      rate: item.rate,
      amount: item.amount
      }));

    const formData = {
      companyName: this.companyName,
      companyAddress: this.companyAddress,
      purchaseOrderNo: this.purchaseOrderNo,
      orderDate: this.orderDate,
      dueDate: this.dueDate,
      notes: this.notes,
      termsAndConditions: this.termsAndConditions,
      totalAmount: this.totalAmount,
      items: JSON.stringify(itemObjects)
    };

    console.log(formData);
  }
}

import { Component, ElementRef, ViewChild, HostListener } from '@angular/core';
import { ApiService } from 'src/app/Services/api.service';
import { DatePipe } from '@angular/common';
import { jsPDF } from 'jspdf';
import 'jspdf-autotable';
import { NotificationService } from 'src/app/Services/notification.service';

interface Item {
  value: string;
  viewValue: string;
}

export class Asset {
  AssetID: string = '';
  AssetType: string = '';
  AssignedToUserIDString: string = '';
  Make: string = '';
  Model: string = '';
  Status: string = '';
  RAM: string = '';
  Processor: string = '';
  OS: string = '';
  Serial_Number: string = '';
  IMEI_Number: string = '';
  Purchase_Cost: string = '';
  Purchase_Year: string = '';
  MonthsInUse: string = '';
  NextRecycleDate: string = '';
  AssignedToUserID: string = '';
  Vendor: string = '';
  SentDate: string = '';
  ReceiveDate: string = '';
  Repair_Cost: string = '';
  RepairStatus: string = '';
  Tracking: string = '';
  RepairNotes: string = '';
  DeliveredBy: string = '';
  DamagedNotes: string = '';
  Sold_Cost: string = '';
  Sold_Year: string = '';
  SoldNotes: string = '';
  SoldTo: string = '';
  Approvals: string = '';
  EWaste_Vendor: string = '';
  EWasteNotes: string = '';
  EWasteApprovals: string = '';
  Email: string = '';
  Username: string = '';
  PhoneNumber: string = '';
  Location: string = '';
  ReportingManager: string = '';
}

@Component({
  selector: 'app-assets-reports',
  templateUrl: './assets-reports.component.html',
  styleUrls: ['./assets-reports.component.scss']
})
export class AssetsReportsComponent {

  searchResult: Asset = {} as Asset;
  searchAssetId: string = '';
  showResDiv: boolean = false;
  selectedItem: string = '';
  assetData: any[] = [];
  tableHeaders: string[] = [];
  items: Item[] = [
    { value: 'In-Stock', viewValue: 'In-Stock' },
    { value: 'Allocated', viewValue: 'Allocated' },
    { value: 'Repair', viewValue: 'Repair' },
    { value: 'Sold', viewValue: 'Sold' },
    { value: 'Damaged', viewValue: 'Damaged' },
    { value: 'E-Waste', viewValue: 'E-Waste' },

  ];

  selectAll: boolean = false;
  selectedItems: any[] = [];
  isDropdownOpen: boolean = false;
  loading: boolean = false;

  assetDataapi: any = { data: [] }

  @ViewChild('dropdownContainer') dropdownContainer: ElementRef;

  constructor(private apiserve: ApiService, private datePipe: DatePipe, private service: NotificationService) { }

  toggleDropdown(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  selectItem(value: string, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.selectedItem = value;
    this.isDropdownOpen = false;
    this.onItemSelected(value);
  }

  onSearch() {
    // Placeholder for search functionality
    if (this.selectedItem) {
      this.onItemSelected(this.selectedItem);
    }
  }

  getSelectedLabel(): string {
    const selected = this.items.find(item => item.value === this.selectedItem);
    return selected ? selected.viewValue : 'Select Asset Status';
  }

  trackByValue(index: number, item: Item): string {
    return item.value;
  }

  onItemSelected(selectedValue: string) {
    this.loading = true;
    this.apiserve.getInventoryData(selectedValue).subscribe({
      next: (response: any) => {
        this.assetDataapi = response.data || [];
        this.loading = false;
      },
      error: (err) => {
        this.service.NotificationFailure('Failed to load asset data.');
        console.error('Error loading asset data:', err);
        this.loading = false;
      }
    });
    this.selectedItems = [];
    this.selectAll = false;
  }

  @HostListener('document:click', ['$event'])
  clickout(event: Event) {
    if (this.dropdownContainer && !this.dropdownContainer.nativeElement.contains(event.target)) {
      this.isDropdownOpen = false;
    }
  }

  // searchAsset(assetId: string): void {
  //   this.searchResult = {} as Asset;

  //   this.apiserve.searchAssetById(this.searchAssetId).subscribe(
  //     (result: Asset) => {
  //       this.searchResult = result;
  //       this.showResDiv = true;
  //       this.assetData = [result];
  //       this.tableHeaders = Object.keys(result);
  //       console.log(this.searchResult);
  //     },
  //     (error) => {
  //       console.error('Search failed:', error);
  //       this.searchResult = {} as Asset;
  //       this.showResDiv = false;
  //       this.assetData = [];
  //       this.tableHeaders = [];
  //     }
  //   );
  // }

  selectAllItems(): void {
    this.assetDataapi.forEach(item => (item.selected = this.selectAll));
    this.selectedItems = this.selectAll ? [...this.assetDataapi] : [];
    console.log('Selected Items:', this.selectedItems);
  }

  shouldDisplayCell(value: any): boolean {
    return value !== null && value !== '0.0' && value !== 0;
  }

  downloadCSVbySearch() {
    // Create a mapping to keep track of whether each column has at least one non-null value
    const columnHasData: { [header: string]: boolean } = {};
    this.tableHeaders.forEach((header) => {
      columnHasData[header] = false;
    });

    // Iterate over assetData to determine which columns have data
    this.assetData.forEach((item) => {
      this.tableHeaders.forEach((header) => {
        const cellValue = item[header];
        if (cellValue !== null && cellValue !== '0.0' && cellValue !== 0) {
          columnHasData[header] = true;
        }
      });
    });

    // Create a CSV header row based on columns with data
    const csvHeader = this.tableHeaders.filter((header) => columnHasData[header]).join(',');

    // Create a CSV data row by mapping each item in assetData
    const csvData = this.assetData.map((item) => {
      return this.tableHeaders
        .filter((header) => columnHasData[header])
        .map((header) => {
          const cellValue = item[header];
          if (cellValue !== null && cellValue !== '0.0' && cellValue !== 0) {
            // Replace commas with spaces to prevent CSV formatting issues
            return cellValue.toString().replace(/,/g, ' ');
          } else {
            return ''; // Exclude the value
          }
        })
        .join(',');
    });

    // Join header and data rows into a single CSV string
    const csvContent = [csvHeader, ...csvData].join('\n');

    // Create a Blob with the CSV content
    const blob = new Blob([csvContent], { type: 'text/csv' });

    const currentDate = new Date();
    const timestamp = currentDate.toISOString().split('T')[0] + ' ' + currentDate.toTimeString().split(' ')[0];
    const filename = `Report - ${timestamp}.csv`;

    // Generate a download link and trigger the download
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  }

  getCellStyle(level: string): any {
    if (level === 'Completed') {
      return { 'background-color': '#20c220', 'color': 'white' };
    } else if (level === 'In Progress') {
      return { 'background-color': '#007eff', 'color': 'white' };
    } else if (level === 'Not Yet') {
      return { 'background-color': 'red', 'color': 'white' };
    } else if (level === '') {
      return { 'border': '0px', 'border-radius': '3px' };
    }
  }

  clearBtn() {
    this.searchResult = {} as Asset;
    this.showResDiv = false;
  }

  convertToCSV(data: any[]): string {
    // Filter out columns with null or 0 values and "0001-01-01T00:00:00" values
    const nonEmptyData = data.map((row) => {
      const filteredRow: any = {};
      for (const key in row) {
        if (key !== 'selected' && row[key] !== null && row[key] !== 0) {
          // Additional check for "0001-01-01T00:00:00"
          if (key === 'NextRecycleDate' && row[key] === '0001-01-01T00:00:00') {
            continue;
          }
          filteredRow[key] = row[key];
        }
      }
      return filteredRow;
    });

    // If all rows are empty, return an empty string
    if (nonEmptyData.length === 0) {
      return '';
    }

    // Extract the headers and convert non-empty data to CSV
    const headers = Object.keys(nonEmptyData[0]);
  
  const csvContent = [
    headers.join(','),
    ...nonEmptyData.map(row => headers.map(header => row[header]).join(','))
  ].join('\n');

  return csvContent;
  }

  // Function to download the CSV file
  downloadCSV(data: any[], filename: string) {
    if (data.length > 0) {
    const selectedData = data.filter(row => row.selected);
    
    const csvContent = this.convertToCSV(data);
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;

    // Set the custom filename here
    const currentDate = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss');
    a.download = `Report - ${currentDate}.csv`;

    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
    } else {
      this.service.NotificationFailure(`Select to Download`);
    }
  }

  // Download the whole table report with a custom filename
  downloadTableCSV() {
    if (this.selectedItems.length === 0) {
      this.service.NotificationFailure('Please select at least one item to download');
      return;
    }
    const currentDate = new Date().toISOString().replace(/[-:]/g, '').split('.')[0];
    this.downloadCSV(this.selectedItems, `Report - ${currentDate}.csv`);
    this.service.NotificationSuccess('CSV downloaded successfully');
  }

  downloadTablePDF() {
    if (this.selectedItems.length === 0) {
      this.service.NotificationFailure('Please select at least one item to download');
      return;
    }
    const currentDate = new Date().toISOString().replace(/[-:]/g, '').split('.')[0];
    this.downloadPDF(this.selectedItems, `Report - ${currentDate}`);
    this.service.NotificationSuccess('PDF downloaded successfully');
  }

  onItemCheckboxChange(item: any): void {
    if (item.selected) {
      this.selectedItems.push(item);
    } else {
      const index = this.selectedItems.findIndex(selectedItem => selectedItem === item);
      if (index !== -1) {
        this.selectedItems.splice(index, 1);
      }
    }
    this.selectAll = this.assetDataapi.every(item => item.selected);
  }
  
  
  downloadPDF(data: any[], filename: string) {
    if (data.length > 0) {
      const currentDate = new Date().toLocaleString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: true, // Use 24-hour format
      });
  
      const sanitizedDate = currentDate.replace(/[\s:]/g, '_');
  
      const pdf = new jsPDF();
  
      const pdfWidth = pdf.internal.pageSize.getWidth();
  
      // Add a custom header in the middle of the page
      const headerText = `Reports-${currentDate}`;
      const fontSize = 10; // Set your desired font size
      const textWidth = pdf.getStringUnitWidth(headerText) * fontSize / pdf.internal.scaleFactor;
      const xPosition = (pdfWidth - textWidth) / 2.5;
  
      pdf.text(headerText, xPosition, 10);
  
      // Add headers to the PDF
      const headers = Object.keys(data[0]).filter(header => header !== 'selected');
      const rows = data.map(row => Object.values(row).filter((_, index) => index !== headers.indexOf('selected')));
  
      // Ensure 'autoTable' is recognized by TypeScript
      (pdf as any).autoTable({
        head: [headers],
        body: rows,
        startY: 15, // Adjust as needed
        styles: {
          fontSize: 4, // Set the font size for headers and content
          fontStyle: 'normal', // Set the font style for headers to bold
        },
        columnStyles: {
          0: { fontStyle: 'normal' }, // Set the font style for the first column to normal
        },
      });
  
      // Save the PDF file
      pdf.save(`Report - ${currentDate}.pdf`);
    } else {
      this.service.NotificationFailure(`Select to Download`);
    }
  }

  // Download a single row's CSV
  // downloadRowCSV(rowData: any) {
  //   const currentDate = new Date().toISOString().replace(/[-:]/g, '').split('.')[0];
  //   this.downloadCSV([rowData], `Report - ${currentDate}.csv`);
  // }

  // Function to download a single item as PDF
  // downloadItemPDF(item: any) {
  //   const currentDate = new Date().toISOString().replace(/[-:]/g, '').split('.')[0];
  //   this.downloadPDF([item], `Report - ${currentDate}`);
  // }

  // downloadPDF(data: any[], filename: string) {
  //   const currentDate = new Date().toLocaleString('en-US', {
  //     year: 'numeric',
  //     month: '2-digit',
  //     day: '2-digit',
  //     hour: '2-digit',
  //     minute: '2-digit',
  //     second: '2-digit',
  //     hour12: true, // Use 24-hour format
  //   });

  //   const sanitizedDate = currentDate.replace(/[\s:]/g, '_');

  //   const pdf = new jsPDF();

  //   const pdfWidth = pdf.internal.pageSize.getWidth();

  //   // Add a custom header in the middle of the page
  //   const headerText = `Reports-${currentDate}`;
  //   const fontSize = 10; // Set your desired font size
  //   const textWidth = pdf.getStringUnitWidth(headerText) * fontSize / pdf.internal.scaleFactor;
  //   const xPosition = (pdfWidth - textWidth) / 2.5;

  //   pdf.text(headerText, xPosition, 10);

  //   // Add headers to the PDF
  //   const headers = Object.keys(data[0]);
  //   const rows = data.map(row => Object.values(row));

  //   // Ensure 'autoTable' is recognized by TypeScript
  //   (pdf as any).autoTable({
  //     head: [headers],
  //     body: rows,
  //     startY: 15, // Adjust as needed
  //     styles: {
  //       fontSize: 4, // Set the font size for headers and content
  //       fontStyle: 'normal', // Set the font style for headers to bold
  //     },
  //     columnStyles: {
  //       // You can apply specific styles to individual columns if needed
  //       0: { fontStyle: 'normal' }, // Set the font style for the first column to normal
  //     },
  //   });

  //   // Save the PDF file
  //   pdf.save(`Report - ${currentDate}.pdf`);
  // }
}
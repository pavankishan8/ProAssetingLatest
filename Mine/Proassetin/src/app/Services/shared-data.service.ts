import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {

  email: string = '';
  password: string = '';
  cardClicked = new Subject<void>();
  
  private selectedValueSource = new BehaviorSubject<string | null>(null);
  selectedValue$ = this.selectedValueSource.asObservable();

  constructor() { }

  updateSelectedValue(value: string | null) {
    this.selectedValueSource.next(value);
  }

  clickCard() {
    this.cardClicked.next();
  }
}

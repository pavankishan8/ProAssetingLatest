import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { EWasteListComponent } from './components/ewaste-list/ewaste-list.component';
import { AddEwasteComponent } from './components/add-ewaste/add-ewaste.component';

const routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' as const },
  { path: 'list', component: EWasteListComponent },
  { path: 'add', component: AddEwasteComponent },
  { path: 'edit/:id', component: AddEwasteComponent }
];

@NgModule({
  declarations: [EWasteListComponent, AddEwasteComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class EWasteModule {}

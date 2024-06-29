import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { OrganizationRoutingModule } from './organization-routing.module';
import { MaterialModule } from '../material/material.module';

//Components
import { OrganizationListingComponent } from './organization-listing/organization-listing.component';
import { CreateDialogComponent } from './create-dialog/create-dialog.component';
import { EditDialogComponent } from './edit-dialog/edit-dialog.component';
import { DeleteDialogComponent } from './delete-dialog/delete-dialog.component';

@NgModule({
  declarations: [OrganizationListingComponent, CreateDialogComponent, EditDialogComponent, DeleteDialogComponent],
  imports: [CommonModule, OrganizationRoutingModule, MaterialModule,FormsModule,ReactiveFormsModule]
})
export class OrganizationModule {}

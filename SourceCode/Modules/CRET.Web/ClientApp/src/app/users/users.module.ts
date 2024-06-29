import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UserListingComponent } from './user-listing/user-listing.component';
import { MaterialModule } from '../material/material.module';


@NgModule({
  declarations: [
    UserListingComponent
  ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    MaterialModule
  ]
})
export class UsersModule { }

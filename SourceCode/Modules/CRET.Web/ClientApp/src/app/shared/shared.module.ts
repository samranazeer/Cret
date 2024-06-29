import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { UnAuthorizedAccessComponent } from './components/un-authorized-access/un-authorized-access.component';


import { RouterModule } from '@angular/router';


@NgModule({
  declarations: [
    NotFoundComponent,
    UnAuthorizedAccessComponent,
  ],
  imports: [
    CommonModule,
    RouterModule
  ]
})
export class SharedModule { }

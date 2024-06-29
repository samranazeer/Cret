import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { OrganizationListingComponent } from './organization-listing/organization-listing.component';


const routes: Routes = [
  {
    path: 'organization',
    canActivate: [MsalGuard],
    children:[
      {path:'organizationList',component:OrganizationListingComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrganizationRoutingModule { }

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

//Components
import { CertificateListingComponent } from './certificate-listing/certificate-listing.component';


const routes: Routes = [
  {
    path:'certificate',
    children:[
      {path:'certificateList',component:CertificateListingComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CertificateRoutingModule { }

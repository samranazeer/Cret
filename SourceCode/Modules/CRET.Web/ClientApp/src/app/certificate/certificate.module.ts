import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { QRCodeModule } from 'angularx-qrcode';



import { CertificateRoutingModule } from './certificate-routing.module';
import { MaterialModule } from '../material/material.module';

//Components
import { CertificateListingComponent } from './certificate-listing/certificate-listing.component';
import { CreateDialogComponent } from './create-dialog/create-dialog.component';
import { EditDialogComponent } from './edit-dialog/edit-dialog.component';
import { DeleteDialogComponent } from './delete-dialog/delete-dialog.component';
import { ImportCsrDialogComponent } from './import-csr-dialog/import-csr-dialog.component';
import { ImportCertificateDialogComponent } from './import-certificate-dialog/import-certificate-dialog.component';
import { BatchSendDialogComponent } from './batch-send-dialog/batch-send-dialog.component';
import { BatchAlertDialogComponent } from './batch-alert-dialog/batch-alert-dialog.component';
// import { CsrImportDialogComponent } from './csr-import-dialog/csr-import-dialog.component';
// import { ProgressComponent } from './progress/progress.component';

@NgModule({
  declarations: [CertificateListingComponent, CreateDialogComponent, EditDialogComponent, DeleteDialogComponent, ImportCsrDialogComponent, ImportCertificateDialogComponent, BatchSendDialogComponent, BatchAlertDialogComponent],
  imports: [CommonModule,QRCodeModule, CertificateRoutingModule, MaterialModule,FormsModule,ReactiveFormsModule]
})
export class CertificateModule {}

import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CertificateStatus } from 'src/app/shared/models/certificate-status';

@Component({
  selector: 'app-batch-send-dialog',
  templateUrl: './batch-send-dialog.component.html',
  styleUrls: ['./batch-send-dialog.component.css']
})
export class BatchSendDialogComponent implements OnInit {
  public StatusEnum = CertificateStatus;

  count;
  certificateStatus;
  organizationEmail;
  newOrganizationEmail;
  isChangeEmailVisible: boolean = false;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  public dialogRef: MatDialogRef<BatchSendDialogComponent>
  ) {
    this.count = this.data.count;
    this.organizationEmail = this.data.email;
    this.newOrganizationEmail = this.data.email;
    this.certificateStatus = this.data.certificateStatus;

   }

  ngOnInit(): void {
  }

  showChangeEmail()
  {
    this.isChangeEmailVisible = true;
  }

  sendBatchRequest()
  {
    if(this.newOrganizationEmail != "")
    {
      this.dialogRef.close({email : this.newOrganizationEmail});
    }
  }



}

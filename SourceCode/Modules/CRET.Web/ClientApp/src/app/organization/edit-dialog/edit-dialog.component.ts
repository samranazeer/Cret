import { Component, Inject, OnInit } from '@angular/core';

import { OrganizationModel } from '../../shared/models/organization-model';
import { OrganizationService } from 'src/app/shared/services/organization.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-dialog',
  templateUrl: './edit-dialog.component.html',
  styleUrls: ['./edit-dialog.component.css'],
})
export class EditDialogComponent implements OnInit {
  model ;
  constructor(
    private organizationService: OrganizationService,
    public dialogRef: MatDialogRef<EditDialogComponent>,private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.model = new OrganizationModel();
    this.model.id = data.organizationData.id;
    this.model.customerNumber = data.organizationData.customerNumber;
    this.model.customerName = data.organizationData.customerName;
    this.model.organizationName = data.organizationData.organizationName;
    this.model.email = data.organizationData.email;
    this.model.contact = data.organizationData.contact;
    this.model.createdAt = data.organizationData.createdAt;
    this.model.createdBy = data.organizationData.createdBy;
  }


  ngOnInit(): void {}

  editOrganization() {
    console.log(this.model);

    this.organizationService.editOrganization(this.model).subscribe({
      next: (data) => {
        this.dialogRef.close(true);
      },
      error: (e: HttpErrorResponse) => {
        this.toastr.error('Something went wrong', 'Error', {
          positionClass: 'toast-bottom-right',
        });
        this.dialogRef.close(false);
      },
    });
  }
}

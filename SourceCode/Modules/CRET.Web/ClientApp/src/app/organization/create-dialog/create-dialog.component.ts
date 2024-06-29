import { Component, OnInit } from '@angular/core';

import { OrganizationModel } from '../../shared/models/organization-model';
import { OrganizationService } from 'src/app/shared/services/organization.service';
import { MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-create-dialog',
  templateUrl: './create-dialog.component.html',
  styleUrls: ['./create-dialog.component.css'],
})
export class CreateDialogComponent implements OnInit {
  constructor(
    private organizationService: OrganizationService,
    public dialogRef: MatDialogRef<CreateDialogComponent>,
    private toastr: ToastrService
  ) {}

  model = new OrganizationModel();

  ngOnInit(): void {}

  addOrganization() {
    this.model.createdBy = 'Admin';
    this.model.createdAt = new Date();
    this.model.isActive = 1;
    this.organizationService.addOrganization(this.model).subscribe({
      next: (data) => {
        this.dialogRef.close(true);
      },
      error: (e: HttpErrorResponse) => {
        if(e.status == 409)
        {
          this.toastr.error('Organization with the same name already exist', 'Error', {
            positionClass: 'toast-bottom-right',
          });
        }
        else{
          this.toastr.error('Something went wrong', 'Error', {
            positionClass: 'toast-bottom-right',
          });
          this.dialogRef.close(false);
        }
       
      },
    });
  }
}

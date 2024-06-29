import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { OrganizationService } from 'src/app/shared/services/organization.service';

@Component({
  selector: 'app-delete-dialog',
  templateUrl: './delete-dialog.component.html',
  styleUrls: ['./delete-dialog.component.css']
})
export class DeleteDialogComponent implements OnInit {

  constructor(private organizationService: OrganizationService,
    public dialogRef: MatDialogRef<DeleteDialogComponent>,private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
  }

  deleteConfirm(id)
  {
    this.organizationService.deleteCountry(id).subscribe({
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

import { Component, Inject, OnInit } from '@angular/core';

import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { CertificateService } from '../../shared/services/certificate.service';
import { OrganizationModel } from 'src/app/shared/models/organization-model';
import { CsrModel } from 'src/app/shared/models/csr-model';
import { CertificateEnum } from '../../shared/models/certificate-enum'; // Import your enum file
import { LocalStorage } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-create-dialog',
  templateUrl: './create-dialog.component.html',
  styleUrls: ['./create-dialog.component.css'],
})
export class CreateDialogComponent implements OnInit {
  constructor(
    private certificateService: CertificateService,
    public dialogRef: MatDialogRef<CreateDialogComponent>,
    private toast: ToastrService,
    private localStorageService: LocalStorage,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.organizationList = data.organizationList;
    this.organizationList = data.organizationList.filter(
      (org) => org.isActive === 1
    );
    
      this.isSuperAdmin = this.localStorageService.IsSuperAdmin();
      this.enumKeys = this.certificateService.getFilteredCertificateEnum(this.isSuperAdmin);
  }
  isSuperAdmin: boolean = false;
  certificateTypes = CertificateEnum;
  enumKeys = [];
  organizationList: OrganizationModel[] = [];
  model = new CsrModel();

  ngOnInit(): void {}

  addCertificate() {
    const selectedOrganization = this.organizationList.find(
      (org) => org.id === this.model.organizartionId
    );
    this.model.organizationName = selectedOrganization.organizationName;
    this.model.createdBy = 'Admin';
    this.model.createdAt = new Date();
    // this.model.level = Number(this.model.level);
    this.certificateService.addCertificate(this.model).subscribe({
      next: (data) => {
        this.dialogRef.close(true);
      },
      error: (e: HttpErrorResponse) => {
        this.toast.error('Something went wrong', 'Error', {
          positionClass: 'toast-bottom-right',
        });
        this.dialogRef.close(false);
      },
    });
  }
}

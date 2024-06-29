import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { CertificateModel } from '../../shared/models/certificate-model';
import { CertificateService } from '../../shared/services/certificate.service';
import { OrganizationModel } from 'src/app/shared/models/organization-model';
import { CertificateEnum } from '../../shared/models/certificate-enum'; // Import your enum file
import { QRCodeComponent } from 'angularx-qrcode';
import { QrImageModel } from 'src/app/shared/models/qr-Image-model';
import { CertificateStatus } from 'src/app/shared/models/certificate-status';
import { LocalStorage } from 'src/app/shared/services/local-storage.service';
import { Clipboard } from '@angular/cdk/clipboard';
import { saveAs as importedSaveAs } from 'file-saver';

@Component({
  selector: 'app-edit-dialog',
  templateUrl: './edit-dialog.component.html',
  styleUrls: ['./edit-dialog.component.css'],
})
export class EditDialogComponent implements OnInit {
  model: CertificateModel;
  secondHalfEnumKeys;
  certificateEnum = CertificateEnum;
  @ViewChild(QRCodeComponent) qrCodeComponent: QRCodeComponent;

  constructor(
    private certificateService: CertificateService,
    private clipboard: Clipboard,
    public dialogRef: MatDialogRef<EditDialogComponent>,
    private toast: ToastrService,
    private localStorageService: LocalStorage,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.model = new CertificateModel();
    this.model = data.certificateData;
    this.incidentNo = data.certificateData.incidentNo;
    this.organizationList = data.organizationList;
  }

  isSuperAdmin: boolean = false;
  statusEnum = CertificateStatus;
  isLoading;
  selectedLevel;
  updateStatusModel;
  certificateTypes = CertificateEnum;
  enumKeys = [];
  organizationList: OrganizationModel[] = [];
  qrCodeData;
  emailToSendQrCode;
  incidentNo;

  incidentNoCopied: boolean = false;
  organizationCopied: boolean = false;
  levelCopied: boolean = false;
  infoCopied: boolean = false;
  csrErrorCopied: boolean = false;
  csrCopied: boolean = false;
  certificateCopied: boolean = false;

  ngOnInit(): void {
    this.generateQRCodeData();
    this.populateQrEmail();
  }

  editCertificate() {
    this.certificateService.editCertificate(this.model).subscribe({
      next: (data) => {
        this.toast.success('Certificate Edit', 'Edit', {
          positionClass: 'toast-bottom-right',
        });
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
  findEnumKeyByValue(value: number): string {
    return Object.keys(this.certificateTypes).find(
      (key) => this.certificateTypes[key] === value
    );
  }

  downloadQRCode(parent) {
    this.certificateService.GetQrCode(this.model.id).subscribe({
      next: (data: any) => {
        let fileName = this.model.organizationName + '_' + this.certificateEnum[this.model.level] + '_ ' + this.incidentNo + '.png';
        this.downloadImageFile(data, fileName);
        this.model.certificateStatus = CertificateStatus.Assigned;
        this.dialogRef.close({ resultFor: 'qRCodeEmail', data: this.model });
      },
      error: (e) => {
        console.log('Error', e);
      },
    });
  }

  downloadImageFile(_data: File, fileName: string) {
    const blob = new Blob([_data], { type: 'image/png' });
    importedSaveAs(blob, fileName, { autoBom: true });
  }

  downloadCertificate() {
    this.certificateService.downloadCertificate(this.model.id)
      .subscribe({
        next: (data: any) => {
          let filename = this.model.organizationName + '_' + this.certificateEnum[this.model.level] + '_ ' + this.model.tag + '.png';
          this.downloadImageFile(data, filename);
        },
        error: (e) => {
          console.log('Error', e);
        },
      });
  }
  sendQrCode() {
    if (!this.emailToSendQrCode) {
      // Show an error message or handle validation logic
      this.toast.error('Email is required.', 'Error', {
        positionClass: 'toast-bottom-right',
      });
      return; // Stop further execution if email is empty
    }
    this.isLoading = true;
    const qrImageModel = new QrImageModel();
    qrImageModel.toEmail = this.emailToSendQrCode;
    qrImageModel.certificateIds.push(this.incidentNo);

    this.certificateService.sendQrCodeToEmail(qrImageModel).subscribe({
      next: (data) => {
        this.toast.success('Qr Code sent', 'Success', {
          positionClass: 'toast-bottom-right',
        });
        this.model.certificateStatus = CertificateStatus.Assigned;
        this.dialogRef.close({ resultFor: 'qRCodeEmail', data: this.model });
      },
      error: (e: HttpErrorResponse) => {
        this.toast.error('Something went wrong', 'Error', {
          positionClass: 'toast-bottom-right',
        });
        this.dialogRef.close(false);
      },
    });
  }

  // getAdditionalName(value: CertificateEnum): string {
  //   return this.CertificateTypeShort[value] || '';
  // }
  generateQRCodeData(): void {
    // Construct the QR code data string
    if (this.model) {
      // const certTpye = this.getAdditionalName(this.model.level);
      const certType = CertificateEnum[this.model.level];

      this.qrCodeData = `n=${this.model.incidentNo}; ou=${certType}; o=${this.model.organizationName}`;
    }
  }

  populateQrEmail() {
    // Filter organizationList based on model.organizationId
    const selectedOrganization = this.organizationList.find(
      (org) => org.id === this.model.organizartionId
    );

    // Set emailToSendQrCode if a matching organization is found
    if (selectedOrganization) {
      this.emailToSendQrCode = selectedOrganization.email;
    }
  }

  updateCertificateStatus() {
    this.certificateService
      .updateCertificateStatus(this.updateStatusModel)
      .subscribe({
        next: (data) => {
          // Handle success, if needed
        },
        error: (e: HttpErrorResponse) => {
          // Handle error, if needed
        },
      });
  }


  signCertificate() {
    this.certificateService
      .signCertificate(this.model.incidentNo)
      .subscribe({
        next: (data) => {
          this.toast.success('', 'Success', {
            positionClass: 'toast-bottom-right',
          });
          this.dialogRef.close({ resultFor: 'signCSR'});
        },
        error: (e: HttpErrorResponse) => {
          if(e.status == 404)
            {
              this.toast.error(e.error, 'Error', {
                positionClass: 'toast-bottom-right',
              });
            }
            else{
              this.toast.error('Something went wrong', 'Error', {
                positionClass: 'toast-bottom-right',
              });
            }
        },
      });
  }

  isDownloadButtonEnabled(): boolean {
    return (
      this.model.certificateStatus === CertificateStatus.Assigned ||
      this.model.certificateStatus === CertificateStatus.Created
    );
  }

  // copyToClipboard() {
  //   const incidentNoValue = this.model.incidentNo;

  //   // Check if the incident number is not empty before copying
  //   if (incidentNoValue) {
  //     this.clipboard.copy(incidentNoValue);
  //     // You can optionally provide some feedback to the user
  //     console.log('Incident number copied to clipboard');
  //   }
  // }

  copyToClipboard(property: string, value: string) {
    if (property == 'incidentNoCopied') {
      this.incidentNoCopied = true;
    } else if (property == 'organizationCopied') {
      this.organizationCopied = true;
    } else if (property == 'infoCopied') {
      this.infoCopied = true;
    }
    else if (property == 'levelCopied') {
      this.levelCopied = true;
    } else if (property == 'csrCopied') {
      this.csrCopied = true;
    } else if (property == 'certificateCopied') {
      this.certificateCopied = true;
    }
    else if (property == 'csrErrorCopied') {
      this.csrErrorCopied = true;
    }
    this.clipboard.copy(value);
  }

  copyTypeToClipboard(value) {
    this.levelCopied = true;
    this.clipboard.copy(CertificateEnum[value]);
  }
}

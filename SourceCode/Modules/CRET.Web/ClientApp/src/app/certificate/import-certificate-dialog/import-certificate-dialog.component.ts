import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CertificateService } from 'src/app/shared/services/certificate.service';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-import-certificate-dialog',
  templateUrl: './import-certificate-dialog.component.html',
  styleUrls: ['./import-certificate-dialog.component.css']
})
export class ImportCertificateDialogComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<ImportCertificateDialogComponent>,
    private toast: ToastrService,
    private certificateService: CertificateService) {}

  @ViewChild('fileInput') fileInput: ElementRef;
  selectedFile: File;
  fileContent: string;
  isLoading= false;

  ngOnInit(): void {}

  selectFile(): void {
    this.fileInput.nativeElement.click();
  }


  handleFileInput(event: any): void {
    const files: FileList = event.target.files;
    if (files.length > 0) {
      this.selectedFile = files[0];

      // Read file content
      this.readFileContent();
    }
  }


  readFileContent(): void {
    const reader = new FileReader();

    reader.onload = (e) => {
      // The result attribute contains the file's content as a data URL
      this.fileContent = reader.result as string;
    };

    // Read the file as text
    reader.readAsText(this.selectedFile);
  }

  uploadFile(): void {
    this.isLoading = true;
    if (!this.fileContent) {
      // File content is empty or undefined
      this.toast.error('Please select a certificate file', 'Error', {
        positionClass: 'toast-bottom-right',
      });
      this.isLoading = false;
      return;
    }
    const csrContent = { content: this.fileContent };

    this.certificateService.importCertificate(csrContent).subscribe({
      next: (data) => {
        this.dialogRef.close(true);
      },
      error: (e: HttpErrorResponse) => {
        if(e.status == 404)
        {
          this.toast.error('Failed to match incident number', 'Error', {
            positionClass: 'toast-bottom-right',
          });
        }
        else{
          this.toast.error('Something went wrong', 'Error', {
            positionClass: 'toast-bottom-right',
          });
        }
        this.dialogRef.close(false);
      },
    });
  }
}

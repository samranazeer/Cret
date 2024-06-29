import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { RestfulService } from './restful.service';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CertificateEnum } from '../models/certificate-enum';
import { CertificateStatus } from '../models/certificate-status';

@Injectable({
  providedIn: 'root',
})
export class CertificateService {
  constructor(private restfulApi: RestfulService, private http: HttpClient) { }
  baseURL = environment.baseUrl;

  getCertificateName(value: CertificateEnum): string {
    switch (value) {
      case CertificateEnum.Con: return 'Consumer';
      case CertificateEnum.Ser: return 'Service';
      case CertificateEnum.Pul: return 'Pulse';
      case CertificateEnum.Lab: return 'Laboratory';
      case CertificateEnum.Pro: return 'Production';
      case CertificateEnum.Bat: return 'Batch';
      case CertificateEnum.Ins: return 'Installer';
      // Add more cases for other enum values with their respective descriptive names
      default: return '';
    }
  }

  getCertificateStatusDisplayName(value: CertificateStatus): string {
    switch (value) {
      case CertificateStatus.Created: return 'Created';
      case CertificateStatus.Assigned: return 'Assigned';
      case CertificateStatus.CSR_Received: return 'Csr Received';
      case CertificateStatus.CertificateCreated: return 'Certificate Created';
      default: return '';
    }
  }


  getCertificateStatusEnum() {
    return Object.keys(CertificateStatus)
      .filter((key) => !isNaN(Number(CertificateStatus[key])))
      .map((key) => ({
        key: CertificateStatus[key],
        name: this.getCertificateStatusDisplayName(CertificateStatus[key]),
      }))
  }

  getFilteredCertificateEnum(isSuperAdmin: boolean) {
    return Object.keys(CertificateEnum)
      .filter((key) => !isNaN(Number(CertificateEnum[key])))
      .filter((key) => {
        const enumValue = CertificateEnum[key];
        // Exclude specific values for all users
        if (enumValue === CertificateEnum.Pro) {
          return false;
        }
        // Check if the user is not a super admin and filter out specific values
        if (!isSuperAdmin) {
          return (
            enumValue !== CertificateEnum.Lab &&
            enumValue !== CertificateEnum.Bat &&
            enumValue !== CertificateEnum.Pul
          );
        }
        return true; // Include all values for super admin
      })
      .map((key) => ({
        key: CertificateEnum[key],
        name: this.getCertificateName(CertificateEnum[key]),
      }));
  }

  GetQrCode(id) {
    const url = `${this.baseURL}Certificate/DownloadQrCode/${id}`;

    return this.http.get<any>(url, { responseType: 'arrayBuffer' as 'json' })
      .pipe(
        map((file: ArrayBuffer) => {
          return file
        })
      ).pipe(catchError(this.errorHandler));
  }

  downloadCertificate(id) {
    const url = `${this.baseURL}Certificate/DownloadCertificate/${id}`;

    return this.http.get<any>(url, { responseType: 'arrayBuffer' as 'json' })
      .pipe(
        map((file: ArrayBuffer) => {
          return file
        })
      ).pipe(catchError(this.errorHandler));
  }

  getAllCertificates(): Observable<any> {
    return this.restfulApi.GetReq('Certificate/getCertificates').pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  getDatatable(
    pageNumber: number,
    pageSize: number,
    sort: string,
    filter: string,
    filterOptions: string
  ) {
    const url = `${this.baseURL}Certificate/Datatable?sort=${sort}&page=${pageNumber}&per_page=${pageSize}&filter=${filter}&filterOptions=${filterOptions}`;
    return this.http.get<any>(url).pipe(catchError(this.errorHandler));
  }
  addCertificate(data) {
    return this.http
      .post<any>(this.baseURL + 'Certificate', data)
      .pipe(catchError(this.errorHandler));
  }

  updateCertificateStatus(data) {
    return this.http
      .post<any>(this.baseURL + 'Certificate/UpdateCertificateStatus', data)
      .pipe(catchError(this.errorHandler));
  }

  sendQrCodeToEmail(data) {
    return this.http
      .post<any>(this.baseURL + 'Certificate/SendQrCode', data)
      .pipe(catchError(this.errorHandler));
  }


  importCsr(data) {
    return this.http
      .post<any>(this.baseURL + 'Certificate/ImportCsr', data)
      .pipe(catchError(this.errorHandler));
  }


  importCertificate(data) {
    return this.http
      .post<any>(this.baseURL + 'Certificate/ImportCertificate', data)
      .pipe(catchError(this.errorHandler));
  }


  editCertificate(data) {
    return this.http
      .put<any>(this.baseURL + 'Certificate/' + data.id, data)
      .pipe(catchError(this.errorHandler));
  }

  deleteCertificate(id) {
    return this.http
      .delete<any>(this.baseURL + 'certificate/' + id)
      .pipe(catchError(this.errorHandler));
  }


  signCertificate(incidentNo) {
    return this.http
      .post<any>(this.baseURL + 'Certificate/SignCertificate/' + incidentNo,null)
      .pipe(catchError(this.errorHandler));
  }
  errorHandler(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}

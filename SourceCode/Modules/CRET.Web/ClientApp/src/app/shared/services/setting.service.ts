import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { RestfulService } from './restful.service';

@Injectable({
  providedIn: 'root',
})
export class SettingService {
  constructor(private http: HttpClient,private restfulApi: RestfulService, ) {}
  baseURL = environment.baseUrl;

  GetCertificateValidity(): Observable<any> {
    return this.restfulApi.GetReq('Settings/GetCertificateValidity').pipe(
      map((response: any) => {
        return response;
      })
    );
  }
  
  saveSettings(data) {
    return this.http
      .post<any>(this.baseURL + 'Settings/AddUpdateCertificateValidity', data)
      .pipe(catchError(this.errorHandler));
  }

  
  errorHandler(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}

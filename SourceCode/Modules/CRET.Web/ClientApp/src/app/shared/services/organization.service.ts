import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { RestfulService } from './restful.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  constructor(private restfulApi: RestfulService, private http: HttpClient) {}
  baseURL = environment.baseUrl;

  getAllOrganizations(): Observable<any> {
    return this.restfulApi.GetReq('Organization/GetOrganizations').pipe(
      map((response: any) => {
        return response;
      })
    );
  }


  getDatatable(
    pageNumber: number,
    pageSize: number,
    sort: string,
    filter: string
  ) {
    const url = `${this.baseURL}Organization/Datatable?sort=${sort}&page=${pageNumber}&per_page=${pageSize}&filter=${filter}`;
    return this.http.get<any>(url).pipe(catchError(this.errorHandler));
  }
  
  addOrganization(data) {
    return this.http
      .post<any>(this.baseURL + 'Organization', data)
      .pipe(catchError(this.errorHandler));
  }

  editOrganization(data) {
    return this.http
      .put<any>(this.baseURL + 'Organization/' + data.id, data)
      .pipe(catchError(this.errorHandler));
  }

  deleteCountry(id) {
    return this.http
      .delete<any>(this.baseURL + 'Organization/' + id)
      .pipe(catchError(this.errorHandler));
  }
  errorHandler(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}

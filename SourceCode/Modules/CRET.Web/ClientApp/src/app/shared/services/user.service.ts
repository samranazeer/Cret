import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { RestfulService } from './restful.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private restfulApi: RestfulService, private http: HttpClient) {}
  baseURL = environment.baseUrl;



  checkIsUserAllowed(): Observable<any> {
    return this.restfulApi.GetReq('User/GetAllowedStatus').pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  GetAllowedUsers(): Observable<any> {
    return this.restfulApi.GetReq('User/GetAllowedUsers').pipe(
      map((response: any) => {
        return response;
      })
    );
  }


  sendAllowedUsers(data) {
    return this.http
      .post<any>(this.baseURL + 'User', data)
      .pipe(catchError(this.errorHandler));
  }

  getUserProfilePicture() : Observable<any>{


    return this.http.get('https://graph.microsoft.com/v1.0/me/photo/$value', { responseType: 'blob'  })
    .pipe(
      map((file: any) => {
        return file;
      })
    ).pipe(catchError(this.errorHandler));
  }
 

  
  errorHandler(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}

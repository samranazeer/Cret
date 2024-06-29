import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LocalStorage } from '../shared/services/local-storage.service';
import { Profile } from '../shared/models/profile-model';

@Injectable()
export class CretHttpInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private localStorageService: LocalStorage
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // Get the user ID from wherever you have it stored (e.g., localStorage, service, etc.)
    let authReq = request;
    const userProfile: Profile = this.localStorageService.getUser();
    if (userProfile != null) {
      authReq = request.clone({
        setHeaders: {
          'user-id': userProfile.id,
          'user-email': userProfile.userPrincipalName
        }
      });
    }
    // Clone the request and add the user ID to the headers

    return next.handle(authReq).pipe(
      catchError((error) => {
        if (error.status === 400) {
          // this.router.navigateByUrl(`/not-found`); //BadRequest
        } else if (error.status === 401) {
          this.router.navigateByUrl(`/unauthorized-access`);
        }
        // else if (error.status === 404) {
        //   this.router.navigateByUrl(`/unauthorized-access`); //Not Found
        // }
        return throwError(() => error);
      })
    );
  }
}

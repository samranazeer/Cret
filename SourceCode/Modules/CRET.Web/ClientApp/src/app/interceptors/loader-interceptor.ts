import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
} from '@angular/common/http';
import { combineLatest, Observable, timer } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';
import { LoaderService } from '../shared/services/loader.service';

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
  constructor(private loaderService: LoaderService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (request.url.includes('RefreshToken')) {
      return next.handle(request);
    } else {
      // Do not start loading if downloading report
      // Download report in background
      // On report generation, send an email to the user containing that report
      this.loaderService.listOfUrls.push(request.url);
      this.loaderService.isLoading = true;

      return (
        combineLatest(timer(250), next.handle(request))
          // Getting the value of the second observable
          .pipe(
            map((x) => {
              return x[1];
            })
          )
          .pipe(
            finalize(() => {
              let index = this.loaderService.listOfUrls.findIndex(
                (x) => x == request.url
              );
              index > -1 ? this.loaderService.listOfUrls.splice(index, 1) : 0;

              setTimeout(() => {
                if (this.loaderService.listOfUrls.length == 0) {
                  this.loaderService.isLoading = false;
                }
              }, 500);
            })
          )
      );
    }
  }
}

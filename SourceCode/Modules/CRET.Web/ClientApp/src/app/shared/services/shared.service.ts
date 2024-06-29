// shared.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private isUserLoggedInSubject = new BehaviorSubject<boolean>(false);
  isUserLoggedIn$ = this.isUserLoggedInSubject.asObservable();

  get isUserLoggedIn(): boolean {
    return this.isUserLoggedInSubject.value;
  }

  login() {
    this.isUserLoggedInSubject.next(true);
  }

  logout() {
    this.isUserLoggedInSubject.next(false);
  }
}

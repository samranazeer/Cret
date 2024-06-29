import { Injectable } from '@angular/core';


const USER_KEY = 'auth-user';


@Injectable({
  providedIn: 'root'
})
export class LocalStorage  {
  certificatesortingKey = 'certificateSortingState';
  organizationSortingStateKey = 'organizationSortingState';
  private readonly STORAGE_KEY = 'redirectUrl';

  constructor() { }

  signOut(): void {
    window.localStorage.clear();
  }
  public saveUserInfo(user: any): void {
    window.localStorage.removeItem(USER_KEY);
    window.localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  public getUser(): any {
    const user = window.localStorage.getItem(USER_KEY);
    if (user) {
      return JSON.parse(user);
    }

    return null;
  }
  IsSuperAdmin() {
    const userProfile = this.getUser();
    if (userProfile != null) {
      return userProfile.isSuperAdmin;
    }
  }

  
  getSortingState(key): any {
    return JSON.parse(localStorage.getItem(key));
  }

  setSortingState(key: string, sortingState: any): void {
    localStorage.setItem(key, JSON.stringify(sortingState));
  }

  clearSortingState(key): void {
    localStorage.removeItem(key);
  }

  saveRedirectUrl(url: string) {
    localStorage.setItem(this.STORAGE_KEY, url);
  }

  getRedirectUrl(): string | null {
    return localStorage.getItem(this.STORAGE_KEY);
  }

  clearRedirectUrl() {
    localStorage.removeItem(this.STORAGE_KEY);
  }
  
}

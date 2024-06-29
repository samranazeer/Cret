import { Component, OnDestroy, OnInit } from '@angular/core';
import { Profile } from './shared/models/profile-model';
import { UserService } from './shared/services/user.service';
import { HttpErrorResponse } from '@angular/common/http';
import { LocalStorage } from './shared/services/local-storage.service';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { Subject, filter, takeUntil } from 'rxjs';
import { Router, NavigationEnd } from '@angular/router';
import { SharedService } from './shared/services/shared.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(
    private userService: UserService,
    private localStorageService: LocalStorage,
    private msalBroadCastService: MsalBroadcastService,
    private authService: MsalService,
    private router: Router,
    private sharedService: SharedService
  ) { }

  accounts;
  isLoginSucceed = false;
  title = 'app';
  profile?: Profile = null;
  private readonly _destroy = new Subject<void>();

  ngOnDestroy(): void {
    this._destroy.next(undefined);
    this._destroy.complete();
  }

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        const currentUrl = this.router.url;
        this.localStorageService.saveRedirectUrl(currentUrl);
      }
    });
    this.msalBroadCastService.inProgress$
      .pipe(
        filter(
          (interactionStatus: InteractionStatus) =>
            interactionStatus == InteractionStatus.None
        ),
        takeUntil(this._destroy)
      )
      .subscribe((x) => {
        const isLogedIn = this.getLoggedInUserData();
        if (!isLogedIn) {
          this.router.navigate(['/login']); // Navigate to login component
        } else {
          let res = this.localStorageService.getUser();
          if (res == null && this.profile != null) {
            this.localStorageService.saveUserInfo(this.profile); // Assuming you store the profile in localStorage
          }
          if (this.profile != null || res != null) {
            this.sharedService.login();
            this.checkUserAllowed();
          }
        }
      });
  }

  getLoggedInUserData() {
    this.accounts = this.authService.instance.getAllAccounts();
    if (this.accounts.length > 0) {
      let res = this.accounts[0];
      this.profile = new Profile();
      this.profile.id = res.localAccountId;
      this.profile.userPrincipalName = res.username;
      this.profile.displayName = res.name;
      this.profile.mail = res.username;
      return true;
    } else {
      return false;
    }
  }
  checkUserAllowed() {
    this.userService.checkIsUserAllowed().subscribe({
      next: (data) => {
        this.isLoginSucceed = true;
        this.profile.isSuperAdmin = data.isSuperAdmin;
        this.localStorageService.saveUserInfo(this.profile); // Assuming you store the profile in localStorage

        const redirectUrl = this.localStorageService.getRedirectUrl();
        if (redirectUrl && redirectUrl != '/login') {
          this.router.navigateByUrl(redirectUrl);
          this.localStorageService.clearRedirectUrl(); // Clear the persisted URL after redirecting
        }
        else {
          this.router.navigate(['']); // Navigate to home component or wherever you want to redirect

        }
      },
      error: (e: HttpErrorResponse) => {
        this.isLoginSucceed = false;
        this.localStorageService.signOut();
      },
    });
  }

} 

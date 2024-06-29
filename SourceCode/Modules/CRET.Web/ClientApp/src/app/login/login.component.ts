import { Component, Inject, OnInit } from '@angular/core';
import {
  MSAL_GUARD_CONFIG,
  MsalBroadcastService,
  MsalGuardConfiguration,
  MsalService,
} from '@azure/msal-angular';
import { LocalStorage } from '../shared/services/local-storage.service';
import { RestfulService } from '../shared/services/restful.service';
import { RedirectRequest } from '@azure/msal-browser';
import { SharedService } from '../shared/services/shared.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private msalBroadCastService: MsalBroadcastService,
    private authService: MsalService,
    private localStorageService: LocalStorage,
    private restfulService: RestfulService,
    private sharedService: SharedService
  ) {
    this.sharedService.isUserLoggedIn$.subscribe((isLoggedIn) => {
      this.showSpinner = isLoggedIn;
    });
  }

  showSpinner = false;

  ngOnInit(): void {
    // this.showSpinner = this.isLoader;
    // let res = this.localStorageService.getUser();
    // this.showSpinner = res != null ? true : false;
  }
  login() {
    if (this.msalGuardConfig.authRequest) {
      this.sharedService.login();
      this.authService.loginRedirect({
        ...this.msalGuardConfig.authRequest,
      } as RedirectRequest);
    } else {
      this.authService.loginRedirect();
    }
  }
}

import { Component, Inject, OnInit } from '@angular/core';
import {
  MsalBroadcastService,
  MsalGuardConfiguration,
  MsalService,
  MSAL_GUARD_CONFIG,
} from '@azure/msal-angular';
import { RedirectRequest } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';
import { Profile } from '../shared/models/profile-model';
import { MicrosoftUserList } from '../shared/models/user-list-model';
import { RestfulService } from '../shared/services/restful.service';
import { LocalStorage } from '../shared/services/local-storage.service';
import { PLACEHOLDER_AVATAR } from '../constants/index';
import { UserService } from '../shared/services/user.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  isSuperAdmin: boolean = false;
  isUserLoggedIn: boolean = false;
  profile?: Profile;
  constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private msalBroadCastService: MsalBroadcastService,
    private authService: MsalService,
    private localStorageService: LocalStorage,
    private restfulService: RestfulService,
    private userService: UserService
  ) {
    this.isSuperAdmin = this.localStorageService.IsSuperAdmin();
  }

  imageAvatarSize = 36;
  profileImageUrl =
    'https://material.angular.io/assets/img/examples/shiba1.jpg';
  personImagePlaceHolder = PLACEHOLDER_AVATAR;

  ngOnInit(): void {
    this.profile = this.localStorageService.getUser();
    this.getProfilePic();
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  login() {
    if (this.msalGuardConfig.authRequest) {
      this.authService.loginRedirect({
        ...this.msalGuardConfig.authRequest,
      } as RedirectRequest);
    } else {
      this.authService.loginRedirect();
    }
  }

  logout() {
    this.authService.logoutRedirect({
      postLogoutRedirectUri: environment.postLogoutUrl,
    });
    this.localStorageService.signOut();
    this.localStorageService.clearSortingState(this.localStorageService.organizationSortingStateKey);
    this.localStorageService.clearSortingState(this.localStorageService.certificatesortingKey);

  }

  getProfilePic() {
    if (this.profile?.profileImage == null || this.profile.profileImage == undefined) {
      this.userService.getUserProfilePicture().subscribe({
        next: (profilePicBlob: Blob) => {
          const reader = new FileReader();
          reader.onload = (event: any) => {
            this.profileImageUrl = event.target.result;
            this.profile.profileImage = this.profileImageUrl;
            this.localStorageService.saveUserInfo(this.profile);
          };
          reader.readAsDataURL(profilePicBlob);
        },
        error: (error) => {
          // console.error('Error fetching profile picture:', error);
          // Handle error appropriately
        },
      });
    }
  }
}

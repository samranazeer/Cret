import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { MaterialModule } from './material/material.module';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { SettingsComponent } from './settings/settings.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { QRCodeModule } from 'angularx-qrcode';

//Azure AD Imports
import {
  MsalGuard,
  MsalInterceptor,
  MsalModule,
  MsalRedirectComponent,
} from '@azure/msal-angular';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { environment } from '../environments/environment';

//Import app modules
import { ToastrModule } from 'ngx-toastr';
import { OrganizationModule } from './organization/organization.module';
import { CertificateModule } from './certificate/certificate.module';
import { UsersModule } from './users/users.module';
import { SharedModule } from './shared/shared.module';
import { LoaderComponent } from './loader/loader.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';

//Interceptors
import { CretHttpInterceptor } from './interceptors/cret-http-interceptor';
import { UnAuthorizedAccessComponent } from './shared/components/un-authorized-access/un-authorized-access.component';
import { LoginComponent } from './login/login.component';
import { LoaderInterceptor } from './interceptors/loader-interceptor';
import { ProfileImageAvatarComponent } from './shared/components/profile-image-avatar/profile-image-avatar.component';

const isIE =
  window.navigator.userAgent.indexOf('MSIE') > -1 ||
  window.navigator.userAgent.indexOf('Trident/') > -1;

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    SettingsComponent,
    LoginComponent,
    LoaderComponent,
    ProfileImageAvatarComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    QRCodeModule,
    ReactiveFormsModule,
    MaterialModule,
    OrganizationModule,
    CertificateModule,
    UsersModule,
    SharedModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'not-found', component: NotFoundComponent },
      { path: 'unauthorized-access', component: UnAuthorizedAccessComponent },
    ]),
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    MsalModule.forRoot(
      new PublicClientApplication({
        auth: {
          clientId: environment.clientId,
          redirectUri: environment.redirectUri,
          authority: environment.authority,
        },
        cache: {
          cacheLocation: 'localStorage',
          storeAuthStateInCookie: isIE,
        },
      }),
      {
        interactionType: InteractionType.Redirect,
        authRequest: {
          scopes: ['user.read'],
        },
      },
      {
        interactionType: InteractionType.Redirect,
        protectedResourceMap: new Map([
          ['https://graph.microsoft.com/v1.0/me', ['user.Read']],
          ['https://graph.microsoft.com/v1.0/users', ['user.Read.All']],
          [environment.domain, [environment.scope]],
        ]),
      }
    ),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CretHttpInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoaderInterceptor,
      multi: true,
    },
    MsalGuard,
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, from } from 'rxjs';
import { concatMap, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Profile } from '../models/profile-model';
import { MicrosoftUserList } from '../models/user-list-model';
import { MicrosoftUser } from '../models/user-model';

const PROFILE_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';
const USERS_ENDPOINT = 'https://graph.microsoft.com/v1.0/users';

@Injectable({
  providedIn: 'root',
})
export class RestfulService {
  baseURL = environment.baseUrl;

  constructor(private http: HttpClient) {
  }

  public GetRequest(url: string, showLoader: boolean = true, parameters?: HttpParams): Observable<any> {
    return this.http.get(this.baseURL + url, {
      observe: "response",
      params: parameters
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public GetRequestWithParametersInUrl(url: string, showLoader: boolean = true): Observable<Object | null> {
    return this.http.get(url, {
      observe: "response"
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public GetReq(url: string) {
    return this.http.get(this.baseURL + url, { observe: "response" }).pipe(map(response => {
      return response.body;
    }));
  }

  public GetRequestBlobResponse(url: string, showLoader: boolean = true, parameters?: HttpParams) {
    return this.http.get(this.baseURL + url, {
      observe: "response",
      params: parameters,
      responseType: 'blob',
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PostRequest(url: string, body: any, showLoader: boolean = true, responseType: any = 'json', parameters?: HttpParams) {
    return this.http.post(this.baseURL + url, JSON.stringify(body), {
      headers: {
        "Content-Type": "application/json"
      },
      observe: 'response',
      responseType: responseType
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PostRequestWithUrl(url: string, body: any, showLoader: boolean = true, responseType: any = 'json', parameters?: HttpParams) {
    return this.http.post(url, JSON.stringify(body), {
      headers: {
        "Content-Type": "application/json"
      },
      observe: 'response',
      responseType: responseType
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PostRequestWithCredentials(url: string, body: any, showLoader: boolean = true, responseType: any = 'json', parameters?: HttpParams) {
    return this.http.post(this.baseURL + url, JSON.stringify(body), {
      headers: {
        "Content-Type": "application/json"
      },
      observe: 'response',
      responseType: responseType,
      withCredentials: true
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public EmptyPostRequest(url: string) {
    return this.http.post(this.baseURL + url, "{}", {
      observe: "response"
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PostRequestWithFormData(url: string, formData: FormData, showLoader: boolean = true, parameters?: HttpParams) {
    return this.http.post(this.baseURL + url, formData, {
      observe: 'response',
      params: parameters,
      headers: new HttpHeaders({ 'HasFormData': 'true' })
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PostRequestWithFormDataBlobResponse(url: string, formData: FormData, showLoader: boolean = true, parameters?: HttpParams) {
    return this.http.post(this.baseURL + url, formData, {
      observe: 'response',
      params: parameters,
      headers: new HttpHeaders({ 'HasFormData': 'true' }),
      responseType: 'blob'
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public PutRequest(url: string, body: any, showLoader: boolean = true, parameters?: HttpParams) {
    return this.http.put(this.baseURL + url, JSON.stringify(body), {
      observe: "response",
      params: parameters,
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }

  public DeleteRequest(url: string, body: any, parameters?: HttpParams) {
    return this.http.delete(this.baseURL + url, {
      headers: {
        "Content-Type": "application/json"
      },
      observe: "response",
      params: parameters,
      body: JSON.stringify(body)
    }).pipe(
      map(response => {
        return response.body;
      })
    );
  }
  getUserProfile() {
    return this.http.get<Profile>(PROFILE_ENDPOINT);
  }
  getUsersList() {
    return this.http.get<MicrosoftUserList>(USERS_ENDPOINT);
  }

  getAllUsersList(): Observable<MicrosoftUser[]> {
    const pageSize = 999; // Change this value according to your needs
    const url = `${USERS_ENDPOINT}?$top=${pageSize}`;
    return this.getAllPages(url);
  }
  getAllPages(url: string, users: MicrosoftUser[] = []): Observable<MicrosoftUser[]> {
    return this.http.get<MicrosoftUserList>(url).pipe(
      concatMap((response: MicrosoftUserList) => {
        users.push(...response.value);
        if (response['@odata.nextLink']) {
          return this.getAllPages(response['@odata.nextLink'], users);
        }
        return from([users]);
      })
    );
  }
}

import {Injectable} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/catch';

import { PortalService } from './portal.service';


export interface ITokenRefresh {
    access_token : string;
}

@Injectable()
export class AuthenticatedHttpService {
  public apiUrl: string;
  public authUrl: string;
  public webUrl: string;

  constructor(private http: HttpClient, private portalService: PortalService) {
    console.log("Api url: " + portalService.appSettings.baseUrls.api);
    console.log("Auth url: " + portalService.appSettings.baseUrls.auth);
    console.log("Web url: " + portalService.appSettings.baseUrls.web);
    console.log("Access Token (response body): " + portalService.appSettings.accessToken);

    this.apiUrl = portalService.appSettings.baseUrls.api;
    this.authUrl = portalService.appSettings.baseUrls.auth;
    this.webUrl = portalService.appSettings.baseUrls.web;
  }

  getApiPrefix() {
    //return this.portalService.isLive()
    //    ? "api/live/"
    //    : "api/test/";
    return "";
  }

  createHeaders(): HttpHeaders {
    const headers = new HttpHeaders();
    return this.createAuthorizationHeaders(headers);
  }

  createAuthorizationHeaders(headers: HttpHeaders): HttpHeaders {
    return headers.append("Authorization", "Bearer " + this.portalService.appSettings.accessToken);
  }

  get<T>(url: string) {
    const request: () => Observable<T> = () => {
      const headers = this.createHeaders();
      return this.http.get<T>(this.apiUrl + "/" + this.getApiPrefix() + url,
        {
          headers: headers
        });
    };
    return this.refreshTokenOnAuthorizationFailure<T>(request);
  }

  post<T>(url: string, data: any) {
    const request: () => Observable<T> = () => {
      const headers = this.createHeaders();
      headers.append("Content-Type", "application/json");
      return this.http.post<T>(this.apiUrl + this.getApiPrefix() + url,
        data,
        {
          headers: headers
        });
    };
    return this.refreshTokenOnAuthorizationFailure<T>(request);
  }

  refreshTokenOnAuthorizationFailure<T>(request: () => Observable<T>) : Observable<T> {
    return request().catch ((err: any) => {
          // Handle unauthorized responses
          if (err.status === 401) {
              return this.refreshTokens()
                  .flatMap((newTokenResponse: ITokenRefresh) => {
                      if (!newTokenResponse || !newTokenResponse.access_token) {
                          // Something's up with the token refresh. Only solution from here is to re-login
                          // Redirect to logout
                          window.location.href = this.webUrl + "/Account/logoff";
                          return Observable.throw("Could not refresh authentication token. Redirecting to logout.");
                      }
                      this.portalService.appSettings.accessToken = newTokenResponse.access_token;
                      return request();
                  });
          }
          // Pass through the error, we don;t want to process it.
          return Observable.throw(err);
      });
  }

  refreshTokens() {
    console.log("Refreshed access token");
    return this.http.get<ITokenRefresh>(this.webUrl + "/Account/RefreshTokens");
  }
}

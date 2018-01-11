import {Injectable} from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';

import {PortalService} from './portal.service';


export interface ITokenRefresh {
    access_token : string;
}

@Injectable()
export class AuthenticatedHttpService {
    public apiUrl: string;
    public authUrl: string;
    public webUrl: string;

    constructor(private http: Http, private portalService: PortalService) {
        console.log("Api url: " + portalService.appSettings.baseUrls.api);
        console.log("Auth url: " + portalService.appSettings.baseUrls.auth);
        console.log("Web url: " + portalService.appSettings.baseUrls.web);
        console.log("Access Token (responsebody): " + portalService.appSettings.accessToken);

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
    createHeaders() {
        const headers = new Headers();
        this.createAuthorizationHeaders(headers);

        return headers;
    }
    createAuthorizationHeaders(headers: Headers) {
        headers.append("Authorization", "Bearer " + this.portalService.appSettings.accessToken);
    }
    get(url:string) {
        const request = () => {
            const headers = this.createHeaders();
            return this.http.get(this.apiUrl + "/" + this.getApiPrefix() + url,
                {
                    headers: headers
                });
        };
        return this.refreshTokenOnAuthorizationFailure(request);
    }

    post(url: string, data: any) {
        const request = () => {
            const headers = this.createHeaders();
            headers.append("Content-Type", "application/json");
            return this.http.post(this.apiUrl + "/" + this.getApiPrefix() + url,
                data,
                {
                    headers: headers
                });
        };
        return this.refreshTokenOnAuthorizationFailure(request);
    }
    post1(url: string, data: any) {
        const request = () => {
            const headers = this.createHeaders();
//            headers.append("Content-Type", "application/json");
            return this.http.post(this.webUrl + "/" + this.getApiPrefix() + url,
                data,
                {
                    headers: headers
                });
        };
        return this.refreshTokenOnAuthorizationFailure(request);
    }

    refreshTokenOnAuthorizationFailure(request: () => Observable<Response>) {
        return request().catch((err:any) => {
            // Handle unauthorized responses
            if (err.status === 401) {
                return this.refreshTokens()
                    .flatMap((newTokenResponse: ITokenRefresh) => {
                        if (!newTokenResponse || !newTokenResponse.access_token) {
                            // Something's up with the token refresg. Only solution from here is to re-login
                            // Redirect to logout
                            window.location.href = this.webUrl + "/account/logoff";
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
        return this.http.get(this.webUrl + "/Account/RefreshTokens")
            .map((r:Response) => <ITokenRefresh>r.json());
    }
}
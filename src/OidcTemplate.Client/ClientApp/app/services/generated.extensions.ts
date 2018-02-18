
import * as generated from "./generated";

import { HttpClient, HttpResponse } from "@angular/common/http"; // ignore
import { Observable } from "rxjs/Observable"; // ignore
import { Http } from "@angular/http"; // ignore
import { API_BASE_URL } from "./generated"; // ignore

import { Injector } from "@angular/core";
import { PortalService } from "./portal.service";


export class ClientBase {
    private portalService: PortalService;
    private baseHttp: Http;

    constructor(private injector: Injector) {
        this.portalService = injector.get(PortalService);
        this.baseHttp = injector.get(Http);
    }

    protected transformOptions(options: any) {
        options.headers.append("Authorization", "Bearer " + this.portalService.appSettings.accessToken);
        return Promise.resolve(options);
    }

    protected transformResult(url: string, response: Response, processor: (response: Response) => any): Observable<any> {
        
        if (response.status === 401) {
            return this.refreshTokens()
                .flatMap((newTokenResponse: ITokenRefresh) => {
                    if (!newTokenResponse || !newTokenResponse.access_token) {
                        // Something's up with the token refresg. Only solution from here is to re-login
                        // Redirect to logout
                        window.location.href = this.portalService.appSettings.baseUrls.web + "/account/logoff";
                        return Observable.throw("Could not refresh authentication token. Redirecting to logout.");
                    }
                    this.portalService.appSettings.accessToken = newTokenResponse.access_token;
                    return processor(response);
                });
        } else {
            return processor(response);
        }
    }
    
    refreshTokens() {
        console.log("Refreshed access token");
        return this.baseHttp.get(this.portalService.appSettings.baseUrls.web + "/Account/RefreshTokens")
            .map((r) => <ITokenRefresh>r.json());
    }
}

export interface ITokenRefresh {
    access_token: string;
}
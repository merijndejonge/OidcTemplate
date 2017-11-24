import {Injectable} from '@angular/core';
import {IAppSettings} from "./IAppSettings";

declare var oidcTemplateAppSettings: any;

@Injectable()
export class PortalService {
    private pAppSettings : IAppSettings;
    get appSettings(): IAppSettings {
        if (!this.pAppSettings) {
            this.pAppSettings = this.loadSettings();
        }
        return this.pAppSettings;
    }

    private loadSettings() : IAppSettings{
        try {
            const appSettings: IAppSettings = {
                baseUrls: {
                    api: oidcTemplateAppSettings.BaseUrls.Api,
                    auth: oidcTemplateAppSettings.BaseUrls.Auth,
                    web: oidcTemplateAppSettings.BaseUrls.Web
                },
                accessToken: oidcTemplateAppSettings.AccessToken,
                userFullName: oidcTemplateAppSettings.UserFullName,
                userEmail: oidcTemplateAppSettings.UserEmail
            };
            return appSettings;
        } catch (e) {
            return undefined as any;
        }
    }
}
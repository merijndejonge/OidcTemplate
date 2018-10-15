export interface IAppSettings {
    baseUrls: IBaseUrls;
    accessToken: string;
    userFullName: string;
    userEmail: string;
}

export interface IBaseUrls {
    api: string;
    auth: string;
    web:string;
}
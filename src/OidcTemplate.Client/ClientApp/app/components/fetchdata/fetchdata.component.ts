import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import {AuthenticatedHttpService} from "../../services/authenticated-http.service";
@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public forecasts: WeatherForecast[];

    constructor(http: AuthenticatedHttpService, @Inject('BASE_URL') baseUrl: string) {
        http.get('api/SampleData/WeatherForecasts').subscribe(result => {
            this.forecasts = result.json() as WeatherForecast[];
        }, error => console.error(error));
    }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

import { Component, OnInit } from '@angular/core';
import {AuthenticatedHttpService} from "../../services/authenticated-http.service";
@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent implements OnInit{
    public forecasts: WeatherForecast[];

    constructor(private http: AuthenticatedHttpService) {
    }
    ngOnInit(): void {
        this.http.get('api/SampleData/WeatherForecasts').subscribe(result => {
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

import { Component, OnInit } from '@angular/core';
import { AuthenticatedHttpService } from "../../services/authenticated-http.service";
import { SampleDataClient, WeatherForecast, SwaggerException } from '../../generated/servermodel';
import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent implements OnInit {
    private forecasts: WeatherForecast[];
    
    constructor(private http: AuthenticatedHttpService, private sampleDataClient: SampleDataClient) {
    }

    ngOnInit(): void {
        this.sampleDataClient
            .weatherForecasts()
            .subscribe((result: WeatherForecast[]) => {
                this.forecasts = result;
            }, (error: SwaggerException) => console.error(error));
    }
}

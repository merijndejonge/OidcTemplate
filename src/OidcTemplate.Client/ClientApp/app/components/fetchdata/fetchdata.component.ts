import { Component, OnInit } from '@angular/core';
import { SampleDataClient, WeatherForecast, SwaggerException } from '../../services/generated';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent implements OnInit{
    public forecasts: WeatherForecast[];

    constructor(private sampleDataClient: SampleDataClient) {
    }

    ngOnInit(): void {
        this.sampleDataClient.weatherForecasts().subscribe(result => {
            console.log(result)
            this.forecasts = result;
        }, (error: SwaggerException)=> console.error(error));
    }
}

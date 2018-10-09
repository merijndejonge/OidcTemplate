import { Component, OnInit } from '@angular/core';
import { AuthenticatedHttpService } from "../services/authenticated-http.service";

@Component({
  selector: 'fetchdata',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent implements OnInit {
  public forecasts: IWeatherForecast[];

  constructor(private http: AuthenticatedHttpService) {
  }

  ngOnInit(): void {
    this.http.get<IWeatherForecast[]>('api/SampleData/WeatherForecasts').subscribe(
      result => this.forecasts = result,
      error => console.error(error));
  }
}

interface IWeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

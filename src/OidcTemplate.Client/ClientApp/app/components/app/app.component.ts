import { Component } from '@angular/core';
import {PortalService} from "../../services/portal.service"

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(portalService: PortalService) {
        console.log("data= " + JSON.stringify(portalService.appSettings));
    }
}

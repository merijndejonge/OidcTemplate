import { Component } from '@angular/core';
import { AuthenticatedHttpService } from "../../services/authenticated-http.service";

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    constructor(private readonly http: AuthenticatedHttpService) {}

    logout() {
        console.log("logging out");
        this.http.post1("account/LogOff", {}).subscribe(result => {
            },
            error => {
                console.error(error);
            });
    }
}

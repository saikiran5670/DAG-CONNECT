import { Injectable } from '@angular/core';
@Injectable()
export class OriginService {
    origin : string = '';
    getOrigin(){
        this.origin = window.location.origin;
        if(this.origin && this.origin.includes("localhost")){
            this.origin = "https://portal.dev1.ct2.atos.net";
        }
        return this.origin;
    }
}
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/internal/operators';

import {
    HttpClient,
    HttpErrorResponse,
    HttpHeaders,
    HttpParams
} from '@angular/common/http';

declare var H: any;

@Injectable({
    providedIn: 'root'
})

export class HereService {

    public platform: any;
    public geocoder: any;
    public router: any;
    public constructor(private httpClient: HttpClient) {
        this.platform = new H.service.Platform({
            "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
        });
        this.geocoder = this.platform.getGeocodingService();
        this.router = this.platform.getRoutingService(null, 8);
    }

    public getAddress(query: string) {
        return new Promise((resolve, reject) => {
            this.geocoder.geocode({ searchText: query }, result => {
                if (result.Response.View.length > 0) {
                    if (result.Response.View[0].Result.length > 0) {
                        resolve(result.Response.View[0].Result);
                    } else {
                        reject({ message: "no results found" });
                    }
                } else {
                    reject({ message: "no results found" });
                }
            }, error => {
                reject(error);
            });
        });
    }

    public getLocationDetails(geocodingParameters: any) {
        return new Promise((resolve, reject) => {
            this.geocoder.geocode(geocodingParameters,
                (result) => {
                    if (result.Response.View[0].Result.length > 0) {
                        resolve(result.Response.View[0].Result);
                    } else {
                        reject({ message: "no results found" });
                    }
                }, (error) => {
                    reject(error);
                });
        });
    }
    public getAddressFromLatLng(query: string) {
        return new Promise((resolve, reject) => {
            this.geocoder.reverseGeocode({ prox: query, mode: "retrieveAddress" }, result => {
                if (result.Response.View.length > 0) {
                    if (result.Response.View[0].Result.length > 0) {
                        resolve(result.Response.View[0].Result);
                    } else {
                        reject({ message: "no results found" });
                    }
                } else {
                    reject({ message: "no results found" });
                }
            }, error => {
                reject(error);
            });
        });
    }

    getRoutes(params: any): Observable<any> {
        let routeURL = 'https://route.api.here.com/routing/7.2/calculateroute.json?';

        return this.httpClient.get<any>(routeURL + params);
    }

    getSuggestions(params: any): Observable<any> {
        let suggestURL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?';

        return this.httpClient.get<any>(suggestURL + params);
    }

    lookUpSuggestion(qParam: any): Observable<any> {
        let lookUpURL = 'https://lookup.search.hereapi.com/v1/lookup?';
        return this.httpClient.get<any>(lookUpURL + qParam);
    }

    getTruckRoutes(params: any): Observable<any> {
        let routeURL = 'https://router.hereapi.com/v8/routes?';
        return this.httpClient.get<any>(routeURL + (params));
    }
    public calculateRoutePoints(parameters: any) {
        return new Promise((resolve, reject) => {
            this.router.calculateRoute(parameters,
                (result) => {
                    if (result) {
                        resolve(result);
                    } else {
                        reject({ message: "no results found" });
                    }
                }, (error) => {
                    reject(error);
                });
        });
    }
}
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { of } from 'rxjs';
import { delay, catchError } from 'rxjs/internal/operators';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParameterCodec
} from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Injectable()
export class ReportService {
  reportServiceUrl: string = '';
  
  constructor(private httpClient: HttpClient, private config: ConfigService) {
    this.reportServiceUrl = config.getSettings("foundationServices").reportRESTServiceURL;
  }

  generateHeader(){
    let genericHeader : object = {
      'Content-Type' : 'application/json',
      'accountId' : localStorage.getItem('accountId') ? localStorage.getItem('accountId') : 0,
      'orgId' : localStorage.getItem('accountOrganizationId') ? localStorage.getItem('accountOrganizationId') : 0,
      'roleId' : localStorage.getItem('accountRoleId') ? localStorage.getItem('accountRoleId') : 0
    }
    let getHeaderObj = JSON.stringify(genericHeader)
    return getHeaderObj;
  }

  getVINFromTrip(accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
    // .get<any[]>(`${this.reportServiceUrl}/getvinsfromtripstatisticsandvehicledetails?accountId=${accountId}&organizationId=${orgId}`, headers)
      .get<any[]>(`${this.reportServiceUrl}/trip/getparameters?accountId=${accountId}&organizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getUserPreferenceReport(reportId: any, accountId: any, orgId: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      //.get<any[]>(`${this.reportServiceUrl}/getuserpreferencereportdatacolumn?reportId=${reportId}&accountId=${accountId}&organizationId=${orgId}`, headers)
      .get<any[]>(`${this.reportServiceUrl}/userpreference/get?reportId=${reportId}&accountId=${accountId}&organizationId=${orgId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getTripDetails(startTime: any, endTime: any, vin: any){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      //.get<any[]>(`${this.reportServiceUrl}/gettripdetails?StartDateTime=${startTime}&EndDateTime=${endTime}&VIN=${vin}`, headers)
      .get<any[]>(`${this.reportServiceUrl}/trip/getdetails?StartDateTime=${startTime}&EndDateTime=${endTime}&VIN=${vin}`, headers)
      .pipe(catchError(this.handleError));
  }

  createReportUserPreference(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/userpreference/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getDriverTimeDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/drivetime/getdetails`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getSelectedDriverDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/drivetime/getdetailssingle`, data, headers
      )
      .pipe(catchError(this.handleError));
  }
  
  getDefaultDriverParameter(data: any): Observable<any[]> {
      let headerObj = this.generateHeader();
      const headers = {
        headers: new HttpHeaders({ headerObj }),
      };
      return this.httpClient
        .post<any[]>(
          `${this.reportServiceUrl}/drivetime/getparameters`, data, headers
        )
        .pipe(catchError(this.handleError));
  }

  getReportDetails(){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      //.get<any[]>(`${this.reportServiceUrl}/getreportdetails`, headers)
      .get<any[]>(`${this.reportServiceUrl}/getdetails`, headers)
      .pipe(catchError(this.handleError));
  }

  getFleetDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetutilization/getdetails`, data, headers
      )
      .pipe(catchError(this.handleError));
  }


  getEcoScoreDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/ecoscore/getdetailsbyalldriver`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getEcoScoreDriverCompare(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/ecoscore/comparedrivers`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getCalendarDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetutilization/getcalenderdata`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getEcoScoreProfiles(profileFlag: boolean){
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .get<any[]>(`${this.reportServiceUrl}/ecoscore/getprofiles?isGlobal=${profileFlag}`, headers)
      .pipe(catchError(this.handleError));
  }

  getEcoScoreProfileKPIs(profileId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .get<any>(`${this.reportServiceUrl}/ecoscore/getprofilekpis?ProfileId=${profileId}`, headers)
      .pipe(catchError(this.handleError));
  }

  createEcoScoreProfile(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/ecoscore/createprofile`, data, headers
      )
      .pipe(catchError(this.handleError));
  }  

  updateEcoScoreProfile(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .put<any[]>(
        `${this.reportServiceUrl}/ecoscore/updateprofile`, data, headers
      )
      .pipe(catchError(this.handleError));
  }  

  deleteEcoScoreProfile(profileId: number): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
   return this.httpClient
      .delete<any>(`${this.reportServiceUrl}/ecoscore/deleteprofile?ProfileId=${profileId}`, headers)
      .pipe(catchError(this.handleError));
  }

  getGraphDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/vehiclegraph`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getdriverGraphDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/drivergraph`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getDriverTripDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/driver/trip`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getVehicleTripDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/trip`, data, headers
      )
      .pipe(catchError(this.handleError));
  }


  getFleetFuelDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/vehicle`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getFleetFueldriverDetails(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/fleetfuel/getdetails/driver`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  getReportUserPreference(reportId: any): Observable<void> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
    };
   return this.httpClient
      .get<any>(`${this.reportServiceUrl}/reportuserpreference/get?reportId=${reportId}`, headers)
      .pipe(catchError(this.handleError));
  }

  updateReportUserPreference(data: any): Observable<any[]> {
    let headerObj = this.generateHeader();
    const headers = {
      headers: new HttpHeaders({ headerObj }),
      responseType: 'text' as 'json'
    };
    return this.httpClient
      .post<any[]>(
        `${this.reportServiceUrl}/reportuserpreference/create`, data, headers
      )
      .pipe(catchError(this.handleError));
  }

  private handleError(errResponse: HttpErrorResponse) {
    console.error('Error : ', errResponse.error);
    return throwError(
      errResponse
    );
}

getFleetOverviewDetails(data: any): Observable<any[]> {
  let headerObj = this.generateHeader();
  const headers = {
    headers: new HttpHeaders({ headerObj }),
  };
  return this.httpClient
    .post<any[]>(
      `${this.reportServiceUrl}/fleetoverview/getfleetoverviewdetails`, data, headers
    )
    .pipe(catchError(this.handleError));
}

//for getfilterdetails for fleet overview
getFilterDetails(): Observable<any[]> {
  let headerObj = this.generateHeader();
  const headers = {
    headers: new HttpHeaders({ headerObj }),
  };
  return this.httpClient
    .get<any[]>(
      `${this.reportServiceUrl}/fleetoverview/getfilterdetails`, headers
    )
    .pipe(catchError(this.handleError));
}


//Fuel Benchmarking API's

getBenchmarkDataByTimePeriod(data:any ): Observable<any[]> {

  let headerObj = this.generateHeader();
  const headers = {
    headers: new HttpHeaders({ headerObj }),
    responseType: 'text' as 'json'
  };
  return this.httpClient
    .post<any[]>(
      `${this.reportServiceUrl}/fuelbenchmark/timeperiod`, data, headers
    )
    .pipe(catchError(this.handleError));
}

getBenchmarkDataByVehicleGroup(data:any ): Observable<any[]> {

  let headerObj = this.generateHeader();
  const headers = {
    headers: new HttpHeaders({ headerObj }),
    responseType: 'text' as 'json'
  };
  return this.httpClient
    .post<any[]>(
      `${this.reportServiceUrl}fuelbenchmark/vehiclegroup`, data, headers
    )
    .pipe(catchError(this.handleError));
}

getLogBookfilterdetails(): Observable<any[]> {
  let headerObj = this.generateHeader();
 const headers = {
   headers: new HttpHeaders({ headerObj }),
 };
     return this.httpClient
         .get<any[]>(`${this.reportServiceUrl}/fleetoverview/getlogbookfilters`,  headers)
         .pipe(catchError(this.handleError));
 }

}

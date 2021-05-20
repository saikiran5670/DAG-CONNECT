import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.less']
})
export class ShopComponent implements OnInit {

  private domainUrl: string;
  private requestBody: any;
    constructor(private httpClient: HttpClient, private config: ConfigService) {
        this.domainUrl = config.getSettings("foundationServices").authZuoraSSOServiceURL;
    }

    ngOnInit(): void {
    }
  
    generateHeader(){
      let genericHeader : object = {
        'accountId' : localStorage.getItem('accountId'),
        'orgId' : localStorage.getItem('accountOrganizationId'),
        'roleId' : localStorage.getItem('accountRoleId')
      }
      let getHeaderObj = JSON.stringify(genericHeader)
      return getHeaderObj;
    }

  getSsoToken(){
    let headerObj = this.generateHeader();
      const httpOptions = {
          headers: new HttpHeaders({
              headerObj,
              'Accept': 'application/json',
              'Content-Type': 'application/json',
              'responseType': 'application/json'
          }),
          observe: "response" as 'body',
      };
      return this.httpClient.post(`${this.domainUrl}`, null, httpOptions);
    }

    ngAfterContentInit(){
    this.getSsoToken().subscribe((data:any) => {
      if(data.status === 200){
        window.open(data.body, '_blank');
      }
      else if(data.status === 401){
        console.log("Error: Unauthorized");
     }
     else if(data.status == 302){
      console.log("Error: Unauthorized");
     }
    },
    (error)=> {
       if(error.status == 404  || error.status == 403){
        console.log("Error: not found");
       }
       else if(error.status === 401){
        console.log("Error: Unauthorized");
       }
       else if(error.status == 302){
        console.log("Error: Unauthorized");
       }
       else if(error.status == 500){
        console.log("Error: Internal server error");
       }
     })
    }
}
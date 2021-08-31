import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportSchedulerService } from '../services/report.scheduler.service';

@Component({
  selector: 'app-unsubscribe-report',
  templateUrl: './unsubscribe-report.component.html',
  styleUrls: ['./unsubscribe-report.component.less']
})
export class UnsubscribeReportComponent implements OnInit {
  token: string= '';
  emailId: string = '';
  id: number = 0;
  errorStatus: any;

  constructor(public router: Router,private route: ActivatedRoute, private reportSchedulerService: ReportSchedulerService) { }

  ngOnInit(): void {
    this.token=  this.route.snapshot.paramMap.get('token');
    console.log("unsubscribe token = "+this.token);
    this.id= parseInt(this.route.snapshot.paramMap.get('id'));
    console.log("unsubscribe id = "+this.id);
    this.emailId=  this.route.snapshot.paramMap.get('emailId');
    console.log("unsubscribe emailId = "+this.emailId);
    if(this.id > 0){
      this.reportSchedulerService.getUnsubscribeForSingle(this.id,this.emailId).subscribe(response => {
        this.errorStatus= response.code;
        
      }, (error)=> {
        this.errorStatus= error.status;
      })
    }

    if(this.id > 0){
      this.reportSchedulerService.getUnsubscribeForAll(this.emailId).subscribe(response => {
        this.errorStatus= 200;
        
      }, (error)=> {
        this.errorStatus= error.status;
      })
    }
  }

}

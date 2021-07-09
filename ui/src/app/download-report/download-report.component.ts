import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReportSchedulerService } from '../services/report.scheduler.service';

@Component({
  selector: 'app-download-report',
  templateUrl: './download-report.component.html',
  styleUrls: ['./download-report.component.less']
})
export class DownloadReportComponent implements OnInit {
  token: string= '';
  errorMessage: string= '';
  constructor(public router: Router, private route: ActivatedRoute, private reportSchedulerService: ReportSchedulerService) { }

  ngOnInit(): void {
    this.token=  this.route.snapshot.paramMap.get('token');
    this.reportSchedulerService.downloadReportFromEmail(this.token).subscribe(data => {
      this.errorMessage= '';
    }, (error)=> {
      this.errorMessage= error;
    })
  }

}

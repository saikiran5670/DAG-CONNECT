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
    this.reportSchedulerService.downloadReportFromEmail(this.token).subscribe(response => {
      this.errorMessage= '';
      
      let arrayBuffer= response["report"];
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      const linkSource = 'data:application/pdf;base64,' + base64File;
      const downloadLink = document.createElement("a");
      const fileName = response["fileName"]+".pdf";

      downloadLink.href = linkSource;
      downloadLink.download = fileName;
      downloadLink.click();
    }, (error)=> {
      this.errorMessage= error.error;
    })
  }

}

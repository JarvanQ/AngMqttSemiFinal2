import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { SignalRService } from './services/signalr.service'
import { UIChart } from 'primeng/chart';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [SignalRService]
})
export class AppComponent implements OnInit {

  @ViewChild("chart", { static: false }) chart: UIChart;
  options: any;

  constructor(public signalRService: SignalRService, private http: HttpClient) {
  
    this.options = {
      responsive:true,
      title: {
        display: true,
        text: 'Mqtt',
        fontSize: 16
      },
      legend: {
        position: 'bottom'
      }
    };
  }

 
  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();

    setInterval(() => {
        this.chart.refresh();
      },
      1000);


  }


}

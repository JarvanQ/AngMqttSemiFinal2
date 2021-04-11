import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { SignalRService } from './services/signalr.service'
import { ChartModule } from 'primeng/chart'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  //export class AppComponent {
  title = 'app';
  data: any;
  options: any;

  constructor(public signalRService: SignalRService, private http: HttpClient) {
    this.data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label: 'First Dataset',
          data: [65, 59, 80, 81, 56, 55, 40]
        },
        {
          label: 'Second Dataset',
          data: [28, 48, 40, 19, 86, 27, 90]
        }
      ]
    }

    this.options = {
      title: {
        display: true,
        text: 'My Title',
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
    this.startHttpRequest();
  }

  private startHttpRequest = () => {
    this.http.get('https://localhost:5001/mqtt').subscribe(res => { console.log(res); });
  };
}

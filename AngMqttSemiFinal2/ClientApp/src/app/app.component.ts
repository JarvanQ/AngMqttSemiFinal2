import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { SignalRService } from './services/signalr.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  //export class AppComponent {
  title = 'app';

  constructor(public signalRService: SignalRService, private http: HttpClient) { }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();
    this.startHttpRequest();
  }

  private startHttpRequest = () => {
    this.http.get('https://localhost:5001/mqtt').subscribe(res => { console.log(res); });
  };
}

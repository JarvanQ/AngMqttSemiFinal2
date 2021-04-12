import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';

import { MqttMessage } from '../_interfaces/MqttMessage.model'

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  public mqqtData: MqttMessage;

  public chartData = {
    labels: [0],
    datasets: [
      {
        label: 'stored',
        data: [],
        borderColor: '#4bc0c0'
      },
      {
        label: 'received',
        data: [],
        borderColor: '#565656'
      }
      ,
      {
        label: 'sent',
        data: [],
        borderColor: '#965656'
      }
    ]
  }

  private hubConnection: signalR.HubConnection;

  public startConnection = () => {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/mqtt")
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('podkluchilis'))
      .catch(err => console.log('Error:' + err));
  }


  ///Получение данных с хаба
  public addTransferChartDataListener = () => {
    this.hubConnection.on('transferchartdata', (data) => {
      this.mqqtData = data;
      let lablesLength = this.chartData.labels.length;
      var currentSet = this.chartData.datasets.find(x => x.label == this.mqqtData.topic).data;
      if (currentSet.length == lablesLength) {
        this.chartData.labels.push(this.chartData.labels[lablesLength-1]+ 1);
      }

      currentSet.push(this.mqqtData.messageValue);
    });
  }
  constructor() { }
}

import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { HubConnection } from '@aspnet/signalr';

@Injectable()
export class SocketService {

  public hubconnection: HubConnection;

  constructor() {
    this.hubconnection = new signalR.HubConnectionBuilder()
            .withUrl('notify')
            .configureLogging(signalR.LogLevel.Information)
            .build();
            
   }

}

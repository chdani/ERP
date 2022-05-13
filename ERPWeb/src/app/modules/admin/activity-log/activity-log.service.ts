import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WebApiService } from 'app/shared/webApiService';

@Injectable({
  providedIn: 'root'
})
export class ActivityLogService {

  constructor(private http: HttpClient,
    private _webApiService: WebApiService) { }

  private getActiveLog = "getActivityLog/";
  private getExceptionLog = "GetExceptionLog/";

  async getActivityLogList(body: any) {
    var result = await this._webApiService.post(this.getActiveLog, body);
    return result;
  }

  async GetExceptionLogList(body: any) {
    var result = await this._webApiService.post(this.getExceptionLog, body);
    return result;
  }

  setLoginHeader(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
    });
  }

}

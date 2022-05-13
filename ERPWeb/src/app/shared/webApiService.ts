import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { NgxSpinnerService } from "ngx-spinner";

@Injectable({
     providedIn: "root"
})
export class WebApiService {
     private serviceURL = environment.serviceUrl;
     public loadingCounter: number = 0;
     constructor(
          public httpClient: HttpClient,
          private spinner: NgxSpinnerService,
     ) { }

     async post(url: string, data: any): Promise<any> {
          this.showHideProgress(true);
          var res = await this.httpClient.post(this.serviceURL + url, data).toPromise();
          this.showHideProgress(false);
          return res;
     }

     async get(url): Promise<any> {
          this.showHideProgress(true);
          var res = await this.httpClient.get(this.serviceURL + url).toPromise();
          this.showHideProgress(false);
          return res;
     }

     async postDocument(url: string, data: any): Promise<any> {
          this.showHideProgress(true);
          var res = await this.httpClient.post(this.serviceURL+ url,data,{ responseType: "arraybuffer"}).toPromise();
          this.showHideProgress(false);
          return res;
      }


     postObserver(url, data: any): Observable<any> {
          return this.httpClient.post(this.serviceURL + url, data);
     }

     getObserver(url): Observable<any> {
          return this.httpClient.get(this.serviceURL + url);
     }

     showHideProgress(show: boolean) {
          if (show) {
               this.loadingCounter++;
               this.spinner.show();
          } else {
               this.loadingCounter--;
               if (this.loadingCounter == 0) this.spinner.hide();;
          }
     }
}

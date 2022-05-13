import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { WebApiService } from '../webApiService';

@Injectable()
export class AppGlobalDataService {
     constructor(private _webApiservice: WebApiService) { }
     async getEmployeeList(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {
          var employees = [];
          var userData = JSON.parse(localStorage.getItem("LUser")) as any;
          var language = userData.userContext.language;

          var result = await this._webApiservice.get("getEmployeeList");
          if (result && !result.validation) {
               employees = result as any;
               employees.forEach(element => {
                    element.empName = language == "en" ? element.fullNameEng : element.fullNameArb
               });
          }
         
          if (includeAll) {
               var allRecord = {
                    id: "",
                    empName: translate.transform("APP_ALL")
               }
               employees.unshift(allRecord);
          }
          if (incldueEmptyRow) {
               var emptyRecord = {
                    id: "",
                    empName: translate.transform("APP_SELECT")
               }
               employees.unshift(emptyRecord);
          }
          if (activeOnly)
               employees = employees.filter(a => a.active == "Y");
          
          return employees;
     }
}
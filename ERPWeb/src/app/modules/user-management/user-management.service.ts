import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { WebApiService } from 'app/shared/webApiService';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserManagementService {
    private language: string;
    constructor(private _webApiService: WebApiService) {
        var userData = JSON.stringify(localStorage.getItem("LUser")) as any;
        if (userData && userData.userContext)
            this.language = userData.userContext.language;

        if (!this.language) this.language = "en";
    }

    async removeUserRoleCatche(){
        localStorage.removeItem("appUserRole");    
    }

    async getUserRole() {
        let userRoleList: any = [];
        let role = JSON.parse(localStorage.getItem("appUserRole"));

        if (!role || role.length == 0) {
            role = await this._webApiService.get("getUserRoleList");
        }
        
        if (role) {
            // result = screen as any;
            localStorage.setItem("appUserRole", JSON.stringify(role));
            role.forEach(element => {
                userRoleList.push({ code: element.id, name: element.roleName });
            });
        }
        return userRoleList;
    }
//    async getLedgerAccount(id){
//     var results= await this._webApiService.get("getledgerAccount/" + id);
//     var userData = JSON.parse(localStorage.getItem("LUser")) as any;
//     if (userData && userData.userContext)  
//       userData.userContext.ledgerAccounts =results;
//       localStorage.setItem("LUser", JSON.stringify(userData))
//     return results;
//    }
//    async getOrganizationList(id){
//        debugger
//     var results= await this._webApiService.get("getOrganizationList/" + id);
//     var userData = JSON.parse(localStorage.getItem("LUser")) as any;
//     if (userData && userData.userContext)  
//       userData.userContext.organizations =results;
//       localStorage.setItem("LUser", JSON.stringify(userData))
//     return results;
//    }
    async getAppScreen() {
        let screensList: any = [];
        let screen = JSON.parse(localStorage.getItem("appAccessScreen"));

        if (!screen || screen.length == 0) {
            screen = await this._webApiService.get("getAppAccessList");
        }

        if (screen) {
            // result = screen as any;
            localStorage.setItem("appAccessScreen", JSON.stringify(screen));
            screen.forEach(element => {
                screensList.push({ code: element.id, name: element.accessName });
            });
        }

        return screensList;
    }

    // async getUserRoleList()
    // {
    //     var userRole = JSON.parse(localStorage.getItem("userRole"));
    //     if (!userRole || userRole.length == 0) {
    //         var input = { language: this.language };
    //         var result = await this._webApiService.post("fetchAllUserRoleLangBased", input)
    //         if (result) {
    //             var output = result as any;
    //             if (output.validations == null) {
    //                 var userRole = output;
    //                 userRole.forEach(element => {
    //                     // element.ledgerDescCode = element.ledgerCode + " - " + element.ledgerDesc;
    //                 });
    //                 localStorage.setItem("userRole", JSON.stringify(userRole));
    //             }
    //         }
    //     }
    //     return userRole;
    // }

    // async getLedgerAccounts(type: string, includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe)
    // {
    //     var ledgerAccounts = JSON.parse(localStorage.getItem("appLedgers"));
    //     if (!ledgerAccounts || ledgerAccounts.length == 0) {
    //         var input = { language: this.language };
    //         var result = await this._webApiService.post("fetchAllLedgerAccountsLangBased", input)
    //         if (result) {
    //             var output = result as any;
    //             if (output.validations == null) {
    //                 var ledgerAccounts = output;
    //                 ledgerAccounts.forEach(element => {
    //                     element.ledgerDescCode = element.ledgerCode + " - " + element.ledgerDesc;
    //                 });
    //                 localStorage.setItem("appLedgers", JSON.stringify(ledgerAccounts));
    //             }
    //         }
    //     }
    //     if (includeAll) {
    //         var allRecord = {
    //             ledgerCode: "",
    //             ledgerDescCode: translate.transform("APP_ALL")
    //         }
    //         ledgerAccounts.unshift(allRecord);
    //     }
    //     if (incldueEmptyRow) {
    //         var emptyRecord = {
    //             ledgerCode: "",
    //             ledgerDescCode: ""
    //         }
    //         ledgerAccounts.unshift(emptyRecord);
    //     }
    //     return type == "" ? ledgerAccounts : ledgerAccounts.filter(a => a.usedFor == type);
    // }

    // async getCostCenters(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe) {

    //     var costCenters = JSON.parse(localStorage.getItem("appCostCenters"));
    //     if (!costCenters || costCenters.length == 0) {
    //         var input = { language: this.language };
    //         var result = await this._webApiService.post("fetchAllCostCentersLangBased", input)
    //         if (result) {
    //             var output = result as any;
    //             if (output.validations == null) {
    //                 var costCenters = output;
    //                 costCenters.forEach(element => {
    //                     element.codeDescription = element.code + " - " + element.description;
    //                 });
    //                 localStorage.setItem("appCostCenters", JSON.stringify(costCenters));
    //             }
    //         }

    //     }
    //     if (includeAll) {
    //         var allRecord = {
    //             code: "",
    //             codeDescription : translate.transform("APP_ALL")
    //         }
    //         costCenters.unshift(allRecord);
    //     }
    //     if (incldueEmptyRow) {
    //         var emptyRecord = {
    //             code: "",
    //             codeDescription: ""
    //         }
    //         costCenters.unshift(emptyRecord);
    //     }
    //     return costCenters;
    // }

    // async getCashTransactionTypes(translatePipe : any)
    // {
    //     var transtypes: any = [];
    //     transtypes.push({
    //         code: 'E',
    //         description: translatePipe.transform("CASHMGMT_EXPENSES")
    //     })
    //     transtypes.push({
    //         code: 'R',
    //         description: translatePipe.transform("CASHMGMT_RECEIPTS")
    //     })

    //     return transtypes;
    // }
}  
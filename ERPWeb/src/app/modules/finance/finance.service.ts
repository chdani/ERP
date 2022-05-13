import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { WebApiService } from 'app/shared/webApiService';

@Injectable({ providedIn: 'root' })
export class FinanceService {
    private language: string;   
    constructor(private _webApiService: WebApiService) {
        var userData = JSON.parse(localStorage.getItem("LUser")) as any;
        if (userData && userData.userContext)
            this.language = userData.userContext.language;

        if (!this.language) this.language = "en";
       
    }

    async getAccountGroups() {
        var accgrps = JSON.parse(localStorage.getItem("appAccGroups"));
        if (!accgrps || accgrps.length == 0) {
            var result = await this._webApiService.get("getLedgerAccountGroups");
            if (result) {
                accgrps = result as any;
                if (accgrps.validations == null) {
                    localStorage.setItem("appAccGroups", JSON.stringify(accgrps));
                }
            }
        }
        return accgrps;
    }

    async getLedgerAccounts(type: string, includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, orgId : string ) {
               
        var input = { language: this.language };
        var userData = JSON.parse(localStorage.getItem("LUser")) as any;
        if (userData)
        {
            var output = userData.userContext.ledgerAccounts as any;
            var ledgerAccounts = output;
            ledgerAccounts.forEach(element => {
                element.ledgerDescCode = element.ledgerCode + " - " + element.ledgerDesc;
            });  
        }
        if (includeAll)
        {
            var allRecord = {
                id: "",
                ledgerCode:"",
                ledgerDescCode: translate.transform("APP_ALL")
            }
            ledgerAccounts.unshift(allRecord);
        }
        return ledgerAccounts
    }

    async getCostCenters(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe) {

        var costCenters = JSON.parse(localStorage.getItem("appCostCenters"));
        if (!costCenters || costCenters.length == 0) {
            var input = { language: this.language };
            var result = await this._webApiService.post("fetchAllCostCentersLangBased", input)
            if (result) {
                var output = result as any;
                if (output.validations == null) {
                    var costCenters = output;
                    costCenters.forEach(element => {
                        element.codeDescription = element.code + " - " + element.description;
                    });
                    localStorage.setItem("appCostCenters", JSON.stringify(costCenters));
                }
            }
        }
        if (includeAll) {
            var allRecord = {
                code: "",
                codeDescription: translate.transform("APP_ALL")
            }
            costCenters.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                code: "",
                codeDescription: ""
            }
            costCenters.unshift(emptyRecord);
        }
        return costCenters;
    }

    async getCashTransactionTypes(translatePipe: any) {
        var transtypes: any = [];
        transtypes.push({
            code: 'E',
            description: translatePipe.transform("CASHMGMT_EXPENSES")
        })
        transtypes.push({
            code: 'R',
            description: translatePipe.transform("CASHMGMT_RECEIPTS")
        })

        return transtypes;
    }

    async getVendorMaster(){
        var vendor = JSON.parse(localStorage.getItem("appVendors"));
        if (!vendor || vendor.length == 0) {
            var vendor: any = [];
            let result = await this._webApiService.get('getVendorMasterList');
            if (result) {
                if (result.validations == null) {
                    result.forEach(element => {
                        vendor.push({
                            vendorName: element.title + " " + element.name,
                            id: element.id
                        });
                    });
                }
            }
            localStorage.setItem("appVendors", JSON.stringify(vendor));
        }
        return vendor;
    }

    async getPettyCashAccountList() {
        //Get Petty Cash Account
        let accounts: any = [];
        let result = await this._webApiService.get('getPettyCashAccounts');
        if (result) {
            if (result.validations == null) {
                result.forEach(element => {
                    accounts.push({ name: element.accountName, code: element.id });
                });
            }
        }
        return accounts;
    }

    async getTellerList() {
        let tellerList: any = [];

        let teller = await this._webApiService.get('getPettyCashTellers');
        if (teller) {
            if (teller.validations == null) {
                teller.forEach(element => {
                    tellerList.push({ name: element.tellerName, code: element.id, userId:element.userId });
                });
            }
        }
        return tellerList;
    }

    async getOrganizationList() {
        //Get Organization
        let organization: any = [];
        let org = await this._webApiService.get('getOrganizations');
        if (org) {
            if (org.validations == null) {
                org.forEach(element => {
                    organization.push({ name: element.orgName, code: element.id });
                });
            }
        }
        return organization;
    }

    async getEmbassyList(includeAll: boolean, translate: TranslatePipe)
    {
        var embassies = JSON.parse(localStorage.getItem("appEmbassyList"));
        if (!embassies || embassies.length == 0)
        {
            embassies = [];
            var result = await this._webApiService.get("getEmbassyMasters");
            if (result)
            {
                var embasyList = result as any;
                if (embasyList.validatations == null) {
                    embasyList.forEach(element => {
                        embassies.push({
                            embassyName: element.number + " - " + element.nameEng + " - " + element.nameArabic,
                            id: element.id,
                            currencyCode: element.defaultCurrency,
                            embassyNumber: element.number
                        })
                    });
                    localStorage.setItem("appEmbassyList", JSON.stringify(embassies));
                }
            }
        }
        if (includeAll) {
            var allRecord = {
                id: "",
                embassyName: translate.transform("APP_ALL")
            }
            embassies.unshift(allRecord);
        }
        return embassies;
    }

    async getCurrencies(includeAll: boolean, translate: TranslatePipe) {
        var currencies = JSON.parse(localStorage.getItem("appCurrencyList"));
        if (!currencies || currencies.length == 0) {
            currencies = [];
            var result = await this._webApiService.get("getCurrencyMasters");
            if (result) {
                var currencyList = result as any;
                if (currencyList.validatations == null) {
                    currencyList.forEach(element => {
                        currencies.push({
                            currencyName: element.code + " - " + element.name,
                            id: element.id,
                            code: element.code
                        })
                    });

                    localStorage.setItem("appCurrencyList", JSON.stringify(currencies));
                }
            }
        }
        if (includeAll) {
            var allRecord = {
                code: "",
                id:"",
                currencyName: translate.transform("APP_ALL")
            }
            currencies.unshift(allRecord);
        }
        return currencies;
    }
}

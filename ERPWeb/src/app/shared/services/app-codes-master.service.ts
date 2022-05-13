import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { WebApiService } from '../webApiService';

@Injectable()
export class CodesMasterService {
    constructor(private _webApiservice : WebApiService) { }
    
    async getCodesDetailByGroupCode(groupCode: string, includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe)
    {
        var response = [];
        var codes = JSON.parse(localStorage.getItem("appCodeMaster"));

        if (!codes)
        {
            var userData = JSON.parse(localStorage.getItem("LUser")) as any;
            var language = userData.userContext.language;
            if (!language) language = "en";
 
            var input = { language: language };
            var result = await this._webApiservice.post("FetchAllCodesMasterDataLangBased", input);
            if (result && !result.validation) {
                codes = result as any;
                localStorage.setItem("appCodeMaster", JSON.stringify(codes));
            }

        }  
        if (codes && codes.length > 0) {
            response = codes.find(x => x.code == groupCode).codesDetail;
        }
        if (includeAll)
        {
            var allRecord = {
                code: "",
                description: translate.transform("APP_ALL")
            }
            response.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                code: "",
                description: translate.transform("APP_SELECT")
            }
            response.unshift(emptyRecord);
        }
        return response;
    }

}
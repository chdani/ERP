import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
    selector: 'app-codes-master',
    templateUrl: './codes-master.component.html',
    styleUrls: ['./codes-master.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class CodesMasterComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _router: Router,
        private _toastrService: ToastrService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector, 'SCR_CODES_MASTER', 'allowView', _commonService)
    }

    codesMasterInfo: any;
    codesMasterModel: any = [];
    CodeDetails: any = [];
    Details: any = [];
    gridDetailsContextMenu: MenuItem[] = [];
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("CODES_MASTER"));
        this.codesMasterInfo = {
            id: "",
            description: "",
            code: "",
        }
        this.showHideOrgFinyear('SCR_CODES_MASTER');
        this.getSearch();
    }
    async getSearch() {
        debugger
        this.codesMasterModel = await this._webApiservice.post("getCodesMasterSerachFilter", this.codesMasterInfo);
        var result = this.codesMasterModel;
    }

    getGridDetailsContextMenu(item) {
        debugger
        this.gridDetailsContextMenu = [];

        if (this.isGranted('SCR_CODES_MASTER', this.actionType.allowEdit) && (item.active == "Y")) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditCodesMaster(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
    }
    async listExpentation(event) {
        this.CodeDetails = [];
        var result = await this._webApiservice.get("getCodesMasterById/" + event.data.id)
        result.codesDetail.forEach(element => {
            this.CodeDetails.push({
                displayOrder: element.displayOrder, code: element.code, description: element.description,
                active: element.active
            })
        });

    }
    async createOreditCodesMaster(data?: any) {
        this._router.navigate(["app-management/create-edit-codes-master"], {
            state: { codesMasterId: data },
        });
    }
    clearSearchCriteria() {
        this.codesMasterInfo = {
            id: "",
            description: "",
            code: "",
        }
    }
    async saveActiveDeactiveCodesMaster(item) {
        debugger
        var result = await this._webApiservice.post("saveCodesMaster", item);
        if (result) {
            var output = result as any;
            if (output.status == "DATASAVESUCSS") {
                this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                this.CodeDetails = [];
                this.getSearch();
            }
            else {
                this._toastrService.error(output.messages[0])
            }
        }

    }


}

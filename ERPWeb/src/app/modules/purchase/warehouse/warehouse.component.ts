import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
    selector: 'app-warehouse',
    templateUrl: './warehouse.component.html',
    styleUrls: ['./warehouse.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class WareHouseComponent extends AppComponentBase implements OnInit {
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
        super(injector,'SCR_WAREHOUSE','allowView', _commonService )
    }

    wareHouseForm: any;
    wareHouseModel: any = [];
    location: any = [];
    gridDetailsContextMenu: MenuItem[] = [];
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("WAREHOUSE"));
        this.wareHouseForm = {
            id: "",
            name: "",
            email: "",
            contactNo: "",
            address: "",
        }
        this.getSearch();
        this.showHideOrgFinyear('SCR_WAREHOUSE');
    }

    async getSearch() {
        this.wareHouseModel = await this._webApiservice.post("getWareHouseListSearch", this.wareHouseForm)
    }
    async listExpentation(event) {
        this.location = [];
        var result = await this._webApiservice.get("getWareHouseById/" + event.data.id)
        this.location = result.wareHouseLocation;
    }

    getGridDetailsContextMenu(item) {
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_WAREHOUSE', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditWarehouse(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_WAREHOUSE', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markInactiveWareHouse(item) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }
    async markInactiveWareHouse(item) {
        this._confirmService.confirm({
            message: this._translate.transform("WAREHOUSE_DELETE_CONF"),
            accept: async () => {
                item.active = "N";
                var result = await this._webApiservice.post("saveWareHouse", item);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.getSearch();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }
    async createOreditWarehouse(data?: any) {
        this._router.navigate(["purchase/create-edit-warehouse"], {
            state: { wareHouseId: data },
        });
    }
    addWareHouse() {
        this._router.navigate(["purchase/create-edit-warehouse"], {
            state: { wareHouseId: "" },
        });
    }
    clearSearchCriteria() {
        this.wareHouseForm = {
            id: "",
            name: "",
            email: "",
            contactNo: "",
            address: ""
        }
    }

}

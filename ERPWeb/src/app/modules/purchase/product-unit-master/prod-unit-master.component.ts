import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { finalize } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { of, Subject } from 'rxjs';
import { NgxSpinnerService } from 'ngx-spinner';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { PurchaseService } from '../purchase.service';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-prod-unit-master',
    templateUrl: './prod-unit-master.component.html',
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe, DatePipe
    ],
})
export class prodUnitMasterComponent extends AppComponentBase implements OnInit {
    searchInputControl: FormControl = new FormControl();
    selectedProductForm: FormGroup;
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _formBuilder: FormBuilder,
        private _translate: TranslatePipe,
        private _router: Router,
        private _webApiService: WebApiService,
        public _commonService: AppCommonService,
        private spinner: NgxSpinnerService,
        private _confirmService: ConfirmationService,
        private _purchaseService: PurchaseService,
        private _toastrService: ToastrService,


    ) {
        super(injector,'SCR_UNIT_MASTER','allowView', _commonService )
    }
    gridDetailsContextMenu: MenuItem[] = [];
    selectedRow: any;
    panelOpenState = false;
    isLoading: boolean = false;
    statuses: any = [];
    prodUnitMaster: [];
    prodUnitForm: any;
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("PRODUCT_UNIT_MASTER"));
        this.prodUnitMaster = [];
        this.prodUnitForm = {
            id: "",
            unitCode: '',
            unitName: '',
            conversionUnit: "",
            active: ""
        };
        this.getSearch();
        this.showHideOrgFinyear('SCR_UNIT_MASTER');

    }

    addUser() {
        this._router.navigate(["purchase/create-edit-prod-unit-master"], {
            state: { id: "" },

        });
    }

    getGridDetailsContextMenu(item) {
        this.selectedRow = item;
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_UNIT_MASTER', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditprodUnit(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }

        if (this.isGranted('SCR_UNIT_MASTER', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.prodUnitDelete(item) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }
    async getSearch() {
        if (this.prodUnitForm.unitCode)
            this.prodUnitForm.unitCode = this.prodUnitForm.unitCode;
        if (this.prodUnitForm.unitName)
            this.prodUnitForm.unitName = this.prodUnitForm.unitName;
        if (this.prodUnitForm.conversionUnit)
            this.prodUnitForm.conversionUnit = this.prodUnitForm.conversionUnit;

        var result = await this._webApiService.post("getSerachFilterProdUnitMaster", this.prodUnitForm);
        this.prodUnitMaster = result ?? of([]);
    }
    clearSearchCriteria() {
        this.prodUnitForm.unitCode = "";
        this.prodUnitForm.unitName = "";
        this.prodUnitForm.conversionUnit = "";




    }
    async createOreditprodUnit(data?: any) {
        this._router.navigate(["purchase/create-edit-prod-unit-master"], {
            state: { id: data },
        });
    }
    async prodUnitDelete(item) {

        this._confirmService.confirm({
            message: this._translate.transform("UNIT_DELETE_CONF"),
            key: 'prodUnitDelete',

            accept: async () => {
                item.active = "N";
                var result = await this._webApiService.post("saveProdUnitMaster", item);
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

}
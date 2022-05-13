import { Component, ChangeDetectorRef, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';
import { PurchaseService } from '../purchase.service';

@Component({
    selector: 'app-product-master',
    templateUrl: './product-master.component.html',
    styleUrls: ['./product-master.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],

})

export class ProductMasterComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _purchaseService: PurchaseService,
        private _router: Router,
        private _toastrService: ToastrService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector, 'SCR_PRODUCT_MASTER', 'allowView', _commonService)
    }

    productMasterForm: any;
    productMasterModel: any = [];
    productCategory: any = [];
    productSubCategory: any = [];
    UnitMaster: any = [];
    isLoading: boolean = false;
    ProductMasterInfo: any;
    ListInfo: any = [];
    ProductMasterListInfo: any;
    gridDetailsContextMenu: MenuItem[] = [];
    panelOpenState = false;
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("PRODUCT_MASTER"));
        this.productMasterForm = {
            id: "",
            prodCode: "",
            prodCategoryId: "",
            prodDescription: "",
            barcode: "",
            reOrderLevel: "",
            isExpirable: "",
            defaultUnitId: "",
            isStockable: "",
            prodSubCategoryId: ""

        }

        this.showHideOrgFinyear('SCR_PRODUCT_MASTER');

        this.getSearch();

    }
    async getSearch() {
        await this.loadDefaults();
        this.productMasterModel = [];
        var result = await this._webApiservice.post("getProductsterSerachFilter", this.productMasterForm)
        this.ProductMasterInfo = result as any
        this.ProductMasterInfo.forEach(element => {
            var productcategory = this.productCategory.find(a => a.code == element.prodCategoryId);
            var unitmaster = this.UnitMaster.find(a => a.id == element.defaultUnitId);
            var productSubCategory = this.productSubCategory.find(a => a.id == element.prodSubCategoryId);
            this.productMasterModel.push({
                prodCode: element.prodCode, defaultUnitId: element.defaultUnitId, prodCategoryId: element.prodCategoryId,
                id: element.id, prodDescription: element.prodDescription, barcode: element.barcode, isExpirable: element.isExpirable, isStockable: element.isStockable, reOrderLevel: element.reOrderLevel,
                createdBy: element.createdBy, createdDate: element.createdDate,
                productcategoryname: productcategory ? productcategory.name : "",
                productSubCategoryName: productSubCategory ? productSubCategory.name : "",
                modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate,
                unitmastername: unitmaster ? unitmaster.unitName : "",
            })
        });
    }
    async listExpentation(event) {
        this.ListInfo = [];
        var result = await this._webApiservice.get("getProductMasterById/" + event.data.id);
        this.ProductMasterListInfo = result as any
        this.ListInfo.push({
            barcode: this.ProductMasterListInfo.barcode, reOrderLevel: this.ProductMasterListInfo.reOrderLevel,
            isExpirable: this.ProductMasterListInfo.isExpirable, isStockable: this.ProductMasterListInfo.isStockable, createdBy: this.ProductMasterListInfo.createdBy, createdDate: this.ProductMasterListInfo.createdDate,
            modifiedBy: this.ProductMasterListInfo.modifiedBy, modifiedDate: this.ProductMasterListInfo.modifiedDate,
        })
    }
    async loadDefaults() {
        this.productCategory = await this._purchaseService.getProductCategory(true, false, this._translate, false);
        this.UnitMaster = await this._purchaseService.getProdUnitMaster(true, false, this._translate, false);
        this.productSubCategory = await this._purchaseService.getProductSubCategory(true, false, this._translate, false);
    }
    getGridDetailsContextMenu(item) {
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_PRODUCT_MASTER', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditProductMaster(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_PRODUCT_MASTER', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markProductMasterInactive(item) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }

    async markProductMasterInactive(item) {
        this._confirmService.confirm({
            message: this._translate.transform("PRODUCT_MASTER_DELETE_CONF"),
            accept: async () => {
                item.active = "N";
                var result = await this._webApiservice.post("saveProductMaster", item);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.productMasterModel = [];
                        this.getSearch();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }

    async createOreditProductMaster(data?: any) {
        this._router.navigate(["purchase/create-edit-product-master"], {
            state: { productMasterId: data },
        });
    }
    addProductMaster() {
        this._router.navigate(["purchase/create-edit-product-master"], {
            state: { productMasterId: "" },
        });
    }
    clearSearchCriteria() {
        this.productMasterForm = {
            id: "",
            prodCode: "",
            prodDescription: "",
            barcode: "",
            reOrderLevel: "",
            isExpirable: "",
            isStockable: "",
            defaultUnitId: "",
            prodCategoryId: ""
        }
    }

}
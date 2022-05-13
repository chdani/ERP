import { Component, ChangeDetectorRef, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { finalize } from 'rxjs/operators';
import { NgxSpinnerService } from 'ngx-spinner';
import { of, Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { FormControl } from '@angular/forms';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FinanceService } from 'app/modules/finance/finance.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
@Component({
    selector: 'app-product-category',
    templateUrl: './product-category.component.html',
    styleUrls: ['./product-category.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class ProductCategoryComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _router: Router,
        private _codeMasterService: CodesMasterService,
        private _toastrService: ToastrService,
        private _finService: FinanceService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector, 'SCR_PRODUCT_CATEGORY', 'allowView', _commonService)
    }

    productCategoryForm: any;
    productCategoryModel: any = [];
    ApprovalType: any = [];
    UserType: any = [];
    workFlow: any = [];
    Category: any = [];
    productsubcategory: any = [];
    ApprovalFlow: any;
    department: any = [];
    User: any = [];
    Department: any = [];
    isLoading: boolean = false;
    gridDetailsContextMenu: MenuItem[] = [];
    panelOpenState = false;
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("PRODUCT_CATEGORY"));
        this.productCategoryForm = {
            id: "",
            name: "",
            code: "",
            approvalLevels: "",
            approvalWorkFlow: []
        }
        this.getApprovalLevel();
        this.getSearch();
        this.showHideOrgFinyear('SCR_PRODUCT_CATEGORY');
        this.Category.push({            
            name:this._translate.transform("PRODUCT_CATEGORY_WORKFLOW") 
          });
       
    }

    async getSearch() {
        this.productCategoryModel = await this._webApiservice.post("getProductCategorySerachFilter", this.productCategoryForm)
    }
    async listExpentation(event) {
        this.workFlow = [];
        this.productsubcategory=[];
        var result = await this._webApiservice.get("getProductCategoryById/" + event.data.id)

        result.approvalWorkFlow.forEach(element => {
            var type = this.ApprovalType.find(a => a.code == element.approvalType)
            if (type.code == this._translate.transform("USER"))
                this.User = this.UserType.find(a => a.id == element.approvalId)
            if (type.code == this._translate.transform("DEPARTMENT"))
                this.User = this.department.find(a => a.id == element.approvalId)
            if (type.code == this._translate.transform("DEPARTMENTHEAD"))
                this.User = this.department.find(a => a.id == element.approvalId)
            if (type.code == this._translate.transform("EMPLOYEEMANAGER")) {
                this.User = [];
            }
            this.ApprovalFlow = {};
            this.ApprovalFlow = {
                aprovallevel: element.approvalLevel,
                approvalTypes: type.description,
                approver: this.User.description,
            };
            this.workFlow.push(this.ApprovalFlow)
        });
result.prodsubCategory.forEach(element => {
    this.productsubcategory.push({
        code: element.code,
        name: element.name,
        ProdCategoryId:element.ProdCategoryId,
        Id:element.Id
      })
});
    }
    async getApprovalLevel() {
        var input = {
            active: "Y"
        }
        var result = await this._codeMasterService.getCodesDetailByGroupCode("WORKFLOWAPPROVALTYPE", false, false, this._translate);
        this.ApprovalType = result;
        var results = await this._webApiservice.get("GetDepartmentList");
        if (results) {
            this.department = results as any;
            this.department.forEach(element => {
                element.description = element.name, element.code = element.code, element.id = element.id
            });
        }
        var userResult = await this._webApiservice.post("getUserList", input);
        if (userResult) {
            this.UserType = userResult as any;
            this.UserType.forEach(element => {
                element.description = element.firstName + " " + element.lastName
            });
        }
    }
    getGridDetailsContextMenu(item) {
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_PRODUCT_CATEGORY', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditProductCategory(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_PRODUCT_CATEGORY', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markProductCategoryInactive(item) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }
    async markProductCategoryInactive(item) {
        this._confirmService.confirm({
            message: this._translate.transform("PRODUCT_CATEGORY_DELETE_CONF"),
            accept: async () => {
                item.active = "N";
                var result = await this._webApiservice.post("saveProductCategory", item);
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

    async createOreditProductCategory(data?: any) {
        this._router.navigate(["purchase/create-edit-product-category"], {
            state: { productCategoryId: data },
        });
    }
    addProductCategory() {
        this._router.navigate(["purchase/create-edit-product-category"], {
            state: { productCategoryId: "" },
        });
    }

    clearSearchCriteria() {
        this.productCategoryForm = {
            id: "",
            name: "",
            code: "",
            approvalLevels: "",
        }

    }

}

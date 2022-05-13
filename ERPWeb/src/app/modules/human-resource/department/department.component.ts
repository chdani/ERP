import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { HumanResourceService } from '../human.resource.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
@Component({
    selector: 'app-human-resource',
    templateUrl: './department.component.html',
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class DepartmentComponent extends AppComponentBase implements OnInit {
    selectedProductForm: FormGroup;
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _router: Router,
        private _codeMasterService: CodesMasterService,
        private _humanresourceService: HumanResourceService,
        private _toastrService: ToastrService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector,'SCR_HR_DEPARTMENT','allowView', _commonService )
    }
    gridDetailsContextMenu: MenuItem[] = [];
    selectedRow: any;
    departmentForm: any;
    departmentModel: any = [];
    type: any = [];
    public showOrHideOrgFinyear: boolean = false;
    Parent: any = [];
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("DEPARTMENT_MASTER"));
        this.departmentForm = {
            code: '',
            name: '',
            parentId: '',
            type: ''
        };

        this.getAll();
        this.showHideOrgFinyear('MNU_DEPARTMENT');
    }

    async getAll() {
        await this.getparentdepartment();
        var result = await this._webApiservice.get("GetDepartmentList")
        result.forEach(ele => {
            var parentName = this.Parent.find(a => a.id == ele.parentId);
            var typename = this.type.find(a => a.code == ele.type);
            this.departmentModel.push({
                code: ele.code, name: ele.name, parentId: parentName?.name, typename: typename?.description, id: ele.id
            })
        });
    }

    getGridDetailsContextMenu(item) {
        this.selectedRow = item;
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_HR_DEPARTMENT', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditdepartment(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_HR_DEPARTMENT', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markdepartmentInactive(item.id) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }

    async getparentdepartment() {
        this.Parent = [];
        this.Parent = await this._humanresourceService.parentdepartment();
        this.type = await this._codeMasterService.getCodesDetailByGroupCode("DEPARTMENTTYPES", false, false, this._translate);

    }
    async markdepartmentInactive(id) {
        this._confirmService.confirm({
            message: this._translate.transform("USER_DEPARTMENT_DELETE_CONF"),
            accept: async () => {
                var result = await this._webApiservice.get("markdepartmentInactive/" + id);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        await this._humanresourceService.removeDepartmentListCatche();
                        this.getAll();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }
    async createOreditdepartment(data?: any) {
        var result = await this._webApiservice.get("GetdepartmentByUserId/" + data);
        this._router.navigate(["human-resource/create-edit-department"], {
            state: { departmentId: data },
        });
    }
    addDepartment() {
        this._router.navigate(["human-resource/create-edit-department"], {
            state: { departmentId: "" },
        });
    }
}

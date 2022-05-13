import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { InventoryService } from 'app/modules/admin/budget/budget.service';
import { InventoryProduct, InventoryBrand, InventoryCategory, InventoryTag, InventoryPagination, InventoryVendor } from 'app/modules/admin/budget/budget.types';
import { UserRoleModel } from 'app/shared/model/user-role.model';
import { NgxSpinnerService } from 'ngx-spinner';
import { ConfirmationService, FilterService, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { PrimeNGConfig } from 'primeng/api';
import { TranslatePipe } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { of, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from 'app/shared/component/app-component-base';


@Component({
    selector: 'app-user-role',
    templateUrl: './user-role.component.html',
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [ DialogService,
        ConfirmationService, TranslatePipe,FilterService]
})
export class UserRoleComponent extends AppComponentBase implements OnInit {
    userRoleModel: UserRoleModel[] = [];
    panelOpenState = false;
    filterDto: UserRoleModel = new UserRoleModel();
    gridDetailsContextMenu: MenuItem[] = [];
    selectedRow:any;
    brands: InventoryBrand[];
    categories: InventoryCategory[];
    filteredTags: InventoryTag[];
    flashMessage: 'success' | 'error' | null = null;
    isLoading: boolean = false;
    pagination: InventoryPagination;
    searchInputControl: FormControl = new FormControl();
    selectedProduct: InventoryProduct | null = null;
    selectedProductForm: FormGroup;
    tags: InventoryTag[];
    tagsEditMode: boolean = false;
    vendors: InventoryVendor[];    
    public showOrHideOrgFinyear:boolean=false;


    constructor(
        Injector:Injector,
        private _changeDetectorRef: ChangeDetectorRef,
        private _formBuilder: FormBuilder,
        private _inventoryService: InventoryService,
        private spinner: NgxSpinnerService,
        private _confirmService: ConfirmationService,
        private _dialogService: DialogService,
        private _webApiService: WebApiService,
        private _toastrService: ToastrService,
        private _translate: TranslatePipe,
        private _primengConfig: PrimeNGConfig,
        public router: Router,
        public _commonService: AppCommonService,
    ) {
        super(Injector,'SCR_APP_USER_ROLE','allowView', _commonService )
    }


    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("USER_ROLE_TITLE"));
        this.getAll();
         this.showHideOrgFinyear('MNU_ADM_USER_ROLE');
    }

    getAll() {
        this.userRoleModel = [];
        this.spinner.show();
        this._webApiService.getObserver("getUserRoleList").pipe(finalize(() => { this.panelOpenState = false; this.spinner.hide(); })).subscribe(result => {
            this.userRoleModel = result ?? of([]);
            this._changeDetectorRef.markForCheck();
        });
    }
   
    getGridDetailsContextMenu(item){
        this.selectedRow=item;
        this.gridDetailsContextMenu = [];
        if(this.isGranted('SCR_APP_USER_ROLE',this.actionType.allowEdit)){
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditRole(item.id)}  };
            this.gridDetailsContextMenu.push(edit);
        }
        if(this.isGranted('SCR_APP_USER_ROLE',this.actionType.allowDelete)){
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markRoleInactive(item.id) } };   
        this.gridDetailsContextMenu.push(Delete);
        }
    }
    addRole(data?: any) {
        this.router.navigate(["user-management/create-edit-user-role"], {
            state: { roleId: "" },
        });
    }
    async roleAccessList(event) {
        debugger
        if (!event.data.screenAccessList) {
            var result = await this._webApiService.get("getAppAccessRoleMapByRoleId/" + event.data.id);
            if (result) {
                event.data.screenAccessList = result;
            }
        }
    }
   async createOreditRole(data?: any) {
        var result = await this._webApiService.get("getUserRoleById/" + data);
        this.router.navigate(["user-management/create-edit-user-role"], {
            state: { roleId: data },
        });
    }

    async markRoleInactive(item) {
        debugger
        // Open the confirmation dialog
        this._confirmService.confirm({
            message:this._translate.transform("ROLE_CODE")+":"+item.roleCode+"<br>"+this._translate.transform("USER_ROLE_DELETE_CONF"),
            accept: async () => {
                var result = await this._webApiService.get("markRoleInactive/" + item);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.getAll();
                        // this.dialogRef.close();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }

    trackByFn(index: number, item: any): any {
        return item.id || index;
    }
}
import { ChangeDetectorRef, Component, Injector, OnInit,  ViewChild,  ViewEncapsulation } from '@angular/core';
import { InventoryBrand, InventoryCategory,  InventoryTag, } from 'app/modules/admin/budget/budget.types';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { User, userTypes } from 'app/shared/model/user.model';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize, takeUntil } from 'rxjs/operators';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { PrimeNGConfig } from 'primeng/api';
import { FilterService } from "primeng/api";
import { AppCommonService } from 'app/shared/services/app-common.service';
import { Router } from '@angular/router';
import { trigger,state,style,transition,animate } from '@angular/animations';

@Component({
    selector: 'app-user',
    templateUrl: './user.component.html',
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        DialogService,
        ConfirmationService,
        FilterService
    ],
    animations: [
        trigger('rowExpansionTrigger', [
            state('void', style({
                transform: 'translateX(-10%)',
                opacity: 0
            })),
            state('active', style({
                transform: 'translateX(0)',
                opacity: 1
            })),
            transition('* <=> *', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
        ])
    ]
})
export class UserComponent extends AppComponentBase implements OnInit {

    constructor(
        Injector: Injector,
        private _changeDetectorRef: ChangeDetectorRef,
        private spinner: NgxSpinnerService,
        private _confirmService: ConfirmationService,
        private _dialogService: DialogService,
        private _webApiService: WebApiService,
        private _toastrService: ToastrService,
        private _translate: TranslatePipe,
        private _primengConfig: PrimeNGConfig,
        public _commonService: AppCommonService,
        private _router:Router
    ) {
        super(Injector,'SCR_APP_USER_MASTER','allowView', _commonService )
    }
    panelOpenState = false;
    users: User[];
    userFilter: User = new User();
    userTypes: userTypes[] = [
        { displaytext: 'Windows User', id: 'W' },
        { displaytext: 'General user', id: 'G' },
    ];

    brands: InventoryBrand[];
    categories: InventoryCategory[];
    filteredTags: InventoryTag[];
    flashMessage: 'success' | 'error' | null = null;
    isLoading: boolean = false;

    filteredUsers: any[];   
    selectedUsers: any;
    opsType: string;
    selectType:boolean = true;
    displayModal: boolean = false;
    userRoleList:any = [];
    userForm: any;
    id:any;
    screenAccessList: any = [];
    gridDetailsContextMenu : MenuItem[] =[];
    selectedRow:any;
    expandedRows: any = {};
    public showOrHideOrgFinyear:boolean=false;
    ngOnInit(): void {
        this._primengConfig.ripple = true;
        this._commonService.updatePageName(this._translate.transform("USER_MASTER_TITLE"));
        this.users = [];
        this.userForm = {
            firstName: '',
            lastName: '',
            userName: '',
            emailId: '',
            userType: '',
          };
        this.getAll();
         this.showHideOrgFinyear('MNU_ADM_USER_MASTER');
    }

    async showRoleList(id) {
        this.expandedRows = {};
        var result = await this._webApiService.get("GetUserRoleByUserId/"+id);
        if(result){
            this.userRoleList = result;
            this.displayModal = true;
        }
    }
    async roleAccessList(event) {
        if (!event.data.screenAccessList) {
            var result = await this._webApiService.get("getAppAccessRoleMapByRoleId/" + event.data.userRoleId);
            if (result) {
                event.data.screenAccessList = result;
            }
        }
    }
    getGridDetailsContextMenu(item){
        this.selectedRow=item;
        this.gridDetailsContextMenu = [];
        if(this.isGranted('SCR_APP_USER_MASTER',this.actionType.allowEdit)){
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditUser (item.id)}  };
            this.gridDetailsContextMenu.push(edit);
        }
        if(this.isGranted('SCR_APP_USER_MASTER',this.actionType.allowDelete)){
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markUserInactive(item.id) } };   
        this.gridDetailsContextMenu.push(Delete);
        }
        let Details: MenuItem = { label: this._translate.transform("APP_DETAILS"),icon:'pi pi-book', command: (event) => { this.showRoleList(item.id) } };  
        this.gridDetailsContextMenu.push(Details); 
        if(item.userType !="W"){
            let ResetPassword: MenuItem = { label: this._translate.transform("RESET_PASSWORD"), icon: 'pi pi-envelope', command: (event) => { this.resetmailsending (item.id)}  }; 
            this.gridDetailsContextMenu.push(ResetPassword);       
        }
       
       
      }
    async resetmailsending(id){
        var emailReset = await this._webApiService.get("sendPasswordResetMail/"+ id);
        if (emailReset.status != "FAILED")
            this._toastrService.success(this._translate.transform("APP_EMAIL_SUCCESS_MSG"));  
      }
   
    getAll() {
        this.spinner.show();
        this.panelOpenState = false;
        this._webApiService.postObserver('getUserList',this.userForm).pipe(finalize(() => { this.isLoading = false, this.spinner.hide(); this.panelOpenState = false; })).subscribe(result => {
            this._changeDetectorRef.markForCheck();
            this.users = result;
        })
    }
    
    addUser() {        
        this.router.navigate(["user-management/create-edit-user"], {
            state: { userId: "" },
        });
    }
   

   async createOreditUser(data?: any) {
        var result = await this._webApiService.get("getUserMasterById/" + data);
        this.router.navigate(["user-management/create-edit-user"], {
            state: { userId: data },
        });
    }
    
    async markUserInactive(id) {
        // Open the confirmation dialog
        this._confirmService.confirm({
            message: this._translate.transform("USER_MASTER_DELETE_CONF"),
            accept:async () => {
                var result = await this._webApiService.get("markUserInactive/"+id);
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

    async searchUserList(){

    }

    clearSearchCriteria()
    {
        this.userForm = {
            firstName: '',
            lastName: '',
            userName: '',
            emailId: '',
            userType: '',
        };
    }

    trackByFn(index: number, item: any): any {
        return item.id || index;
    }
}
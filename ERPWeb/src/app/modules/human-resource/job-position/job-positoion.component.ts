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

@Component({
    selector: 'app-job-position',
    templateUrl: './job-position.component.html',
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
    ],
})
export class JobPositionComponent extends AppComponentBase implements OnInit {
    searchInputControl: FormControl = new FormControl();
    selectedProductForm: FormGroup;
    constructor(
        injector:Injector, 
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _formBuilder: FormBuilder,
        private _translate: TranslatePipe,
        private _router: Router,
        private _webApiService: WebApiService,
        public _commonService: AppCommonService,
        private spinner: NgxSpinnerService,
        private _changeDetectorRef: ChangeDetectorRef,
        private _confirmService: ConfirmationService,
        private _toastrService: ToastrService,
        
       )
        {
            super(injector,'SCR_HR_JOBPOSITION','allowView', _commonService )
    }
        gridDetailsContextMenu: MenuItem [] = [];
        selectedRow: any;
        panelOpenState = false;
        JobPosition: [];
        JobForm: any;
        public showOrHideOrgFinyear:boolean=false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("HR-JOBPOSITION"));
        this.JobPosition = [];
        this.JobForm = {
            code: '',
            name: '',
            actives:'',
          };
          this.getAll();
          this.showHideOrgFinyear('MNU_JOBPOSITION');
    }

    addUser() {
        this._router.navigate(["human-resource/create-edit-job-position"], {
            state: { id: "" },
            
        });
    }

    getAll(){
        this.JobPosition = [];
        this.spinner.show();
        this._webApiService.getObserver("getalljobposition").pipe(finalize(() => { this.panelOpenState = false; this.spinner.hide(); })).subscribe(result => {
           
            this.JobPosition = result ?? of([]);
            this._changeDetectorRef.markForCheck();
        });
    }
     
    getGridDetailsContextMenu(item){
        this.selectedRow=item;
        this.gridDetailsContextMenu = [];
        if(this.isGranted('SCR_HR_JOBPOSITION',this.actionType.allowEdit)){
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditjob(item.id)}  };
            this.gridDetailsContextMenu.push(edit);
        }
        if(item.actives==true)
        {
        if(this.isGranted('SCR_HR_JOBPOSITION',this.actionType.allowEdit)){
            let inactive: MenuItem = { label: this._translate.transform("APP_INACTIVE"), icon: 'pi pi-times', command: (event) => { this.jobpositionActiveorInactive(item)}  };
            this.gridDetailsContextMenu.push(inactive);
        }
        }
        if(item.actives==false)
        {
        if(this.isGranted('SCR_HR_JOBPOSITION',this.actionType.allowEdit)){
            let active: MenuItem = { label: this._translate.transform("APP_ACTIVE"),  icon: 'pi pi-check', command: (event) => { this.jobpositionActiveorInactive(item)}  };
            this.gridDetailsContextMenu.push(active);
        }
        }
        if(this.isGranted('SCR_HR_JOBPOSITION',this.actionType.allowDelete)){
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.jobpositionDelete(item.id) } };   
        this.gridDetailsContextMenu.push(Delete);
        }
    }

   async createOreditjob(data?: any){
        var result = await this._webApiService.get("getJobPositionById/" + data);
        this._router.navigate(["human-resource/create-edit-job-position"], {
            state: { id:data },
        });
    }
    async jobpositionDelete(id){  
            // Open the confirmation dialog
            this._confirmService.confirm({
                message: this._translate.transform("JOb_DELETE_CONF"),
                key: 'jobpositionDelete',
                accept: async () => {
                    var result = await this._webApiService.get("jobpositionDelete/" + id);
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
    async jobpositionActiveorInactive(item){        
        if (item.actives == true) {            
            this._confirmService.confirm({
            message: this._translate.transform("JOb_DEACTIVE_CONF"),
            key: 'jobpositionActiveorInactive',
            accept: async () => {
            this.jobpositionActiveorInactivefunction(item);
        }
        });
        }
        else{
            this._confirmService.confirm({
            message: this._translate.transform("JOb_ACTIVE_CONF"),
            key: 'jobpositionActiveorInactive',
            accept: async () => {
            this.jobpositionActiveorInactivefunction(item);
        }
        }); 
        }
    }
    async jobpositionActiveorInactivefunction(item){
        var result = await this._webApiService.get("jobpositionActiveorInactive/" + item.id);
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
}

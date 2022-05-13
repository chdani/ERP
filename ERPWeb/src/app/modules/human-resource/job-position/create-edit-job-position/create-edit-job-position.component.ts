
import { ThrowStmt } from '@angular/compiler';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { result } from 'lodash';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { UserManagementService } from 'app/modules/user-management/user-management.service';
import { HumanResourceService } from '../../human.resource.service';
import { Router } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';

@Component({
  selector: 'app-job-position',
  templateUrl: './create-edit-job-position.component.html',
  providers: [
    TranslatePipe
  ]
})
export class CreatEditjobpositionComponent extends AppComponentBase implements OnInit {

  constructor(private formBuilder: FormBuilder,
    injector:Injector,
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _userManagementService: UserManagementService,
    private _humanresourceService: HumanResourceService,
    private _primengConfig: PrimeNGConfig,
    private _router: Router
  ) {  super(injector,'SCR_HR_JOBPOSITION','allowAdd', _commonService )}

  formFieldHelpers: string[] = [''];
  registerForm: FormGroup;
  submitted = false;
  roleList: any = [];
  filteredRoles: any = [];
  selectedRole: any;
  userRoleList: any = [];
  userTypeList: any = {};  
  checked: boolean = false;
  joblist:any;
  id: any;
  jobInfo: any = {
    id:"",
    Code: "",
    Name: "",
    createdby: "",
    createdate:"",
    modifiedby: "",
    modifieddate:""
  }

  ngOnInit(): void {    
      if (history.state && history.state.id && history.state.id != '') {
        this.id = history.state.id;
      }
      if (this.id && this.id !== "")
        this._commonService.updatePageName(this._translate.transform("JOB_POSITION_EDIT"));
      else
        this._commonService.updatePageName(this._translate.transform("JOB_POSITION_ADD"));
  
        this.registerForm = this.formBuilder.group({
          id: [''],
          code: ['', Validators.required],
          name: ['', Validators.required]
        });
      if (this.id && this.id !== "") {
        this.spinner.show();
        this._webApiService.getObserver("getJobPositionById/" + this.id).pipe(finalize(() => { this.spinner.hide(); })).subscribe(result => { 
          
          this.joblist = result as any    
          this.registerForm.setValue({          
            id: this.id,
            code: this.joblist.code,
            name: this.joblist.name,          
          });
        }, error => { });
      }
      
  }
  
  async onSubmit(){
    if( this.registerForm.value.code == "" || this.registerForm.value.name == ""){
      this._toastrService.error("Fill the mandatory items!..");
      
    }
    else{
      this.submitted = true;
      this.jobInfo.id = this.id !== "" ? this.id : "";
      this.jobInfo.code = this.registerForm.value.code;
      this.jobInfo.name = this.registerForm.value.name;
      if(this.joblist,this.id && this.joblist.id != "")
      {
        this.jobInfo.createdBy = this.joblist.createdBy;
        this.jobInfo.createdDate = this.joblist.createdDate;
        this.jobInfo.modifiedBy = this.joblist.modifiedBy;
        this.jobInfo.modifiedDate = this.joblist.modifiedDate;
      }
     
      var result = await this._webApiService.post("saveJobPosition", this.jobInfo);
      if (result) {
        var output = result as any;
        if (output.status == "DATASAVESUCSS") {
          await this._humanresourceService.removeDepartmentListCatche()
          this._toastrService.success(this._translate.transform("APP_SUCCESS"));
          this._router.navigateByUrl("human-resource/job-position");
        }
        else {
          this._toastrService.error(output.messages[0])
        }
      }
    }
    }

  cancelAddEdit(){
    this._router.navigateByUrl("human-resource/job-position");
  }
}
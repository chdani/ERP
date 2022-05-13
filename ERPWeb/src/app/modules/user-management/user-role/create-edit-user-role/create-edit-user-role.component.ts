import { Component, OnInit, Injector } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { PrimeNGConfig } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { UserManagementService } from 'app/modules/user-management/user-management.service';

@Component({
  selector: 'app-create-edit-user-role',
  templateUrl: './create-edit-user-role.component.html',
  styleUrls: ['./create-edit-user-role.component.scss'],
  providers: [TranslatePipe]
})
export class CreateEditUserRoleComponent extends AppComponentBase implements OnInit {
  
  createOrEditFrom: FormGroup;
  submitted = false;
  screensList: any = [];
  filteredScreens: any = [];
  selectedScreen: any;
  addChck: any;
  editChck: any;
  deleteChck: any;
  approveChck:any; 
  viewChck:any;
  items :any;
  view :any;
  item :any;

  constructor(
    injector: Injector,
    private formBuilder: FormBuilder,  
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _primengConfig: PrimeNGConfig,
    private _userManagementService: UserManagementService,
   
    
  ) {
    super(injector,'SCR_APP_USER_ROLE','allowAdd', _commonService );
    this._primengConfig.ripple = true;
   }

  id: any;
  userScreenAccess: any = [];
  userRole: any = {
    id: "",
    ruleCode: "",
    ruleName: "",
    active: "",
    userScreenAccess: []
  }

  ngOnInit(): void {
    if (history.state && history.state.roleId && history.state.roleId != '') {
      this.id = history.state.roleId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("USER_ROLE_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("USER_ROLE_ADD"));

    this.createOrEditFrom = this.formBuilder.group({
      id: [''],
      roleCode: ['', Validators.required],
      roleName: ['', Validators.required],
    });

    this.getAppAccessList();
  }
  get f() { return this.createOrEditFrom.controls; }

  async getAppAccessList() {
    var result = await this._userManagementService.getAppScreen();
    this.screensList = result;
    await this.loadExistingValues();
  }

  filterScreens(event) {
    let query = event.query.toUpperCase();
    this.filteredScreens = this.screensList.filter(a => a.name.toUpperCase().includes(query));
  }

  async loadExistingValues()
  {
    if (this.id && this.id !== "") {
      this.spinner.show();
      this._webApiService.getObserver("getUserRoleById/" + this.id).pipe(finalize(() => { this.spinner.hide(); })).subscribe(result => {

        var screenAccessList = result.userScreenAccess;
        this.screensList.forEach(element => {
          var screen = screenAccessList.find(a => a.id == element.code)
          if (screen)
          {
            element.add = screen.add;
            element.edit = screen.edit;
            element.delete = screen.delete;
            element.approve = screen.approve;
            element.view = screen.view;
          }
        });
        
        this.createOrEditFrom.setValue({
          id: this.id,
          roleCode: result.roleCode,
          roleName: result.roleName,
        });
      }, error => { });
    }
  }

  Add(item) {
    let validation: Boolean = false;
    let items = item;
   
     if(items.add == true || items.edit ==true || items.delete ==true || items.approve == true){
        items.view = true;
     }else{
        items.view= false;
     } 
     this.item.push(items)
  }

  views(item){
    let validation: Boolean = false;
    let items = item;
    if(item.view == false){
      item.add = false;
      item.edit = false;
      item.delete = false;
      item.approve = false;
    }
  }


  async onSubmit() {
    debugger
    this.submitted = true;
    if (this.createOrEditFrom.invalid) {
      return;
    }
    var screenAccessList=[];
    this.screensList = this.screensList.filter(
      book => book.view === true);
      for(let i=0; i<this.screensList.length;i++){
        screenAccessList.push({
          id: this.screensList[i].code,
          name: this.screensList[i].name,
          add: this.screensList[i].add,
          edit: this.screensList[i].edit,
          delete: this.screensList[i].delete,
          view: this.screensList[i].view,
          approve:this.screensList[i].approve
        });
      }

    this.userRole.id = this.id !== "" ? this.id : "";
    this.userRole.active = this.id !== "" ? this.id : "";
    this.userRole.roleCode = this.createOrEditFrom.value.roleCode;
    this.userRole.roleName = this.createOrEditFrom.value.roleName;
    this.userRole.userScreenAccess = screenAccessList;
    var result = await this._webApiService.post("saveUserRole", this.userRole);

    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        debugger;
        await this._userManagementService.removeUserRoleCatche();
        this.router.navigateByUrl("user-management/user-role");
      }
      else
        this._toastrService.error(output.messages[0]);
        this.getAppAccessList();
    }
  }
  cancelAddEdit() {
    this.router.navigateByUrl("user-management/user-role");
  }

  public errorHandling = (controlName: string, errorName: string) => {
    return this.createOrEditFrom.controls[controlName].hasError(errorName);
  }
}
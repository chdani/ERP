
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
import { PickListModule } from 'primeng/picklist';



import { Router } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';

@Component({
  selector: 'app-creat-edit-user',
  templateUrl: './creat-edit-user.component.html',
  styleUrls: ['./creat-edit-user.component.scss'],
  providers: [
    TranslatePipe
  ]
})
export class CreatEditUserComponent extends AppComponentBase implements OnInit {

  constructor(
    Injector:Injector,
    private formBuilder: FormBuilder,
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _userManagementService: UserManagementService,
    private _primengConfig: PrimeNGConfig,
    private _router: Router
  ) { super(Injector,'SCR_APP_USER_MASTER','allowAdd', _commonService ) }
  userTypes: userTypes[] = [
    { name: 'Windows User', code: 'W' },
    { name: 'General user', code: 'G' }
  ];

  formFieldHelpers: string[] = [''];
  registerForm: FormGroup;
  submitted = false;
  roleList: any = [];
  organizationLists: AppListItem[];
  orgTypeList: any = [];
  filteredRoles: any = [];
  selectedRole: any;
  userTypeList: any = {};
  selectedEmployee: any;
  checked: boolean = false;
  ledgerAccountData: any = [];
  ledgerAccountFilterData: any = [];

  dataaccount = [];
  ledgerAccountSelectedData: any = [];
  ledgerAccountListData: any = [];
  ledgerAccountUnSelectData: any = [];
  organizationTypes: any = [];
  employee: any = [];
  selectedUser: any;
  selectedEmail: any;
  email: boolean = false;
  disabledEmployee:boolean = false;

  organizationSelect: AppListItem[];
  organizationFilter: any = [];
  selectedOrganizations: AppListItem[];
  id: any;
  userInfo: any;

  // ledgerAccountInfo:any = {

  // }

  ngOnInit(): void {

    this.userInfo = {
      id: "",
      firstName: "",
      lastName: "",
      userName: "",
      emailId: "",
      userType: "",
      employeeId: "",
      active: "",
      organizations: [],
      userRole: [],
      ledgerAccnts: []
    }
    if (history.state && history.state.userId && history.state.userId != '') {
      this.id = history.state.userId;
    }

    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("USER_MASTER_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("USER_MASTER_ADD"));

    this.roleList = [];
    this.loadDefaults();


  }

  async loadDefaults() {
    await this.getLedgerAccountList();
    await this.getOrganizationtypes();
    await this.getUserRole();
    await this.getEmployee();
    if (this.id && this.id !== "") {
      this.getUserData();
    }
  }
  get f() { return this.registerForm.controls; }

  async getEmployee() {
    this.employee = await this._webApiService.get("getEmployeeList");

  }
  async getLedgerAccountList() {
    var input = {
      ledgerCodeFrom: "",
      ledgerCodeTo: ""
    }
    this.ledgerAccountData = await this._webApiService.post("getLedgerAccountList", input);
    this.ledgerAccountData.forEach(element => {
      this.ledgerAccountFilterData.push({
        code: element.ledgerCode,
        name: element.ledgerDesc
      })

    });
    this.dataaccount = this.ledgerAccountFilterData;
  }
  async getOrganizationtypes() {
    this.organizationLists = [];
    this.organizationTypes = await this._webApiService.get("getOrganizations");
    this.organizationTypes.forEach(element => {
      this.organizationLists.push({
        id: element.id, name: element.orgName
      });
    });

  }
  async getUserRole() {
    let result = await this._userManagementService.getUserRole();
    this.roleList = result;
  }

  async getUserData() {

    let result = await this._webApiService.get('getUserMasterById/' + this.id);


    this.selectedEmployee = this.employee.find(a => a.id == result.employeeId);
    if(this.selectedEmployee != null){
            this.disabledEmployee = true;
    }
    else{
      this.disabledEmployee = false;
    }
    let type = this.userTypes.filter(a => a.code == result.userType)[0];
    if (type)
      this.userTypeList = { name: type.name, code: type.code };
    this.selectedUser = type.code;
    this.selectedEmail = result.emailId;
    if (result) {
      result.userRoleMap.forEach(element => {
        var role = this.roleList.find(a => a.code == element.userRoleId)
        if (role)
          role.active = true;
      });
      // let output=this.roleList.map((item,i)=> Object.assign({},item, this.userRoleList[i]));
      // this.roleList=[];    
      // this.roleList=output;
      // this.roleList=output;
      result.ledgerAccnts.forEach(element => {
        var acc = this.dataaccount.find(a => a.code == element.accountCode)
        this.ledgerAccountListData.push({
          code: acc.code,
          name: acc.name

        });
      });
      this.ledgerAccountSelectedData = this.ledgerAccountListData;
      this.ledgerAccountSelectedData.forEach(element => {
        this.dataaccount = this.dataaccount.filter(a => a.code !== element.code)
      })

      result.organizations.forEach(element => {
        var orgname = this.organizationLists.find(a => a.id == element.organizationId)
        this.organizationFilter.push({
          id: element.organizationId, name: orgname.name,
        });
      });
      if (result) {
        this.userInfo = {
          firstName: result.firstName,
          lastName: result.lastName,
          userName: result.userName,
          organizations: this.organizationFilter,
        }

        // this.organizationSelect.push({
        //   id:element.id,name:element.orgName 
        // })



      }
    }

  }

  cancelAddEdit() {
    this._router.navigateByUrl("user-management/user");
  }

  filterRole(event) {
    let query = event.query.toUpperCase();
    this.filteredRoles = this.roleList.filter(a => a.name.toUpperCase().includes(query));
  }
  async onselectemployye() {
    this.selectedEmail = this.selectedEmployee.email;
    this.email = true

  }
  async onSubmit() {
    this.submitted = true;

    var userRoleList = [];
    this.roleList = this.roleList.filter(x => x.active == true);
    this.roleList.forEach(element => {
      userRoleList.push({
        code: element.code, name: element.name, active: element.active
      });
    });

    //Set Values
    this.userInfo.id = this.id !== "" ? this.id : "";
    this.userInfo.employeeId = this.selectedEmployee?.id;
    this.userInfo.userType = this.selectedUser;
    this.userInfo.emailId = this.selectedEmail;


    this.ledgerAccountListData = [];
    this.ledgerAccountSelectedData.forEach(element => {
      this.ledgerAccountListData.push({
        accountCode: element.code, Active: "Y"
      });
    });
    this.userInfo.ledgerAccnts = this.ledgerAccountListData;

    // this.userInfo.userRole = this.registerForm.value.userRole; //.code;
    this.userInfo.userRole = userRoleList; //.code;
    console.log(this.registerForm);
    // this.registerForm.disable();
    //var result2 = await this._webApiService.post("saveUserLedgerAccount", this.userInfo);
    var result = await this._webApiService.post("saveUserInfo", this.userInfo);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        if (result.referenceId && !this.userInfo.id && this.userInfo.userType != "W") {
          var emailRes = await this._webApiService.get("sendPasswordResetMail/" + result.referenceId);
          if (emailRes.status != "FAILED") {
            this._toastrService.success(this._translate.transform("APP_EMAIL_SUCCESS_MSG"));
          }
          else {
            console.log(emailRes.messages[0]);
            this._toastrService.error(this._translate.transform("APP_EMAIL_FAILED_MSG"));
          }
          this._router.navigateByUrl("user-management/user");
        }
        else {
          this._router.navigateByUrl("user-management/user");
        }
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }
  }

  public errorHandling = (controlName: string, errorName: string) => {
    return this.registerForm.controls[controlName].hasError(errorName);
  }


}

export function MustMatch(controlName: string, matchingControlName: string) {
  return (formGroup: FormGroup) => {
    const control = formGroup.controls[controlName];
    const matchingControl = formGroup.controls[matchingControlName];

    if (matchingControl.errors && !matchingControl.errors.mustMatch) {
      // return if another validator has already found an error on the matchingControl
      return;
    }

    // set error on matchingControl if validation fails
    if (control.value !== matchingControl.value) {
      matchingControl.setErrors({ mustMatch: true });
    } else {
      matchingControl.setErrors(null);
    }
  }


}
interface userTypes {
  name: string;
  code: string;
}

interface AppListItem {
  name: string;
  id: string;
}
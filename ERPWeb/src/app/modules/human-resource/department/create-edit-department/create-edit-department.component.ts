import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { FormControl } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { HumanResourceService } from '../../human.resource.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
@Component({
  selector: 'app-department',
  templateUrl: './create-edit-department.component.html',
  styles: [],
  encapsulation: ViewEncapsulation.None,
  providers: [
    TranslatePipe,
  ],
})
export class CreateEditDepartmentComponent extends AppComponentBase implements OnInit {
  constructor(
    injector:Injector,
    private formBuilder: FormBuilder,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _formBuilder: FormBuilder,
    private _toastrService: ToastrService,
    private _humanresourceService: HumanResourceService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _codeMasterService: CodesMasterService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
  ) {super(injector,'SCR_HR_DEPARTMENT','allowAdd', _commonService ) }
  id: any;
  users: [];
  selectedParent: any;
  SelectedType: any;
  parentId: any = [];
  departmenttype: any = [];
  DpartmentList: any = [];
  dept1: any = [];
  userForm: any;
  department: any;
  DeptInfo: any;
  departmentForm: FormGroup;
  submitted = false;


  ngOnInit(): void {
    this.department = {
      id: "",
      code: "",
      name: "",
      parentId: "",
      type: "",
    }
    if (history.state && history.state.departmentId && history.state.departmentId != '') {
      this.id = history.state.departmentId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("DEPARTMENT_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("DEPARTMENT_ADD"));

    this.departmentDetails();

    if (this.id && this.id !== "")
      this.getall();

  }
  async getall() {
    var result = await this._webApiservice.get("GetdepartmentByUserId/" + this.id);
    this.DeptInfo = result as any;
    // let filterparent = this.parentId.filter(i => i.id !== this.id);
    // this.parentId = filterparent;
    let type = this.parentId.filter(a => a.id == this.DeptInfo.parentId)[0];
    let depttype = this.departmenttype.find(a => a.code == this.DeptInfo.type);
    if (type) {
      this.selectedParent = type.id;
    }
    this.SelectedType = depttype?.code;
    this.department.code = this.DeptInfo.code;
    this.department.name = this.DeptInfo.name;
    if (!!type)
      this.DpartmentList = { name: type.name, id: type.id };
    else
      this.DpartmentList = this.parentId;
    this.codeChange();
  }
  async codeChange() {
    var result = await this._webApiservice.get("GetDepartmentList")
    this.dept1 = result as any;
    this.dept1.forEach(ele => {
      var name = this.parentId.find(a => a.id == ele.parentId)
      ele.parentId = name?.name;
    });
    var duplicateCheck = this.dept1.filter(a => a.parentId == this.department.name);
    this.parentId = this.parentId.filter(a => a.name !== this.department.name);
    duplicateCheck.forEach(e => {
      this.parentId = this.parentId.filter(a => a.name !== e.parentId)
    });
    duplicateCheck.forEach(e => {
      if (e.name != null) {
        this.duplicate(e.name)
      }
    });
  }
  async duplicate(name) {
    var duplicateCheck1 = this.dept1.filter(a => a.parentId == name);
    this.parentId = this.parentId.filter(a => a.name !== name);
    duplicateCheck1.forEach(el => {
      this.parentId = this.parentId.filter(a => a.name !== el.name)
    });
    duplicateCheck1.forEach(e => {
      if (e.name != null) {
        this.duplicate(e.name)
      }
    });

  }
  async departmentDetails() {
    this.parentId = await this._humanresourceService.parentdepartment();
    this.departmenttype = await this._codeMasterService.getCodesDetailByGroupCode("DEPARTMENTTYPES", false, false, this._translate);

  }
  cancelAddEdit() {
    this._router.navigateByUrl("human-resource/jdepartment");
  }

  async onSubmit() {
    this.submitted = true;
    if (this.department.code == "") {
      this._toastrService.error(this._translate.transform("DEPARTMENT_CODE_REQ"));
      return;
    }
    if (this.department.name == "") {
      this._toastrService.error(this._translate.transform("DEPARTMENT_NAME_REQ"));
      return;
    }
    this.department.id = this.id !== "" ? this.id : "";
    this.department.parentId = this.selectedParent;
    this.department.type = this.SelectedType;
    this.department.active = "Y";
    if (this.department.id && this.department.id != "") {
      this.department.createdBy = this.DeptInfo.createdBy;
      this.department.createdDate = this.DeptInfo.createdDate;
      this.department.modifiedBy = this.DeptInfo.modifiedBy;
      this.department.modifiedDate = this.DeptInfo.modifiedDate;
    }
    await this._humanresourceService.removeDepartmentListCatche();

    var result = await this._webApiservice.post("SaveHrDepartment", this.department);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this._router.navigateByUrl("human-resource/jdepartment");
      }
      else
        this._toastrService.error(output.messages[0])
    }
  }
}


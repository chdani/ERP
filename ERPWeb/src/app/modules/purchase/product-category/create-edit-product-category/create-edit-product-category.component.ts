import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroupDirective, FormGroup, FormControl, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertType } from '@fuse/components/alert';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { MessageService } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { MatStepperModule } from '@angular/material/stepper';
import { ErrorStateMatcher } from '@angular/material/core';
import { ValidatorFn, AbstractControl } from '@angular/forms';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';


/**
 * @title Stepper overview
 */
@Component({
  selector: 'app-product-category',
  templateUrl: './create-edit-product-category.component.html',
  styleUrls: ['./create-edit-product-category.component.scss'],
  providers: [MatStepperModule]


})
export class createeditProductCategoryComponend extends AppComponentBase implements OnInit {
  isLinear = true;
  submitted = false;
  productCategorylist: any;
  validation: boolean = true;
  ApprovalType: any = [];
  UserType: any = [];
  Approvals: any = [];
  department: any = [];
  User: any = [];
  Department: any = [];
  approvalWorkFlows: any = [];
  approval: any;
  basicInfo: any;
  productCategory: any;
  selectedUploadData: any = {};
  ApprovalFlow: any = {
    approvalTypes: "",
    userType: "",
    department: ""

  };
  id: any;
   
  constructor(
    injector: Injector,
    private formBuilder: FormBuilder,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _formBuilder: FormBuilder,
    private _codeMasterService: CodesMasterService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,

  ) { super(injector, 'SCR_PRODUCT_CATEGORY', 'allowAdd', _commonService) }

  ngOnInit() {
    this.productCategory = {
      id: "",
      name: "",
      code: "",
      approvalWorkFlow: [],
      prodsubCategory: [],
    }
    if (history.state && history.state.productCategoryId && history.state.productCategoryId != '') {
      this.id = history.state.productCategoryId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("PRODUCT_CATEGORY_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("PRODUCT_CATEGORY_ADD"));




    this.getAlldata();

  }
  async getAlldata() {
    await this.getApprovalLevel();
    if (this.id && this.id !== "") {
      var result = await this._webApiservice.get("getProductCategoryById/" + this.id);
      this.productCategorylist = result as any
      this.productCategory.code = this.productCategorylist.code;
      this.productCategory.name = this.productCategorylist.name;
      this.productCategory.id = this.productCategorylist.id;
      this.productCategory.prodsubCategory=this.productCategorylist.prodsubCategory;
      this.productCategorylist.approvalWorkFlow.forEach(element => {
        var type = this.ApprovalType.find(a => a.code == element.approvalType)
        if (type.code == this._translate.transform("USER"))
          this.User = this.UserType.find(a => a.id == element.approvalId)
        if (type.code == (this._translate.transform("DEPARTMENT")))
          this.Department = this.department.find(a => a.id == element.approvalId)
        if (type.code == (this._translate.transform("DEPARTMENTHEAD")))
          this.Department = this.department.find(a => a.id == element.approvalId)
        if (type.code == this._translate.transform("EMPLOYEEMANAGER")) {

        }
        this.ApprovalFlow = {};
        this.ApprovalFlow = {
          approvallevel: element.approvalLevel,
          approvalTypes: type,
          User: this.User,
          Department: this.Department
        };
        this.Approvals.push(this.ApprovalFlow)
      });
     
      this.hideShowMoveButtons();

    }
  }

  addNewWorkFlow() {
    var newwork =
    {
      approvalTypes: "",
      userType: "",
      department: "",
      Department: "",
      User: ""

    }
    this.Approvals.push(newwork);
    this.hideShowMoveButtons();
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

  async clickapprovalType(item) {
    this.selectedUploadData == item;
    var type = item.approvalTypes.code;
    if (type == this._translate.transform("USER")) {
      this.User = this.UserType;
    }
    else if (type == (this._translate.transform("DEPARTMENT") || this._translate.transform("DEPARTMENTHEAD"))) {
      this.Department = this.department;
    }
    else if (type == this._translate.transform("EMPLOYEEMANAGER")) {
    }
  }

  moveUp(value, index) {
    if (index > 0) {
      const tmp = this.Approvals[index - 1];
      this.Approvals[index - 1] = this.Approvals[index];
      this.Approvals[index] = tmp;
    }
    this.hideShowMoveButtons();
  }

  moveDown(value, index) {
    if (index < this.Approvals.length - 1) {
      const tmp = this.Approvals[index + 1];
      this.Approvals[index + 1] = this.Approvals[index];
      this.Approvals[index] = tmp;
    }
    this.hideShowMoveButtons();
  }

  hideShowMoveButtons() {
    for (var index = 0; index < this.Approvals.length; index++) {
      if (this.Approvals.length == 1) {
        this.Approvals[index].moveDown = false
        this.Approvals[index].moveUp = false;
      }
      else if (index == 0) {
        this.Approvals[index].moveUp = false;
        this.Approvals[index].moveDown = true;
      }
      else if (index == this.Approvals.length - 1) {
        this.Approvals[index].moveDown = false
        this.Approvals[index].moveUp = true;
      }
      else {
        this.Approvals[index].moveUp = true;
        this.Approvals[index].moveDown = true;
      }
    }
  }
  async addNewProdSubCategoryDetReq() {
    var newSubCategoryDetReq =
    {
      id:"",
      code: "",
      name: "",
      prodCategoryId:"",
      active:"Y",
      createdBy :"",
      createdDate : "",
      modifiedBy :"",
      modifiedDate : ""
    }

  
    this.productCategory.prodsubCategory.unshift(newSubCategoryDetReq);
   
   
  }
  async deleteProdSubCategoryDetReq(item) {
   
    var index = this.productCategory.prodsubCategory.indexOf(item);
    if (index >= 0){     
      this.productCategory.prodsubCategory.forEach((elem, index) => {
        if (elem.id == item.id) 
          elem.active = 'N'
      });
    }
  }
  removeWorkFlow(workflow) {
    this.Approvals.forEach((item, index) => {
      if (item === workflow) this.Approvals.splice(index, 1);
    });
    this.hideShowMoveButtons();
  }

  ProductCategoryCancel() {
    this._router.navigateByUrl("purchase/product-category");
  }
  async ProductCategorysave() {

    this.validation = true;
    if (this.productCategory.code == "") {
      this._toastrService.error(this._translate.transform("PRODUCT_CODE_REQ"));
      return;
    }
    if (this.productCategory.name == "") {
      this._toastrService.error(this._translate.transform("PRODUCT_NAME_REQ"));
      return;
    }
    if (this.Approvals.length == 0) {
      this.validation = false;
    } else {
      this.Approvals.forEach(element => {
        if (element.approvalTypes.code == (this._translate.transform("DEPARTMENT") || this._translate.transform("DEPARTMENTHEAD")) && element.Department == "" ||
          element.approvalTypes.code == this._translate.transform("USER") && element.User == "") {
          this.validation = false;
        } else if (element.approvalTypes == "") {
          this.validation = false;
        }
      });
    }
    if (this.validation) {

      this.submitted = true;
      this.productCategory.id = this.id !== "" ? this.id : "";

      this.productCategory.approvalLevels = this.Approvals.length;
      this.productCategory.active = "Y";

      var level = 1;
      this.approvalWorkFlows = [];
      this.Approvals.forEach(element => {
        if ((element.Department == "" && !element.Department.id) || (element.User == "" && !element.User.id))
          element.userType = {
            id: "",
          };
        if (element.approvalTypes.code == this._translate.transform("DEPARTMENT")) {
          this.approvalWorkFlows.push({
            approvalLevel: level, approvalType: element.approvalTypes.code, approvalId: element.Department.id
          })
        }
        else if (element.approvalTypes.code == this._translate.transform("DEPARTMENTHEAD")) {
          this.approvalWorkFlows.push({
            approvalLevel: level, approvalType: element.approvalTypes.code, approvalId: element.Department.id
          })
        }
        else if (element.approvalTypes.code == this._translate.transform("USER")) {
          this.approvalWorkFlows.push({
            approvalLevel: level, approvalType: element.approvalTypes.code, approvalId: element.User.id
          })
        }
        else if (element.approvalTypes.code == this._translate.transform("EMPLOYEEMANAGER")) {
          this.approvalWorkFlows.push({
            approvalLevel: level, approvalType: element.approvalTypes.code, approvalId: 0
          })
        }
        level++;
      });
      this.productCategory.approvalWorkFlow = this.approvalWorkFlows;

      const status = this.approvalWorkFlows.some(user => {
        let counter = 0;
        for (const iterator of this.approvalWorkFlows) {
          if (iterator.approvalType === user.approvalType && iterator.approvalId === user.approvalId) {
            counter += 1;
          }
        }
        return counter > 1;
      });
      if (!status) {
        if (this.productCategory.id && this.productCategory.id != "") {
          this.productCategory.createdBy = this.productCategorylist.createdBy;
          this.productCategory.createdDate = this.productCategorylist.createdDate;
          this.productCategory.modifiedBy = this.productCategorylist.modifiedBy;
          this.productCategory.modifiedDate = this.productCategorylist.modifiedDate;
        }
        var result = await this._webApiservice.post("saveProductCategory", this.productCategory);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this._router.navigateByUrl("purchase/product-category");
          }
          else
            this._toastrService.error(output.messages[0])
        }
        else {
          this._toastrService.error(this._translate.transform("PLEASE_ENTER_APPROVER"));
        }
      } else {
        this._toastrService.error(this._translate.transform("ALREADY_EXCITING_APPROVER"));
      }
    }
  }
}
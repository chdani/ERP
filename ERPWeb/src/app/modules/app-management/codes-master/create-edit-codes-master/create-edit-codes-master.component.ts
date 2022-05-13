import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { MessageService } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { ErrorStateMatcher } from '@angular/material/core';
import { ValidatorFn, AbstractControl } from '@angular/forms';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';


/**
 * @title Stepper overview
 */
@Component({
  selector: 'app-codes-master',
  templateUrl: './create-edit-codes-master.component.html',
  styleUrls: ['./create-edit-codes-master.component.scss'],
  providers: []


})
export class CreateEditCodesMasterComponent extends AppComponentBase implements OnInit {
  submitted = false;
  id: any;
  validation: boolean = true;
  codesMaster: any;
  codeDetails: any = [];
  Details: any = [];

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _codeMasterService: CodesMasterService,
    private _translate: TranslatePipe,
    private _router: Router,
    private _toastrService: ToastrService,
    private _webApiservice: WebApiService,
    public _commonService: AppCommonService,
    private _confirmService: ConfirmationService,

  ) { super(injector, 'SCR_CODES_MASTER', 'allowAdd', _commonService) }

  ngOnInit() {
    this.codesMaster = {
      id: "",
      description: "",
      code: "",
      approvalWorkFlow: [],
    }
    if (history.state && history.state.codesMasterId && history.state.codesMasterId != '') {
      this.id = history.state.codesMasterId;
    }
    if (this.id && this.id !== "")
      this._commonService.updatePageName(this._translate.transform("CODE_MASTER_EDIT"));
    else
      this._commonService.updatePageName(this._translate.transform("CODE_MASTER_ADD"));

    this.getAlldata();

  }
  async getAlldata() {
    debugger
    if (this.id && this.id !== "") {
      var result = await this._webApiservice.get("getCodesMasterById/" + this.id);
      this.codesMaster = result;
      this.codeDetails = this.codesMaster.codesDetail;
      this.hideShowMoveButtons();
    }
  }

  addNewCodeMaster() {
    var newCodesMaster =
    {
      code: "",
      description: "",
      id: "",
      displayOrder: "",
    }
    this.codeDetails.push(newCodesMaster);
    this.hideShowMoveButtons();
  }

  moveUp(value, index) {
    if (index > 0) {
      const tmp = this.codeDetails[index - 1];
      this.codeDetails[index - 1] = this.codeDetails[index];
      this.codeDetails[index] = tmp;
    }
    this.hideShowMoveButtons();
  }

  moveDown(value, index) {
    if (index < this.codeDetails.length - 1) {
      const tmp = this.codeDetails[index + 1];
      this.codeDetails[index + 1] = this.codeDetails[index];
      this.codeDetails[index] = tmp;
    }
    this.hideShowMoveButtons();
  }
  removeCodeMaster(workflow) {
    this.codeDetails.forEach((item, index) => {
      if (item === workflow) this.codeDetails.splice(index, 1);
    });
    this.hideShowMoveButtons();
  }

  hideShowMoveButtons() {
    for (var index = 0; index < this.codeDetails.length; index++) {
      if (this.codeDetails.length == 1) {
        this.codeDetails[index].moveDown = false
        this.codeDetails[index].moveUp = false;
      }
      else if (index == 0) {
        this.codeDetails[index].moveUp = false;
        this.codeDetails[index].moveDown = true;
      }
      else if (index == this.codeDetails.length - 1) {
        this.codeDetails[index].moveDown = false
        this.codeDetails[index].moveUp = true;
      }
      else {
        this.codeDetails[index].moveUp = true;
        this.codeDetails[index].moveDown = true;
      }
    }
  }


  async codemasterActiveorInactivefunction(item) {
    debugger
    if (item.active == "Y") {
      this._confirmService.confirm({
        message: this._translate.transform("CODE_DEACTIVE_CONF"),
        key: 'codemasterActiveorInactive',
        accept: async () => {
          item.active = "N"

        }
      });
    }
    else {
      this._confirmService.confirm({
        message: this._translate.transform("CODE_ACTIVE_CONF"),
        key: 'codemasterActiveorInactive',
        accept: async () => {
          item.active = "Y"

        }
      });
    }
  }

  codesMasterCancel() {
    this._router.navigateByUrl("app-management/app-codes-master");
  }
  async codesMastersave() {
    debugger
    this.validation = true;

    if (this.codeDetails.length == 0) {
      this.validation = false;
    } else {
      this.codeDetails.forEach(element => {
        if (element.description == "" || element.code == "")
          this.validation = false;
      });
    }
    if (this.validation) {
      var level = 1;
      this.Details = [];
      this.codeDetails.forEach(element => {
        debugger
        if (element.id != '') {
          this.Details.push({
            id: element.id, description: element.description, code: element.code, DisplayOrder: level, action: "M", createdBy: element.createdBy, active: element.active,
            createdDate: element.createdDate, modifiedBy: element.modifiedBy, modifiedDate: element.modifiedDate
          })

        }
        else {
          this.Details.push({
            id: element.id, description: element.description, code: element.code, DisplayOrder: level, action: "N", createdBy: "", active: "Y",
            createdDate: "", modifiedBy: "", modifiedDate: ""
          })

        }


        level++;
      });

      const status = this.Details.some(user => {
        let counter = 0;
        for (const iterator of this.Details) {
          if (iterator.code === user.code && iterator.name === user.name) {
            counter += 1;
          }
        }
        return counter > 1;
      });
      if (!status) {
        this.codesMaster.codesDetail = this.Details;
        this.codesMaster.CodeType = "U";
        this.codesMaster.active = "Y";
        this.codesMaster.action = "N";
        if (this.codesMaster.id && this.codesMaster.id != "") {
          this.codesMaster.action = "M";
          this.codesMaster.createdBy = this.codesMaster.createdBy;
          this.codesMaster.createdDate = this.codesMaster.createdDate;
          this.codesMaster.modifiedBy = this.codesMaster.modifiedBy;
          this.codesMaster.modifiedDate = this.codesMaster.modifiedDate;
        }
        var result = await this._webApiservice.post("saveCodesMaster", this.codesMaster);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this._router.navigateByUrl("app-management/app-codes-master");
          }
          else
            this._toastrService.error(output.messages[0])
        }
      }
      else {
        this._toastrService.error(this._translate.transform("ALREADY_EXCITING_CODE"));
      }

    } else {
      this._toastrService.error(this._translate.transform("PLEASE_ENTER_CODE"));
    }


  }
}
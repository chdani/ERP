import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';

@Component({
  selector: 'app-creat-edit-teller',
  templateUrl: './creat-edit-teller.component.html',
  styleUrls: ['./creat-edit-teller.component.scss'],
  providers: [TranslatePipe]
})
export class CreatEditTellerComponent extends AppComponentBase implements OnInit {
  lang;
  formFieldHelpers: string[] = [''];
  tellForm: any = {};
  orginalTellerData: any;
  userList: any = [];
  filteredUserList: any = [];
  selectedUser: any;

  constructor(
    injector: Injector,
    private formBuilder: FormBuilder,
    private _translate: TranslatePipe,
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    public _commonService: AppCommonService,
    private _webApiService: WebApiService,
    private ref: DynamicDialogRef, private config: DynamicDialogConfig
  ) { super(injector, 'SCR_PETTY_CASH_MGMT', 'allowAdd', _commonService) }
  id: any;

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.id = this.config.data === undefined ? "" : this.config.data.id;

    this.tellForm = {
      id: '',
      tellerCode: '',
      tellerName: '',
      isHeadTeller: false,
    };
    this.getDefaultValues();
  }

  async getDefaultValues() {
    var input = {
      active: "Y"
    }
    var userResult = await this._webApiService.post("getUserList", input);
    if (userResult) {
      this.userList = userResult as any;
      this.userList.forEach(element => {
        element.fullName = element.firstName + " " + element.lastName
      });
    }
    if (this.id !== "") {
      this.spinner.show();
      this._webApiService.getObserver("getPettyCashTellerById/" + this.id)
        .pipe(finalize(() => { this.spinner.hide(); })).subscribe(result => {

          this.orginalTellerData = result as any;
          this.tellForm = {
            id: this.id,
            tellerCode: result.tellerCode,
            tellerName: result.tellerName,
            isHeadTeller: result.isHeadTeller,
          };
          var user = this.userList.filter(a => a.id == result.userId);
          if (user && user.length > 0)
            this.selectedUser = user[0];
        }, error => { });
    }
  }

  get f() { return this.tellForm.controls; }

  async onSubmit() {

    if (!this.selectedUser) {
      this._toastrService.error(this._translate.transform("PETTY_CASHIER_NAME_REQ"));
      return;
    }

    if (this.tellForm.tellerCode == "") {
      this._toastrService.error(this._translate.transform("PETTY_TELLER_CODE_REQ"));
      return;
    }

    if (this.tellForm.tellerName == "") {
      this._toastrService.error(this._translate.transform("PETTY_TELLER_NAME_REQ"));
      return;
    }


    //Set Value
    var tellerInfo: any = {};

    if (this.tellForm.id && this.tellForm.id != "")
      tellerInfo = this.orginalTellerData;

    tellerInfo.id = this.tellForm.id;
    tellerInfo.tellerCode = this.tellForm.tellerCode;
    tellerInfo.tellerName = this.tellForm.tellerName;
    tellerInfo.isHeadTeller = this.tellForm.isHeadTeller;
    tellerInfo.active = "Y";
    tellerInfo.userId = this.selectedUser.id;

    var result = await this._webApiService.post("savePettyCashTeller", tellerInfo);

    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.ref.close();
      }
      else {
        this._toastrService.error(output.messages[0])
      }
    }

  }


  filterUser(event: any) {
    this.filteredUserList = this.userList.filter(a => a.fullName.toUpperCase().includes(event.query.toUpperCase()));
  }

}


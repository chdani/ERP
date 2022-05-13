import { ThrowStmt } from '@angular/compiler';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { result } from 'lodash';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { WebApiService } from 'app/shared/webApiService';
import { TranslatePipe } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';

@Component({
  selector: 'app-creat-edit-account',
  templateUrl: './creat-edit-account.component.html',
  styleUrls: ['./creat-edit-account.component.scss'],
  providers: [TranslatePipe]
})
export class CreatEditAccountComponent extends AppComponentBase implements OnInit {
  lang;
  formFieldHelpers: string[] = [''];
  registerForm: FormGroup;
  submitted = false;

  pettyAccount: any = {
    id:'',accountCode: '', accountName: '', Active:'Y'
  };

  constructor(
    injector:Injector,
    private formBuilder: FormBuilder,
    private _translate: TranslatePipe,
    private spinner: NgxSpinnerService,
    private _toastrService: ToastrService,
    public _commonService : AppCommonService,
    private _webApiService: WebApiService,
    private ref: DynamicDialogRef, private config: DynamicDialogConfig
    ) {super(injector,'SCR_CASH_MGMT','allowAdd', _commonService ) }
    id: any;

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.id = this.config.data === undefined ? "" : this.config.data.id;

    this.registerForm = this.formBuilder.group({
      id:[''],
      accountCode: ['', Validators.required],
      accountName: ['', Validators.required],
     // isHeadAccount: [''],
    });
    if(this.id !== ""){
      this.spinner.show();
      var result = this._webApiService.getObserver("getPettyCashAccountById/"+this.id)
      .pipe(finalize(()=>{this.spinner.hide();})).subscribe(result=>{
        this.registerForm.setValue({
          id:this.id,
          accountCode:result.accountCode,
          accountName:result.accountName,
         // isHeadAccount:result.isHeadAccount,
        });
      }, error=>{});
    }
  }

  get f() { return this.registerForm.controls; }

  async onSubmit() {
    this.submitted = true;
    if (this.registerForm.invalid) {
      return;
    }
    //Set Value
    this.pettyAccount.id = this.registerForm.value.id;
    this.pettyAccount.accountCode = this.registerForm.value.accountCode;
    this.pettyAccount.accountName  = this.registerForm.value.accountName;
   // this.pettyAccount.isHeadAccount  = this.registerForm.value.isHeadAccount;
    // this.registerForm.disable();
    var result = await this._webApiService.post("savePettyCashAccount", this.pettyAccount);

    if (result)
    {
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

  public errorHandling = (controlName: string, errorName: string) => {
    return this.registerForm.controls[controlName].hasError(errorName);
  }
}

interface userTypes {
  displaytext: string;
  id: string;
}

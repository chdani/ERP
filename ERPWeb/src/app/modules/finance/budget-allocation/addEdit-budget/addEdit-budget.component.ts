import { Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { takeUntil } from 'rxjs/operators';
import { FinanceService } from '../../finance.service';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePipe } from '@angular/common';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';

@Component({
  selector: 'addEdit-budget',
  templateUrl: './addEdit-budget.component.html',
  styleUrls: ['./addEdit-budget.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService, DatePipe],
})

export class AddEditBudgetAllocationComponent extends AppComponentBase implements OnInit, OnDestroy {
  budgetForm: any;
  budgetAmount: any;
  ledgerCode: any;
  remarks: any;
  AppDocuments: any = [];
  userId: any = [];
  selectedUploadData: any = {};

  public budgetTypes: any = [];
  public facilityCodes: any = [];
  uploadConfig: FileUploadConfig;
  public ledgerGroup: any = [];
  public filteredLedgerGroup: any = [];
  public filteredLedgerGroupTo: any = [];
  public ledgerCodes: any = [];
  public ledgerCodesTo: any = [];
  public filteredLedgers: any = [];
  public filteredLedgersTo: any = [];
  header: any = [];
  public budgetDetails: any = [];
  public selectedBudget: any;
  public selectedLedger: any;
  public selectedLedgerGroup: any;
  public selectedLedgerGroupTo: any;
  public selectedLedgerTo: any;
  public selectedBudgetType: any;
  public prevBudgetInfo: any;
  public showTransfer: boolean = false;
  public showReturn: boolean = false;
  public allowDeletePermission: boolean = true;
  public ledgerBalance: any = [];
  public Filecontent: any = [];
  public FilecontentBudDetails: any = [];
  public balanceAmount: number = 0;
  public balanceAmountTo: number = 0;
  public fileUpload: any = [];
  public todayDateTime = new Date();
  public ledgerCodesForReturn: any = [];
  public filteredLedgersForReturn: any = [];
  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private dialogService: DialogService,
    private _toastrService: ToastrService,
    private _primengConfig: PrimeNGConfig,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService,
    private _financeService: FinanceService,
  ) {
    super(injector, 'SCR_BUDGT_ALLOCATION', "allowAdd", _commonService);
    this._primengConfig.ripple = true;
    this._commonService.fileObserver.pipe((takeUntil(this._unsubscribeAll)))
      .subscribe((file: any) => {
        this.selectedUploadData.appDocuments = file;
      });

  }
  dialogRef: DynamicDialogRef;
 
  ngOnInit(): void {
    this.budgetForm = {
      budgetType: '',
      budgetDate: '',
      id: '',
      transNo: '',
      appDocuments: [],
    };

    if (history.state && history.state.budgetId && history.state.budgetId != '') {
      this.budgetForm.id = history.state.budgetId;
      this._commonService.updatePageName(this._translate.transform("BUDGT_EDIT"));
    }
    else
      this._commonService.updatePageName(this._translate.transform("BUDGT_ADD"));

    this.getAll()
  }
  attachmentLink(data, type) {
    if (this.isGranted('SCR_BUDGT_ALLOCATION', this.actionType.allowDelete)) {
      this.allowDeletePermission = false;
    }
    this.header = this._translate.transform("FILE_ATTACHMENT");
    this.selectedUploadData = data;
    this.uploadConfig =
    {
      TransactionId: data.id,
      TransactionType: type,
      AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
      DocTypeRequired: false,
      DocumentReference: "",
      ReadOnly: this.allowDeletePermission,
      ScanEnabled: true,
      ShowSaveButton: false,
      FileContent: data.appDocuments
    };

    this.dialogRef = this.dialogService.open(FileUploadComponent, {
      header: this.header,
      width: "700px",
      closable: false,
      contentStyle: { "height": "500px", overflow: "auto" },
      baseZIndex: 500,
      dismissableMask: true,
      data: this.uploadConfig
    });
  }
  async getAll() {
    this.getLedgerAccountGroupList();
    this.budgetTypes = await this._codeMasterService.getCodesDetailByGroupCode("BUDGTDOCTYPE", false, false, this._translate);
    this.ledgerCodesForReturn = await this._financeService.getLedgerAccounts("LDGACCUSEDFORBA", false, false, this._translate, this.orgId);
    this.filteredLedgersForReturn = this.ledgerCodesForReturn;
   
    var search = {
      orgId: this.orgId,
      finYear: this.financialYear
    };
   
    var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    if (result) {
      this.ledgerBalance = result as any;
      this.filteredLedgersForReturn.forEach(element => {
        var balanceAcc = this.ledgerBalance.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }

    if (this.budgetForm.id && this.budgetForm.id != '') {
      var result = await this._webApiService.get("getBudgetAlloctionById/" + this.budgetForm.id);
      if (result) {
        this.prevBudgetInfo = result as any;
        if (this.prevBudgetInfo.validations == null) {
          this.budgetDetails = [];
          this.budgetForm = {
            budgetType: this.prevBudgetInfo.budgetType,
            finYear: this.prevBudgetInfo.finYear,
            budgetDate: new Date(this.prevBudgetInfo.budgetDate),
            id: this.prevBudgetInfo.id,
            transNo: this.prevBudgetInfo.transNo,
            appDocuments: this.prevBudgetInfo.appDocuments,
          };

          if (this.budgetForm.budgetType == "BUDG_TRANS") this.showTransfer = true;
          else if (this.budgetForm.budgetType == "BUDG_RETURN") this.showReturn = true;
          this.selectedBudgetType = this.prevBudgetInfo.budgetType;
          this.prevBudgetInfo.budgAllocDet.forEach(element => {
            var ledger = this.ledgerCodes.filter(a => a.ledgerCode == element.ledgerCode)[0];
            if (ledger)
              element.ledgerDesc = element.ledgerCode + " - " + ledger.ledgerDesc;

            ledger = this.ledgerCodes.filter(a => a.ledgerCode == element.toLedgerCode)[0];
            if (ledger)
              element.toLedgerDesc = element.ledgerCode + " - " + ledger.ledgerDesc;
            this.budgetDetails.push(element);
          });
        }
      }
    }
  }

  async onBudgetTypeChange() {
    if (this.selectedBudgetType == "BUDG_TRANS" || this.selectedBudgetType == "BUDG_RETURN") {
      if (this.selectedBudgetType == "BUDG_RETURN") {
        this.showReturn = true;
        this.showTransfer = false;
      }
      if (this.selectedBudgetType == "BUDG_TRANS") {
        this.showTransfer = true;
        this.showReturn = false;
      }
      this.budgetDetails = [];
     
    }

    else {
      if (this.showTransfer) this.budgetDetails = [];
      this.showTransfer = false;
      this.showReturn = false;
    }

  }
  async createOredit() {
    if (!this.budgetDetails || this.budgetDetails.length == 0) {
      this._toastrService.error(this._translate.transform("BUDGT_DETAILS_REQUIRED"));
      return;
    }
    if (!this.selectedBudgetType) {
      this._toastrService.error(this._translate.transform("BUDGT_TYPE_REQUIRED"));
      return;
    }

    if (this.budgetForm.budgetDate == "") {
      this._toastrService.error(this._translate.transform("BUDGT_BUDGT_DATE_REQ"));
      return;
    }
    var budgetAmount = 0;
    this.budgetDetails.forEach(element => {
      budgetAmount += Number(element.budgetAmount);
    });

    var updatedBudDet = [];
    if (this.budgetForm.id && this.budgetForm.id != "") {
      this.prevBudgetInfo.budgAllocDet.forEach(element => {
        var currentDet = this.budgetDetails.filter(a => a.id == element.id)
        if (!currentDet || currentDet.length == 0) {
          element.active = "N";
          updatedBudDet.push(element);
        }
      });
      this.budgetDetails.forEach(element => {
        updatedBudDet.push(element);

      });
      this.budgetForm.modifiedDate = this.prevBudgetInfo.modifiedDate;
    }
    else {
      updatedBudDet = this.budgetDetails;
      this.budgetForm.finYear = this.financialYear;

    }
    this.fileUpload.forEach(ele => {
      this.budgetForm.appDocuments = ele;
    });
    this.budgetForm.budgAllocDet = updatedBudDet;
    this.budgetForm.active = "Y";
    this.budgetForm.budgetAmount = budgetAmount;
    this.budgetForm.budgetType = this.selectedBudgetType;
    this.budgetForm.orgId = this.orgId;
    this.budgetForm.status = "SUBMITTED";
    var result = await this._webApiService.post("saveBudgetAllocation", this.budgetForm)
    if (result) {

      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.router.navigateByUrl("finance/budget-allocation");
      }
      else {
        console.log(output.messages[0]);
        this._toastrService.error(output.messages[0])
      }
    }
  }

    
  async getLedgerAccountGroupList() {
    var result = await this._webApiService.get("getLedgerAccountGroups");
    if (result){
    
      result.forEach(element => {
        if(element.active=='Y')
          element.accountDescCode = element.accountCode + " - " + element.accountDesc;
    });  
    this.ledgerGroup = result;
   
    }
     
  }

  cancelAddEdit() {
    this.router.navigateByUrl("finance/budget-allocation");
  }

  addBudgetDetail() {
    if (!this.selectedLedger || !this.selectedLedger.ledgerCode) {
      this._toastrService.error(this._translate.transform("BUDGT_LEDG_ACC_PLACEHOLDER"));
      return;
    }
    if (this.showTransfer && (!this.selectedLedgerTo || !this.selectedLedgerTo.ledgerCode)) {
      this._toastrService.error(this._translate.transform("BUDGT_LEDG_ACC_PLACEHOLDER"));
      return;
    }
    if (!this.budgetAmount || this.budgetAmount == "" || this.budgetAmount == "0") {
      this._toastrService.error(this._translate.transform("BUDGT_AMOUNT_REQUIRED"));
      return;
    }
    if (!this.budgetDetails)
      this.budgetDetails = [];
    else {
      if (this.showTransfer) {
        var duplicate = this.budgetDetails.filter(a => a.ledgerCode == this.selectedLedger.ledgerCode
          && a.toledgerCode == this.selectedLedgerTo.ledgerCode && a.active == 'Y');
        if (duplicate && duplicate.length > 0) {
          this._toastrService.error(this._translate.transform("BUDGT_ACC_ALREADY_ADDED"));
          return;
        }
        var totalAmount = this.budgetAmount;
        this.budgetDetails.forEach(element => {
          if (element.ledgerCode == this.selectedLedger.ledgerCode)
            totalAmount += element.budgetAmount;
        });
        if (totalAmount > this.selectedLedger.balance) {
          this._toastrService.error(this._translate.transform("BUDGT_ACC_NO_BALANCE"));
          return;
        }
      }

      else {
        var duplicate = this.budgetDetails.filter(a => a.ledgerCode == this.selectedLedger.ledgerCode && a.active == 'Y');
        if (duplicate && duplicate.length > 0) {
          this._toastrService.error(this._translate.transform("BUDGT_ACC_ALREADY_ADDED"));
          return;
        }
      }
    }
    var detail = {
      action: 'N',
      active: 'Y',
      budgetAmount: this.budgetAmount,
      ledgerCode: this.selectedLedger.ledgerCode,
      ledgerDesc: this.selectedLedger.ledgerDescCode,
      toLedgerCode: "",
      toLedgerDesc: "",
      orgId: this.orgId,
      remarks: this.remarks,
      appDocuments: [],
    };
    if (this.showTransfer) {
      detail.toLedgerCode = this.selectedLedgerTo.ledgerCode;
      detail.toLedgerDesc = this.selectedLedgerTo.ledgerDescCode;
    }
    else {
      detail.toLedgerCode = "";
      detail.toLedgerDesc = "";
    }
    this.budgetDetails.push(detail);

    this.budgetAmount = ""
    this.selectedLedger = null;
    this.selectedLedgerTo = null;
    this.remarks = "";
  }
  onLedgerChange() {
    this.selectedLedgerTo = null;
    if (this.showTransfer) {

      var totalAmount = 0;
      if (this.budgetDetails) {
        this.budgetDetails.forEach(element => {
          if (element.ledgerCode == this.selectedLedger.ledgerCode)
            totalAmount += element.budgetAmount;
        });
      }
      this.balanceAmount = this.selectedLedger.balance - totalAmount;
    }
  }
  onLedgerToChange() {
    
    if (this.showTransfer) {

      var totalAmount = 0;
      if (this.budgetDetails) {
        this.budgetDetails.forEach(element => {
          if (element.ledgerCodeTo == this.selectedLedgerTo.ledgerCodeTo)
            totalAmount += element.budgetAmount;
        });
      }
      this.balanceAmountTo = this.selectedLedgerTo.balance - totalAmount;
    }
  }
  async onLedgerGroupsChange(type:string) {
    // var search = {
    //   orgId: this.orgId,
    //   finYear: this.financialYear
    // };
    //   var result = await this._webApiService.post("getLedgerAccWiseCurrentBalance", search);
    //   if (result) {
    //     this.ledgerBalance = result as any;       
    //   }

    if(type=='To'){
      var input = {
        LedgerCodeFrom: this.selectedLedgerGroupTo.ledgerCodeFrom,
        LedgerCodeTo: this.selectedLedgerGroupTo.ledgerCodeTo
      }
      this.ledgerCodesTo = await this._webApiService.post("getLedgerAccountList", input);
      this.ledgerCodesTo.forEach(element => {
       element.ledgerDescCode = element.ledgerCode + " - " + element.ledgerDesc;
       }); 
      this.filteredLedgersTo = this.ledgerCodesTo;
      this.filteredLedgersTo.forEach(element => {
        var balanceAcc = this.ledgerBalance.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }     
    else{
      var input = {
        LedgerCodeFrom: this.selectedLedgerGroup.ledgerCodeFrom,
        LedgerCodeTo: this.selectedLedgerGroup.ledgerCodeTo
      }
      this.ledgerCodes = await this._webApiService.post("getLedgerAccountList", input);
      this.ledgerCodes.forEach(element => {
       element.ledgerDescCode = element.ledgerCode + " - " + element.ledgerDesc;
       }); 
       this.filteredLedgers = this.ledgerCodes;
       this.filteredLedgers.forEach(element => {
        var balanceAcc = this.ledgerBalance.find(a => a.ledgerCode == element.ledgerCode)
        if (balanceAcc)
          element.balance = balanceAcc.balance;
        else
          element.balance = 0;
      });
    }
    
  }

  filterledgerGroups(event: any,type:string) {
    if(type=='To'){
      this.filteredLedgerGroupTo = this.ledgerGroup.filter(a => a.accountDescCode.toUpperCase().includes(event.query.toUpperCase()));
      this.filteredLedgersTo=[];
      this.selectedLedgerTo=null;
    }
    else{
      this.filteredLedgerGroup = this.ledgerGroup.filter(a => a.accountDescCode.toUpperCase().includes(event.query.toUpperCase()));
      this.filteredLedgers=[]; 
      this.selectedLedger=null; 
    }
  }

  filterLedgerAccounts(event: any) {
    if (this.showTransfer || this.showReturn)
      this.filteredLedgers = this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
        && a.balance > 0);   
    else
      this.filteredLedgers = this.ledgerCodes.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase()));

  }

  filterLedgerAccountsTo(event: any) {
    if (this.selectedLedger) {
      this.filteredLedgersTo = this.ledgerCodesTo.filter(a =>
        a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase())
        && a.ledgerCode != this.selectedLedger.ledgerCode);
    }
    else
      this.filteredLedgersTo = this.ledgerCodesTo.filter(a => a.ledgerDescCode.toUpperCase().includes(event.query.toUpperCase()));

  }
  deleteBudgetDet(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.budgetDetails.indexOf(item);
        if (index >= 0)
          this.budgetDetails.splice(index, 1);
      }
    });
  }

  deletefilteredLedgersForReturn(item) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        var index = this.filteredLedgersForReturn.indexOf(item);
        if (index >= 0)
          this.filteredLedgersForReturn.splice(index, 1);
      }
    });
  }
  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
}

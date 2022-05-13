import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'primeng/api';
import { of } from 'rxjs';

@Component({
  selector: 'cash-journal',
  templateUrl: './cash-journal.component.html',
  styleUrls: [ './cash-journal.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [TranslatePipe]
})
export class CashJournalComponent extends AppComponentBase implements OnInit {
  lang;
  public budgetAllocations: any = [];
  public budgetDetails: any = [];
  public showOrHideOrgFinyear:boolean=false;
  cashmodel:any =[];
  displayModal: boolean = false;
  constructor(
    injector:Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _changeDetectorRef: ChangeDetectorRef,
    private spinner: NgxSpinnerService,
    private _codeMasterService: CodesMasterService,
    private _confirmService: ConfirmationService

  ) {
    super(injector,'SCR_CASH_JOURNAL','allowView', _commonService )
  }
  panelOpenState = false;
  isLoading: boolean = false;
  cashtransactionitem = [];
  tellerNameitem = [];
  accountnameitem = [];
  account =[];
  Teller=[];
  cashjournalform:any;

    ngOnInit(): void {
      this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
      this._commonService.updatePageName(this._translate.transform("CASH_TRANSACTION"));
      this.cashjournalform = {
        accountName:"",
        tellerId:"",
        tellerName:"",
        fromDate: this.plusMinusMonthToCurrentDate(-1),
        toDate: new Date(),
        open:"",
        credit:"",
        debit:"",
        close:"",
        finYear : this.financialYear,
        orgId : this.orgId,
      }
      this.loadDefaults();     
       this.showHideOrgFinyear('MNU_CASH_JOURNAL');
  }
  async loadDefaults(){
    await this.tellerNameList();
    await this.accountnamelist()
    await this.cashtransactionlist();
  }

  async cashtransactionlist(){
    this.cashtransactionitem = await this._webApiService.post("getCashJournals", this.cashjournalform); 
      this.cashtransactionitem.forEach(element => {
       var tellername = this.tellerNameitem.find(a => a.id == element.tellerId);
       var Accountname = this.accountnameitem.find(a => a.id == element.accountId);
       element.tellerName=tellername.tellerName,
       element.accountId = Accountname.accountName,
       element.balanceDate = element.transDate,
       element.open = element.credit,
       element.credit = element.credit,
       element.debit = element.debit,
       element.close = (element.open+element.credit)-(element.debit)

   });
  
  }
  async tellerNameList(){
    this.tellerNameitem = [];
      let tellerlist = await this._webApiService.get("tellerNameList");
      this.tellerNameitem=tellerlist;
      tellerlist.forEach(element => {
        this.Teller.push({
          id: element.id,  name:element.tellerName
              });
     });
  }

  async accountnamelist()
  {
       this.accountnameitem = [];
        let accountlist = await this._webApiService.get("accountNameList");
        this.accountnameitem=accountlist;
        accountlist.forEach(element => {
          this.account.push({
            id: element.id,  name:element.accountName
                });
       });
  }
  clearSearchCriteria()
  {
    this.cashjournalform = {
      accountName:"",
      accountId:"",
      tellerId:"",
      tellerName:"",
      fromDate: this.plusMinusMonthToCurrentDate(-1),
      toDate: new Date(),
      open:"",
      credit:"",
      debit:"",
      close:"",
      finYear : this.financialYear,
      orgId : this.orgId,
    }
  }

  accountdetails(item){
  }
}

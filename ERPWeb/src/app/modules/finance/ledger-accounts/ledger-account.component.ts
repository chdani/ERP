import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { ToastrService } from 'ngx-toastr';
import { DialogService } from 'primeng/dynamicdialog';
import { AppCommonService } from 'app/shared/services/app-common.service';

@Component({
  selector: 'app-ledger-account',
  templateUrl: './ledger-account.component.html',
  styleUrls: ['./ledger-account.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class LedgerMasterComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _confirmService: ConfirmationService,
    private _dialogService: DialogService,
    public _commonService: AppCommonService
  ) {
    super(injector,'SCR_LEDGER_ACCOUNT_MST','allowView', _commonService )
    this._commonService.updatePageName(this._translate.transform("LEDGER_MASTER"));
  }

  gridDetailsContextMenu: MenuItem [] = [];
  selectedRow: any;
  ledgerAccountData: any = []
  ledgerGroupData: any = []
  expandedRows:any = {};
  public showOrHideOrgFinyear:boolean=false;

  activeTabIndex: number = 0;

  ngOnInit(): void {
    this.getHistoryData();
    this.showHideOrgFinyear('MNU_LEDGER_ACCOUNTS');
  }
  // get history data --------------------------------------------------------------------------------------------------
  getHistoryData() {
    if (history.state != null && history.state != undefined && history.state != '') {
      let index_value = JSON.stringify(history.state.setActiveTabIndex);
      if (index_value != null && index_value != undefined && index_value != '') {
        this.activeTabIndex = parseInt(JSON.parse(index_value));
        this.switchTab(this.activeTabIndex, true);
      }
      else {
    this.getLedgerAccountGroupList();
      }
    }
    else {
      this.getLedgerAccountGroupList();
    }
  }
   
   
  getGridDetailsContextMenu(item){
    this.selectedRow=item;
    this.gridDetailsContextMenu = [];
    if(this.isCompGranted('SCR_LEDGER_ACCOUNTS',this.actionType.allowEdit)){
        let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.  editLedgerGroupAccount(item)}  };
        this.gridDetailsContextMenu.push(edit);
    }
    if(this.isCompGranted('SCR_LEDGER_ACCOUNTS',this.actionType.allowDelete)){
        let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.  deleteLedgerGroupAccount(item) } };   
    this.gridDetailsContextMenu.push(Delete);
    }
}

  async getLedgerAccountList() {
    var input = {
      ledgerCode: "",
      ledgerDesc: "",
    }
    this.ledgerAccountData = await this._webApiService.post("getLedgerAccountList", input);
  }

  async getLedgerAccountGroupList() {
    var result = await this._webApiService.get("getLedgerAccountGroups");
    if (result)
      this.ledgerGroupData = result;
  }

  switchTab(event, takeIndex: boolean = false) {
    let index = (takeIndex == true) ? event : event.index;
    this.activeTabIndex = index;
    if (index == 0)
      this.getLedgerAccountGroupList();
    else
      this.getLedgerAccountList();
  }

  // Add and Edit Ledger Account 
  addLedgerGroup() {
    if (this.activeTabIndex == 0) {
      this.router.navigate(["finance/add-edit-ledger-group-account"], {
        state: { setActiveTabIndex: this.activeTabIndex },
      });
     }
  }

  addLedgerAccount(){
    this.router.navigate(["finance/add-edit-ledger-account"], {
      state: { setActiveTabIndex: this.activeTabIndex },
    });
  }

 async editLedgerAccount(item) {
    let result = await this._webApiService.get('getLedgerAccountById/' +item.id);
    this.router.navigate(["finance/add-edit-ledger-account"], {
      state: { transactionId: item.id, setActiveTabIndex: this.activeTabIndex }
    });
  }

 async editLedgerGroupAccount(item) {
    let result = await this._webApiService.get('getLedgerAccountGroupById/' +item.id);
    this.router.navigate(["finance/add-edit-ledger-group-account"], {
      state: { transactionId: item.id, setActiveTabIndex: this.activeTabIndex }
    });
  }

  deleteLedgerAccount(item) {
    this._confirmService.confirm({
      message: this._translate.transform("LEDGER_ACCOUNT_DELETE_CONF"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveLedgerAccount", item);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getLedgerAccountList();
          }
          else {
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  deleteLedgerGroupAccount(item) {
    this._confirmService.confirm({
      message: this._translate.transform("LEDGER_GROUP_ACCOUNT_DELETE_CONF"),
      accept: async () => {
        item.active = "N";
        var result = await this._webApiService.post("saveLedgerAccountGroup", item);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getLedgerAccountGroupList();
          }
          else {
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  async getLedgeraccount(event){
    this.expandedRows = {};
    var input = {
      LedgerCodeFrom: event.data.ledgerCodeFrom,
      LedgerCodeTo: event.data.ledgerCodeTo
    }
    var result = await this._webApiService.post("getLedgerAccountList", input);
    if (result) {
     this.ledgerAccountData = result;
  }
    
  }
}

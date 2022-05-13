import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { CreatEditAccountComponent } from './creat-edit-account/creat-edit-account.component'
import { DialogService } from 'primeng/dynamicdialog';
import { CreatEditTellerComponent } from './creat-edit-teller/creat-edit-teller.component';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { items } from 'app/mock-api/apps/file-manager/data';

@Component({
  selector: 'app-petty-cash',
  templateUrl: './petty-cash.component.html',
  styleUrls: ['./petty-cash.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DialogService]
})

export class PettyCashComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _confirmService: ConfirmationService,
    private _dialogService: DialogService,
    public _commonService : AppCommonService
  ) {
    super(injector,'SCR_CASH_MGMT','allowView', _commonService )
  }


  url: any = {
    accountListUrl: "getPettyCashAccounts",
    saveAccounturl: "savePettyCashAccount",
    markHeadAccountUrl: "markAsHeadAccount/",
    markAccountInactiveUrl: "markPettyAccountInactive/"
  }

  pettyAccountData: any = {
    pettyAccountId: "",
    AccountCode: "",
    AccountName: "",
    IsHead: true
  }

  gridDetailsContextMenu: MenuItem[] =[];
  gridTellersDetailsContextMenu: MenuItem [] = [];
  selectedRow: any;
  accountData: any = [];
  tellerData: any = [];

  public activeTabIndex: number = 0;
  public showOrHideOrgFinyear:boolean=false;

  ngOnInit(): void {
    this.getPettyCashAccountList();
    this.showHideOrgFinyear('MNU_PETTY_CASH_MGMT');
  }

  async getPettyCashAccountList() {
    this.accountData = await this._webApiService.get(this.url.accountListUrl);
  }

  async getTellerList() {
    this.tellerData = await this._webApiService.get("getPettyCashTellers");    
  }

  addAccountTeller() {
    if (this.activeTabIndex == 0) {
      const ref = this._dialogService.open(CreatEditAccountComponent, {
        header: this._translate.transform("PETTY_ADD_ACCOUNT_TITLE"),
        width: '50%',
      });

      ref.onClose.subscribe((data: any) => {
        this.getPettyCashAccountList();
      });
    }
    else {
      const ref = this._dialogService.open(CreatEditTellerComponent, {
        header: this._translate.transform("PETTY_ADD_TELLER_TITLE"),
        width: '50%',
      });

      ref.onClose.subscribe((data: any) => {
        this.getTellerList();
      });
    }
  }
   
  getGridDetailsContextMenu(item){
    this.selectedRow=item;
    this.gridDetailsContextMenu = [];
    if(this.isGranted('SCR_PETTY_CASH_MGMT',this.actionType.allowEdit)){
        let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditAccount(item.id)}  };
        this.gridDetailsContextMenu.push(edit);
    }
    if(this.isGranted('SCR_PETTY_CASH_MGMT',this.actionType.allowDelete)){
        let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.deleteAccount(item) } };   
    this.gridDetailsContextMenu.push(Delete);
    }
 }

  

  createOreditAccount(data?: any) {

    const ref = this._dialogService.open(CreatEditAccountComponent, {
      header: this._translate.transform("PETTY_ADD_ACCOUNT_TITLE"),
      width: '60%',
      data: {
        id: data
      }
    });

    ref.onClose.subscribe((data: any) => {
      this.getPettyCashAccountList();
    });
  }

  editTeller(data?: any) {

    const ref = this._dialogService.open(CreatEditTellerComponent, {
      header: this._translate.transform("PETTY_ADD_TELLER_TITLE"),
      width: '60%',
      data: {
        id: data
      }
    });

    ref.onClose.subscribe((data: any) => {
      this.getTellerList();
    });
  }

  async deleteAccount(account) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        account.active = "N";
        var result = await this._webApiService.post(this.url.saveAccounturl, account);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getPettyCashAccountList();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  async deleteTeller(teller) {
    this._confirmService.confirm({
      message: this._translate.transform("APP_DELETE_CONFIRM_MSG"),
      accept: async () => {
        teller.active = "N";
        var result = await this._webApiService.post("savePettyCashTeller", teller);
        if (result) {
          var output = result as any;
          if (output.status == "DATASAVESUCSS") {
            this._toastrService.success(this._translate.transform("APP_SUCCESS"));
            this.getTellerList();
          }
          else {
            console.log(output.messages[0]);
            this._toastrService.error(output.messages[0])
          }
        }
      }
    });
  }

  switchTab(event, takeIndex: boolean = false) {
    debugger
    let index = (takeIndex == true) ? event : event.index;
    this.activeTabIndex = index;
    if (index == 0) {
      this.getPettyCashAccountList();
    }
    else {
      this.getTellerList();
    }
  }

  getTellersGridDetailsContextMenu(item){
    this.selectedRow=item;
    this.gridTellersDetailsContextMenu = [];
    if(this.isGranted('SCR_PETTY_CASH_MGMT',this.actionType.allowEdit)){
        let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.editTeller(item.id)}  };
        this.gridTellersDetailsContextMenu.push(edit);
    }
    if(this.isGranted('SCR_PETTY_CASH_MGMT',this.actionType.allowDelete)){
        let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.deleteTeller(item) } };   
    this.gridTellersDetailsContextMenu.push(Delete);
    }
   }
}

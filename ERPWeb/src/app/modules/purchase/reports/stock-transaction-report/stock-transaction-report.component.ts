import { Component, OnInit, ViewEncapsulation, Injector } from '@angular/core';
import { WebApiService } from 'app/shared/webApiService';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { AppExportExcelConfig, AppExportExcelHeaderConfig, AppExportExcelHorizantolAlign, AppExportExcelVerticalAlign, AppExportService } from 'app/shared/services/app-export.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { items } from 'app/mock-api/apps/file-manager/data';
import { MenuItem } from 'primeng/api';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';
import { PurchaseService } from '../../purchase.service';

@Component({
  selector: 'stock-transaction-report',
  templateUrl: './stock-transaction-report.component.html',
  styleUrls: ['./stock-transaction-report.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe, CurrencyPipe]
})

export class StockTransactionRepComponent extends AppComponentBase implements OnInit {
  lang;
  stockTrans: any = [];
  ledgers: any = [];
  ledgerSearch: any = {};
  drpTransTypes: any = [];
  transactionTypes: any = [];
  showOrHideOrgFinyear: boolean = false;
  openingBalance: any;
  closingBalance: any;
  pdfdisabled: boolean = true;
  item: MenuItem[];
  allProducts: any = [];
  whLocations: any = [];
  prodCategories: any = [];
  products: any = [];
  selectedProdCategory: any = {};
  selectedWHLocation: any = {};
  selectedProduct: any = {}
  selectedTransType: any = {}

  fromDate: any;
  toDate: any;

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private _webApiService: WebApiService,
    private _translate: TranslatePipe,
    public _commonService: AppCommonService,
    private _purchaseService: PurchaseService,
    private _export: AppExportService,
    private _codeMasterService: CodesMasterService,
    private _datePipe: DatePipe,
    private _exportService: ExportService
  ) {
    super(injector, 'SCR_INV_STK_TRN_REP', 'allowView', _commonService)
    this._commonService.updatePageName(this._translate.transform("STK_TRANS_REP_NAME"));
  }

  ngOnInit(): void {
    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this.getDefaults();
    this.showHideOrgFinyear('MNU_PUR_STK_TRN_REP');
    this.item = [
      {
        label: this._translate.transform("APP_PDF"),
        icon: 'pi pi-file-pdf',
        command: (event) => { this.export('PDF'); }
      },
      {
        label: this._translate.transform("APP_EXCEL"),
        icon: 'pi pi-file-excel',
        command: (event) => { this.export('EXCEL'); }
      },
    ];
  }
  export(exportType) {
    this.ledgerSearch.exportType = exportType;
    this.ledgerSearch.exportHeaderText = this._translate.transform("STK_TRANS_REP_NAME");
    var _url = "downloadStockTransReport";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: this._translate.transform("STK_TRANS_REP_NAME"),
      _request: this.ledgerSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }

  async getDefaults() {
    this.whLocations = await this._purchaseService.getWarehouseAndLocation(true, false, this._translate, false);
    this.prodCategories = await this._purchaseService.getProductCategory(true, false, this._translate, false);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, "", false, false);
    this.transactionTypes = await this._codeMasterService.getCodesDetailByGroupCode("STKTRANTYPES", true, false, this._translate);
    this.allProducts = await this._purchaseService.getProductMaster(true, false, this._translate, "", false, false);;

    this.transactionTypes.forEach(element => {
      if (element.code != "TRNOPENINGBALANCE" && element.code != "TRNCLOSINGBALANCE" && element.code != "TRNDIRECTINVOICE")
        this.drpTransTypes.push(element);
    });

    this.clearSearchCriteria();
    this.searchStockTrans();
  }

  async searchStockTrans() {
    var input = {
      fromTransDate: this.fromDate,
      toTransDate: this.toDate,
      productMasterId: this.selectedProduct.id,
      transactionType: this.selectedTransType.code,
      wareHouseLocationId: this.selectedWHLocation.id,
      rodCategoryId: this.selectedProdCategory.code
    };
    var result = await this._webApiService.post("getProductInvTransactions", input);
    if (result) {
      result.forEach(element => {
        if (element.transType == "TRNOPENINGBALANCE")
          this.openingBalance = element.stockIn;
        else if (element.transType == "TRNCLOSINGBALANCE") {
          this.closingBalance = element.stockOut;
        }
        else {
          element.stockIn = element.stockIn > 0 ? element.stockIn : "";
          element.stockOut = element.stockOut > 0 ? element.stockOut : "";

          var transType = this.transactionTypes.find(a => a.code == element.transType);
          var warehouse = this.whLocations.find(a => a.id == element.wareHouseLocationId);
          var product = this.allProducts.find(a => a.id == element.productMasterId);

          element.transaction = transType?.description;
          element.productName = product?.name;
          element.whLocationName = warehouse?.name;
        }
      });

      this.stockTrans = result.filter(a => a.transType != "TRNCLOSINGBALANCE" && a.transType != "TRNOPENINGBALANCE");
    }
  }

  clearSearchCriteria() {
    this.selectedProdCategory = this.prodCategories.find(a => a.code == "");
    this.selectedWHLocation = this.whLocations.find(a => a.id == "");
    this.selectedProduct = this.products.find(a => a.id == "");
    this.fromDate = this.plusMinusMonthToCurrentDate(-1);
    this.toDate = new Date()
  }

  async loadProducts() {

    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, this.selectedProdCategory.code, false, false);
    this.selectedProduct = this.products.find(a => a.id == "");
  }
}

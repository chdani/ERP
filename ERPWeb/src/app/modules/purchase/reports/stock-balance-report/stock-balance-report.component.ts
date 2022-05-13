import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { WebApiService } from 'app/shared/webApiService';
import { MenuItem } from 'primeng/api';
import { PurchaseService } from '../../purchase.service';
import { ExportModel } from 'app/shared/model/export-model';
import { ExportService } from 'app/shared/services/export-service';

@Component({
  selector: 'stock-balance-report',
  templateUrl: './stock-balance-report.component.html',
  styleUrls: ['./stock-balance-report.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe, DatePipe]
})
export class StockBalanceRepComponent extends AppComponentBase implements OnInit {
  lang;
  showOrHideOrgFinyear: boolean = false;
  stockBalnces: any = [];
  whLocations: any = [];
  prodCategories: any = [];
  pdfdisabled: boolean = true;
  allProducts: any = [];
  products: any = [];
  selectedProdCategory: any = {};
  selectedWHLocation: any = {};
  selectedProduct: any = {}
  ledgerSearch: any = {};
  exportMenuItem: MenuItem[];
  constructor(
    injector: Injector,
    private _webApiService: WebApiService,
    public _commonService: AppCommonService,
    private _translate: TranslatePipe,
    private _datePipe: DatePipe,
    private _changeDetectorRef: ChangeDetectorRef,
    private _purchaseService: PurchaseService,
    private _exportService: ExportService

  ) {
    super(injector,'SCR_INV_STK_BAL_REP','allowView', _commonService )
  }

  ngOnInit(): void {

    this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
    this._commonService.updatePageName(this._translate.transform("STK_BALANCE_REP_NAME"));
    this.loadDefaults();
    this.showHideOrgFinyear('MNU_PUR_STK_BAL_REP');
    this.exportMenuItem = [
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
    this.ledgerSearch.exportHeaderText = "Stock Balance Report";

    var _url = "downloadProdInventoryBalance";
    var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
    let exportModel: ExportModel = {
      _fileName: "Stock Balance Report",
      _request: this.ledgerSearch,
      _type: exportType,
      _url: _url,
      _date: fileDate
    };
    this._exportService.exportFile(exportModel);
  }


  async loadDefaults() {
    this.whLocations = await this._purchaseService.getWarehouseAndLocation(true, false, this._translate, false);
    this.prodCategories = await this._purchaseService.getProductCategory(true, false, this._translate, false);
    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, "", false,false);
    this.allProducts = await this._purchaseService.getProductMaster(true, false, this._translate, "", false,false);;

    await this.searchStockBalance();
  }

  async searchStockBalance() {

    this.ledgerSearch = {
      wareHouseLocationId: this.selectedWHLocation.id,
      productMasterId: this.selectedProduct.id,
      prodCategoryId: this.selectedProdCategory.code
    }

    var result = await this._webApiService.post('getProdInventoryBalance', this.ledgerSearch)
    if (result.length == 0)
      this.pdfdisabled = false;
    else
      this.pdfdisabled = true;
    if (result) {
      this._changeDetectorRef.markForCheck();
      this.stockBalnces = result as any;
      this.stockBalnces.forEach(element => {
        var location = this.whLocations.find(a => a.id == element.wareHouseLocationId);
        var product = this.allProducts.find(a => a.id == element.productMasterId);
        element.productName = product?.name,
          element.whLocationName = location?.name
      });
    }
  }

  clearSearchCriteria() {
    this.selectedProdCategory = this.prodCategories.find(a => a.code == "");
    this.selectedWHLocation = this.whLocations.find(a => a.id == "");
    this.selectedProduct = this.products.find(a => a.id == "");
  }

  async loadProducts() {

    this.products = await this._purchaseService.getProductMaster(true, false, this._translate, this.selectedProdCategory.code, false,false);
    this.selectedProduct = this.products.find(a => a.id == "");
  }

}

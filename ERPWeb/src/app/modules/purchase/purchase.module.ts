import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { FuseAlertModule } from '@fuse/components/alert';
import { SharedModule } from 'app/shared/shared.module';
import { MatRadioModule } from '@angular/material/radio';
import { MatSidenavModule } from '@angular/material/sidenav';
import { CalendarModule } from 'primeng/calendar';
import { A11yModule } from '@angular/cdk/a11y';
import { OverlayModule } from '@angular/cdk/overlay';
import { PortalModule } from '@angular/cdk/portal';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CdkTableModule } from '@angular/cdk/table';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { TranslateModule } from '@ngx-translate/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { TabViewModule } from 'primeng/tabview';
import { TableModule } from 'primeng/table';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { RadioButtonModule } from 'primeng/radiobutton';
import { MultiSelectModule } from 'primeng/multiselect';
import { DialogModule } from 'primeng/dialog';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DividerModule } from 'primeng/divider';
import { InputNumberModule } from 'primeng/inputnumber';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { PurchaseRoutingModule } from './purchase-routing.module';
import { VendorComponent } from './vendor/vendor.component';
import { createeditvendorComponend } from './vendor/create-edit-vendor/create-edit-vendor.component';
import { PickListModule } from 'primeng/picklist';
import { ProductCategoryComponent } from './product-category/product-category.component';
import { createeditProductCategoryComponend } from './product-category/create-edit-product-category/create-edit-product-category.component';
import { MenuModule } from 'primeng/menu';
import { ContextMenuModule } from 'primeng/contextmenu';
import { prodUnitMasterComponent } from './product-unit-master/prod-unit-master.component';
import { CreateEditProdUnitMasterComponent } from './product-unit-master/create-edit-prod-unit-master/create-edit-prod-unit-master/create-edit-prod-unit-master.component';
import { ProductMasterComponent } from './product-master/product-master.component';
import { CreateEditProductMasterComponent } from './product-master/create-edit-product-master/create-edit-product-master.component';
import { CreateEditQuotationRequestComponent } from './quotation-request/create-edit-quotation-request/create-edit-quotation-request/create-edit-quotation-request.component';
import { VendorQuotationComponent } from './vendor-quotation/vendor-quotation/vendor-quotation.component';
import { CreateEditVendorQuotationComponent } from './vendor-quotation/create-edit-vendor-quotation/create-edit-vendor-quotation/create-edit-vendor-quotation.component';
import { ServiceReqComponent } from './service-request/service-request.component';
import { AddEditServiceReqComponent } from './service-request/addEdit-service-request/addEdit-service-request.component';
import { PanelModule } from 'primeng/panel';
import { SidebarModule } from 'primeng/sidebar';
import { QuotationRequestComponent } from './quotation-request/quotation-request.component';
import { AppGlobalDataService } from 'app/shared/services/app-global-data-service';
import { AccordionModule } from 'primeng/accordion';
import { AddEditGRNComponent } from './goods-receipt-notes/addEdit-goods-receipt-notes/addEdit-goods-receipt-notes.component';
import { GoodsReceiptNotesComponent } from './goods-receipt-notes/goods-receipt-notes.component';
import { WareHouseComponent } from './warehouse/warehouse.component';
import { CreateEditWareHouseComponent } from './warehouse/create-edit-warehouse/create-edit-warehouse.component';
import { AddEditInvIssueComponent } from './inventory-issue/addEdit-inventory-issue/addEdit-inventory-issue.component';
import { InventoryIssueComponent } from './inventory-issue/inventory-issue.component';
import { PurchaseOrderComponent } from './purchase-order/purchase-order.component';
import { CreateEditPurchaseOrderComponent } from './purchase-order/create-edit-purchase-order/create-edit-purchase-order.component';
import { StockBalanceRepComponent } from './reports/stock-balance-report/stock-balance-report.component';
import { StockTransactionRepComponent } from './reports/stock-transaction-report/stock-transaction-report.component';
import { PurchaseRequestComponent } from './purchase-request/purchase-request.component';
import { CreateEditPurchaseRequestComponent } from './purchase-request/create-edit-purchase-request/create-edit-purchase-request.component';
import { InventoryTransferComponent } from './inventory-transfer/inventory-transfer.component';
import { CreateEditInventoryTransferComponent } from './inventory-transfer/create-edit-inventory-transfer/create-edit-inventory-transfer.component';

@NgModule({
  declarations: [
    VendorComponent,
    createeditvendorComponend,
    ProductCategoryComponent,
    createeditProductCategoryComponend,
    prodUnitMasterComponent,
    CreateEditProdUnitMasterComponent,
    ProductMasterComponent,
    CreateEditProductMasterComponent,
    ServiceReqComponent,
    AddEditServiceReqComponent,
    CreateEditQuotationRequestComponent,
    QuotationRequestComponent,
    AddEditGRNComponent,
    VendorQuotationComponent,
    CreateEditVendorQuotationComponent,
    GoodsReceiptNotesComponent,
    WareHouseComponent,
    CreateEditWareHouseComponent,
    AddEditInvIssueComponent,
    InventoryIssueComponent,
    PurchaseOrderComponent,
    CreateEditPurchaseOrderComponent,
    StockBalanceRepComponent,
    StockTransactionRepComponent,
    PurchaseRequestComponent,
    CreateEditPurchaseRequestComponent,
    InventoryTransferComponent,
    CreateEditInventoryTransferComponent
  ],
  imports: [

    SharedModule,
    CommonModule,
    MatButtonModule,
    MatFormFieldModule,
    PurchaseRoutingModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    MenuModule,
    ContextMenuModule,
    MatIconModule,
    MatInputModule,
    MatRadioModule,
    MatSelectModule,
    MatSidenavModule,
    MatSlideToggleModule,
    FuseAlertModule,
    SharedModule,
    A11yModule,
    CdkTableModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatStepperModule,
    MatCheckboxModule,
    CalendarModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    InputNumberModule,
    MatIconModule,
    MatInputModule,
    PickListModule,

    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,

    MatSlideToggleModule,

    MatSortModule,
    MatTableModule,

    MatTooltipModule,
    MatExpansionModule,
    MatTabsModule,

    OverlayModule,
    PortalModule,
    ScrollingModule,
    TranslateModule,
    CardModule,
    ToolbarModule,
    ButtonModule,
    DropdownModule,

    TabViewModule,
    TooltipModule,
    TableModule,
    DynamicDialogModule,
    InputTextModule,
    FormsModule,
    ConfirmDialogModule,
    AutoCompleteModule,
    RadioButtonModule,
    MultiSelectModule,
    DialogModule,
    BadgeModule,
    CheckboxModule,
    OverlayPanelModule,
    DividerModule,
    PanelModule,
    SidebarModule,
    AccordionModule
  ],
  providers: [ToastrService, ConfirmDialogModule,
    CodesMasterService,
    AppGlobalDataService,
    ConfirmationService]


})
export class PurchaseModule { }

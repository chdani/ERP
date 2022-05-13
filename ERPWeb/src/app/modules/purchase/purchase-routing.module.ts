import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VendorComponent } from './vendor/vendor.component';
import { createeditvendorComponend } from './vendor/create-edit-vendor/create-edit-vendor.component';
import { ProductCategoryComponent } from './product-category/product-category.component';
import { createeditProductCategoryComponend } from './product-category/create-edit-product-category/create-edit-product-category.component';
import { prodUnitMasterComponent } from './product-unit-master/prod-unit-master.component';
import { CreateEditProdUnitMasterComponent } from './product-unit-master/create-edit-prod-unit-master/create-edit-prod-unit-master/create-edit-prod-unit-master.component';
import { ProductMasterComponent } from './product-master/product-master.component';
import { CreateEditProductMasterComponent } from './product-master/create-edit-product-master/create-edit-product-master.component';
import { ServiceReqComponent } from './service-request/service-request.component';
import { QuotationRequestComponent } from './quotation-request/quotation-request.component';
import { CreateEditQuotationRequestComponent } from './quotation-request/create-edit-quotation-request/create-edit-quotation-request/create-edit-quotation-request.component';
import { AddEditServiceReqComponent } from './service-request/addEdit-service-request/addEdit-service-request.component';
import { AddEditGRNComponent } from './goods-receipt-notes/addEdit-goods-receipt-notes/addEdit-goods-receipt-notes.component';
import { GoodsReceiptNotesComponent } from './goods-receipt-notes/goods-receipt-notes.component';
import { WareHouseComponent } from './warehouse/warehouse.component';
import { CreateEditWareHouseComponent } from './warehouse/create-edit-warehouse/create-edit-warehouse.component';
import { InventoryIssueComponent } from './inventory-issue/inventory-issue.component';
import { AddEditInvIssueComponent } from './inventory-issue/addEdit-inventory-issue/addEdit-inventory-issue.component';
import { VendorQuotationComponent } from './vendor-quotation/vendor-quotation/vendor-quotation.component';
import { CreateEditVendorQuotationComponent } from './vendor-quotation/create-edit-vendor-quotation/create-edit-vendor-quotation/create-edit-vendor-quotation.component';
import { PurchaseOrderComponent } from './purchase-order/purchase-order.component';
import { CreateEditPurchaseOrderComponent } from './purchase-order/create-edit-purchase-order/create-edit-purchase-order.component';
import { StockBalanceRepComponent } from './reports/stock-balance-report/stock-balance-report.component';
import { StockTransactionRepComponent } from './reports/stock-transaction-report/stock-transaction-report.component';
import { PurchaseRequestComponent } from './purchase-request/purchase-request.component';
import { CreateEditPurchaseRequestComponent } from './purchase-request/create-edit-purchase-request/create-edit-purchase-request.component';
import { InventoryTransferComponent } from './inventory-transfer/inventory-transfer.component';
import { CreateEditInventoryTransferComponent } from './inventory-transfer/create-edit-inventory-transfer/create-edit-inventory-transfer.component';

const routes: Routes = [
  {
    path:'create-edit-warehouse',
    component: CreateEditWareHouseComponent,
  },
  {
    path:'warehouse',
    component:WareHouseComponent,
  },
  {
    path: 'create-edit-product-master',
    component: CreateEditProductMasterComponent,
  },
  {
    path: 'create-edit-quotation-request',
    component: CreateEditQuotationRequestComponent,
  },
  {
    path: 'quotation-request',
    component: QuotationRequestComponent,
  },
  {
    path: 'product-master',
    component: ProductMasterComponent,
  },
  {
    path: 'create-edit-vendor',
    component: createeditvendorComponend,
  },
  {
    path: 'unit-master',
    component: prodUnitMasterComponent,
  },
  {
    path: 'create-edit-prod-unit-master',
    component: CreateEditProdUnitMasterComponent,
  },
  {
  path: 'purchase-request',
    component: PurchaseRequestComponent,
  },
  {
    path:'create-edit-purchase-request',
    component:CreateEditPurchaseRequestComponent,
  },
  {
    path: 'vendor',
    component: VendorComponent,
  },
  {
    path: 'product-category',
    component: ProductCategoryComponent,
  }, 
  {
    path:'vendor-quotation',
    component:VendorQuotationComponent,
  },
  {
    path:'create-edit-vendor-quotation',
    component:CreateEditVendorQuotationComponent,
  },
  {
    path: 'create-edit-product-category',
    component: createeditProductCategoryComponend,
  },
  {
    path: 'service-request',
    component: ServiceReqComponent,
  },
  {
    path: 'addEdit-service-request',
    component: AddEditServiceReqComponent,
  },
  {
    path: 'goods-receipt-notes',
    component: GoodsReceiptNotesComponent,
  },
  {
    path: 'addEdit-goods-receipt-notes',
    component: AddEditGRNComponent,
  },
  {
    path: 'inventory-issue',
    component: InventoryIssueComponent,
  },
  {
    path: 'addEdit-inventory-issue',
    component: AddEditInvIssueComponent,
  },
  {
    path: 'purchase-order',
    component:PurchaseOrderComponent,
  },
  {
    path:'create-edit-purchase-order',
    component:CreateEditPurchaseOrderComponent,
  },
  {
    path: 'stock-balance-report',
    component: StockBalanceRepComponent,
  },
  {
    path: 'stock-transaction-report',
    component: StockTransactionRepComponent,
  },
  {
    path: 'inventory-transfer',
    component: InventoryTransferComponent,
  },
  {
    path: 'create-edit-inventory-transfer',
    component: CreateEditInventoryTransferComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PurchaseRoutingModule { }

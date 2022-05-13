import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CashInvoicePrintComponent } from 'app/modules/finance/cash-management/cash-invoice-print/cash-invoice-print.component';
import { EmbPostPaymentPrintComponent } from 'app/modules/finance/embassy-payment/print-post-pyament/print-post-payment.component';

const routes: Routes = [
  {
    path: 'cash-invoice-print',
    component: CashInvoicePrintComponent,
  },
  {
    path: 'print-emb-post-payment',
    component: EmbPostPaymentPrintComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppPrintRoutingModule { }

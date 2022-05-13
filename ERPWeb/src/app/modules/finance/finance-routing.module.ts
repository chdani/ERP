import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddEditBudgetAllocationComponent } from './budget-allocation/addEdit-budget/addEdit-budget.component';
import { BudgetAllocationComponent } from './budget-allocation/budget-allocation.component';
import { CashJournalComponent } from './reports/cash-journal-report/cash-journal.component';
import { AddEditCashTransactionComponent } from './cash-management/cash-transaction/addedit-cash-transaction/addEdit-cash-transaction.component';
import { CashTransactionComponent } from './cash-management/cash-transaction/cash-transaction.component';
import { PettyCashComponent } from './cash-management/petty-cash-mgmt/petty-cash.component';
import { CreatEditAccountComponent } from './cash-management/petty-cash-mgmt/creat-edit-account/creat-edit-account.component';
import { CashTransferComponent } from './cash-management/cash-transaction/cash-transfer/cash-transfer.component';
import { EmbassyPaymentComponent } from './embassy-payment/embassy-payment.component';
import { AddEditEmbassyPrePaymentComponent } from './embassy-payment/addedit-embassy-pre-payment/addedit-embassy-pre-payment.component';
import { AddEditEmbassyPostPaymentComponent } from './embassy-payment/addedit-embassy-post-payment/addedit-embassy-post-payment.component';
import { DirectInvoiceComponent } from './direct-invoice/direct-invoice.component';
import { AddeditDirectInvoicePrePaymentComponent } from './direct-invoice/addedit-direct-invoice-pre-payment/addedit-direct-invoice-pre-payment.component';
import { AddeditDirectInvoicePostPaymentComponent } from './direct-invoice/addedit-direct-invoice-post-payment/addedit-direct-invoice-post-payment.component';
import { LedgerMasterComponent } from './ledger-accounts/ledger-account.component';
import { AddEditLedgerAccountsComponent } from './ledger-accounts/add-edit-ledger-accounts/add-edit-ledger-accounts.component';
import { AddEditLedgerGroupAccountsComponent } from './ledger-accounts/add-edit-ledger-group-accounts/add-edit-ledger-group-accounts.component';
import { LedgerBalanceRepComponent } from './reports/ledger-balance-report/ledger-balance.component';
import { LedgerHistroyRepComponent } from './reports/ledger-histroy-report/ledger-histroy.component';

const routes: Routes = [
  {
    path: 'budget-allocation',
    component: BudgetAllocationComponent,
  },
  {
    path: 'addEdit-budget',
    component: AddEditBudgetAllocationComponent,
  },
  {
    path: 'cash-transaction',
    component: CashTransactionComponent,
  },
  {
    path: 'addEdit-cash-transaction',
    component: AddEditCashTransactionComponent,
  },
  {
    path: 'cash-journal',
    component: CashJournalComponent,
  },
  {
    path: 'petty-cash-mgmt',
    component: PettyCashComponent,
  },
  {
    path: 'creat-edit-account',
    component: CreatEditAccountComponent,
  },
  {
    path: 'cash-transfer',
    component: CashTransferComponent,
  },
  {
    path: 'embassy-payment',
    component: EmbassyPaymentComponent,
  },
  {
    path: 'addedit-embassy-pre-payment',
    component: AddEditEmbassyPrePaymentComponent,
  },
  {
    path: 'addedit-embassy-post-payment',
    component: AddEditEmbassyPostPaymentComponent,
  },
  {
    path: 'direct-invoice',
    component: DirectInvoiceComponent,
  },
  {
    path: 'invoice-pre-payments',
    component: AddeditDirectInvoicePrePaymentComponent,
  },
  {
    path: 'invoice-post-payments',
    component: AddeditDirectInvoicePostPaymentComponent,
  },
  {
    path: 'ledger-account',
    component: LedgerMasterComponent,
  },
  {
    path: 'ledger-balance',
    component: LedgerBalanceRepComponent,
  },
  {
    path: 'ledger-Histroy',
    component: LedgerHistroyRepComponent,
  },
  {
    path: 'add-edit-ledger-account',
    component: AddEditLedgerAccountsComponent,
  },
  {
    path: 'add-edit-ledger-group-account',
    component: AddEditLedgerGroupAccountsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FinanceRoutingModule { }

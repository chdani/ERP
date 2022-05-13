import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FileUploadModule } from 'primeng/fileupload';
import { HttpClientModule } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FuseAlertModule } from '@fuse/components/alert';
import { SharedModule } from 'app/shared/shared.module';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FinanceRoutingModule } from './finance-routing.module';
import { BudgetAllocationComponent } from './budget-allocation/budget-allocation.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { AddEditBudgetAllocationComponent } from './budget-allocation/addEdit-budget/addEdit-budget.component';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { PanelModule } from 'primeng/panel';
import { DialogModule } from 'primeng/dialog';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { InputNumberModule } from 'primeng/inputnumber';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { CashTransactionComponent } from './cash-management/cash-transaction/cash-transaction.component';
import { AddEditCashTransactionComponent } from './cash-management/cash-transaction/addedit-cash-transaction/addEdit-cash-transaction.component';
import { CashJournalComponent } from './reports/cash-journal-report/cash-journal.component';
import { TabViewModule } from 'primeng/tabview';
import { CardModule } from 'primeng/card';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DividerModule } from 'primeng/divider';
import { PettyCashComponent } from './cash-management/petty-cash-mgmt/petty-cash.component';
import { CreatEditAccountComponent } from './cash-management/petty-cash-mgmt/creat-edit-account/creat-edit-account.component'
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { CreatEditTellerComponent } from './cash-management/petty-cash-mgmt/creat-edit-teller/creat-edit-teller.component';
import { FieldsetModule } from 'primeng/fieldset';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { CashTransferComponent } from './cash-management/cash-transaction/cash-transfer/cash-transfer.component';
import { EmbassyPaymentComponent } from './embassy-payment/embassy-payment.component';
import { AddEditEmbassyPrePaymentComponent } from './embassy-payment/addedit-embassy-pre-payment/addedit-embassy-pre-payment.component';
import { AddEditEmbassyPostPaymentComponent } from './embassy-payment/addedit-embassy-post-payment/addedit-embassy-post-payment.component';
import { DirectInvoiceComponent } from './direct-invoice/direct-invoice.component';
import { AddeditDirectInvoicePostPaymentComponent } from './direct-invoice/addedit-direct-invoice-post-payment/addedit-direct-invoice-post-payment.component';
import { AddeditDirectInvoicePrePaymentComponent } from './direct-invoice/addedit-direct-invoice-pre-payment/addedit-direct-invoice-pre-payment.component';
import { LedgerMasterComponent } from './ledger-accounts/ledger-account.component';
import { AddEditLedgerAccountsComponent } from './ledger-accounts/add-edit-ledger-accounts/add-edit-ledger-accounts.component';
import { AddEditLedgerGroupAccountsComponent } from './ledger-accounts/add-edit-ledger-group-accounts/add-edit-ledger-group-accounts.component';
import { ExportService } from 'app/shared/services/export-service';
import { MenuModule } from 'primeng/menu';
import { SpeedDialModule } from 'primeng/speeddial';
import { ContextMenuModule } from 'primeng/contextmenu';
import { AccordionModule } from 'primeng/accordion';
import { SidebarModule } from 'primeng/sidebar';
import { MultiSelectModule } from 'primeng/multiselect';
import { LedgerBalanceRepComponent } from './reports/ledger-balance-report/ledger-balance.component';
import { LedgerHistroyRepComponent } from './reports/ledger-histroy-report/ledger-histroy.component';

@NgModule({
  declarations: [
    BudgetAllocationComponent,
    AddEditBudgetAllocationComponent,
    CashTransactionComponent,
    AddEditCashTransactionComponent,
    CashJournalComponent,
    PettyCashComponent,
    CreatEditAccountComponent,
    CreatEditTellerComponent,
    CashTransferComponent,
    EmbassyPaymentComponent,
    AddEditEmbassyPrePaymentComponent,
    AddEditEmbassyPostPaymentComponent,
    DirectInvoiceComponent,
    LedgerBalanceRepComponent,
    AddeditDirectInvoicePostPaymentComponent,
    AddeditDirectInvoicePrePaymentComponent,
    LedgerMasterComponent,
    AddEditLedgerAccountsComponent,
    AddEditLedgerGroupAccountsComponent,
    LedgerHistroyRepComponent,
  ],
  imports: [

    SharedModule,
    FinanceRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    FuseAlertModule,
    TranslateModule,

    //PrimeNG Modules
    TableModule,
    ButtonModule,
    InputTextModule,
    FileUploadModule,
    SpeedDialModule,
    HttpClientModule,
    ToolbarModule,
    CalendarModule,
    MenuModule,
    MultiSelectModule,
    DropdownModule,
    PanelModule,
    DialogModule,
    AutoCompleteModule,
    InputNumberModule,
    ConfirmDialogModule,
    TabViewModule,
    CardModule,
    InputTextareaModule,
    OverlayPanelModule,
    DividerModule,
    DynamicDialogModule,
    CheckboxModule,
    FieldsetModule,
    BadgeModule,
    TooltipModule,
    ContextMenuModule,
    AccordionModule,
    SidebarModule
  ],
  providers: [
    ToastrService,
    CodesMasterService,
    ConfirmationService,
    ExportService,
    AddeditDirectInvoicePostPaymentComponent,
    AddeditDirectInvoicePrePaymentComponent
  ]


})
export class FinanceModule { }

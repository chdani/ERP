import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FuseAlertModule } from '@fuse/components/alert';
import { SharedModule } from 'app/shared/shared.module';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { PanelModule } from 'primeng/panel';
import { DialogModule } from 'primeng/dialog';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { InputNumberModule } from 'primeng/inputnumber';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { TabViewModule } from 'primeng/tabview';
import { CardModule } from 'primeng/card';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DividerModule } from 'primeng/divider';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import {CheckboxModule} from 'primeng/checkbox';
import { FieldsetModule } from 'primeng/fieldset';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { CashInvoicePrintComponent } from 'app/modules/finance/cash-management/cash-invoice-print/cash-invoice-print.component';
import { AppPrintRoutingModule } from './app-print.routing.module'
import { EmbPostPaymentPrintComponent } from 'app/modules/finance/embassy-payment/print-post-pyament/print-post-payment.component';

@NgModule({
  declarations: [
    CashInvoicePrintComponent,
    EmbPostPaymentPrintComponent
  ],
  imports: [
  
    SharedModule,
    AppPrintRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    FuseAlertModule,
    SharedModule,
    TranslateModule,
    
    //PrimeNG Modules
    TableModule,
    ButtonModule,
    InputTextModule,
    ToolbarModule,
    CalendarModule,
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
    TooltipModule
  ],
  providers: [
    ToastrService,
    CodesMasterService,
    ConfirmationService
  ]
    
  
})
export class AppPrintModule { }

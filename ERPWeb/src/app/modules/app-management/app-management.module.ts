import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { FuseAlertModule } from '@fuse/components/alert';
import { SharedModule } from 'app/shared/shared.module';
import { MatRadioModule } from '@angular/material/radio';
import { MatSidenavModule } from '@angular/material/sidenav';
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
import { MatTabsModule } from '@angular/material/tabs'
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
import { CheckboxModule } from 'primeng/checkbox';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DividerModule } from 'primeng/divider';
import { PickListModule } from 'primeng/picklist';

import { ListboxModule } from 'primeng/listbox';

import { ExportService } from 'app/shared/services/export-service';
import { MenuModule } from 'primeng/menu';
import { ContextMenuModule } from 'primeng/contextmenu';
import { AccordionModule } from 'primeng/accordion';
import { SidebarModule } from 'primeng/sidebar';
import { AppManagementRoutingModule } from './app-management-routing.module';
import { CodesMasterComponent } from './codes-master/codes-master.component';
import { CreateEditCodesMasterComponent } from './codes-master/create-edit-codes-master/create-edit-codes-master.component';
import { LanguageMasterComponent } from './language-master/language-master/language-master.component';
import { SystemSettingsComponent } from './system-settings/system-settings.components';
import { MatToolbarModule } from '@angular/material/toolbar';


@NgModule({
  declarations: [
    CodesMasterComponent,
    CreateEditCodesMasterComponent,
    LanguageMasterComponent,
    SystemSettingsComponent,

  ],
  imports: [

    SharedModule,
    AppManagementRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    MatIconModule,
    MatInputModule,
    MatToolbarModule,
    ContextMenuModule,
    MenuModule,
    AccordionModule,
    SidebarModule,
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
    ListboxModule,
    MatCheckboxModule,

    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,

    MatIconModule,
    MatInputModule,

    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    PickListModule,

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
    DividerModule
  ],
  providers: [ToastrService, ConfirmDialogModule,
    ConfirmationService]


})
export class AppManagementModule { }

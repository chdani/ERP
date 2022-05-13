import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import{MatStepperModule} from'@angular/material/stepper';
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
import {FormsModule} from '@angular/forms';
import {InputTextModule} from 'primeng/inputtext';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { RadioButtonModule } from 'primeng/radiobutton';
import { MultiSelectModule } from 'primeng/multiselect';
import { DialogModule } from 'primeng/dialog';
import { BadgeModule } from 'primeng/badge';
import {TooltipModule} from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DividerModule } from 'primeng/divider';
import {DepartmentComponent} from './department/department.component';
import{HRRoutingModule} from 'app/modules/human-resource/human-resource-routing.module';
import { CreateEditDepartmentComponent } from './department/create-edit-department/create-edit-department.component';
import { JobPositionComponent } from './job-position/job-positoion.component';
import { CreatEditjobpositionComponent } from './job-position/create-edit-job-position/create-edit-job-position.component';
import { createeditemployeeComponend } from './employee/create-edit-employee/create-edit-employee.component';
import { EmployeeComponent } from './employee/employee.component';
import {InputNumberModule} from 'primeng/inputnumber';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import {ContextMenuModule} from 'primeng/contextmenu';

@NgModule({
  declarations: [
    DepartmentComponent,
    CreateEditDepartmentComponent,
    JobPositionComponent,
    CreatEditjobpositionComponent,
    createeditemployeeComponend,
    EmployeeComponent
  ],
  imports: [
  
    SharedModule,
    MatButtonModule,
    MatFormFieldModule,
    HRRoutingModule,
    ToastrModule.forRoot({
      positionClass :'toast-bottom-right'
    }),
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
    DividerModule
  ],
  providers: [ToastrService,ConfirmDialogModule,
    CodesMasterService,
    ConfirmationService]
    
  
})
export class HRModule { }

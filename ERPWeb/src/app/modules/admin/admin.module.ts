import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Route, RouterModule } from '@angular/router';
import { DashboardComponent } from 'app/modules/admin/dashboard/dashboard.component';
import { SharedModule } from 'app/shared/shared.module';
import { BudgetComponent } from './budget/budget.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { InventoryBrandsResolver, InventoryCategoriesResolver, InventoryProductsResolver, InventoryTagsResolver, InventoryVendorsResolver } from './budget/budget.resolvers';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { NgApexchartsModule } from 'ng-apexcharts';
import { FinanceResolver } from './dashboard/finance.resolvers';
import { TranslateModule } from '@ngx-translate/core';
import { ActivityLogComponent } from './activity-log/activity-log.component';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { TableModule } from 'primeng/table';
import { DatePipe } from "@angular/common";
import { MatNativeDateModule } from '@angular/material/core';

const AdminRouting: Route[] = [
    {
        path     : '',
        component: DashboardComponent,
        resolve  : {
            data: FinanceResolver
        }
    },
    {
        path     : 'finance/budget',
        component: BudgetComponent,
        resolve  : {
            brands    : InventoryBrandsResolver,
            categories: InventoryCategoriesResolver,
            products  : InventoryProductsResolver,
            tags      : InventoryTagsResolver,
            vendors   : InventoryVendorsResolver
        }
    },
    {
      path: 'activity-log',
      component: ActivityLogComponent,
      resolve  : {
      }
    }
]

@NgModule({
    declarations: [
        DashboardComponent,
        BudgetComponent,
        ActivityLogComponent
    ],
    imports : [
        RouterModule.forChild(AdminRouting),
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatMenuModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatRippleModule,
        MatSortModule,
        MatSelectModule,
        MatSlideToggleModule,
        MatTooltipModule,
        MatButtonModule,
        MatDividerModule,
        MatIconModule,
        MatMenuModule,
        MatProgressBarModule,
        MatSortModule,
        MatTableModule,
        NgApexchartsModule,
        SharedModule,
        TranslateModule,
        MatDatepickerModule,
        TableModule,
        MatNativeDateModule
    ],
    providers: [DatePipe]
})
export class AdminModule
{
}

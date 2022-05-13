import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';
import { EmptyLayoutComponent } from 'app/layout/layouts/empty/empty.component';
import {  ButtonModule } from 'primeng/button';
import {  ToolbarModule } from 'primeng/toolbar';
import { TranslateModule } from '@ngx-translate/core';
import { PrintLayoutComponent } from './print.component';

@NgModule({
    declarations: [
        PrintLayoutComponent
    ],
    imports     : [
        RouterModule,
        SharedModule,
        ButtonModule,
        ToolbarModule,
        TranslateModule
    ],
    exports     : [
        PrintLayoutComponent
    ]
})
export class PrintLayoutModule
{
}

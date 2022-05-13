import { Component, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Subject } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { AppPrintService } from 'app/shared/services/app-print.service';


@Component({
    selector: 'print-layout',
    templateUrl: './print.component.html',
    styleUrls: ['./print.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [TranslatePipe]
})

export class PrintLayoutComponent implements OnDestroy {

    public getActiveTabIndex: any;

    private _unsubscribeAll: Subject<any> = new Subject<any>();

    constructor(
        private _printService: AppPrintService
    ) { }

    ngOnInit(): void {
        debugger
        this._printService.getActiveTabIndex_value().subscribe(data => {
            this.getActiveTabIndex = data;
            console.log(data)
            if (data.length > -1) {
                setTimeout(() => {
                    this.printToPdf();
                }, 500);
            }
        });
    }


    getActiveTabIndex_value(data: any, goBackPage: string) {
        this.getActiveTabIndex = data, goBackPage;
    }

    printToPdf() {
        this._printService.printContent();
    }

    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }
}
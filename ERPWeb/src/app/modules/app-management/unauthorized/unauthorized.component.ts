import { Component,  OnInit,  ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';


@Component({
    selector: 'app-unauthorized',
    templateUrl: './unauthorized.component.html',
    styleUrls:['./unauthorized.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [TranslatePipe]
})
export class UnauthorizedComponent  implements  OnInit {
    constructor(
        private _translate: TranslatePipe,
       )
        {}
  
    

    ngOnInit(): void {
        
    }
        
}
        
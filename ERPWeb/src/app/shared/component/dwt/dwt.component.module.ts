import { NgModule } from '@angular/core';
import {FileUploadModule} from 'primeng/fileupload';
import { MatDialogModule } from '@angular/material/dialog';
import { DwtComponent } from './dwt.component';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { DataService } from 'app/shared/services/app-data-share.service';
import { TranslateModule } from '@ngx-translate/core';


@NgModule({
  declarations: [
    DwtComponent,
  ],
  imports: [
    DropdownModule,
    FormsModule,
    MatDialogModule,
    TranslateModule
  ],
  providers:[DataService]
})
export class DWTModule { }

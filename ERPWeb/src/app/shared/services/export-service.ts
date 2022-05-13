import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { WebApiService } from "../webApiService";
import { saveAs } from 'file-saver/dist/FileSaver'
import { ExportFileExtensions, ExportMimeTypes, ExportModel } from "../model/export-model";

@Injectable({
    providedIn: 'root',

  })
export class ExportService{
    

    constructor(private _webApiService: WebApiService,
    ) {
    }

    exportFile(exportModel:ExportModel) {
      debugger
        return this._webApiService.postDocument(exportModel._url,exportModel._request).then((res) => {            
              var blob = new Blob([res], {type:exportModel._type == "PDF"?  ExportMimeTypes._pdfMimeType :ExportMimeTypes._excelMimeType});
              saveAs(blob, exportModel._fileName+"_" + exportModel._date + (exportModel._type == "PDF"?ExportFileExtensions._pdf:ExportFileExtensions._excel));
          });
    }
}
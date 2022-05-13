import { Injectable } from "@angular/core";
import { FileSaverService } from "ngx-filesaver";
import * as ExcelJS from "exceljs/dist/exceljs.min.js";

@Injectable({ providedIn: "root" })
export class AppExportService {
     public excelConfig: AppExportExcelConfig
     constructor(private _FileSaverService: FileSaverService
     ) { }

     async exportExcel()   
     {
          debugger
          const workbook = new ExcelJS.Workbook();
          //const sheet = workbook.addWorksheet(this.excelConfig.SheetName, { views: [{ state: 'frozen', xSplit: 1, ySplit: 1 }] });

          const sheet = workbook.addWorksheet(this.excelConfig.SheetName, { views: [{ state: 'frozen',  ySplit: 1 }] });
          var row = 1;
          var excelRow = sheet.getRow(row);
          var cell = 1
          this.excelConfig.HeaderConfig.forEach(item => {
               excelRow.getCell(cell).value = item.HeaderText;
               excelRow.getCell(cell).alignment = { vertical: item.VerticalAlign, horizontal: item.HorizontalAlign };
               excelRow.getCell(cell).font = {
                    size: item.Fontsize,
                    bold: item.Bold
               };
               const dobCol = sheet.getColumn(cell);
               dobCol.width = item.ColumnWidth;
               sheet.getColumn(cell).numFmt = item.ColumnFormat;
               cell++;
          });

          this.excelConfig.ExcelData.forEach(element => {
               row++;
               excelRow = sheet.getRow(row);
               cell = 1;
               for (let key of Object.keys(element)) {
                    excelRow.getCell(cell).value = element[key];
                    cell++;
               }
               
          });

          const buffer = await workbook.xlsx.writeBuffer();
          this.saveAsExcelFile(buffer);
     }
     saveAsExcelFile(buffer: any ): void {
          let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
          const data: Blob = new Blob([buffer], {
               type: EXCEL_TYPE
          });
          this._FileSaverService.save(data, this.excelConfig.FileName);
     }
}

export class AppExportExcelConfig {
     HeaderConfig: AppExportExcelHeaderConfig[];
     SheetName: string = "Sheet 1";
     ExcelData: any[];
     FileName : string = "ERP1.xlsx"
     fontName: string = 'Arial'
}

export class AppExportExcelHeaderConfig {
     HeaderText: string;
     VerticalAlign: AppExportExcelVerticalAlign = AppExportExcelVerticalAlign.Middle;
     HorizontalAlign: AppExportExcelHorizantolAlign = AppExportExcelHorizantolAlign.Left;
     Fontsize: number = 12;
     Bold: boolean = true;
     ColumnWidth: number = 10;
     ColumnFormat: string = "";
}

export enum AppExportExcelVerticalAlign {
     Top = "top",
     Middle ="middle",
     Bottom = "bottom"
}

export enum AppExportExcelHorizantolAlign {
     Left = "left",
     Center = "center",
     Right = "right"
}
export class ExportModel{
    _url: string
    _request: {}
    _fileName: string
    _type: string
    _date:string
}

enum ExcelType{
    PDF,
    EXCEL
}


export const ExportMimeTypes={
    _pdfMimeType :"application/pdf;",
    _excelMimeType : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8;"
}

export const ExportFileExtensions={
    _pdf : ".pdf",
    _excel :".xlsx"
}
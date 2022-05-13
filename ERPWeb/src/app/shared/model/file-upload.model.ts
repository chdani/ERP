export class FileUploadConfig {
    AttachmentId?: number;
    TransactionId?:number;
    DocumentReference?: string;
    MaximumSize?: number;
    MinmumSize?: number;
    AllowedExtns?: string;
    DocTypeRequired?: boolean;
    ReadOnly: boolean=false;
    TransactionType:string;
    ScanEnabled: boolean = false;
    ShowSaveButton: boolean = false;
    FileContent: any = [];
  }

export class DWTModel{
  TransactionId?:number;
  DocType:String;
  TransactionType:string;
}
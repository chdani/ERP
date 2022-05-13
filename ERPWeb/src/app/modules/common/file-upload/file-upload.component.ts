import { Byte } from '@angular/compiler/src/util';
import { AfterViewInit, Component, Injector, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { saveAs } from 'file-saver';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DWTModel, FileUploadConfig } from 'app/shared/model/file-upload.model';
import { FileUpload } from 'primeng/fileupload';
import { forkJoin } from 'rxjs';
import { UploadControlService } from 'app/shared/services/app-file-upload.service';
import { DomSanitizer } from '@angular/platform-browser';
import { DwtComponent } from 'app/shared/component/dwt/dwt.component';
import { ExportMimeTypes } from 'app/shared/model/export-model';
import { DataService } from 'app/shared/services/app-data-share.service';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { i18nMetaToJSDoc } from '@angular/compiler/src/render3/view/i18n/meta';

@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [TranslatePipe],

})
export class FileUploadComponent implements OnInit {

  displayModal: boolean = false;
  fileToUpload: any = [];
  deletedFile = [];
  attachfilecondent = [];
  fileContent: Byte[];

  Attachementform: FormGroup;
  submitted: boolean = false;
  transactionId: any;
  documentTypes: any = [];
  getByIdFiles: any = [];
  docAcceptTypes = "";

  selectedDocType: any;
  disableFileUpload: boolean = true;
  public uploadConfig = new FileUploadConfig();

  constructor(

    private _webApiService: WebApiService,
    private _toastrService: ToastrService,
    private _translate: TranslatePipe,
    private _codeMasterService: CodesMasterService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public uploadService: UploadControlService,
    public sanitizer: DomSanitizer,
    private dialogService: DialogService,
    private dataService: DataService,
    private formBuilder: FormBuilder,
    public _commonService: AppCommonService
  ) {

    this.uploadConfig = config.data;
    this.transactionId = this.uploadConfig.TransactionId;
    this.fileToUpload = this.uploadConfig.FileContent;
  }


  ngOnInit(): void {
    this.Attachementform = this.formBuilder.group({
      id: [''],
      documentType: [''],
      documentTypeDesc: "",
      fileContent: [],
    });
    this.attachmentLink();

    this.dataService.getData().subscribe(message => {
      let blob = null;
      let byte = this._base64ToArrayBuffer(message);
      blob = new Blob([byte], { type: 'application/pdf' });
      blob.lastModifiedDate = new Date();
      blob.name = "Scanned_Pdf";
      blob.Active = "Y";
      blob.attachmentType = 'application/pdf';
      blob.objectURL = this.sanitizer.bypassSecurityTrustUrl((window.URL.createObjectURL(blob)));

      var reader = new FileReader();
      reader.readAsDataURL(blob);
      reader.onloadend = async () => {
        var base64data: any = await reader.result;
        var decodedString = btoa(base64data);
        var filData = {
          name: this.getUniqueFileName(1),
          fileContent: decodedString
        }
        this.addFiledata(filData);
      }
    })
  }

  getUniqueFileName(count: number): string {
    var fileName = "ScannedDocument_" + count + ".pdf";
    this.fileToUpload.forEach(element => {
      if (element.fileName == fileName) {
        count += 1;
        fileName = this.getUniqueFileName(count);
      }
    });
    return fileName;
  }
  scanDialogueref: any;
  scanDocument() {
    let scanConfig: DWTModel = { DocType: this.selectedDocType?.description, TransactionType: this.uploadConfig.TransactionType, TransactionId: this.uploadConfig.TransactionId }
    this.scanDialogueref = this.dialogService.open(DwtComponent, {
      header: "Scan Document",
      width: "700px",
      closable: true,
      contentStyle: { "height": "500px", overflow: "auto" },
      baseZIndex: 500,
      dismissableMask: true,
      data: scanConfig
    });

  }

  async attachmentLink() {
    this.displayModal = true;
    if (this.uploadConfig.DocTypeRequired)
      this.documentTypes = await this._codeMasterService.getCodesDetailByGroupCode(this.uploadConfig.DocumentReference, false, false, this._translate);

      if (  this.uploadConfig.ShowSaveButton  && this.transactionId) {
      var results = await this._webApiService.get("getFileAttachment/" + this.transactionId);
      if (!this.uploadConfig.ShowSaveButton)
        this.fileToUpload = [];
      if (results) {
        this.getByIdFiles = results;
        this.fileToUpload = [];
        this.getByIdFiles.forEach(element => {

          var documenname = this.documentTypes.find(a => a.code == element.documentType);
          this.fileToUpload.push({
            fileName: element.fileName,
            id: element.id,
            transactionId: element.transactionId,
            transactionType: element.transactionType,
            documentType: element.documentType,
            documentTypeDesc: documenname?.description,
            filetype: element.fileType,
            fileContent: element.fileContent,
            active: element.active,
          });
        });
      }
    }
    else {
      if (this.fileToUpload) {
        this.fileToUpload.forEach(app => {
          var documenname = this.documentTypes.find(a => a.code == app.documentType);
          app.documentTypeDesc = documenname?.description;
        })
      }
    }
  }

  choosethefile(files) {
    const file: FileList = files.target.files;
    var fileObject = file.item(0);
    if (this.uploadConfig.MinmumSize == 0 || fileObject.size > this.uploadConfig.MinmumSize) {
      this._toastrService.error(this._translate.transform("APP_FILE_ATTACH_SIZE") + this.uploadConfig.MaximumSize);
      return;
    }
    if (this.uploadConfig.DocTypeRequired && !this.Attachementform.value.documentType.code) {
      this._toastrService.error(this._translate.transform("APP_FILE_ATTACH_DOC_TYPE_REQ"));
      return;
    }

    var blob = new Blob([fileObject], { type: fileObject.type });
    var reader = new FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = async () => {
      var base64data: any = await reader.result;
      var decodedString = btoa(base64data);
      var filData = {
        name: fileObject.name,
        fileContent: decodedString
      }
      this.addFiledata(filData);
    }
    files.target.value = null;
  }

  addFiledata(fileData) {
    if (!this.fileToUpload)
      this.fileToUpload = [];
    this.fileToUpload.push({
      fileName: fileData.name,
      documentType: this.Attachementform.value.documentType.code,
      documentDesc: this.Attachementform.value.documentType.description,
      fileContent: fileData.fileContent,
      transactionType: this.uploadConfig.TransactionType,
      transactionId: this.transactionId,
      id: "",
      active: "Y"
    });

    this.Attachementform.setValue({
      id: "",
      documentType: "",
      documentTypeDesc: "",
      fileContent: "",
    });
    this.selectedDocType = null;
    this.disableFileUpload = this.uploadConfig.DocTypeRequired;
  }
  cancel() {
    this._commonService.updateFileStatus(this.fileToUpload);
    this.ref.close();
  }
  fileinactive(items) {
    if (items.id != null && items.id != "") {
      this.deletedFile.push(items);
      this.deletedFile.forEach(detFile => {
        var deleteRecord = this.fileToUpload.find(a => a.id == detFile.id);
        if (deleteRecord != null) {
          deleteRecord.active = "N";
        }
      });
    }
    else {
      this.fileToUpload.forEach((item, index) => {
        if (item === items) this.fileToUpload.splice(index, 1);
      });
    }

  }
  async dowenload(item) {
    var results = await this._webApiService.get("getFileDowenload/" + item.id);
    if (results)
      var decodedString = atob(results.fileContent);
    var file = decodedString;
    saveAs.saveAs(file, results.fileName);
  }

  FileAttackOK() {
    this._commonService.updateFileStatus(this.fileToUpload);
    this.ref.close();
  }
  async onSubmitLink() {
    this.submitted = true;
    var fileAttachments = [];
    if (this.fileToUpload.length != 0) {
      this.fileToUpload.forEach(element => {
        if (element.id != null && element.active == "N") {
          fileAttachments.push({
            fileName: element.fileName,
            documentType: element.documentType,
            fileContent: "",
            transactionType: element.transactionType,
            filetype: "",
            transactionId: element.transactionId,
            createdBy: element.createdBy,
            createdDate: element.createdDate,
            modifiedBy: element.modifiedBy,
            modifiedDate: element.modifiedDate,
            id: element.id,
            active: element.active,
          })
        }
        if (element.id == "") {
          fileAttachments.push({
            fileName: element.fileName,
            documentType: element.documentType,
            fileContent: element.fileContent,
            transactionType: element.transactionType,
            filetype: "",
            transactionId: element.transactionId,
            createdBy: element.createdBy,
            createdDate: element.createdDate,
            modifiedBy: element.modifiedBy,
            modifiedDate: element.modifiedDate,
            id: element.id,
            active: element.active,
          })
        }
      });
    }
    else {
      this._toastrService.error(this._translate.transform("APP_FILE_SELECT_FILE"));
      return;
    }
    var result = await this._webApiService.post("saveAppDocument", fileAttachments);
    if (result) {
      var output = result as any;
      if (output.status == "DATASAVESUCSS") {
        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
        this.ref.close();
      }
    }

  }
  _base64ToArrayBuffer(base64) {
    var binary_string = window.atob(base64);
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
      bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes.buffer;
  }
}

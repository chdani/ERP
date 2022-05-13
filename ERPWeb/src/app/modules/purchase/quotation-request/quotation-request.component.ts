import { Component, ChangeDetectorRef, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';
import { PurchaseService } from '../purchase.service';
import { FinanceService } from 'app/modules/finance/finance.service';
import { FileUploadConfig } from 'app/shared/model/file-upload.model';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FileUploadComponent } from 'app/modules/common/file-upload/file-upload.component';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
import { ExportModel } from 'app/shared/model/export-model';
import { DatePipe } from '@angular/common';
import { ExportService } from 'app/shared/services/export-service';

@Component({
    selector: 'app-quotation-request',
    templateUrl: './quotation-request.component.html',
    styleUrls: ['./quotation-request.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe, DatePipe,
        ConfirmationService, TranslatePipe
    ],

})

export class QuotationRequestComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _financeService: FinanceService,
        private _purchaseService: PurchaseService,
        private _router: Router,
        private _datePipe: DatePipe,
        private _exportService: ExportService,
        private _toastrService: ToastrService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,
        private dialogService: DialogService,
        private _codeMasterService: CodesMasterService,

    ) {
        super(injector, 'SCR_QUOTATION_REQ', 'allowView', _commonService)
    }
    quotationForm: any;
    quotationModel: any = [];
    productMaster: any = [];
    UnitMaster: any = [];
    vendorMaster: any = [];
    vendordet: any = [];
    Listdetails: any = [];
    ListInfo: any = [];
    quotationDet: any = [];
    gridDetailsContextMenu: MenuItem[] = [];
    gridQuoDetContextMenu: MenuItem[] = [];
    public showOrHideOrgFinyear: boolean = false;
    showApproveReject: boolean = false;
    selectedTrans: any;
    approveRejRemarks: string = "";
    approveRejectHdr: string = "";
    showApproveButton: boolean = false;
    showRejectButton: boolean = false;
    header: any = [];
    uploadConfig: FileUploadConfig;
    addoredit: boolean = false;
    dialogRef: DynamicDialogRef;
    showReqHistory: boolean = false;
    showReqDetHistory: boolean = false;
    userComment: any = {};
    historyInfo: any;
    statuses: any = [];
    index: number = -1;
    lastIndex = -1;
    allHighlight: boolean = false;
    pendHighlight: boolean = false;
    approvedHighlight: boolean = false;
    rejectHighlight: boolean = false;
    selectedStatus: any;
    disableExport: boolean = true;
    exportMenus: MenuItem[];
    lang;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("QUOTATION_REQUEST"));
        this.quotationForm = {
            id: "",
            productMasterId: "",
            transDate: "",
            FromTransDate: "",
            ToTransDate: "",
            remarks: ""
        }
        this.exportMenus = [
            {
                label: this._translate.transform("APP_PDF"),
                icon: 'pi pi-file-pdf',
                command: (event) => { this.export('PDF'); }
            },
            {
                label: this._translate.transform("APP_EXCEL"),
                icon: 'pi pi-file-excel',
                command: (event) => { this.export('EXCEL'); }
            },
        ];
        this.lang = JSON.parse(localStorage.getItem('LUser')).userContext.language;
        this.showHideOrgFinyear('SCR_QUOTATION_REQ');
        this.getSearch();
    }
    export(exportType) {
        this.quotationForm.exportType = exportType;
        this.quotationForm.exportHeaderText = this._translate.transform("QUOTATION_REQUEST");
        var _url = "downloadQuotationRequest";
        var fileDate = this._datePipe.transform(new Date(), "ddMMMyyyy_hhmm");
        let exportModel: ExportModel = {
            _fileName: this._translate.transform("QUOTATION_REQUEST"),
            _request: this.quotationForm,
            _type: exportType,
            _url: _url,
            _date: fileDate
        };
        this._exportService.exportFile(exportModel);
    }
    onClickStatus(status) {
        this.selectedStatus = {};
        if (status != "ALL") {
            this.selectedStatus = this.statuses.find(a => a.code == status);
            this.quotationForm.FromTransDate = "";
            this.quotationForm.ToTransDate = "";
            this.allHighlight = false;
            if (status == "PURTRNSTSREJECTED") {
                this.pendHighlight = false;
                this.approvedHighlight = false;
                this.rejectHighlight = true;
            }
            else if (status == "PURTRNSTSAPPROVED") {
                this.pendHighlight = false;
                this.approvedHighlight = true;
                this.rejectHighlight = false;
            }
            else {
                this.pendHighlight = true;
                this.approvedHighlight = false;
                this.rejectHighlight = false;
            }
        }
        else {
            this.allHighlight = true;
            this.pendHighlight = false;
            this.approvedHighlight = false;
            this.rejectHighlight = false;

            this.quotationForm.FromTransDate = this.plusMinusMonthToCurrentDate(-1);
            this.quotationForm.ToTransDate = new Date();
            this.selectedStatus = this.statuses.find(a => a.code == "");
        }
        this.getSearch();
    }
    showApproveRejectWindow(item: any, status: string) {
        this.showApproveReject = true;
        this.selectedTrans = JSON.parse(JSON.stringify(item));
        this.approveRejRemarks = "";

        if (status == "APPROVE") {
            this.approveRejectHdr = this._translate.transform("APP_APPROVE_FOR_TRANSNO") + item.transNo;
            this.showApproveButton = true;
            this.showRejectButton = false;
        }
        else {
            this.approveRejectHdr = this._translate.transform("APP_REJECT_FOR_TRANSNO") + item.transNo;
            this.showApproveButton = false;
            this.showRejectButton = true;
        }
    }
    async processApproveOrReject(status: string) {
        if (this.approveRejRemarks == "") {
            this._toastrService.error(this._translate.transform("APP_REMARKS_REQ"));
            return;
        }

        if (this.selectedTrans) {

            this.selectedTrans.approverRemarks = this.approveRejRemarks;
            this.selectedTrans.status = status;
            this.selectedTrans.id = this.selectedTrans.id;
            this.selectedTrans.action = 'M';
            var result = await this._webApiservice.post("approveQuotationRequest", this.selectedTrans);
            if (result) {
                var output = result as any;
                if (output.validations == null) {
                    if (output.status == "DATASAVESUCSS") {
                        if (output.referenceId && this.selectedTrans.status == "PURTRNSTSAPPROVED") {
                            var emailRes = await this._webApiservice.get("sendVendorDetailsMail/" + result.referenceId);
                        }
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.getSearch();
                        this.showApproveReject = false;
                        this.showApproveButton = false;
                        this.showRejectButton = false;
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
                else {
                    this._toastrService.error(output.messages[0])
                }
            }

        }

    }
    attachmentLink(data, type, showSaveButton) {
        if (data != "") {

            if (data.transNo != "" && data.transNo != undefined) {
                this.header = this._translate.transform("FILE_ATTACHMENT_FOR_TRANSNO") + data.transNo;
            }
            else if (data.comments == "") {
                this.header = this._translate.transform("FILE_ATTACHMENT_COMMENTS");
            }
            else {
                this.header = this._translate.transform("FILE_ATTACHMENT");
            }
        }
        this.addoredit = data.id ? false : true;
        this.uploadConfig =
        {
            TransactionId: data.id,
            TransactionType: type,
            AllowedExtns: ".png,.jpg,.gif,.jpeg,.bmp,.docx,.doc,.pdf,.msg",
            DocTypeRequired: false,
            DocumentReference: "",
            ReadOnly: false,
            ScanEnabled: this.addoredit,
            ShowSaveButton: !this.addoredit,
            FileContent: []
        };

        this.dialogRef = this.dialogService.open(FileUploadComponent, {
            header: this.header,
            width: "700px",
            closable: false,
            contentStyle: { "height": "500px", overflow: "auto" },
            baseZIndex: 500,
            dismissableMask: true,
            data: this.uploadConfig
        });
    }

    async getSearch() {
        this.statuses = await this._codeMasterService.getCodesDetailByGroupCode("PURTRANSSTATUS", true, false, this._translate);
        this.quotationForm.status = this.selectedStatus?.code;
        this.vendorMaster = await this._financeService.getVendorMaster();
        this.quotationModel = await this._webApiservice.post("getQuotationRequestList", this.quotationForm)
        if (this.quotationModel && this.quotationModel.length > 0) {
            this.disableExport = false;
        }
        else
            this.disableExport = true;

    }
    async listExpentation(event) {
        this.ListInfo = [];
        this.vendordet = [];
        this.Listdetails = [];
        var result = await this._webApiservice.get("getQuotationRequestById/" + event.data.id);
        this.ListInfo = result.quotationReqDet;
        result.quotaReqVendorDets.forEach(element => {
            var vendorMaster = this.vendorMaster.find(a => a.id == element.vendorMasterId);
            this.vendordet.push({
                vendorName: vendorMaster.vendorName, id: vendorMaster.id
            })
        });
        this.Listdetails.push({
            remarks: result.remarks
        })
    }
    getGridQuoDetContextMenu(item) {
        this.gridQuoDetContextMenu = [];
        let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "QUOTATIONREQDET", true) } };
        let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showReqDetHistoryInfo(item) } };
        this.gridQuoDetContextMenu.push(attach);
        this.gridQuoDetContextMenu.push(history);
    }
    async showReqDetHistoryInfo(item) {
        this.showReqHistory = false;
        this.showReqDetHistory = true;
        this.historyInfo = {
            selectedItem: item,
            comments: [],
            appDocument: []
        }
        this.loadQuotationReqDetComments();
        this.loadQuotationDocuments();
        this.loadQuotationReqDetHistory();
    }
    async loadQuotationDocuments() {

        var result = await this._webApiservice.get("getQuotationReqDetAttachments/" + this.historyInfo.selectedItem.id);
        this.historyInfo.appDocument = [];
        result.forEach(element => {
            this.historyInfo.appDocument.push({
                id: element.id,
                fileName: element.fileName,
                createdDate: element.createdDate,
                userName: element.userName,
            })
        });
    }

    async loadQuotationReqDetHistory() {
        var result = await this._webApiservice.get("getQuotationReqDetHistory/" + this.historyInfo.selectedItem.id);
        this.historyInfo.history = [];
        result.forEach(element => {
            this.historyInfo.history.push({
                id: element.id,
                fieldName: this._translate.transform(element.fieldName),
                createdDate: element.createdDate,
                userName: element.userName,
                currentValue: element.currentValue,
                prevValue: element.prevValue
            })
        });
    }

    async saveQuotationReqDetComment() {
        if (this.userComment.comments == "")
            return;

        if (this.uploadConfig && this.uploadConfig.FileContent)
            this.userComment.appDocuments = this.uploadConfig.FileContent

        this.userComment.QuotationReqDetId = this.historyInfo.selectedItem.id;
        this.userComment.active = "Y";
        var result = await this._webApiservice.post("saveQuotationReqDetComment", this.userComment);
        if (result) {
            this.loadQuotationReqDetComments();
            this.loadQuotationDocuments();
            this.userComment.comments = "";
            this.uploadConfig.FileContent = [];

        }
    }

    async loadQuotationReqDetComments() {
        this.historyInfo.comments = [];
        var result = await this._webApiservice.get("getQuotationReqDetComment/" + this.historyInfo.selectedItem.id);

        if (result) {
            result.forEach(element => {
                var attachbtn = false;
                if (element.appDocuments.length != 0) {
                    attachbtn = true;
                }
                this.historyInfo.comments.push({
                    id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
                    attachbtn: attachbtn,
                })
            });
        }
    }

    getGridDetailsContextMenu(item) {
        this.gridDetailsContextMenu = [];
        if (item.status != "PURTRNSTSAPPROVED") {
            if (this.isGranted('SCR_QUOTATION_REQ', this.actionType.allowEdit)) {
                let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditQuotationRequest(item.id) } };
                this.gridDetailsContextMenu.push(edit);
            }
            if (this.isGranted('SCR_QUOTATION_REQ', this.actionType.allowDelete)) {
                let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markQuotationInactive(item) } };
                this.gridDetailsContextMenu.push(Delete);
            }
            if (item.status != "PURTRNSTSREJECTED") {
                if (this.isGranted('SCR_QUOTATION_REQ', this.actionType.allowApprove)) {
                    let approve: MenuItem = { label: this._translate.transform("APP_APPROVE"), icon: 'pi pi-thumbs-up', command: (event) => { this.showApproveRejectWindow(item, 'APPROVE') } };
                    this.gridDetailsContextMenu.push(approve);

                    let reject: MenuItem = { label: this._translate.transform("APP_REJECT"), icon: 'pi pi-thumbs-down', command: (event) => { this.showApproveRejectWindow(item, 'REJECT') } };
                    this.gridDetailsContextMenu.push(reject);
                }
            }
        }
        let attach: MenuItem = { label: this._translate.transform("FILE_ATTACHMENT"), icon: 'pi pi-paperclip', command: (event) => { this.attachmentLink(item, "QUOTATIONREQ", true) } };
        let history: MenuItem = { label: this._translate.transform("APP_HISTORY"), icon: 'pi pi-history', command: (event) => { this.showReqHistoryInfo(item) } };
        this.gridDetailsContextMenu.push(attach);
        this.gridDetailsContextMenu.push(history);
        if (item.status == "PURTRNSTSAPPROVED" && this.isGranted('SCR_QUOTATION_REQ', this.actionType.allowAdd)) {
            let create: MenuItem = { label: this._translate.transform("RESEND_VENDORMAIL"), icon: 'pi pi-envelope', command: (event) => { this.ResendVendorPassword(item) } };
            this.gridDetailsContextMenu.push(create);
        }
        if (item.status == "PURTRNSTSAPPROVED" && this.isGranted('SCR_VENDOR_QUOT', this.actionType.allowAdd)) {
            let create: MenuItem = { label: this._translate.transform("CREATE_VENDORQUTO"), icon: 'pi pi-plus', command: (event) => { this.CreateVendorWuotation(item) } };
            this.gridDetailsContextMenu.push(create);
        }
    }
    async ResendVendorPassword(item) {
        var emailRes = await this._webApiservice.get("sendVendorDetailsMail/" + item.id);
    }
    async CreateVendorWuotation(item) {
        this._router.navigate(["purchase/create-edit-vendor-quotation"], {
            state: { transNo: item.transNo },
        });
    }
    async showReqHistoryInfo(item) {
        this.showReqHistory = true;
        this.showReqDetHistory = false;
        this.historyInfo = {
            selectedItem: item,
            comments: [],
            history: [],
            appDocument: [],
            statusHistory: []
        }
        this.loadQuotationComments();
        this.loadQuotationReqDocuments();
        this.loadQuotationReqStatusHistory();
        this.loadQuotationReqHistory();
    }
    async saveQuotationReqComment() {
        if (this.userComment.comments == "")
            return;

        if (this.uploadConfig && this.uploadConfig.FileContent)
            this.userComment.appDocuments = this.uploadConfig.FileContent;

        this.userComment.quotationRequestId = this.historyInfo.selectedItem.id;
        this.userComment.active = "Y";
        var result = await this._webApiservice.post("saveQuotationRequestComment", this.userComment);
        if (result) {
            this.loadQuotationComments();
            this.loadQuotationReqDocuments();
            this.userComment.comments = "";
            this.uploadConfig.FileContent = [];

        }
    }

    async loadQuotationComments() {
        this.historyInfo.comments = [];

        var result = await this._webApiservice.get("getQuotationReqComment/" + this.historyInfo.selectedItem.id);
        if (result) {
            result.forEach(element => {
                var attachbtn = false;
                if (element.appDocuments.length != 0) {
                    attachbtn = true;
                }
                this.historyInfo.comments.push({
                    id: element.id, Comments: element.comments, CreatedDate: element.createdDate, UserName: element.userName, appDoucument: element.appDoucument,
                    attachbtn: attachbtn,
                })
            });
        }
    }

    async loadQuotationReqDocuments() {

        var result = await this._webApiservice.get("getQuotationReqAttachments/" + this.historyInfo.selectedItem.id);
        this.historyInfo.appDocument = [];
        result.forEach(element => {
            this.historyInfo.appDocument.push({
                id: element.id,
                fileName: element.fileName,
                createdDate: element.createdDate,
                userName: element.userName,
            })
        });
    }

    async loadQuotationReqStatusHistory() {
        if (!this.historyInfo.statusHistory || this.historyInfo.statusHistory.length == 0) {
            var result = await this._webApiservice.get("getQuotationReqStatusHistory/" + this.historyInfo.selectedItem.id);
            this.historyInfo.statusHistory = [];
            result.forEach(element => {
                var status = this.statuses.find(x => x.code == element.status);
                this.historyInfo.statusHistory.push({
                    id: element.id,
                    Status: status?.description,
                    CreatedDate: element.createdDate,
                    UserName: element.userName,
                    Comments: element.comments
                })
            });
        }

    }

    async loadQuotationReqHistory() {
        if (!this.historyInfo.history || this.historyInfo.history.length == 0) {
            var result = await this._webApiservice.get("getQuotationReqHistory/" + this.historyInfo.selectedItem.id);
            this.historyInfo.history = [];
            result.forEach(element => {
                this.historyInfo.history.push({
                    id: element.id,
                    fieldName: this._translate.transform(element.fieldName),
                    createdDate: element.createdDate,
                    userName: element.userName,
                    currentValue: element.currentValue,
                    prevValue: element.prevValue
                })
            });
        }
    }

    async markQuotationInactive(item) {
        this._confirmService.confirm({
            message: this._translate.transform("TRANSNO_FOR") + item.transNo + "<br>" + this._translate.transform("QUOTATION_REQUEST_DELETE_CONF"),
            accept: async () => {
                item.active = "N";
                item.quotationReqDet.forEach(element => {
                    this.quotationDet.push({
                        productMasterId: element.productMasterId, unitMasterId: element.unitMasterId,
                        quantity: element.quantity, remarks: element.remarks, active: "N", createdBy: item.createdBy,
                        createdDate: element.createdDate, modifiedBy: item.modifiedBy, modifiedDate: item.modifiedDate
                    })
                });
                item.quotationReqDet = this.quotationDet;
                var result = await this._webApiservice.post("saveQuotationRequest", item);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.quotationModel = [];
                        this.getSearch();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }

    async createOreditQuotationRequest(data?: any) {
        this._router.navigate(["purchase/create-edit-quotation-request"], {
            state: { quotationId: data },
        });
    }
    addQuotationRequest() {
        this._router.navigate(["purchase/create-edit-quotation-request"], {
            state: { quotationId: "" },
        });
    }
    sidenavClosed() {
        this.index = this.lastIndex;
        this.addoredit = false;
    }
    clearSearchCriteria() {
        this.quotationForm = {
            id: "",
            productMasterId: "",
            transDate: "",
            FromTransDate: this.plusMinusMonthToCurrentDate(-1),
            ToTransDate: new Date(),
            remarks: ""
        }
        this.selectedStatus = this.statuses.find(a => a.code == "");
    }

}

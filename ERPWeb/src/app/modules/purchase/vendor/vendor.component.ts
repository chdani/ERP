import { Component, OnInit, ViewChild, ViewEncapsulation, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { WebApiService } from 'app/shared/webApiService';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { AppCommonService } from 'app/shared/services/app-common.service';
import { TranslatePipe } from '@ngx-translate/core';
import { AppComponentBase } from 'app/shared/component/app-component-base';
import { FinanceService } from 'app/modules/finance/finance.service';
import { PurchaseService } from '../purchase.service';
import { CodesMasterService } from 'app/shared/services/app-codes-master.service';
@Component({
    selector: 'app-vendor',
    templateUrl: './vendor.component.html',
    styleUrls: ['./vendor.component.scss'],
    styles: [],
    encapsulation: ViewEncapsulation.None,
    providers: [
        TranslatePipe,
        ConfirmationService, TranslatePipe
    ],
})
export class VendorComponent extends AppComponentBase implements OnInit {
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _authService: AuthService,
        private _purchaseService: PurchaseService,
        private _codeMasterService: CodesMasterService,
        private _confirmService: ConfirmationService,
        private _translate: TranslatePipe,
        private _router: Router,
        private _toastrService: ToastrService,
        private _finService: FinanceService,
        private _webApiservice: WebApiService,
        public _commonService: AppCommonService,

    ) {
        super(injector, 'SCR_VENDOR_MASTER', 'allowView', _commonService)
    }
    vendorForm: any;
    vendorModel: any = [];
    dataaccount = [];
    productMaster = [];
    PaymentTerm = [];
    vendorcontact: any = [];
    VendorProduct: any = [];
    VendorContract: any = [];
    ledgerAccountData: any = [];
    ledgerAccountFilterData: any = [];
    gridDetailsContextMenu: MenuItem[] = [];
    BasicInfo: any = [];
    BankDetails: any = [];
    vendorlist: any;
    public showOrHideOrgFinyear: boolean = false;
    ngOnInit(): void {
        this._commonService.updatePageName(this._translate.transform("VENDOR_MASTER"));
        this.vendorForm = {
            id: "",
            name: "",
            title: "",
            address1: "",
            address2: "",
            countryName: "",
            mobile: "",
            email: "",
            telephone: "",
            poBox: "",
            bankCountryCode: "",
            bankCode: "",
            ibanSwift: "",
            bankAccName: "",
            bankAccNo: "",
            ledgerCode: "",
            Others: "",
        }
        this.loadDefault();
        this.getSearch();

    }

    async getSearch() {
        this.vendorModel = await this._webApiservice.post('getVendorSerachFilter', this.vendorForm)
    }
    async loadDefault() {
        var input = {
            ledgerCodeFrom: "",
            ledgerCodeTo: ""
        }
        this.ledgerAccountData = await this._webApiservice.post("getLedgerAccountList", input);
        this.ledgerAccountData.forEach(element => {
            this.ledgerAccountFilterData.push({
                ledgercode: element.ledgerCode,
                code: element.ledgerCode + '-' + element.ledgerDesc,
                name: element.ledgerDesc
            })
        });
        this.dataaccount = this.ledgerAccountFilterData;
        var allRecord = {
            id: "",
            ledgerCode: "",
            code: this._translate.transform("APP_ALL")
        }
        this.dataaccount.unshift(allRecord);
        this.productMaster = await this._purchaseService.getProductMaster(false, false, this._translate, "", false, false);
        this.PaymentTerm = await this._codeMasterService.getCodesDetailByGroupCode("PAYMENTTERM", false, false, this._translate);
    }
    async listExpentation(event) {
        this.BankDetails = [];
        this.BasicInfo = [];
        this.vendorcontact = [];
        this.VendorContract = [];
        this.VendorProduct = [];
        this.vendorlist = await this._webApiservice.get("getVendorMasterById/" + event.data.id)
        this.BasicInfo.push({
            poBox: this.vendorlist.poBox, address1: this.vendorlist.address1, address2: this.vendorlist.address2,
            telephone: this.vendorlist.telephone,
        });
        var ledgerCode = this.dataaccount.find(a => a.ledgercode == this.vendorlist.ledgerCode);
        this.BankDetails.push({
            bankCountryCode: this.vendorlist.bankCountryCode, bankCode: this.vendorlist.bankCode, ibanSwift: this.vendorlist.ibanSwifT,
            bankAccName: this.vendorlist.bankAccName, ledgerCode: ledgerCode?.code, bankAccNo: this.vendorlist.bankAccNo,
        });

        this.vendorlist.vendorContacts.forEach(element => {
            this.vendorcontact.push({
                contactName: element.contactName, emailId: element.emailId,
                mobileNo: element.mobileNo
            })
        });

        this.vendorlist.vendorContracts.forEach(element => {
            var payment = this.PaymentTerm.find(a => a.code == element.paymentTerm)
            var ledgerCodes = this.dataaccount.find(a => a.ledgercode == element.ledgerCode);
            if (element.startDate == "0001-01-01T00:00:00")
                element.startDate = "";
            if (element.endDate == "0001-01-01T00:00:00")
                element.endDate = "";
            this.VendorContract.push({
                duration: element.duration, startDate: element.startDate,
                endDate: element.endDate, paymentTerm: payment?.description,
                ledgerCode: ledgerCodes?.code, amountToHold: element.amountToHold,
                description: element.description
            })
        });



        this.vendorlist.vendorProducts.forEach(element => {
            this.VendorProduct.push({
                name: element.productName
            })
        });
        this.vendorModel = this.vendorModel;
    }
    getGridDetailsContextMenu(item) {
        this.gridDetailsContextMenu = [];
        if (this.isGranted('SCR_VENDOR_MASTER', this.actionType.allowEdit)) {
            let edit: MenuItem = { label: this._translate.transform("APP_EDIT"), icon: 'pi pi-pencil', command: (event) => { this.createOreditVendor(item.id) } };
            this.gridDetailsContextMenu.push(edit);
        }
        if (this.isGranted('SCR_VENDOR_MASTER', this.actionType.allowDelete)) {
            let Delete: MenuItem = { label: this._translate.transform("APP_DELETE"), icon: 'pi pi-trash', command: (event) => { this.markVendorInactive(item) } };
            this.gridDetailsContextMenu.push(Delete);
        }
    }

    async markVendorInactive(item) {
        this._confirmService.confirm({
            message: this._translate.transform("VENDOR_MASTER_DELETE_CONF"),
            accept: async () => {
                item.active = "N";
                if (item.vendorContracts) {
                    item.vendorContracts.active = "N"; item.vendorContracts.modifiedBy = item.modifiedBy; item.vendorContracts.modifiedDate = item.modifiedDate;
                }
                if (item.vendorContacts) {
                    item.vendorContacts.active = "N"; item.vendorContacts.modifiedBy = item.modifiedBy; item.vendorContacts.modifiedDate = item.modifiedDate;
                }
                item.vendorProducts.forEach(element => {
                    this.VendorProduct.push({
                        productMasterId: element.productMasterId, active: "N", modifiedBy: item.modifiedBy, modifiedDate: item.modifiedDate,
                    })
                });
                item.vendorProducts = this.VendorProduct;
                var result = await this._webApiservice.post("saveVendor", item);
                if (result) {
                    var output = result as any;
                    if (output.status == "DATASAVESUCSS") {
                        this._toastrService.success(this._translate.transform("APP_SUCCESS"));
                        this.getSearch();
                    }
                    else {
                        this._toastrService.error(output.messages[0])
                    }
                }
            }
        });
    }

    async createOreditVendor(data?: any) {
        this._router.navigate(["purchase/create-edit-vendor"], {
            state: { vendorId: data },
        });
    }
    addVendor() {
        this._router.navigate(["purchase/create-edit-vendor"], {
            state: { vendorId: "" },
        });
    }
    clearSearchCriteria() {
        this.vendorForm = {
            id: "",
            name: "",
            title: "",
            address1: "",
            address2: "",
            countryName: "",
            mobile: "",
            email: "",
            telePhone: "",
            poBox: "",
            bankCountryCode: "",
            bankCode: "",
            ibanSwift: "",
            bankAccName: "",
            bankAccNo: "",
            ledgerCode: "",
            Others: "",
        }

    }

}

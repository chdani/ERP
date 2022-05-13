import { Injectable } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { products } from 'app/mock-api/apps/ecommerce/inventory/data';
import { WebApiService } from 'app/shared/webApiService';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PurchaseService {
    private language: string;
    constructor(private _webApiService: WebApiService) {
        var userData = JSON.parse(localStorage.getItem("LUser")) as any;
        if (userData && userData.userContext)
            this.language = userData.userContext.language;

        if (!this.language) this.language = "en";
    }
    async getProdUnitMaster(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly :boolean ) {
        debugger
        var prodUnitMasters = JSON.parse(localStorage.getItem("appProdUnitMasters"));
        if (!prodUnitMasters || prodUnitMasters.length == 0) {
            var input = { language: this.language };
            var result = await this._webApiService.post("fetchAllProdUnitMasterLangBased", input)
            if (result) {
                prodUnitMasters = result as any;
                localStorage.setItem("appProdUnitMasters", JSON.stringify(prodUnitMasters));
            }
        }
 
        if (activeOnly)
            prodUnitMasters = prodUnitMasters.filter(a => a.active == "Y");
 
        if (includeAll) {
            var allRecord = {
                id: "",
                unitCode: "",
                unitName: translate.transform("APP_ALL")
            }
            prodUnitMasters.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                unitCode: "",
                unitName: translate.transform("SR_UNIT_SELECT")
            }
            prodUnitMasters.unshift(emptyRecord);
        }

        return prodUnitMasters;
    }
    async getVendorMasters(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly :boolean ) {
        var vendorMasters = JSON.parse(localStorage.getItem("appVendorMaster"));
        if (!vendorMasters || vendorMasters.length == 0) {
            var input = { language: this.language };
            var result = await this._webApiService.post("fetchAllVendorMasterLangBased", input)
            if (result) {
                var output = result as any;
                if (output.validations == null) {
                    vendorMasters = output;
                    localStorage.setItem("appVendorMaster", JSON.stringify(output));
                }
            }
        }
 
        if (activeOnly)
        vendorMasters = vendorMasters.filter(a => a.active == "Y");
 
        if (includeAll) {
            var allRecord = {
                id: "",
                title: translate.transform("APP_ALL")
            }
            vendorMasters.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                title: translate.transform("SR_VENDORMASTER_SELECT")
            }
            vendorMasters.unshift(emptyRecord);
        }

        return vendorMasters;
    }




    async getProductCategory(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {
        let productCategoryList = JSON.parse(localStorage.getItem("appProductCategory"));

        if (!productCategoryList || productCategoryList.length == 0) {
            productCategoryList = [];
            var input = { language: this.language };

            var result = await this._webApiService.post("fetchLangBasedDataForProductCategory", input);
            if (result) {
                result.forEach(element => {
                    productCategoryList.push({ code: element.id, name: element.name, active: element.active });
                });
                localStorage.setItem("appProductCategory", JSON.stringify(productCategoryList));
            }
        }

        if (activeOnly)
            productCategoryList = productCategoryList.filter(a => a.active == "Y");
        
        if (includeAll) {
            var allRecord = {
                code: "",
                name: translate.transform("APP_ALL")

            }
            productCategoryList.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                code: "",
                name: translate.transform("SR_CATEGORY_SELECT")
            }
            productCategoryList.unshift(emptyRecord);
        }
        return productCategoryList;
    }
    async getProductSubCategory(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {
    
        let productSubCategoryList = JSON.parse(localStorage.getItem("appProductSubCategory"));

        if (!productSubCategoryList || productSubCategoryList.length == 0) {
            productSubCategoryList = [];
            var input = { language: this.language };

            var result = await this._webApiService.post("fetchLangBasedDataForProductSubCategory", input);
            if (result) {
                result.forEach(element => {
                    productSubCategoryList.push({ id: element.id,prodCategoryId:element.prodCategoryId,code:element.code, name: element.name, active: element.active });
                });
                localStorage.setItem("appProductSubCategory", JSON.stringify(productSubCategoryList));
            }
        }

        if (activeOnly)
        productSubCategoryList = productSubCategoryList.filter(a => a.active == "Y");
        
        if (includeAll) {
            var allRecord = {
                code: "",
                name: translate.transform("APP_ALL")

            }
            productSubCategoryList.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id:"",
                code: "",
                name: translate.transform("SR_SUBCATEGORY_SELECT")
            }
            productSubCategoryList.unshift(emptyRecord);
        }
        return productSubCategoryList;
    }

    async getProductMaster(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, categoryId: string, activeOnly: boolean,isStockable:boolean) {
       
        let productMasters = JSON.parse(localStorage.getItem("appProductMaster"));

        if (!productMasters || productMasters.length == 0) {
            productMasters = [];
            var input = { language: this.language };
            var result = await this._webApiService.post("fetchLangBasedDataForProductMaster", input);
            result.forEach(element => {
                productMasters.push({
                    id: element.id,
                    name: element.prodDescription,
                    categoryId: element.prodCategoryId,
                    prodSubCategoryId:element.prodSubCategoryId,
                    defaultUnitId: element.defaultUnitId,
                    active: element.active,
                    isStockable:element.isStockable,
                    vendorMasterId:element.vendorMasterId
                });
            });
            localStorage.setItem("appProductMaster", JSON.stringify(productMasters));
        }

        if (activeOnly)
            productMasters = productMasters.filter(a => a.active == "Y");
        
        if (categoryId || categoryId != "")
            productMasters = productMasters.filter(a => a.categoryId == categoryId);
        
        if (includeAll) {
            var allRecord = {
                id: "",
                name: translate.transform("APP_ALL")
            }
            productMasters.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                name: translate.transform("SR_PRODUCT_SELECT")
            }
            productMasters.unshift(emptyRecord);
        }
        if(isStockable){
            debugger
            productMasters = productMasters.filter(a=>a.isStockable == true);
        }
        return productMasters;
    }

    async getVendorMaster(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {

        let vendorMasters = JSON.parse(localStorage.getItem("appVendorsList"));

        if (!vendorMasters || vendorMasters.length == 0) {
            
            vendorMasters = await this._webApiService.get("getVendorMasterList");
            vendorMasters.forEach(element => {
                element.name = (element.title == "" ? "" : (element.title + ". ")) + element.name;
            });
            localStorage.setItem("appVendorsList", JSON.stringify(vendorMasters));
        }

        if (activeOnly)
             vendorMasters = vendorMasters.filter(a => a.active == "Y");

        if (includeAll) {
            var allRecord = {
                id: "",
                name: translate.transform("APP_ALL")
            }
            vendorMasters.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                name: translate.transform("GRN_VENDOR_SELECT")
            }
            vendorMasters.unshift(emptyRecord);
        }
        return vendorMasters;
    }

    async getWarehouseLocation(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {

        let locations = JSON.parse(localStorage.getItem("appWareHouseLoc"));

        if (!locations || locations.length == 0) {
            locations = await this._webApiService.get("getWareHouseLocationList");
            localStorage.setItem("appWareHouseLoc", JSON.stringify(locations));
        }

        if (activeOnly)
            locations = locations.filter(a => a.active == "Y");

        if (includeAll) {
            var allRecord = {
                id: "",
                name: translate.transform("APP_ALL")
            }
            locations.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                name: translate.transform("PUR_WAREHOUSE_LOCATION_SELECT")
            }
            locations.unshift(emptyRecord);
        }
        return locations;
    }

    async getWarehouse(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {

        let wareHouses = JSON.parse(localStorage.getItem("appWareHouse"));

        if (!wareHouses || wareHouses.length == 0) {
            wareHouses = await this._webApiService.get("getWareHouseList");
            localStorage.setItem("appWareHouse", JSON.stringify(wareHouses));
        }

        if (activeOnly)
            wareHouses = wareHouses.filter(a => a.active == "Y");

        if (includeAll) {
            var allRecord = {
                id: "",
                name: translate.transform("APP_ALL")
            }
            wareHouses.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                name: translate.transform("PUR_WAREHOUSE_SELECT")
            }
            wareHouses.unshift(emptyRecord);
        }
        return wareHouses;
    }

    async getWarehouseAndLocation(includeAll: boolean, incldueEmptyRow: boolean, translate: TranslatePipe, activeOnly: boolean) {

        let wareHouses = await this.getWarehouse(false, false, translate, activeOnly);
        let locations = await this.getWarehouseLocation(false, false, translate, activeOnly);
        
        locations.forEach(element => {
            var wh = wareHouses.find(a => a.id == element.warehouseId)
            element.name = (wh == null ? "" : wh.name + " - ") + element.name ;
        });
        if (activeOnly)
            locations = locations.filter(a => a.active == "Y");

        if (includeAll) {
            var allRecord = {
                id: "",
                name: translate.transform("APP_ALL")
            }
            locations.unshift(allRecord);
        }
        if (incldueEmptyRow) {
            var emptyRecord = {
                id: "",
                name: translate.transform("PUR_WAREHOUSE_LOCATION_SELECT")
            }
            locations.unshift(emptyRecord);
        }
        return locations;
    }
}
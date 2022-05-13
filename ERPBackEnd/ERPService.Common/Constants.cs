using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common
{
    public static class ERPSettings
    {
        public static string AllowedOrigins => ConfigurationManager.AppSettings["AllowedOrigins"].ToString();

        public static string JWTSecretKey = ConfigurationManager.AppSettings["JWTKey"].ToString();
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        public static string JWTTokenExpiry = ConfigurationManager.AppSettings["JWTExpiration"].ToString();
        public static string PasswordResetLink = ConfigurationManager.AppSettings["PasswordResetLink"].ToString();
        public static string EmailIdSubject = ConfigurationManager.AppSettings["EmailIdSubject"].ToString();
        public static string EmailIdTemplate = ConfigurationManager.AppSettings["EmailIdTemplate"].ToString();
        public static string QuotationRequestMail = ConfigurationManager.AppSettings["QuotationRequestMail"].ToString();
        public static string QuotationRequestSubject = ConfigurationManager.AppSettings["QuotationRequestSubject"].ToString();
        public static string PurchaseOrdMailTemplate = ConfigurationManager.AppSettings["PurchaseOrdMailTemplate"].ToString();
        public static string PurchaseOrderMailSubject = ConfigurationManager.AppSettings["PurchaseOrderMailSubject"].ToString();
        public static string GoodsReceiptNotesMailTemplate = ConfigurationManager.AppSettings["GoodsReceiptNotesMailTemplate"].ToString();
        public static string GoodsReceiptNotesMailSubject = ConfigurationManager.AppSettings["GoodsReceiptNotesMailSubject"].ToString();
        public static class Auth
        {
            public static string Issuer => "oauth2:issuer";
            public static string Audience => "oauth2:audience";
            public static string CertThumbprint => "oauth2:cert-thumbprint";
        }

        public static Dictionary<string, string> APPSYSTEMSETTINGS = new Dictionary<string, string>();

    }

    public static class ERPResponse
    {
        public static KeyValuePair<string, string> REQ_FAILED = new KeyValuePair<string, string>("FAILED", "One or More Error occured");
        public static KeyValuePair<string, string> REQ_SUCCESS = new KeyValuePair<string, string>("SUCCESS", "Data saved sucessfully");
        public static KeyValuePair<string, string> REQ_TLR_DAY_NOT_CLOSED = new KeyValuePair<string, string>("FAILED", "Teller(s) day is not closed.");
        public static KeyValuePair<string, string> REQ_TLR_DAY_NO_BLNCE = new KeyValuePair<string, string>("FAILED", "No Available Balance.");
    }
    public static class ERPExceptions
    {
        public static KeyValuePair<string, string> DB_Dublicate_Entry = new KeyValuePair<string, string>("DUPLICATE", "Duplicate data");
        public static KeyValuePair<string, string> DB_MODIFIED_BY_OTHER_USER = new KeyValuePair<string, string>("NOTLATEST", "Record is modified by some other user");
        public static KeyValuePair<string, string> EXCEPTION_UNHDLD = new KeyValuePair<string, string>("UNHDLDEX", "Unhandled exception : ");
        public static KeyValuePair<string, string> APP_PASSWORD_REQ = new KeyValuePair<string, string>("PASSWORDREQ", "Password must be supplied");
        public static KeyValuePair<string, string> APP_INVALID_CREDENTIALS = new KeyValuePair<string, string>("INVLOGIN", "Invalid user name or password");
        public static KeyValuePair<string, string> APP_MANDATORY_MISSING = new KeyValuePair<string, string>("MANDMISSING", "Please provide all mandatory fields");
        public static KeyValuePair<string, string> APP_RECORD_NOTFOUND = new KeyValuePair<string, string>("NOTFOUND", "Records does not exist");
        public static KeyValuePair<string, string> APP_USER_INACTIVE = new KeyValuePair<string, string>("INACTIVE", "User is not active in the system");
        public static KeyValuePair<string, string> APP_INVALID_OPERATION = new KeyValuePair<string, string>("INVALID_ACTION", "Invalid Operation");
        public static KeyValuePair<string, string> DB_Dublicate_TELEX = new KeyValuePair<string, string>("DUPLICATETELEX", "Duplicate Telex data");

        public static Dictionary<string, string> APP_MESSAGES = new Dictionary<string, string>();
    }
    public static class APPMessageKey
    {
        public const string DUPLICATE = "DUPLICATE";
        public const string NOTLATEST = "NOTLATEST";
        public const string UNHDLDEX = "UNHDLDEX";
        public const string CODEALREADYEXIST = "CODEALREADYEXIST";
        public const string PASSWORDREQ = "PASSWORDREQ";
        public const string INVLOGIN = "INVLOGIN";
        public const string MANDMISSING = "MANDMISSING";
        public const string RECNOTFOUND = "RECNOTFOUND";
        public const string USERINACTIVESYS = "USERINACTIVESYS";
        public const string INVALID_OPERATION = "INVALID_OPERATION";
        public const string DUPLICATETELEX = "DUPLICATETELEX";
        public const string ONEORMOREERR = "ONEORMOREERR";
        public const string DATASAVESUCSS = "DATASAVESUCSS";
        public const string TELLERDAYNOTCLO = "TELLERDAYNOTCLO";
        public const string NODATAAVAILABLE = "NODATAAVAILABLE";
        public const string LEDCODENOTEXIT = "LEDCODENOTEXIT";
        public const string DUPLIBUDLEDCODE = "DUPLIBUDLEDCODE";
        public const string RECEXITBUDLEDCODE = "RECEXITBUDLEDCODE";
        public const string CONTACTHEADTELLER = "CONTACTHEADTELLER";
        public const string NOTALOWRECTRANS = "NOTALOWRECTRANS";
        public const string NOBALPETYCASHACC = "NOBALPETYCASHACC";
        public const string NOSUFFICIENTACC = "NOSUFFICIENTACC";
        public const string NOTALOWHEADTELOPE = "NOTALOWHEADTELOPE";
        public const string INVOPEACCNOBAL = "INVOPEACCNOBAL";
        public const string INVOPEACCNOSUFBAL = "INVOPEACCNOSUFBAL";
        public const string DUPLICODEDES = "DUPLICODEDES";
        public const string RECNOTCODEDES = "RECNOTCODEDES";
        public const string DUPINVNOVENDOR = "DUPINVNOVENDOR";
        public const string PREPAYNOSUFFBAL = "PREPAYNOSUFFBAL";
        public const string EDITNOTPOSTPAYINV = "EDITNOTPOSTPAYINV";
        public const string PLZPROVIDEVALIDDATA = "PLZPROVIDEVALIDDATA";
        public const string PLZPROVIDEEMBDATA = "PLZPROVIDEEMBDATA";
        public const string DUPBOOKNUMBER = "DUPBOOKNUMBER";
        public const string PLZFILLNECESSEMBDET = "PLZFILLNECESSEMBDET";
        public const string PLZFILLNECESSINVDET = "PLZFILLNECESSINVDET";
        public const string PLZSECTSOMEINV = "PLZSECTSOMEINV";
        public const string NOTSUFFBAL = "NOTSUFFBAL";
        public const string CODEORDESMISSING = "CODEORDESMISSING";
        public const string ACCFROM_TOCODELAP = "ACCFROM_TOCODELAP";
        public const string CODEORNAMEMISSING = "CODEORNAMEMISSING";
        public const string HEADTELLERALREADY = "HEADTELLERALREADY";
        public const string NAMEALREDYANOTERTELL = "NAMEALREDYANOTERTELL";
        public const string DUPLICATECODE_NAME = "DUPLICATECODE_NAME";
        public const string CANOTDETASSEMP = "CANOTDETASSEMP";
        public const string CANOTDETASSPARDEP = "CANOTDETASSPARDEP";
        public const string DUBLICATEDATAALREADY = "DUBLICATEDATAALREADY";
        public const string NOREC_EMPDEP = "NOREC_EMPDEP";
        public const string NOREC_EMPEDUCATE = "NOREC_EMPEDUCATE";
        public const string DUPLIQATARIID = "DUPLIQATARIID";
        public const string DUPLIEMPLOYEEID = "DUPLIEMPLOYEEID";
        public const string DATATIMEVALID = "DATATIMEVALID";
        public const string DUPLINAMEALREADY = "DUPLINAMEALREADY";
        public const string SAVEAPPACCERR = "SAVEAPPACCERR";
        public const string GETAPPACCBYIDERR = "GETAPPACCBYIDERR";
        public const string GETAPPACCLISTERR = "GETAPPACCLISTERR";
        public const string SAVEAPPACCROLEMAP = "SAVEAPPACCROLEMAP";
        public const string GETBYAPPACCROMAPBYID = "GETBYAPPACCROMAPBYID";
        public const string GETAPPACCROLMAPLIST = "GETAPPACCROLMAPLIST";
        public const string FAILED = "FAILED";
        public const string SAVEMENUMASTERERR = "SAVEMENUMASTERERR";
        public const string GETMANUMASTERBYIDERR = "GETMANUMASTERBYIDERR";
        public const string GETMANUMASTERLISTERR = "GETMANUMASTERLISTERR";
        public const string RECMODIOTERUSERCODEC = "RECMODIOTERUSERCODEC";
        public const string RECNOTEXITCODEDEC = "RECNOTEXITCODEDEC";
        public const string CODEORDECMISS = "CODEORDECMISS";
        public const string DUPCODEDETCODE_DEC = "DUPCODEDETCODE_DEC";
        public const string RECMODICODETCODEDEC = "RECMODICODETCODEDEC";
        public const string DUPDATACODENAME = "DUPDATACODENAME";
        public const string SECEXP_REGENEMAIL = "SECEXP_REGENEMAIL";
        public const string BCVALIUSERLOGINERR = "BCVALIUSERLOGINERR";
        public const string DUPLIEMAILNAMEALRY = "DUPLIEMAILNAMEALRY";
        public const string BCUSERMASBYIDERR = "BCUSERMASBYIDERR";
        public const string USERNOTFOUDMAIL = "USERNOTFOUDMAIL";
        public const string SAVEUSERROLEERR = "SAVEUSERROLEERR";
        public const string GETUSERROLEBYIDERR = "GETUSERROLEBYIDERR";
        public const string SAVEUSERROLEMAPERR = "SAVEUSERROLEMAPERR";
        public const string GETUSERROLEMAPIDERR = "GETUSERROLEMAPIDERR";
        public const string GETUSERROLEMAPLISERR = "GETUSERROLEMAPLISERR";
        public const string PROUSERIDORLOGINSYS = "PROUSERIDORLOGINSYS";
        public const string NOTDETASSOCIPRODMAS = "NOTDETASSOCIPRODMAS";
        public const string NOTDETROWEXIST = "NOTDETROWEXIST";
        public const string NOSTOCK = "NOSTOCK";
        public const string ALLREADYAPPROVE = "ALLREADYAPPROVE";
        public const string PROVIDEFUTUREDATEGRN = "PROVIDEFUTUREDATEGRN";
    }
    public static class APPSystemsettingsKey
    {
        public const string EMAILSERVERURL = "EMAILSERVERURL";
        public const string EMAILSERVERPORT = "EMAILSERVERPORT";
        public const string ENABLESSLFOREMAIL = "ENABLESSLFOREMAIL";
        public const string SENDEREMAILID = "SENDEREMAILID";
        public const string SENDEREMAILPWD = "SENDEREMAILPWD";
        public const string USR_LANG = "USR_LANG";
    }

    public static class ERPCacheKey
    {
        public const string ACCESS = "ACCESS";
        public const string CODES = "CODESMASTER";
        public const string UNITS = "PRODUNITMASTER";
        public const string MENUS = "MENUS";
        public const string SUBMENUS = "SUBMENUS";
        public const string ROLES = "ROLES";
        public const string LEDGERACC = "LEDGERACCOUNTS";
        public const string LEDGERACCGRP = "LEDGERACCGRP";
        public const string COSTCENTERS = "COSTCENTERS";
        public const string PETTYCASHACCOUNT = "PETTYCASHACC";
        public const string PETTYCASHTELLER = "PETTYCASHTELLER";
        public const string ORGINFO = "ORGINFO";
        public const string EMBASSYMASTER = "EMBASSYMASTER";
        public const string CURRENCYMASTER = "CURRENCYMASTER";
        public const string APPMESSAGE = "APPMESSAGES";
        public const string PRODUCTCATEGORY = "PRODUCTCATEGORY";
        public const string PRODUCTMASTER = "PRODUCTMASTER";
        public const string DEPARTMENT = "DEPARTMENT";
        public const string VENDORMASTER = "VENDORMASTER";
        public const string SYSTEMSETTING = "SYSTEMSETTING";
        public const string PRODUCTSUBCATEGORY = "PRODUCTSUBCATEGORY";
    }

    public static class ERPTransaction
    {
        public const string BUDGET_ALLOCATION = "TRNBUDGTALLOCATION";
        public const string BUDGET_TRANSFER = "TRNBUDGTTRANSFER";
        public const string EMB_POST_PAYMENT = "TRNEMBPOSTPAYMENT";
        public const string EMB_PRE_PAYMENT = "TRNEMBPREPAYMENT";
        public const string DIRECT_INVOICE_POST_PAYMENT = "TRNDIRECTINVPOSTPAY";
        public const string DIRECT_INVOICE_PRE_PAYMENT = "TRNDIRECTINVPREPAY";
        public const string OPENING_BALANCE = "TRNOPENINGBALANCE";
        public const string CLOSING_BALANCE = "TRNCLOSINGBALANCE";
    }

    public static class ExportType
    {
        public const string PDF = "PDF";
        public const string EXCEL = "EXCEL";
    }

    public static class ERPConstants
    {
        public const string DATE_FORMAT = "dd-MM-yyyy";
        public const string DATE_TIME_FORMAT = "dd-MM-yyyy hh:mm";
        public const string DATE_TIME_SECS_FORMAT = "dd-MM-yyyy hh:mm:ss";
        public const string CURRENCY_FORMAT = "#,##0";
        public const string CURRENCY_WITH_DECIMAL_FORMAT = "#,##0.00";
        public const string NUMBER_FORMAT = "###0";
        public const string NUMBER_WITH_DECIMAL_FORMAT = "###0.00";

    }
}
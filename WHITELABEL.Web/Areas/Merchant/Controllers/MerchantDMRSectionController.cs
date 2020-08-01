﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using System.Data.Entity.Core;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Security;
using System.Net;
using System.Text.RegularExpressions;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantDMRSectionController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Merchant Dashboard";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
        //            if (currUser != null)
        //            {
        //                Session["MerchantUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["MerchantUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["MerchantUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;
        //    }
        //    catch (Exception e)
        //    {
        //        //ViewBag.UserName = CurrentUser.UserId;
        //        Console.WriteLine(e.InnerException);
        //        return;
        //    }
        //}
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Merchant";
                if (Session["MerchantUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        [HttpPost]
        public JsonResult GetCustomerInformation(string Custmerid)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //// Fetch Customer
                var FetchCustomer = TransXT_DMR_API.FetchCustomer(Custmerid);
                Session["DMRMobileNo"] = Custmerid;
                //var ResponseResult = JObject.Parse(FetchCustomer);
                //var data = JsonConvert.SerializeObject(FetchCustomer);
                //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer("6290665805");
                return Json(FetchCustomer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetCustomerInformation(POST) Line No:- 90", ex);
                throw ex;
            }
            
        }
        [HttpPost]
        public JsonResult GenerateOTP(string Custmerid)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //// Fetch Customer
                var OTPCall = TransXT_DMR_API.GenerateOTP(Custmerid, "1", "");
                var errmsg = OTPCall.errorMsg.Value;
                if (errmsg == "SUCCESS")
                {
                    return Json("OTP Send to your Mobile no.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(errmsg, JsonRequestBehavior.AllowGet);
                }
                //return Json(OTPCall, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GenerateOTP(POST) Line No:- 118", ex);
                throw ex;
            }

        }

        // GET: Merchant/MerchantDMRSection
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
           
        }

        //public ActionResult CreateCustomer(string CustId="")
        public ActionResult CreateCustomer()
        {
            initpage();
            try
            {
                string dmrid = Session["DMRMobileNo"].ToString();
                if (dmrid != "")
                //if (CustId != "")
                {
                    
                    //var OTPCall = TransXT_DMR_API.GenerateOTP(CustId, "1", "");
                    TransXT_ADDCustomerModal objval = new TransXT_ADDCustomerModal();
                    objval.MobileNumber = dmrid;
                    //objval.MobileNumber = CustId;
                    //objval.OTP = OTPCall.response.otp.Value;
                    return View(objval);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new {area="Merchant" });
                }
                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- CreateCustomer(POST) Line No:- 165", ex);
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostCreateCustomer(TransXT_ADDCustomerModal objval)
        {
            initpage();
            string valuemsg = string.Empty;
            try
            {
                var db = new DBContext();
                DateTime DOB_Val = Convert.ToDateTime(objval.DOB);
                string DOB = DOB_Val.ToString("yyyy-MM-dd");
                var CreateCustomerCall = TransXT_DMR_API.CreateCustomer(objval.MobileNumber, objval.CustomerName, objval.Address, DOB, objval.OTP);
                valuemsg = JsonConvert.SerializeObject(CreateCustomerCall);
                var CustomerInfo = CreateCustomerCall.response.customerId.Value;
                var txnId = CreateCustomerCall.txnId.Value;
                var errmsg = CreateCustomerCall.errorMsg.Value;
                string val = "";
                TBL_DMR_CUSTOMER_DETAILS objAddCust = new TBL_DMR_CUSTOMER_DETAILS()
                {
                    MEM_ID=CurrentMerchant.MEM_ID,
                    CUSTOMER_MOBILE = CustomerInfo,
                    CUSTOMER_NAME = objval.CustomerName,
                    ADDRESS = objval.Address,
                    DOB= DOB,
                    CREATED_DATE=System.DateTime.Now,
                    STATUS=0,
                    TRANSACTIONLIMIT=0
                };
                db.TBL_DMR_CUSTOMER_DETAILS.Add(objAddCust);
                await db.SaveChangesAsync();
                if (errmsg == "SUCCESS")
                {
                    return Json("Customer Created", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(errmsg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostCreateCustomer(POST) Line No:- 209", ex);
                throw ex;
            }
        }
        public ActionResult AddRecipient(string CustId = "")
        {
            initpage();
            try
            {
                if (CustId != "")
                {
                    //var fetchRecipient = TransXT_DMR_API.FetchRecipient("3034265");
                    //var fetchAllRecipient = TransXT_DMR_API.FetchAllRecipient("9659874569");
                    string custid = Decrypt.DecryptMe(CustId);
                    TBL_DMR_RECIPIENT_DETAILS obnRecipient = new TBL_DMR_RECIPIENT_DETAILS();
                    obnRecipient.CUSTOMER_ID = custid;
                    return View(obnRecipient);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- AddRecipient(POST) Line No:- 235", ex);
                throw ex;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostData_AddRecipient(TBL_DMR_RECIPIENT_DETAILS objRecipient)
        {
            initpage();
            string valuemsg = string.Empty;
            try
            {
                var db = new DBContext();
                var checkrecipientlist = db.TBL_DMR_RECIPIENT_DETAILS.Count(x => x.CUSTOMER_ID == objRecipient.CUSTOMER_ID);
                if (checkrecipientlist <= 10)
                {
                    var CreateRecipient = TransXT_DMR_API.AddRecipient(objRecipient.CUSTOMER_ID, objRecipient.ACCOUNT_NO, objRecipient.IFSC_CODE, objRecipient.BENEFICIARY_MOBILE, objRecipient.BENEFICIARY_NAME, "2");
                    string vvv = "";
                    valuemsg = JsonConvert.SerializeObject(CreateRecipient);
                    var Transid = CreateRecipient.txnId.Value;
                    var recipientId = CreateRecipient.response.recipientId.Value;
                    string bal = string.Empty;
                    TBL_DMR_RECIPIENT_DETAILS rectDetails = new TBL_DMR_RECIPIENT_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        CUSTOMER_ID = objRecipient.CUSTOMER_ID,
                        BENEFICIARY_NAME = objRecipient.BENEFICIARY_NAME,
                        ACCOUNT_NO = objRecipient.ACCOUNT_NO,
                        IFSC_CODE = objRecipient.IFSC_CODE,
                        BENEFICIARY_MOBILE = objRecipient.BENEFICIARY_MOBILE,
                        BENEFICIARY_TYPE = "2",
                        CREATE_DATE = System.DateTime.Now,
                        STATUS = 0,
                        TRANSACTIONID = Transid,
                        RECIPIENT_ID = recipientId,
                        ISVERIFIED = 0,
                        EMAIL_ID = objRecipient.EMAIL_ID,
                        REMARKS = objRecipient.REMARKS
                    };
                    db.TBL_DMR_RECIPIENT_DETAILS.Add(rectDetails);
                    await db.SaveChangesAsync();
                    var errmsg = CreateRecipient.errorMsg.Value;
                    if (errmsg == "SUCCESS")
                    {
                        return Json("Beneficiary Added Successfully.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(errmsg, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("You add atleast 10 recipient information.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostData_AddRecipient(POST) Line No:- 286", ex);
                throw ex;
            }

        }

        [HttpGet]
        public PartialViewResult GetAllRecipientLIst(string CustId)
        {
            try
            {
                //string RecipientID = Request.QueryString["CustId"];
                if (CustId != "")
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    //var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.CUSTOMER_ID == CustId && x.STATUS == 0).ToList();

                    var BeneficiaryList11 = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                           where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           select new
                                           {
                                               ID = x.ID,
                                               MEM_ID = x.MEM_ID,
                                               CUSTOMER_ID = x.CUSTOMER_ID,
                                               RECIPIENT_ID = x.RECIPIENT_ID,
                                               BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                               ACCOUNT_NO = x.ACCOUNT_NO,
                                               IFSC_CODE = x.IFSC_CODE,
                                               BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                               CREATE_DATE = x.CREATE_DATE,
                                               BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                               STATUS = x.STATUS,
                                               TRANSACTIONID = x.TRANSACTIONID,
                                               ISVERIFIED = x.ISVERIFIED,
                                               REMARKS = x.REMARKS,
                                               EMAIL_ID = x.EMAIL_ID,
                                               ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "MobileWare").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                           }).ToList();

                    var BeneficiaryList = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                           where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           select new
                                           {
                                               ID = x.ID,
                                               MEM_ID = x.MEM_ID,
                                               CUSTOMER_ID = x.CUSTOMER_ID,
                                               RECIPIENT_ID = x.RECIPIENT_ID,
                                               BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                               ACCOUNT_NO = x.ACCOUNT_NO,
                                               IFSC_CODE = x.IFSC_CODE,
                                               BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                               CREATE_DATE = x.CREATE_DATE,
                                               BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                               STATUS = x.STATUS,
                                               TRANSACTIONID = x.TRANSACTIONID,
                                               ISVERIFIED = x.ISVERIFIED,
                                               REMARKS = x.REMARKS,
                                               EMAIL_ID = x.EMAIL_ID,
                                               ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "MobileWare").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                           }).AsEnumerable().Select(z => new TBL_DMR_RECIPIENT_DETAILS
                                           {
                                               ID = z.ID,
                                               MEM_ID = z.MEM_ID,
                                               CUSTOMER_ID = z.CUSTOMER_ID,
                                               RECIPIENT_ID = z.RECIPIENT_ID,
                                               BENEFICIARY_NAME = z.BENEFICIARY_NAME,
                                               ACCOUNT_NO = z.ACCOUNT_NO,
                                               IFSC_CODE = z.IFSC_CODE,
                                               BENEFICIARY_MOBILE = z.BENEFICIARY_MOBILE,
                                               CREATE_DATE = z.CREATE_DATE,
                                               BENEFICIARY_TYPE = z.BENEFICIARY_TYPE,
                                               STATUS = z.STATUS,
                                               TRANSACTIONID = z.TRANSACTIONID,
                                               ISVERIFIED = z.ISVERIFIED,
                                               REMARKS = z.REMARKS,
                                               EMAIL_ID = z.EMAIL_ID,
                                               ChargedAmount = z.ChargedAmount
                                           }).ToList();


                    return PartialView("GetAllRecipientLIst", BeneficiaryList);
                }
                else
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.STATUS == 0).ToList();
                    return PartialView("GetAllRecipientLIst", BeneficiaryList);
                }
               
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetAllRecipientLIst(GET) Line No:- 305", ex);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteRecipientInformation(string RecipientId,string CustomerId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var DeleteRecipient = TransXT_DMR_API.DeleteRecipient(RecipientId, CustomerId);
                var RecipientList = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefaultAsync();
               
                //var errmsg = "SUCCESS";
                var errmsg = DeleteRecipient.errorMsg.Value;
                if (errmsg == "SUCCESS")
                {
                    RecipientList.STATUS = 1;
                    db.Entry(RecipientList).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new { Result = "true" });
                }
                else
                {
                    return Json(errmsg, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }
            
        }
        [HttpPost]
        public async Task<JsonResult> RecipientEnquiry(string RecipientId, string CustomerId)
        {
            try
            {
                initpage();
                var db = new DBContext();
                var GSTTAX = await db.TBL_TAX_MASTERS.FirstOrDefaultAsync(x => x.TAX_NAME == "GST");
                decimal Walletamt = 0;
                decimal subWaltamt = 0;
                var mastinfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                string MER_BENE_VERIFY_AMT = System.Configuration.ConfigurationManager.AppSettings["MER_BENE_VERIFY_AMT"];
                decimal Mer_BENE_VRY_AMT = 0;
                decimal.TryParse(MER_BENE_VERIFY_AMT, out Mer_BENE_VRY_AMT);

                
                decimal GSTCALCULATED = Math.Round(((Mer_BENE_VRY_AMT * GSTTAX.TAX_VALUE) / 118),2);

                string CUST_BENE_VERIFY_RTN_AMT = System.Configuration.ConfigurationManager.AppSettings["CUST_BENE_VERIFY_RTN_AMT"];
                decimal CUST_BENE_VRY_AMT = 0;
                decimal.TryParse(CUST_BENE_VERIFY_RTN_AMT, out CUST_BENE_VRY_AMT);
                string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                var getRecipientInfo = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefaultAsync();

                var checkAccount = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId && x.ACCOUNT_NO == getRecipientInfo.ACCOUNT_NO).FirstOrDefault();
                if (checkAccount.ISVERIFIED == 0)
                {
                    var RecipientEnquiry = TransXT_DMR_API.RecipientEnquiry(CustomerId, getRecipientInfo.ACCOUNT_NO, getRecipientInfo.IFSC_CODE, "2", RecipientId, "INR", "1");
                    var customerInfo = await db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustomerId).FirstOrDefaultAsync();
                    Walletamt = customerInfo.TRANSACTIONLIMIT;
                    subWaltamt = (Convert.ToDecimal(Walletamt) - 1);
                    var errmsg = RecipientEnquiry.errorMsg.Value;
                    decimal openingAmt = 0;
                    decimal closingAmt = 0;
                    decimal deductedamt = 0;
                    decimal RecipientverifyAmt = 0;
                    decimal UpdateMer_Bal = 0;
                    decimal RecipientMerchantverifyAmt = 0;
                    var Accountverification = db.TBL_ACCOUNT_VERIFICATION_TABLE.FirstOrDefault(x => x.APINAME == "MobileWare");
                    decimal.TryParse(Accountverification.APPLIED_AMT_TO_MERCHANT.ToString(), out RecipientverifyAmt);
                    decimal.TryParse(Accountverification.APPLIED_AMT_TO_MERCHANT.ToString(), out RecipientMerchantverifyAmt);
                    var Acountdetails = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    if (Acountdetails != null)
                    {
                        decimal.TryParse(Acountdetails.OPENING.ToString(), out openingAmt);
                        decimal.TryParse(Acountdetails.CLOSING.ToString(), out closingAmt);
                        //deductedamt = closingAmt - RecipientverifyAmt;
                        deductedamt = closingAmt - Mer_BENE_VRY_AMT;
                        UpdateMer_Bal = Convert.ToDecimal(mastinfo.BALANCE) -(Mer_BENE_VRY_AMT);
                    }
                    decimal WLP_CLOSE = 0;
                    decimal WLP_CLOSE_AMT_Deduction = 0;
                    var WLPAMt = await db.TBL_ACCOUNTS.Where(x=>x.MEM_ID== mastinfo.UNDER_WHITE_LEVEL).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    WLP_CLOSE = WLPAMt.CLOSING;
                    WLP_CLOSE_AMT_Deduction = WLP_CLOSE - Mer_BENE_VRY_AMT;
                    decimal WLPBalUpdate = 0;
                    var WMP_Balance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WLPAMt.MEM_ID);
                    WLPBalUpdate = Convert.ToDecimal(WMP_Balance.BALANCE) - Mer_BENE_VRY_AMT;
                    //var errmsg = "SUCCESS";
                    if (errmsg == "SUCCESS")
                    {
                        getRecipientInfo.ISVERIFIED = 1;
                        getRecipientInfo.WLP_GST_OUTPUT = GSTCALCULATED;
                        getRecipientInfo.MER_GST_INPUT = GSTCALCULATED;
                        getRecipientInfo.TIMESTAMP = DateTime.Now;
                        getRecipientInfo.VERIFY_BENE_CHARGE = Mer_BENE_VRY_AMT;
                        getRecipientInfo.RETURN_BACK_TO_CUST_CHARGE = CUST_BENE_VRY_AMT;
                        db.Entry(getRecipientInfo).State = System.Data.Entity.EntityState.Modified;
                        customerInfo.TRANSACTIONLIMIT = subWaltamt;
                        db.Entry(customerInfo).State = System.Data.Entity.EntityState.Modified;
                        mastinfo.BALANCE = UpdateMer_Bal;
                        db.Entry(mastinfo).State = System.Data.Entity.EntityState.Modified;
                       
                        TBL_ACCOUNTS objMERacnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = CurrentMerchant.MEM_ID,
                            MEMBER_TYPE = "RETAILER",
                            TRANSACTION_TYPE = "Bank Account Verification",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            //AMOUNT = RecipientverifyAmt,
                            AMOUNT = Mer_BENE_VRY_AMT,
                            NARRATION = "Amount Deduction on Bank Account Verification",
                            OPENING = closingAmt,
                            CLOSING = deductedamt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID= sRandomOTP
                        };
                        db.TBL_ACCOUNTS.Add(objMERacnt);

                        TBL_ACCOUNTS WLPObj = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)mastinfo.UNDER_WHITE_LEVEL,
                            MEMBER_TYPE = "WHITE LEVEL",
                            TRANSACTION_TYPE = "Bank Account Verification",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            //AMOUNT = RecipientverifyAmt,
                            AMOUNT = Mer_BENE_VRY_AMT,
                            NARRATION = "Amount Deduction on Bank Account Verification",
                            OPENING = WLP_CLOSE,
                            CLOSING = WLP_CLOSE_AMT_Deduction,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = sRandomOTP
                        };
                        db.TBL_ACCOUNTS.Add(WLPObj);
                        await db.SaveChangesAsync();
                        WMP_Balance.BALANCE = WLPBalUpdate;
                        db.Entry(WMP_Balance).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { Result = "true" });
                    }
                    else
                    {
                        return Json(errmsg, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = "Varified" });
                }
            }
            catch (Exception ex)
            {
                var msg = "Something is going worng";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- RecipientEnquiry(POST) Line No:- 378", ex);
                throw ex;
            }

        }

        public ActionResult GetRecipient()
        {
            initpage();
            return View();
        }
        public ActionResult MoneyTransfer(string CustId="")
        {
            initpage();
            try
            {
                if (CustId != "")
                {
                    ViewBag.GoogleCapchaInt = TransXT_DMR_API.CaptchaSecretKey;
                    string RecipientId = Decrypt.DecryptMe(CustId);
                    var db = new DBContext();
                    var CustomerInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    var recipentInfo = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId).FirstOrDefault();
                    //var recipentInfo = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == "3034264").FirstOrDefault();
                    TransXTDMR_Transaction objTransxt = new TransXTDMR_Transaction();
                    objTransxt.customerId = recipentInfo.CUSTOMER_ID;
                    objTransxt.recSeqId = RecipientId;
                    //objTransxt.recSeqId = "3034264";
                    objTransxt.RecipientName = recipentInfo.BENEFICIARY_NAME;
                    objTransxt.RecipientMobileNo = recipentInfo.BENEFICIARY_MOBILE;
                    objTransxt.RecipientAccountNo = recipentInfo.ACCOUNT_NO;
                    objTransxt.RecipientIFSCCode = recipentInfo.IFSC_CODE;
                    objTransxt.SenderMobileNo = CustomerInfo.MEMBER_MOBILE;
                    objTransxt.SenderName = CustomerInfo.MEMBER_NAME;
                    return View(objTransxt);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }   
                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- MoneyTransfer(POST) Line No:- 415", ex);
                throw ex;
            }
        }
        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> PostTransferAmountToRecipient(TransXTDMR_Transaction objtrns)
        {
            //initpage();
            string valuemsg = string.Empty;
            try
            {
                //CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
                //if (response.Success && ModelState.IsValid)

                string IpAddress = string.Empty;
                if (objtrns.IpAddress != null)
                {
                    IpAddress = objtrns.IpAddress.Replace("\"", "");
                }
                else
                {
                    IpAddress = "";
                }

                bool trigger = true;
                if (trigger == true)
                {
                    decimal balAmt = decimal.Parse(objtrns.amount);
                    var db = new DBContext();
                    var mem = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                    #region Checking Wallet Amount with Transaction amt with 1 % Transaction charge
                    decimal Transactionamt = decimal.Parse(objtrns.amount);
                    string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                    decimal DMRFixedGST = 0;
                    decimal.TryParse(DMRGSTPerValue, out DMRFixedGST);
                    decimal Tranacharge = (Transactionamt * DMRFixedGST) / 100;
                    decimal TotalTransAMt = Transactionamt + Tranacharge;
                    #endregion
                    var AvailableBalcheck = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z=>z.BALANCE).FirstOrDefaultAsync();
                    if (AvailableBalcheck >= TotalTransAMt)
                    {
                        var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                                     join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                                     //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                                     join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                                     where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                                     select new
                                                     {
                                                         WHTBalance = WBL.BALANCE
                                                     }).FirstOrDefaultAsync();
                        if (getWhiteLevelID.WHTBalance >= TotalTransAMt)
                        {
                            var checkbalance = TransXT_DMR_API.FetchCustomer(objtrns.customerId);
                            decimal balAvailable = 0;
                            decimal totalMonthlyLimitValue = 0;
                            var AvailBal = checkbalance.response.walletbal.Value;
                            var totalMonthlyLimit = checkbalance.response.totalMonthlyLimit.Value;
                            decimal.TryParse(AvailBal.ToString(), out balAvailable);
                            
                            totalMonthlyLimit = Convert.ToDecimal(totalMonthlyLimit);

                            if (balAvailable <= totalMonthlyLimit)
                            {
                                string TransAmount = string.Empty;
                                TransAmount = objtrns.amount + ".00";
                                long merchantid = 0;
                                long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                                decimal Trans_AmountVal = 0;
                                decimal.TryParse(TransAmount, out Trans_AmountVal);

                                string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                                bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "DMR", Transactionamt, Tranacharge, Tranacharge, "DMR", "Money Transfer", IpAddress, sRandomOTP, objtrns.recSeqId, objtrns.customerId, "DMT");
                                if (ComCheck == true)
                                {
                                    
                                    var Doremit = TransXT_DMR_API.TransactiondoRemit(objtrns.recSeqId, objtrns.customerId, TransAmount, sRandomOTP, "INR", "1", mem.MEMBER_MOBILE, mem.MEMBER_NAME, objtrns.RecipientMobileNo, objtrns.RecipientName, objtrns.RecipientAccountNo, objtrns.RecipientIFSCCode);
                                    valuemsg = JsonConvert.SerializeObject(Doremit);
                                    var errmsg = Doremit.errorMsg.Value;
                                    
                                    if (errmsg == "SUCCESS")
                                    {
                                        DMR_API_Response objdmrResp = new DMR_API_Response()
                                        {
                                            serviceTax = Doremit.response.serviceTax.Value,
                                            clientRefId = Doremit.response.clientRefId.Value,
                                            fee = Doremit.response.fee.Value,
                                            initiatorId = Doremit.response.initiatorId.Value,
                                            accountNumber = Doremit.response.accountNumber.Value,
                                            txnStatus = Doremit.response.txnStatus.Value,
                                            name = Doremit.response.name.Value,
                                            ifscCode = Doremit.response.ifscCode.Value,
                                            impsRespCode = Doremit.response.impsRespCode.Value,
                                            impsRespMessage = Doremit.response.impsRespMessage.Value,
                                            txnId = Doremit.response.txnId.Value,
                                            timestamp = Doremit.response.timestamp.Value
                                            //serviceTax = "0",
                                            //clientRefId = "W13719201171222669",
                                            //fee = "00",
                                            //initiatorId = "4",
                                            //accountNumber = "31923879504",
                                            //txnStatus = "00",
                                            //name = "Mr  RAHUL  SHARMA",
                                            //ifscCode = "SBIN0001899",
                                            //impsRespCode = "00",
                                            //impsRespMessage = "00",
                                            //txnId = "920117665902",
                                            //timestamp = "20/07/2019 17:16:10"
                                        };

                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "DMR", Trans_AmountVal, Trans_AmountVal, Trans_AmountVal, "DMR", "Money Transfer", IpAddress, sRandomOTP, objdmrResp, valuemsg);
                                        if (checkComm == true)
                                        {
                                            
                                            return Json("Amount transfer successfully.", JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        if (errmsg == "SUCCESS")
                                        {
                                            return Json("Amount transfer successfully.", JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //return Json(errmsg, JsonRequestBehavior.AllowGet);
                                            return Json(errmsg, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        string Statusval = string.Empty;
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        
                                            DMR_API_Response objdmrResp =null;
                                            Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "DMR", "");
                                       
                                       
                                        if (Statusval == "Return Success")
                                        {
                                            return Json(errmsg);
                                        }
                                        else
                                        {
                                            return Json("Try again after sometime.", JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                else
                                {
                                    return Json("Transaction Failed. Plerase try again after sometime.", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                string msg = "This customer id:-" + objtrns.customerId + " has crossed the limit. Please add new customer Id.";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            string msg = "Insufficient amount in White Lable.";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        string msg = "Insuffient balance in Merchant Wallet.";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msg = "Your Google reCaptcha validation failed";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostTransferAmountToRecipient(POST) Line No:- 494", ex);
                throw ex;
            }
        }
        private static string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)
        {
            string sOTP = String.Empty;
            string sTempChars = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                int p = rand.Next(0, saAllowedCharacters.Length);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }

        public ActionResult MoneyTransferList()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpGet]
        public PartialViewResult GetAllTransactionLIst()
        {
            try
            {
                // Only grid query values will be available here.
                var db = new DBContext();
                var BeneficiaryList = db.TBL_TRANSXT_DMR_TRANSACTION_LIST.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("GetAllTransactionLIst", BeneficiaryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GEtAll_TransactionInformation()
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var list = TransXT_DMR_API.SearchTransaction("", "O91519096064152720");
                //var list = TransXT_DMR_API.SearchTransaction("", "");
                var Transactionlist = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID && x.TXN_ID!="").ToList().OrderByDescending(z=>z.TRANSACTION_DATE);
                
                return Json(Transactionlist, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public ActionResult CustomerDetails(string CustId="")
        public ActionResult CustomerDetails()
        {
            initpage();
            try
            {
                string CustId = Session["DMRMobileNo"].ToString();
                if (CustId != "")
                {
                    var db = new DBContext();
                    var FetchCustomer11 = TransXT_DMR_API.FetchCustomer(CustId);
                    var CustomerWallet = FetchCustomer11.response.walletbal.Value;
                    var datainfo = db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustId).FirstOrDefault();
                    var info = db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustId).FirstOrDefault();
                    info.TRANSACTIONLIMIT = decimal.Parse(CustomerWallet);
                    db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var checkbalance = TransXT_DMR_API.FetchCustomer(CustId);
                    decimal balAvailable = 0;
                    var AvailBal = checkbalance.response.walletbal.Value;
                    decimal.TryParse(AvailBal.ToString(), out balAvailable);
                    TransXT_ADDCustomerModal objCustdetails = new TransXT_ADDCustomerModal();
                    objCustdetails.MobileNumber = info.CUSTOMER_MOBILE;
                    objCustdetails.CustomerName = info.CUSTOMER_NAME;
                    objCustdetails.Address = info.ADDRESS;
                    objCustdetails.TRANSACTIONLIMIT = balAvailable.ToString();
                    objCustdetails.DOB = Convert.ToDateTime(info.DOB);
                    return View(objCustdetails);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- CustomerDetails(POST) Line No:- 588", ex);
                throw ex;
            }
        }
        public ActionResult PrintDMRInvoice(string txnid,string RefClientid)
        {
            return View();
        }


        public ActionResult SearchDMRCustomer()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var Db = new DBContext();
                var GEtMoneyRemittenceservice = Db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == CurrentMerchant.MEM_ID && x.SERVICE_ID == 3).FirstOrDefault().ACTIVE_SERVICE;
                ViewBag.DMRServiceStatus = GEtMoneyRemittenceservice;
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        public JsonResult GetDMRCustomerInformation(GetDMRCustomerInfo objsub)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //// Fetch Customer
                var FetchCustomer = TransXT_DMR_API.FetchCustomer(objsub.CustomerMobileNo);
                Session["DMRMobileNo"] = objsub.CustomerMobileNo;
                //var ResponseResult = JObject.Parse(FetchCustomer);
                //var data = JsonConvert.SerializeObject(FetchCustomer);
                //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer("6290665805");
                return Json(FetchCustomer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetCustomerInformation(POST) Line No:- 90", ex);
                throw ex;
            }

        }


        [HttpPost]
        public async Task<JsonResult> GetBeneficiaryInformation(string RecipientId, string CustomerId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var RecipientInformation = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefaultAsync();                
                return Json(RecipientInformation,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }

        }


        [HttpPost]
        public JsonResult PrintTransferAmountInvoice(string txnID,string RefId)
        {            
            try
            {
                var db = new DBContext();
                //var list = TransXT_DMR_API.SearchTransaction("", "O91519096064152720");
                var Transactionlist = (from Comp in db.TBL_MASTER_MEMBER join mem in db.TBL_MASTER_MEMBER on Comp.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                       join dmt in db.TBL_DMR_TRANSACTION_LOGS on mem.MEM_ID equals dmt.MER_ID
                                       join Retailer in db.TBL_MASTER_MEMBER on dmt.MER_ID equals Retailer.MEM_ID
                                       join benRef in db.TBL_DMR_RECIPIENT_DETAILS on dmt.RECIPIENT_ID equals benRef.RECIPIENT_ID
                                       join host in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on mem.UNDER_WHITE_LEVEL equals host.MEM_ID
                                       
                                       where dmt.TXN_ID == txnID && dmt.CLIENT_REF_ID == RefId && dmt.MER_ID == CurrentMerchant.MEM_ID
                                       select new
                                       {
                                           CompanyLogo= Comp.LOGO,
                                           CompanyName= (db.TBL_MASTER_MEMBER.Where(c => c.MEM_ID == CurrentMerchant.MEM_ID).Select(d=>d.COMPANY).FirstOrDefault()),
                                           CompanyMobileNo = (db.TBL_MASTER_MEMBER.Where(c => c.MEM_ID == CurrentMerchant.MEM_ID).Select(x => x.MEMBER_MOBILE).FirstOrDefault()),
                                           CompanyAddress= Retailer.ADDRESS,
                                           BeneficiaryName=dmt.NAME,
                                           //BeneficiaryMobile=(db.TBL_DMR_RECIPIENT_DETAILS.Where(c=>c.RECIPIENT_ID==dmt.RECIPIENT_ID).Select(x=>x.BENEFICIARY_MOBILE).FirstOrDefault()),
                                           BeneficiaryMobile=benRef.BENEFICIARY_MOBILE,
                                           BeneficiaryAccountNo = dmt.ACCOUNT_NO,
                                           BeneficiaryIFSCCode = dmt.IFSC_CODE,
                                           SenderName = dmt.SENDER_NAME,
                                           SenderMObile = dmt.SENDER_MOBILE_NO,
                                           TransferAmt=dmt.TRANSFER_AMT,
                                           TransactionDate=dmt.TRANSACTION_DATE,
                                           PoweredBy= Comp.COMPANY,
                                           CompGST = Comp.COMPANY_GST_NO,
                                           Website = host.DOMAIN,
                                           Email= Comp.EMAIL_ID,
                                           TransactionId=dmt.TXN_ID,
                                           TransactionStatus = dmt.TRANSACTION_STATUS,
                                       }).AsEnumerable().Select(z => new PrintInvoice
                                       {
                                           CompanyLogo =z.CompanyLogo.Remove(0, 1),
                                           CompanyName =z.CompanyName,
                                           CompanyAddress=z.CompanyAddress,
                                           MobileNo=z.CompanyMobileNo,
                                           BeneficiaryName=z.BeneficiaryName,
                                           BeneficiaryMobile=z.BeneficiaryMobile,
                                           BeneficiaryAccountNo=z.BeneficiaryAccountNo,
                                           BeneficiaryIFSCCode=z.BeneficiaryIFSCCode,
                                           SenderName=z.SenderName,
                                           SenderMobile=z.SenderMObile,
                                           TransferAmount=z.TransferAmt,
                                           TransactionDate=z.TransactionDate.ToString(),
                                           PowerBy=z.PoweredBy,
                                           GSTNo=z.CompGST,
                                           Website=z.Website,
                                           EmailID=z.Email,
                                           TransactionId=z.TransactionId,
                                           TransactionStatus = z.TransactionStatus
                                       }).FirstOrDefault();


                return Json(Transactionlist, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
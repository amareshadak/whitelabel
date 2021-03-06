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
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Device.Location;
using WHITELABEL.Web.Controllers;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRechargeServiceController : MerchantBaseController
    {
        // GET: Merchant/MerchantRechargeService
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
                ViewBag.ControllerName = "Merchant";
                
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var checkList = (from user in db.TBL_WHITELABLE_SERVICE
                                 join serv in db.TBL_SETTINGS_SERVICES_MASTER on user.SERVICE_ID equals serv.SLN
                                 where user.MEMBER_ID == CurrentMerchant.MEM_ID
                                 select new ServiceList
                                 {
                                     ServiceName = serv.SERVICE_NAME,
                                     ServiceStatus = user.ACTIVE_SERVICE
                                 }).ToList();
                //ViewBag.ActiveServiceList = checkList;
                var checkoutlet = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(c => c.OUTLETID).FirstOrDefault();
                if (checkoutlet != null)
                {
                    ViewBag.Outletcheck = checkoutlet;
                }
                else
                {
                    ViewBag.Outletcheck = "";
                }
              
                var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID" && x.STATUS==0).OrderBy(c => c.TYPE).ToList();
                ViewBag.operatorList = OperatorList;
                var ElectricityOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "ELECTRICITY" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.ElectricityOperator = ElectricityOperator;
                var WaterOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "WATER" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.WaterOperator = WaterOperator;
                var DTHOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "DTH" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.DTHOperator = DTHOperator;
                var LANDLINEOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "LANDLINE" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.LANDLINEOperator = LANDLINEOperator;
                var BROADBANDOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "BROADBAND" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.BROADBANDOperator = BROADBANDOperator;
                var GasOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "GAS" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
                ViewBag.GasOperator = GasOperator;
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View(checkList);
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

        public ActionResult BindAllMobileOperator()
        {
            initpage();
               var db = new DBContext();
            var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID" && x.STATUS == 0).OrderBy(c => c.TYPE).ToList();
            ViewBag.operatorList = OperatorList;
            return PartialView();
        }

        [HttpPost]
        public JsonResult OpenAllProviderList(string radioValue)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == radioValue).OrderBy(c => c.TYPE).ToList();
                //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                ViewBag.operatorList = OperatorList;
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult OperatorsDetails(string customerId)
        {
            initpage();
            var db = new DBContext();
            var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == customerId).OrderBy(c => c.TYPE).FirstOrDefault();
            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            ViewBag.operatorList = OperatorList;
            return PartialView("_OperatorsDetails", OperatorList);
            //return PartialView("~/Areas/Merchant/Views/MerchantRechargeService/OperatorsDetails.cshtml", OperatorList);
            //return PartialView("~/Merchant/MerchantRechargeService/OperatorsDetails.cshtml", OperatorList);
        }
        //public ActionResult CheckOperator(string EmployeeId)
        //public ActionResult CheckOperator()
        public ActionResult CheckOperator(string OperatorType)
        {
            initpage();
            try
            {
                var db = new DBContext();
                if (OperatorType == "POSTPAID")
                {
                    var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "POSTPAID").OrderBy(c => c.SERVICE_NAME).ToList();
                    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                    ViewBag.operatorList = OperatorList;
                }
                else
                {
                    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "MobileOperator").OrderBy(c => c.SERVICE_NAME).ToList();
                    var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                    ViewBag.operatorList = OperatorList;
                }
                
                return PartialView("CheckOperator");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // Mobile recharge section
        #region Mobile Recharge


        public ActionResult MobileRecharge()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

        }
        public ActionResult GetOperatorName(string query)
        {
            return Json(_GetOperator(query), JsonRequestBehavior.AllowGet);
        }
        private List<Autocomplete> _GetOperator(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            var db = new DBContext();
            try
            {
                var results = (from p in db.TBL_OPERATOR_MASTER
                               where (p.OPERATORNAME).Contains(query)
                               orderby p.OPERATORNAME
                               select p).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete Username = new Autocomplete();
                    //Username.FromUser = string.Format("{0} {1}", r.UName);
                    Username.Name = (r.OPERATORNAME);
                    Username.Id = r.PRODUCTID;

                    people.Add(Username);
                }

            }
            catch (EntityCommandExecutionException eceex)
            {
                if (eceex.InnerException != null)
                {
                    throw eceex.InnerException;
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return people;
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

        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostMobileRecharge(MobileRechargeModel objval)
        {
            //initpage();
            try
            {
                var db = new DBContext();
                var VendorInfo = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == 1);
                string IpAddress = string.Empty;
                if (objval.IpAddress != null)
                {
                    if (objval.IpAddress != "")
                    {
                        IpAddress = objval.IpAddress.Replace("\"", "");
                    }
                    else
                    {
                        IpAddress = "";
                    }
                }
                else
                {
                    IpAddress = "";
                }
                string AccountId_Val = System.Configuration.ConfigurationManager.AppSettings["MOSACCONTID"];
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["OperatorId"];
                        string CircleCode = Request.Form["CircleCodeId"];
                        string CircleName_Value = Request.Form["CircleName"];
                        long merchantid = Convert.ToInt64(Session["MerchantUserId"].ToString());
                        string Scheme = objval.PrepaidRecharge;
                        string schemeCode = string.Empty;
                        string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        if (Scheme == "Prepaid")
                        {
                            #region InstantPay


                            schemeCode = "RR";

                            //string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                            //string sRandomOTP = GenerateRandomOTP(6, saAllowedCharacters);
                            string transType = string.Empty;

                            //string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", IpAddress, sRandomOTP, objval.ContactNo, "InstantPay", "MobileOperator");
                            if (ComCheck == true)
                            {
                                string Outletid = string.Empty;
                                var Prepaidoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                                if (Prepaidoutletid != null)
                                {
                                    Outletid = Prepaidoutletid;
                                }
                                else
                                {
                                    Outletid = "";
                                }
                                var PaymentValidation = PaymentAPI.Validation(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Outletid, objval.ContactNo);
                                if (PaymentValidation == "TXN")
                                {
                                    var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Outletid, objval.ContactNo);
                                    string Rcrgeresp = Convert.ToString(Recharge);

                                    string ErrorDescription = Recharge.status;
                                    string errorcodeValue = Recharge.res_code;
                                    string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;//res.res_code;                            
                                    if (errorcode == "TXN" || errorcode == "TUP")
                                    {
                                        string status = Recharge.status;
                                        string statusCode = Recharge.res_code;
                                        var ipat_id = Recharge.ipay_id.Value;
                                        decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.trans_amt.Value));
                                        //decimal chrg_amt = 0;
                                        //decimal.TryParse(Recharge.charged_amt.Value, out chrg_amt);
                                        //decimal Charged_Amt = decimal.Parse(Convert.ToString(chrg_amt));
                                        decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.charged_amt.Value));
                                        decimal openAmt = 0;
                                        decimal.TryParse(Recharge.opening_bal.Value, out openAmt);
                                        decimal Opening_Balance = decimal.Parse(Convert.ToString(openAmt));
                                        DateTime datevalue = Convert.ToDateTime(Recharge.datetime.Value);
                                        string agentId_Value = Recharge.agent_id;
                                        string Operator_ID = Recharge.opr_id;
                                        string outputval = Recharge.status;
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Prepaid Recharge", IpAddress, sRandomOTP);
                                        //    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", IpAddress, sRandomOTP);
                                        //bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                        if (checkComm == true)
                                        {
                                            var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                            ApiResponse.Ipay_Id = ipat_id;
                                            ApiResponse.AgentId = agentId_Value;
                                            ApiResponse.Opr_Id = Operator_ID;
                                            ApiResponse.AccountNo = objval.ContactNo;
                                            ApiResponse.Sp_Key = Recharge.sp_key;
                                            ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                            ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                            ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                            ApiResponse.DateVal = System.DateTime.Now;
                                            ApiResponse.Status = Recharge.status;
                                            ApiResponse.Res_Code = statusCode;
                                            ApiResponse.res_msg = status;
                                            ApiResponse.RechargeType = objval.PrepaidRecharge;
                                            ApiResponse.RechargeResponse = Rcrgeresp;
                                            ApiResponse.ERROR_TYPE = "SUCCESS";
                                            ApiResponse.ISREVERSE = "Yes";
                                            ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                            db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                            return Json("Transaction Successfull");
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        return Json(outputval);
                                    }
                                    else
                                    {
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                        if (Statusval == "Return Success")
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        else
                                        {
                                            return Json(ErrorDescription);
                                        }
                                        return Json(ErrorDescription);
                                    }
                                }
                                else
                                {
                                    return Json(PaymentValidation);
                                }

                                //var Recharge = MultilinkRechargeAPI.MultiLinkRechargeAPI.PaymentAPI(schemeCode, objval.OperatorName, objval.RechargeAmt.ToString(), objval.ContactNo, CircleCode, sRandomOTP);
                                //string RchgResponse = Convert.ToString(Recharge);
                                //string status_val = Recharge.Status;
                                //string MultilinkResp = string.Empty;
                                //if (status_val == "2" || status_val == "0")
                                //{
                                //    transType = "s";
                                //    MultilinkResp = "SUCCESS";
                                //}
                                //else
                                //{
                                //    transType = "f";
                                //    MultilinkResp = "FAILURE";
                                //}
                                //var ipat_id_Value = Recharge.TransId.Value;                                
                                //string ErrorDescription = Recharge.Status;
                                //string errorcodeValue = Recharge.Message;
                                //if (ErrorDescription == "0" || ErrorDescription == "2")
                                //{
                                //    string status = Recharge.Status;
                                //    var ipat_id = Recharge.TransId.Value;
                                //    string servname = Recharge.ServiceName.Value;
                                //    decimal Balance = decimal.Parse(Convert.ToString(Recharge.Balance.Value));
                                //    decimal Amount = decimal.Parse(Convert.ToString(Recharge.Amount.Value));
                                //    string Message = Recharge.Message.Value;
                                //    string MobileNo = Recharge.MobileNo.Value;
                                //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                //    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", IpAddress, sRandomOTP);
                                //    if (checkComm == true)
                                //    {
                                //        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                //        ApiResponse.Ipay_Id = ipat_id;
                                //        ApiResponse.AgentId = "";
                                //        ApiResponse.Opr_Id = servname;
                                //        ApiResponse.AccountNo = objval.ContactNo;
                                //        ApiResponse.Sp_Key = servname;
                                //        ApiResponse.Trans_Amt = decimal.Parse(Amount.ToString());
                                //        ApiResponse.Charged_Amt = decimal.Parse(Balance.ToString());
                                //        ApiResponse.Opening_Balance = decimal.Parse(Balance.ToString());
                                //        ApiResponse.DateVal = System.DateTime.Now;
                                //        ApiResponse.Status = MultilinkResp;
                                //        ApiResponse.Res_Code = status;
                                //        //ApiResponse.res_msg = Message;
                                //        ApiResponse.res_msg = "Transaction Successful";
                                //        ApiResponse.RechargeType = objval.PrepaidRecharge.Trim() + "-Mobile";
                                //        ApiResponse.RechargeResponse = RchgResponse;
                                //        ApiResponse.ERROR_TYPE = "SUCCESS";
                                //        ApiResponse.ISREVERSE = "Yes";
                                //        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                //        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                //        db.SaveChanges();
                                //        return Json("Transaction Successfull");
                                //    }
                                //    else
                                //    {
                                //        return Json("Transaction Failed");
                                //    }
                                //}
                                //else
                                //{
                                //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                //    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                //    if (Statusval == "Return Success")
                                //    {
                                //        return Json("Transaction Failed");
                                //    }
                                //    else
                                //    {
                                //        return Json(ErrorDescription);
                                //    }
                                //    return Json(errorcodeValue);
                                //}
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }

                            #endregion
                        }
                        else
                        {
                            schemeCode = "PP";
                            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                            if (outletid != null)
                            {
                                //var OperatorKey = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME.Contains(operatorId) && x.TYPE == "POSTPAID").Select(z => z.SERVICE_KEY).FirstOrDefault();
                                var OperatorKey = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME.Contains(OperatorName) && x.TYPE == "POSTPAID").Select(z => z.SERVICE_KEY).FirstOrDefault();

                                CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                                //bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile PostPaid Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Multilink", "POSTPAID");
                                bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Instantpay", Scheme);
                                if (ComCheck == true)
                                {
                                    string GeoLocation = string.Empty;
                                    string geoLoc = string.Empty;
                                    //if (objval.geolocation != "" || objval.geolocation != null)
                                    if (objval.geolocation != null)
                                    {
                                        if (objval.geolocation != "")
                                        {
                                            geoLoc = objval.geolocation.Replace("\"", "");
                                        }
                                        else
                                        {
                                            geoLoc = "";
                                        }
                                    }
                                    else
                                    {
                                        geoLoc = "";
                                    }
                                    var Recharge = BBPSPaymentAPI.BBPSBillPaymentPOSTPAID(operatorId, objval.ContactNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.Reference_Id);
                                    //var Recharge = BBPSPaymentAPI.BBPSBillPaymentPOSTPAID(OperatorKey, objval.ContactNo, objval.ContactNo, objval.RechargeAmt.ToString(), objval.geolocation, outletid, sRandomOTP);
                                    string Rcrgeresp = Convert.ToString(Recharge);

                                    string ErrorDescription = Recharge.status;
                                    string errorcodeValue = Recharge.statuscode;
                                    string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;//res.res_code;                            
                                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                                    {
                                        string status = Recharge.status;
                                        string statusCode = Recharge.statuscode;
                                        var ipat_id = Recharge.data.ipay_id.Value;
                                        decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                        decimal chrg_amt = 0;
                                        decimal.TryParse(Recharge.charged_amt.Value, out chrg_amt);
                                        decimal Charged_Amt = decimal.Parse(Convert.ToString(chrg_amt));
                                        //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.charged_amt.Value));
                                        decimal openAmt = 0;
                                        decimal.TryParse(Recharge.opening_bal.Value, out openAmt);
                                        decimal Opening_Balance = decimal.Parse(Convert.ToString(openAmt));
                                        //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                        //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                        DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                        string agentId_Value = Recharge.data.agent_id;
                                        string Operator_ID = Recharge.data.opr_id;
                                        string outputval = Recharge.status;

                                        //var client = new WebClient();
                                        //var content = client.DownloadString("https://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + Recharge.data.agent_id.Value + "&opr_id=" + Recharge.data.opr_id.Value + "&status=" + status + "&res_code=" + Recharge.statuscode + "&res_msg=" + status + "");
                                        //var callUrlRespResult = JObject.Parse(content);
                                        //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                        //if (urlcalbackresp == "TXN")
                                        //{

                                        //}

                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                        //bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                        if (checkComm == true)
                                        {
                                            var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                            ApiResponse.Ipay_Id = ipat_id;
                                            ApiResponse.AgentId = agentId_Value;
                                            ApiResponse.Opr_Id = Operator_ID;
                                            ApiResponse.AccountNo = objval.ContactNo;
                                            ApiResponse.Sp_Key = Recharge.data.sp_key;
                                            ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                            ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                            ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                            ApiResponse.DateVal = System.DateTime.Now;
                                            ApiResponse.Status = Recharge.data.status;
                                            ApiResponse.Res_Code = statusCode;
                                            ApiResponse.res_msg = status;
                                            ApiResponse.RechargeType = objval.PrepaidRecharge;
                                            ApiResponse.RechargeResponse = Rcrgeresp;
                                            ApiResponse.ERROR_TYPE = "SUCCESS";
                                            ApiResponse.ISREVERSE = "Yes";
                                            ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                            db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                            return Json("Transaction Successfull");
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        return Json(outputval);
                                    }
                                    else
                                    {
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                        if (Statusval == "Return Success")
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        else
                                        {
                                            return Json(ErrorDescription);
                                        }
                                        return Json(ErrorDescription);
                                    }
                                }
                                else
                                {
                                    return Json("Transaction Failed");
                                }
                            }
                            else
                            {
                                string msgval = "Please generate outlet id.. ";
                                return Json(msgval);
                            }
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }


            }
            catch (Exception ex)
            {
                //var msg = Recharge.ipay_errordesc.Value;
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostMobileRecharge(POST) Line No:- 230", ex.InnerException);
                var msg = "Please try again after 15 minute";
                return Json(msg);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix, string OperatorType)
        {
            try
            {
                var db = new DBContext();
                if (OperatorType == "Prepaid")
                {
                    var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                               where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "PREPAID"
                                               select new
                                               {
                                                   //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                                   label = oper.SERVICE_NAME,
                                                   val = oper.SERVICE_KEY,
                                                   image = oper.IMAGE
                                               }).ToListAsync();
                    return Json(OperatorValue);
                }
                else
                {
                    var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                               where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "POSTPAID"
                                               select new
                                               {
                                                   //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                                   label = oper.SERVICE_NAME,
                                                   val = oper.SERVICE_KEY,
                                                   image = oper.IMAGE
                                               }).ToListAsync();
                    return Json(OperatorValue);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> CircleName(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_CIRCLE_OPERATOR
                                           where oper.CIRCLE_NAME.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.CIRCLE_NAME,
                                               val = oper.CIRCLE_CODE
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPostPaidBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey,string Amount)
        {

            var db = new DBContext();

            string OperatorName = Request.Form["txtOperator"];
            string operatorId = Request.Form["OperatorId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            string geoLoc = string.Empty;

            if (GeoLocation != "" || GeoLocation != null)
            {
                geoLoc = GeoLocation.Replace("\"", "");
            }
            else
            {
                geoLoc = "";
            }

            
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());

            

            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentValidation(ServiceKey, MobileNo, MobileNo, Amount, geoLoc, outletid, sRandomOTP);
            string errordesc = PaymentValidation.statuscode;

            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (errordesc == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                //return Json(data, JsonRequestBehavior.AllowGet);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }



        //End Mobile recharge section      
        #endregion

        // DTH Recharge    
        #region DTH Recharge  
        [HttpPost]
        public async Task<JsonResult> DTHCircleName(string prefix)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_CIRCLE_OPERATOR
                                           where oper.CIRCLE_NAME.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.CIRCLE_NAME,
                                               val = oper.CIRCLE_CODE
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> AutoDTHRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "DTHOPERATOR"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoDTHRechargeComplete(POST) Line No:- 293", ex);
                throw ex;
            }
            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                     where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE == "DTH"
            //                     select new
            //                     {
            //                         label = oper.OPERATORNAME + "-" + oper.RECHTYPE,
            //                         val = oper.PRODUCTID
            //                     }).ToList();

            //return Json(OperatorValue);
        }
        public ActionResult DTHRecharge()
        {
            initpage();

           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult POSTDTHRecharge(DTHRechargeModel objval)
        public async Task<JsonResult> POSTDTHRecharge(MobileRechargeModel objval)
        {
            initpage();
            try
            {
                //string OperatorName = Request.Form["DTHOperatorName"];
                //    string operatorId = Request.Form["DTHOperatorId"];
                //const string agentId = "2"; 
                string IpAddress = string.Empty;
                if (objval.IpAddress != null)
                {
                    IpAddress = objval.IpAddress.Replace("\"", "");
                }
                else
                {
                    IpAddress = "";
                }

                string AccountId_Val = System.Configuration.ConfigurationManager.AppSettings["MOSACCONTID"];
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {

                    //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["DTHOperatorId"];
                        string CircleCode = Request.Form["DTHCircleCodeId"];
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);

                        string transType = string.Empty;
                        string Scheme = objval.PrepaidRecharge;
                        //string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                        //string sRandomOTP = GenerateRandomOTP(6, saAllowedCharacters);
                        string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                        bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Multilink", "DTHOPERATOR");
                        //bool checkComm11 = await objChkComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP);
                        if (ComCheck == true)
                        {

                            var Prepaidoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                            var PaymentValidation = PaymentAPI.Validation(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Prepaidoutletid, objval.ContactNo);
                            if (PaymentValidation == "TXN")
                            {
                                var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Prepaidoutletid, objval.ContactNo);
                                string Rcrgeresp = Convert.ToString(Recharge);

                                string ErrorDescription = Recharge.status;
                                string errorcodeValue = Recharge.res_code;
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;//res.res_code;                            
                                if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                                {
                                    string status = Recharge.status;
                                    string statusCode = Recharge.res_code;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.trans_amt.Value));
                                    decimal chrg_amt = 0;
                                    decimal.TryParse(Recharge.charged_amt.Value, out chrg_amt);
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(chrg_amt));
                                    decimal openAmt = 0;
                                    decimal.TryParse(Recharge.opening_bal.Value, out openAmt);
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(openAmt));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.datetime.Value);
                                    string agentId_Value = Recharge.agent_id;
                                    string Operator_ID = Recharge.opr_id;
                                    string outputval = Recharge.status;
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP);
                                    //bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = agentId_Value;
                                        ApiResponse.Opr_Id = Operator_ID;
                                        ApiResponse.AccountNo = objval.ContactNo;
                                        ApiResponse.Sp_Key = Recharge.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.status;
                                        ApiResponse.Res_Code = statusCode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = objval.PrepaidRecharge;
                                        ApiResponse.RechargeResponse = Rcrgeresp;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json(PaymentValidation);
                            }




                            //var Recharge = MultilinkRechargeAPI.MultiLinkRechargeAPI.PaymentAPI("DTH", objval.OperatorName, objval.RechargeAmt.ToString(), objval.ContactNo, CircleCode, sRandomOTP);
                            //string DTHres = Convert.ToString(Recharge);
                            //string ErrorDescription = Recharge.Status;
                            //string errorcodeValue = Recharge.Message;
                            ////if (ErrorDescription == "0" || ErrorDescription == "2")
                            //if (ErrorDescription == "0")
                            //{
                            //    string status = Recharge.Status;
                            //    //string status_val = Recharge.Status;
                            //    if (status == "2" || status == "0")
                            //    {
                            //        transType = "s";
                            //    }
                            //    else
                            //    {
                            //        transType = "f";
                            //    }
                            //    var ipat_id = Recharge.TransId.Value;
                            //    string servname = Recharge.ServiceName.Value;
                            //    decimal Balance = decimal.Parse(Convert.ToString(Recharge.Balance.Value));
                            //    decimal Amount = decimal.Parse(Convert.ToString(Recharge.Amount.Value));
                            //    string Message = Recharge.Message.Value;
                            //    string MobileNo = Recharge.MobileNo.Value;
                            //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                            //    var checkTransactionStatus = MultilinkRechargeAPI.MultiLinkRechargeAPI.MultiLinkTransactionStatusCheck("mytxid", sRandomOTP);
                            //    string TransStatus = checkTransactionStatus.Status;
                            //    string DTHTransactionResp = Convert.ToString(checkTransactionStatus);
                            //    //var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                            //    if (TransStatus == "2")
                            //    {

                            //        DMR_API_Response objdmrResp = new DMR_API_Response()
                            //        {
                            //            serviceTax = Amount.ToString(),
                            //            clientRefId = ipat_id,
                            //            fee = "0",
                            //            initiatorId = ipat_id,
                            //            accountNumber = objval.ContactNo,
                            //            txnStatus = Message,
                            //            name = servname,
                            //            ifscCode = Balance.ToString(),
                            //            impsRespCode = Balance.ToString(),
                            //            impsRespMessage = status,
                            //            txnId = ipat_id,
                            //            timestamp = status
                            //            //serviceTax = "10",
                            //            //clientRefId = "23026041",
                            //            //fee = "00",
                            //            //initiatorId = "23026041",
                            //            //accountNumber = "9903116214",
                            //            //txnStatus = "00",
                            //            //name = "AIRTEL",
                            //            //ifscCode = "10714.41",
                            //            //impsRespCode = "10714.41",
                            //            //impsRespMessage = "00",
                            //            //txnId = "23026041",
                            //            //timestamp = "00"
                            //        };

                            //        //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", objval.IpAddress, sRandomOTP);
                            //        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP, objdmrResp, DTHres);
                            //        if (checkComm == true)
                            //        {
                            //            //var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                            //            //ApiResponse.Ipay_Id = ipat_id;
                            //            //ApiResponse.AgentId = "";
                            //            //ApiResponse.Opr_Id = servname;
                            //            //ApiResponse.AccountNo = objval.ContactNo;
                            //            //ApiResponse.Sp_Key = servname;
                            //            //ApiResponse.Trans_Amt = decimal.Parse(Amount.ToString());
                            //            //ApiResponse.Charged_Amt = decimal.Parse(Balance.ToString());
                            //            //ApiResponse.Opening_Balance = decimal.Parse(Balance.ToString());
                            //            //ApiResponse.DateVal = System.DateTime.Now;
                            //            //ApiResponse.Status = Message;
                            //            //ApiResponse.Res_Code = status;
                            //            //ApiResponse.res_msg = status;
                            //            //ApiResponse.RechargeType = "DTH";
                            //            //ApiResponse.RechargeResponse = DTHTransactionResp;
                            //            //ApiResponse.REC_COMM_TYPE = ApiResponse.REC_COMM_TYPE;
                            //            //ApiResponse.MER_COMM_VALUE = ApiResponse.MER_COMM_VALUE;
                            //            //ApiResponse.MER_COMM_AMT = ApiResponse.MER_COMM_AMT;
                            //            //ApiResponse.MER_TDS_DR_COMM_AMT = ApiResponse.MER_TDS_DR_COMM_AMT;
                            //            //ApiResponse.DIST_COMM_VALUE = ApiResponse.DIST_COMM_VALUE;
                            //            //ApiResponse.DIST_COMM_AMT = ApiResponse.DIST_COMM_AMT;
                            //            //ApiResponse.DIST_TDS_DR_COMM_AMT = ApiResponse.DIST_TDS_DR_COMM_AMT;
                            //            //ApiResponse.SUPER_COMM_VALUE = ApiResponse.SUPER_COMM_VALUE;
                            //            //ApiResponse.SUPER_COMM_AMT = ApiResponse.SUPER_COMM_AMT;
                            //            //ApiResponse.SUPER_TDS_DR_COMM_AMT = ApiResponse.SUPER_TDS_DR_COMM_AMT;
                            //            //ApiResponse.WHITELABEL_VALUE = ApiResponse.WHITELABEL_VALUE;
                            //            //ApiResponse.WHITELABEL_COMM_AMT = ApiResponse.WHITELABEL_COMM_AMT;
                            //            //ApiResponse.WHITELABEL_TDS_DR_COMM_AMT = ApiResponse.WHITELABEL_TDS_DR_COMM_AMT;
                            //            //ApiResponse.TDS_RATE = ApiResponse.TDS_RATE;
                            //            //ApiResponse.COMMISSIONDISBURSEDATE = DateTime.Now;
                            //            //ApiResponse.GST_RATE = ApiResponse.GST_RATE;
                            //            //ApiResponse.MER_COMM_GST_AMT = ApiResponse.MER_COMM_GST_AMT;
                            //            //ApiResponse.DIST_COMM_GST_AMT = ApiResponse.DIST_COMM_GST_AMT;
                            //            //ApiResponse.SUPER_COMM_GST_AMT = ApiResponse.SUPER_COMM_GST_AMT;
                            //            //ApiResponse.WHITELABEL_COMM_GST_AMT = ApiResponse.WHITELABEL_COMM_GST_AMT;
                            //            //ApiResponse.ERROR_TYPE = "SUCCESS";
                            //            //ApiResponse.ISREVERSE = "No";
                            //            //ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                            //            //db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                            //            //db.SaveChanges();
                            //            return Json("Transaction Successfull");
                            //        }
                            //        else
                            //        {
                            //            return Json("Transaction Failed");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        var AgaincheckTransactionStatus = MultilinkRechargeAPI.MultiLinkRechargeAPI.MultiLinkTransactionStatusCheck("mytxid", sRandomOTP);
                            //        string AgainTransStatus = AgaincheckTransactionStatus.Status;
                            //        string DTHAgainTransactionResp = Convert.ToString(AgaincheckTransactionStatus);
                            //        if (AgainTransStatus == "2"  || AgainTransStatus == "0")    
                            //        {
                            //            DMR_API_Response objdmrResp = new DMR_API_Response()
                            //            {
                            //                serviceTax = Amount.ToString(),
                            //                clientRefId = ipat_id,
                            //                fee = "0",
                            //                initiatorId = ipat_id,
                            //                accountNumber = objval.ContactNo,
                            //                txnStatus = Message,
                            //                name = servname,
                            //                ifscCode = Balance.ToString(),
                            //                impsRespCode = Balance.ToString(),
                            //                impsRespMessage = status,
                            //                txnId = ipat_id,
                            //                timestamp = status
                            //                //serviceTax = "10",
                            //                //clientRefId = "23026041",
                            //                //fee = "00",
                            //                //initiatorId = "23026041",
                            //                //accountNumber = "9903116214",
                            //                //txnStatus = "00",
                            //                //name = "AIRTEL",
                            //                //ifscCode = "10714.41",
                            //                //impsRespCode = "10714.41",
                            //                //impsRespMessage = "00",
                            //                //txnId = "23026041",
                            //                //timestamp = "00"
                            //            };
                            //            bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP, objdmrResp, DTHres);
                            //            if (checkComm == true)
                            //            {
                            //                //var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                            //                //ApiResponse.Ipay_Id = ipat_id;
                            //                //ApiResponse.AgentId = "";
                            //                //ApiResponse.Opr_Id = servname;
                            //                //ApiResponse.AccountNo = objval.ContactNo;
                            //                //ApiResponse.Sp_Key = servname;
                            //                //ApiResponse.Trans_Amt = decimal.Parse(Amount.ToString());
                            //                //ApiResponse.Charged_Amt = decimal.Parse(Balance.ToString());
                            //                //ApiResponse.Opening_Balance = decimal.Parse(Balance.ToString());
                            //                //ApiResponse.DateVal = System.DateTime.Now;
                            //                //ApiResponse.Status = Message;
                            //                //ApiResponse.Res_Code = status;
                            //                //ApiResponse.res_msg = status;
                            //                //ApiResponse.RechargeType = "DTH";
                            //                //ApiResponse.RechargeResponse = DTHTransactionResp;
                            //                //ApiResponse.ERROR_TYPE = "SUCCESS";
                            //                //ApiResponse.REC_COMM_TYPE = ApiResponse.REC_COMM_TYPE;
                            //                //ApiResponse.MER_COMM_VALUE = ApiResponse.MER_COMM_VALUE;
                            //                //ApiResponse.MER_COMM_AMT = ApiResponse.MER_COMM_AMT;
                            //                //ApiResponse.MER_TDS_DR_COMM_AMT = ApiResponse.MER_TDS_DR_COMM_AMT;
                            //                //ApiResponse.DIST_COMM_VALUE = ApiResponse.DIST_COMM_VALUE;
                            //                //ApiResponse.DIST_COMM_AMT = ApiResponse.DIST_COMM_AMT;
                            //                //ApiResponse.DIST_TDS_DR_COMM_AMT = ApiResponse.DIST_TDS_DR_COMM_AMT;
                            //                //ApiResponse.SUPER_COMM_VALUE = ApiResponse.SUPER_COMM_VALUE;
                            //                //ApiResponse.SUPER_COMM_AMT = ApiResponse.SUPER_COMM_AMT;
                            //                //ApiResponse.SUPER_TDS_DR_COMM_AMT = ApiResponse.SUPER_TDS_DR_COMM_AMT;
                            //                //ApiResponse.WHITELABEL_VALUE = ApiResponse.WHITELABEL_VALUE;
                            //                //ApiResponse.WHITELABEL_COMM_AMT = ApiResponse.WHITELABEL_COMM_AMT;
                            //                //ApiResponse.WHITELABEL_TDS_DR_COMM_AMT = ApiResponse.WHITELABEL_TDS_DR_COMM_AMT;
                            //                //ApiResponse.TDS_RATE = ApiResponse.TDS_RATE;
                            //                //ApiResponse.COMMISSIONDISBURSEDATE = DateTime.Now;
                            //                //ApiResponse.GST_RATE = ApiResponse.GST_RATE;
                            //                //ApiResponse.MER_COMM_GST_AMT = ApiResponse.MER_COMM_GST_AMT;
                            //                //ApiResponse.DIST_COMM_GST_AMT = ApiResponse.DIST_COMM_GST_AMT;
                            //                //ApiResponse.SUPER_COMM_GST_AMT = ApiResponse.SUPER_COMM_GST_AMT;
                            //                //ApiResponse.WHITELABEL_COMM_GST_AMT = ApiResponse.WHITELABEL_COMM_GST_AMT;
                            //                //ApiResponse.ISREVERSE = "Yes";
                            //                //ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                            //                //db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                            //                //db.SaveChanges();
                            //                return Json("Transaction Successfull");
                            //            }
                            //            else
                            //            {
                            //                return Json("Transaction Failed");
                            //            }
                            //        }
                            //        else
                            //        {
                            //            var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                            //            ApiResponse.RechargeType = "DTH";
                            //            ApiResponse.RechargeResponse = DTHres;
                            //            ApiResponse.ERROR_TYPE = "PENDING";
                            //            ApiResponse.REC_COMM_TYPE = ApiResponse.REC_COMM_TYPE;
                            //            ApiResponse.MER_COMM_VALUE = ApiResponse.MER_COMM_VALUE;
                            //            ApiResponse.MER_COMM_AMT = ApiResponse.MER_COMM_AMT;
                            //            ApiResponse.MER_TDS_DR_COMM_AMT = ApiResponse.MER_TDS_DR_COMM_AMT;
                            //            ApiResponse.DIST_COMM_VALUE = ApiResponse.DIST_COMM_VALUE;
                            //            ApiResponse.DIST_COMM_AMT = ApiResponse.DIST_COMM_AMT;
                            //            ApiResponse.DIST_TDS_DR_COMM_AMT = ApiResponse.DIST_TDS_DR_COMM_AMT;
                            //            ApiResponse.SUPER_COMM_VALUE = ApiResponse.SUPER_COMM_VALUE;
                            //            ApiResponse.SUPER_COMM_AMT = ApiResponse.SUPER_COMM_AMT;
                            //            ApiResponse.SUPER_TDS_DR_COMM_AMT = ApiResponse.SUPER_TDS_DR_COMM_AMT;
                            //            ApiResponse.WHITELABEL_VALUE = ApiResponse.WHITELABEL_VALUE;
                            //            ApiResponse.WHITELABEL_COMM_AMT = ApiResponse.WHITELABEL_COMM_AMT;
                            //            ApiResponse.WHITELABEL_TDS_DR_COMM_AMT = ApiResponse.WHITELABEL_TDS_DR_COMM_AMT;
                            //            ApiResponse.TDS_RATE = ApiResponse.TDS_RATE;
                            //            ApiResponse.COMMISSIONDISBURSEDATE = DateTime.Now;
                            //            ApiResponse.GST_RATE = ApiResponse.GST_RATE;
                            //            ApiResponse.MER_COMM_GST_AMT = ApiResponse.MER_COMM_GST_AMT;
                            //            ApiResponse.DIST_COMM_GST_AMT = ApiResponse.DIST_COMM_GST_AMT;
                            //            ApiResponse.SUPER_COMM_GST_AMT = ApiResponse.SUPER_COMM_GST_AMT;
                            //            ApiResponse.WHITELABEL_COMM_GST_AMT = ApiResponse.WHITELABEL_COMM_GST_AMT;
                            //            ApiResponse.ISREVERSE = "No";
                            //            ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                            //            db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                            //            db.SaveChanges();
                            //            return Json("Transaction Under Process");
                            //        } 
                            //    }
                            //}
                            //else
                            //{
                            //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                            //    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                            //    if (Statusval == "Return Success")
                            //    {
                            //        return Json("Transaction Failed");
                            //    }
                            //    else
                            //    {
                            //        return Json(ErrorDescription);
                            //    }
                            //    return Json(errorcodeValue);
                            //}
                        }
                        else
                        {
                            return Json("Transaction Failed");
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }           
                else
                {
                var msg = "Insufficient balance in master wallet.";
                return Json(msg);
            }
        }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- POSTDTHRecharge(POST) Line No:- 392", ex);
                throw ex;
            }
        }
        #endregion

        // Land Line recharge
        #region Landline Recharge
        public ActionResult LandlineRecharge()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoLandlineRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "LANDLINE"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoLandlineRechargeComplete(POST) Line No:- 421", ex);
                throw ex;
            }
            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                     where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE == "LANDLINE"
            //                     select new
            //                     {
            //                         label = oper.OPERATORNAME + "-" + oper.RECHTYPE,
            //                         val = oper.PRODUCTID
            //                     }).ToList();

            //return Json(OperatorValue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult PostLindlineRecharge(LandlineRecharge objval)
        public async Task<JsonResult> PostLindlineRecharge(LandlineRecharge objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        string IpAddress = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        if (objval.geolocation != null )
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }                            
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["LandlineOperatorId"];
                        const string agentId = "74Y104314";
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "LandLine Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Instantpay", "LANDLINE");
                            if (ComCheck == true)
                            {
                                
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentLANDLINE(operatorId, objval.CustomerNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.LandLineRefId);
                                string LandlineResponse = Convert.ToString(Recharge);
                                string ErrorDescription = Recharge.status;
                                string errorcodeValue = Recharge.statuscode;
                               
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                //string errorcode = string.IsNullOrEmpty(Recharge.res_code.Value) ? Recharge.res_msg.Value : Recharge.res_code.Value;//res.res_code;
                                if (errorcode == "TXN" || errorcode == "TUP" )
                                {
                                    string status = Recharge.status;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;
                                    
                                    DMR_API_Response objLandlineResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.AccountNo,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", objval.IpAddress, sRandomOTP);


                                    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", IpAddress, sRandomOTP, objLandlineResp, LandlineResponse);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = agentId_Value;
                                        ApiResponse.Opr_Id = Operator_ID;
                                        ApiResponse.AccountNo = Recharge.data.account_no;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = statusCode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "LANDLINE";
                                        ApiResponse.RechargeResponse = LandlineResponse;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    //TBL_INSTANTPAY_RECHARGE_RESPONSE insta = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                                    //{
                                    //    Ipay_Id = ipat_id,
                                    //    AgentId = agentId_Value,
                                    //    Opr_Id = Operator_ID,
                                    //    AccountNo = Recharge.data.account_no,
                                    //    Sp_Key = Recharge.data.sp_key,
                                    //    Trans_Amt = trans_amt,
                                    //    Charged_Amt = Charged_Amt,
                                    //    Opening_Balance = Opening_Balance,
                                    //    DateVal = System.DateTime.Now,
                                    //    Status = Recharge.data.status,
                                    //    Res_Code = Recharge.statuscode,
                                    //    res_msg = Recharge.status,
                                    //    Mem_ID = merchantid,
                                    //    RechargeType = "LANDLINE",
                                    //    IpAddress = objval.IpAddress,
                                    //    API_Name = "Instantpay"
                                    //};
                                    //db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(insta);
                                    //await db.SaveChangesAsync();                            
                                    //var client = new WebClient();
                                    //var content = client.DownloadString("http://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + agentId_Value + "&opr_id=" + Operator_ID + "&status=" + status + "&res_code=" + statusCode + "&res_msg=" + status + "");
                                    //var callUrlRespResult = JObject.Parse(content);
                                    //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                    //if (urlcalbackresp == "TXN")
                                    //{
                                    //    string UniqueIdgen = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                    //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //    bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", objval.IpAddress, UniqueIdgen);

                                    //}
                                    //return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }   
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostLindlineRecharge(POST) Line No:- 469", ex);
                throw ex;
            }
        }


        [HttpPost]
        public async Task<JsonResult> GetLandlineBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey,  string CityName,string landlineAmt)
        {
            initpage();
            var db = new DBContext();

            string OperatorName = Request.Form["hfLandlineOperator"];
            string operatorId = Request.Form["LandlineOperatorId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, landlineAmt, GeoLocation, outletid, CityName, sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        #endregion

        // Data Card Recharge
        #region Data Card Recharge
        public ActionResult DatacardRecharge()
        {
            initpage();
            return View();
        }
        #endregion

        #region Broadband segment
        public ActionResult BroadbandRecharge()
        {
            initpage();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AutoBroadbandRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "BROADBAND"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoBroadbandRechargeComplete(POST) Line No:- 510", ex);
                throw ex;
            }
        }
        //[HttpPost]
        //public async Task<JsonResult> GetBillBroadbandInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string Unitno)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtBroadbandOperator"];
        //    string operatorId = Request.Form["broadbandoperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, Unitno, AccountNo, "", GeoLocation, outletid, "", sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetBROADBANDBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string landlineAmt)
        {
            initpage();
            var db = new DBContext();

            //string OperatorName = Request.Form["hfLandlineOperator"];
            //string operatorId = Request.Form["LandlineOperatorId"];
            string OperatorName = Request.Form["broadbandperatorID"];
            string operatorId = Request.Form["broadbandoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, landlineAmt, GeoLocation, outletid, "", sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostBroadbandRecharge(BroadbandViewModel objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmount)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmount <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.geolocation != null)
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["txtBroadbandOperator"];
                        string operatorId = Request.Form["broadbandperatorID"];
                        const string agentId = "74Y104314";
                        string REfID = Request.Form["Broadband_referenceID"];
                        string REfID_Val = Request.Form["BroadbandReferenceId"];
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {

                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmount, objval.RechargeAmount, objval.RechargeAmount, operatorId, "BroadBand Recharge", IpAddress, sRandomOTP, objval.AccountNo, "Instantpay", "BROADBAND");
                            if (ComCheck == true)
                            {
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentLANDLINE(operatorId, objval.CustomerNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.LandLineRefId);
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentBroadBand(operatorId, objval.PhoneNo,  objval.AccountNo, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.BroadbandrefNo);
                                string Broadbandres = Convert.ToString(Recharge);
                                string ErrorDescription = Recharge.ipay_errordesc;
                                string errorcodeValue = Recharge.statuscode;
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                //if (errorcode == "TXN" || errorcode == "TUP")
                                if (errorcode == "TXN" || errorcode == "TUP" || errorcode== "ERR")
                                {
                                    string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //var opr_idVal = Recharge.data.opr_id.Value;
                                    //var RechMsgStatus = Recharge.data.status; 
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.datetime.Value);
                                    //string Operator_ID = Recharge.opr_id.Value;
                                    //string agentId_Value = Recharge.agent_id.Value;
                                    //string statusCode = Recharge.res_code.Value;
                                    //string outputval = Recharge.res_msg;

                                    //string status = Recharge.status;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;


                                    DMR_API_Response objBroadBandResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.AccountNo,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", objval.IpAddress, sRandomOTP);                                    
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "BROADBAND", IpAddress, sRandomOTP, objBroadBandResp, Broadbandres);
                                    //bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, 0, 0, operatorId, "BROADBAND", IpAddress, sRandomOTP, objBroadBandResp, Broadbandres);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = agentId_Value;
                                        ApiResponse.Opr_Id = Operator_ID;
                                        ApiResponse.AccountNo = Recharge.data.account_no;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = statusCode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "BROADBAND";
                                        ApiResponse.RechargeResponse = Broadbandres;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }

                                    //TBL_INSTANTPAY_RECHARGE_RESPONSE insta = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                                    //{
                                    //    Ipay_Id = ipat_id,
                                    //    AgentId = Recharge.agent_id.Value,
                                    //    Opr_Id = Recharge.opr_id.Value,
                                    //    AccountNo = Recharge.account_no.Value,
                                    //    Sp_Key = Recharge.sp_key.Value,
                                    //    Trans_Amt = trans_amt,
                                    //    Charged_Amt = Charged_Amt,
                                    //    Opening_Balance = Opening_Balance,
                                    //    DateVal = System.DateTime.Now,
                                    //    Status = Recharge.status.Value,
                                    //    Res_Code = Recharge.res_code.Value,
                                    //    res_msg = Recharge.res_msg.Value,
                                    //    Mem_ID = merchantid,
                                    //    RechargeType = "BROARDBAND",
                                    //    IpAddress = objval.IpAddress,
                                    //    API_Name = "Instantpay"
                                    //};
                                    //db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(insta);
                                    //await db.SaveChangesAsync();
                                    //var client = new WebClient();
                                    //var content = client.DownloadString("https://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + agentId_Value + "&opr_id=" + Operator_ID + "&status=" + status + "&res_code=" + statusCode + "&res_msg=" + status + "");

                                    //var callUrlRespResult = JObject.Parse(content);
                                    //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                    //if (urlcalbackresp == "TXN")
                                    //{
                                    //    string UniqueIdgen = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                    //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //    bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "DTH Recharge", objval.IpAddress, UniqueIdgen);
                                    //}

                                    return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }

                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostBroadbandRecharge(POST) Line No:- 547", ex);
                throw ex;
            }
        }
        #endregion#

        #region Electricity Recharge section
        public ActionResult ElectricityBillPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoElectricityBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "ELECTRICITY"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoElectricityBillService(POST) Line No:- 576", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string Unitno, string CityName)
        {
            initpage();
            var db = new DBContext();

            string OperatorName = Request.Form["txtElectricityOperator"];
            string operatorId = Request.Form["ElectricityoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, Unitno, AccountNo, "", GeoLocation, outletid, CityName, sRandomOTP);
            
                string errordesc = PaymentValidation.status;
                //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
                if (PaymentValidation.statuscode == "TXN")
                {
                    var data = JsonConvert.SerializeObject(PaymentValidation);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var data = JsonConvert.SerializeObject(PaymentValidation);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            
            

            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostElectricityBill(ElectricityViewModel objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {  //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {

                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.geolocation != null)
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }
                        }
                        else
                        {
                            geoLoc = "";
                        }

                        string OperatorName = Request.Form["txtElectricityOperator"];
                        string operatorId = Request.Form["ElectricityoperId"];

                        string REfID = Request.Form["ELECreferenceID"];
                        string REfID_Val = Request.Form["REfferenceElecId"];
                        const string agentId = "74Y104314";
                        long merchantid = CurrentMerchant.MEM_ID;
                        var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        string option9 = objval.geolocation + "|" + Pincode;
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "ELECTRICITY", IpAddress, sRandomOTP, objval.CustomerId, "Instantpay", "ELECTRICITY");
                            if (ComCheck == true)
                            {
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentELECTRICITY(operatorId, objval.MobileNo, objval.BillUnit, objval.CustomerId, objval.RechargeAmt.ToString(), geoLoc, outletid, REfID_Val, objval.City, sRandomOTP);
                                string Electricityres = Convert.ToString(Recharge);
                                string ErrorDescription = Recharge.ipay_errordesc;
                                string errorcode = string.IsNullOrEmpty(Recharge.statuscode.Value) ? Recharge.status.Value : Recharge.statuscode.Value;//res.res_code;
                                if (errorcode == "TXN" || errorcode == "TUP" || errorcode== "ERR")
                                {
                                    string status = Recharge.status;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var RechMsgStatus = Recharge.data.status;
                                    var opr_idVal = Recharge.data.opr_id;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string outputval = Recharge.status;


                                    DMR_API_Response objElectricityResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.CustomerId,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "ELECTRICITY Bill", IpAddress, sRandomOTP, objElectricityResp, Electricityres);
                                    
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        ApiResponse.AccountNo = Recharge.data.account_no;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "ELECTRICITY";
                                        ApiResponse.RechargeResponse = Electricityres;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                }
                                else if (errorcode == "ERR")
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostElectricityBill(POST) Line No:- 613", ex);
                throw ex;
            }
        }


        #endregion

        #region Gass Bill Payment
        public ActionResult GasBillPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoGasBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "GAS"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoGasBillService(POST) Line No:- 646", ex);
                throw ex;
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> GetGasBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtGassServiceOperator"];
        //    string operatorId = Request.Form["GassServiceOperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, "", GeoLocation, outletid, "", sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetGasBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string GasBillAmt)
        {
            initpage();
            var db = new DBContext();

            //string OperatorName = Request.Form["hfLandlineOperator"];
            //string operatorId = Request.Form["LandlineOperatorId"];
            string OperatorName = Request.Form["GassServiceOperId"];
            string operatorId = Request.Form["GassServiceOperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, GasBillAmt, GeoLocation, outletid, "", sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostGasBillPayment(GasBillPaymentViewModel objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmount)
                { //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();

                    if (objval.RechargeAmount <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            IpAddress = objval.IpAddress.Replace("\"", "");
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        if (objval.geolocation != "" || objval.geolocation != null)
                        {
                            geoLoc = objval.geolocation.Replace("\"", "");
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["txtGassServiceOperator"];
                        string operatorId = Request.Form["GassServiceOperId"];
                        const string agentId = "74Y104314";
                        string REfID = Request.Form["Gas_referenceID"];
                        string REfID_Val = Request.Form["GassReferenceId"];

                        long merchantid = CurrentMerchant.MEM_ID;
                        var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        string option9 = objval.geolocation + "|" + Pincode;
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmount, objval.RechargeAmount, objval.RechargeAmount, operatorId, "GAS", IpAddress, sRandomOTP, objval.CustomerID, "Instantpay", "GAS");
                            if (ComCheck == true)
                            {
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentGASBILL(operatorId, objval.ContactNo, objval.CustomerID, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.GassReferenceNo);
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentGASBILL(operatorId, objval.ContactNo, "", objval.CustomerID, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.GassReferenceNo, "", sRandomOTP);
                                string GASRespo = Convert.ToString(Recharge);
                                //string ErrorDescription = Recharge.ipay_errordesc;
                                //string errorcode = string.IsNullOrEmpty(Recharge.statuscode.Value) ? Recharge.status.Value : Recharge.statuscode.Value;//res.res_code;
                                string ErrorDescription = Recharge.ipay_errordesc;
                                string errorcodeValue = Recharge.statuscode;
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                if (errorcode == "TXN" || errorcode == "TUP" )
                                {
                                    string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    //string outputval = Recharge.status;


                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;


                                    DMR_API_Response ObjGassBillRes = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.CustomerID,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "GAS Bill", IpAddress, sRandomOTP, ObjGassBillRes, GASRespo);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        ApiResponse.AccountNo = Recharge.data.account_no.Value;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key.Value;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "GAS";
                                        ApiResponse.RechargeResponse = GASRespo;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }  
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostGasBillPayment(POST) Line No:- 682", ex);
                throw ex;
            }
            //try
            //{
            //    string OperatorName = Request.Form["txtGassServiceOperator"];
            //    string operatorId = Request.Form["GassServiceOperId"];
            //    //const string agentId = "2";
            //    //var PaymentValidation = PaymentAPI.Validation(agentId, objval.RechargeAmount.ToString(), operatorId, objval.CustomerID);
            //    //if (PaymentValidation == "TXN")
            //    //{
            //    //    //var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), "ATP", objval.ContactNo);
            //    //    var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmount.ToString(), operatorId, objval.CustomerID);
            //    //    if (Recharge == "TXN")
            //    //    {
            //    //        var ipat_id = Recharge;
            //    //        Session["msgCheck"] = "Transaction Successful";
            //    //    }
            //    //    else
            //    //    {
            //    //        Session["msgCheck"] = Recharge;
            //    //    }
            //    //    return Json(Recharge);
            //    //}
            //    //else
            //    //{
            //    //    return Json(PaymentValidation);
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostGasBillPayment(POST) Line No:- 682", ex);
            //    throw ex;
            //}


        }
        #endregion

        #region Insurance Payment
        public ActionResult InsurancePayment()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoInsuranceService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "INSURANCE"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoInsuranceService(POST) Line No:- 713", ex);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PostInsurancerecharge(InsuranceViewModel objval)
        {
            try
            {
                string OperatorName = Request.Form["txtInsuranceServiceOperator"];
                string operatorId = Request.Form["InsuranceOperId"];
                const string agentId = "2";
                var PaymentValidation = PaymentAPI.Validation(agentId, objval.PolicyAmount.ToString(), operatorId, objval.PolicyNo,"","");
                if (PaymentValidation == "TXN")
                {
                    //var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), "ATP", objval.ContactNo);
                    var Recharge = PaymentAPI.Payment(agentId, objval.PolicyAmount.ToString(), operatorId, objval.PolicyNo, "", "");
                    if (Recharge == "TXN")
                    {
                        var ipat_id = Recharge;
                        Session["msgCheck"] = "Transaction Successful";
                    }
                    else
                    {
                        Session["msgCheck"] = Recharge;
                    }
                    return Json(Recharge);
                }
                else
                {
                    return Json(PaymentValidation);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostInsurancerecharge(POST) Line No:- 750", ex);
                throw ex;
            }

        }
        #endregion

        #region Recharge Details list
        public ActionResult RechargeTransactionList()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            //return View();
        }
        public PartialViewResult PartialRechargeInfoList()
        {
            var db = new DBContext();
            //var Mem_ID = long.Parse(CurrentMerchant.MEM_ID);
            //var RechargeTransaction = db.TBL_MULTILINK_API_RESPONSE.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
            var RechargeTransaction = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Where(x => x.Mem_ID == CurrentMerchant.MEM_ID).ToList();
            return PartialView("PartialRechargeInfoList", RechargeTransaction);
            //return PartialView(DisplayRechargeransaction());
        }
        public FileResult ExportRechargeTransactionList()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_INSTANTPAY_RECHARGE_RESPONSE> grid = DisplayRechargeransaction();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_INSTANTPAY_RECHARGE_RESPONSE> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                //return File(package.GetAsByteArray(), "application/unknown");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        private IGrid<TBL_INSTANTPAY_RECHARGE_RESPONSE> DisplayRechargeransaction()
        {
            try
            {
                var db = new DBContext();

                var Mem_ID = long.Parse(Session["MerchantUserId"].ToString());
                var RechargeTransaction = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Where(x => x.Mem_ID == Mem_ID).ToList();

                ////var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0  && x.MEM_ID==MemberCurrentUser.MEM_ID).ToList();
                //var bankdetails = db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.IsActive == 0 && x.RemitterID == remitterid).ToList();

                IGrid<TBL_INSTANTPAY_RECHARGE_RESPONSE> grid = new Grid<TBL_INSTANTPAY_RECHARGE_RESPONSE>(RechargeTransaction);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.Ipay_Id).Titled("IPAY_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.AgentId).Titled("AGENT_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Opr_Id).Titled("OPR_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.AccountNo).Titled("ACCOUNT_NO.").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Sp_Key).Titled("SERVICE PROVIDER KEY").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Trans_Amt).Titled("TRANS_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Charged_Amt).Titled("CHARGED_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Opening_Balance).Titled("CLOSING BAL.").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.DateVal).Titled("RECHARGE DATE").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true).MultiFilterable(true);
                grid.Columns.Add(model => model.RechargeType).Titled("RECHARGE TYPE").Filterable(true).Sortable(true).MultiFilterable(true);
                //grid.Columns.Add(model => model.Status).Titled("STATUS").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ID).Titled("STATUS").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<label class='label " + (model.Status == "SUCCESS" ? "label-success" : model.Status == "FAILED" ? "label-danger" : "label-info") + "'> " + model.Status + " </label>");
                grid.Columns.Add(model => model.res_msg).Titled("RECHARGE STATUS").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RechargeType).Titled("RECHARGE TYPE").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeActivateBeneficiary(" + model.ID + ");return 0;'>DELETE</a>");
                grid.Pager = new GridPager<TBL_INSTANTPAY_RECHARGE_RESPONSE>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 1000000;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;


            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- DisplayRechargeransaction(POST) Line No:- 823", ex);
                throw ex;
            }

        }
        #endregion


        #region water Bill Payment
        public ActionResult WaterSupplyPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoWaterBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "WATER"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoElectricityBillService(POST) Line No:- 576", ex);
                throw ex;
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> GetWaterBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtWaterSupplyOperator"];
        //    string operatorId = Request.Form["WaterSupplyoperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentWaterBillValidation(Service_key, MobileNo, "", AccountNo, "11", GeoLocation, outletid, sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetWaterBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string WaterBillAmt)
        {
            initpage();
            var db = new DBContext();

            //string OperatorName = Request.Form["hfLandlineOperator"];
            //string operatorId = Request.Form["LandlineOperatorId"];
            string OperatorName = Request.Form["txtWaterSupplyOperator"];
            string operatorId = Request.Form["WaterSupplyoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, WaterBillAmt, GeoLocation, outletid, "", sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostWaterSypplyBillPayment(WaterSupplyPaymentModel objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();

                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            IpAddress = objval.IpAddress.Replace("\"", "");
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        if (objval.geolocation != "" || objval.geolocation != null)
                        {
                            geoLoc = objval.geolocation.Replace("\"", "");
                        }
                        else
                        {
                            geoLoc = "";
                        }

                        string OperatorName = Request.Form["txtWaterSupplyOperator"];
                        string operatorId = Request.Form["WaterSupplyoperId"];
                        const string agentId = "74Y104314";

                        string Ref_ID = Request.Form["Water_referenceID"];
                        string Ref_IDValue = Request.Form["Water_ref_Name"];

                        long merchantid = CurrentMerchant.MEM_ID;
                        //long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                        var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        string option9 = objval.geolocation + "|" + Pincode;

                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "WATER", IpAddress, sRandomOTP, objval.CustomerId, "Instantpay", "WATER");
                            if (ComCheck == true)
                            {
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentWATER(operatorId, objval.MobileNo, objval.CustomerId, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.WaterRefNo);
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentWaterBill(operatorId, objval.MobileNo, objval.BillUnit, objval.CustomerId, objval.RechargeAmt.ToString(), objval.geolocation, outletid, objval.Water, sRandomOTP);
                                string WATERRespo = Convert.ToString(Recharge);
                                //string ErrorDescription = Recharge.ipay_errordesc;
                                //string errorcode = string.IsNullOrEmpty(Recharge.statuscode.Value) ? Recharge.status.Value : Recharge.statuscode.Value;//res.res_code
                                string ErrorDescription = Recharge.ipay_errordesc;
                                string errorcodeValue = Recharge.statuscode;
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                if (errorcode == "TXN" || errorcode == "TUP")
                                {
                                    string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    //string outputval = Recharge.status;

                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;

                                    DMR_API_Response objWaterResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.CustomerId,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "GAS Bill", objval.IpAddress, sRandomOTP);
                                    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "WATER Bill", IpAddress, sRandomOTP, objWaterResp, WATERRespo);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        ApiResponse.AccountNo = Recharge.data.account_no.Value;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key.Value;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "WATER";
                                        ApiResponse.RechargeResponse = WATERRespo;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }

                                    //TBL_INSTANTPAY_RECHARGE_RESPONSE insta = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                                    //{
                                    //    Ipay_Id = ipat_id,
                                    //    AgentId = Recharge.data.agent_id.Value,
                                    //    Opr_Id = Recharge.data.opr_id.Value,
                                    //    AccountNo = Recharge.data.account_no.Value,
                                    //    Sp_Key = Recharge.data.sp_key.Value,
                                    //    Trans_Amt = decimal.Parse(trans_amt.ToString()),
                                    //    Charged_Amt = decimal.Parse(Charged_Amt.ToString()),
                                    //    Opening_Balance = decimal.Parse(Opening_Balance.ToString()),
                                    //    DateVal = System.DateTime.Now,
                                    //    Status = Recharge.data.status.Value,
                                    //    Res_Code = Recharge.statuscode,
                                    //    res_msg = Recharge.status,
                                    //    Mem_ID = merchantid,
                                    //    RechargeType = "WATER",
                                    //    IpAddress = objval.IpAddress,
                                    //    API_Name = "Instantpay"
                                    //};
                                    //db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(insta);
                                    //await db.SaveChangesAsync();
                                    //var client = new WebClient();
                                    //var content = client.DownloadString("https://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + Recharge.data.agent_id.Value + "&opr_id=" + Recharge.data.opr_id.Value + "&status=" + status + "&res_code=" + Recharge.statuscode + "&res_msg=" + status + "");
                                    //var callUrlRespResult = JObject.Parse(content);
                                    //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                    //if (urlcalbackresp == "TXN")
                                    //{
                                    //    string UniqueIdgen = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                    //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //    bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "WATER Bill", objval.IpAddress, UniqueIdgen);
                                    //}
                                    //return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }


            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostElectricityBill(POST) Line No:- 613", ex);
                throw ex;
            }


        }


        #endregion


        #region Get Merchant Outlet Information
        [HttpPost]
        public JsonResult GetMerchantOutletInformation()
        {
            initpage();
            try
            {
                initpage();
                var dbCon = new DBContext();
                var data = dbCon.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                if (data != null)
                {
                    var GetOutletInfo = dbCon.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (GetOutletInfo == null)
                    {
                        return Json(data);
                    }
                    else
                    {
                        return Json("NotFound");
                    }
                }
                else
                {
                    return Json("NotFound");
                }
               
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  PostOutletInformation(Merchant Outlet), method:- GenerateOutletOTP(POST) Line No:- 118", ex);
                throw ex;
            }

        }


        [HttpPost]
        public async Task<JsonResult> PostRegisterOutletData(string PanNo, string OTP )
        {

            try
            {
                var db = new DBContext();
                var GetMerinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                //string mobileno = Request.Form["Reg_Mobile"].Remove(',');
                var PaymentValidation = OutletApi.RegisterOutlet(GetMerinfo.MEMBER_MOBILE, OTP, GetMerinfo.EMAIL_ID, GetMerinfo.COMPANY, GetMerinfo.MEMBER_NAME, GetMerinfo.ADDRESS, GetMerinfo.PIN, PanNo);
                if (PaymentValidation.statuscode == "TXN")
                {
                    string statuscode = PaymentValidation.statuscode;
                    string outletid = PaymentValidation.data.outlet_id;
                    string Outletname= PaymentValidation.data.outlet_name;
                    string OutletPancard = PaymentValidation.data.pan_no;
                    string OutletStatus = PaymentValidation.data.outlet_status;
                    string OutletKycStatus = PaymentValidation.data.kyc_status;
                    var checkoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.OUTLETID == outletid).FirstOrDefault();
                    if (checkoutletid == null)
                    {
                        TBL_MERCHANT_OUTLET_INFORMATION objmsg = new TBL_MERCHANT_OUTLET_INFORMATION()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            OUTLETID = outletid,
                            MOBILE = GetMerinfo.MEMBER_MOBILE,
                            EMAIL = GetMerinfo.EMAIL_ID,
                            OUTLETNAME = Outletname,
                            CONTACTPERSON = PaymentValidation.data.contact_person.Value,
                            //AADHAARNO = PaymentValidation.data.aadhaar_no.Value,
                            PANCARDNO = PaymentValidation.data.pan_no.Value,
                            KYC_STATUS = 0,
                            OUTLET_STATUS = 0,
                            INSERTED_DATE = System.DateTime.Now,
                            INSERTED_BY = CurrentMerchant.MEM_ID,
                            PINCODE = GetMerinfo.PIN

                        };
                        db.TBL_MERCHANT_OUTLET_INFORMATION.Add(objmsg);
                        await db.SaveChangesAsync();
                    }
                    return Json(statuscode);
                }
                else
                {
                    string statuscode = PaymentValidation.status;
                    return Json(statuscode);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                throw;
            }
        }
        #endregion

    }
    public class Location
    {
        public string IPAddress { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TimeZone { get; set; }
    }
}
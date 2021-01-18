using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberPaymentGatewayListController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Logout", "AdminLogin", new { area = "Admin" }));
                    return;
                }
                bool Islogin = false;

                if (Session["WhiteLevelUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
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
        // GET: Admin/MemberPaymentGatewayList
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();                
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid(string SearchVal = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (SearchVal != "")
                {
                    //var Railmemberinfo = dbcontext.TBL_RAIL_AGENT_INFORMATION.Where(x => x.RAIL_USER_ID.StartsWith(SearchVal) || x.TRAVEL_AGENT_NAME.StartsWith(SearchVal) || x.AGENCY_NAME.StartsWith(SearchVal) || x.OFFICE_ADDRESS.StartsWith(SearchVal) || x.RESIDENCE_ADDRESS.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.MOBILE_NO.StartsWith(SearchVal) || x.OFFICE_PHONE.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.DIGITAL_CERTIFICATE_DETAILS.StartsWith(SearchVal)).ToList();
                    var memberinfo = (from x in dbcontext.TBL_MASTER_MEMBER
                                      join y in dbcontext.TBL_PAYMENT_GATEWAY_RESPONSE
on x.MEM_ID equals y.MEM_ID where x.MEM_UNIQUE_ID.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || y.CORELATION_ID.StartsWith(SearchVal) || y.PAY_REF_NO.StartsWith(SearchVal) || y.RES_STATUS.StartsWith(SearchVal)
                                      select new
                                      {
                                          MEM_UniqueId = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                          SLN = y.SLN,
                                          RES_MSG = y.RES_MSG,
                                          RES_DATE = y.RES_DATE,
                                          RES_STATUS = y.RES_STATUS,
                                          PAY_REF_NO = y.PAY_REF_NO,
                                          CORELATION_ID = y.CORELATION_ID,
                                          EMAIL_ID = y.EMAIL_ID,
                                          MOBILE_No = y.MOBILE_No,
                                          TRANSACTION_AMOUNT = y.TRANSACTION_AMOUNT,
                                          RES_CODE = y.RES_CODE,
                                          TRANSACTION_DETAILS = y.TRANSACTION_DETAILS,
                                          AMOUNT_WITH_GST = y.AMOUNT_WITH_GST,
                                          STATUS=y.STATUS

                                      }).AsEnumerable().Select((z, index) => new TBL_PAYMENT_GATEWAY_RESPONSE
                                      {
                                          Serial_No=index+1,
                                          SLN = z.SLN,
                                          Member_Name = z.MEM_UniqueId,
                                          RES_MSG = z.RES_MSG,
                                          RES_DATE = z.RES_DATE,
                                          RES_STATUS = z.RES_STATUS,
                                          PAY_REF_NO = z.PAY_REF_NO,
                                          CORELATION_ID = z.CORELATION_ID,
                                          EMAIL_ID = z.EMAIL_ID,
                                          MOBILE_No = z.MOBILE_No,
                                          TRANSACTION_AMOUNT = z.TRANSACTION_AMOUNT,
                                          RES_CODE = z.RES_CODE,
                                          TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                          AMOUNT_WITH_GST = z.AMOUNT_WITH_GST,
                                         STATUS=z.STATUS
                                      }).ToList().OrderByDescending(c=>c.SLN);
                    return PartialView("IndexGrid", memberinfo);
                }
                else
                {
                    var memberinfo = (from x in dbcontext.TBL_MASTER_MEMBER
                                      join y in dbcontext.TBL_PAYMENT_GATEWAY_RESPONSE
on x.MEM_ID equals y.MEM_ID
                                      select new
                                      {
                                          MEM_UniqueId = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                          SLN = y.SLN,
                                          RES_MSG = y.RES_MSG,
                                          RES_DATE = y.RES_DATE,
                                          RES_STATUS = y.RES_STATUS,
                                          PAY_REF_NO = y.PAY_REF_NO,
                                          CORELATION_ID = y.CORELATION_ID,
                                          EMAIL_ID = y.EMAIL_ID,
                                          MOBILE_No = y.MOBILE_No,
                                          TRANSACTION_AMOUNT = y.TRANSACTION_AMOUNT,
                                          RES_CODE = y.RES_CODE,
                                          TRANSACTION_DETAILS = y.TRANSACTION_DETAILS,
                                          AMOUNT_WITH_GST = y.AMOUNT_WITH_GST,
                                          STATUS=y.STATUS
                                      }).AsEnumerable().Select((z, index) => new TBL_PAYMENT_GATEWAY_RESPONSE
                                      {
                                          Serial_No=index+1,
                                          SLN = z.SLN,
                                          Member_Name = z.MEM_UniqueId,
                                          RES_MSG = z.RES_MSG,
                                          RES_DATE = z.RES_DATE,
                                          RES_STATUS = z.RES_STATUS,
                                          PAY_REF_NO = z.PAY_REF_NO,
                                          CORELATION_ID = z.CORELATION_ID,
                                          EMAIL_ID = z.EMAIL_ID,
                                          MOBILE_No = z.MOBILE_No,
                                          TRANSACTION_AMOUNT = z.TRANSACTION_AMOUNT,
                                          RES_CODE = z.RES_CODE,
                                          TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                          AMOUNT_WITH_GST = z.AMOUNT_WITH_GST,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(c => c.SLN);
                    return PartialView("IndexGrid", memberinfo);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult MemberPaymentGatewayList(string slnValue)
        {
            try
            {
                var db = new DBContext();
                long SLn = 0;
                long.TryParse(slnValue, out SLn);
                var memberinfo = (from x in db.TBL_MASTER_MEMBER
                                  join y in db.TBL_PAYMENT_GATEWAY_RESPONSE
on x.MEM_ID equals y.MEM_ID
                                  where y.SLN == SLn
                                  select new
                                  {
                                      MEM_UniqueId = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                      SLN = y.SLN,
                                      RES_MSG = y.RES_MSG,
                                      RES_DATE = y.RES_DATE,
                                      RES_STATUS = y.RES_STATUS,
                                      PAY_REF_NO = y.PAY_REF_NO,
                                      CORELATION_ID = y.CORELATION_ID,
                                      EMAIL_ID = y.EMAIL_ID,
                                      MOBILE_No = y.MOBILE_No,
                                      TRANSACTION_AMOUNT = y.TRANSACTION_AMOUNT,
                                      RES_CODE = y.RES_CODE,
                                      TRANSACTION_DETAILS = y.TRANSACTION_DETAILS,
                                      AMOUNT_WITH_GST = y.AMOUNT_WITH_GST,
                                      STATUS = y.STATUS
                                  }).AsEnumerable().Select(z => new TBL_PAYMENT_GATEWAY_RESPONSE
                                  {
                                      SLN = z.SLN,
                                      Member_Name = z.MEM_UniqueId,
                                      RES_MSG = z.RES_MSG,
                                      RES_DATE = z.RES_DATE,
                                      RES_STATUS = z.RES_STATUS,
                                      PAY_REF_NO = z.PAY_REF_NO,
                                      CORELATION_ID = z.CORELATION_ID,
                                      EMAIL_ID = z.EMAIL_ID,
                                      MOBILE_No = z.MOBILE_No,
                                      TRANSACTION_AMOUNT = z.TRANSACTION_AMOUNT,
                                      RES_CODE = z.RES_CODE,
                                      TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                      AMOUNT_WITH_GST = z.AMOUNT_WITH_GST,
                                      STATUS = z.STATUS
                                  }).FirstOrDefault();
                //var GEtPaymentInfo = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.SLN == SLn);
                return Json(memberinfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PostRequeryTransactionDetails(string slnval,string PaymentTrnId,string Ref_No)
        {
            var db = new DBContext();
            long Sln = 0;
            long.TryParse(slnval, out Sln);
            decimal Baln = 0;
            decimal OpenningBal = 0;
            decimal ColsingBal = 0;
            decimal MainBaln = 0;
            decimal AddmainBal = 0;
            var getDetails = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.SLN == Sln);
            decimal TransactionAmt = 0;
            //decimal.TryParse(Amount, out TransactionAmt);
            decimal.TryParse(getDetails.TRANSACTION_AMOUNT.ToString(), out TransactionAmt);
            long MEM_IDVAue = 0;
            //long.TryParse(Session["MerchantUserId"].ToString(), out MEM_IDVAue);
            long MEM_ID = 0;
            //long.TryParse(MEMID, out MEM_ID);
            long.TryParse(getDetails.MEM_ID.ToString(), out MEM_ID);
            //string COrelationID = Settings.GetUniqueKey(MEM_ID.ToString());//PaymentTrnId
            string COrelationID = PaymentTrnId;
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MEM_ID).FirstOrDefault();
                    var accountdetails = db.TBL_ACCOUNTS.Where(X => X.MEM_ID == MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (accountdetails != null)
                    {
                        Baln = TransactionAmt;
                        OpenningBal = accountdetails.CLOSING;
                        ColsingBal = OpenningBal + Baln;
                        decimal.TryParse(whiteleveluser.BALANCE.ToString(), out MainBaln);
                        AddmainBal = MainBaln + Baln;
                        TBL_ACCOUNTS objmer = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = MEM_ID,
                            MEMBER_TYPE = "MERCHANT",
                            //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                            TRANSACTION_TYPE = "Merchant Wallet Recharge",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                            AMOUNT = Baln,
                            NARRATION = "Merchant Wallet Recharge",
                            OPENING = OpenningBal,
                            CLOSING = ColsingBal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            TDS = 0,
                            GST = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID
                        };
                        db.TBL_ACCOUNTS.Add(objmer);
                        whiteleveluser.BALANCE = AddmainBal;
                        db.Entry(whiteleveluser).State = System.Data.Entity.EntityState.Modified;
                        
                        getDetails.RES_STATUS = "success";
                        getDetails.STATUS = 1;
                        getDetails.PAY_REF_NO = Ref_No;
                        getDetails.TRANSACTION_DETAILS = "Online Process";
                        db.Entry(getDetails).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        ContextTransaction.Commit();
                        string name = whiteleveluser.MEMBER_NAME;
                        string sub = "Easypay payment is successfull";
                        string usermsgdesc = "Dear <b>" + whiteleveluser.MEMBER_NAME + "</b>, You have successfully recharge your wallet of amount " + Baln.ToString() + "  Rs. by Easypay Payment gateway.";
                        EmailHelper emailhelper = new EmailHelper();
                        string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID, sub, usermsgbody);
                        return Json("Transaction Successfull",JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Baln = TransactionAmt;
                        OpenningBal = 0;
                        ColsingBal = OpenningBal + Baln;
                        decimal.TryParse(whiteleveluser.BALANCE.ToString(), out MainBaln);
                        AddmainBal = MainBaln + Baln;
                        TBL_ACCOUNTS objmer = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = MEM_ID,
                            MEMBER_TYPE = "MERCHANT",
                            //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                            TRANSACTION_TYPE = "Merchant Wallet Recharge",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                            AMOUNT = Baln,
                            NARRATION = "Merchant Wallet Recharge",
                            OPENING = OpenningBal,
                            CLOSING = ColsingBal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            TDS = 0,
                            GST = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID
                        };
                        db.TBL_ACCOUNTS.Add(objmer);
                        whiteleveluser.BALANCE = AddmainBal;
                        db.Entry(whiteleveluser).State = System.Data.Entity.EntityState.Modified;

                        getDetails.RES_STATUS = "success";
                        getDetails.STATUS = 1;
                        getDetails.TRANSACTION_DETAILS = "Online Process";
                        db.Entry(getDetails).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        string name = whiteleveluser.MEMBER_NAME;
                        string sub = "Easypay payment is successfull";
                        string usermsgdesc = "Dear <b>" + whiteleveluser.MEMBER_NAME + "</b>, You have successfully recharge your wallet of amount " + Baln.ToString() + "  Rs. by Easypay Payment gateway.";
                        EmailHelper emailhelper = new EmailHelper();
                        string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID, sub, usermsgbody);
                        return Json("Transaction Successfull", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw;
                 
                }
            }
        }
    }
}
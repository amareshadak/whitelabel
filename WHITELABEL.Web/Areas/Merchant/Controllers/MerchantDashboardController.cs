using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using System.Web.Security;
using easebuzz_.net;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantDashboardController : MerchantBaseController
    {
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
        //                //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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

        // GET: Merchant/MerchantDashboard
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();

                //if ((TempData["EaseBuzzResponse"]) != null)
                //{
                //    ViewBag.IsShowPrintTicket = TempData["EaseBuzzResponse"];

                //    ViewBag.TXnStatus = TempData["TXnStatus"];
                //    ViewBag.txnid = TempData["txnid"];
                //    ViewBag.txnAmt = TempData["txnAmt"];
                //}
                //else
                //{
                //    ViewBag.IsShowPrintTicket = "";
                //    ViewBag.TXnStatus = "";
                //    ViewBag.txnid ="";
                //    ViewBag.txnAmt = "";
                //}
                return View();
                //var db = new DBContext();
                //var availablebal = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                //if (availablebal != null)
                //{
                //    if (availablebal.BALANCE > 0)
                //    {
                //        ViewBag.AvailableBalance = availablebal.BALANCE;
                //    }
                //    else
                //    {
                //        ViewBag.AvailableBalance = 0;
                //    }
                //}
                //else
                //{
                //    ViewBag.AvailableBalance = 0;
                //}
                //List<string> val = new List<string>();
                //MerchantBaseController objval = new MerchantBaseController();
                //val = objval.GetTreeMember(CurrentMerchant.MEM_ID);
                //foreach (var listinfo in val)
                //{
                //    string[] userinfo = listinfo.Split(',');
                //    string UserID = userinfo[0];
                //    string UserIdVal = Decrypt.DecryptMe(UserID);
                //    string UserName = userinfo[1];
                //}
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public ActionResult DistributorBankDetails()
        {
            return View();
        }
        [HttpGet]
        public PartialViewResult DIS_BankIndexGrid()
        {
            try
            {
                // Only grid query values will be available here.
                var db = new DBContext();
                var introducer_Id = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                var dis_BankDetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == introducer_Id.INTRODUCER).ToList();
                return PartialView("DIS_BankIndexGrid", dis_BankDetails);
            }
            catch (Exception ex)
            {

                throw ex;
            }            
        }

        [HttpGet]
        public PartialViewResult GetNotificationList()
        {
            try
            {
                var db = new DBContext();
                var notification = db.TBL_NOTIFICATION_SETTING.Where(x => x.STATUS == 0).ToList();
                return PartialView("GetNotificationList", notification);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public PartialViewResult Merchant_RequisitionList()
        {
            try
            {
                var db = new DBContext();
                var memberrequisition = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.FROM_MEMBER == CurrentMerchant.MEM_ID).ToList();
                return PartialView("Merchant_RequisitionList", memberrequisition);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public JsonResult LoadAvailableBalance()
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                var walletamount = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                if (walletamount != null)
                {
                    //Session["openingAmt"] = walletamount.BALANCE;
                    //Session["closingAmt"] = walletamount.BALANCE;
                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {                    
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                //return Json(walletamount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EaseBuzzRechargeWallet()
        {
            initpage();
            try
            {
                string salt = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzSaltKey"];
                string Key = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzKey"];
                string env = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzEnviroment"];
                //string salt = "4NGY1NYJJP";
                // string Key = "W8A3NHRAWY";
                //string env = "test";
                string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                var db = new DBContext();
                var memberinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                string amount = "100";
                string firstname = memberinfo.MEMBER_NAME.Trim();
                string email = memberinfo.EMAIL_ID.Trim();
                string phone = memberinfo.MEMBER_MOBILE.Trim();
                string productinfo = "Easebuzz payment integration text";
                string surl = "http://b2b.boomtravels.com/Merchant/MerchantRequisition/EasepaySuccess";
                string furl = "http://b2b.boomtravels.com/Merchant/MerchantRequisition/EasepaySuccess";
                //string surl = "http://localhost:56049/Merchant/MerchantRequisition/EasepaySuccess";
                //string furl = "http://localhost:56049/Merchant/MerchantRequisition/EasepayFailure";
                string Txnid = COrelationID.Trim();
                string UDF1 = "";
                string UDF2 = "";
                string UDF3 = "";
                string UDF4 = "";
                string UDF5 = "";
                string Show_payment_mode = "";
                Easebuzz t = new Easebuzz(salt, Key, env);
                string strForm = t.initiatePaymentAPI(amount, firstname, email, phone, productinfo, surl, furl, Txnid, UDF1, UDF2, UDF3, UDF4, UDF5, Show_payment_mode);
                return Content(strForm, System.Net.Mime.MediaTypeNames.Text.Html);
                //return Json(walletamount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("please try again later", JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }
    }
}
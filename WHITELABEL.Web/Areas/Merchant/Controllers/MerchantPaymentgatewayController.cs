using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantPaymentgatewayController : Controller
    {
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
                    ViewBag.CurrentUserId = Session["MerchantUserId"];
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

        // GET: Merchant/MerchantPaymentgateway
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EasepaySuccess()
        {
            //initpage();
            var db = new DBContext();
            try
            {
                //var MerchantGetMember = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                decimal AmountVal = 0;
                string ResponseOut = "";
                long MEMBER_ID = 0;
                string txnid = "";
                string Easypayid = "";
                string error_Message = "";
                string TXNStatus = "";
                decimal AMOUNT_With_GST = 0;
                string salt = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzSaltKey"];
                string Key = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzKey"];
                //string env = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzEnviroment"];
                //string salt = "4NGY1NYJJP";
                //string Key = "W8A3NHRAWY";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                merc_hash_vars_seq = hash_seq.Split('|');
                Array.Reverse(merc_hash_vars_seq);
                merc_hash_string = salt + "|" + Request.Form["status"];
                foreach (string merc_hash_var in merc_hash_vars_seq)
                {
                    merc_hash_string += "|";
                    merc_hash_string = merc_hash_string + (Request.Form[merc_hash_var] != null ? Request.Form[merc_hash_var] : "");
                }
                merc_hash = Easebuzz_Generatehash512(merc_hash_string).ToLower();
                if (merc_hash != Request.Form["hash"])
                {
                    Response.Write("Hash value did not matched");
                    ViewBag.messagevalue = "Hash value did not matched";
                }
                else
                {
                    order_id = Request.Form["txnid"];
                    //Response.Write("value matched");
                    if (Request.Form["status"] == "success")
                    {
                        //Response.Write(Request.Form);
                        ViewBag.messagevalue = Request.Form;                        
                        ViewBag.TXnStatus = Request.Form["status"];
                        ViewBag.txnid = Request.Form["txnid"];
                        ViewBag.txnAmt = Request.Form["amount"];
                        decimal TranAmount = Convert.ToDecimal(Request.Form["amount"]);
                        string ValueCheck = Request.Form["amount"].ToString();
                        ViewBag.easepayid = Request.Form["easepayid"];
                        string MEM_ID = Request.Form["udf1"];
                        string MEM_Password = Request.Form["udf2"];
                        string TransactionAmount = Request.Form["udf3"];
                        Easypayid = Request.Form["easepayid"];
                        ViewBag.EasypayId = Easypayid;
                        txnid = Request.Form["txnid"];
                        string payment_source = Request.Form["payment_source"];
                        ViewBag.payment_source = payment_source;
                        error_Message = Request.Form["error_Message"];
                        ViewBag.error_Message = error_Message;
                         ResponseOut = Convert.ToString(Request.Form);
                        ViewBag.iewBegCheckError = ValueCheck;
                        TXNStatus = Request.Form["status"];
                        //decimal.TryParse(ValueCheck, out AmountVal);
                        decimal.TryParse(TransactionAmount, out AmountVal);
                        decimal.TryParse(ValueCheck, out AMOUNT_With_GST);
                        long.TryParse(MEM_ID, out MEMBER_ID);
                        var GEtMemberinfpo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MEMBER_ID);
                        var getPaymentStatus = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.MEM_ID == MEMBER_ID && x.CORELATION_ID == txnid);
                        if (getPaymentStatus != null)
                        {
                            getPaymentStatus.RES_MSG = ResponseOut;
                            getPaymentStatus.RES_STATUS = TXNStatus;
                            getPaymentStatus.PAY_REF_NO = Easypayid;
                            getPaymentStatus.RES_CODE = error_Message;
                            getPaymentStatus.STATUS = 1;
                            getPaymentStatus.TRANSACTION_DETAILS = "Online Process";
                            db.Entry(getPaymentStatus).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            string MSgAmt = AccountBalance(TransactionAmount, MEM_ID, txnid);
                        }
                        //TBL_PAYMENT_GATEWAY_RESPONSE objres = new TBL_PAYMENT_GATEWAY_RESPONSE()
                        //{
                        //    MEM_ID = MEMBER_ID,
                        //    RES_MSG = ResponseOut,
                        //    RES_DATE = DateTime.Now,
                        //    RES_STATUS = TXNStatus,
                        //    PAY_REF_NO = Easypayid,
                        //    CORELATION_ID = txnid,
                        //    EMAIL_ID = GEtMemberinfpo.EMAIL_ID,
                        //    MOBILE_No = GEtMemberinfpo.MEMBER_MOBILE,
                        //    TRANSACTION_AMOUNT = AmountVal,
                        //    RES_CODE = error_Message,
                        //    TRANSACTION_DETAILS = "Online Process",
                        //    AMOUNT_WITH_GST= AMOUNT_With_GST
                        //};
                        //db.TBL_PAYMENT_GATEWAY_RESPONSE.Add(objres);
                        //db.SaveChanges();
                        
                        //string MSgAmt = AccountBalance(TransactionAmount, MEM_ID, txnid);
                    }
                    else
                    {
                        //Response.Write(Request.Form);
                        ViewBag.messagevalue = Request.Form;
                        //string Res = Convert.ToString(Request.Form);
                        //string Responseval = ViewBag.messagevalue;
                        ResponseOut = Convert.ToString(Request.Form);
                        ViewBag.TXnStatus = Request.Form["status"];
                        TXNStatus = Request.Form["status"];
                        ViewBag.txnid = Request.Form["txnid"];
                        ViewBag.txnAmt = Request.Form["amount"];
                        ViewBag.easepayid = Request.Form["easepayid"];                        
                        string MEM_ID = Request.Form["udf1"];
                        string MEM_Password = Request.Form["udf2"];
                        string TransactionAmount = Request.Form["udf3"];
                        decimal TranAmount = Convert.ToDecimal(Request.Form["amount"]);
                        string ValueCheck = Request.Form["amount"].ToString();
                        ViewBag.iewBegCheckError = ValueCheck;
                        decimal.TryParse(TransactionAmount, out AmountVal);
                        decimal.TryParse(ValueCheck, out AMOUNT_With_GST);
                        Easypayid = Request.Form["easepayid"];
                        ViewBag.EasypayId = Easypayid;
                         txnid = Request.Form["txnid"];
                         string payment_source = Request.Form["payment_source"];
                        ViewBag.payment_source = payment_source;
                         error_Message = Request.Form["error_Message"];
                        ViewBag.error_Message = error_Message;
                        string Outpot = Convert.ToString(Request.Form);
                        long.TryParse(MEM_ID, out MEMBER_ID);
                        long.TryParse(MEM_ID, out MEMBER_ID);
                        var GEtMemberinfpo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MEMBER_ID);
                        var getPaymentStatus = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.MEM_ID == MEMBER_ID && x.CORELATION_ID == txnid);
                        if (getPaymentStatus != null)
                        {
                            getPaymentStatus.RES_MSG = ResponseOut;
                            getPaymentStatus.RES_STATUS = TXNStatus;
                            getPaymentStatus.PAY_REF_NO = Easypayid;
                            getPaymentStatus.RES_CODE = error_Message;
                            getPaymentStatus.STATUS = 0;
                            getPaymentStatus.TRANSACTION_DETAILS = "Online Process Pending";
                            db.Entry(getPaymentStatus).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //TBL_PAYMENT_GATEWAY_RESPONSE objres = new TBL_PAYMENT_GATEWAY_RESPONSE()
                        //{
                        //    MEM_ID = MEMBER_ID,
                        //    RES_MSG = ResponseOut,
                        //    RES_DATE = DateTime.Now,
                        //    RES_STATUS = TXNStatus,
                        //    PAY_REF_NO = Easypayid,
                        //    CORELATION_ID = txnid,
                        //    EMAIL_ID = GEtMemberinfpo.EMAIL_ID,
                        //    MOBILE_No = GEtMemberinfpo.MEMBER_MOBILE,
                        //    TRANSACTION_AMOUNT = AmountVal,
                        //    RES_CODE = error_Message,
                        //    TRANSACTION_DETAILS = "Online Process",
                        //    AMOUNT_WITH_GST= AMOUNT_With_GST
                        //};
                        //db.TBL_PAYMENT_GATEWAY_RESPONSE.Add(objres);
                        //db.SaveChanges();
                        //string MSgAmt = AccountBalance(ValueCheck, MEM_ID);


                    }
                    //Hash value did not matched
                }
                return View();
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
                ViewBag.messagevalue = "Null";
                ViewBag.TXnStatus = "Transaction Cancelled.Please contact your administrator.";
                ViewBag.txnid ="XXXXXXXXX";
                ViewBag.txnAmt = "XXXXXXXX";
                ViewBag.easepayid = "XXXXXXXXXX";
                ViewBag.iewBegCheckError ="Transaction Desclined";
                ViewBag.EasypayId = "Null";
                ViewBag.payment_source = "Boom Travel";
                ViewBag.error_Message = "Declined";
                return RedirectToAction("EasepaySuccess", "MerchantPaymentgateway", new { area = "Merchant" });
            }
        }
        public string AccountBalance(string Amount,string MEMID,string corelationid)
        {
            var db = new DBContext();
            decimal Baln = 0;
            decimal OpenningBal = 0;
            decimal ColsingBal = 0;
            decimal MainBaln = 0;
            decimal AddmainBal = 0;
            decimal TransactionAmt = 0;
            decimal.TryParse(Amount, out TransactionAmt);
            long MEM_IDVAue = 0;
            //long.TryParse(Session["MerchantUserId"].ToString(), out MEM_IDVAue);
            long MEM_ID = 0;
            long.TryParse(MEMID,out MEM_ID);
            string COrelationID = Settings.GetUniqueKey(MEM_ID.ToString());
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
                            CORELATIONID = corelationid
                        };
                        db.TBL_ACCOUNTS.Add(objmer);
                        whiteleveluser.BALANCE = AddmainBal;
                        db.Entry(whiteleveluser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        string name = whiteleveluser.MEMBER_NAME;
                        string sub = "Easypay payment is successfull";
                        string usermsgdesc = "Dear <b>" + whiteleveluser.MEMBER_NAME + "</b>, You have successfully recharge your wallet of amount " + Baln.ToString() + "  Rs. by Easypay Payment gateway.";
                        EmailHelper emailhelper = new EmailHelper();
                        string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID, sub, usermsgbody);
                        return "true";
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
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        string name = whiteleveluser.MEMBER_NAME;
                        string sub = "Easypay payment is successfull";
                        string usermsgdesc = "Dear <b>" + whiteleveluser.MEMBER_NAME + "</b>, You have successfully recharge your wallet of amount " + Baln.ToString() + "  Rs. by Easypay Payment gateway.";
                        EmailHelper emailhelper = new EmailHelper();
                        string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID, sub, usermsgbody);
                        return "true";
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw;
                    return "false";
                }
            }
        }
        public string Easebuzz_Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }
    }
}
﻿using System;
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
                        ViewBag.iewBegCheckError = ValueCheck;  
                        string MSgAmt = AccountBalance(ValueCheck, MEM_ID);
                        //Session["MerchantUserName"] = MerchantGetMember.UName;
                        //Session["MerchantCompanyName"] = MerchantGetMember.COMPANY;
                        //Session["UserType"] = "Merchant";
                        //Session["CreditLimitAmt"] = MerchantGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                        //Session["ReservedCreditLimitAmt"] = MerchantGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
                        //decimal TranAmount = Convert.ToDecimal(Request.Form["amount"]);

                        //string UpdateAccount = AccountBalance(ValueCheck);
                    }
                    else
                    {
                        //Response.Write(Request.Form);
                        ViewBag.messagevalue = Request.Form;
                        //string Res = Convert.ToString(Request.Form);
                        //string Responseval = ViewBag.messagevalue;
                        ViewBag.TXnStatus = Request.Form["status"];
                        ViewBag.txnid = Request.Form["txnid"];
                        ViewBag.txnAmt = Request.Form["amount"];
                        ViewBag.easepayid = Request.Form["easepayid"];                        
                        string MEM_ID = Request.Form["udf1"];
                        string MEM_Password = Request.Form["udf2"];
                        decimal TranAmount = Convert.ToDecimal(Request.Form["amount"]);
                        string ValueCheck = Request.Form["amount"].ToString();
                        ViewBag.iewBegCheckError = ValueCheck;

                        string MSgAmt = AccountBalance(ValueCheck, MEM_ID);

                        //Session["MerchantUserName"] = MerchantGetMember.UName;
                        //Session["MerchantCompanyName"] = MerchantGetMember.COMPANY;
                        //Session["UserType"] = "Merchant";
                        //Session["CreditLimitAmt"] = MerchantGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                        //Session["ReservedCreditLimitAmt"] = MerchantGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
                    }
                    //Hash value did not matched
                }
                return View();
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
                return RedirectToAction("EasepaySuccess", "MerchantPaymentgateway", new { area = "Merchant" });
            }
        }
        public string AccountBalance(string Amount,string MEMID)
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
                            CORELATIONID = COrelationID
                        };
                        db.TBL_ACCOUNTS.Add(objmer);
                        whiteleveluser.BALANCE = AddmainBal;
                        db.Entry(whiteleveluser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
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
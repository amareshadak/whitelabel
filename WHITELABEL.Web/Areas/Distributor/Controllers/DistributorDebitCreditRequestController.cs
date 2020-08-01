﻿using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    public class DistributorDebitCreditRequestController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Distributor Requisition";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 4);
        //            if (currUser != null)
        //            {
        //                Session["DistributorUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["DistributorUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["DistributorUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
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
                ViewBag.ControllerName = "Distributor";
                if (Session["DistributorUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["DistributorUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Distributor/DistributorDebitCreditRequest
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        //public async Task<ActionResult> Index(TBL_BALANCE_TRANSFER_LOGS obj_bal)
        public async Task<JsonResult> Index(TBL_BALANCE_TRANSFER_LOGS obj_bal)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var checkAvailableMember = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == obj_bal.FROM_MEMBER).FirstOrDefault();
                    var checkAvailableMember = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                    //if (checkAvailableMember != null)
                    if (checkAvailableMember != null)
                    {
                       
                            var obj = obj_bal;
                            //var mem_id = Request.Form["memberDomainId"].ToString();
                            long memberid = long.Parse(obj_bal.FROM_MEMBER.ToString());

                            decimal closingamt = 0;
                            decimal Openingamt = 0;
                            decimal transamt = 0;
                            decimal AddAmount = 0;
                            string MEWR_TYHPETRN = string.Empty;
                            var amtval = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == memberid).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                            if (amtval != null)
                            {
                                Openingamt = amtval.OPENING;
                                closingamt = amtval.CLOSING;
                                transamt = amtval.AMOUNT;
                                if (obj_bal.PAYMENT_METHOD == "CR")
                                {

                                    AddAmount = decimal.Parse(obj_bal.AMOUNT.ToString()) + closingamt;
                                }
                                else
                                {
                                    AddAmount = closingamt - decimal.Parse(obj_bal.AMOUNT.ToString());
                                }
                            }
                            else
                            {
                                Openingamt = 0;
                                closingamt = decimal.Parse(obj_bal.AMOUNT.ToString());
                                AddAmount = decimal.Parse(obj_bal.AMOUNT.ToString());
                            }
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = memberid,
                                MEMBER_TYPE = "RETAILER",
                                //TRANSACTION_TYPE = "Cash Deposit in bank",
                                TRANSACTION_TYPE= obj_bal.TRANSACTION_DETAILS,
                                TRANSACTION_DATE = DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = obj_bal.PAYMENT_METHOD,
                                AMOUNT = obj_bal.AMOUNT,
                                NARRATION = obj_bal.TRANSACTION_DETAILS,
                                CLOSING = AddAmount,
                                OPENING = closingamt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                SERVICE_ID = 0,
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            //await db.SaveChangesAsync();
                            var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memberid).FirstOrDefaultAsync();
                            memberlist.BALANCE = AddAmount;
                            db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                            //await db.SaveChangesAsync();

                            #region For WhiteLevel

                            decimal WLOpening = 0;
                            decimal WLClosing = 0;
                            decimal WLAmtvalue = 0;
                            var tbl_accnt = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                            if (tbl_accnt != null)
                            {
                                string trasftyoe = string.Empty;
                                WLOpening = tbl_accnt.OPENING;
                                WLClosing = tbl_accnt.CLOSING;
                                if (obj_bal.PAYMENT_METHOD == "CR")
                                {
                                    trasftyoe = "DR";
                                    WLAmtvalue = WLClosing - decimal.Parse(obj_bal.AMOUNT.ToString());
                                }
                                else
                                {
                                    trasftyoe = "CR";
                                    WLAmtvalue = WLClosing + decimal.Parse(obj_bal.AMOUNT.ToString());
                                }
                                TBL_ACCOUNTS objWL = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    MEMBER_TYPE = "DISTRIBUTOR",
                                    //TRANSACTION_TYPE = "Cash Deposit in bank",
                                    TRANSACTION_TYPE = obj_bal.TRANSACTION_DETAILS,
                                    TRANSACTION_DATE = DateTime.Now,
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = trasftyoe,
                                    AMOUNT = obj_bal.AMOUNT,
                                    NARRATION = obj_bal.TRANSACTION_DETAILS,
                                    CLOSING = WLAmtvalue,
                                    OPENING = WLClosing,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    GST = 0,
                                    TDS = 0,
                                    SERVICE_ID = 0,
                                };
                                db.TBL_ACCOUNTS.Add(objWL);
                                //await db.SaveChangesAsync();
                                var WLBalance = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
                                WLBalance.BALANCE = WLAmtvalue;
                                db.Entry(WLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            ContextTransaction.Commit();
                        #endregion

                        //return View("DisplayAccount");
                        return Json("Manual Balance Transfer done successfully.",JsonRequestBehavior.AllowGet);

                       
                    }
                    else
                    {
                        //ViewBag.msg = "Please provide valid merchant user name";
                        //return View("Index");
                        return Json("Please contact to your administrator.", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }                
        }
        public ActionResult GetPeople(string query)
        {
            return Json(_GetPeople(query), JsonRequestBehavior.AllowGet);
        }

        private List<Autocomplete> _GetPeople(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            var db = new DBContext();
            try
            {
                var results = (from p in db.TBL_MASTER_MEMBER
                               where (p.UName).Contains(query) && p.INTRODUCER == MemberCurrentUser.MEM_ID
                               orderby p.UName
                               select p).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete Username = new Autocomplete();

                    //Username.FromUser = string.Format("{0} {1}", r.UName);
                    Username.Name = (r.UName + "-" + r.MEMBER_MOBILE + "-" + r.COMPANY);
                    Username.Id = r.MEM_ID;

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

        public ActionResult DisplayAccount()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult indexgrid(string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            //var listaccount = db.TBL_ACCOUNTS.Where(x => x.MEMBER_TYPE == "WHITE LEVEL" && (x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR")).ToList();
            if (DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var listaccount = (from x in db.TBL_ACCOUNTS
                                   join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                   where x.MEMBER_TYPE == "RETAILER" && y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR") && x.TRANSACTION_DATE >= Date_From_Val && x.TRANSACTION_DATE <= Date_To_Val
                                   select new
                                   {
                                       UserName = y.UName,
                                       MEM_ID = x.MEM_ID,
                                       MEMBER_TYPE = x.MEMBER_TYPE,
                                       API_ID = x.API_ID,
                                       Transaction_TYpe = x.TRANSACTION_TYPE,
                                       TransactionDate = x.TRANSACTION_DATE,
                                       Transactiontime = x.TRANSACTION_TIME,
                                       DR_CR = x.DR_CR,
                                       Narration = x.NARRATION,
                                       OpeningAmt = x.OPENING,
                                       ClosingAmt = x.CLOSING,
                                       Amount = x.AMOUNT,
                                       Rec_No = x.REC_NO,
                                       CommAmt = x.COMM_AMT
                                   }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                   {
                                       SerialNo = index + 1,
                                       UserName = z.UserName,
                                       MEM_ID = z.MEM_ID,
                                       API_ID = z.API_ID,
                                       MEMBER_TYPE = z.MEMBER_TYPE,
                                       TRANSACTION_DATE = z.TransactionDate,
                                       TRANSACTION_TIME = z.Transactiontime,
                                       TRANSACTION_TYPE = z.Transaction_TYpe,
                                       DR_CR = z.DR_CR,
                                       AMOUNT = z.Amount,
                                       OPENING = z.OpeningAmt,
                                       CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                       DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                       CLOSING = z.ClosingAmt,
                                       REC_NO = z.Rec_No,
                                       COMM_AMT = z.CommAmt

                                   }).ToList();
                return PartialView("indexgrid", listaccount);
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var listaccount = (from x in db.TBL_ACCOUNTS
                               join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                               where x.MEMBER_TYPE == "RETAILER" && y.INTRODUCER==MemberCurrentUser.MEM_ID  && (x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR") && x.TRANSACTION_DATE == Todaydate
                                   select new
                               {
                                   UserName = y.UName,
                                   MEM_ID = x.MEM_ID,
                                   MEMBER_TYPE = x.MEMBER_TYPE,
                                   API_ID = x.API_ID,
                                   Transaction_TYpe = x.TRANSACTION_TYPE,
                                   TransactionDate = x.TRANSACTION_DATE,
                                   Transactiontime = x.TRANSACTION_TIME,
                                   DR_CR = x.DR_CR,
                                   Narration = x.NARRATION,
                                   OpeningAmt = x.OPENING,
                                   ClosingAmt = x.CLOSING,
                                   Amount = x.AMOUNT,
                                   Rec_No = x.REC_NO,
                                   CommAmt = x.COMM_AMT
                               }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                               {
                                   SerialNo = index + 1,
                                   UserName = z.UserName,
                                   MEM_ID = z.MEM_ID,
                                   API_ID = z.API_ID,
                                   MEMBER_TYPE = z.MEMBER_TYPE,
                                   TRANSACTION_DATE = z.TransactionDate,
                                   TRANSACTION_TIME = z.Transactiontime,
                                   TRANSACTION_TYPE = z.Transaction_TYpe,
                                   DR_CR = z.DR_CR,
                                   AMOUNT = z.Amount,
                                   OPENING = z.OpeningAmt,
                                   CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                   DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                   CLOSING = z.ClosingAmt,
                                   REC_NO = z.Rec_No,
                                   COMM_AMT = z.CommAmt

                               }).ToList();
            return PartialView("indexgrid", listaccount);
            }
        }
    }
}
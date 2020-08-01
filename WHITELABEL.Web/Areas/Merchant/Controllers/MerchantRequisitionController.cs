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
using System.Threading;
using System.Globalization;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRequisitionController : MerchantBaseController
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


        // GET: Merchant/MerchantRequisition
        public ActionResult Index()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.MEM_ID == CurrentMerchant.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
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
        }

        public PartialViewResult IndexGrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var db = new DBContext();

                return PartialView(CreateExportableGridINExcel(DateFrom, Date_To));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public FileResult GridExportIndex(string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportableGridINExcel(DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_BALANCE_TRANSFER_LOGS> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGridINExcel(string DateFrom , string Date_To)
        {
            var db = new DBContext();
            if (DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.FROM_MEMBER == CurrentMerchant.MEM_ID  && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
                                       select new
                                       {
                                           Touser = (x.REF_NO == "Admin" ? db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.UNDER_WHITE_LEVEL).Select(d => d.UName).FirstOrDefault() : db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(d => d.UName).FirstOrDefault()),
                                           //Touser = db.TBL_MASTER_MEMBER.Where(s=>s.MEM_ID==y.INTRODUCER).Select(d=>d.UName).FirstOrDefault(),
                                           transid = x.TransactionID,
                                           ReferenceNo = x.REFERENCE_NO,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           REFERENCE_NO = z.ReferenceNo,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User").Filterable(true);
                grid.Columns.Add(model => model.FromUser).Titled("From Member").Filterable(true);
                grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No").Filterable(true);
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount").Filterable(true);
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode").Filterable(true);
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt").Filterable(true);
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description").Filterable(true);

                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'> <a href='" + @Url.Action("RequisitionDetails", "MerchantRequisition", new { area = "Merchant", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'   title='Edit'><i class='fa fa-edit'></i></a></div>");


                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 10000000;
                return grid;
            }
            else
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.FROM_MEMBER == CurrentMerchant.MEM_ID
                                       select new
                                       {
                                           Touser = (x.REF_NO == "Admin" ? db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.UNDER_WHITE_LEVEL).Select(d => d.UName).FirstOrDefault() : db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(d => d.UName).FirstOrDefault()),
                                           //Touser = db.TBL_MASTER_MEMBER.Where(s=>s.MEM_ID==y.INTRODUCER).Select(d=>d.UName).FirstOrDefault(),
                                           transid = x.TransactionID,
                                           ReferenceNo = x.REFERENCE_NO,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           REFERENCE_NO = z.ReferenceNo,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User").Filterable(true);
                grid.Columns.Add(model => model.FromUser).Titled("From Member").Filterable(true);
                grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No").Filterable(true);
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount").Filterable(true);
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode").Filterable(true);
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt").Filterable(true);
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description").Filterable(true);

                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'> <a href='" + @Url.Action("RequisitionDetails", "MerchantRequisition", new { area = "Merchant", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'   title='Edit'><i class='fa fa-edit'></i></a></div>");


                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 10000000;
                return grid;
            }
            
        }
        public ActionResult RequisitionDetails(string transId = "")
        {
            if (Session["MerchantUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                    var introducerdetails = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == whiteleveluser.INTRODUCER).FirstOrDefault();
                    var Whitelabeldetails = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == whiteleveluser.UNDER_WHITE_LEVEL).FirstOrDefault();
                    ViewBag.IntroducerName = introducerdetails.MEMBER_NAME;
                    ViewBag.IntroducerEmail = introducerdetails.EMAIL_ID;
                    ViewBag.IntroducerMobile = introducerdetails.MEMBER_MOBILE;
                    ViewBag.IntroducerMemberId = introducerdetails.MEM_ID;
                    ViewBag.UnderWhiteLabel = Whitelabeldetails.UName;
                    ViewBag.WhiteLabelMemberId = Whitelabeldetails.MEM_ID;
                    ViewBag.WhitelabelName = Whitelabeldetails.MEMBER_NAME;
                    ViewBag.WhiteLabelMobile = Whitelabeldetails.MEMBER_MOBILE;
                    ViewBag.WhiteLabelEmail = Whitelabeldetails.EMAIL_ID;
                    if (transId == "")
                    {
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.CREATED_BY == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.MEMBER_NAME
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = z.MEM_ID.ToString(),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                        ////var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                        //var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                        //                       where x.MEM_ID == whiteleveluser.INTRODUCER && x.ISDELETED == 0
                        //                       select new
                        //                       {
                        //                           BankID = x.SL_NO,
                        //                           BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                        //                       }).AsEnumerable().Select(z => new ViewBankDetails
                        //                       {
                        //                           BankID = z.BankID.ToString(),
                        //                           BankName = z.BankName
                        //                       }).ToList().Distinct();
                        //ViewBag.BankInformation = new SelectList(BankInformation, "BankID", "BankName");
                        ViewBag.Introducer = db.TBL_MASTER_MEMBER.Where(x=>x.MEM_ID== whiteleveluser.INTRODUCER).Select(z=>z.UName).FirstOrDefault();
                        ViewBag.checkbank = "0";
                        return View();
                    }
                    else
                    {
                        string decripttransId = Decrypt.DecryptMe(transId);
                        long transID = long.Parse(decripttransId);
                        var TransactionInfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transID).FirstOrDefault();
                        //var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                        ViewBag.Introducer = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == whiteleveluser.INTRODUCER).Select(z => z.UName).FirstOrDefault(); ;
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.MEM_ID == TransactionInfo.FROM_MEMBER
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.MEMBER_NAME
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = z.MEM_ID.ToString(),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");

                        //var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                        //                       where x.MEM_ID == whiteleveluser.INTRODUCER && x.ISDELETED == 0
                        //                       select new
                        //                       {
                        //                           //BankID = x.SL_NO,
                        //                           BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                        //                       }).AsEnumerable().Select(z => new ViewBankDetails
                        //                       {
                        //                           //BankID = z.BankID.ToString(),
                        //                           BankName = z.BankName
                        //                       }).ToList().Distinct();
                        //ViewBag.BankInformation = new SelectList(BankInformation, "BankName", "BankName");
                        TransactionInfo.FromUser = TransactionInfo.FROM_MEMBER.ToString();
                        long introducer = long.Parse(whiteleveluser.INTRODUCER.ToString());
                        TransactionInfo.FROM_MEMBER = introducer;
                        TransactionInfo.PAYMENT_METHOD = TransactionInfo.PAYMENT_METHOD;
                        TransactionInfo.REQUEST_DATE = Convert.ToDateTime(TransactionInfo.REQUEST_DATE.ToString("yyyy-MM-dd").Substring(0, 10));
                        TransactionInfo.BANK_ACCOUNT = TransactionInfo.BANK_ACCOUNT;
                        TransactionInfo.TRANSACTION_DETAILS = TransactionInfo.TRANSACTION_DETAILS;
                        TransactionInfo.FromUser = "Test";
                        TransactionInfo.RequisitionSendTO = TransactionInfo.REF_NO;

                        ViewBag.checkbank = "1";
                        return View(TransactionInfo);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  DistributorRequestRequisition(Distributor), method:- RequisitionDetails (GET) Line No:- 276", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> RequisitionDetails(TBL_BALANCE_TRANSFER_LOGS objval)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    
                    //long userId = long.Parse(objval.FromUser);
                    // var membertype = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == userId).FirstOrDefault();
                  
                                         
                    var whiteleveluser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    var translist = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                    if (translist != null)
                    {
                        translist.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                        
                        translist.REQUEST_TIME = System.DateTime.Now;
                        translist.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        translist.AMOUNT = objval.AMOUNT;
                        translist.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                        translist.TRANSFER_METHOD = "Cash";
                        translist.FromUser = "testval";
                        translist.BANK_CHARGES = objval.BANK_CHARGES;
                        translist.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                        translist.INSERTED_BY = CurrentMerchant.MEM_ID;
                        translist.REF_NO = objval.RequisitionSendTO;
                        db.Entry(translist).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        //long fromuser = long.Parse(objval.FromUser);
                        long fromuser = 0;
                        if (objval.RequisitionSendTO == "Distributor")
                        {
                            fromuser = long.Parse(whiteleveluser.INTRODUCER.ToString());
                        }
                        else
                        {
                            fromuser = long.Parse(whiteleveluser.UNDER_WHITE_LEVEL.ToString());
                        }
                     
                        objval.TransactionID = fromuser + "" + CurrentMerchant.MEM_ID + DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.ToString("HHMMss");
                        objval.TO_MEMBER = fromuser;
                        objval.FROM_MEMBER = CurrentMerchant.MEM_ID;
                        objval.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                        
                        objval.REQUEST_TIME = System.DateTime.Now;
                        objval.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        objval.STATUS = "Pending";
                        objval.FromUser = "test";
                        objval.TRANSFER_METHOD = "Cash";
                        objval.BANK_CHARGES = objval.BANK_CHARGES;
                        objval.INSERTED_BY = CurrentMerchant.MEM_ID;
                        objval.REF_NO = objval.RequisitionSendTO;
                        db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                        await db.SaveChangesAsync();
                        //return RedirectToAction("Index");
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  DistributorRequestRequisition(Distributor), method:- RequisitionDetails (POST) Line No:- 341", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckReferenceNo(string referenceno)
        {
            initpage();
            var context = new DBContext();
            var User = await context.TBL_BALANCE_TRANSFER_LOGS.Where(model => model.REFERENCE_NO == referenceno && model.FROM_MEMBER == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
            if (User != null)
            {
                var list = context.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == User.FROM_MEMBER).FirstOrDefault();
                return Json(new { result = "unavailable" });
                //return Json(new { result = "unavailable",Mem_Name= list.DOMAIN,mem_Id =User.FROM_MEMBER,Req_Date=User.REQUEST_DATE,amt=User.AMOUNT,Bankid=User.BANK_ACCOUNT,paymethod=User.PAYMENT_METHOD,Transdetails=User.TRANSACTION_DETAILS,BankCharges=User.BANK_CHARGES});
            }
            else
            {
                return Json(new { result = "available" });
            }
        }
        [HttpPost]
        public JsonResult GetBankListAccordingtoMemberType(string MemberType="")
        {
            try
            {
                var db = new DBContext();
                var memInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID==CurrentMerchant.MEM_ID);
                if (MemberType == "Admin")
                {
                    //var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                    //                       where x.MEM_ID == memInfo.UNDER_WHITE_LEVEL && x.ISDELETED == 0
                    //                       select new
                    //                       {
                    //                           //BankID = x.SL_NO,
                    //                           BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                    //                       }).AsEnumerable().Select(z => new ViewBankDetails
                    //                       {
                    //                           //BankID = z.BankID.ToString(),
                    //                           BankName = z.BankName
                    //                       }).ToList().Distinct();

                    var memberService = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                         where x.MEM_ID == memInfo.UNDER_WHITE_LEVEL && x.ISDELETED ==0

                                             //where x.INTRODUCER == dis_Id
                                         select new
                                         {
                                             MEM_ID = (x.BANK + "-" + x.ACCOUNT_NO),
                                             UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    return Json(memberService, JsonRequestBehavior.AllowGet);
                }
                else if (MemberType == "Distributor")
                {
                    var memberService = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                         where x.MEM_ID == memInfo.INTRODUCER && x.ISDELETED == 0

                                         //where x.INTRODUCER == dis_Id
                                         select new
                                         {
                                             MEM_ID = (x.BANK + "-" + x.ACCOUNT_NO),
                                             UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    return Json(memberService, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var memberService = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                         where x.MEM_ID == memInfo.INTRODUCER && x.ISDELETED == 0

                                         //where x.INTRODUCER == dis_Id
                                         select new
                                         {
                                             MEM_ID = (x.BANK + "-" + x.ACCOUNT_NO),
                                             UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    return Json(memberService, JsonRequestBehavior.AllowGet);
                }
                
                //return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }
    }
}
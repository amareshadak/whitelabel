using System;
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
using easebuzz_.net;
using System.Text;
using System.Security.Cryptography;

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

                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
                return File(package.GetAsByteArray(), "application/unknown", "RequisitionExport.xlsx");
            }
        }

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGridINExcel(string DateFrom , string Date_To)
        {
            if (Session["MerchantUserId"] != null)
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
                                           where x.STATUS == "Pending" && x.FROM_MEMBER == CurrentMerchant.MEM_ID && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               //Touser = (x.REF_NO == "Admin" ? db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.UNDER_WHITE_LEVEL).Select(d => d.UName).FirstOrDefault() : db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(d => d.UName).FirstOrDefault()),
                                               Touser = x.REF_NO,
                                               //Touser = db.TBL_MASTER_MEMBER.Where(s=>s.MEM_ID==y.INTRODUCER).Select(d=>d.UName).FirstOrDefault(),
                                               transid = x.TransactionID,
                                               ReferenceNo = x.REFERENCE_NO,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               STATUS = x.STATUS
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
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               STATUS = z.STATUS
                                           }).ToList();

                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                    grid.Columns.Add(model => model.ToUser).Titled("To User").Filterable(true);
                    grid.Columns.Add(model => model.FromUser).Titled("From Member").Filterable(true);
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No").Filterable(true);
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:MM/dd/yyyy}").MultiFilterable(true);
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Time").Formatted("{0:T}").Filterable(false);
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amount").Filterable(true);
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode").Filterable(true);
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt").Filterable(true);
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description").Filterable(true);
                    grid.Columns.Add(model => model.STATUS).Titled("Req. Status").Filterable(true);
                    grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                        .RenderedAs(model => model.STATUS == "Pending" ? ("<div style='text-align:center'> <a href='" + @Url.Action("RequisitionDetails", "MerchantRequisition", new { area = "Merchant", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'   title='Edit'><i class='fa fa-edit'></i></a></div>") : "");


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
                                               //Touser = (x.REF_NO == "Admin" ? db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.UNDER_WHITE_LEVEL).Select(d => d.UName).FirstOrDefault() : db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(d => d.UName).FirstOrDefault()),
                                               Touser = x.REF_NO,
                                               //Touser = db.TBL_MASTER_MEMBER.Where(s=>s.MEM_ID==y.INTRODUCER).Select(d=>d.UName).FirstOrDefault(),
                                               transid = x.TransactionID,
                                               ReferenceNo = x.REFERENCE_NO,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               STATUS = x.STATUS
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
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               STATUS = z.STATUS
                                           }).ToList();

                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                    grid.Columns.Add(model => model.ToUser).Titled("To User").Filterable(true);
                    grid.Columns.Add(model => model.FromUser).Titled("From Member").Filterable(true);
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No").Filterable(true);
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:MM/dd/yyyy}").MultiFilterable(true);
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Time").Formatted("{0:T}").Filterable(false);
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amount").Filterable(true);
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode").Filterable(true);
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt").Filterable(true);
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description").Filterable(true);
                    grid.Columns.Add(model => model.STATUS).Titled("Req. Status").Filterable(true);
                    grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                        .RenderedAs(model => model.STATUS == "Pending" ? ("<div style='text-align:center'> <a href='" + @Url.Action("RequisitionDetails", "MerchantRequisition", new { area = "Merchant", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'   title='Edit'><i class='fa fa-edit'></i></a></div>") : "");
                    //.RenderedAs(model =>model.STATUS== "Pending")?("<div style='text-align:center'> <a href='" + @Url.Action("RequisitionDetails", "MerchantRequisition", new { area = "Merchant", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'   title='Edit'><i class='fa fa-edit'></i></a></div>"):"");


                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 10000000;
                    return grid;
                }
            }
            else
            {
                
                //IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(null);
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                 RedirectToAction("Index", "Login", new { area = "" });
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(null);
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
                    ViewBag.PaymentMode = whiteleveluser.PAYMENT_MODE;
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
                        if (TransactionInfo.BANK_ACCOUNT == "Cash Deposit-Office")//Cash Deposit AC - Office- IFSC - Office
                        {
                            //var bank = db.TBL_SETTINGS_BANK_DETAILS.FirstOrDefault(x => x.SL_NO == 1).BANK;
                            var bank = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                                 where x.SL_NO==1

                                                 //where x.INTRODUCER == dis_Id
                                                 select new
                                                 {
                                                     MEM_ID = (x.BANK + "-" + x.ACCOUNT_NO),
                                                     //UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                                     //UName = (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC)
                                                     UName = (x.BANK == "CASH - TO - OFFICE" ? "CASH - TO - OFFICE" : (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC))
                                                 }).AsEnumerable().Select(z => new MemberView
                                                 {
                                                     IDValue = z.MEM_ID.ToString(),
                                                     TextValue = z.UName
                                                 }).FirstOrDefault();
                            //TransactionInfo.BANK_ACCOUNT = bank.TextValue;//
                            TransactionInfo.BANK_ACCOUNT = "Cash Deposit-Office";
                        }
                        else {
                            TransactionInfo.BANK_ACCOUNT = TransactionInfo.BANK_ACCOUNT;
                        }
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
                    string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                    var whiteleveluser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.checkboxBilldesk == true)
                    {
                        string TsnAMt = objval.AMOUNT.ToString();
                        string UpdateAccount = AccountBalance(TsnAMt);
                        string salt = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzSaltKey"];
                            string Key = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzKey"];
                            string env = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzEnviroment"];
                            //string salt = "4NGY1NYJJP";
                            // string Key = "W8A3NHRAWY";
                            //string env = "test";
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

                       
                    }
                    else
                    {
                        //long userId = long.Parse(objval.FromUser);
                        // var membertype = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == userId).FirstOrDefault();

                        var translist = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                        if (translist != null)
                        {
                            var getsuperior = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == translist.TO_MEMBER);
                            translist.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);

                            translist.REQUEST_TIME = System.DateTime.Now;
                            //translist.BANK_ACCOUNT = objval.BANK_ACCOUNT;
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

                            #region Email Code done by Sayan at 11-10-2020
                            string name = whiteleveluser.MEMBER_NAME;
                            string Regmsg = "Hi " + whiteleveluser.MEM_UNIQUE_ID + "(" + whiteleveluser.MEMBER_NAME + ")." + " You have successfully updated requisition of amount:- " + objval.AMOUNT + " to " + getsuperior.MEM_UNIQUE_ID + ".<br /> Regards, <br/><br/>BOOM Travels";
                            EmailHelper emailhelper = new EmailHelper();
                            string msgbody = emailhelper.GetEmailTemplate(name, Regmsg, "UserEmailTemplate.html");
                            emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID.Trim(), "Requisition has been updated successfully!", msgbody);
                            #endregion

                            //EmailHelper objsms = new EmailHelper();
                            //string Regmsg = "Hi " + whiteleveluser.MEM_UNIQUE_ID + " \r\n. You have successfully updated requisition of amount:- " + objval.AMOUNT + " to " + getsuperior.MEM_UNIQUE_ID + ".\r\n Regards\r\n BOOM Travels";
                            //objsms.SendUserEmail(whiteleveluser.EMAIL_ID, "Your requisition update successfully.", Regmsg);

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
                           
                            var getsuperior = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == fromuser);
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
                            if (objval.BANK_ACCOUNT == "Cash Deposit-Office")
                            {
                                objval.REFERENCE_NO = "0000000000";
                                objval.PAYMENT_METHOD = "Cash Deposit";
                            }
                            else
                            {
                                objval.REF_NO = objval.RequisitionSendTO;
                                objval.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                            }
                            db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                            await db.SaveChangesAsync();

                            #region Email Code done by Sayan at 11-10-2020
                                string name = whiteleveluser.MEMBER_NAME;
                            string Regmsg = "Hi " + whiteleveluser.MEM_UNIQUE_ID + "(" + whiteleveluser.MEMBER_NAME + ")." + " You have successfully updated requisition of amount:- " + objval.AMOUNT + " to " + getsuperior.MEM_UNIQUE_ID + ".<br /> Regards, <br/><br/>BOOM Travels";
                            EmailHelper emailhelper = new EmailHelper();
                            string msgbody = emailhelper.GetEmailTemplate(name, Regmsg, "UserEmailTemplate.html");
                            emailhelper.SendUserEmail(whiteleveluser.EMAIL_ID.Trim(), "Your requisition has been updated successfully!", msgbody);
                            if (objval.RequisitionSendTO != "Distributor")
                            {
                                var GetWLP = db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID== whiteleveluser.UNDER_WHITE_LEVEL);
                                string WLPname = GetWLP.MEMBER_NAME;
                                string SUb_val= whiteleveluser.MEM_UNIQUE_ID+" ("+ whiteleveluser.MEMBER_NAME+")"+" send a requisition.";
                                string WLPRegmsg = "Hi " + GetWLP.MEM_UNIQUE_ID + " (" + GetWLP.MEMBER_NAME + ").<br />" + whiteleveluser.MEM_UNIQUE_ID +" ("+ whiteleveluser.MEMBER_NAME +"), send a requisition of amount:- " + objval.AMOUNT + " Rs.<br /> Regards, <br/><br/>BOOM Travels";
                                
                                string WLP_msgbody = emailhelper.GetEmailTemplate(WLPname, WLPRegmsg, "UserEmailTemplate.html");
                                emailhelper.SendUserEmail("payments.requisition@gmail.com", SUb_val, WLP_msgbody);
                            }
                            #endregion

                            //EmailHelper objsms = new EmailHelper();
                            //string Regmsg = "Hi " + whiteleveluser.MEM_UNIQUE_ID + " \r\n. You have successfully send requisition of amount:- " + objval.AMOUNT + " to " + getsuperior.MEM_UNIQUE_ID + ".\r\n Regards\r\n BOOM Travels";
                            //objsms.SendUserEmail(whiteleveluser.EMAIL_ID, "Your requisition send successfully.", Regmsg);

                        }
                        ContextTransaction.Commit();
                        return RedirectToAction("Index");
                    }
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
        public async Task<ActionResult> PaymentgatewayRechargewallet(TBL_BALANCE_TRANSFER_LOGS objval)
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        //string amount = objval.AMOUNT.ToString();
                        string amount = objval.BANK_CHARGES.ToString();
                        string WalletAmount = objval.AMOUNT.ToString();
                        string salt = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzSaltKey"];
                        string Key = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzKey"];
                        string env = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzEnviroment"];
                        //string salt = "4NGY1NYJJP";
                        // string Key = "W8A3NHRAWY";
                        //string env = "test";
                        var memberinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                        //Session["MerchantUserId"] = memberinfo.MEM_ID;
                        //Session["MerchantPassword"] = memberinfo.SECURITY_PIN_MD5;
                        string firstname = memberinfo.MEMBER_NAME.Trim();
                        string email = memberinfo.EMAIL_ID.Trim();
                        string phone = memberinfo.MEMBER_MOBILE.Trim();
                        string productinfo = "Easebuzz payment integration text";
                        string surl = "http://boomtravels.com/Merchant/MerchantPaymentgateway/EasepaySuccess";
                        string furl = "http://boomtravels.com/Merchant/MerchantPaymentgateway/EasepaySuccess";
                        //string surl = "http://localhost:56049/Merchant/MerchantPaymentgateway/EasepaySuccess";
                        //string furl = "http://localhost:56049/Merchant/MerchantPaymentgateway/EasepaySuccess";
                        string Txnid = COrelationID.Trim();
                        string UDF1 = memberinfo.MEM_ID.ToString();
                        string UDF2 = memberinfo.User_pwd.ToString();
                        string UDF3 = WalletAmount;
                        string UDF4 = "";
                        string UDF5 = "";
                        string Show_payment_mode = objval.PAYMENT_METHOD;
                        TBL_PAYMENT_GATEWAY_RESPONSE objres = new TBL_PAYMENT_GATEWAY_RESPONSE()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            RES_MSG = "",
                            RES_DATE = DateTime.Now,
                            RES_STATUS = "Pening",
                            PAY_REF_NO = "",
                            CORELATION_ID = Txnid,
                            EMAIL_ID = CurrentMerchant.EMAIL_ID,
                            MOBILE_No = CurrentMerchant.MEMBER_MOBILE,
                            TRANSACTION_AMOUNT = objval.AMOUNT,
                            RES_CODE = "",
                            TRANSACTION_DETAILS = "Process Pending",
                            AMOUNT_WITH_GST = objval.BANK_CHARGES,
                            STATUS = 0
                        };
                        db.TBL_PAYMENT_GATEWAY_RESPONSE.Add(objres);
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        Easebuzz t = new Easebuzz(salt, Key, env);
                        //string strForm = t.initiatePaymentAPI(amount, firstname, email, phone, productinfo, surl, furl, Txnid, UDF1, UDF2, UDF3, UDF4, UDF5, Show_payment_mode);
                        string strForm = t.initiatePaymentAPI(amount, firstname, email, phone, productinfo, surl, furl, Txnid, UDF1, UDF2, UDF3, UDF4, UDF5, Show_payment_mode);
                        return Content(strForm, System.Net.Mime.MediaTypeNames.Text.Html);
                    }
                    catch (Exception ex)
                    {
                        ContextTransaction.Rollback();
                        throw ex;
                    }
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
        public async Task<ActionResult> CreditRequisitionRequest(TBL_BALANCE_TRANSFER_LOGS objCredit)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long ToMemberId = 0;
                    string MemberPaymentStatus = CurrentMerchant.PAYMENT_MODE;
                    if (MemberPaymentStatus == "Direct")
                    {
                        ToMemberId = (long)CurrentMerchant.UNDER_WHITE_LEVEL;
                    }
                    else
                    {
                        ToMemberId = (long)CurrentMerchant.INTRODUCER;
                    }
                    string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                    decimal CR_Opening = 0;
                    decimal CR_Closinging = 0;
                    decimal ADD_CR_Closinging = 0;
                    decimal creditlimitBalance = 0;
                    decimal.TryParse(objCredit.AMOUNT.ToString(), out creditlimitBalance);
                    var CreditLimit_Val = db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Where(x => x.FROM_MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.SLN).FirstOrDefault();
                    if (CreditLimit_Val != null)
                    {
                        CR_Opening = (decimal)CreditLimit_Val.CREDIT_OPENING;
                        CR_Closinging = (decimal)CreditLimit_Val.CREDITCLOSING;
                        ADD_CR_Closinging = CR_Closinging + creditlimitBalance;
                    }
                    else
                    {
                        CR_Closinging = 0;
                        ADD_CR_Closinging = creditlimitBalance;
                    }
                    TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION objLimit = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                    {
                        TO_MEM_ID = ToMemberId,
                        FROM_MEM_ID = CurrentMerchant.MEM_ID,
                        CREDIT_DATE = DateTime.Now,
                        //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                        CREDIT_AMOUNT = objCredit.AMOUNT,
                        GST_VAL = 0,
                        GST_AMOUNT = 0,
                        TDS_VAL = 0,
                        TDS_AMOUNT = 0,
                        CREDIT_NOTE_DESCRIPTION = objCredit.TRANSACTION_DETAILS,
                        CREDIT_STATUS = false,
                        CREDIT_OPENING = CR_Closinging,
                        CREDITCLOSING = ADD_CR_Closinging,
                        CREDIT_TRN_TYPE = "CR",
                        CORELATIONID = COrelationID,
                        STATUS="PENDING"
                    };
                    db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(objLimit);
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    EmailHelper emailhelper = new EmailHelper();
                    var GetWLP = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.UNDER_WHITE_LEVEL);
                    string WLPname = GetWLP.MEMBER_NAME;
                    string SUb_val = CurrentMerchant.MEM_UNIQUE_ID + " (" + CurrentMerchant.MEMBER_NAME + ")" + " send a credit requisition.";
                    string WLPRegmsg = "Hi " + GetWLP.MEM_UNIQUE_ID + " (" + GetWLP.MEMBER_NAME + ").<br />" + CurrentMerchant.MEM_UNIQUE_ID + " (" + CurrentMerchant.MEMBER_NAME + "), send a credit requisition of amount:- " + objCredit.AMOUNT + " Rs.<br /> Regards, <br/><br/>BOOM Travels";

                    string WLP_msgbody = emailhelper.GetEmailTemplate(WLPname, WLPRegmsg, "UserEmailTemplate.html");
                    emailhelper.SendUserEmail("payments.requisition@gmail.com", SUb_val, WLP_msgbody);
                    return RedirectToAction("CreditRequisitionList");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Commit();
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
                                             //UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                             //UName = (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC)
                                             UName = (x.BANK == "CASH - TO - OFFICE" ? "CASH - TO - OFFICE" : (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC))
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
                                             //UName = (x.BANK + "-" + x.ACCOUNT_NO)
                                             //UName = (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC)
                                             UName = (x.BANK == "CASH - TO - OFFICE" ? "CASH - TO - OFFICE" : (x.BANK + " AC - " + x.ACCOUNT_NO + "- IFSC - " + x.IFSC))
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

        public ActionResult EasebuzzPaymentGateway()
        {
            return View();
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

        public ActionResult EasepaySuccess()
        {
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
                        //string Responseval = ViewBag.messagevalue;
                        ViewBag.TXnStatus = Request.Form["status"];
                        ViewBag.txnid = Request.Form["txnid"];
                        ViewBag.txnAmt =Request.Form["amount"];
                        decimal TranAmount = Convert.ToDecimal(Request.Form["amount"]);
                        string ValueCheck = Request.Form["amount"].ToString();
                        ViewBag.iewBegCheckError = ValueCheck;
                        ViewBag.MEMID = Session["MerchantUserId"];
                        //initpage();
                        Session["PaymentGatewayAmount"] = ValueCheck;
                        string MEMID = Session["MerchantUserId"].ToString();
                        string MEM_Pass = Session["MerchantPassword"].ToString();
                        ViewBag.AountVAl = Session["PaymentGatewayAmount"];
                        //return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });

                        //TempData["EaseBuzzResponse"] = ViewBag.messagevalue;
                        //TempData["TXnStatus"] = ViewBag.TXnStatus;
                        //TempData["txnid"] = ViewBag.txnid;
                        //TempData["txnAmt"] = ViewBag.txnAmt;
                        //Session["MerchantUserId"] = MerchantGetMember.MEM_ID;

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
                return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
            }            
        }
        public string AccountBalance(string Amount)
        {
            var db = new DBContext();
            decimal Baln = 0;
            decimal OpenningBal = 0;
            decimal ColsingBal = 0;
            decimal MainBaln = 0;
            decimal AddmainBal = 0;
            decimal TransactionAmt = 0;
            decimal.TryParse(Amount,out TransactionAmt);
            long MEM_IDVAue = 0;
            long.TryParse(Session["MerchantUserId"].ToString(),out MEM_IDVAue);

            string COrelationID = Settings.GetUniqueKey(MEM_IDVAue.ToString());
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                    var accountdetails = db.TBL_ACCOUNTS.Where(X => X.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
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
                            MEM_ID = CurrentMerchant.MEM_ID,
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
                            MEM_ID = CurrentMerchant.MEM_ID,
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
                catch (Exception ex )
                {
                    ContextTransaction.Rollback();
                    throw;
                    return "false";
                }
            }
        }
        public ActionResult EasepayFailure()
        {
            try
            {
                string salt = "4NGY1NYJJP";
                string Key = "W8A3NHRAWY";

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
                        Response.Write(Request.Form);
                        ViewBag.messagevalue = Request.Form;
                    }
                    else
                    {
                        Response.Write(Request.Form);
                        ViewBag.messagevalue = Request.Form;
                    }
                    //Hash value did not matched
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }
            return View();
        }
        [HttpPost]
        public ActionResult CallEasebuzzAPI()
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
            //return View();
        }

        public ActionResult CreditRequisitionList()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();               
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
        public PartialViewResult CreditRequisitionIndexGrid(string DateFrom = "", string Date_To = "")
        {
            try
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
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);

                    var memberinfo = (from tblcre in db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in db.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= Date_To_Val
                                      where tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          TO_MEMBER = (db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID==tblcre.TO_MEM_ID).MEMBER_ROLE==1?"ADMIN":"DISTRIBUTOR"),
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS=tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          TOUser= z.TO_MEMBER,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CreditRequisitionIndexGrid", memberinfo);
                }
                else
                {
                    var memberinfo = (from tblcre in db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in db.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= Date_To_Val
                                      where tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID 
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          TO_MEMBER = (db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == tblcre.TO_MEM_ID).MEMBER_ROLE == 1 ? "ADMIN" : "DISTRIBUTOR"),
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS=tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          TOUser = z.TO_MEMBER,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CreditRequisitionIndexGrid", memberinfo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> CheckTotalTransactionAmount(string PaymentMode, string transactionAmount)
        {
            //initpage();////
            try
            {
                var context = new DBContext();
                string DebitCardLess = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzDebitCardLess"];
                string DebitCardgreater = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzDebitCardGreater"];
                string CreditCard = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzCreditCard"];
                string NetBanking = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzNetBanking"];
                string UPI = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzUPI"];
                string MobileWallet = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzMobileWallet"];
                string EMI = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzEMI"];
                string OlaMoney = System.Configuration.ConfigurationSettings.AppSettings["EaseBuzzOlaMoney"];
                decimal DebitCard_LessValue = 0;
                decimal.TryParse(DebitCardLess,out DebitCard_LessValue);
                decimal DebitCard_GreterValue = 0;
                decimal.TryParse(DebitCardgreater, out DebitCard_GreterValue);
                decimal CreditCard_Value = 0;
                decimal.TryParse(CreditCard, out CreditCard_Value);
                decimal NetBanking_Value = 0;
                decimal.TryParse(NetBanking, out NetBanking_Value);
                decimal UPI_Value = 0;
                decimal.TryParse(UPI, out UPI_Value);
                decimal MobileWallet_Value = 0;
                decimal.TryParse(MobileWallet, out MobileWallet_Value);
                decimal EMI_Value = 0;
                decimal.TryParse(EMI, out EMI_Value);
                decimal OlaMoney_Value = 0;
                decimal.TryParse(OlaMoney, out OlaMoney_Value);
                decimal Transaction_AMount = 0;
                decimal.TryParse(transactionAmount, out Transaction_AMount);
                decimal CalculateAmount = 0;
                decimal GSTAmount = 0;
                decimal GrossAmount = 0;
                if (PaymentMode == "DC")
                {
                    if (Transaction_AMount < 2000)
                    {
                        CalculateAmount= ((Transaction_AMount* DebitCard_LessValue)/100);
                        GSTAmount = ((CalculateAmount * 18) / 100);
                        GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                    }
                    else
                    {
                        CalculateAmount = ((Transaction_AMount * DebitCard_GreterValue) / 100);
                        GSTAmount = ((CalculateAmount * 18) / 100);
                        GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                    }
                }
                else if (PaymentMode == "CC") {
                    CalculateAmount = ((Transaction_AMount * CreditCard_Value) / 100);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else if (PaymentMode == "NB") {
                    CalculateAmount = (NetBanking_Value);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else if (PaymentMode == "UPI")
                {
                    CalculateAmount = (UPI_Value);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else if (PaymentMode == "MW") {
                    CalculateAmount = ((Transaction_AMount * MobileWallet_Value) / 100);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else if (PaymentMode == "OM")
                {
                    CalculateAmount = ((Transaction_AMount * OlaMoney_Value) / 100);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else if (PaymentMode == "EMI")
                {
                    CalculateAmount = ((Transaction_AMount * EMI_Value) / 100);
                    GSTAmount = ((CalculateAmount * 18) / 100);
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                else {
                    CalculateAmount = 0;
                    GSTAmount = 0;
                    GrossAmount = Transaction_AMount + CalculateAmount + GSTAmount;
                }
                return Json(new { GrossAmount= GrossAmount,GST= GSTAmount,NetAmount= CalculateAmount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
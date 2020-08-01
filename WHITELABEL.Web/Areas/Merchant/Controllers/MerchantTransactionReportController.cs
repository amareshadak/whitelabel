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

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantTransactionReportController : MerchantBaseController
    {
        // GET: Merchant/MerchantTransactionReport
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
                initpage();
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

        public PartialViewResult IndexGrid(string search = "",string DateFrom="",string Date_To="")
        {
            var db = new DBContext();

            
            if (DateFrom != "" && Date_To != "")
            {
                //string FromDATE = string.Empty;
                //string TO_DATE = string.Empty;
                //FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                //DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                //string From_TO = string.Empty;
                //TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                //DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(search, CurrentMerchant.MEM_ID, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else
            {

               
                var transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(search, CurrentMerchant.MEM_ID, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }

           
        }

        // Admin/WL
        public FileResult ExportIndexMerchantTransReport(string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportMerchantTableGrid(statusval, DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_ACCOUNTS> gridRow in grid.Rows)
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

        private IGrid<TBL_ACCOUNTS> CreateExportMerchantTableGrid(string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
             var  transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(statusval, CurrentMerchant.MEM_ID, DateFrom, Date_To);            
                long mem_id = long.Parse(CurrentMerchant.MEM_ID.ToString());
                
                IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(transactionlistvalue);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
                grid.Columns.Add(model => model.UserName).Titled("User Name");
                //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
                grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
                grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CR_Col).Titled("CR");
            grid.Columns.Add(model => model.DR_Col).Titled("DR");
            //grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
                grid.Columns.Add(model => model.CLOSING).Titled("Closing");
                grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
                grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 1000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            
        }

        public ActionResult DailyTransaction()
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
        public PartialViewResult DailyTransactiongrid(string search = "")
        {
            var db = new DBContext();
            var transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(search, CurrentMerchant.MEM_ID,"","");
            return PartialView("DailyTransactiongrid", transactionlistvalue);
            
        }
        public ActionResult _GridFooter()
        {
            var db = new DBContext();
            var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                        join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                        where x.MEM_ID == CurrentMerchant.MEM_ID
                                        select new
                                        {
                                            CommissionAmt = x.COMM_AMT
                                        }).AsEnumerable().Select(z => new TBL_ACCOUNTS
                                        {                                           
                                            COMM_AMT = z.CommissionAmt
                                        }).ToList();
            return PartialView("_GridFooter", transactionlistvalue);
        }

        public ActionResult DMRTransactionList()
        {
            initpage();
            return View();
        }
        public PartialViewResult DMR_TransactionGridView()
        {
            try
            {
                var db = new DBContext();
                var GetTransaction = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("DMR_TransactionGridView", GetTransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public PartialViewResult DMRGridview()
        {
            try
            {
                var db = new DBContext();
                var GetTransaction = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("DMRGridview", GetTransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult MerchantCreditLimitList()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
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

        public PartialViewResult MerchantCreditReportindexgrid(string DateFrom="",string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
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
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("MerchantCreditReportindexgrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("MerchantCreditReportindexgrid", memberinfo);
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult MerchantRDSBookingReport()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
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
        public PartialViewResult MerchantRDSBookingReportgrid()
        {
            try
            {
                var dbcontext = new DBContext();
                var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).INTRODUCER;
                var memberinfo = (from tblrds in dbcontext.TBL_FINAL_RDS_BOOKING                                  
                                  where tblrds.MER_ID == CurrentMerchant.MEM_ID
                                  select new
                                  {
                                      Sln=tblrds.SLN,
                                      tran_id = tblrds.TRAN_ID,

                                  }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                  {
                                      SLN = z.Sln,
                                     
                                  }).ToList();
                return PartialView("MerchantCreditReportindexgrid", memberinfo);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult MerchantPNRCancellationList()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
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
        public PartialViewResult MerchantPNRCancellationgrid(string DateFrom="", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var Mem_info = dbcontext.TBL_FINAL_CANCELLATION.Where(x => x.MER_ID == CurrentMerchant.MEM_ID && x.TRN_DATE>= Date_From_Val && x.TRN_DATE<= Date_To_Val).ToList();
                    return PartialView("MerchantPNRCancellationgrid", Mem_info);
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var Mem_info = dbcontext.TBL_FINAL_CANCELLATION.Where(x => x.MER_ID == CurrentMerchant.MEM_ID && x.TRN_DATE == Todaydate).ToList();
                    return PartialView("MerchantPNRCancellationgrid", Mem_info);
                }

               
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        private IGrid<TBL_FINAL_CANCELLATION> CreateExportCancellationableGrid(string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            if (DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                  where x.MER_ID == CurrentMerchant.MEM_ID && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                select new
                                  {
                                      SLN = x.SLN,
                                      //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                      TRAN_ID = x.TRN_ID,
                                      PNR = x.PNR_NO,
                                      OPR_ID = x.OPR_ID,
                                      BOOKING_AMT = x.REFUND_AMT,
                                      //PG_CHARGE = x.PG_CHARGE,
                                      TRAN_DATE = x.TRN_DATE,
                                      TDR_CAN = x.TDR_CAN,
                                      CANCELLATION_ID = x.CANCELLATION_ID,
                                      CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                      CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                      SYSTEM_DATE = x.SYSTEM_DATE,
                                      WT_AUTO_CAN = x.WT_AUTO_CAN,
                                      //APP_CODE = x.APP_CODE,
                                      //PAYMODE = x.PAYMODE,
                                      //SECURITY_ID = x.SECURITY_ID,
                                      //RU = x.RU,
                                      //PAY_REQ = x.PAY_REQ,
                                      //RET_RES = x.RET_RES,
                                      MER_RAIL_ID = y.RAIL_ID,
                                      PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                      PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                      PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                      PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                      PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                      PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                      ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                      ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                      ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                      ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                      REMARK = x.REMARKS,
                                      NOTES = x.NOTES,
                                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = x.CORRELATION_ID,
                                      GST_RATE = x.GST_RATE,
                                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                  }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                  {
                                      SLN = z.SLN,
                                      //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                      TRN_ID = z.TRAN_ID,
                                      PNR_NO = z.PNR,
                                      OPR_ID = z.OPR_ID,
                                      REFUND_AMT = z.BOOKING_AMT,
                                      //PG_CHARGE = z.PG_CHARGE,
                                      TRN_DATE = z.TRAN_DATE,
                                      TDR_CAN = z.TDR_CAN,
                                      CANCELLATION_ID = z.CANCELLATION_ID,
                                      CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                      SYSTEM_DATE = z.SYSTEM_DATE,
                                      WT_AUTO_CAN = z.WT_AUTO_CAN,
                                      //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                      //APP_CODE = z.APP_CODE,
                                      //PAYMODE = z.PAYMODE,
                                      //SECURITY_ID = z.SECURITY_ID,
                                      //RU = z.RU,
                                      //PAY_REQ = z.PAY_REQ,
                                      //RET_RES = z.RET_RES,
                                      MER_RAIL_ID = z.MER_RAIL_ID,
                                      PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                      PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                      PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                      PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                      PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                      PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                      ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                      ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                      ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                      ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                      REMARKS = z.REMARK,
                                      NOTES = z.NOTES,
                                      ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = z.CORRELATION_ID,
                                      GST_RATE = z.GST_RATE,
                                      MERCHANT_NAME = z.MERCHANT_NAME,
                                  }).ToList();
                //var memberinfo = dbcontext.TBL_FINAL_RDS_BOOKING.Where(x => x.WLP_ID == CurrentUser.USER_ID ).ToList().OrderByDescending(x => x.TRAN_DATE);
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.WT_AUTO_CAN).Titled("Wt Auto Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;


                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;

                //}
                //foreach (IGridColumn row in grid.Rows)
                //{
                //    row.CssClasses = "red";

                //}

                return grid;
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                  where x.MER_ID == CurrentMerchant.MEM_ID && x.TRN_DATE == Todaydate
                                  select new
                                  {
                                      SLN = x.SLN,
                                      //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                      TRAN_ID = x.TRN_ID,
                                      PNR = x.PNR_NO,
                                      OPR_ID = x.OPR_ID,
                                      BOOKING_AMT = x.REFUND_AMT,
                                      //PG_CHARGE = x.PG_CHARGE,
                                      TRAN_DATE = x.TRN_DATE,
                                      TDR_CAN = x.TDR_CAN,
                                      CANCELLATION_ID = x.CANCELLATION_ID,
                                      CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                      CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                      SYSTEM_DATE = x.SYSTEM_DATE,
                                      WT_AUTO_CAN = x.WT_AUTO_CAN,
                                      //APP_CODE = x.APP_CODE,
                                      //PAYMODE = x.PAYMODE,
                                      //SECURITY_ID = x.SECURITY_ID,
                                      //RU = x.RU,
                                      //PAY_REQ = x.PAY_REQ,
                                      //RET_RES = x.RET_RES,
                                      MER_RAIL_ID = y.RAIL_ID,
                                      PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                      PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                      PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                      PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                      PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                      PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                      ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                      ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                      ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                      ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                      REMARK = x.REMARKS,
                                      NOTES = x.NOTES,
                                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = x.CORRELATION_ID,
                                      GST_RATE = x.GST_RATE,
                                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                  }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                  {
                                      SLN = z.SLN,
                                      //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                      TRN_ID = z.TRAN_ID,
                                      PNR_NO = z.PNR,
                                      OPR_ID = z.OPR_ID,
                                      REFUND_AMT = z.BOOKING_AMT,
                                      //PG_CHARGE = z.PG_CHARGE,
                                      TRN_DATE = z.TRAN_DATE,
                                      TDR_CAN = z.TDR_CAN,
                                      CANCELLATION_ID = z.CANCELLATION_ID,
                                      CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                      SYSTEM_DATE = z.SYSTEM_DATE,
                                      WT_AUTO_CAN = z.WT_AUTO_CAN,
                                      //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                      //APP_CODE = z.APP_CODE,
                                      //PAYMODE = z.PAYMODE,
                                      //SECURITY_ID = z.SECURITY_ID,
                                      //RU = z.RU,
                                      //PAY_REQ = z.PAY_REQ,
                                      //RET_RES = z.RET_RES,
                                      MER_RAIL_ID = z.MER_RAIL_ID,
                                      PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                      PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                      PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                      PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                      PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                      PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                      ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                      ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                      ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                      ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                      REMARKS = z.REMARK,
                                      NOTES = z.NOTES,
                                      ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = z.CORRELATION_ID,
                                      GST_RATE = z.GST_RATE,
                                      MERCHANT_NAME = z.MERCHANT_NAME,
                                  }).ToList();


                //var memberinfo = dbcontext.TBL_FINAL_RDS_BOOKING.Where(x => x.WLP_ID == CurrentUser.USER_ID ).ToList().OrderByDescending(x => x.TRAN_DATE);
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.WT_AUTO_CAN).Titled("Wt Auto Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;


                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;

                //}
                //foreach (IGridColumn row in grid.Rows)
                //{
                //    row.CssClasses = "red";

                //}

                return grid;
            }
                
        }
        [HttpGet]
        public FileResult ExportCancellationIndex(string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_CANCELLATION> grid = CreateExportCancellationableGrid(DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_CANCELLATION> gridRow in grid.Rows)
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
    }
}
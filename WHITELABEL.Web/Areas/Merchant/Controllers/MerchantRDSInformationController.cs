﻿using System;
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
using System.Threading.Tasks;
using WHITELABEL.Web.Areas.Merchant.Models;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRDSInformationController : MerchantBaseController
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
        // GET: Merchant/MerchantRDSInformation
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var db = new DBContext();
               
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
        public PartialViewResult IndexGrid(string DateFrom = "", string Date_To = "")
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
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
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
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE == Todaydate
                                          select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
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
                                          REMARK=x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SerialNo=index+1,
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
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

                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                return PartialView("IndexGrid", RDSbookinglist);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        [HttpGet]
        public FileResult ExportIndex(string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportableGrid(DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_RDS_BOOKING> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "RDSCancellationReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_FINAL_RDS_BOOKING> CreateExportableGrid(string DateFrom = "", string Date_To = "")
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
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
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
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
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

                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);


                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                  where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE == Todaydate
                                  select new
                                  {
                                      SLN = x.SLN,
                                      BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                      TRAN_ID = x.TRAN_ID,
                                      PNR = x.PNR,
                                      OPR_ID = x.OPR_ID,
                                      BOOKING_AMT = x.BOOKING_AMT,
                                      PG_CHARGE = x.PG_CHARGE,
                                      TRAN_DATE = x.TRAN_DATE,
                                      TRAN_STATUS = x.TRAN_STATUS,
                                      BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                      BOOKING_TIME = x.BOOKING_TIME,
                                      CURRENCY_TYPE = x.CURRENCY_TYPE,
                                      APP_CODE = x.APP_CODE,
                                      PAYMODE = x.PAYMODE,
                                      SECURITY_ID = x.SECURITY_ID,
                                      RU = x.RU,
                                      PAY_REQ = x.PAY_REQ,
                                      RET_RES = x.RET_RES,
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

                                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = x.CORRELATION_ID,
                                      GST_RATE = x.GST_RATE,
                                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                  }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                  {
                                      SLN = z.SLN,
                                      BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                      TRAN_ID = z.TRAN_ID,
                                      PNR = z.PNR,
                                      OPR_ID = z.OPR_ID,
                                      BOOKING_AMT = z.BOOKING_AMT,
                                      PG_CHARGE = z.PG_CHARGE,
                                      TRAN_DATE = z.TRAN_DATE,
                                      TRAN_STATUS = z.TRAN_STATUS,
                                      BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                      BOOKING_TIME = z.BOOKING_TIME,
                                      CURRENCY_TYPE = z.CURRENCY_TYPE,
                                      APP_CODE = z.APP_CODE,
                                      PAYMODE = z.PAYMODE,
                                      SECURITY_ID = z.SECURITY_ID,
                                      RU = z.RU,
                                      PAY_REQ = z.PAY_REQ,
                                      RET_RES = z.RET_RES,
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
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);


                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
        }

        #region Merchant Refund List
        public ActionResult MerchantRDSRefundList()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var db = new DBContext();

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
        public PartialViewResult MerchantRDSRefundGrid(string DateFrom = "", string Date_To = "")
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
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS== "Failed"
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
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
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("MerchantRDSRefundGrid", RDSbookinglist);
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_STATUS == "Failed" && x.TRAN_DATE == Todaydate
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
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
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("MerchantRDSRefundGrid", RDSbookinglist);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> MerchantRDSRefundExportableGrid(string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            if(DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
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
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
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
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);


                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else{
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                  where x.MER_ID == CurrentMerchant.MEM_ID && x.TRAN_STATUS == "Failed" && x.TRAN_DATE == Todaydate
                                  select new
                                  {
                                      SLN = x.SLN,
                                      BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                      TRAN_ID = x.TRAN_ID,
                                      PNR = x.PNR,
                                      OPR_ID = x.OPR_ID,
                                      BOOKING_AMT = x.BOOKING_AMT,
                                      PG_CHARGE = x.PG_CHARGE,
                                      TRAN_DATE = x.TRAN_DATE,
                                      TRAN_STATUS = x.TRAN_STATUS,
                                      BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                      BOOKING_TIME = x.BOOKING_TIME,
                                      CURRENCY_TYPE = x.CURRENCY_TYPE,
                                      APP_CODE = x.APP_CODE,
                                      PAYMODE = x.PAYMODE,
                                      SECURITY_ID = x.SECURITY_ID,
                                      RU = x.RU,
                                      PAY_REQ = x.PAY_REQ,
                                      RET_RES = x.RET_RES,
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

                                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                      CORRELATION_ID = x.CORRELATION_ID,
                                      GST_RATE = x.GST_RATE,
                                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                  }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                  {
                                      SLN = z.SLN,
                                      BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                      TRAN_ID = z.TRAN_ID,
                                      PNR = z.PNR,
                                      OPR_ID = z.OPR_ID,
                                      BOOKING_AMT = z.BOOKING_AMT,
                                      PG_CHARGE = z.PG_CHARGE,
                                      TRAN_DATE = z.TRAN_DATE,
                                      TRAN_STATUS = z.TRAN_STATUS,
                                      BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                      BOOKING_TIME = z.BOOKING_TIME,
                                      CURRENCY_TYPE = z.CURRENCY_TYPE,
                                      APP_CODE = z.APP_CODE,
                                      PAYMODE = z.PAYMODE,
                                      SECURITY_ID = z.SECURITY_ID,
                                      RU = z.RU,
                                      PAY_REQ = z.PAY_REQ,
                                      RET_RES = z.RET_RES,
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
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);


                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
        public FileResult ExportMerchantRDSRefund(string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = MerchantRDSRefundExportableGrid(DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_RDS_BOOKING> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "RDSCancellationReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        #endregion
    }
}
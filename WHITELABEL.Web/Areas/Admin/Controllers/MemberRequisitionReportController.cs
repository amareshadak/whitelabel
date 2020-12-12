﻿using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberRequisitionReportController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Member Requisition";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                //Session["UserId"] = currUser.MEM_ID;
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["WhiteLevelUserId"] != null)
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
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "AdminLogin", new { area = "Admin" }));
                    //Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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
                Console.WriteLine(e.InnerException);
                return;
            }
        } 
        // GET: Admin/MemverRequisitionReport
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();                    
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
            //return View();
        }
        public PartialViewResult IndexGrid(string search="",string DateFrom="",string Date_To="")
        {
            var Adminrequisition = MemberRequisitionReportModel.GetAdminRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), search, DateFrom, Date_To);
            return PartialView("IndexGrid", Adminrequisition);
        }

        public ActionResult SuperRequisitionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
        public PartialViewResult SuperRequisitonGrid(string search = "",string Status = "")
        {
            if (Status == "" && search == "")
            {
                var Adminrequisition = MemberRequisitionReportModel.GetAllSuperRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("SuperRequisitonGrid", Adminrequisition);
            }
            else if (Status != "" && search == "")
            {
                var Adminrequisition = MemberRequisitionReportModel.GetAllSuperRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("SuperRequisitonGrid", Adminrequisition);
            }
            else if (Status == "" && search != "")
            {
                var Adminrequisition = MemberRequisitionReportModel.GetSuperRequisitionReport(search, Status);
                return PartialView("SuperRequisitonGrid", Adminrequisition);
            }
            else if (Status != "" && search != "")
            {
                var Adminrequisition = MemberRequisitionReportModel.GetSuperRequisitionReport(search, Status);
                return PartialView("SuperRequisitonGrid", Adminrequisition);
            }

            return PartialView();
        }

        public ActionResult DistributorRequisitionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x=> SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();

                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                             //where SuperMemId.Contains(x.INTRODUCER.ToString())
                                         where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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

        public PartialViewResult DistributorReqGrid(string Super = "", string search = "", string Status = "",string DateFrom="", string Date_To="")
        {
            if (Super=="" && search == "" && Status == "")
            {
                var distributorlist = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            else if (Super == "" && search == "" && Status == "")
            {
                var distributorlist = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            else if (Super !="" && search != "" && Status != "")
            {
                var distributorlist = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super,search.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            else if (Super != "" && search != "" && Status == "")
            {
                var distributorlist = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super,search.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            else if (Super != "" && search == "" && Status != "")
            {
                var distributorlist = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super,search.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            else if (Super != "" && search == "" && Status == "")
            {
                var distributorlist = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super,search.ToString(), Status, DateFrom, Date_To);
                return PartialView("DistributorReqGrid", distributorlist);
            }
            return PartialView();
        }

        public ActionResult MerchantRequisitionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                             //where DistributorMemId.Contains(x.INTRODUCER.ToString())
                                         where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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

        public PartialViewResult MercantRequisitionGrid(string Super="", string Distributor="", string search = "", string Status = "",string DateFrom = "" , string Date_To="")
        {
            if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, search, Status, DateFrom, Date_To);
                //var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if(string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, search, Status, DateFrom, Date_To);
                return PartialView("MercantRequisitionGrid", MerchantList);
            }

            return PartialView();
        }
        // White Level
        [HttpGet]
        public FileResult ExportWhiteLevelIndex(string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExporWhiteLevelGrid(statusval, DateFrom, Date_To);
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
                return File(package.GetAsByteArray(), "application/unknown", "AdminRequisitionReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExporWhiteLevelGrid(string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            var Adminrequisition = MemberRequisitionReportModel.GetAdminRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.APPROVED_BY).Titled("Apprv By");
            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 10000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }
        // Super Level
        [HttpGet]
        public FileResult ExportSuperIndex(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExporSuperGrid(Disid,statusval);
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
                return File(package.GetAsByteArray(), "application/unknown", "AdminRequisitionReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExporSuperGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition =new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (statusval == "" && Disid == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllSuperRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
                
            }
            else if (statusval != "" && Disid == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllSuperRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
                
            }
            else if (statusval == "" && Disid != "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetSuperRequisitionReport(Disid, statusval);
                
            }
            else if (statusval != "" && Disid != "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetSuperRequisitionReport(Disid, statusval);
                
            }
            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.APPROVED_BY).Titled("Apprv By");
            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }
        //Distributor
        [HttpGet]
        public FileResult ExportDistributorIndex(string Super, string Disid, string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExporDistributorGrid(Super, Disid, statusval, DateFrom, Date_To);
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
                return File(package.GetAsByteArray(), "application/unknown", "AdminRequisitionReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExporDistributorGrid(string Super, string Disid, string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();

            if (Super == "" && Disid == "" && statusval == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (Super == "" && Disid == "" && statusval == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (Super != "" && Disid != "" && statusval != "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (Super != "" && Disid != "" && statusval == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (Super != "" && Disid == "" && statusval != "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetDistributorRequisitionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (Super != "" && Disid == "" && statusval == "")
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                
            }

            //if (statusval == "" && Disid == "")
            //{
            //    Adminrequisition = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

            //}
            //else if (statusval != "" && Disid == "")
            //{
            //    Adminrequisition = MemberRequisitionReportModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

            //}
            //else if (statusval == "" && Disid != "")
            //{
            //    Adminrequisition = MemberRequisitionReportModel.GetDistributorRequisitionReport(Disid, statusval);

            //}
            //else if (statusval != "" && Disid != "")
            //{
            //    Adminrequisition = MemberRequisitionReportModel.GetDistributorRequisitionReport(Disid, statusval);

            //}
            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.APPROVED_BY).Titled("Apprv By");
            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 1000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }
        [HttpPost]
        public JsonResult GetDistributor(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMerchant(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        //Merchant
        [HttpGet]
        public FileResult ExportMerchantIndex(string Super,string Distributor, string Disid, string statusval, string DateFrom = "", string Date_To = "")        
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportMerchantGrid(Super, Distributor, Disid, statusval, DateFrom, Date_To);
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
                return File(package.GetAsByteArray(), "application/unknown", "AdminRequisitionReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportMerchantGrid(string Super, string Distributor, string Disid, string statusval, string DateFrom = "", string Date_To = "")        
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, Disid, statusval, DateFrom, Date_To);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, Disid, statusval, DateFrom, Date_To);
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, Disid, statusval, DateFrom, Date_To);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(Super, Distributor, Disid, statusval, DateFrom, Date_To);
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, Disid, statusval, DateFrom, Date_To);
                //Adminrequisition = MemberRequisitionReportModel.GetAllMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                Adminrequisition = MemberRequisitionReportModel.GetMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, Disid, statusval, DateFrom, Date_To);
            }
            

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.APPROVED_BY).Titled("Apprv By");
            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 1000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }
            
            return grid;
        }





        public ActionResult PaymentGatewayWalletRechargeReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
        public PartialViewResult PaymentGatewayWalletRechargeReportIndexGrid()
        {
            var db = new DBContext();
            var getResponse = (from x in db.TBL_PAYMENT_GATEWAY_RESPONSE
                               join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                               select new
                               {
                                   SLN = x.SLN,
                                   EmailId = x.EMAIL_ID,
                                   MOBILE = x.MOBILE_No,
                                   STATUS = x.RES_STATUS,
                                   RED_DATE = x.RES_DATE,
                                   CORELATIONID = x.CORELATION_ID,
                                   TXNREF_ID = x.PAY_REF_NO,
                                   TXN_AMOUNT = x.TRANSACTION_AMOUNT,
                                   MEMBERNAME = y.MEMBER_NAME,
                                   MEMBER_COMPANY = y.COMPANY,
                                   RES_CODE = x.RES_CODE
                               }).AsEnumerable().Select((z, index) => new TBL_PAYMENT_GATEWAY_RESPONSE
                               {
                                   Serial_No = index + 1,
                                   SLN = z.SLN,
                                   EMAIL_ID = z.EmailId,
                                   MOBILE_No = z.MOBILE,
                                   RES_STATUS = z.STATUS,
                                   RES_DATE = z.RED_DATE,
                                   PAY_REF_NO = z.TXNREF_ID,
                                   TRANSACTION_AMOUNT = z.TXN_AMOUNT,
                                   CORELATION_ID = z.CORELATIONID,
                                   Member_Name = z.MEMBERNAME,
                                   Member_Company_Name = z.MEMBER_COMPANY,
                                   RES_CODE = z.RES_CODE
                               }).ToList();
            return PartialView("PaymentGatewayWalletRechargeReportIndexGrid", getResponse);

        }
        [HttpPost]
        public JsonResult fetchPaymentGatewayTransaction(string SlnValue = "")
        {
            try
            {
                if (SlnValue != "")
                {
                    var db = new DBContext();
                    long Sln = 0;
                    long.TryParse(SlnValue, out Sln);
                    var GettxnInfo = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.SLN == Sln);
                    //return Json(GettxnInfo,JsonRequestBehavior.AllowGet);
                    return Json(new { Result = GettxnInfo, Status = "0" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { Result = "Please Contact to Administrator", Status = "1" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Please Contact to Administrator", Status = "1" }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        [HttpPost]
        public JsonResult POSTProcessRechargewallet(TBL_PAYMENT_GATEWAY_RESPONSE objres)
        {
            var db = new DBContext();
            decimal Baln = 0;
            decimal OpenningBal = 0;
            decimal ColsingBal = 0;
            decimal MainBaln = 0;
            decimal AddmainBal = 0;
            decimal TransactionAmt = 0;
            decimal.TryParse(objres.TRANSACTION_AMOUNT.ToString(), out TransactionAmt);
            long MEM_IDVAue = 0;
            //long.TryParse(Session["MerchantUserId"].ToString(), out MEM_IDVAue);
            long MEM_ID = 0;
            long.TryParse(objres.MEM_ID.ToString(), out MEM_ID);
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
                        //db.SaveChanges();
                        var getPayRes = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.SLN == objres.SLN);
                        getPayRes.RES_STATUS = "SUCCESS";
                        getPayRes.TRANSACTION_DETAILS = "Offline Process";
                        getPayRes.TRANSACTION_DETAILS = "Offline Process";
                        getPayRes.RES_CODE = "0300";
                        db.Entry(getPayRes).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Transaction Successfull", JsonRequestBehavior.AllowGet);
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
                        var getPayRes = db.TBL_PAYMENT_GATEWAY_RESPONSE.FirstOrDefault(x => x.SLN == objres.SLN);
                        getPayRes.RES_STATUS = "SUCCESS";
                        getPayRes.TRANSACTION_DETAILS = "Offline Process";
                        getPayRes.TRANSACTION_DETAILS = "Offline Process";
                        getPayRes.RES_CODE = "0300";
                        db.Entry(getPayRes).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Transaction Successfull", JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    return Json("Please try again later", JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
        }
        public string AccountBalance(string Amount, string MEMID)
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
            long.TryParse(MEMID, out MEM_ID);
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

    }
}
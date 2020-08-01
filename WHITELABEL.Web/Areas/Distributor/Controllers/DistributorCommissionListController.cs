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
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Device.Location;
using WHITELABEL.Web.Controllers;
using System.Web.UI;
using System.Text.RegularExpressions;


namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    public class DistributorCommissionListController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Distributor";

                if (Session["DistributorUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Distributor/DistributorCommissionList
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
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                var MerchantComm = (from CommMer in dbcontext.TBL_COMM_SLAB_MOBILE_RECHARGE
                                    join WLComm in dbcontext.TBL_WHITE_LEVEL_COMMISSION_SLAB on CommMer.SLAB_ID equals WLComm.SLN
                                    where CommMer.MEM_ID == MemberCurrentUser.MEM_ID
                                    select new
                                    {
                                        SLAB_NAME = WLComm.SLAB_NAME,
                                        Operator_Name = CommMer.OPERATOR_NAME,
                                        Operator_Code = CommMer.OPERATOR_CODE,
                                        Operator_Type = CommMer.OPERATOR_TYPE,
                                        MER_COMMTYPE = CommMer.COMMISSION_TYPE,
                                        MER_COMMAMT = CommMer.MERCHANT_COM_PER
                                    }).AsEnumerable().Select(z => new TBL_COMM_SLAB_MOBILE_RECHARGE
                                    {
                                        COMM_TYPE = z.SLAB_NAME,
                                        OPERATOR_NAME = z.Operator_Name,
                                        OPERATOR_CODE = z.Operator_Code,
                                        OPERATOR_TYPE = z.Operator_Type,
                                        COMMISSION_TYPE = z.MER_COMMTYPE,
                                        MERCHANT_COM_PER = z.MER_COMMAMT
                                    }).ToList();

                return PartialView("IndexGrid", MerchantComm);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public FileResult ExportIndexGrid()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_COMM_SLAB_MOBILE_RECHARGE> grid = CreateExportableGridINExcel();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_COMM_SLAB_MOBILE_RECHARGE> gridRow in grid.Rows)
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
        private IGrid<TBL_COMM_SLAB_MOBILE_RECHARGE> CreateExportableGridINExcel()
        {
            var dbcontext = new DBContext();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
            var MerchantComm = (from CommMer in dbcontext.TBL_COMM_SLAB_MOBILE_RECHARGE
                                join WLComm in dbcontext.TBL_WHITE_LEVEL_COMMISSION_SLAB on CommMer.SLAB_ID equals WLComm.SLN
                                where CommMer.MEM_ID == MemberCurrentUser.MEM_ID
                                select new
                                {
                                    SLAB_NAME = WLComm.SLAB_NAME,
                                    Operator_Name = CommMer.OPERATOR_NAME,
                                    Operator_Code = CommMer.OPERATOR_CODE,
                                    Operator_Type = CommMer.OPERATOR_TYPE,
                                    MER_COMMTYPE = CommMer.COMMISSION_TYPE,
                                    MER_COMMAMT = CommMer.MERCHANT_COM_PER
                                }).AsEnumerable().Select(z => new TBL_COMM_SLAB_MOBILE_RECHARGE
                                {
                                    COMM_TYPE = z.SLAB_NAME,
                                    OPERATOR_NAME = z.Operator_Name,
                                    OPERATOR_CODE = z.Operator_Code,
                                    OPERATOR_TYPE = z.Operator_Type,
                                    COMMISSION_TYPE = z.MER_COMMTYPE,
                                    MERCHANT_COM_PER = z.MER_COMMAMT
                                }).ToList();
            IGrid<TBL_COMM_SLAB_MOBILE_RECHARGE> grid = new Grid<TBL_COMM_SLAB_MOBILE_RECHARGE>(MerchantComm);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.COMM_TYPE).Titled("Slab Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.OPERATOR_NAME).Titled("Operator Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.OPERATOR_CODE).Titled("Operator Code").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.OPERATOR_TYPE).Titled("Operator Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMMISSION_TYPE).Titled("Merchant Comm Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MERCHANT_COM_PER).Titled("Merchant Comm Amt").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
            //    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "DistributorRequisition", new { area = "Distributor", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a></div>");
            //grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
            //        .RenderedAs(model => "<div style='text-align:center'> <a href='javascript:void(0);' class='btn btn-primary btn-xs' data-toggle='modal' data-target='.transd' id='transactionvalueid' data-id=" + model.SLN + " onclick='getvalue(" + model.SLN + ");' title='Activate/Deactivate'>Action</a></div>");

            grid.Pager = new GridPager<TBL_COMM_SLAB_MOBILE_RECHARGE>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 1000000;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }

            public PartialViewResult DMTCommissionIndexGrid()
        {
            try
            {
                var db = new DBContext();
                var memberinfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                var MerDMRComm = (from commdmr in db.TBL_COMM_SLAB_DMR_PAYMENT
                                  join WLCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on commdmr.SLAB_ID equals WLCom.SLN
                                  where commdmr.MEM_ID == MemberCurrentUser.MEM_ID
                                  select new
                                  {
                                      SLAB_NAME = WLCom.SLAB_NAME,
                                      SLAB_FROM = commdmr.SLAB_FROM,
                                      SLAB_TO = commdmr.SLAB_TO,
                                      SLAB_TYPE = commdmr.COMM_TYPE,
                                      MER_COMMTYPE = commdmr.MERCHANT_COMM_TYPE,
                                      MER_COMMAMT = commdmr.MERCHANT_COM_PER
                                  }).AsEnumerable().Select(z => new TBL_COMM_SLAB_DMR_PAYMENT
                                  {
                                      DISTRIBUTOR_COMM_TYPE = z.SLAB_NAME,
                                      SLAB_FROM = z.SLAB_FROM,
                                      SLAB_TO = z.SLAB_TO,
                                      COMM_TYPE = z.SLAB_TYPE,
                                      MERCHANT_COMM_TYPE = z.MER_COMMTYPE,
                                      MERCHANT_COM_PER = z.MER_COMMAMT

                                  }).ToList();
                return PartialView("DMTCommissionIndexGrid", MerDMRComm);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        [HttpGet]
        public FileResult ExportDMTCommissionIndexGrid()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_COMM_SLAB_DMR_PAYMENT> grid = CreateExportDMTCommissionIndexGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_COMM_SLAB_DMR_PAYMENT> gridRow in grid.Rows)
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
        private IGrid<TBL_COMM_SLAB_DMR_PAYMENT> CreateExportDMTCommissionIndexGrid()
        {
            var db = new DBContext();
            var memberinfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
            var MerDMRComm = (from commdmr in db.TBL_COMM_SLAB_DMR_PAYMENT
                              join WLCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on commdmr.SLAB_ID equals WLCom.SLN
                              where commdmr.MEM_ID == MemberCurrentUser.MEM_ID
                              select new
                              {
                                  SLAB_NAME = WLCom.SLAB_NAME,
                                  SLAB_FROM = commdmr.SLAB_FROM,
                                  SLAB_TO = commdmr.SLAB_TO,
                                  SLAB_TYPE = commdmr.COMM_TYPE,
                                  MER_COMMTYPE = commdmr.MERCHANT_COMM_TYPE,
                                  MER_COMMAMT = commdmr.MERCHANT_COM_PER
                              }).AsEnumerable().Select(z => new TBL_COMM_SLAB_DMR_PAYMENT
                              {
                                  DISTRIBUTOR_COMM_TYPE = z.SLAB_NAME,
                                  SLAB_FROM = z.SLAB_FROM,
                                  SLAB_TO = z.SLAB_TO,
                                  COMM_TYPE = z.SLAB_TYPE,
                                  MERCHANT_COMM_TYPE = z.MER_COMMTYPE,
                                  MERCHANT_COM_PER = z.MER_COMMAMT
                              }).ToList();

            IGrid<TBL_COMM_SLAB_DMR_PAYMENT> grid = new Grid<TBL_COMM_SLAB_DMR_PAYMENT>(MerDMRComm);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.DISTRIBUTOR_COMM_TYPE).Titled("Slab Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SLAB_FROM).Titled("Slab From").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SLAB_TO).Titled("Slab to").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMM_TYPE).Titled("Comm Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MERCHANT_COMM_TYPE).Titled("Merchant Comm Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MERCHANT_COM_PER).Titled("Merchant Comm Amt").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
            //    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "DistributorRequisition", new { area = "Distributor", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a></div>");
            //grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
            //        .RenderedAs(model => "<div style='text-align:center'> <a href='javascript:void(0);' class='btn btn-primary btn-xs' data-toggle='modal' data-target='.transd' id='transactionvalueid' data-id=" + model.SLN + " onclick='getvalue(" + model.SLN + ");' title='Activate/Deactivate'>Action</a></div>");

            grid.Pager = new GridPager<TBL_COMM_SLAB_DMR_PAYMENT>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 1000000;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }
    }
}
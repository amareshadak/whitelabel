using log4net;
using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class WhiteLevelAdminController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Super Admin";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==1 );
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
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
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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

        // GET: Admin/WhiteLevelAdmin      
        

        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    string datevv = "2020-03-14";
                    DateTime dateval = Convert.ToDateTime(datevv);

                    //var myLINQ = from dt in db.TBL_ACCOUNTS.AsEnumerable()
                    //             where dt.TRANSACTION_DATE== dateval
                    //             group dt by dt.MEM_ID into grp
                    //             select new
                    //             {
                    //                 ID = grp.Max(T => T.ACC_NO),
                    //                 Status = grp.Key,
                    //                 Date = grp.Max(T => T.TRANSACTION_TIME)
                    //             };


                    //var lastItems = (from t in db.TBL_ACCOUNTS
                    //                 where t.TRANSACTION_DATE== dateval
                    //                 group t by t.MEM_ID into grp 
                    //                 select grp.OrderByDescending(u => u.TRANSACTION_TIME).ToList());


                    //var query = (from p in db.TBL_ACCOUNTS
                    //             group p by p.MEM_ID into g
                    //             let date = g.Max(d => d.TRANSACTION_TIME)
                    //             from pr in g
                    //             where pr.TRANSACTION_DATE == dateval
                    //             select pr).Distinct().ToList();

                    
                    //var dataget= db.Database.ExecuteSqlCommand("select t.ACC_NO,t.MEM_ID, t.TRANSACTION_TIME,t.OPENING ,t.AMOUNT, t.CLOSING from ACCOUNTS t inner join ( select MEM_ID, max(TRANSACTION_TIME) as MaxDate from ACCOUNTS  group by MEM_ID) tm on t.MEM_ID = tm.MEM_ID and t.TRANSACTION_TIME = tm.MaxDate where t.TRANSACTION_DATE = '2020-03-14'");

                    //var Memblistdb = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).Take(6).ToList().OrderByDescending(x => x.JOINING_DATE);
                    //ViewBag.whitelevel = Memblistdb;
                    //var availablebal = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                    //if (availablebal.BALANCE > 0)
                    //{
                    //    ViewBag.AvailableBalance = availablebal.BALANCE;

                    //}
                    //else
                    //{
                    //    ViewBag.AvailableBalance = 0;
                    //}
                    //var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    //if (walletamount != null)
                    //{
                    //    ViewBag.openingAmt = walletamount.OPENING;
                    //    ViewBag.closingAmt = walletamount.CLOSING;
                    //}
                    //else
                    //{
                    //    ViewBag.openingAmt = "0";
                    //    ViewBag.closingAmt = "0";
                    //}


                    //var requisitionlist = (from acnt in db.TBL_BALANCE_TRANSFER_LOGS
                    //                       join mem in db.TBL_MASTER_MEMBER on acnt.FROM_MEMBER equals mem.MEM_ID
                    //                       where acnt.TO_MEMBER == MemberCurrentUser.MEM_ID
                    //                       select new
                    //                       {
                    //                           tranid = acnt.SLN,
                    //                           transactionid = acnt.TransactionID,
                    //                           tranDate = acnt.REQUEST_DATE,
                    //                           userName = mem.UName
                    //                       }).AsEnumerable().Select(z => new GetRequisitiondetails()
                    //                       {
                    //                           TransId = z.tranid.ToString(),
                    //                           TransactionID = z.transactionid,
                    //                           TransDate = z.tranDate.ToString("yyyy-MM-dd"),
                    //                           TransUserName = z.userName
                    //                       }).ToList().Take(6).OrderByDescending(z => z.TransDate);
                    //ViewBag.RequisitionList = requisitionlist;
                    //var BankListdb = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).ToList();
                    //ViewBag.BankDetailsList = BankListdb;

                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  WhiteLevelAdmin(Admin), method:- Index (GET) Line No:- 94", ex);
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
                
        }
        public ActionResult AdminMemberInfo()
        {
            initpage();////
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  WhiteLevelAdmin(Admin), method:- AdminMemberInfo (GET) Line No:- 121", ex);
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult MemberGrid()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var dbcontext = new DBContext();
                    //var memberinfo = dbcontext.TBL_MASTER_MEMBER.ToList().OrderByDescending(x=>x.JOINING_DATE);
                    //// Only grid query values will be available here.
                    //return PartialView("IndexGrid", memberinfo);

                    return PartialView(ExporMembertableGrid());
                }
                catch (Exception ex)
                {
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
                RedirectToAction("Index", "Login", new { area = "" });
                return PartialView(ExporMembertableGrid());
            }

        }

        private IGrid<TBL_MASTER_MEMBER> ExporMembertableGrid()
        {
            var dbcontext = new DBContext();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x=>x.INTRODUCER==MemberCurrentUser.MEM_ID).ToList().OrderByDescending(x => x.JOINING_DATE);
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.UName).Titled("UserName").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            return grid;
        }

        public PartialViewResult RequisitionGrid()
        {

            try
            {
                var dbcontext = new DBContext();
                return PartialView(ExporRequisitionGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> ExporRequisitionGrid()
        {
            var db = new DBContext();
            var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID
                                   select new
                                   {
                                       Touser = "White Label",
                                       TransId = x.TransactionID,
                                       FromUser = y.UName,
                                       REQUEST_DATE = x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       TransactionID = z.TransId,
                                       FromUser = z.FromUser,
                                       AMOUNT = z.AMOUNT,
                                       REQUEST_DATE = z.REQUEST_DATE,
                                       BANK_ACCOUNT = z.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                       SLN = z.SLN
                                   }).ToList();

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("From Member");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            //grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
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

        public PartialViewResult BankDetailsGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                return PartialView(ExportBankDetailsGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_SETTINGS_BANK_DETAILS> ExportBankDetailsGrid()
        {
            var db = new DBContext();
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).ToList().OrderByDescending(x => x.CREATED_DATE);

            IGrid<TBL_SETTINGS_BANK_DETAILS> grid = new Grid<TBL_SETTINGS_BANK_DETAILS>(bankdetails);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.BANK).Titled("Bank").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ACCOUNT_NO).Titled("Account No").Filterable(true).Sortable(true);
            grid.Columns.Add(model => (model.ISDELETED == 0 ? "Active" : "Deactive")).Titled("Status").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_SETTINGS_BANK_DETAILS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            return grid;
        }
        [HttpPost]
        public JsonResult LoadAvailableBalance()
        {
            initpage();////
            try
            {
                var db = new DBContext();
                //var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                var walletamount = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                if (walletamount != null)
                {
                    //Session["openingAmt"] = walletamount.OPENING;
                    //Session["closingAmt"] = walletamount.CLOSING;
                    //ViewBag.openingAmt = walletamount.OPENING;
                    //ViewBag.closingAmt = walletamount.CLOSING;
                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Session["openingAmt"] = "0";
                    //Session["closingAmt"] = "0";
                    //ViewBag.openingAmt = "0";
                    //ViewBag.closingAmt = "0";
                    //walletamount.CLOSING = 0;
                    //return Json(walletamount, JsonRequestBehavior.AllowGet);
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }

        //public ActionResult TestURL()
        //{
        //    return View();
        //}

        [HttpGet]
        public JsonResult MultiLinkApiBalance()
        {
            var balance = MultiLinkAirAPI.GetMultilinkBalance();
            return Json(balance.ToString(), JsonRequestBehavior.AllowGet);
        }

        public static class ClientHelper
        {
            public static HttpClient GetClient()
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            }
            public static HttpClient GetDocClient()
            {
                HttpClient Docclient = new HttpClient();
                Docclient.DefaultRequestHeaders.Accept.Clear();
                Docclient.DefaultRequestHeaders.Add("Accept", "application/json");
                Docclient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                Docclient.DefaultRequestHeaders.Add("ContentType", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                return Docclient;
            }
        }
        private static dynamic GetResponse<T>(string url, string method, Dictionary<string, T> param)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = ClientHelper.GetClient())
                {
                    switch (method.ToUpper())
                    {
                        case "GET":
                            {
                                response = client.GetAsync(url).Result;
                                break;
                            }
                        case "POST":
                            {
                                response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")).Result;
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                    response.EnsureSuccessStatusCode();
                    // return string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result) ? response : JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    return response;
                }
            }
            catch (Exception e)
            {
                // Handle exception
                throw e;
            }
        }

    }
}
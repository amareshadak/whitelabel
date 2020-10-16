using log4net;
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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberFlightMarkupSettingController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
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
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Admin/MemberFlightMarkupSetting
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
               
                return View();
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
        public PartialViewResult IndexGrid(string SearchVal = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (SearchVal != "")
                {
                    var Member_Info = (from x in dbcontext.TBL_FLIGHT_MARKUP
                                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                       where y.MEMBER_NAME.StartsWith(SearchVal) || y.MEMBER_MOBILE.StartsWith(SearchVal) ||
                                       y.EMAIL_ID.StartsWith(SearchVal) || y.MEM_UNIQUE_ID.StartsWith(SearchVal) || y.COMPANY.StartsWith(SearchVal) || y.ADDRESS.StartsWith(SearchVal)
                                       select new
                                       {
                                           SLN = x.SLN,
                                           MEM_ID = x.MEM_ID,
                                           MEM_NAME = y.MEMBER_NAME,
                                           MEM_MOBILE = y.MEMBER_MOBILE,
                                           MEM_EMAIL = y.EMAIL_ID,
                                           MEM_COMPANY = y.COMPANY,
                                           MEM_ADD = y.ADDRESS + "-" + y.CITY,
                                           MEM_INTERNATIONAL_MARKUP = x.INTERNATIONAL_MARKUP,
                                           MEM_DOMESTIC_MARKUP = x.DOMESTIC_MARKUP,
                                           MEM_UNIQUE_ID = y.MEM_UNIQUE_ID,
                                           MEM_ASSiGN_DATE = x.ASSIGN_DATE,
                                           MEM_MODIFIED_DATE = x.MODIFIED_DATE,
                                           MEM_STATUS = x.STATUS,
                                           MEM_ASSIGN_STATUS = x.ASSIGN_TYPE,
                                           ASSIGN_BY = x.ASSIGN_BY,
                                           DIST_NAME=dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(s=>s.MEM_ID==x.DIST_ID).MEMBER_NAME,
                                           DIST_MEM_ID = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.DIST_ID).MEM_UNIQUE_ID,
                                       }).AsEnumerable().Select((z, index) => new TBL_FLIGHT_MARKUP
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           MEMBER_NAME = z.MEM_NAME,
                                           MEMBER_ADDRESS = z.MEM_ADD,
                                           MEMBER_MOBILE = z.MEM_MOBILE,
                                           MEMBER_EMAIL = z.MEM_EMAIL,
                                           MEMBER_COMPANY = z.MEM_COMPANY,
                                           MEMBER_UNIQUE_ID = z.MEM_UNIQUE_ID,
                                           INTERNATIONAL_MARKUP = z.MEM_INTERNATIONAL_MARKUP,
                                           DOMESTIC_MARKUP = z.MEM_DOMESTIC_MARKUP,
                                           STATUS = z.MEM_STATUS,
                                           ASSIGN_TYPE = z.MEM_ASSIGN_STATUS,
                                           ASSIGN_DATE = z.MEM_ASSiGN_DATE,
                                           MODIFIED_DATE = z.MEM_MODIFIED_DATE,
                                           ASSIGN_BY = z.ASSIGN_BY,
                                           DIST_NAME=z.DIST_NAME,
                                           DIST_MEM_ID=z.DIST_MEM_ID
                                       }).ToList();
                    return PartialView("IndexGrid", Member_Info);
                }
                else
                {
                    var Member_Info = (from x in dbcontext.TBL_FLIGHT_MARKUP
                                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                       select new
                                       {
                                           SLN = x.SLN,
                                           MEM_ID = x.MEM_ID,
                                           MEM_NAME = y.MEMBER_NAME,
                                           MEM_MOBILE = y.MEMBER_MOBILE,
                                           MEM_EMAIL = y.EMAIL_ID,
                                           MEM_COMPANY = y.COMPANY,
                                           MEM_ADD = y.ADDRESS + "-" + y.CITY,
                                           MEM_INTERNATIONAL_MARKUP = x.INTERNATIONAL_MARKUP,
                                           MEM_DOMESTIC_MARKUP = x.DOMESTIC_MARKUP,
                                           MEM_UNIQUE_ID = y.MEM_UNIQUE_ID,
                                           MEM_ASSiGN_DATE = x.ASSIGN_DATE,
                                           MEM_MODIFIED_DATE = x.MODIFIED_DATE,
                                           MEM_STATUS = x.STATUS,
                                           MEM_ASSIGN_STATUS = x.ASSIGN_TYPE,
                                           ASSIGN_BY = x.ASSIGN_BY,
                                           DIST_NAME = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.DIST_ID).MEMBER_NAME,
                                           DIST_MEM_ID = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.DIST_ID).MEM_UNIQUE_ID,
                                       }).AsEnumerable().Select((z, index) => new TBL_FLIGHT_MARKUP
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           MEMBER_NAME = z.MEM_NAME,
                                           MEMBER_ADDRESS = z.MEM_ADD,
                                           MEMBER_MOBILE = z.MEM_MOBILE,
                                           MEMBER_EMAIL = z.MEM_EMAIL,
                                           MEMBER_COMPANY = z.MEM_COMPANY,
                                           MEMBER_UNIQUE_ID = z.MEM_UNIQUE_ID,
                                           INTERNATIONAL_MARKUP = z.MEM_INTERNATIONAL_MARKUP,
                                           DOMESTIC_MARKUP = z.MEM_DOMESTIC_MARKUP,
                                           STATUS = z.MEM_STATUS,
                                           ASSIGN_TYPE = z.MEM_ASSIGN_STATUS,
                                           ASSIGN_DATE = z.MEM_ASSiGN_DATE,
                                           MODIFIED_DATE = z.MEM_MODIFIED_DATE,
                                           ASSIGN_BY = z.ASSIGN_BY,
                                           DIST_NAME = z.DIST_NAME,
                                           DIST_MEM_ID = z.DIST_MEM_ID
                                       }).ToList();
                    return PartialView("IndexGrid", Member_Info);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult FetchFlightMarkUpByMember(string SlnValue = "")
        {
            try
            {
                if (SlnValue != "")
                {
                    var db = new DBContext();
                    long Sln = 0;
                    long.TryParse(SlnValue, out Sln);
                    //var GettxnInfo = db.TBL_FLIGHT_MARKUP.FirstOrDefault(x => x.SLN == Sln);
                    var Member_Info = (from x in db.TBL_FLIGHT_MARKUP
                                       join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                       where x.SLN ==Sln
                                       select new
                                       {
                                           SLN = x.SLN,
                                           MEM_ID = x.MEM_ID,
                                           MEM_NAME = y.MEMBER_NAME,
                                           MEM_MOBILE = y.MEMBER_MOBILE,
                                           MEM_EMAIL = y.EMAIL_ID,
                                           MEM_COMPANY = y.COMPANY,
                                           MEM_ADD = y.ADDRESS + "-" + y.CITY,
                                           MEM_INTERNATIONAL_MARKUP = x.INTERNATIONAL_MARKUP,
                                           MEM_DOMESTIC_MARKUP = x.DOMESTIC_MARKUP,
                                           MEM_UNIQUE_ID = y.MEM_UNIQUE_ID,
                                           MEM_ASSiGN_DATE = x.ASSIGN_DATE,
                                           MEM_MODIFIED_DATE = x.MODIFIED_DATE,
                                           MEM_STATUS = x.STATUS,
                                           MEM_ASSIGN_STATUS = x.ASSIGN_TYPE,
                                           ASSIGN_BY = x.ASSIGN_BY,
                                           DIST_NAME=db.TBL_MASTER_MEMBER.FirstOrDefault(s=>s.MEM_ID==x.DIST_ID).MEMBER_NAME,
                                           DIST_MEM_ID= db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.DIST_ID).MEM_UNIQUE_ID
                                       }).AsEnumerable().Select((z, index) => new TBL_FLIGHT_MARKUP
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           MEMBER_NAME = z.MEM_NAME,
                                           MEMBER_ADDRESS = z.MEM_ADD,
                                           MEMBER_MOBILE = z.MEM_MOBILE,
                                           MEMBER_EMAIL = z.MEM_EMAIL,
                                           MEMBER_COMPANY = z.MEM_COMPANY,
                                           MEMBER_UNIQUE_ID = z.MEM_UNIQUE_ID,
                                           INTERNATIONAL_MARKUP = z.MEM_INTERNATIONAL_MARKUP,
                                           DOMESTIC_MARKUP = z.MEM_DOMESTIC_MARKUP,
                                           STATUS = z.MEM_STATUS,
                                           ASSIGN_TYPE = z.MEM_ASSIGN_STATUS,
                                           ASSIGN_DATE = z.MEM_ASSiGN_DATE,
                                           MODIFIED_DATE = z.MEM_MODIFIED_DATE,
                                           ASSIGN_BY = z.ASSIGN_BY,
                                           DIST_MEM_ID=z.DIST_MEM_ID,
                                           DIST_NAME=z.DIST_NAME
                                       }).FirstOrDefault();

                    return Json(new { Result = Member_Info, Status = "0" }, JsonRequestBehavior.AllowGet);
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
        public JsonResult POSTTAGFLIGHTMARKUP(TBL_FLIGHT_MARKUP objMarkup)
        {
            var db = new DBContext();            
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var getMArkup = db.TBL_FLIGHT_MARKUP.FirstOrDefault(x=>x.SLN== objMarkup.SLN);
                    if (getMArkup != null)
                    {
                        getMArkup.DOMESTIC_MARKUP = objMarkup.DOMESTIC_MARKUP;
                        getMArkup.INTERNATIONAL_MARKUP = objMarkup.INTERNATIONAL_MARKUP;
                        getMArkup.MODIFIED_DATE = DateTime.Now;
                        db.Entry(getMArkup).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Flight Markup is Set.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Flight Mark Up is Not Set Please contact to your Administrator.", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json("Please try again later", JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
        }

    }
}
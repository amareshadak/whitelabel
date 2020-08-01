using log4net;
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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorLoginDetailsController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Distributor/DistributorLoginDetails
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
                var LoginInfo = (from x in dbcontext.TBL_TRACE_MEMBER_LOGIN_DETAILS
                                 join y in dbcontext.TBL_MASTER_MEMBER
                                 on x.MEM_ID equals y.MEM_ID
                                 where y.MEMBER_ROLE == 5 && y.INTRODUCER == MemberCurrentUser.MEM_ID
                                 select new
                                 {
                                     SLN = x.ID,
                                     MemID = y.MEM_ID,
                                     MenberName = y.UName,
                                     Logintime = x.LOGINTIME,
                                     LogoutTime = x.LOGOUTTIME,
                                     IpAdress=x.IP_ADDRESS
                                 }).AsEnumerable().Select(z => new TBL_TRACE_MEMBER_LOGIN_DETAILS
                                 {
                                     ID = z.SLN,
                                     MEM_ID = z.MemID,
                                     From_Member_Name = z.MenberName,
                                     LOGINTIME = z.Logintime,
                                     LOGOUTTIME = z.Logintime,
                                     IP_ADDRESS=z.IpAdress
                                 }).ToList();


                return PartialView("IndexGrid", LoginInfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
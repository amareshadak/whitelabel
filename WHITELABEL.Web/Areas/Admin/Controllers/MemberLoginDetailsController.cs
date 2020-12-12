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
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberLoginDetailsController : AdminBaseController
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
        // GET: Admin/MemberLoginDetails
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
                
                return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                var LoginInfo = (from x in dbcontext.TBL_TRACE_MEMBER_LOGIN_DETAILS join y in dbcontext.TBL_MASTER_MEMBER
                                 on x.MEM_ID equals y.MEM_ID where y.MEMBER_ROLE == 4 && y.INTRODUCER == MemberCurrentUser.MEM_ID
                                 select new
                                 {
                                     SLN=x.ID,
                                     MemID = y.MEM_ID,
                                     MenberName = y.UName,
                                     Logintime = x.LOGINTIME,
                                     LogoutTime = x.LOGOUTTIME,
                                     IpAddress=x.IP_ADDRESS
                                 }).AsEnumerable().Select(z => new TBL_TRACE_MEMBER_LOGIN_DETAILS
                                 {
                                     ID=z.SLN,
                                     MEM_ID=z.MemID,
                                     From_Member_Name=z.MenberName,
                                     LOGINTIME=z.Logintime,
                                     LOGOUTTIME=z.Logintime,
                                     IP_ADDRESS=z.IpAddress
                                 }).ToList();

                
                return PartialView("IndexGrid", LoginInfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> EmulateDistributor(string memid = "")
        {
            var db = new DBContext();
            if (memid != "")
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["CreditLimitAmt"] = null;
                Session["ReservedCreditLimitAmt"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session["DistCompanyName"] = null;
               // Session.Clear();
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("CreditLimitAmt");
                Session.Remove("ReservedCreditLimitAmt");
                Session.Remove("UserType");
                Session.Remove("DistCompanyName");
                //Session.Clear();
                string decrptSlId = Decrypt.DecryptMe(memid);
                long Dist_ID = 0;
                long.TryParse(decrptSlId, out Dist_ID);
                var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_ID == Dist_ID);
                //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1 && x.MEM_UNIQUE_ID == "");
                //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
                if (GetMember != null)
                {
                    Session["DistributorUserId"] = GetMember.MEM_ID;
                    Session["DistributorUserName"] = GetMember.UName;
                    Session["UserType"] = "Distributor";
                    Session["CompanyName"] = GetMember.COMPANY;
                    HttpCookie AuthCookie;
                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                    Response.Cookies.Add(AuthCookie);
                    return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
                    //return View();
                }
                else
                { return RedirectToAction("Index", "MemberAPILabel", new { area = "Admin" }); }
            }
            else
            {
                return RedirectToAction("Index", "MemberAPILabel", new { area = "Admin" });
            }
        }

        public async Task<ActionResult> EmulateMerchant(string memid = "")
        {
            var db = new DBContext();
            if (memid != "")
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["CreditLimitAmt"] = null;
                Session["ReservedCreditLimitAmt"] = null;
                Session["UserType"] = null;
                Session["MERCHANTCompanyName"] = null;
                // Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("CreditLimitAmt");
                Session.Remove("ReservedCreditLimitAmt");
                Session.Remove("UserType");
                Session.Remove("MERCHANTCompanyName");
                //Session.Clear();
                string decrptSlId = Decrypt.DecryptMe(memid);
                long MEM_ID = 0;
                long.TryParse(decrptSlId, out MEM_ID);
                var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_ID == MEM_ID);
                //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1 && x.MEM_UNIQUE_ID == "");
                //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
                if (GetMember != null)
                {
                    Session["MerchantUserId"] = GetMember.MEM_ID;
                    Session["MerchantUserName"] = GetMember.UName;
                    Session["MERCHANTCompanyName"] = GetMember.COMPANY;
                    Session["UserType"] = "Merchant";

                    HttpCookie AuthCookie;
                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                    Response.Cookies.Add(AuthCookie);
                    return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                    //Session["DistributorUserId"] = GetMember.MEM_ID;
                    //Session["DistributorUserName"] = GetMember.UName;
                    //Session["UserType"] = "Distributor";
                    //Session["CompanyName"] = GetMember.COMPANY;
                    //HttpCookie AuthCookie;
                    //System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    //AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                    //AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                    //Response.Cookies.Add(AuthCookie);
                    //return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
                    //return View();
                }
                else
                { return RedirectToAction("Index", "MemberAPILabel", new { area = "Admin" }); }
            }
            else
            {
                return RedirectToAction("Index", "MemberAPILabel", new { area = "Admin" });
            }
        }
    }
}
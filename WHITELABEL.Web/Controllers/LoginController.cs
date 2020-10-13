using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
using WHITELABEL.Data.Models;
using log4net.Repository.Hierarchy;
using System.Security.Cryptography;
using System.Text;

namespace WHITELABEL.Web.Controllers
{
    public class LoginController : Controller
    {     
        public ActionResult Index(string returnUrl)
        {
            var db = new DBContext();
            //var GetUser = db.TBL_AUTH_ADMIN_USERS.FirstOrDefault(x => x.USER_EMAIL == "neeraj.g@traveliq.in");


            string idval = TimeToHexString();
            string valueid = UniqueID;
            string iidd = TicksToString();
            var random = new Random(System.DateTime.Now.Millisecond);
            int randomNumber = random.Next(1, 50000);
            string tetrid = ToHex(6);
            string randomnumber = GetRandomKey(5);

            //var db = new DBContext();
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["UserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["UserId"].ToString();
                
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();
                
                if (userinfo.MEMBER_ROLE == 1)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.MEMBER_ROLE == null)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.UNDER_WHITE_LEVEL == null)
                {
                    Response.RedirectToRoute("Dashboard", "Index");
                }
            }
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }


            string host = Request.Url.Host;
            //string host = "www.ramkrushnaharitravels.co.in";
            var logochecking = (from x in db.TBL_MASTER_MEMBER
                                join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                 on x.MEM_ID equals y.MEM_ID
                                //where y.DOMAIN == DomaineName && y.STATUS == 1
                                where y.DOMAIN.Contains(host) && y.STATUS == 1
                                select new
                                {
                                    logoPath = x.LOGO,
                                    LogoStyle = x.LOGO_STYLE,
                                    CompanyName = x.COMPANY
                                }).FirstOrDefault();
            if (logochecking != null)
            {
                if (logochecking.logoPath != null)
                {
                    if (logochecking.logoPath != "")
                    {
                        ViewBag.Logopath = Url.Content(logochecking.logoPath);
                        ViewBag.LogoStyle = logochecking.LogoStyle;
                        ViewBag.CompanyName = logochecking.CompanyName;
                    }
                    else
                    {
                        ViewBag.Logopath = "";
                        ViewBag.LogoStyle = "";
                        ViewBag.CompanyName = "";
                    }
                   
                }
                else
                {
                    ViewBag.Logopath = "";
                    ViewBag.LogoStyle = "";
                    ViewBag.CompanyName = "";
                }
            }
            else
            {
                return RedirectToAction("DomainError", "Login");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            //if (Session["PowerAdminUserId"] ==null)
            //{
            using (var db = new DBContext())
            {
                string DomaineName = Request.Url.Host;
                var logochecking = (from x in db.TBL_MASTER_MEMBER
                                    join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     on x.MEM_ID equals y.MEM_ID
                                    //where y.DOMAIN == DomaineName && y.STATUS == 1
                                    where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                    select new
                                    {
                                        logoPath = x.LOGO,
                                        LogoStyle = x.LOGO_STYLE,
                                        CompanyName = x.COMPANY
                                    }).FirstOrDefault();
                if (logochecking != null)
                {
                    if (logochecking.logoPath != null)
                    {
                        if (logochecking.logoPath != "")
                        {
                            Session["LogoPath"] = Url.Content(logochecking.logoPath);
                            Session["LogoStyle"] = logochecking.LogoStyle;
                            Session["CompanyName"] = logochecking.CompanyName;
                        }
                        else {
                            Session["LogoPath"] = "";
                            Session["LogoStyle"] = "";
                            Session["CompanyName"] = "";
                        }

                    }
                    else
                    {
                        Session["LogoPath"] = "";
                        Session["LogoStyle"] = "";
                        Session["CompanyName"] = "";
                    }
                }
                else
                {
                    return RedirectToAction("DomainError", "Login");
                }
                var DistGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
                if (DistGetMember != null)
                {
                    if (DistGetMember.MEMBER_ROLE == 4)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                string IpAddress = string.Empty;
                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                {
                                    IpAddress = User.GetIPAddress.Replace("\"", "");
                                }
                                else
                                {
                                    IpAddress = "";
                                }
                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                {
                                    MEM_ID = DistGetMember.MEM_ID,
                                    LOGINTIME = DateTime.Now,
                                    STATUS = 1,
                                    IP_ADDRESS = IpAddress
                                };
                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                db.SaveChanges();
                                Session["DistributorUserId"] = DistGetMember.MEM_ID;
                                Session["DistributorUserName"] = DistGetMember.UName;
                                Session["UserType"] = "Distributor";
                                Session["DistCompanyName"] = DistGetMember.COMPANY;
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();

                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }
                        }
                        else
                        {
                            return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else if (DistGetMember.MEMBER_ROLE == 5)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                string IpAddress = string.Empty;
                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                {
                                    IpAddress = User.GetIPAddress.Replace("\"", "");
                                }
                                else
                                {
                                    IpAddress = "";
                                }
                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                {
                                    MEM_ID = DistGetMember.MEM_ID,
                                    LOGINTIME = DateTime.Now,
                                    STATUS = 1,
                                    IP_ADDRESS = IpAddress
                                };
                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                db.SaveChanges();
                                Session["MerchantUserId"] = DistGetMember.MEM_ID;
                                Session["MerchantUserName"] = DistGetMember.UName;
                                Session["MerchantRailID"] = DistGetMember.RAIL_ID_QUANTITY;
                                Session["MERCHANTCompanyName"] = DistGetMember.COMPANY;
                                Session["UserType"] = "Merchant";
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }
                        }
                        else
                        {
                            return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Credential or Access Denied";
                    return View();
                }
            }
            //SystemClass sclass = new SystemClass();
            //string userID = sclass.GetLoggedUser();
            ////var userpass = "india123";
            ////userpass = userpass.GetPasswordHash();
            ////if (Session["PowerAdminUserId"] ==null)
            ////{
            //using (var db = new DBContext())
            //{

            //    string DomaineName = Request.Url.Host;
            //    var logochecking = (from x in db.TBL_MASTER_MEMBER
            //                        join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
            //                         on x.MEM_ID equals y.MEM_ID
            //                        //where y.DOMAIN == DomaineName && y.STATUS == 1
            //                        where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
            //                        select new
            //                        {
            //                            logoPath = x.LOGO,
            //                            LogoStyle = x.LOGO_STYLE,
            //                            CompanyName = x.COMPANY
            //                        }).FirstOrDefault();
            //    if (logochecking != null)
            //    {
            //        if (logochecking.logoPath != null)
            //        {
            //            if (logochecking.logoPath != "")
            //            {
            //                Session["LogoPath"] = Url.Content(logochecking.logoPath);
            //                Session["LogoStyle"] = logochecking.LogoStyle;
            //                Session["CompanyName"] = logochecking.CompanyName;
            //            }
            //            else {
            //                Session["LogoPath"] = "";
            //                Session["LogoStyle"] = "";
            //                Session["CompanyName"] = "";
            //            }
            //        }
            //        else
            //        {
            //            Session["LogoPath"] = "";
            //            Session["LogoStyle"] = "";
            //            Session["CompanyName"] = "";
            //        }
            //    }
            //    else
            //    {
            //        return RedirectToAction("DomainError", "Login");
            //    }
            //    var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1);
            //    if (GetMember != null)
            //    {
            //        if (GetMember.MEMBER_ROLE == 1)
            //        {
            //            var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
            //                                       join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
            //                                        on x.MEM_ID equals y.MEM_ID
            //                                       //where y.DOMAIN == DomaineName && y.STATUS == 1
            //                                       //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
            //                                       where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && y.MEM_ID == GetMember.MEM_ID
            //                                       select new
            //                                       {
            //                                           MEM_ID = x.MEM_ID,
            //                                           MEMBER_ROLE = x.MEMBER_ROLE,
            //                                           ACTIVE_MEMBER = x.ACTIVE_MEMBER,
            //                                           User_pwd = x.User_pwd,
            //                                           UName = x.UName,
            //                                           DOMAIN = y.DOMAIN
            //                                       }).FirstOrDefault();
            //            if (GETWHITELevelDOMAIn != null)
            //            {
            //                Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

            //                if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
            //                {
            //                    ViewBag.Message = "Invalid Credential or Access Denied";
            //                    FormsAuthentication.SignOut();
            //                    return View();
            //                }
            //                else
            //                {
            //                    string IpAddress = string.Empty;
            //                    if (User.GetIPAddress != "" && User.GetIPAddress != null)
            //                    {
            //                        IpAddress = User.GetIPAddress.Replace("\"", "");
            //                    }
            //                    else
            //                    {
            //                        IpAddress = "";
            //                    }
            //                    TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
            //                    {
            //                        MEM_ID = GetMember.MEM_ID,
            //                        LOGINTIME = DateTime.Now,
            //                        STATUS = 1,
            //                        IP_ADDRESS = IpAddress
            //                    };
            //                    db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
            //                    db.SaveChanges();
            //                    Session["WhiteLevelUserId"] = GetMember.MEM_ID;
            //                    Session["WhiteLevelUserName"] = GetMember.UName;
            //                    Session["UserType"] = "White Level";
            //                    HttpCookie AuthCookie;
            //                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
            //                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
            //                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
            //                    Response.Cookies.Add(AuthCookie);
            //                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
            //                    return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
            //                }
            //            }
            //            else
            //            {
            //                return RedirectToAction("DomainError", "Login");
            //            }
            //        }
            //        else
            //        {
            //            ViewBag.Message = "Invalid Credential or Access Denied";
            //            return View();
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.Message = "Invalid Credential or Access Denied";
            //        return View();
            //    }
            //}
        }
        //public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        //{
        //    SystemClass sclass = new SystemClass();
        //    string userID = sclass.GetLoggedUser();
        //    //var userpass = "india123";
        //    //userpass = userpass.GetPasswordHash();
        //    //if (Session["PowerAdminUserId"] ==null)
        //    //{
        //    using (var db = new DBContext())
        //    {
        //        //var GetUser = await db.TBL_AUTH_ADMIN_USERS.FirstOrDefaultAsync(x => x.USER_EMAIL == User.Email && x.USER_PASSWORD_MD5==User.Password);
        //        var GetUser = await db.TBL_AUTH_ADMIN_USERS.FirstOrDefaultAsync(x => x.USER_EMAIL == User.Email && User.Password=="Indiawestbengal123456789");
        //        if (GetUser != null)
        //        {
        //            if (!GetUser.ACTIVE_USER || !GetUser.USER_PASSWORD_MD5.VerifyHashedPassword(User.Password))
        //            {
        //                ViewBag.Message = "Invalid Credential or Access Denied";
        //                FormsAuthentication.SignOut();
        //                return View();
        //            }
        //            else
        //            {
        //                Session["PowerAdminUserId"] = GetUser.USER_ID;
        //                Session["PowerAdminUserName"] = GetUser.USER_NAME;
        //                Session["UserType"] = "Power Admin";
        //                HttpCookie AuthCookie;
        //                System.Web.Security.FormsAuthentication.SetAuthCookie(GetUser.USER_NAME + "||" + Encrypt.EncryptMe(GetUser.USER_ID.ToString()), true);
        //                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetUser.USER_NAME + "||" + Encrypt.EncryptMe(GetUser.USER_ID.ToString()), true);
        //                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                Response.Cookies.Add(AuthCookie);
        //                //return RedirectToAction("Index", "login", new { area = "" });
        //                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                return RedirectToAction("Index", "PowerAdminHome", new { area = "PowerAdmin" });
        //            }
        //        }
        //        else
        //        {                    
        //            string DomaineName = Request.Url.Host;
        //            var logochecking = (from x in db.TBL_MASTER_MEMBER
        //                                join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                 on x.MEM_ID equals y.MEM_ID
        //                                //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
        //                                select new
        //                                {
        //                                    logoPath = x.LOGO,
        //                                    LogoStyle = x.LOGO_STYLE,
        //                                    CompanyName = x.COMPANY
        //                                }).FirstOrDefault();
        //            if (logochecking != null)
        //            {
        //                if (logochecking.logoPath != null)
        //                {
        //                    if (logochecking.logoPath != "")
        //                    {
        //                        Session["LogoPath"] = Url.Content(logochecking.logoPath);
        //                        Session["LogoStyle"] = logochecking.LogoStyle;
        //                        Session["CompanyName"] = logochecking.CompanyName;
        //                    }
        //                    else {
        //                        Session["LogoPath"] = "";
        //                        Session["LogoStyle"] = "";
        //                        Session["CompanyName"] = "";
        //                    }

        //                }
        //                else
        //                {
        //                    Session["LogoPath"] = "";
        //                    Session["LogoStyle"] = "";
        //                    Session["CompanyName"] = "";
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("DomainError", "Login");
        //            }
        //            var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1);
        //            //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1 && x.MEM_UNIQUE_ID == "");
        //            //var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
        //            if (GetMember != null)
        //            {
        //                if (GetMember.MEMBER_ROLE == 1)
        //                {
        //                    var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                               join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                on x.MEM_ID equals y.MEM_ID
        //                                               //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                               //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                               where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && y.MEM_ID == GetMember.MEM_ID
        //                                               select new
        //                                               {
        //                                                   MEM_ID = x.MEM_ID,
        //                                                   MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                   ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                   User_pwd = x.User_pwd,
        //                                                   UName = x.UName,
        //                                                   DOMAIN = y.DOMAIN
        //                                               }).FirstOrDefault();
        //                    if (GETWHITELevelDOMAIn != null)
        //                    {
        //                        Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

        //                        if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
        //                        {
        //                            ViewBag.Message = "Invalid Credential or Access Denied";
        //                            FormsAuthentication.SignOut();
        //                            return View();
        //                        }
        //                        else
        //                        {
        //                            string IpAddress = string.Empty;
        //                            if (User.GetIPAddress != "" && User.GetIPAddress != null)
        //                            {
        //                                IpAddress = User.GetIPAddress.Replace("\"", "");
        //                            }
        //                            else
        //                            {
        //                                IpAddress = "";
        //                            }
        //                            TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
        //                            {
        //                                MEM_ID = GetMember.MEM_ID,
        //                                LOGINTIME=DateTime.Now,
        //                                STATUS=1,
        //                                IP_ADDRESS= IpAddress
        //                            };
        //                            db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
        //                            db.SaveChanges();
        //                            Session["WhiteLevelUserId"] = GetMember.MEM_ID;
        //                            Session["WhiteLevelUserName"] = GetMember.UName;
        //                            Session["UserType"] = "White Level";

        //                            HttpCookie AuthCookie;
        //                            System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                            AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                            AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                            Response.Cookies.Add(AuthCookie);
        //                            //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                            return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("DomainError", "Login");
        //                    }
        //                }
        //                else if (GetMember.MEMBER_ROLE == 2)
        //                {


        //                    if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
        //                    {
        //                        ViewBag.Message = "Invalid Credential or Access Denied";
        //                        FormsAuthentication.SignOut();
        //                        return View();
        //                    }
        //                    else
        //                    {
        //                        Session["UserId"] = GetMember.MEM_ID;
        //                        Session["UserName"] = GetMember.UName;

        //                        HttpCookie AuthCookie;
        //                        System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                        AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                        AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                        Response.Cookies.Add(AuthCookie);
        //                        return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
        //                        //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                    }


        //                }
        //                else if (GetMember.MEMBER_ROLE == 3)
        //                {
        //                    var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                               join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                on x.UNDER_WHITE_LEVEL equals y.MEM_ID
        //                                               //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                               //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                               where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == GetMember.MEM_ID
        //                                               select new
        //                                               {
        //                                                   MEM_ID = x.MEM_ID,
        //                                                   MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                   ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                   User_pwd = x.User_pwd,
        //                                                   UName = x.UName,
        //                                                   DOMAIN = y.DOMAIN
        //                                               }).FirstOrDefault();
        //                    if (GETWHITELevelDOMAIn != null)
        //                    {
        //                        Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

        //                        if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
        //                        {
        //                            ViewBag.Message = "Invalid Credential or Access Denied";
        //                            FormsAuthentication.SignOut();
        //                            return View();
        //                        }
        //                        else
        //                        {
        //                            Session["SuperDistributorId"] = GetMember.MEM_ID;
        //                            Session["SuperDistributorUserName"] = GetMember.UName;
        //                            Session["UserType"] = "Super Distributor";

        //                            HttpCookie AuthCookie;

        //                            System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                            AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                            //System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.EMAIL_ID +"||"+GetMember.User_pwd +"||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
        //                            //AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + GetMember.User_pwd + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);

        //                            AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                            Response.Cookies.Add(AuthCookie);
        //                            return RedirectToAction("Index", "SuperDashboard", new { area = "Super" });
        //                            //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("DomainError", "Login");
        //                    }
        //                }
        //                else if (GetMember.MEMBER_ROLE == 4)
        //                {
        //                    var DistGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
        //                    if (DistGetMember != null)
        //                    {
        //                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
        //                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
        //                                                   select new
        //                                                   {
        //                                                       MEM_ID = x.MEM_ID,
        //                                                       MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                       User_pwd = x.User_pwd,
        //                                                       UName = x.UName,
        //                                                       DOMAIN = y.DOMAIN
        //                                                   }).FirstOrDefault();
        //                        if (GETWHITELevelDOMAIn != null)
        //                        {
        //                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
        //                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
        //                            {
        //                                ViewBag.Message = "Invalid Credential or Access Denied";
        //                                FormsAuthentication.SignOut();
        //                                return View();
        //                            }
        //                            else
        //                            {
        //                                string IpAddress = string.Empty;
        //                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
        //                                {
        //                                    IpAddress = User.GetIPAddress.Replace("\"", "");
        //                                }
        //                                else
        //                                {
        //                                    IpAddress = "";
        //                                }
        //                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
        //                                {
        //                                    MEM_ID = DistGetMember.MEM_ID,
        //                                    LOGINTIME = DateTime.Now,
        //                                    STATUS = 1,
        //                                    IP_ADDRESS = IpAddress
        //                                };
        //                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
        //                                db.SaveChanges();
        //                                Session["DistributorUserId"] = DistGetMember.MEM_ID;
        //                                Session["DistributorUserName"] = DistGetMember.UName;
        //                                Session["UserType"] = "Distributor";
        //                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
        //                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();

        //                                HttpCookie AuthCookie;
        //                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                                Response.Cookies.Add(AuthCookie);
        //                                return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
        //                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("DomainError", "Login");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ViewBag.Message = "Invalid Credential or Access Denied";
        //                        return View();
        //                    }
        //                }
        //                else if (GetMember.MEMBER_ROLE == 5)
        //                {
        //                    var MerchantGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
        //                    if (MerchantGetMember != null)
        //                    {
        //                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
        //                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == MerchantGetMember.MEM_ID
        //                                                   select new
        //                                                   {
        //                                                       MEM_ID = x.MEM_ID,
        //                                                       MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                       User_pwd = x.User_pwd,
        //                                                       UName = x.UName,
        //                                                       DOMAIN = y.DOMAIN
        //                                                   }).FirstOrDefault();
        //                        if (GETWHITELevelDOMAIn != null)
        //                        {
        //                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
        //                            if (MerchantGetMember.ACTIVE_MEMBER == false || MerchantGetMember.User_pwd != User.Password)
        //                            {
        //                                ViewBag.Message = "Invalid Credential or Access Denied";
        //                                FormsAuthentication.SignOut();
        //                                return View();
        //                            }
        //                            else
        //                            {
        //                                string IpAddress = string.Empty;
        //                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
        //                                {
        //                                    IpAddress = User.GetIPAddress.Replace("\"", "");
        //                                }
        //                                else
        //                                {
        //                                    IpAddress = "";
        //                                }
        //                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
        //                                {
        //                                    MEM_ID = MerchantGetMember.MEM_ID,
        //                                    LOGINTIME = DateTime.Now,
        //                                    STATUS = 1,
        //                                    IP_ADDRESS = IpAddress
        //                                };
        //                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
        //                                db.SaveChanges();
        //                                Session["MerchantUserId"] = MerchantGetMember.MEM_ID;

        //                                Session["MerchantUserName"] = MerchantGetMember.UName;
        //                                Session["MerchantCompanyName"] = MerchantGetMember.COMPANY;
        //                                Session["UserType"] = "Merchant";
        //                                Session["CreditLimitAmt"] = MerchantGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
        //                                Session["ReservedCreditLimitAmt"] = MerchantGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
        //                                HttpCookie AuthCookie;
        //                                System.Web.Security.FormsAuthentication.SetAuthCookie(MerchantGetMember.UName + "||" + Encrypt.EncryptMe(MerchantGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(MerchantGetMember.UName + "||" + Encrypt.EncryptMe(MerchantGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                                Response.Cookies.Add(AuthCookie);
        //                                return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
        //                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("DomainError", "Login");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ViewBag.Message = "Invalid Credential or Access Denied";
        //                        return View();
        //                    }

        //                }
        //                else
        //                {
        //                    ViewBag.Message = "Invalid Credential or Access Denied";
        //                    return View();
        //                }
        //            }
        //            else
        //            {
        //                var DistGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
        //                if (DistGetMember != null)
        //                {
        //                    if (DistGetMember.MEMBER_ROLE == 4)
        //                    {
        //                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
        //                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
        //                                                   select new
        //                                                   {
        //                                                       MEM_ID = x.MEM_ID,
        //                                                       MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                       User_pwd = x.User_pwd,
        //                                                       UName = x.UName,
        //                                                       DOMAIN = y.DOMAIN
        //                                                   }).FirstOrDefault();
        //                        if (GETWHITELevelDOMAIn != null)
        //                        {
        //                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
        //                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
        //                            {
        //                                ViewBag.Message = "Invalid Credential or Access Denied";
        //                                FormsAuthentication.SignOut();
        //                                return View();
        //                            }
        //                            else
        //                            {
        //                                string IpAddress = string.Empty;
        //                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
        //                                {
        //                                    IpAddress = User.GetIPAddress.Replace("\"", "");
        //                                }
        //                                else
        //                                {
        //                                    IpAddress = "";
        //                                }
        //                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
        //                                {
        //                                    MEM_ID = DistGetMember.MEM_ID,
        //                                    LOGINTIME = DateTime.Now,
        //                                    STATUS = 1,
        //                                    IP_ADDRESS = IpAddress
        //                                };
        //                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
        //                                db.SaveChanges();
        //                                Session["DistributorUserId"] = DistGetMember.MEM_ID;
        //                                Session["DistributorUserName"] = DistGetMember.UName;
        //                                Session["UserType"] = "Distributor";
        //                                Session["DistCompanyName"] = DistGetMember.COMPANY;
        //                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
        //                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();

        //                                HttpCookie AuthCookie;
        //                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                                Response.Cookies.Add(AuthCookie);
        //                                return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
        //                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("DomainError", "Login");
        //                        }
        //                    }
        //                    else if (DistGetMember.MEMBER_ROLE == 5)
        //                    {
        //                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
        //                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
        //                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
        //                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
        //                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
        //                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
        //                                                   select new
        //                                                   {
        //                                                       MEM_ID = x.MEM_ID,
        //                                                       MEMBER_ROLE = x.MEMBER_ROLE,
        //                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
        //                                                       User_pwd = x.User_pwd,
        //                                                       UName = x.UName,
        //                                                       DOMAIN = y.DOMAIN
        //                                                   }).FirstOrDefault();
        //                        if (GETWHITELevelDOMAIn != null)
        //                        {
        //                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
        //                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
        //                            {
        //                                ViewBag.Message = "Invalid Credential or Access Denied";
        //                                FormsAuthentication.SignOut();
        //                                return View();
        //                            }
        //                            else
        //                            {
        //                                string IpAddress = string.Empty;
        //                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
        //                                {
        //                                    IpAddress = User.GetIPAddress.Replace("\"", "");
        //                                }
        //                                else
        //                                {
        //                                    IpAddress = "";
        //                                }
        //                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
        //                                {
        //                                    MEM_ID = DistGetMember.MEM_ID,
        //                                    LOGINTIME = DateTime.Now,
        //                                    STATUS = 1,
        //                                    IP_ADDRESS = IpAddress
        //                                };
        //                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
        //                                db.SaveChanges();
        //                                Session["MerchantUserId"] = DistGetMember.MEM_ID;
        //                                Session["MerchantUserName"] = DistGetMember.UName;
        //                                Session["MerchantRailID"] = DistGetMember.RAIL_ID_QUANTITY;
        //                                Session["MERCHANTCompanyName"] = DistGetMember.COMPANY;
        //                                Session["UserType"] = "Merchant";
        //                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
        //                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
        //                                HttpCookie AuthCookie;
        //                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
        //                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
        //                                Response.Cookies.Add(AuthCookie);
        //                                return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
        //                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("DomainError", "Login");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ViewBag.Message = "Invalid Credential or Access Denied";
        //                        return View();
        //                    }
        //                }
        //                else
        //                {
        //                    ViewBag.Message = "Invalid Credential or Access Denied";
        //                    return View();
        //                }
        //                //ViewBag.Message = "Invalid Credential or Access Denied";
        //                //return View();
        //            }
        //            //}
        //            //else
        //            //{
        //            //    return RedirectToAction("DomainError", "Login");
        //            //}
        //            //ViewBag.Message = "Invalid Credential or Access Denied";
        //            //return View();
        //        }
        //    }
        //    //}
        //    //else
        //    //{
        //    //    Response.RedirectToRoute("Home", "Index");
        //    //}
        //    //return View();
        //}
        public ActionResult DomainError()
        {
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            return View();
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnURL=" + Url.Action("Index", "Login"));
            return RedirectToAction("AdminLogin", "Login");
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgettenPassword user)
        {
            if (ModelState.IsValid)
            {
                var db = new DBContext();
                var UserProfile = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID == user.Email && x.ACTIVE_MEMBER == true).FirstOrDefault();
                if (UserProfile != null)
                {
                    var token = TokenGenerator.GenerateToken();
                    var PasswordResetObj = new TBL_PASSWORD_RESET
                    {
                        ID = token,
                        EmailID = user.Email,
                        Time = DateTime.Now
                    };
                    db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                    db.SaveChanges();
                    string resetLink = "<a href='"
                           + Url.Action("ResetPassword", "Login", new { rt = token }, "http")
                           + "'>Password Reset</a>";
                    string nameurl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    string firstname = UserProfile.MEMBER_NAME;

                    //string fullName = firstname + " " + lastname;
                    string fullName = UserProfile.UName;
                    string Email = user.Email;
                    string name = fullName;

                    EmailHelper emailHelper = new EmailHelper();
                    string Subject = "Password Reset";
                    string mailBody = emailHelper.GetEmailTemplate(fullName, "Someone has asked to reset the password on your Urwex account. <br /><br />If you didn’t request a password reset, you can disregard this email. <strong>No changes have been made to your account</strong>.<br /> <br />To reset your password, click this link: " + resetLink + " or copy and paste it into your browser.The password reset link is only valid for the next 90 minutes.", "WelcomeTemplate.html");
                    emailHelper.SendUserEmail(Email, Subject, mailBody);
                    ViewBag.Message = "Please check your email for further instructions";
                    return View(user);
                }
                else
                {
                    var UserProfilevale= db.TBL_AUTH_ADMIN_USERS.FirstOrDefault(x => x.USER_EMAIL == user.Email);
                    if (UserProfilevale != null)
                    {
                        var token = TokenGenerator.GenerateToken();
                        var PasswordResetObj = new TBL_PASSWORD_RESET
                        {
                            ID = token,
                            EmailID = user.Email,
                            Time = DateTime.Now
                        };
                        db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                        db.SaveChanges();
                        string resetLink = "<a href='"
                               + Url.Action("ResetPassword", "Login", new { rt = token }, "http")
                               + "'>Password Reset</a>";
                        string nameurl = Request.Url.GetLeftPart(UriPartial.Authority);
                        string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                        string firstname = UserProfile.MEMBER_NAME;

                        //string fullName = firstname + " " + lastname;
                        string fullName = UserProfile.UName;
                        string Email = user.Email;
                        string name = fullName;

                        EmailHelper emailHelper = new EmailHelper();
                        string Subject = "Password Reset";
                        string mailBody = emailHelper.GetEmailTemplate(fullName, "Someone has asked to reset the password on your Urwex account. <br /><br />If you didn’t request a password reset, you can disregard this email. <strong>No changes have been made to your account</strong>.<br /> <br />To reset your password, click this link: " + resetLink + " or copy and paste it into your browser.The password reset link is only valid for the next 90 minutes.", "WelcomeTemplate.html");
                        emailHelper.SendUserEmail(Email, Subject, mailBody);
                        ViewBag.Message = "Please check your email for further instructions";
                        return View(user);
                    }
                    else {
                        ViewBag.Message = "Email is not available";
                    }
                                        
                }
            }
            //ViewBag.Message = "Please check your email for further instructions";
            return View(user);

        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DBContext();
                var passwordset = db.TBL_PASSWORD_RESET.Where(x => x.ID == model.ReturnToken).FirstOrDefault();
                if (passwordset != null)
                {
                    var UserInfo = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID == passwordset.EmailID && x.ACTIVE_MEMBER == true).FirstOrDefault();
                    if (UserInfo != null)
                    {
                        UserInfo.User_pwd = model.Password;
                        db.Entry(UserInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // SEND EMAIL AFTER RESET
                        string Email = passwordset.EmailID;
                        string name = UserInfo.MEMBER_NAME;

                        EmailHelper emailHelper = new EmailHelper();
                        string Subject = "Your Password Has Been Successfully Reset";
                        string mailBody = emailHelper.GetEmailTemplate(UserInfo.MEMBER_NAME, "<p>Your password reset has been reset successfully.</p> <br />  If you weren’t the one who reset your password, please contact our support team at <a href='mailto: support@urwex.com ? subject = Support % 20Query' style='font - size: 17px; background - color: #ffffff;'>support@urwex.com immediately</a></p>", "WelcomeTemplate.html");
                        emailHelper.SendUserEmail(Email, Subject, mailBody);

                        // SEND UI MESSAGE
                        ViewBag.Message = "Password Reset successful. Please login";
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Access denied!";
                    }
                }
                else
                {
                    ViewBag.Message = "Access denied!";
                }
            }
            return View(model);
        }
        public ActionResult DistributorSignUp()
        {
            try
            {
                var db = new DBContext();
                var model = new TBL_MASTER_MEMBER();
                string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                string UniqId = "TIQ" + GetUniqueNo;

                var StateName = db.TBL_STATES.ToList();
                ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");                
                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "DISTRIBUTOR").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                model.UName = UniqId;
                return View(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<JsonResult> POSTADDDistributor(TBL_MASTER_MEMBER objsupermem)
        {            
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkEmail = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.EMAIL_ID == objsupermem.EMAIL_ID);
                    if (checkEmail == null)
                    {
                        ////string GetUniqueNo = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 10000);
                        //string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                        //string UniqId = "TIQ" + GetUniqueNo;
                        string DomaineName = Request.Url.Host;
                        var logochecking = (from x in db.TBL_MASTER_MEMBER
                                            join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                             on x.MEM_ID equals y.MEM_ID
                                            //where y.DOMAIN == DomaineName && y.STATUS == 1
                                            where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                            select new
                                            {
                                                mem_id = x.MEM_ID,
                                                logoPath = x.LOGO,
                                                LogoStyle = x.LOGO_STYLE,
                                                CompanyName = x.COMPANY
                                            }).FirstOrDefault();

                        objsupermem.BALANCE = 0;
                        if (objsupermem.BLOCKED_BALANCE == null)
                        {
                            objsupermem.BLOCKED_BALANCE = 0;
                            objsupermem.BALANCE = 0;
                        }
                        else
                        {
                            objsupermem.BLOCKED_BALANCE = objsupermem.BLOCKED_BALANCE;
                            objsupermem.BALANCE = objsupermem.BLOCKED_BALANCE;
                        }
                        objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                        objsupermem.UNDER_WHITE_LEVEL = logochecking.mem_id;
                        objsupermem.INTRODUCER = logochecking.mem_id;
                        objsupermem.ACTIVE_MEMBER = true;
                        objsupermem.IS_DELETED = false;
                        objsupermem.JOINING_DATE = System.DateTime.Now;
                        objsupermem.CREATED_BY = logochecking.mem_id;

                        objsupermem.LAST_MODIFIED_DATE = System.DateTime.Now;
                        objsupermem.GST_MODE = 1;
                        objsupermem.TDS_MODE = 1;
                        objsupermem.DUE_CREDIT_BALANCE = 0;
                        objsupermem.CREDIT_BALANCE = 0;
                        objsupermem.IS_TRAN_START = true;
                        objsupermem.MEM_UNIQUE_ID = objsupermem.UName;
                        db.TBL_MASTER_MEMBER.Add(objsupermem);
                        await db.SaveChangesAsync();
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        var imageupload = db.TBL_MASTER_MEMBER.Find(objsupermem.MEM_ID);
                        imageupload.AADHAAR_FILE_NAME = aadhaarfilename;
                        imageupload.PAN_FILE_NAME = Pancardfilename;
                        db.Entry(imageupload).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        var servlist = await db.TBL_SETTINGS_SERVICES_MASTER.ToListAsync();
                        foreach (var lst in servlist)
                        {
                            TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                            {
                                MEMBER_ID = long.Parse(objsupermem.MEM_ID.ToString()),
                                SERVICE_ID = long.Parse(lst.SLN.ToString()),
                                ACTIVE_SERVICE = false
                            };
                            db.TBL_WHITELABLE_SERVICE.Add(objser);
                            await db.SaveChangesAsync();
                        }

                        ContextTransaction.Commit();
                        return Json("Your Distributor Information  Added Successfully", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Email Id is already exists. Please enter another email id.!!!", JsonRequestBehavior.AllowGet);
                    }
                    
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();                    
                    throw ex;
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);

                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }
        public ActionResult MerchantSignUp()
        {
            try
            {
                var db = new DBContext();                
                var StateName = db.TBL_STATES.ToList();
                var model = new TBL_MASTER_MEMBER();
                //string GetUniqueNo = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 10000);
                string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                string UniqId = "BMT" + GetUniqueNo;

                ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");                
                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_ID == 4 || x.ROLE_ID==5).ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                model.UName = UniqId;
                return View(model);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> POSTADDMerchant(TBL_MASTER_MEMBER objsupermem)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkEmail = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.EMAIL_ID == objsupermem.EMAIL_ID);
                    if (checkEmail == null)
                    {
                        ////string GetUniqueNo = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 10000);
                        //string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                        //string UniqId = "TIQ" + GetUniqueNo;
                        string DomaineName = Request.Url.Host;
                        var logochecking = (from x in db.TBL_MASTER_MEMBER
                                            join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                             on x.MEM_ID equals y.MEM_ID
                                            //where y.DOMAIN == DomaineName && y.STATUS == 1
                                            where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                            select new
                                            {
                                                mem_id = x.MEM_ID,
                                                logoPath = x.LOGO,
                                                LogoStyle = x.LOGO_STYLE,
                                                CompanyName = x.COMPANY
                                            }).FirstOrDefault();
                        objsupermem.BALANCE = 0;
                        decimal AmountVal = 0;
                        if (objsupermem.BLOCKED_BALANCE == null)
                        {
                            objsupermem.BLOCKED_BALANCE = 0;
                            objsupermem.BALANCE = 0;
                            AmountVal = 0;
                        }
                        else
                        {
                            objsupermem.BLOCKED_BALANCE = objsupermem.BLOCKED_BALANCE;
                            objsupermem.BALANCE = objsupermem.BLOCKED_BALANCE;
                            AmountVal = (decimal)objsupermem.BLOCKED_BALANCE;
                        }
                        objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                        objsupermem.UNDER_WHITE_LEVEL = logochecking.mem_id;
                        objsupermem.INTRODUCER = 0;
                        //objsupermem.BLOCKED_BALANCE = 0;
                        objsupermem.ACTIVE_MEMBER = true;
                        objsupermem.IS_DELETED = false;
                        objsupermem.JOINING_DATE = System.DateTime.Now;

                        //objsupermem.CREATED_BY = long.Parse(Session["UserId"].ToString());
                        objsupermem.CREATED_BY = logochecking.mem_id;
                        //objsupermem.CREATED_BY = CurrentUser.USER_ID;
                        objsupermem.LAST_MODIFIED_DATE = System.DateTime.Now;
                        //objsupermem.GST_MODE = 1;
                        //objsupermem.TDS_MODE = 1;
                        if (objsupermem.GST_FLAG != null)
                        {
                            objsupermem.GST_MODE = 1;
                        }
                        else
                        {
                            objsupermem.GST_MODE = 0;
                        }
                        if (objsupermem.TDS_FLAG != null)
                        {
                            objsupermem.TDS_MODE = 1;
                        }
                        else
                        {
                            objsupermem.TDS_MODE = 0;
                        }
                        objsupermem.DUE_CREDIT_BALANCE = 0;
                        objsupermem.CREDIT_BALANCE = 0;
                        objsupermem.IS_TRAN_START = true;
                        objsupermem.MEM_UNIQUE_ID = objsupermem.UName;
                        db.TBL_MASTER_MEMBER.Add(objsupermem);
                        await db.SaveChangesAsync();
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        var imageupload = db.TBL_MASTER_MEMBER.Find(objsupermem.MEM_ID);
                        imageupload.AADHAAR_FILE_NAME = aadhaarfilename;
                        imageupload.PAN_FILE_NAME = Pancardfilename;
                        db.Entry(imageupload).State = System.Data.Entity.EntityState.Modified;

                        await db.SaveChangesAsync();
                        long? intIdt = db.TBL_MASTER_MEMBER.Max(u => (long?)u.MEM_ID);
                        var servlist = await db.TBL_SETTINGS_SERVICES_MASTER.ToListAsync();
                        foreach (var lst in servlist)
                        {
                            TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                            {
                                MEMBER_ID = long.Parse(objsupermem.MEM_ID.ToString()),
                                SERVICE_ID = long.Parse(lst.SLN.ToString()),
                                ACTIVE_SERVICE = false
                            };
                            db.TBL_WHITELABLE_SERVICE.Add(objser);
                            await db.SaveChangesAsync();
                        }
                        TBL_ACCOUNTS MemberObj = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = long.Parse(objsupermem.MEM_ID.ToString()),
                            MEMBER_TYPE = "MEMBER",
                            TRANSACTION_TYPE = "ADD MEMBER",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                            AMOUNT = AmountVal,
                            NARRATION = "Add Member",
                            OPENING = 0,
                            CLOSING = AmountVal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            TDS = 0,
                            GST = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = ""
                        };
                        db.TBL_ACCOUNTS.Add(MemberObj);
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();

                        #region Email Code done by Sayan at 11-10-2020
                        string name = objsupermem.MEMBER_NAME;
                        string Regmsg = "Hi " + objsupermem.MEM_UNIQUE_ID + "(" + objsupermem.MEMBER_NAME + ")" + " \r\n. You have successfully joined in Boom Travels. For any query please contact your Admin. \r\n Regards,\r\n Boom Travels";
                        EmailHelper emailhelper = new EmailHelper();
                        string msgbody = emailhelper.GetEmailTemplate(name, Regmsg, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(objsupermem.EMAIL_ID.Trim(), "Welcome to Boom Travels!", msgbody);
                        #endregion

                        return Json("You have joined successfully in Boom Travels", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Email Id is already exists. Please enter another email id.!!!", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);

                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }
        [HttpPost]
        public async Task<JsonResult> CheckEmailAvailability(string emailid)
        {
            //initpage();////
            try
            {
                var context = new DBContext();
                var User = await context.TBL_MASTER_MEMBER.Where(model => model.EMAIL_ID == emailid).FirstOrDefaultAsync();
                if (User != null)
                {
                    return Json(new { result = "unavailable" });
                }
                else
                {
                    return Json(new { result = "available" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetDistributorMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;
                string DomaineName = Request.Url.Host;
                var logochecking = (from x in db.TBL_MASTER_MEMBER
                                    join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     on x.MEM_ID equals y.MEM_ID
                                    //where y.DOMAIN == DomaineName && y.STATUS == 1
                                    where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                    select new
                                    {
                                        mem_id = x.MEM_ID,
                                        logoPath = x.LOGO,
                                        LogoStyle = x.LOGO_STYLE,
                                        CompanyName = x.COMPANY
                                    }).FirstOrDefault();
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.MEMBER_ROLE == 4 && oper.UNDER_WHITE_LEVEL== logochecking.mem_id
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.MEM_ID,
                                               val = oper.MEM_ID
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                //Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        public static string TimeToHexString()
        {
            long ms = DateTime.Now.Second;
            long ms2 = DateTime.Now.Millisecond;
            return string.Format("{0:X}{1:X}", ms, ms2).ToLower();
        }
        public static string UniqueID
        {
            get
            {
                DateTime date = DateTime.Now;

                string uniqueID = String.Format(
                  "{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}{6:000}",
                  date.Year, date.Month, date.Day,
                  date.Hour, date.Minute, date.Second, date.Millisecond
                  );
                return uniqueID;
            }
        }
        public static string TicksToString()
        {
            long ticks = DateTime.Now.Ticks;
            return string.Format("{0:X}", ticks).ToLower();
        }
        static Random random = new Random();
        public static List<int> GenerateRandom(int count)
        {
            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();
            while (candidates.Count < count)
            {
                // May strike a duplicate.
                candidates.Add(random.Next());
            }

            // load them in to a list.
            List<int> result = new List<int>();
            result.AddRange(candidates);

            // shuffle the results:
            int i = result.Count;
            while (i > 1)
            {
                i--;
                int k = random.Next(i + 1);
                int value = result[k];
                result[k] = result[i];
                result[i] = value;
            }
            return result;
        }
        public string ToHex(int num)
        {
            //Check for invalid number
            if (num < 1 || num > 999999) //or throw an exception
                return "";
            return num.ToString("X").PadLeft(6, '0');
        }
        private string GetRandomKey(int len)
        {
            int maxSize = len;
            char[] chars = new char[30];
            string a;
            a = "1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data) { result.Append(chars[b % (chars.Length)]); }
            return result.ToString();
        }
        public ActionResult MemberLogin(string returnUrl)
        {
            var db = new DBContext();
            //var GetUser = db.TBL_AUTH_ADMIN_USERS.FirstOrDefault(x => x.USER_EMAIL == "neeraj.g@traveliq.in");


            string idval = TimeToHexString();
            string valueid = UniqueID;
            string iidd = TicksToString();
            var random = new Random(System.DateTime.Now.Millisecond);
            int randomNumber = random.Next(1, 50000);
            string tetrid = ToHex(6);
            string randomnumber = GetRandomKey(5);

            //var db = new DBContext();
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["UserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["UserId"].ToString();

                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();

                if (userinfo.MEMBER_ROLE == 1)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.MEMBER_ROLE == null)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.UNDER_WHITE_LEVEL == null)
                {
                    Response.RedirectToRoute("Dashboard", "Index");
                }
            }
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }


            string host = Request.Url.Host;
            //string host = "www.ramkrushnaharitravels.co.in";
            var logochecking = (from x in db.TBL_MASTER_MEMBER
                                join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                 on x.MEM_ID equals y.MEM_ID
                                //where y.DOMAIN == DomaineName && y.STATUS == 1
                                where y.DOMAIN.Contains(host) && y.STATUS == 1
                                select new
                                {
                                    logoPath = x.LOGO,
                                    LogoStyle = x.LOGO_STYLE,
                                    CompanyName = x.COMPANY
                                }).FirstOrDefault();
            if (logochecking != null)
            {
                if (logochecking.logoPath != null)
                {
                    if (logochecking.logoPath != "")
                    {
                        ViewBag.Logopath = Url.Content(logochecking.logoPath);
                        ViewBag.LogoStyle = logochecking.LogoStyle;
                        ViewBag.CompanyName = logochecking.CompanyName;
                    }
                    else
                    {
                        ViewBag.Logopath = "";
                        ViewBag.LogoStyle = "";
                        ViewBag.CompanyName = "";
                    }

                }
                else
                {
                    ViewBag.Logopath = "";
                    ViewBag.LogoStyle = "";
                    ViewBag.CompanyName = "";
                }
            }
            else
            {
                return RedirectToAction("DomainError", "Login");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> MemberLogin(LoginViewModel User)
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            //if (Session["PowerAdminUserId"] ==null)
            //{
            using (var db = new DBContext())
            {

                string DomaineName = Request.Url.Host;
                var logochecking = (from x in db.TBL_MASTER_MEMBER
                                    join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     on x.MEM_ID equals y.MEM_ID
                                    //where y.DOMAIN == DomaineName && y.STATUS == 1
                                    where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                    select new
                                    {
                                        logoPath = x.LOGO,
                                        LogoStyle = x.LOGO_STYLE,
                                        CompanyName = x.COMPANY
                                    }).FirstOrDefault();
                if (logochecking != null)
                {
                    if (logochecking.logoPath != null)
                    {
                        if (logochecking.logoPath != "")
                        {
                            Session["LogoPath"] = Url.Content(logochecking.logoPath);
                            Session["LogoStyle"] = logochecking.LogoStyle;
                            Session["CompanyName"] = logochecking.CompanyName;
                        }
                        else {
                            Session["LogoPath"] = "";
                            Session["LogoStyle"] = "";
                            Session["CompanyName"] = "";
                        }

                    }
                    else
                    {
                        Session["LogoPath"] = "";
                        Session["LogoStyle"] = "";
                        Session["CompanyName"] = "";
                    }
                }
                else
                {
                    return RedirectToAction("DomainError", "Login");
                }
                var DistGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
                if (DistGetMember != null)
                {
                    if (DistGetMember.MEMBER_ROLE == 4)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                string IpAddress = string.Empty;
                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                {
                                    IpAddress = User.GetIPAddress.Replace("\"", "");
                                }
                                else
                                {
                                    IpAddress = "";
                                }
                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                {
                                    MEM_ID = DistGetMember.MEM_ID,
                                    LOGINTIME = DateTime.Now,
                                    STATUS = 1,
                                    IP_ADDRESS = IpAddress
                                };
                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                db.SaveChanges();
                                Session["DistributorUserId"] = DistGetMember.MEM_ID;
                                Session["DistributorUserName"] = DistGetMember.UName;
                                Session["UserType"] = "Distributor";
                                Session["DistCompanyName"] = DistGetMember.COMPANY;
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();

                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }
                        }
                        else
                        {
                            return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else if (DistGetMember.MEMBER_ROLE == 5)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                string IpAddress = string.Empty;
                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                {
                                    IpAddress = User.GetIPAddress.Replace("\"", "");
                                }
                                else
                                {
                                    IpAddress = "";
                                }
                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                {
                                    MEM_ID = DistGetMember.MEM_ID,
                                    LOGINTIME = DateTime.Now,
                                    STATUS = 1,
                                    IP_ADDRESS = IpAddress
                                };
                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                db.SaveChanges();
                                Session["MerchantUserId"] = DistGetMember.MEM_ID;
                                Session["MerchantUserName"] = DistGetMember.UName;
                                Session["MerchantRailID"] = DistGetMember.RAIL_ID_QUANTITY;
                                Session["MERCHANTCompanyName"] = DistGetMember.COMPANY;
                                Session["UserType"] = "Merchant";
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }
                        }
                        else
                        {
                            return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Credential or Access Denied";
                    return View();
                }
            }
        }

        public ActionResult AdminLogin(string returnUrl)
        {
            var db = new DBContext();
            //var GetUser = db.TBL_AUTH_ADMIN_USERS.FirstOrDefault(x => x.USER_EMAIL == "neeraj.g@traveliq.in");


            string idval = TimeToHexString();
            string valueid = UniqueID;
            string iidd = TicksToString();
            var random = new Random(System.DateTime.Now.Millisecond);
            int randomNumber = random.Next(1, 50000);
            string tetrid = ToHex(6);
            string randomnumber = GetRandomKey(5);

            //var db = new DBContext();
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["UserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["UserId"].ToString();

                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();

                if (userinfo.MEMBER_ROLE == 1)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.MEMBER_ROLE == null)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.UNDER_WHITE_LEVEL == null)
                {
                    Response.RedirectToRoute("Dashboard", "Index");
                }
            }
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }


            string host = Request.Url.Host;
            //string host = "www.ramkrushnaharitravels.co.in";
            var logochecking = (from x in db.TBL_MASTER_MEMBER
                                join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                 on x.MEM_ID equals y.MEM_ID
                                //where y.DOMAIN == DomaineName && y.STATUS == 1
                                where y.DOMAIN.Contains(host) && y.STATUS == 1
                                select new
                                {
                                    logoPath = x.LOGO,
                                    LogoStyle = x.LOGO_STYLE,
                                    CompanyName = x.COMPANY
                                }).FirstOrDefault();
            if (logochecking != null)
            {
                if (logochecking.logoPath != null)
                {
                    if (logochecking.logoPath != "")
                    {
                        ViewBag.Logopath = Url.Content(logochecking.logoPath);
                        ViewBag.LogoStyle = logochecking.LogoStyle;
                        ViewBag.CompanyName = logochecking.CompanyName;
                    }
                    else
                    {
                        ViewBag.Logopath = "";
                        ViewBag.LogoStyle = "";
                        ViewBag.CompanyName = "";
                    }

                }
                else
                {
                    ViewBag.Logopath = "";
                    ViewBag.LogoStyle = "";
                    ViewBag.CompanyName = "";
                }
            }
            else
            {
                return RedirectToAction("DomainError", "Login");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AdminLogin(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            //if (Session["PowerAdminUserId"] ==null)
            //{
            using (var db = new DBContext())
            {

                string DomaineName = Request.Url.Host;
                var logochecking = (from x in db.TBL_MASTER_MEMBER
                                    join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     on x.MEM_ID equals y.MEM_ID
                                    //where y.DOMAIN == DomaineName && y.STATUS == 1
                                    where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                    select new
                                    {
                                        logoPath = x.LOGO,
                                        LogoStyle = x.LOGO_STYLE,
                                        CompanyName = x.COMPANY
                                    }).FirstOrDefault();
                if (logochecking != null)
                {
                    if (logochecking.logoPath != null)
                    {
                        if (logochecking.logoPath != "")
                        {
                            Session["LogoPath"] = Url.Content(logochecking.logoPath);
                            Session["LogoStyle"] = logochecking.LogoStyle;
                            Session["CompanyName"] = logochecking.CompanyName;
                        }
                        else {
                            Session["LogoPath"] = "";
                            Session["LogoStyle"] = "";
                            Session["CompanyName"] = "";
                        }
                    }
                    else
                    {
                        Session["LogoPath"] = "";
                        Session["LogoStyle"] = "";
                        Session["CompanyName"] = "";
                    }
                }
                else
                {
                    return RedirectToAction("DomainError", "Login");
                }
                var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true && x.MEMBER_ROLE == 1);
                if (GetMember != null)
                {
                    if (GetMember.MEMBER_ROLE == 1)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.MEM_ID equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && y.MEM_ID == GetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

                            if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                string IpAddress = string.Empty;
                                if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                {
                                    IpAddress = User.GetIPAddress.Replace("\"", "");
                                }
                                else
                                {
                                    IpAddress = "";
                                }
                                TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                {
                                    MEM_ID = GetMember.MEM_ID,
                                    LOGINTIME = DateTime.Now,
                                    STATUS = 1,
                                    IP_ADDRESS = IpAddress
                                };
                                db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                db.SaveChanges();
                                Session["WhiteLevelUserId"] = GetMember.MEM_ID;
                                Session["WhiteLevelUserName"] = GetMember.UName;
                                Session["UserType"] = "White Level";
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Credential or Access Denied";
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostMemberLogin(string MemberId="",string Password="")
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            //if (Session["PowerAdminUserId"] ==null)
            //{
            using (var db = new DBContext())
            {

                string DomaineName = Request.Url.Host;
                var logochecking = (from x in db.TBL_MASTER_MEMBER
                                    join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     on x.MEM_ID equals y.MEM_ID
                                    //where y.DOMAIN == DomaineName && y.STATUS == 1
                                    where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                    select new
                                    {
                                        logoPath = x.LOGO,
                                        LogoStyle = x.LOGO_STYLE,
                                        CompanyName = x.COMPANY
                                    }).FirstOrDefault();
                if (logochecking != null)
                {
                    if (logochecking.logoPath != null)
                    {
                        if (logochecking.logoPath != "")
                        {
                            Session["LogoPath"] = Url.Content(logochecking.logoPath);
                            Session["LogoStyle"] = logochecking.LogoStyle;
                            Session["CompanyName"] = logochecking.CompanyName;
                        }
                        else {
                            Session["LogoPath"] = "";
                            Session["LogoStyle"] = "";
                            Session["CompanyName"] = "";
                        }

                    }
                    else
                    {
                        Session["LogoPath"] = "";
                        Session["LogoStyle"] = "";
                        Session["CompanyName"] = "";
                    }
                }
                else
                {
                    //return RedirectToAction("DomainError", "Login");  new { Result = "Invalid Credential or Access Denied", msgRole = "x" }
                    return Json(new { Result = "Domain Error", msgRole = "0" });
                }
                var DistGetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.MEM_UNIQUE_ID == MemberId && x.User_pwd == Password && x.ACTIVE_MEMBER == true);
                if (DistGetMember != null)
                {
                    if (DistGetMember.MEMBER_ROLE == 4)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return Json(new { Result = "Invalid Credential or Access Denied", msgRole = "x" });
                                //return View();
                            }
                            else
                            {
                                //string IpAddress = string.Empty;
                                //if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                //{
                                //    IpAddress = User.GetIPAddress.Replace("\"", "");
                                //}
                                //else
                                //{
                                //    IpAddress = "";
                                //}
                                //TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                //{
                                //    MEM_ID = DistGetMember.MEM_ID,
                                //    LOGINTIME = DateTime.Now,
                                //    STATUS = 1,
                                //    IP_ADDRESS = IpAddress
                                //};
                                //db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                //db.SaveChanges();
                                Session["DistributorUserId"] = DistGetMember.MEM_ID;
                                Session["DistributorUserName"] = DistGetMember.UName;
                                Session["UserType"] = "Distributor";
                                Session["DistCompanyName"] = DistGetMember.COMPANY;
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();

                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return Json(new { Result = "Login Successfully.", msgRole = "4" });
                                //return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });

                            }
                        }
                        else
                        {
                            return Json(new { Result = "Domain Error", msgRole = "0" });
                            //return RedirectToAction("DomainError", "Login");
                        }
                    }
                    else if (DistGetMember.MEMBER_ROLE == 5)
                    {
                        var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                   join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                    on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                   //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                   //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                   where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == DistGetMember.MEM_ID
                                                   select new
                                                   {
                                                       MEM_ID = x.MEM_ID,
                                                       MEMBER_ROLE = x.MEMBER_ROLE,
                                                       ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                       User_pwd = x.User_pwd,
                                                       UName = x.UName,
                                                       DOMAIN = y.DOMAIN
                                                   }).FirstOrDefault();
                        if (GETWHITELevelDOMAIn != null)
                        {
                            Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                            if (DistGetMember.ACTIVE_MEMBER == false || DistGetMember.User_pwd != Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return Json(new { Result = "Invalid Credential or Access Denied", msgRole = "x" });
                                //return View();
                            }
                            else
                            {
                                //string IpAddress = string.Empty;
                                //if (User.GetIPAddress != "" && User.GetIPAddress != null)
                                //{
                                //    IpAddress = User.GetIPAddress.Replace("\"", "");
                                //}
                                //else
                                //{
                                //    IpAddress = "";
                                //}
                                //TBL_TRACE_MEMBER_LOGIN_DETAILS objlogin = new TBL_TRACE_MEMBER_LOGIN_DETAILS()
                                //{
                                //    MEM_ID = DistGetMember.MEM_ID,
                                //    LOGINTIME = DateTime.Now,
                                //    STATUS = 1,
                                //    IP_ADDRESS = IpAddress
                                //};
                                //db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Add(objlogin);
                                //db.SaveChanges();
                                Session["MerchantUserId"] = DistGetMember.MEM_ID;
                                Session["MerchantUserName"] = DistGetMember.UName;
                                Session["MerchantRailID"] = DistGetMember.RAIL_ID_QUANTITY;
                                Session["MERCHANTCompanyName"] = DistGetMember.COMPANY;
                                Session["UserType"] = "Merchant";
                                Session["CreditLimitAmt"] = DistGetMember.CREDIT_LIMIT.ToString().Replace(".00", "").Trim();
                                Session["ReservedCreditLimitAmt"] = DistGetMember.RESERVED_CREDIT_LIMIT.ToString().ToString().Replace(".00", "").Trim();
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(DistGetMember.UName + "||" + Encrypt.EncryptMe(DistGetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                //return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                                return Json(new { Result = "Login Successfully.", msgRole = "5" });
                            }
                        }
                        else
                        {
                            //return RedirectToAction("DomainError", "Login");
                            return Json(new { Result = "Domain Error", msgRole = "0" });
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return Json(new { Result = "Invalid Credential or Access Denied", msgRole = "x" });
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Credential or Access Denied";
                    return Json(new { Result = "Invalid Credential or Access Denied", msgRole = "x" });
                }
            }
        }

        public ActionResult CommingSoon()
        {
            return View();
        }

    }
}

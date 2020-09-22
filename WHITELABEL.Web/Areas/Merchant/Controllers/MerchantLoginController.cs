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

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantLoginController : Controller
    {
        // GET: Merchant/MerchantLogin
        public ActionResult Index(string returnUrl)
        {
            SystemClass sclass = new SystemClass();
            //string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["MerchantUserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["MerchantUserId"].ToString();
                var db = new DBContext();
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username && x.MEMBER_ROLE == 5).FirstOrDefault();
                Response.Redirect(Url.Action("Index", "MerchantDashboard", new { area = "Merchant" }));
            }

            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();
            ////// string userID = sclass.GetLoggedUser();
            ////var userpass = "india123";
            ////userpass = userpass.GetPasswordHash();
            //if (Session["SuperDistributorId"] == null || Session["DistributorUserId"] == null)
            //{
            using (var db = new DBContext())
            {
                var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER == true);
                if (GetMember != null)
                {
                    if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        //ViewBag.Islogin = false;
                        return View();
                    }
                    else
                    {
                        Session["MerchantUserId"] = GetMember.MEM_ID;
                        Session["MerchantUserName"] = GetMember.UName;
                        //ViewBag.Islogin = true;
                        //var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == GetMember.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
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
                        Session["UserType"] = "Merchant";
                        HttpCookie AuthCookie;
                        System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                        AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                        AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                        Response.Cookies.Add(AuthCookie);
                        return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                        //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Credential or Access Denied";
                    return View();
                }
            }
            //}
            //else
            //{
            //    Response.RedirectToRoute("Home", "Index");
            //}
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                long mem_id = 0;
                long.TryParse(Session["MerchantUserId"].ToString(), out mem_id);
                var GetLogindetails = db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Where(x => x.MEM_ID == mem_id).OrderByDescending(z => z.LOGINTIME).FirstOrDefault();
                if (GetLogindetails != null)
                {
                    GetLogindetails.LOGOUTTIME = DateTime.Now;
                    db.Entry(GetLogindetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["MerchantCompanyName"] = null;
                Session["ReservedCreditLimitAmt"] = null;
                Session["MerchantRailID"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session["CompanyName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("MerchantCompanyName");
                Session.Remove("MerchantRailID");
                Session.Remove("CreditLimitAmt");
                Session.Remove("ReservedCreditLimitAmt");
                Session.Remove("UserType");
                Session.Remove("CompanyName");
                Session.Clear();
                SystemClass sclass = new SystemClass();
                return RedirectToAction("Index", "login", new { area = "" });
                //Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnURL=" + Url.Action("Index", "Login"));
                return View();
            }
            else
            {
                return RedirectToAction("Index", "login", new { area = "" });
            }
        }
        [AllowAnonymous]
        public ActionResult Message()
        {
            FormsAuthentication.SignOut();
            Session["MerchantUserId"] = null;
            Session["MerchantUserName"] = null;
            Session.Clear();
            Session.Remove("MerchantUserId");
            Session.Remove("MerchantUserName");
            SystemClass sclass = new SystemClass();
            return View();
            //return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
        }
    }
}
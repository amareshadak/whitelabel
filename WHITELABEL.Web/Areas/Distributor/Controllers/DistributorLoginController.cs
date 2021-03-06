﻿using System;
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

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    public class DistributorLoginController : Controller
    {
        // GET: Distributor/DistributorLogin
        public ActionResult Index(string returnUrl)
        {
            SystemClass sclass = new SystemClass();
            //string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["DistributorUserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["DistributorUserId"].ToString();
                var db = new DBContext();
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();
                Response.Redirect(Url.Action("Index", "DistributorDashboard", new { area = "Distributor" }));
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
            //////string userID = sclass.GetLoggedUser();
            ////var userpass = "india123";
            ////userpass = userpass.GetPasswordHash();
            //if (Session["SuperDistributorId"] == null || Session["DistributorUserId"] == null)
            //{
            using (var db = new DBContext())
            {
                var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.MEMBER_ROLE== 4 && x.ACTIVE_MEMBER == true);
                if (GetMember != null)
                {
                    if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                    else
                    {
                        Session["DistributorUserId"] = GetMember.MEM_ID;
                        Session["DistributorUserName"] = GetMember.UName;
                        Session["UserType"] = "Distributor";
                        HttpCookie AuthCookie;
                        System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                        AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                        AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                        Response.Cookies.Add(AuthCookie);
                        return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
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
            if (Session["DistributorUserId"] != null)
            {
                var db = new DBContext();
                long mem_id = 0;
                long.TryParse(Session["DistributorUserId"].ToString(), out mem_id);
                var GetLogindetails = db.TBL_TRACE_MEMBER_LOGIN_DETAILS.Where(x => x.MEM_ID == mem_id).OrderByDescending(z => z.LOGINTIME).FirstOrDefault();
                if (GetLogindetails != null)
                {
                    GetLogindetails.LOGOUTTIME = DateTime.Now;
                    db.Entry(GetLogindetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                FormsAuthentication.SignOut();
                Session["DistributorUserId"] = null;
                Session["CreditLimitAmt"] = null;
                Session["ReservedCreditLimitAmt"] = null;
                Session["DistributorUserName"] = null;
                
                Session["UserType"] = null;
                Session["CompanyName"] = null;
                Session.Clear();
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("CreditLimitAmt");
                Session.Remove("ReservedCreditLimitAmt");
                Session.Remove("UserType");
                Session.Remove("CompanyName");
                Session.Clear();
                SystemClass sclass = new SystemClass();
                return RedirectToAction("Index", "login", new { area = "" });
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
            Session["DistributorUserId"] = null;
            Session["DistributorUserName"] = null;
            Session.Clear();
            Session.Remove("DistributorUserId");
            Session.Remove("DistributorUserName");
            SystemClass sclass = new SystemClass();
            return View();
            //return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
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
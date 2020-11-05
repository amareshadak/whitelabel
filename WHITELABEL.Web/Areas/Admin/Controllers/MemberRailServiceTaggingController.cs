﻿using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    public class MemberRailServiceTaggingController : AdminBaseController
    {
        // GET: Admin/MemberRailServiceTagging
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
        public ActionResult ServiceDetails(string memid = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    //long UserId = MemberCurrentUser.MEM_ID;
                    if (memid != "")
                    {


                        var userinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.UNDER_WHITE_LEVEL == Memid);
                        ViewBag.decrptSlId = Encrypt.EncryptMe(Memid.ToString());
                        //var ServiceInfo = db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == Memid).FirstOrDefault();
                        var memberinfo = (from x in db.TBL_WHITELABLE_SERVICE
                                          join
                                            y in db.TBL_SETTINGS_SERVICES_MASTER on x.SERVICE_ID equals y.SLN
                                          join
                                            w in db.TBL_MASTER_MEMBER on x.MEMBER_ID equals w.MEM_ID
                                          where x.MEMBER_ID == Memid && y.ACTIVESTATUS == true && y.MEM_ID == 1
                                          select new
                                          {
                                              Member_ID = x.MEMBER_ID,
                                              UserName = w.UName,
                                              ServiceName = y.SERVICE_NAME,
                                              SLN = x.SL_NO,
                                              ActiveService = x.ACTIVE_SERVICE

                                          }).AsEnumerable().Select(z => new TBL_WHITELABLE_SERVICE
                                          {
                                              MEMBER_ID = z.Member_ID,
                                              UserName = z.UserName,
                                              ServiceName = z.ServiceName,
                                              SL_NO = z.SLN,
                                              ACTIVE_SERVICE = z.ActiveService
                                          }).ToList();
                        ViewBag.checkVal = "1";
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.MEM_ID == Memid
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.MEMBER_NAME + "-" + x.MEMBER_MOBILE
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                        return View(memberinfo);
                    }
                    else
                    {

                        //var memberService = db.TBL_MASTER_MEMBER.ToList().Distinct();
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.INTRODUCER == Memid
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.MEMBER_NAME + "-" + x.MEMBER_MOBILE
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                        ViewBag.checkVal = "0";
                        var memberinfo = new List<TBL_WHITELABLE_SERVICE>();
                        return View(memberinfo);
                    }
                    //return View();
                }
                catch (Exception ex)
                {
                    //Logger.Error("Controller:-  DistributorService(Distributor), method:- ServiceDetails (POST) Line No:- 165", ex);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateActiveService(string memberId, string Id, bool isActive)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string decrptMemId = Decrypt.DecryptMe(memberId);
                    long MembId = long.Parse(decrptMemId);
                    string decrptSlID = Decrypt.DecryptMe(Id);
                    long SlId = long.Parse(decrptSlID);
                    var updateServiceStatus = await db.TBL_WHITELABLE_SERVICE.Where(x => x.SL_NO == SlId && x.MEMBER_ID == MembId).FirstOrDefaultAsync();
                    if (updateServiceStatus != null)
                    {
                        updateServiceStatus.ACTIVE_SERVICE = isActive;
                        db.Entry(updateServiceStatus).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    //Logger.Error("Controller:-  DistributorService(Distributor), method:- UpdateActiveService (POST) Line No:- 209", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }

        }

        public JsonResult GetserviceInfoMemberWise(string memid = "")
        {
            initpage();
            try
            {
                if (!string.IsNullOrEmpty(memid))
                {
                    var db = new DBContext();
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    var userinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == Memid);
                    ViewBag.UserName = userinfo.UName;
                    //var ServiceInfo = db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == Memid).FirstOrDefault();
                    var memberinfo = (from x in db.TBL_WHITELABLE_SERVICE
                                      join
                                        y in db.TBL_SETTINGS_SERVICES_MASTER on x.SERVICE_ID equals y.SLN
                                      join
                                        w in db.TBL_MASTER_MEMBER on x.MEMBER_ID equals w.MEM_ID
                                      where x.MEMBER_ID == Memid && y.ACTIVESTATUS == true && y.MEM_ID == 1
                                      select new
                                      {
                                          Member_ID = x.MEMBER_ID,
                                          UserName = w.UName,
                                          ServiceName = y.SERVICE_NAME,
                                          SLN = x.SL_NO,
                                          ActiveService = x.ACTIVE_SERVICE

                                      }).AsEnumerable().Select(z => new TBL_WHITELABLE_SERVICE
                                      {
                                          MEMBER_ID = z.Member_ID,
                                          UserName = z.UserName,
                                          ServiceName = z.ServiceName,
                                          SL_NO = z.SLN,
                                          ACTIVE_SERVICE = z.ActiveService
                                      }).ToList();
                    return Json(new { Result = "true", infor = memberinfo });
                }
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {
                //Logger.Error("Controller:-  DistributorService(Distributor), method:- GetserviceInfoMemberWise (POST) Line No:- 257", ex);
                throw;
            }


        }
    }
}
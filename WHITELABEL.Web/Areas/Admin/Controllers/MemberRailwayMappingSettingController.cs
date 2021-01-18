using log4net;
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
using WHITELABEL.Web.Areas.Admin.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberRailwayMappingSettingController : AdminBaseController
    {
        // GET: Admin/MemberRailwayMappingSetting
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
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.MEM_ID equals y.MEM_ID
                                     where x.UNDER_WHITE_LEVEL == 0
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = y.DOMAIN + "-" + x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName,
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
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
        [HttpPost]
        public JsonResult GetDistributor(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id && x.ACTIVE_MEMBER == true
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName,
                                     MobileNo = x.MEMBER_MOBILE
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName + "-" + z.MobileNo
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
                                 where x.INTRODUCER == dis_Id && x.ACTIVE_MEMBER == true
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName,
                                     MobileNo = x.MEMBER_MOBILE
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName + "-" + z.MobileNo
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult MerchantIndexgrid(string SearchVal = "")
        {
            var dbcontext = new DBContext();
            if (SearchVal != "")
            {

                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.UName.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.COMPANY.StartsWith(SearchVal) || x.COMPANY_GST_NO.StartsWith(SearchVal) || x.ADDRESS.StartsWith(SearchVal) || x.CITY.StartsWith(SearchVal) || x.PIN.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.AADHAAR_NO.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.RAIL_ID.StartsWith(SearchVal) || x.FACEBOOK_ID.StartsWith(SearchVal) || x.WEBSITE_NAME.StartsWith(SearchVal) || x.MEM_UNIQUE_ID.StartsWith(SearchVal) && x.ACTIVE_MEMBER==true && x.MEMBER_ROLE==5).ToList();
                var memberinfo = (from emp in dbcontext.TBL_MASTER_MEMBER
                                  where (emp.UName.StartsWith(SearchVal) || emp.MEMBER_MOBILE.StartsWith(SearchVal) || emp.MEMBER_NAME.StartsWith(SearchVal) || emp.COMPANY.StartsWith(SearchVal) || emp.COMPANY_GST_NO.StartsWith(SearchVal) || emp.ADDRESS.StartsWith(SearchVal) || emp.CITY.StartsWith(SearchVal) || emp.PIN.StartsWith(SearchVal) || emp.EMAIL_ID.StartsWith(SearchVal) || emp.AADHAAR_NO.StartsWith(SearchVal) || emp.PAN_NO.StartsWith(SearchVal) || emp.RAIL_ID.StartsWith(SearchVal) || emp.FACEBOOK_ID.StartsWith(SearchVal) || emp.WEBSITE_NAME.StartsWith(SearchVal) || emp.MEM_UNIQUE_ID.StartsWith(SearchVal)) && emp.MEMBER_ROLE == 5 && emp.ACTIVE_MEMBER == true
                                  select new
                                  {
                                      MEM_UNIQUE_ID = emp.MEM_UNIQUE_ID,
                                      EMAIL_ID = emp.EMAIL_ID,
                                      MEMBER_NAME = emp.MEMBER_NAME,
                                      COMPANY = emp.COMPANY,
                                      RAIL_PWD = emp.RAIL_PWD,
                                      MEMBER_MOBILE = emp.MEMBER_MOBILE,
                                      RAIL_ID = emp.RAIL_ID,
                                      SECURITY_PIN_MD5 = emp.SECURITY_PIN_MD5,
                                      ACTIVE_MEMBER = emp.ACTIVE_MEMBER,
                                      MEM_ID = emp.MEM_ID,
                                      RailSLN = ((dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).SLN) != null ? (dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).SLN) : 0),
                                      RailIdTagged = ((dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).RAIL_COMM_TAG) == null ? false : (dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).RAIL_COMM_TAG) == true ? true : false)
                                  }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                                  {
                                      RailIdTagged = z.RailIdTagged,
                                      RailSLN = z.RailSLN,
                                      MEM_UNIQUE_ID = z.MEM_UNIQUE_ID,
                                      EMAIL_ID = z.EMAIL_ID,
                                      MEMBER_NAME = z.MEMBER_NAME,
                                      COMPANY = z.COMPANY,
                                      MEMBER_MOBILE = z.MEMBER_MOBILE,
                                      RAIL_ID = z.RAIL_ID,
                                      RAIL_PWD = z.RAIL_PWD,
                                      SECURITY_PIN_MD5 = z.SECURITY_PIN_MD5,
                                      ACTIVE_MEMBER = z.ACTIVE_MEMBER,
                                      MEM_ID = z.MEM_ID
                                  }).ToList();
                return PartialView("MerchantIndexgrid", memberinfo);
            }
            else
            {
                //var memberinfo1 = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER == true).ToList();
                var memberinfo = (from emp in dbcontext.TBL_MASTER_MEMBER
                                   where emp.MEMBER_ROLE == 5 && emp.ACTIVE_MEMBER == true
                                   select new
                                   {
                                       MEM_UNIQUE_ID = emp.MEM_UNIQUE_ID,
                                       EMAIL_ID = emp.EMAIL_ID,
                                       MEMBER_NAME = emp.MEMBER_NAME,
                                       COMPANY = emp.COMPANY,
                                       RAIL_PWD = emp.RAIL_PWD,
                                       MEMBER_MOBILE = emp.MEMBER_MOBILE,
                                       RAIL_ID = emp.RAIL_ID,
                                       SECURITY_PIN_MD5 = emp.SECURITY_PIN_MD5,
                                       ACTIVE_MEMBER = emp.ACTIVE_MEMBER,
                                       MEM_ID = emp.MEM_ID,
                                       RailSLN= ((dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).SLN) != null ? (dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).SLN) : 0),
                                       RailIdTagged =((dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c=>c.MEM_ID==emp.MEM_ID).RAIL_COMM_TAG)==null?false: (dbcontext.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(c => c.MEM_ID == emp.MEM_ID).RAIL_COMM_TAG)==true?true: false)
                                   }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                                   {
                                       RailIdTagged = z.RailIdTagged,
                                       RailSLN=z.RailSLN,
                                       MEM_UNIQUE_ID = z.MEM_UNIQUE_ID,
                                       EMAIL_ID = z.EMAIL_ID,
                                       MEMBER_NAME = z.MEMBER_NAME,
                                       COMPANY = z.COMPANY,
                                       MEMBER_MOBILE = z.MEMBER_MOBILE,
                                       RAIL_ID = z.RAIL_ID,
                                       RAIL_PWD = z.RAIL_PWD,
                                       SECURITY_PIN_MD5 = z.SECURITY_PIN_MD5,
                                       ACTIVE_MEMBER = z.ACTIVE_MEMBER,
                                       MEM_ID = z.MEM_ID
                                   }).ToList();
                //                var memberinfo1 = (from e in dbcontext.TBL_RAIL_AGENT_INFORMATION
                //                                   join d in dbcontext.TBL_MASTER_MEMBER
                //on e.MEM_ID equals d.MEM_ID into emp
                //                                   from employee in emp.DefaultIfEmpty()
                //                                   where employee.MEMBER_ROLE == 5 && employee.ACTIVE_MEMBER == true
                //                                   select new
                //                                   {
                //                                       MEM_UNIQUE_ID = employee.MEM_UNIQUE_ID,
                //                                       EMAIL_ID = employee.EMAIL_ID,
                //                                       MEMBER_NAME = employee.MEMBER_NAME,
                //                                       COMPANY = employee.COMPANY,
                //                                       RAIL_PWD = employee.RAIL_PWD,
                //                                       MEMBER_MOBILE = employee.MEMBER_MOBILE,
                //                                       RAIL_ID = employee.RAIL_ID,
                //                                       SECURITY_PIN_MD5 = employee.SECURITY_PIN_MD5,
                //                                       ACTIVE_MEMBER = employee.ACTIVE_MEMBER,
                //                                       MEM_ID = employee.MEM_ID,
                //                                       DISTRIBUTOR_ID = e.SLN,
                //                                   }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                //                                   {
                //                                       DISTRIBUTOR_ID = z.DISTRIBUTOR_ID,
                //                                       MEM_UNIQUE_ID = z.MEM_UNIQUE_ID,
                //                                       EMAIL_ID = z.EMAIL_ID,
                //                                       MEMBER_NAME = z.MEMBER_NAME,
                //                                       COMPANY = z.COMPANY,
                //                                       MEMBER_MOBILE = z.MEMBER_MOBILE,
                //                                       RAIL_ID = z.RAIL_ID,
                //                                       RAIL_PWD = z.RAIL_PWD,
                //                                       SECURITY_PIN_MD5 = z.SECURITY_PIN_MD5,
                //                                       ACTIVE_MEMBER = z.ACTIVE_MEMBER,
                //                                       MEM_ID = z.MEM_ID
                //                                   }).Take(5).ToList();



                return PartialView("MerchantIndexgrid", memberinfo);
            }
            

        }

        //public PartialViewResult MerchantIndexgrid(string WhiteLevel = "", string Distributor = "", string Merchant = "")
        //{
        //    var dbcontext = new DBContext();
        //    if (WhiteLevel == "" && Merchant == "" && Distributor == "")
        //    {
        //        var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 1).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        return PartialView("MerchantIndexgrid", memberinfo);
        //    }
        //    else if (WhiteLevel != "" && Distributor == "" && Merchant == "")
        //    {
        //        long wlpiD = 0;
        //        long.TryParse(WhiteLevel, out wlpiD);
        //        var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 1 && x.MEM_ID == wlpiD).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        return PartialView("MerchantIndexgrid", memberinfo);
        //    }
        //    else if (WhiteLevel != "" && Distributor != "" && Merchant == "")
        //    {
        //        long DistId = 0;
        //        long.TryParse(Distributor, out DistId);
        //        var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 4 && x.MEM_ID == DistId && x.ACTIVE_MEMBER==true).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 5 && x.INTRODUCER==DistId).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        return PartialView("MerchantIndexgrid", memberinfo);
        //    }
        //    else if (WhiteLevel != "" && Distributor != "" && Merchant != "")
        //    {
        //        long MertId = 0;
        //        long.TryParse(Merchant, out MertId);
        //        var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MertId && x.ACTIVE_MEMBER == true).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        return PartialView("MerchantIndexgrid", memberinfo);
        //    }
        //    else
        //    {
        //        var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER == true).ToList().OrderByDescending(x => x.JOINING_DATE);
        //        return PartialView("MerchantIndexgrid", memberinfo);
        //    }

        //}
        public async Task<ActionResult> GetMerchantDetails(string memid = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var dbcontext = new DBContext();
                    var StateName = await dbcontext.TBL_STATES.ToListAsync();
                    ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");

                    if (memid != "")
                    {

                        var model = new TBL_MASTER_MEMBER();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "RETAILER").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        ViewBag.checkstatus = "1";
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        //long Memid = long.Parse(decrptSlId);
                        long idval = long.Parse(decrptSlId);
                        model = await dbcontext.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == idval);
                        model.BLOCKED_BALANCE = Math.Round(Convert.ToDecimal(model.BLOCKED_BALANCE), 0);
                        model.MEMBER_ROLE = model.MEMBER_ROLE;
                        ViewBag.checkmail = true;
                        Session.Remove("msg");
                        Session["msg"] = null;
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        return View(model);
                    }
                    else
                    {

                        ViewBag.checkstatus = "0";
                        //var memberrole =await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToListAsync();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        var user = new TBL_MASTER_MEMBER();
                        Session.Remove("msg");
                        Session["msg"] = null;
                        ViewBag.checkmail = false;
                        user.UName = "";
                        ModelState.SetModelValue("UName", new ValueProviderResult(null, string.Empty, CultureInfo.InvariantCulture));
                        ModelState.SetModelValue("User_pwd", new ValueProviderResult(null, string.Empty, CultureInfo.InvariantCulture));
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        ModelState.Clear();
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    //throw ex;
                    Logger.Error("Controller:-  CreateMember(PowerAdmin), method:- CreateMember (GET) Line No:- 136", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> POSTADDMerchantDetails(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> POSTUpdateMerchantDetails(TBL_MASTER_MEMBER objsupermem)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var CheckUser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == objsupermem.MEM_ID).FirstOrDefaultAsync();
                    if (CheckUser != null)
                    {
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;

                        CheckUser.UName = objsupermem.UName;
                        CheckUser.STATE_ID = objsupermem.STATE_ID;
                        CheckUser.MEMBER_MOBILE = objsupermem.MEMBER_MOBILE;
                        CheckUser.FACEBOOK_ID = objsupermem.FACEBOOK_ID;
                        CheckUser.WEBSITE_NAME = objsupermem.WEBSITE_NAME;
                        CheckUser.NOTES = objsupermem.NOTES;
                        CheckUser.OPTIONAL_EMAIL_ID = objsupermem.OPTIONAL_EMAIL_ID;
                        CheckUser.SEC_OPTIONAL_EMAIL_ID = objsupermem.SEC_OPTIONAL_EMAIL_ID;
                        CheckUser.OPTIONAL_MOBILE_NO = objsupermem.OPTIONAL_MOBILE_NO;
                        CheckUser.SEC_OPTIONAL_MOBILE_NO = objsupermem.SEC_OPTIONAL_MOBILE_NO;
                        CheckUser.OLD_MEMBER_ID = objsupermem.OLD_MEMBER_ID;
                        //CheckUser.UNDER_WHITE_LEVEL = value.UNDER_WHITE_LEVEL;
                        CheckUser.MEMBER_MOBILE = objsupermem.MEMBER_MOBILE;
                        CheckUser.MEMBER_NAME = objsupermem.MEMBER_NAME;
                        CheckUser.COMPANY = objsupermem.COMPANY;
                        //CheckUser.INTRODUCER = value.INTRODUCER;
                        CheckUser.ADDRESS = objsupermem.ADDRESS;
                        CheckUser.CITY = objsupermem.CITY;
                        CheckUser.PIN = objsupermem.PIN;
                        //CheckUser.EMAIL_ID = value.EMAIL_ID;
                        CheckUser.SECURITY_PIN_MD5 = objsupermem.SECURITY_PIN_MD5;
                        CheckUser.BLOCKED_BALANCE = objsupermem.BLOCKED_BALANCE;
                        CheckUser.DUE_CREDIT_BALANCE = 0;
                        CheckUser.CREDIT_BALANCE = 0;
                        CheckUser.IS_TRAN_START = true;
                        CheckUser.RAIL_ID = objsupermem.RAIL_ID.Trim();
                        CheckUser.RAIL_PWD = objsupermem.RAIL_PWD;
                        db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        var GetRailInfo = await db.TBL_RAIL_AGENT_INFORMATION.Where(x => x.RAIL_USER_ID == objsupermem.RAIL_ID).FirstOrDefaultAsync();
                        if (GetRailInfo != null)
                        {
                            GetRailInfo.RAIL_USER_ID = objsupermem.RAIL_ID;
                            GetRailInfo.MEM_ID = objsupermem.MEM_ID;
                            GetRailInfo.IRCTC_LOGIN_ID = objsupermem.RAIL_ID;
                            GetRailInfo.STATUS = true;
                            db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            TBL_RAIL_AGENT_INFORMATION objRail = new TBL_RAIL_AGENT_INFORMATION()
                            {
                                MEM_ID = objsupermem.MEM_ID,
                                RAIL_USER_ID = objsupermem.RAIL_ID,
                                IRCTC_LOGIN_ID = objsupermem.RAIL_ID,
                                //TRAVEL_AGENT_NAME=null,
                                //AGENCY_NAME=null,
                                //OFFICE_ADDRESS=null,
                                //RESIDENCE_ADDRESS=null,
                                //EMAIL_ID=null,
                                //MOBILE_NO=null,
                                //CERTIFICATE_BEGIN_DATE=null,
                                //CERTIFICATE_END_DATE=null,
                                STATUS = true,
                                RAIL_COMM_TAG = false
                            };
                            db.TBL_RAIL_AGENT_INFORMATION.Add(objRail);
                            await db.SaveChangesAsync();
                        }
                        ContextTransaction.Commit();

                        return Json("Merchant updated successfully.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please try after sometime", JsonRequestBehavior.AllowGet);
                    }

                    //throw new Exception();


                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  Retailer(Distributor), method:- CreateMember (POST) Line No:- 336", ex);
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }
        }

        [HttpPost]
        public JsonResult GetmerchantInformationList(string Prefix)
        {
            //Note : you can bind same list from database  
            var Db = new DBContext();

            //Searching records from list using LINQ query  
            var CityList = (from N in Db.TBL_MASTER_MEMBER
                            where N.UName.StartsWith(Prefix) && N.MEMBER_ROLE == 5
                            select new
                            {
                                N.UName,
                                N.MEM_ID
                            }).ToList();
            return Json(CityList, JsonRequestBehavior.AllowGet);
            ////Searching records from list using LINQ query  
            //var CityList = (from N in Db.TBL_MASTER_MEMBER
            //                where N.UName.StartsWith(Prefix) && N.MEMBER_ROLE==5
            //                select new {
            //                    //N.UName,N.MEM_ID
            //                    label = N.UName,
            //                    val = N.MEM_ID
            //                }).ToList();
            //return Json(CityList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> POSTUpdateRailAgentDetails(RailAgentViewModel objRailAgent)
        public async Task<ActionResult> POSTUpdateRailAgentDetails(TBL_MASTER_MEMBER objRailAgent, HttpPostedFileBase DSCFileUpload)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var GEtRailInfo = await db.TBL_RAIL_AGENT_INFORMATION.Where(x => x.MEM_ID == objRailAgent.MEM_ID).FirstOrDefaultAsync();
                    if (GEtRailInfo != null)
                    {
                        GEtRailInfo.TRAVEL_AGENT_NAME = objRailAgent.TRAVEL_AGENT_NAME;
                        GEtRailInfo.AGENCY_NAME = objRailAgent.AGENCY_NAME;
                        GEtRailInfo.OFFICE_ADDRESS = objRailAgent.OFFICE_ADDRESS;
                        GEtRailInfo.RESIDENCE_ADDRESS = objRailAgent.RESIDENCE_ADDRESS;
                        GEtRailInfo.EMAIL_ID = objRailAgent.AGENTEMAIL_ID;
                        GEtRailInfo.MOBILE_NO = objRailAgent.MOBILE_NO;
                        GEtRailInfo.OFFICE_PHONE = objRailAgent.OFFICE_PHONE;
                        GEtRailInfo.PAN_NO = objRailAgent.AGENTPAN_NO;
                        GEtRailInfo.DIGITAL_CERTIFICATE_DETAILS = objRailAgent.DIGITAL_CERTIFICATE_DETAILS;
                        GEtRailInfo.CERTIFICATE_BEGIN_DATE = objRailAgent.CERTIFICATE_BEGIN_DATE;
                        GEtRailInfo.CERTIFICATE_END_DATE = objRailAgent.CERTIFICATE_END_DATE;
                        GEtRailInfo.USER_STATE = objRailAgent.USER_STATE;
                        GEtRailInfo.AGENT_VERIFIED_STATUS = objRailAgent.AGENT_VERIFIED_STATUS;
                        GEtRailInfo.DEACTIVATION_REASON = objRailAgent.DEACTIVATION_REASON;
                        GEtRailInfo.AADHAAR_VERIFICATION_STATUS = objRailAgent.AADHAAR_VERIFICATION_STATUS;
                        GEtRailInfo.ENTRY_DATE = DateTime.Now;
                        GEtRailInfo.FLAG1 = "Status Value";
                        GEtRailInfo.FLAG2 = "Status Value";
                        GEtRailInfo.STATUS = true;
                        GEtRailInfo.STATE_ID = objRailAgent.AGENT_STATE_ID;
                        GEtRailInfo.RAIL_COMM_TAG = false;
                        db.Entry(GEtRailInfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        string DSCfilename = string.Empty;
                        if (DSCFileUpload != null)
                        {
                            string DSCFilepath = Path.GetFileName(DSCFileUpload.FileName);
                            string DSCFilefileName = DSCFilepath.Substring(DSCFilepath.LastIndexOf(((char)92)) + 1);
                            int index = DSCFilefileName.LastIndexOf('.');
                            string onyName = DSCFilefileName.Substring(0, index);
                            string DSCfileExtension = DSCFilefileName.Substring(index + 1);
                            var InputPanCard = objRailAgent.MEM_ID + "_" + onyName + "." + DSCfileExtension;
                            //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                            var PanserverSavePath = (Server.MapPath(@"/DSCFilesList/") + InputPanCard);
                            DSCfilename = "~/DSCFilesList/" + InputPanCard;
                            DSCFileUpload.SaveAs(PanserverSavePath);
                        }
                        var GetDSC_Info = db.TBL_RAIL_DSC_INFORMATION.FirstOrDefault(x => x.MEM_ID == objRailAgent.MEM_ID && x.RAIL_USER_ID == objRailAgent.RAIL_USER_ID);
                        if (GetDSC_Info == null)
                        {

                            TBL_RAIL_DSC_INFORMATION objraildsc = new TBL_RAIL_DSC_INFORMATION()
                            {
                                RAIL_USER_ID = GEtRailInfo.RAIL_USER_ID,
                                MEM_ID = objRailAgent.MEM_ID,
                                RAIL_DSC_ID = objRailAgent.RAIL_USER_ID,
                                CREATE_DATE = DateTime.Now,
                                STATUS = true,
                                DSC_DOC_Path = DSCfilename
                            };
                            db.TBL_RAIL_DSC_INFORMATION.Add(objraildsc);
                            db.SaveChanges();
                        }
                        else
                        {
                            TBL_RAIL_DSC_INFORMATION objraildsc = new TBL_RAIL_DSC_INFORMATION()
                            {
                                RAIL_USER_ID = GEtRailInfo.RAIL_USER_ID,
                                MEM_ID = objRailAgent.MEM_ID,
                                RAIL_DSC_ID = objRailAgent.RAIL_USER_ID,
                                CREATE_DATE = DateTime.Now,
                                STATUS = true,
                                DSC_DOC_Path = DSCfilename
                            };
                            db.TBL_RAIL_DSC_INFORMATION.Add(objraildsc);
                            db.SaveChanges();
                        }
                        ContextTransaction.Commit();
                        Session["msg"] = "Data Updated Successfully";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session["msg"] = "Please try again later";
                        return RedirectToAction("Index");
                    }

                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  Retailer(Distributor), method:- CreateMember (POST) Line No:- 336", ex);
                    Session["msg"] = "Please try again later";
                    return RedirectToAction("Index");
                    throw ex;
                }
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckRDSUserId(string rdsUserId)
        {
            var context = new DBContext();
            var User = await context.TBL_MASTER_MEMBER.Where(model => model.RAIL_ID == rdsUserId).FirstOrDefaultAsync();
            if (User != null)
            {
                return Json(new { result = "unavailable", UserName = User.UName, MEM_ID = User.MEM_ID, MobileNo = User.MEMBER_MOBILE, EmailID = User.EMAIL_ID, CompanyName = User.COMPANY, Address = User.ADDRESS, GSTNo = User.COMPANY_GST_NO, PANNo = User.PAN_NO, RDSId = User.RAIL_USER_ID });
                //return Json(new { result = "unavailable",Mem_Name= list.DOMAIN,mem_Id =User.FROM_MEMBER,Req_Date=User.REQUEST_DATE,amt=User.AMOUNT,Bankid=User.BANK_ACCOUNT,paymethod=User.PAYMENT_METHOD,Transdetails=User.TRANSACTION_DETAILS,BankCharges=User.BANK_CHARGES});
            }
            else
            {
                return Json(new { result = "available", UserName = "", MEM_ID = "0" });
            }
        }


        public ActionResult MerchantDSCFilesList()
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
                var db = new DBContext();
                var documentlist = db.TBL_RAIL_DSC_INFORMATION.ToList();
                //return PartialView(documentlist);
                return PartialView("IndexGrid", documentlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public PartialViewResult DSCIndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();

                var memberinfo = (from x in dbcontext.TBL_RAIL_DSC_INFORMATION
                                  join
                                y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                  select new
                                  {
                                      MemberName = y.UName,
                                      MEM_ID=x.MEM_ID,
                                      DistributorName = (dbcontext.TBL_MASTER_MEMBER.Where(d => d.MEM_ID == y.INTRODUCER).Select(s => s.UName).FirstOrDefault()),
                                      rail_User_Id = x.RAIL_USER_ID,
                                      Mem_Id = x.MEM_ID,
                                      SLN = x.SLN,
                                      DocFile = x.DSC_DOC_Path,
                                      UpdatedDate = x.CREATE_DATE
                                  }).AsEnumerable().Select(z => new TBL_RAIL_DSC_INFORMATION
                                  {
                                      SLN = z.SLN,
                                      MEM_ID=z.MEM_ID,
                                      MerchantName = z.MemberName,
                                      DistributorName = z.DistributorName,
                                      RAIL_USER_ID = z.rail_User_Id,
                                      DSC_DOC_Path = z.DocFile,
                                      CREATE_DATE = z.UpdatedDate
                                  }).ToList();
                //return Json(CityList, JsonRequestBehavior.AllowGet);


                //var memberinfo = dbcontext.TBL_RAIL_DSC_INFORMATION.ToList();
                return PartialView("DSCIndexGrid", memberinfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileResult downloadfiles(string memid="")
        {
            try
            {
                var db = new DBContext();
                string decrptMemId = Decrypt.DecryptMe(memid);
                long fid = long.Parse(decrptMemId);
                var filename = db.TBL_RAIL_DSC_INFORMATION.Where(x => x.SLN == fid).FirstOrDefault();
                string filepath = string.Empty;
                string fileNameinfo = string.Empty;
                filepath = filename.DSC_DOC_Path.ToString();
                fileNameinfo = "DSCFile";
                //if (type == "Pan")
                //{
                //    filename = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == fid).FirstOrDefault();
                //    filepath = filename.PAN_FILE_NAME.ToString();
                //    fileNameinfo = "PanCard";
                //}
                //else
                //{
                //    filename = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == fid).FirstOrDefault();
                //    filepath = filename.AADHAAR_FILE_NAME.ToString();
                //    fileNameinfo = "AadhaarCard";
                //}


                string contentType = string.Empty;

                string path = filepath;
                string fileName = path.Substring(path.LastIndexOf(((char)92)) + 1);
                int index = fileName.LastIndexOf('.');
                string onyName = fileName.Substring(0, index);
                string fileExtension = fileName.Substring(index + 1);
                if (fileExtension == "png" || fileExtension == "PNG")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "Images/png";
                }
                else if (fileExtension == "jpg" || fileExtension == "JPG" || fileExtension == "jpeg")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "Images/jpg";
                }
                //else if (fileExtension == "pdf" || fileExtension == "pdf")
                else if (fileExtension == "cer" || fileExtension == "cer")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "application/cer";
                }
                //Parameters to file are
                //1. The File Path on the File Server
                //2. The content type MIME type
                //3. The parameter for the file save by the browser
                return File(filepath, contentType, fileNameinfo);
                //string filename = (from f in files
                //                   where f.FileId == fid
                //                   select f.FilePath).First();
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminKYCVerificationC(PowerAdmin), method:- downloadfiles (Files) Line No:- 152", ex);
                throw;
            }



        }


        // for export data
        [HttpGet]
        public FileResult ExportIndex(string Disid = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                long Mem_Id = 0;
                long.TryParse(Disid, out Mem_Id);
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_MASTER_MEMBER> grid = CreateExportableGrid(Mem_Id);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_MASTER_MEMBER> gridRow in grid.Rows)
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

        private IGrid<TBL_MASTER_MEMBER> CreateExportableGrid(long Mem_Id = 0)
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false && x.CREATED_BY== userid).ToList();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == Mem_Id).ToList();
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Agent ID").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_NAME).Titled("Agent Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Agency Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.RAIL_ID).Titled("Rail User Id").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SECURITY_PIN_MD5).Titled("M Pin").Filterable(true).Sortable(true);
            grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
            //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
            //    .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a>&nbsp;<a href='" + @Url.Action("CreateMember", "MemberAPILabel", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Edit</a><a href='" + @Url.Action("ServiceDetails", "MemberService", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Service</a></div>");
            //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
            //  .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='MemberStatus(\"" + model.MEM_ID + "\",\"" + model.ACTIVE_MEMBER + "\");return 0;'>" + (model.ACTIVE_MEMBER == true ? "Deactive" : "Active") + "</a>");
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 99999999;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }

        public async Task<ActionResult> ChangeMemberPassword(string memid = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                if (memid != "")
                {
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long idval = long.Parse(decrptSlId);
                    var Getmember = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == idval);
                    var GetMember_Role = db.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == Getmember.MEMBER_ROLE).ROLE_NAME;
                    MemberPasswordChange objmodel = new MemberPasswordChange()
                    {
                        MemberName = Getmember.MEMBER_NAME,
                        MemberRole = GetMember_Role,
                        MemberEmailId = Getmember.EMAIL_ID,
                        MemberMobileNo = Getmember.MEMBER_MOBILE,
                        MEM_ID = Getmember.MEM_ID
                    };
                    return View(objmodel);
                }
                else
                {
                    //"/PowerAdminMerchantSearch/Index";
                    Response.Redirect(Url.Action("Index", "PowerAdminMerchantSearch"));
                    return View();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        //public async Task<ActionResult> ChangePassword(DistributorChangePasswordModel value)
        public async Task<JsonResult> PostChangeMemberPassword(MemberPasswordChange value)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long mem_id = value.MEM_ID;
                    var changepass = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == mem_id).FirstOrDefault();
                    if (changepass != null)
                    {
                        var userpass = value.User_pwd;
                        changepass.RAIL_PWD = userpass;
                        db.Entry(changepass).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        //throw new Exception();
                        var token = TokenGenerator.GenerateToken();
                        var PasswordResetObj = new TBL_PASSWORD_RESET
                        {
                            ID = token,
                            EmailID = changepass.EMAIL_ID,
                            Time = DateTime.Now
                        };
                        db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Password changed Successfully", JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json("Please contact to your admin for change password  ", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberChangePassword(Admin), method:- ChangePassword (POST) Line No:- 271", ex);
                    throw ex;
                    return Json("Try Again After Sometime", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostRailCommissionSettingInfor(TBL_RAIL_AGENTS_COMMISSION objrailComm)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkRailAgentComm = await db.TBL_RAIL_AGENTS_COMMISSION.FirstOrDefaultAsync(x => x.MEM_ID == objrailComm.MEM_ID);
                    if (checkRailAgentComm != null)
                    {
                        checkRailAgentComm.PG_MAX_VALUE = objrailComm.PG_MAX_VALUE;
                        checkRailAgentComm.PG_EQUAL_LESS_2000 = objrailComm.PG_EQUAL_LESS_2000;
                        checkRailAgentComm.PG_EQUAL_GREATER_2000 = objrailComm.PG_EQUAL_GREATER_2000;
                        checkRailAgentComm.PG_GST_STATUS = objrailComm.PG_GST_STATUS;
                        checkRailAgentComm.ADDITIONAL_CHARGE_MAX_VAL = objrailComm.ADDITIONAL_CHARGE_MAX_VAL;
                        checkRailAgentComm.ADDITIONAL_CHARGE_AC = objrailComm.ADDITIONAL_CHARGE_AC;
                        checkRailAgentComm.ADDITIONAL_CHARGE_NON_AC = objrailComm.ADDITIONAL_CHARGE_NON_AC;
                        checkRailAgentComm.ADDITIONAL_GST_STATUS = objrailComm.ADDITIONAL_GST_STATUS;
                        checkRailAgentComm.COMM_UPDATE_DATE = DateTime.Now;
                        checkRailAgentComm.STATUS = true;
                        if (objrailComm.Additional_Charges_Apply_Val == "1")
                        {
                            checkRailAgentComm.ADDITIONAL_CHARGES_APPLY = true;
                        }
                        else
                        {
                            checkRailAgentComm.ADDITIONAL_CHARGES_APPLY = false;
                        }
                        if (objrailComm.PG_Charges_Apply_Val == "1")
                        {
                            checkRailAgentComm.PG_CHARGES_APPLY = true;
                        }
                        else
                        {
                            checkRailAgentComm.PG_CHARGES_APPLY = false;
                        }
                        db.Entry(checkRailAgentComm).State = System.Data.Entity.EntityState.Modified;

                        var RailAgentid = await db.TBL_RAIL_AGENT_INFORMATION.FirstOrDefaultAsync(x => x.SLN == objrailComm.Rail_table_Id);
                        RailAgentid.RAIL_COMM_TAG = true;
                        db.Entry(RailAgentid).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json("Commission Save Successfully.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        bool ADDnlCharges = false;
                        bool PGCharges = false;
                        if (objrailComm.Additional_Charges_Apply_Val == "1")
                        {
                            ADDnlCharges = true;
                        }
                        else
                        {
                            ADDnlCharges = false;
                        }
                        if (objrailComm.PG_Charges_Apply_Val == "1")
                        {
                            PGCharges = true;
                        }
                        else
                        {
                            PGCharges = false;
                        }

                        TBL_RAIL_AGENTS_COMMISSION RAILCOMM = new TBL_RAIL_AGENTS_COMMISSION()
                        {
                            WLP_ID = objrailComm.WLP_ID,
                            DIST_ID = objrailComm.DIST_ID,
                            MEM_ID = objrailComm.MEM_ID,
                            RAIL_AGENT_ID = objrailComm.RAIL_AGENT_ID,
                            PG_MAX_VALUE = objrailComm.PG_MAX_VALUE,
                            PG_EQUAL_LESS_2000 = objrailComm.PG_EQUAL_LESS_2000,
                            PG_EQUAL_GREATER_2000 = objrailComm.PG_EQUAL_GREATER_2000,
                            PG_GST_STATUS = objrailComm.PG_GST_STATUS,
                            ADDITIONAL_CHARGE_MAX_VAL = objrailComm.ADDITIONAL_CHARGE_MAX_VAL,
                            ADDITIONAL_CHARGE_AC = objrailComm.ADDITIONAL_CHARGE_AC,
                            ADDITIONAL_CHARGE_NON_AC = objrailComm.ADDITIONAL_CHARGE_NON_AC,
                            ADDITIONAL_GST_STATUS = objrailComm.ADDITIONAL_GST_STATUS,
                            COMM_ENTRY_DATE = DateTime.Now,
                            STATUS = true,
                            PG_CHARGES_APPLY = PGCharges,
                            ADDITIONAL_CHARGES_APPLY = ADDnlCharges
                        };
                        db.TBL_RAIL_AGENTS_COMMISSION.Add(RAILCOMM);
                        var RailAgentid = await db.TBL_RAIL_AGENT_INFORMATION.FirstOrDefaultAsync(x => x.SLN == objrailComm.Rail_table_Id);
                        RailAgentid.RAIL_COMM_TAG = true;
                        db.Entry(RailAgentid).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json("Commission Save Successfully.", JsonRequestBehavior.AllowGet);
                    }


                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  Retailer(Distributor), method:- CreateMember (POST) Line No:- 336", ex);
                    Session["msg"] = "Please try again later";
                    return Json("");
                    throw ex;
                }
            }
            //return Json("");
        }


    }
}
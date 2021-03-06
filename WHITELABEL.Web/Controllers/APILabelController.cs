﻿using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Controllers
{
   public class APILabelController : BaseController
    {
        public void initpage()
        {
            try
            {
                //string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                //ViewBag.Siteroot = baseUrl + ConfigurationManager.AppSettings["Siteroot"];

                ViewBag.ControllerName = "APILabel";

                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {

                    //string[] pair = userID.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    //string email = pair[0];
                    //string passwordHash = pair[1];
                    //string timezoneoffset = pair[2];
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);

                    if (currUser != null)
                    {
                        Session["UserId"] = currUser.USER_ID;
                       // Session["UserName"] = currUser.UserName;

                    }
                }
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login/LogOut");
                    return;
                }
                bool Islogin = false;

                if (Session["UserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentUser.USER_ID;
                }
                ViewBag.Islogin = Islogin;
                // string countryPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\csv\\" + "Countries.csv"; 
                //using (MainContext maincontext = new MainContext())
                //{
                //    int uid = CurrentUser.UserId;
                //    var UserProfile = maincontext.tbl_UserProfiles.Where(x => x.UserId == uid).FirstOrDefault();
                //    var User = maincontext.tbl_Users.Where(x => x.UserId == uid).FirstOrDefault();
                //    ViewBag.UserName = User.UserName;
                //    ViewBag.UserImage = UserProfile.UserImage;
                //}
                //ViewBag.IsChatPage = "No";
                //ViewBag.IsleftBarMenuOpen = false;
            }
            catch (Exception e)
            {
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;

            }
        }

        // GET: APILabel
        public ActionResult Index()
        {
            initpage();
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.ToList().OrderByDescending(x=>x.JOINING_DATE);
                //// Only grid query values will be available here.
                //return PartialView("IndexGrid", memberinfo);

                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public ActionResult CreateMember(string memid = "")
        {
            //long userid = long.Parse(Session["UserId"].ToString());
            try
            {
                if (memid != "")
                {
                    
                    var dbcontext = new DBContext();

                    var model = new TBL_MASTER_MEMBER();
                    var memberrole = dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x=>x.ROLE_NAME== "WHITE LEVEL" || x.ROLE_NAME== "API USER").ToList();
                    ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                    ViewBag.checkstatus = "1";
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    //long Memid = long.Parse(decrptSlId);
                    long idval = long.Parse(decrptSlId);
                    model = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == idval);
                    return View(model);
                }
                else
                {
                    var dbcontext = new DBContext();
                    ViewBag.checkstatus = "0";
                    var memberrole = dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToList();
                    ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                    var user = new TBL_MASTER_MEMBER();
                    user.UName = "";
                    return View();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                return RedirectToAction("Notfound", "ErrorHandler");
            }
        }
        [HttpPost]
        public ActionResult CreateMember(TBL_MASTER_MEMBER value, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {                    
                    var CheckUser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == value.MEM_ID).FirstOrDefault();
                    if (CheckUser == null)
                    {
                        if (value.AADHAAR_NO != null || AadhaarFile != null)
                        {
                            if (AadhaarFile == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Aadhaar Card Image...");
                                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToList();
                                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                                return View("CreateMember", value);
                            }
                            else if (value.AADHAAR_NO == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please give Aadhaar Card Number...");
                                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToList();
                                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                                return View("CreateMember", value);
                            }
                        }
                        if (value.PAN_NO != null || PanFile != null)
                        {
                            if (PanFile == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Pan Card Image...");
                                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToList();
                                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                                return View("CreateMember", value);
                            }
                            else if (value.PAN_NO == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please give Pan Card Number...");
                                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "WHITE LEVEL" || x.ROLE_NAME == "API USER").ToList();
                                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                                return View("CreateMember", value);
                            }
                        }
                        value.BALANCE = 0;
                        value.UNDER_WHITE_LEVEL = 0;
                        value.INTRODUCER = 1;
                        //value.BLOCKED_BALANCE = 0;
                        value.ACTIVE_MEMBER = true;
                        if (value.BLOCKED_BALANCE == null)
                        {
                            value.BLOCKED_BALANCE = 0;
                        }
                        else
                        {
                            value.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                        }
                        value.IS_DELETED = false;
                        value.JOINING_DATE = System.DateTime.Now;
                        //value.CREATED_BY = long.Parse(Session["UserId"].ToString());
                        value.CREATED_BY = CurrentUser.USER_ID;
                        value.LAST_MODIFIED_DATE = System.DateTime.Now;
                        db.TBL_MASTER_MEMBER.Add(value);
                        db.SaveChanges();
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        //Checking file is available to save.  
                        if (AadhaarFile != null)
                        {
                            string aadharpath = Path.GetFileName(AadhaarFile.FileName);
                            string AadharfileName = aadharpath.Substring(aadharpath.LastIndexOf(((char)92)) + 1);
                            int index = AadharfileName.LastIndexOf('.');
                            string onyName = AadharfileName.Substring(0, index);
                            string fileExtension = AadharfileName.Substring(index + 1);

                            var AadhaarFileName = value.MEM_ID + "_" + value.AADHAAR_NO + "." + fileExtension;
                            //var AdharServerSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + AadhaarFileName);
                            var AdharServerSavePath = (Server.MapPath(@"/MemberFiles/") + AadhaarFileName);
                            aadhaarfilename = "/MemberFiles/" + AadhaarFileName;
                            AadhaarFile.SaveAs(AdharServerSavePath);
                        }
                        if (PanFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);
                            var InputPanCard = value.MEM_ID + "_" + value.PAN_NO + "." + PanfileExtension;
                            //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                            var PanserverSavePath = (Server.MapPath(@"/MemberFiles/") + InputPanCard);
                            Pancardfilename = "/MemberFiles/" + InputPanCard;
                            PanFile.SaveAs(PanserverSavePath);
                        }
                        var imageupload = db.TBL_MASTER_MEMBER.Find(value.MEM_ID);
                        imageupload.AADHAAR_FILE_NAME = aadhaarfilename;
                        imageupload.PAN_FILE_NAME = Pancardfilename;
                        db.Entry(imageupload).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var servlist = db.TBL_SETTINGS_SERVICES_MASTER.ToList();
                        foreach (var lst in servlist)
                        {
                            TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                            {
                                MEMBER_ID = long.Parse(value.MEM_ID.ToString()),
                                SERVICE_ID = long.Parse(lst.SLN.ToString()),
                                ACTIVE_SERVICE = false
                            };
                            db.TBL_WHITELABLE_SERVICE.Add(objser);
                            db.SaveChanges();
                        }
                        ViewBag.savemsg = "Data Save Successfully";
                        Session["msg"] = "Data Save Successfully";
                        //ContextTransaction.Commit();
                    }
                    else
                    {
                        ViewBag.checkstatus = "1";
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        //Checking file is available to save.  
                        if (AadhaarFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);


                            var AadhaarFileName = value.MEM_ID + "_" + value.AADHAAR_NO + "." + PanfileExtension;
                            //var AdharServerSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + AadhaarFileName);
                            var AdharServerSavePath = (Server.MapPath(@"/MemberFiles/") + AadhaarFileName);
                            aadhaarfilename = "/MemberFiles/" + AadhaarFileName;
                            AadhaarFile.SaveAs(AdharServerSavePath);
                            CheckUser.AADHAAR_FILE_NAME = aadhaarfilename;
                            CheckUser.AADHAAR_NO = value.AADHAAR_NO;
                        }
                        if (PanFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);

                            var InputPanCard = value.MEM_ID + "_" + value.PAN_NO + "." + PanfileExtension;
                            //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                            var PanserverSavePath = (Server.MapPath(@"/MemberFiles/") + InputPanCard);
                            Pancardfilename = "/MemberFiles/" + InputPanCard;
                            PanFile.SaveAs(PanserverSavePath);
                            CheckUser.PAN_FILE_NAME = Pancardfilename;
                            CheckUser.PAN_NO = value.PAN_NO;
                        }
                        
                        CheckUser.MEMBER_MOBILE = value.MEMBER_MOBILE;
                        //CheckUser.UNDER_WHITE_LEVEL = value.UNDER_WHITE_LEVEL;
                        CheckUser.MEMBER_MOBILE = value.MEMBER_MOBILE;
                        CheckUser.MEMBER_NAME = value.MEMBER_NAME;
                        CheckUser.COMPANY = value.COMPANY;
                        CheckUser.MEMBER_ROLE = value.MEMBER_ROLE;
                        //CheckUser.INTRODUCER = value.INTRODUCER;
                        CheckUser.ADDRESS = value.ADDRESS;
                        CheckUser.CITY = value.CITY;
                        CheckUser.PIN = value.PIN;
                        CheckUser.EMAIL_ID = value.EMAIL_ID;
                        CheckUser.SECURITY_PIN_MD5 = value.SECURITY_PIN_MD5;
                        CheckUser.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                        db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.savemsg = "Data Update Successfully";
                        Session["msg"] = "Data Update Successfully";
                    }
                    //throw new Exception();
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }
        // GET: APILabel/Create
        public ActionResult UpdateMember()
        {
            return View();
        }

        // POST: APILabel/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: APILabel/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: APILabel/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: APILabel/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: APILabel/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        //public JsonResult DeleteInformation(int id)
        public JsonResult DeleteInformation(string id)
        {
            var context = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = context.Database.BeginTransaction())
            {
                try
                {

                    string decrptSlId = Decrypt.DecryptMe(id);
                    long Memid = long.Parse(decrptSlId);
                    var membinfo = context.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Memid).FirstOrDefault();
                    membinfo.IS_DELETED = true;
                    context.Entry(membinfo).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }
            

        }
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    Exception exception = filterContext.Exception;
        //    //Logging the Exception
        //    filterContext.ExceptionHandled = true;


        //    var Result = this.View("Error", new HandleErrorInfo(exception,
        //        filterContext.RouteData.Values["controller"].ToString(),
        //        filterContext.RouteData.Values["action"].ToString()));

        //    filterContext.Result = Result;

        //}

        [HttpPost]
        public JsonResult MemberStatusUpdate(string id,string statusval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long memid = long.Parse(id);

                    bool memberstatus = false;
                    if (statusval == "True")
                    {
                        memberstatus = false;
                    }
                    else
                    {
                        memberstatus = true;
                    }
                    var memberlist = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memid).FirstOrDefault();
                    if (statusval == "True")
                    {
                        memberlist.ACTIVE_MEMBER = false;
                    }
                    else
                    {
                        memberlist.ACTIVE_MEMBER = true;
                    }
                    
                    memberlist.IS_DELETED = true;
                    //memberlist.UName = memberlist.UName;
                    //memberlist.PIN = memberlist.PIN;
                    //memberlist.User_pwd = memberlist.User_pwd;
                    //memberlist.BLOCKED_BALANCE = memberlist.BLOCKED_BALANCE;

                    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }
               
            
        }
        [HttpPost]
        public JsonResult PasswordSendtoUser(string id)
        {
            try
            {
                EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long memId = long.Parse(id);
                var meminfo = db.TBL_MASTER_MEMBER.Where(x=>x.MEM_ID==memId).FirstOrDefault();
                if (meminfo != null)
                {
                    //string decriptpass = Decrypt.DecryptMe(meminfo.User_pwd);
                    string password = meminfo.User_pwd;
                    string mailbody = "Hi " + meminfo.UName + ",<p>Your WHITE LEVEL LOGIN USER ID:- "+meminfo.EMAIL_ID+" and  PASSWORD IS:- " + password + "</p>";
                    emailhelper.SendUserEmail(meminfo.EMAIL_ID, "White Level Password", mailbody);
                }
                return Json(new { Result = "true" });
            }
            catch(Exception ex)
            {
                return Json(new { Result = "false" });
            }
            
        }


        [HttpGet]
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_MASTER_MEMBER> grid = CreateExportableGrid();
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
        //IGrid<TBL_MASTER_MEMBER> grid { get; set; }
        private IGrid<TBL_MASTER_MEMBER> CreateExportableGrid()
        {
            var dbcontext = new DBContext();
            //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false).ToList();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.ToList().OrderByDescending(x=>x.JOINING_DATE);
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.UName).Titled("UserName").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_NAME).Titled("MEMBER NAME").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("COMPANY").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("MOBILE").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BALANCE).Titled("BALANCE").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.BLOCKED_BALANCE).Titled("BLK BLNC").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                 .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a><a href='APILabel/CreateMember?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Edit</a><a href='Hosting/HostingDetails?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Hosting</a><a href='Service/ServiceDetails?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Service</a></div>");
   
            grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status").Css("<style>.table - hover tbody tr: hover {background - color:red;}</style> ").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
              .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='MemberStatus(\"" + model.MEM_ID + "\",\"" + model.ACTIVE_MEMBER + "\");'>" + (model.ACTIVE_MEMBER == true ? "Deactive" : "Active") + "</a>");
           
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;
           

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;

            //}
            //foreach (IGridColumn row in grid.Rows)
            //{
            //    row.CssClasses = "red";
                
            //}

            return grid;
        }


        [HttpPost]
        public JsonResult CheckEmailAvailability(string emailid)
        {
            var context = new DBContext();
            var User = context.TBL_MASTER_MEMBER.Where(model => model.EMAIL_ID == emailid).FirstOrDefault();
            if (User != null)
            {
                return Json(new { result = "unavailable" });
            }
            else
            {
                return Json(new { result = "available" });
            }
        }


        [AllowAnonymous]
        [HttpPost]       
        public JsonResult IsAlreadySigned(string EMAIL_ID)
        {
            return Json(IsUserAvailable(EMAIL_ID));
        }
        public bool IsUserAvailable(string EmailId)
        {
            // Assume these details coming from database  
            var db = new DBContext();
            var checkmail = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID == EmailId).FirstOrDefault();
            bool status;
            if (checkmail != null)
            {
                //Already registered  
                status = false;
            }
            else
            {
                //Available to use  
                status = true;
            }


            return status;
        }

        [AllowAnonymous]
        public ActionResult checkEmail(string EMAIL_ID)
        {
            if (string.IsNullOrEmpty(EMAIL_ID))
            {
                return Json("Please Enter email id.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var db = new DBContext();
                var emailcheck = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID.Contains(EMAIL_ID)).FirstOrDefault();
                if (emailcheck != null)
                {
                    return Json("Email id is available", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

        }

    }
}

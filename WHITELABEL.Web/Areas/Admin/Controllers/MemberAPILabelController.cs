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
    public class MemberAPILabelController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {                
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) ==false )
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

        //GET: APILabel
        //[Route("Index")]
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                
                //GetFormNumber();
                //System.Threading.Thread.Sleep(1000);
                //long code = (long)DateTime.UtcNow.Subtract(new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                //string uni = DateTime.Now.ToString("ddssf");
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
        public static string GetFormNumber()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            return FormNumber.ToString("X");

        }
        public PartialViewResult IndexGrid(string SearchVal="")
        {
            try
            {
                var dbcontext = new DBContext();
                if (SearchVal != "")
                {
                    //var Railmemberinfo = dbcontext.TBL_RAIL_AGENT_INFORMATION.Where(x => x.RAIL_USER_ID.StartsWith(SearchVal) || x.TRAVEL_AGENT_NAME.StartsWith(SearchVal) || x.AGENCY_NAME.StartsWith(SearchVal) || x.OFFICE_ADDRESS.StartsWith(SearchVal) || x.RESIDENCE_ADDRESS.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.MOBILE_NO.StartsWith(SearchVal) || x.OFFICE_PHONE.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.DIGITAL_CERTIFICATE_DETAILS.StartsWith(SearchVal)).ToList();
                    var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.UName.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.COMPANY.StartsWith(SearchVal) || x.COMPANY_GST_NO.StartsWith(SearchVal) || x.ADDRESS.StartsWith(SearchVal) || x.CITY.StartsWith(SearchVal) || x.PIN.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.AADHAAR_NO.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.RAIL_ID.StartsWith(SearchVal) || x.FACEBOOK_ID.StartsWith(SearchVal) || x.WEBSITE_NAME.StartsWith(SearchVal) || x.MEM_UNIQUE_ID.StartsWith(SearchVal)).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                else
                {
                    var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[Route("WhiteLevel/Registration")]
        public async Task<ActionResult> CreateMember(string memid = "")
        {
            initpage();////
            if (Session["WhiteLevelUserId"] != null)
            {
                var dbcontext = new DBContext();
                try
                {
                    var StateName = dbcontext.TBL_STATES.ToList();
                    ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
                    if (memid != "")
                    {
                    
                        var model = new TBL_MASTER_MEMBER();
                        //var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER").ToListAsync();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        ViewBag.checkstatus = "1";
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        //long Memid = long.Parse(decrptSlId);
                        long idval = long.Parse(decrptSlId);
                        ViewBag.checkmail = true;
                        model = await dbcontext.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == idval);
                        model.BLOCKED_BALANCE = Math.Round(Convert.ToDecimal(model.BLOCKED_BALANCE), 0); Session.Remove("msg");
                        Session.Remove("msg");
                        Session["msg"] = null;
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        return View(model);
                        //return View("CreateMember", "MemberAPILabel", new {area ="Admin" },model);
                    }
                    else
                    {
                        var model = new TBL_MASTER_MEMBER();
                        string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                        string UniqId = "BMT" + GetUniqueNo;
                        ViewBag.checkstatus = "0";
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "DISTRIBUTOR").ToListAsync();
                        //var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "SUPER DISTRIBUTOR" ).ToListAsync();//DISTRIBUTOR
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        var user = new TBL_MASTER_MEMBER();
                        ViewBag.checkmail = false;
                        user.UName = "";
                        Session.Remove("msg");
                        Session["msg"] = null;
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        //ModelState.Clear();
                        model.UName = UniqId;
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    //throw ex;
                    Logger.Error("Controller:-  MemberAPILabel(Admin), method:- CreateMember (GET) Line No:- 140", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    //return RedirectToAction("Notfound", "ErrorHandler");
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
        [ValidateInput(true)]
        public async Task<ActionResult> CreateMember(TBL_MASTER_MEMBER value, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        {
            initpage();////
            var db = new DBContext();
            //EmailClassHelper objemail = new EmailClassHelper();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var Poweradmin = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
                    var CheckUser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == value.MEM_ID).FirstOrDefaultAsync();
                    if (CheckUser == null)
                    {
                        //string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                        //string UniqId = "TIQ" + GetUniqueNo;
                        if (AadhaarFile != null)
                        //if (value.AADHAAR_NO != null || AadhaarFile != null)
                        {
                            if (AadhaarFile == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Aadhaar Card Image...");
                                return View("CreateMember", value);
                            }
                            else if (value.AADHAAR_NO == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please give Aadhaar Card Number...");
                                return View("CreateMember", value);
                            }
                        }
                        //if (value.PAN_NO != null || PanFile != null)
                        if (PanFile != null)
                        {
                            if (PanFile == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Pan Card Image...");
                                return View("CreateMember", value);
                            }
                            else if (value.PAN_NO == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please give Pan Card Number...");
                                return View("CreateMember", value);
                            }

                        }
                        value.BALANCE = 0;
                        decimal AmountVal = 0;
                        if (value.BLOCKED_BALANCE == null)
                        {
                            value.BLOCKED_BALANCE = 0;
                            value.BALANCE = 0;
                            AmountVal = 0;
                        }
                        else
                        {
                            value.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                            value.BALANCE = value.BLOCKED_BALANCE;
                            AmountVal = (decimal)value.BLOCKED_BALANCE;
                        }
                        string GetUniqueNo = String.Format("{0:d5}", (DateTime.Now.Ticks / 10) % 10000);
                        string UniqId = "BMT" + GetUniqueNo;
                        value.UName = UniqId;
                        value.MEM_UNIQUE_ID = UniqId;
                        value.AADHAAR_NO = value.AADHAAR_NO;
                        value.PAN_NO = value.PAN_NO;
                        value.EMAIL_ID = value.EMAIL_ID.ToLower();
                        value.UNDER_WHITE_LEVEL = MemberCurrentUser.MEM_ID;
                        value.INTRODUCER = MemberCurrentUser.MEM_ID;
                        //value.BLOCKED_BALANCE = 0;
                        value.ACTIVE_MEMBER = true;
                        value.IS_DELETED = false;
                        value.JOINING_DATE = System.DateTime.Now;
                        //value.CREATED_BY = long.Parse(Session["UserId"].ToString());
                        value.CREATED_BY = MemberCurrentUser.MEM_ID;
                        //value.CREATED_BY = CurrentUser.USER_ID;
                        value.LAST_MODIFIED_DATE = System.DateTime.Now;
                        if (value.GST_FLAG != null)
                        {
                            value.GST_MODE = 1;
                        }
                        else
                        {
                            value.GST_MODE = 0;
                        }
                        if (value.TDS_FLAG != null)
                        {
                            value.TDS_MODE = 1;
                        }
                        else
                        {
                            value.TDS_MODE = 0;
                        }
                        //value.GST_MODE = 1;
                        //value.TDS_MODE = 1;
                        value.DUE_CREDIT_BALANCE = 0;
                        value.CREDIT_BALANCE = 0;
                        value.IS_TRAN_START = true;
                        //value.MEM_UNIQUE_ID = value.UName;
                        db.TBL_MASTER_MEMBER.Add(value);
                        await db.SaveChangesAsync();
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
                        await db.SaveChangesAsync();
                        var servlist = await db.TBL_SETTINGS_SERVICES_MASTER.ToListAsync();
                        foreach (var lst in servlist)
                        {
                            TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                            {
                                MEMBER_ID = long.Parse(value.MEM_ID.ToString()),
                                SERVICE_ID = long.Parse(lst.SLN.ToString()),
                                ACTIVE_SERVICE = false
                            };
                            db.TBL_WHITELABLE_SERVICE.Add(objser);
                            await db.SaveChangesAsync();
                        }
                        TBL_ACCOUNTS MemberObj = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = long.Parse(value.MEM_ID.ToString()),
                            MEMBER_TYPE = "DISTRIBUTOR",
                            TRANSACTION_TYPE = "ADD DISTRIBUTOR",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                            AMOUNT = AmountVal,
                            NARRATION = "Add Distributor",
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
                        TBL_FLIGHT_MARKUP objflight = new TBL_FLIGHT_MARKUP()
                        {
                            MEM_ID = long.Parse(value.MEM_ID.ToString()),
                            ASSIGN_BY = 0,
                            INTERNATIONAL_MARKUP = 0,
                            DOMESTIC_MARKUP = 0,
                            ASSIGN_DATE = DateTime.Now,
                            STATUS = 0,
                            ASSIGN_TYPE = "MARK UP ASSIGN",
                            DIST_ID= MemberCurrentUser.MEM_ID
                    };
                        db.TBL_FLIGHT_MARKUP.Add(objflight);
                        
                        await db.SaveChangesAsync();                        
                        ViewBag.savemsg = "Data Saved Successfully";

                        #region Email Code done by sayan at 10-10-2020

                        string name = value.MEMBER_NAME;
                        string sub = "Welcome to Boom Travels.";
                        string usermsgdesc = "Dear <b>" + value.MEMBER_NAME + "</b> You have successfully joined in Boom Travels.<br /><p>Your User Id:- " + UniqId + " <br/>Password:- " + value.User_pwd + " </p> "+"<br /> Regards, <br/>< br />BOOM Travels";
                        EmailHelper emailhelper = new EmailHelper();
                        string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                        emailhelper.SendUserEmail(value.EMAIL_ID, sub, usermsgbody);
                        #endregion
                        Session["msg"] = "Data Saved Successfully";
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
                        }
                        CheckUser.STATE_ID = value.STATE_ID;
                        //CheckUser.UName = value.UName;
                        CheckUser.PAN_NO = value.PAN_NO;
                        CheckUser.AADHAAR_NO = value.AADHAAR_NO;
                        CheckUser.FACEBOOK_ID = value.FACEBOOK_ID;
                        CheckUser.WEBSITE_NAME = value.WEBSITE_NAME;
                        CheckUser.NOTES = value.NOTES; CheckUser.NOTES = value.NOTES;
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
                        //CheckUser.EMAIL_ID = value.EMAIL_ID;
                        CheckUser.SECURITY_PIN_MD5 = value.SECURITY_PIN_MD5;                      
                        //CheckUser.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                        //CheckUser.BALANCE = value.BLOCKED_BALANCE;
                        CheckUser.DUE_CREDIT_BALANCE = 0;
                        CheckUser.CREDIT_BALANCE = 0;
                        if (CheckUser.GST_FLAG != null)
                        {
                            CheckUser.GST_MODE = 1;
                        }
                        else
                        {
                            CheckUser.GST_MODE = 0;
                        }
                        if (CheckUser.TDS_FLAG != null)
                        {
                            CheckUser.TDS_MODE = 1;
                        }
                        else
                        {
                            CheckUser.TDS_MODE = 0;
                        }
                        CheckUser.IS_TRAN_START = true;
                        CheckUser.OPTIONAL_EMAIL_ID = value.OPTIONAL_EMAIL_ID;
                        CheckUser.SEC_OPTIONAL_EMAIL_ID = value.SEC_OPTIONAL_EMAIL_ID;
                        CheckUser.OPTIONAL_MOBILE_NO = value.OPTIONAL_MOBILE_NO;
                        CheckUser.SEC_OPTIONAL_MOBILE_NO = value.SEC_OPTIONAL_MOBILE_NO;
                        CheckUser.OLD_MEMBER_ID = value.OLD_MEMBER_ID;
                        db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        //var BalanceUpdate = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == value.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        //if (BalanceUpdate != null)
                        //{
                        //    decimal closing = 0;
                        //    decimal AddBalance = 0;
                        //    decimal.TryParse(BalanceUpdate.CLOSING.ToString(), out closing);
                        //    decimal Updated_Balance = (decimal)value.BLOCKED_BALANCE;
                        //    AddBalance = closing + Updated_Balance;
                        //    TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = long.Parse(value.MEM_ID.ToString()),
                        //        MEMBER_TYPE = "DISTRIBUTOR",
                        //        TRANSACTION_TYPE = "ADD DISTRIBUTOR",
                        //        TRANSACTION_DATE = DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                        //        AMOUNT = Updated_Balance,
                        //        NARRATION = "Add Distributor",
                        //        OPENING = closing,
                        //        CLOSING = AddBalance,
                        //        REC_NO = 0,
                        //        COMM_AMT = 0,
                        //        TDS = 0,
                        //        GST = 0,
                        //        IPAddress = "",
                        //        SERVICE_ID = 0,
                        //        CORELATIONID = ""
                        //    };
                        //    db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);

                        //}
                        ViewBag.savemsg = "Data Updated Successfully";
                        Session["msg"] = "Data Updated Successfully";

                        //#region Email Code done by sayan at 10-10-2020
                        //string msgdesc = "Dear <b>"+CheckUser.MEM_UNIQUE_ID + "("+CheckUser.MEMBER_NAME+")</b>, Your Profile is updated.";
                        //string usersub = "Your profile is updated";                        
                        //EmailHelper emailhelper = new EmailHelper();
                        //string msgbody = emailhelper.GetEmailTemplate(CheckUser.MEMBER_NAME, msgdesc, "UserEmailTemplate.html");                        
                        //emailhelper.SendUserEmail(CheckUser.EMAIL_ID.Trim(), usersub, msgbody);
                        //#endregion
                        //string Regmsg = "Hi "+ CheckUser.MEM_UNIQUE_ID + " \r\n. Your profile information is updated successfully.\r\n Regards\r\n BOOM Travels";
                        //objsms.SendUserEmail(value.EMAIL_ID, "Your Profile is Updated.", Regmsg);
                    }
                    //throw new Exception();
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberAPILabel(Admin), method:- CreateMember (POST) Line No:- 340", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
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
        [ValidateAntiForgeryToken]
        //public JsonResult DeleteInformation(int id)
        public async Task<JsonResult> DeleteInformation(string id)
        {
            initpage();////
            var context = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = context.Database.BeginTransaction())
            {
                try
                {

                    string decrptSlId = Decrypt.DecryptMe(id);
                    long Memid = long.Parse(decrptSlId);
                    var membinfo = await context.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Memid).FirstOrDefaultAsync();
                    membinfo.IS_DELETED = true;
                    context.Entry(membinfo).State = System.Data.Entity.EntityState.Modified;
                    await context.SaveChangesAsync();
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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MemberStatusUpdate(string id, string statusval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long memid = long.Parse(id);
                    string MsgBody = string.Empty;
                    bool memberstatus = false;
                    if (statusval == "True")
                    {
                        memberstatus = false;
                        MsgBody = "Your Boom Travels profile is deactivated";
                    }
                    else
                    {
                        memberstatus = true;
                        MsgBody = "Your Boom Travels profile is activated";
                    }
                    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memid).FirstOrDefaultAsync();
                    if (statusval == "True")
                    {
                        memberlist.ACTIVE_MEMBER = false;
                    }
                    else
                    {
                        memberlist.ACTIVE_MEMBER = true;
                    }

                    memberlist.IS_DELETED = true;

                    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    ContextTransaction.Commit();
                    //#region Email Code done by sayan at 10-10-2020
                    //string name = memberlist.MEMBER_NAME;
                    //string sub = MsgBody;
                    //string usermsgdesc = "Dear <b>" + memberlist.MEMBER_NAME + "</b> " + MsgBody + " by Admin. For any query please contact your Admin.";
                    //EmailHelper emailhelper = new EmailHelper();
                    //string usermsgbody = emailhelper.GetEmailTemplate(name, usermsgdesc, "UserEmailTemplate.html");
                    //emailhelper.SendUserEmail(memberlist.EMAIL_ID.Trim(), sub, usermsgbody);
                    //#endregion

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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PasswordSendtoUser(string id)
        {
            try
            {
                initpage();////
                //EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long memId = long.Parse(id);
                var meminfo = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memId).FirstOrDefaultAsync();
                if (meminfo != null)
                {
                    #region Email Code done by sayan at 13-10-2020
                    string name = meminfo.MEMBER_NAME;
                    string password = meminfo.User_pwd;
                    string Regmsg = "Hi <b>" + meminfo.UName + " " + "(" + meminfo.MEMBER_NAME + ")" + "</b>. Your Admin has been sent your login credentials. Your Boom Travels Login USER ID:- <b>" + meminfo.MEM_UNIQUE_ID + "</b> and  PASSWORD is:- <b>" + password + "</b>.<br /><br/> Regards, <br/><br/>Boom Travels.";
                    EmailHelper emailhelper = new EmailHelper();
                    string usermsgbody = emailhelper.GetEmailTemplate(name, Regmsg, "UserEmailTemplate.html");
                    emailhelper.SendUserEmail(meminfo.EMAIL_ID.Trim(), "You Have Received Your Boom Travels User Id & Password!", usermsgbody);
                    #endregion
                    ////string decriptpass = Decrypt.DecryptMe(meminfo.User_pwd);
                    //string password = meminfo.User_pwd;
                    //string mailbody = "Hi " + meminfo.UName + ",<p>Your WHITE LABEL LOGIN USER ID:- " + meminfo.EMAIL_ID + " and  PASSWORD IS:- " + password + "</p>";
                    //emailhelper.SendUserEmail(meminfo.EMAIL_ID, "White Label Password", mailbody);
                }
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "false" });
            }

        }


        [HttpGet]
        public FileResult ExportIndex(string Super="0", string SearchVal = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_MASTER_MEMBER> grid = CreateExportableGrid(SearchVal);
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

        private IGrid<TBL_MASTER_MEMBER> CreateExportableGrid(string SearchVal )
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false && x.CREATED_BY== userid).ToList();
            if (SearchVal != "")
            {
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.UName.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.COMPANY.StartsWith(SearchVal) || x.COMPANY_GST_NO.StartsWith(SearchVal) || x.ADDRESS.StartsWith(SearchVal) || x.CITY.StartsWith(SearchVal) || x.PIN.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.AADHAAR_NO.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.RAIL_ID.StartsWith(SearchVal) || x.FACEBOOK_ID.StartsWith(SearchVal) || x.WEBSITE_NAME.StartsWith(SearchVal) || x.MEM_UNIQUE_ID.StartsWith(SearchVal)).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Agent ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Agent Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Agency Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RESERVED_CREDIT_LIMIT).Titled("PCL").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Limit").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CITY).Titled("City").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.SECURITY_PIN_MD5).Titled("M Pin").Filterable(true).Sortable(true);
                grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
                //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a>&nbsp;<a href='" + @Url.Action("CreateMember", "MemberAPILabel", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Edit</a><a href='" + @Url.Action("ServiceDetails", "MemberService", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Service</a></div>");
                //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //  .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='MemberStatus(\"" + model.MEM_ID + "\",\"" + model.ACTIVE_MEMBER + "\");return 0;'>" + (model.ACTIVE_MEMBER == true ? "Deactive" : "Active") + "</a>");
                grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 10000000;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            else
            {
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == userid).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Agent ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Agent Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Agency Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RESERVED_CREDIT_LIMIT).Titled("PCL").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Limit").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CITY).Titled("City").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.SECURITY_PIN_MD5).Titled("M Pin").Filterable(true).Sortable(true);
                grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
                //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a>&nbsp;<a href='" + @Url.Action("CreateMember", "MemberAPILabel", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Edit</a><a href='" + @Url.Action("ServiceDetails", "MemberService", new { area = "Admin", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Service</a></div>");
                //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //  .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='MemberStatus(\"" + model.MEM_ID + "\",\"" + model.ACTIVE_MEMBER + "\");return 0;'>" + (model.ACTIVE_MEMBER == true ? "Deactive" : "Active") + "</a>");
                grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 10000000;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
          
        }

        [HttpPost]
        public async Task<JsonResult> CheckEmailAvailability(string emailid)
        {
            initpage();////
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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMemberPassword(string id)
        {
            initpage();////
            try
            {
                EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long memId = long.Parse(id);
                var meminfo = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memId).FirstOrDefaultAsync();
                if (meminfo != null)
                {
                    //string decriptpass = Decrypt.DecryptMe(meminfo.User_pwd);
                    string password = meminfo.User_pwd;
                    
                }
                return Json(meminfo,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }


        public ActionResult UploadLogo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadLogo( HttpPostedFileBase LogoUpload)
        {
            if (LogoUpload != null)
            {
                var db = new DBContext();
                string Logofilename = string.Empty;
                string Pannopath = Path.GetFileName(LogoUpload.FileName);
                string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                int index = PannofileName.LastIndexOf('.');
                string onyName = PannofileName.Substring(0, index);
                string PanfileExtension = PannofileName.Substring(index + 1);
                //var InputPanCard = value.MEM_ID + "_" + value.PAN_NO + "." + PanfileExtension;
                var InputPanCard = MemberCurrentUser.MEM_ID + "AdminLogo" + "." + PanfileExtension;
                var PanserverSavePath = (Server.MapPath(@"~/WhiteLabelLogo/") + InputPanCard);
                Logofilename = "~/WhiteLabelLogo/" + InputPanCard;
                LogoUpload.SaveAs(PanserverSavePath);
                var UpdateLogo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                if (UpdateLogo != null)
                {
                    UpdateLogo.LOGO_STYLE = "height:100px; width:200px;";
                    UpdateLogo.LOGO = Logofilename;
                    db.Entry(UpdateLogo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    Session["msg"] = "Logo Update Successfully";
                }
            }
            else
            {
                Session["msg"] = "Please Upload Logo";
            }
            return View("UploadLogo");
        }

        public ActionResult GetAllMerchantInformation()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();                   
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                             //where SuperMemId.Contains(x.INTRODUCER.ToString())
                                         where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                         select new
                                         {
                                             MEM_ID = x.MEM_ID,
                                             UName = x.UName,
                                             MobileNo=x.MEMBER_MOBILE
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName+"-"+z.MobileNo
                                         }).ToList().Distinct();
                    ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
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

        public PartialViewResult AllMerchantInformationGrid(string SearchVal = "")
        {
            try
            {
                var db = new DBContext();
                if (SearchVal != "")
                {
                    long DistId = 0;
                    //long.TryParse(DistributorId, out DistId);
                    //var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == DistId && x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5).OrderByDescending(x => x.JOINING_DATE).ToList();
                    var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.ACTIVE_MEMBER == true && x.MEMBER_ROLE==5 && (x.UName.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.COMPANY.StartsWith(SearchVal) || x.COMPANY_GST_NO.StartsWith(SearchVal) || x.ADDRESS.StartsWith(SearchVal) || x.CITY.StartsWith(SearchVal) || x.PIN.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.AADHAAR_NO.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.RAIL_ID.StartsWith(SearchVal) || x.FACEBOOK_ID.StartsWith(SearchVal) || x.WEBSITE_NAME.StartsWith(SearchVal) || x.MEM_UNIQUE_ID.StartsWith(SearchVal))).ToList();
                    return PartialView("AllMerchantInformationGrid", MerchantList);
                }
                else
                {
                    var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER==true).OrderByDescending(x=>x.JOINING_DATE).ToList();
                    return PartialView("AllMerchantInformationGrid", MerchantList);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public FileResult ExportMerchantInfoIndex(string MEM_ID="")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                //long mem_Id = 0;
                //long.TryParse(  MEM_ID,out mem_Id);
                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_MASTER_MEMBER> grid = CreateExporMerchantInfotableGrid(MEM_ID);
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

        private IGrid<TBL_MASTER_MEMBER> CreateExporMerchantInfotableGrid(string MEM_ID)
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            if (MEM_ID != "")
            {
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.UName.StartsWith(MEM_ID) || x.MEMBER_MOBILE.StartsWith(MEM_ID) || x.MEMBER_NAME.StartsWith(MEM_ID) || x.COMPANY.StartsWith(MEM_ID) || x.COMPANY_GST_NO.StartsWith(MEM_ID) || x.ADDRESS.StartsWith(MEM_ID) || x.CITY.StartsWith(MEM_ID) || x.PIN.StartsWith(MEM_ID) || x.EMAIL_ID.StartsWith(MEM_ID) || x.AADHAAR_NO.StartsWith(MEM_ID) || x.PAN_NO.StartsWith(MEM_ID) || x.RAIL_ID.StartsWith(MEM_ID) || x.FACEBOOK_ID.StartsWith(MEM_ID) || x.WEBSITE_NAME.StartsWith(MEM_ID) || x.MEM_UNIQUE_ID.StartsWith(MEM_ID)).ToList();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == Mem_ID).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Agent ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Agent Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Agency Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RESERVED_CREDIT_LIMIT).Titled("PCL").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Limit").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CITY).Titled("City").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.SECURITY_PIN_MD5).Titled("M Pin").Filterable(true).Sortable(true);
                grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
                grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 1000000000;
                //grid.Pager.RowsPerPage = 6;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            else
            {//var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false && x.CREATED_BY== userid).ToList();

                string[] DistributorMemId = dbcontext.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == userid).Select(a => a.MEM_ID.ToString()).ToArray();                
                string[] MerchantuserId = dbcontext.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var memberinfo = (from x in dbcontext.TBL_MASTER_MEMBER
                                where MerchantuserId.Contains(x.MEM_ID.ToString()) select x).ToList();

                var MEm_Info = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == userid).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Agent ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Agent Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Agency Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RESERVED_CREDIT_LIMIT).Titled("PCL").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Limit").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CITY).Titled("City").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.SECURITY_PIN_MD5).Titled("M Pin").Filterable(true).Sortable(true);
                grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
                grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 1000000000;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            
            
        }

        public ActionResult EditMerchant(string memid = "")
        {
            var dbcontext = new DBContext();
            var StateName = dbcontext.TBL_STATES.ToList();
            ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var model = new TBL_MASTER_MEMBER();
                var memberrole = dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "RETAILER").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");                
                string decrptSlId = Decrypt.DecryptMe(memid);
                //long Memid = long.Parse(decrptSlId);
                long idval = long.Parse(decrptSlId);
                model = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == idval);
                var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                return View(model);
                
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
        //public async Task<JsonResult> POSTADDMerchantDetails(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> UpdateMerchantInformation(TBL_MASTER_MEMBER objdetails)
        {
            var db = new DBContext();
            var CheckUser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == objdetails.MEM_ID).FirstOrDefaultAsync();
            if (CheckUser != null)
            {
                CheckUser.STATE_ID = objdetails.STATE_ID;
                CheckUser.FACEBOOK_ID = objdetails.FACEBOOK_ID;
                CheckUser.WEBSITE_NAME = objdetails.WEBSITE_NAME;
                CheckUser.NOTES = objdetails.NOTES;
                CheckUser.UNDER_WHITE_LEVEL = MemberCurrentUser.MEM_ID;
                CheckUser.INTRODUCER = CheckUser.INTRODUCER;
                CheckUser.AADHAAR_NO = objdetails.AADHAAR_NO;
                CheckUser.PAN_NO = objdetails.PAN_NO;
                
                //CheckUser.UNDER_WHITE_LEVEL = value.UNDER_WHITE_LEVEL;
                CheckUser.MEMBER_MOBILE = objdetails.MEMBER_MOBILE;
                CheckUser.MEMBER_NAME = objdetails.MEMBER_NAME;
                CheckUser.COMPANY = objdetails.COMPANY;
                //CheckUser.MEMBER_ROLE = value.MEMBER_ROLE;
                //CheckUser.INTRODUCER = value.INTRODUCER;
                CheckUser.ADDRESS = objdetails.ADDRESS;
                CheckUser.CITY = objdetails.CITY;
                CheckUser.PIN = objdetails.PIN;
                if (CheckUser.GST_FLAG != null)
                {
                    CheckUser.GST_MODE = 1;
                }
                else
                {
                    CheckUser.GST_MODE = 0;
                }
                if (CheckUser.TDS_FLAG != null)
                {
                    CheckUser.TDS_MODE = 1;
                }
                else
                {
                    CheckUser.TDS_MODE = 0;
                }
                //CheckUser.EMAIL_ID = value.EMAIL_ID;
                CheckUser.SECURITY_PIN_MD5 = objdetails.SECURITY_PIN_MD5;
                CheckUser.BLOCKED_BALANCE = objdetails.BLOCKED_BALANCE;
                CheckUser.DUE_CREDIT_BALANCE = 0;
                CheckUser.CREDIT_BALANCE = 0;
                CheckUser.IS_TRAN_START = true;
                CheckUser.OPTIONAL_EMAIL_ID = objdetails.OPTIONAL_EMAIL_ID;
                CheckUser.SEC_OPTIONAL_EMAIL_ID = objdetails.SEC_OPTIONAL_EMAIL_ID;
                CheckUser.OPTIONAL_MOBILE_NO = objdetails.OPTIONAL_MOBILE_NO;
                CheckUser.SEC_OPTIONAL_MOBILE_NO = objdetails.SEC_OPTIONAL_MOBILE_NO;
                CheckUser.OLD_MEMBER_ID = objdetails.OLD_MEMBER_ID;
                db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();

                //#region Email Code done by sayan at 10-10-2020
                //string name = CheckUser.MEMBER_NAME;
                //string regmsg = "Hi " + CheckUser.MEM_UNIQUE_ID + "("+ CheckUser.MEMBER_NAME +")" + " \r\n. Your profile information is updated successfully.\r\n Regards\r\n BOOM Travels";
                //EmailHelper emailhelper = new EmailHelper();
                //string usermsgbody = emailhelper.GetEmailTemplate(name, regmsg, "UserEmailTemplate.html");
                //emailhelper.SendUserEmail(objdetails.EMAIL_ID.Trim(), "Your Profile is Updated.", usermsgbody);
                //#endregion

                return Json("Profile update successfully ", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Please try again later", JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}
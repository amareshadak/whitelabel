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
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberRailAgentInformationController : AdminBaseController
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/MemberRailAgentInformation
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
        public PartialViewResult IndexGrid(string SearchVal = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (SearchVal != "")
                {
                    var Railmemberinfo = dbcontext.TBL_RAIL_AGENT_INFORMATION.Where(x => x.RAIL_USER_ID.StartsWith(SearchVal) || x.TRAVEL_AGENT_NAME.StartsWith(SearchVal) || x.AGENCY_NAME.StartsWith(SearchVal) || x.OFFICE_ADDRESS.StartsWith(SearchVal) || x.RESIDENCE_ADDRESS.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.MOBILE_NO.StartsWith(SearchVal) || x.OFFICE_PHONE.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.DIGITAL_CERTIFICATE_DETAILS.StartsWith(SearchVal)).ToList();
                    return PartialView("IndexGrid", Railmemberinfo);
                }
                else
                {

                    var Railmemberinfo = dbcontext.TBL_RAIL_AGENT_INFORMATION.ToList();
                    return PartialView("IndexGrid", Railmemberinfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ActionResult> GetRailwayAgentInfo(string memid = "")
        {
            var dbcontext = new DBContext();
            var StateName = await dbcontext.TBL_STATES.ToListAsync();
            ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
            if (Session["WhiteLevelUserId"] != null)
            {
                if (memid != "")
                {
                    long Sln = 0;
                    string decriptsln = Decrypt.DecryptMe(memid);
                    long.TryParse(decriptsln, out Sln);
                    long State_Id = 0;
                    var model = new RailAgentViewModel();
                    var memberrole = await dbcontext.TBL_RAIL_AGENT_INFORMATION.Where(x => x.SLN == Sln).FirstOrDefaultAsync();
                    long.TryParse(memberrole.STATE_ID.ToString(), out State_Id);
                    model.SLN = memberrole.SLN;
                    model.MEM_ID = memberrole.MEM_ID;
                    model.TRAVEL_AGENT_NAME = memberrole.TRAVEL_AGENT_NAME;
                    model.AGENCY_NAME = memberrole.AGENCY_NAME;
                    model.OFFICE_ADDRESS = memberrole.OFFICE_ADDRESS;
                    model.RESIDENCE_ADDRESS = memberrole.RESIDENCE_ADDRESS;
                    model.EMAIL_ID = memberrole.EMAIL_ID;
                    model.MOBILE_NO = memberrole.MOBILE_NO;
                    model.OFFICE_PHONE = memberrole.OFFICE_PHONE;
                    model.PAN_NO = memberrole.PAN_NO;
                    model.DIGITAL_CERTIFICATE_DETAILS = memberrole.DIGITAL_CERTIFICATE_DETAILS;
                    model.CERTIFICATE_BEGIN_DATE = memberrole.CERTIFICATE_BEGIN_DATE;
                    model.CERTIFICATE_END_DATE = memberrole.CERTIFICATE_END_DATE;
                    model.USER_STATE = memberrole.USER_STATE;
                    model.AGENT_VERIFIED_STATUS = memberrole.AGENT_VERIFIED_STATUS;
                    model.DEACTIVATION_REASON = memberrole.DEACTIVATION_REASON;
                    model.AGENT_STATE_ID = State_Id;
                    model.AADHAAR_VERIFICATION_STATUS = memberrole.AADHAAR_VERIFICATION_STATUS;
                    return View(model);
                }
                else
                {
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
        //public async Task<JsonResult> POSTUpdateRailAgentDetails(RailAgentViewModel objRailAgent)
        public async Task<ActionResult> POSTUpdateRailAgentDetails(RailAgentViewModel objRailAgent, HttpPostedFileBase DSCFileUpload)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var GEtRailInfo = await db.TBL_RAIL_AGENT_INFORMATION.Where(x => x.SLN == objRailAgent.SLN).FirstOrDefaultAsync();
                    if (GEtRailInfo != null)
                    {
                        GEtRailInfo.TRAVEL_AGENT_NAME = objRailAgent.TRAVEL_AGENT_NAME;
                        GEtRailInfo.AGENCY_NAME = objRailAgent.AGENCY_NAME;
                        GEtRailInfo.OFFICE_ADDRESS = objRailAgent.OFFICE_ADDRESS;
                        GEtRailInfo.RESIDENCE_ADDRESS = objRailAgent.RESIDENCE_ADDRESS;
                        GEtRailInfo.EMAIL_ID = objRailAgent.EMAIL_ID;
                        GEtRailInfo.MOBILE_NO = objRailAgent.MOBILE_NO;
                        GEtRailInfo.OFFICE_PHONE = objRailAgent.OFFICE_PHONE;
                        GEtRailInfo.PAN_NO = objRailAgent.PAN_NO;
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
        public JsonResult GetRAILAGENTInfo(string TransId, string Mem_ID)
        {
            try
            {
                long valueid = long.Parse(TransId);
                long MemId = long.Parse(Mem_ID);
                var db = new DBContext();
                var GetCommVal = db.TBL_RAIL_AGENTS_COMMISSION.FirstOrDefault(x => x.MEM_ID == MemId);
                if (GetCommVal == null)
                {
                    var RailAgentInfo = (from x in db.TBL_MASTER_MEMBER
                                         join h in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.UNDER_WHITE_LEVEL equals h.MEM_ID
                                         where x.MEM_ID == MemId
                                         select new
                                         {
                                             RailIdTable = valueid,
                                             MEM_ID = x.MEM_ID,
                                             DomainName = h.DOMAIN,
                                             WLP_ID_val = x.UNDER_WHITE_LEVEL,
                                             DIST_ID = x.INTRODUCER,
                                             WLP_NAME = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.UNDER_WHITE_LEVEL).UName,
                                             DIST_NAME = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.INTRODUCER).UName,
                                             DIST_NAMEMobile = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.INTRODUCER).MEMBER_MOBILE,
                                             RailAgentId = x.RAIL_ID,
                                             sln = 0,
                                             PG_ChargesApply = "0",
                                             Addntional_ChargesApply = "0",
                                         }).AsEnumerable().Select(z => new TBL_RAIL_AGENTS_COMMISSION
                                         {
                                             MEM_ID = z.MEM_ID,
                                             WLP_ID = (long)z.WLP_ID_val,
                                             DIST_ID = (long)z.DIST_ID,
                                             WLP_NAME = z.DomainName + "-" + z.WLP_NAME,
                                             DIST_NAME = z.DIST_NAME + "-" + z.DIST_NAMEMobile,
                                             RAIL_AGENT_ID = z.RailAgentId,
                                             PG_MAX_VALUE = 0,
                                             PG_EQUAL_LESS_2000 = 0,
                                             PG_EQUAL_GREATER_2000 = 0,
                                             PG_GST_STATUS = "No",
                                             ADDITIONAL_CHARGE_MAX_VAL = 0,
                                             ADDITIONAL_CHARGE_AC = 0,
                                             ADDITIONAL_CHARGE_NON_AC = 0,
                                             ADDITIONAL_GST_STATUS = "No",
                                             Rail_table_Id = z.RailIdTable,
                                             SLN = z.sln,
                                             PG_Charges_Apply_Val = z.PG_ChargesApply,
                                             Additional_Charges_Apply_Val = z.Addntional_ChargesApply

                                         }).FirstOrDefault();
                    return Json(RailAgentInfo, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var RailAgentInfo = (from x in db.TBL_MASTER_MEMBER
                                         join h in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.UNDER_WHITE_LEVEL equals h.MEM_ID
                                         join y in db.TBL_RAIL_AGENTS_COMMISSION on x.MEM_ID equals y.MEM_ID
                                         where y.MEM_ID == MemId
                                         select new
                                         {
                                             RailIdTable = valueid,
                                             MEM_ID = x.MEM_ID,
                                             DomainName = h.DOMAIN,
                                             WLP_ID_val = x.UNDER_WHITE_LEVEL,
                                             DIST_ID = x.INTRODUCER,
                                             WLP_NAME = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.UNDER_WHITE_LEVEL).UName,
                                             DIST_NAME = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.INTRODUCER).UName,
                                             DIST_NAMEMobile = db.TBL_MASTER_MEMBER.FirstOrDefault(s => s.MEM_ID == x.INTRODUCER).MEMBER_MOBILE,
                                             RailAgentId = x.RAIL_ID,
                                             PG_MAX_Val = y.PG_MAX_VALUE,
                                             PG_LessThenTwo = y.PG_EQUAL_LESS_2000,
                                             PG_Greater_Two = y.PG_EQUAL_GREATER_2000,
                                             PG_GST_Apply = y.PG_GST_STATUS,
                                             ADDN_Max_Val = y.ADDITIONAL_CHARGE_MAX_VAL,
                                             ADDn_Charge_AC = y.ADDITIONAL_CHARGE_AC,
                                             ADDn_Charge_NON_AC = y.ADDITIONAL_CHARGE_NON_AC,
                                             ADDn_GSTAPPLY = y.ADDITIONAL_GST_STATUS,
                                             Sln = y.SLN,
                                             PG_Charges_Apply = y.PG_CHARGES_APPLY,
                                             ADDITIONALCHARGESApply = y.ADDITIONAL_CHARGES_APPLY
                                         }).AsEnumerable().Select(z => new TBL_RAIL_AGENTS_COMMISSION
                                         {
                                             MEM_ID = z.MEM_ID,
                                             WLP_ID = (long)z.WLP_ID_val,
                                             DIST_ID = (long)z.DIST_ID,
                                             WLP_NAME = z.DomainName + "-" + z.WLP_NAME,
                                             DIST_NAME = z.DIST_NAME + "-" + z.DIST_NAMEMobile,
                                             RAIL_AGENT_ID = z.RailAgentId,
                                             PG_MAX_VALUE = z.PG_MAX_Val,
                                             PG_EQUAL_LESS_2000 = z.PG_LessThenTwo,
                                             PG_EQUAL_GREATER_2000 = z.PG_Greater_Two,
                                             PG_GST_STATUS = z.PG_GST_Apply,
                                             ADDITIONAL_CHARGE_MAX_VAL = z.ADDN_Max_Val,
                                             ADDITIONAL_CHARGE_AC = z.ADDn_Charge_AC,
                                             ADDITIONAL_CHARGE_NON_AC = z.ADDn_Charge_NON_AC,
                                             ADDITIONAL_GST_STATUS = z.ADDn_GSTAPPLY,
                                             Rail_table_Id = z.RailIdTable,
                                             SLN = z.Sln,
                                             PG_Charges_Apply_Val = (z.PG_Charges_Apply == false ? 0 : 1).ToString(),
                                             Additional_Charges_Apply_Val = (z.ADDITIONALCHARGESApply == false ? "0" : "1").ToString()
                                         }).FirstOrDefault();
                    return Json(RailAgentInfo, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetRAILAgentInformation(string TransId, string Mem_ID)
        {
            try
            {
                long valueid = long.Parse(TransId);
                long MemId = long.Parse(Mem_ID);
                var db = new DBContext();
                var GetRailAgent = db.TBL_RAIL_AGENT_INFORMATION.FirstOrDefault(x=>x.SLN== valueid && x.MEM_ID== MemId);
                if (GetRailAgent != null)
                {
                    return Json(new { data = GetRailAgent, Status = "0" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "", Status = "1" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
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


        [HttpPost]
        public async Task<JsonResult> GetMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) || oper.MEMBER_MOBILE.StartsWith(prefix) || oper.COMPANY.StartsWith(prefix) || oper.MEMBER_NAME.StartsWith(prefix) || oper.EMAIL_ID.StartsWith(prefix) || oper.CITY.StartsWith(prefix)
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
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
    }
}
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
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberCreditManagmentController : AdminBaseController
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
        // GET: Admin/MemberCreditManagment
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_ID != 1 && x.ROLE_ID != 2 && x.ROLE_ID != 3).ToList();
                    ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public async Task<JsonResult> PostCREDITBALANCE(MemberCreditBalanceModel objCredit)
        {
            var DBcon = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = DBcon.Database.BeginTransaction())
            {
                try
                {
                    var MerberName = DBcon.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                    var TDSVal = DBcon.TBL_TAX_MASTERS.FirstOrDefault(x => x.SLN == 2).TAX_VALUE;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == objCredit.MEMBER_ROLE).ROLE_NAME;
                    decimal TdsVal = 0;
                    decimal GstVal = 0;
                    decimal TotalGST = 0;
                    decimal TDS_Amount = 0;
                    decimal GST_Amount = 0;
                    decimal TotalCreditAmt = 0;
                    decimal CreditAmtWhithTDS = 0;
                    decimal AddMainBalance = 0;
                    decimal UpdateMainBalance = 0;
                    string TransactionType = "";
                    decimal.TryParse(TDSVal.ToString(), out TdsVal);
                    if (objCredit.CREDIT_TYPE == "DR")
                    {
                        var GetGSTVAlue = DBcon.TBL_STATES.FirstOrDefault(x => x.STATEID == MerberName.STATE_ID);
                        TotalGST = GetGSTVAlue.SGST + GetGSTVAlue.IGST + GetGSTVAlue.CGST;
                        decimal.TryParse(TotalGST.ToString(), out GstVal);
                        if (objCredit.GSTAPPLY == "Yes")
                        {
                            GST_Amount = ((objCredit.CREDIT_AMOUNT) * TotalGST) / 100;
                            TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                        }
                        else
                        {
                            GST_Amount = 0;
                            TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                        }
                    }
                    else
                    {
                        GST_Amount = 0;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                    }
                    if (objCredit.TDSAPPLY == "Yes")
                    {
                        TDS_Amount = ((objCredit.CREDIT_AMOUNT) * TdsVal) / 100;
                        CreditAmtWhithTDS = objCredit.CREDIT_AMOUNT - TDS_Amount;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount - TDS_Amount;
                    }
                    else
                    {
                        TDS_Amount = 0;
                        CreditAmtWhithTDS = objCredit.CREDIT_AMOUNT - TDS_Amount;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount - TDS_Amount;
                    }
                    AddMainBalance = Convert.ToDecimal(MerberName.BALANCE);
                    if (objCredit.CREDIT_TYPE == "DR")
                    {
                        UpdateMainBalance = AddMainBalance - TotalCreditAmt;
                        TransactionType = "DEBIT NOTE";
                    }
                    else
                    {
                        UpdateMainBalance = AddMainBalance + TotalCreditAmt;
                        TransactionType = "CREDIT NOTE";
                    }                    
                    TBL_CREDIT_BALANCE_DISTRIBUTION objcr = new TBL_CREDIT_BALANCE_DISTRIBUTION()
                    {
                        TO_MEM_ID = MemberCurrentUser.MEM_ID,
                        FROM_MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_ROLE = objCredit.MEMBER_ROLE,
                        CREDIT_DATE = DateTime.Now,
                        CREDIT_TYPE = objCredit.CREDIT_TYPE,
                        //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                        CREDIT_AMOUNT = TotalCreditAmt,
                        GST_VAL = TotalGST,
                        GST_AMOUNT = GST_Amount,
                        TDS_VAL = TdsVal,
                        TDS_AMOUNT = TDS_Amount,
                        CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        CREDIT_STATUS = true
                    };
                    DBcon.TBL_CREDIT_BALANCE_DISTRIBUTION.Add(objcr);
                    DBcon.SaveChanges();
                    MerberName.BALANCE = UpdateMainBalance;
                    DBcon.Entry(MerberName).State = System.Data.Entity.EntityState.Modified;
                    DBcon.SaveChanges();
                    decimal Frm_OpeningAmt = 0;
                    decimal Frm_ClosingAmt = 0;
                    decimal Frm_AddOpeningAmt = 0;
                    decimal Frm_AddClosingAmt = 0;
                    //var FromMemberamtobj = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var FromMemberamtobj = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (FromMemberamtobj != null)
                    {
                        decimal.TryParse(FromMemberamtobj.CLOSING.ToString(), out Frm_ClosingAmt);
                        if (objCredit.CREDIT_TYPE == "DR")
                        {
                            Frm_AddClosingAmt = Frm_ClosingAmt - TotalCreditAmt;
                        }
                        else
                        {
                            Frm_AddClosingAmt = Frm_ClosingAmt + TotalCreditAmt;
                        }
                    }
                    else
                    {
                        Frm_AddClosingAmt = TotalCreditAmt;
                    }
                    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_TYPE = member_Role,
                        TRANSACTION_TYPE = TransactionType,
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = objCredit.CREDIT_TYPE,
                        AMOUNT = (decimal)TotalCreditAmt,
                        NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        OPENING = Frm_ClosingAmt,
                        CLOSING = Frm_AddClosingAmt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (double)GST_Amount,
                        TDS = (double)TDS_Amount,
                        IPAddress = "",
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID
                    };
                    DBcon.TBL_ACCOUNTS.Add(objacnt);
                    DBcon.SaveChanges();
                    ContextTransaction.Commit();
                    //var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == objCredit.MEMBER_ROLE).ROLE_NAME;
                    return Json("Credit balance given to " + MerberName.UName + ". ");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("Try again after some time");
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberName(string prefix, string MemberType)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;
                long.TryParse(MemberType, out MEm_RoleId);
                    var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                               where oper.MEMBER_ROLE== MEm_RoleId && oper.ACTIVE_MEMBER == true &&(oper.UName.StartsWith(prefix) || oper.MEMBER_NAME.StartsWith(prefix) || oper.MEMBER_MOBILE.StartsWith(prefix) || oper.EMAIL_ID.StartsWith(prefix) || oper.COMPANY.StartsWith(prefix) || oper.ADDRESS.StartsWith(prefix) || oper.COMPANY_GST_NO.StartsWith(prefix))
                                               select new
                                               {
                                                   //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                                   label = oper.UName +" - "+ oper.MEMBER_MOBILE +" - "+ oper.COMPANY ,                                                   
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
        [HttpPost]
        public async Task<JsonResult> GetCreditSettleMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;
                
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.ACTIVE_MEMBER == true && (oper.UName.StartsWith(prefix) || oper.MEMBER_NAME.StartsWith(prefix) || oper.MEMBER_MOBILE.StartsWith(prefix) || oper.EMAIL_ID.StartsWith(prefix) || oper.COMPANY.StartsWith(prefix) || oper.ADDRESS.StartsWith(prefix) || oper.COMPANY_GST_NO.StartsWith(prefix))
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.COMPANY,
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

        public ActionResult GetCreditNoteList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();                   
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public PartialViewResult IndexGrid(string MemberInfo = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MemberInfo!="" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= To_Date_Val && (mem.COMPANY.Contains(MemberInfo) || mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TYPE.Contains(MemberInfo) || tblcre.CREDIT_AMOUNT.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CompanyName=mem.COMPANY,
                                          CompnanyGST=mem.COMPANY_GST_NO,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME=z.CompanyName,
                                          COMPANY_GST=z.CompnanyGST
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                else if (MemberInfo == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val 
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CompanyName = mem.COMPANY,
                                          CompnanyGST = mem.COMPANY_GST_NO,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.CompanyName,
                                          COMPANY_GST = z.CompnanyGST
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                if (MemberInfo != "" && DateFrom == "" && Date_To == "")
                {
                  
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID  && (mem.COMPANY.Contains(MemberInfo) || mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TYPE.Contains(MemberInfo) || tblcre.CREDIT_AMOUNT.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CompanyName = mem.COMPANY,
                                          CompnanyGST = mem.COMPANY_GST_NO,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.CompanyName,
                                          COMPANY_GST = z.CompnanyGST
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      //where tblcre.TO_MEM_ID==MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CompanyName = mem.COMPANY,
                                          CompnanyGST = mem.COMPANY_GST_NO,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.CompanyName,
                                          COMPANY_GST = z.CompnanyGST
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CreditLimitSetting()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberInfo = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER==MemberCurrentUser.MEM_ID).ToList();
                    ViewBag.MemberDetails = new SelectList(memberInfo, "MEM_ID", "UName");
                    var memberResivedCreditInfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault().RESERVED_CREDIT_LIMIT;
                    TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION model =new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION();
                    model.Reserved_credit_limit =Convert.ToDecimal(memberResivedCreditInfo);
                    return View(model);
                }
                catch (Exception ex)
                {
                    throw;
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
        public async Task<JsonResult> PostCREDITLIMITBALANCE(TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION objCredit)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal creditlimitBalance = 0;
                    decimal MainBal = 0;
                    decimal creditLimitMainBal = 0;
                    decimal AddMainBalance = 0;
                    decimal AddMainCreditLimitBalance = 0;
                    decimal closingAmt = 0;
                    decimal AddCloingAMt = 0;
                    decimal.TryParse(objCredit.CREDIT_AMOUNT.ToString(), out creditlimitBalance);
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    decimal CR_Opening = 0;
                    decimal CR_Closinging = 0;
                    decimal ADD_CR_Closinging = 0;
                    var CreditLimit_Val = db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Where(x => x.FROM_MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(c => c.SLN).FirstOrDefault();
                    if (CreditLimit_Val != null)
                    {
                        CR_Opening = (decimal)CreditLimit_Val.CREDIT_OPENING;
                        CR_Closinging = (decimal)CreditLimit_Val.CREDITCLOSING;
                        ADD_CR_Closinging = CR_Closinging + creditlimitBalance;
                    }
                    else
                    {
                        CR_Closinging = 0;
                        ADD_CR_Closinging = creditlimitBalance;
                    }
                    TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION objLimit = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                    {
                        TO_MEM_ID = MemberCurrentUser.MEM_ID,
                        FROM_MEM_ID = objCredit.FROM_MEM_ID,
                        CREDIT_DATE = DateTime.Now,
                        //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                        CREDIT_AMOUNT = creditlimitBalance,
                        GST_VAL = 0,
                        GST_AMOUNT = 0,
                        TDS_VAL = 0,
                        TDS_AMOUNT = 0,
                        CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        CREDIT_STATUS = true,
                        CREDIT_OPENING= CR_Closinging,
                        CREDITCLOSING = ADD_CR_Closinging,
                        CREDIT_TRN_TYPE="CR",
                        CORELATIONID= COrelationID,
                        STATUS="APPROVE"
                    };
                    db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(objLimit);
                    db.SaveChanges();
                    
                    var memberinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                    decimal.TryParse(memberinfo.BALANCE.ToString(),out MainBal);
                    decimal.TryParse(memberinfo.CREDIT_LIMIT.ToString(), out creditLimitMainBal);
                    AddMainBalance = MainBal + creditlimitBalance;
                    AddMainCreditLimitBalance = creditLimitMainBal + creditlimitBalance;
                    memberinfo.BALANCE = AddMainBalance;
                    memberinfo.CREDIT_LIMIT = AddMainCreditLimitBalance;
                    db.Entry(memberinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Accounts
                    //var FromMemberamtobj = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var FromMemberamtobj = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (FromMemberamtobj != null)
                    {
                        decimal.TryParse(FromMemberamtobj.CLOSING.ToString(), out closingAmt);
                        AddCloingAMt = closingAmt + creditlimitBalance;
                    }
                    else
                    {
                        AddCloingAMt = creditlimitBalance;
                        //decimal.TryParse(FromMemberamtobj.CLOSING.ToString(), out closingAmt);
                    }
                    string MEM_ROLE = string.Empty;
                    if (memberinfo.MEMBER_ROLE == 5)
                    {
                        MEM_ROLE = "RETAILER";
                    }
                    else
                    {
                        MEM_ROLE = "DISTRIBUTOR";
                    }
                    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_TYPE = MEM_ROLE,
                        TRANSACTION_TYPE = "CREDIT LIMIT",
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "CR",
                        AMOUNT = creditlimitBalance,
                        NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        OPENING = closingAmt,
                        CLOSING = AddCloingAMt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = 0,
                        TDS = 0,
                        IPAddress = "",
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID
                    };
                    db.TBL_ACCOUNTS.Add(objacnt);
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    string msgVal = "Credit Limit is given to member :-"+ memberinfo.MEMBER_NAME+"-"+ memberinfo.MEM_UNIQUE_ID;
                    return Json(msgVal,JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("Please try again after some time");
                    throw ex;
                }
          
            }
            
        }
        public ActionResult GetCreditLimitList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public PartialViewResult CREEEDITLimitGrid(string MemberInfo = "", string DateFrom ="",string Date_To="")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MemberInfo!="" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);


                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= Date_To_Val
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val && (mem.COMPANY.Contains(MemberInfo)|| mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TRN_TYPE.Contains(MemberInfo) || tblcre.CREDIT_OPENING.ToString().Contains(MemberInfo) || tblcre.CREDITCLOSING.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          COMPANY_NAME=mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS=tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CREEEDITLimitGrid", memberinfo);
                } 
                else if (MemberInfo == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);


                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= Date_To_Val
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CREEEDITLimitGrid", memberinfo);
                }
                else if (MemberInfo != "" && DateFrom == "" && Date_To == "")
                {
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= Date_To_Val
                                      where (mem.COMPANY.Contains(MemberInfo) || mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TRN_TYPE.Contains(MemberInfo) || tblcre.CREDIT_OPENING.ToString().Contains(MemberInfo) || tblcre.CREDITCLOSING.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CREEEDITLimitGrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE); ;
                    return PartialView("CREEEDITLimitGrid", memberinfo);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetReservedCreditAmount()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberrole = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER== MemberCurrentUser.MEM_ID).ToList();
                    ViewBag.RoleDetails = new SelectList(memberrole, "MEM_ID", "UName");
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public async Task<JsonResult> PostReservedCREDITBALANCE(ReservedCreditLimitModel objCredit)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var SetReservedcreditLimit =db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID==objCredit.FROM_MEM_ID);
                    SetReservedcreditLimit.RESERVED_CREDIT_LIMIT =Convert.ToDecimal(objCredit.ReservedCreditLimit);
                    db.Entry(SetReservedcreditLimit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    string msgval = "Reserved limit is saved for member :-"+SetReservedcreditLimit.MEMBER_NAME+"-"+ SetReservedcreditLimit.MEM_UNIQUE_ID;
                    //return Json("Reserved limit is saved to distributor.");
                    return Json(msgval, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("try again after some time");
                    throw;
                }
            }
                
        }


        [HttpPost]
        public async Task<JsonResult> GetDistributorMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;

                //var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           join role in db.TBL_MASTER_MEMBER_ROLE on oper.MEMBER_ROLE equals role.ROLE_ID
                                           //where oper.UName.StartsWith(prefix) && oper.INTRODUCER == MemberCurrentUser.MEM_ID
                                           where oper.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && oper.PAYMENT_MODE=="Direct" && oper.ACTIVE_MEMBER==true && (oper.UName.StartsWith(prefix) || oper.MEMBER_NAME.StartsWith(prefix) || oper.MEMBER_MOBILE.StartsWith(prefix) || oper.EMAIL_ID.StartsWith(prefix) || oper.COMPANY.StartsWith(prefix) || oper.ADDRESS.StartsWith(prefix) || oper.COMPANY_GST_NO.StartsWith(prefix))
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + "-" + oper.MEMBER_NAME + " - " + oper.MEMBER_MOBILE + " - " + oper.COMPANY+"-("+role.ROLE_NAME+")",
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
        public ActionResult DistributorCreditLimitReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();                  
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public PartialViewResult DistributorCreditindexgrid(string MemberInfo = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MemberInfo != "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);

                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= Date_To_Val
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val && (mem.COMPANY.Contains(MemberInfo)|| mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TRN_TYPE.Contains(MemberInfo) || tblcre.CREDIT_AMOUNT.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          STATUS=tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                else if (MemberInfo == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);

                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= Date_To_Val
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,

                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                else if (MemberInfo != "" && DateFrom == "" && Date_To == "")
                {


                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= Date_To_Val
                                      where (mem.COMPANY.Contains(MemberInfo)|| mem.COMPANY_GST_NO.Contains(MemberInfo) || mem.UName.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.MEMBER_MOBILE.Contains(MemberInfo) || mem.EMAIL_ID.Contains(MemberInfo) || tblcre.CREDIT_TRN_TYPE.Contains(MemberInfo) || tblcre.CREDIT_AMOUNT.ToString().Contains(MemberInfo))
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      //where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date

                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          COMPANY_NAME = mem.COMPANY,
                                          COMPANY_GST = mem.COMPANY_GST_NO,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          STATUS = tblcre.STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          COMPANY_NAME = z.COMPANY_NAME,
                                          COMPANY_GST = z.COMPANY_GST,
                                          STATUS=z.STATUS
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }



            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetReservedCreditLimit(string Mem_ID)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long MEmber_Id = 0;
                long.TryParse(Mem_ID, out MEmber_Id);
                var walletamount = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MEmber_Id).FirstOrDefault();
                if (walletamount != null)
                {

                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }
        public ActionResult CreditBalanceSettlement()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public async Task<JsonResult> PostCREDITBALANCESettlement(MemberCreditBalanceModel objCredit)
        {
            var DBcon = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = DBcon.Database.BeginTransaction())
            {
                try
                {
                    var MerberName = DBcon.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                    var TDSVal = DBcon.TBL_TAX_MASTERS.FirstOrDefault(x => x.SLN == 2).TAX_VALUE;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == MerberName.MEMBER_ROLE).ROLE_NAME;
                    decimal TdsVal = 0;
                    decimal GstVal = 0;
                    decimal TotalGST = 0;
                    decimal TDS_Amount = 0;
                    decimal GST_Amount = 0;
                    decimal TotalCreditAmt = 0;
                    decimal CreditAmtWhithTDS = 0;
                    decimal AddMainBalance = 0;
                    decimal UpdateMainBalance = 0;
                    string TransactionType = "Credit Balance Settlement";
                    decimal.TryParse(TDSVal.ToString(), out TdsVal);
                    decimal CreditBalance = 0;
                    decimal.TryParse(MerberName.CREDIT_LIMIT.ToString(), out CreditBalance);
                    decimal MainBalance = 0;
                    decimal.TryParse(MerberName.BALANCE.ToString(), out MainBalance);
                    TotalCreditAmt = objCredit.CREDIT_AMOUNT;
                    decimal UpdateClosingBal = 0;
                    TBL_CREDIT_BALANCE_DISTRIBUTION objcr = new TBL_CREDIT_BALANCE_DISTRIBUTION()
                    {
                        TO_MEM_ID = MemberCurrentUser.MEM_ID,
                        FROM_MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_ROLE = (long)MerberName.MEMBER_ROLE,
                        CREDIT_DATE = DateTime.Now,
                        CREDIT_TYPE = "CR",
                        //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                        CREDIT_AMOUNT = TotalCreditAmt,
                        GST_VAL = TotalGST,
                        GST_AMOUNT = GST_Amount,
                        TDS_VAL = TdsVal,
                        TDS_AMOUNT = TDS_Amount,
                        CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        CREDIT_STATUS = true
                    };
                    DBcon.TBL_CREDIT_BALANCE_DISTRIBUTION.Add(objcr);
                   // DBcon.SaveChanges();
                    UpdateMainBalance = MainBalance + TotalCreditAmt;
                    MerberName.BALANCE = UpdateMainBalance;
                    DBcon.Entry(MerberName).State = System.Data.Entity.EntityState.Modified;
                   // DBcon.SaveChanges();
                    decimal Frm_OpeningAmt = 0;
                    decimal Frm_ClosingAmt = 0;
                    decimal Frm_AddOpeningAmt = 0;
                    decimal Frm_AddClosingAmt = 0;
                    var FromMemberamtobj = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (FromMemberamtobj != null)
                    {
                        Frm_ClosingAmt = FromMemberamtobj.CLOSING;
                        Frm_AddClosingAmt = Frm_ClosingAmt + TotalCreditAmt;
                    }
                    else
                    {
                        Frm_AddClosingAmt = TotalCreditAmt;
                    }
                    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_TYPE = member_Role,
                        TRANSACTION_TYPE = TransactionType,
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "CR",
                        AMOUNT = (decimal)TotalCreditAmt,
                        NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        OPENING = Frm_ClosingAmt,
                        CLOSING = Frm_AddClosingAmt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (double)GST_Amount,
                        TDS = (double)TDS_Amount,
                        IPAddress = "",
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID
                    };
                    DBcon.TBL_ACCOUNTS.Add(objacnt);
                   // DBcon.SaveChanges();
                    if (CreditBalance <= TotalCreditAmt)
                    {
                        UpdateMainBalance = MainBalance + TotalCreditAmt- CreditBalance;
                        MerberName.BALANCE = UpdateMainBalance;
                        MerberName.CREDIT_LIMIT = 0;
                        DBcon.Entry(MerberName).State = System.Data.Entity.EntityState.Modified;
                       // DBcon.SaveChanges();

                        Frm_ClosingAmt = FromMemberamtobj.CLOSING;
                        UpdateClosingBal= Frm_AddClosingAmt- CreditBalance;
                        TBL_ACCOUNTS objUpdateacnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = objCredit.FROM_MEM_ID,
                            MEMBER_TYPE = member_Role,
                            TRANSACTION_TYPE = "CREDIT Amount Settlement",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            AMOUNT = (decimal)CreditBalance,
                            NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                            OPENING = Frm_AddClosingAmt,
                            CLOSING = UpdateClosingBal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = (double)GST_Amount,
                            TDS = (double)TDS_Amount,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID
                        };
                        DBcon.TBL_ACCOUNTS.Add(objUpdateacnt);
                        //DBcon.SaveChanges();
                    }
                    else
                    {
                        UpdateMainBalance = MainBalance + TotalCreditAmt - CreditBalance;
                        //MerberName.BALANCE = UpdateMainBalance;
                        MerberName.CREDIT_LIMIT = CreditBalance- TotalCreditAmt;
                        DBcon.Entry(MerberName).State = System.Data.Entity.EntityState.Modified;
                        //DBcon.SaveChanges();

                        //Frm_ClosingAmt = FromMemberamtobj.CLOSING;
                        //UpdateClosingBal = Frm_AddClosingAmt - CreditBalance;
                        //TBL_ACCOUNTS objUpdateacnt = new TBL_ACCOUNTS()
                        //{
                        //    API_ID = 0,
                        //    MEM_ID = objCredit.FROM_MEM_ID,
                        //    MEMBER_TYPE = member_Role,
                        //    TRANSACTION_TYPE = "CREDIT Amount Settlement",
                        //    TRANSACTION_DATE = DateTime.Now,
                        //    TRANSACTION_TIME = DateTime.Now,
                        //    DR_CR = "DR",
                        //    AMOUNT = (decimal)CreditBalance,
                        //    NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        //    OPENING = Frm_AddClosingAmt,
                        //    CLOSING = UpdateClosingBal,
                        //    REC_NO = 0,
                        //    COMM_AMT = 0,
                        //    GST = (double)GST_Amount,
                        //    TDS = (double)TDS_Amount,
                        //    IPAddress = "",
                        //    SERVICE_ID = 0,
                        //    CORELATIONID = COrelationID
                        //};
                        //DBcon.TBL_ACCOUNTS.Add(objUpdateacnt);
                        //DBcon.SaveChanges();
                    }
                    DBcon.SaveChanges();
                    ContextTransaction.Commit();
                    //var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == objCredit.MEMBER_ROLE).ROLE_NAME;
                    return Json("Credit balance is settle of " + MerberName.UName + ". ");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("Try again after some time");
                    throw;
                }
            }
        }
        public ActionResult CreditCallingList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();                 
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
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
        public PartialViewResult AllMerchantCreditCallingGrid(string SearchVal = "")
        {
            try
            {
                var db = new DBContext();
                if (SearchVal != "")
                {
                    long DistId = 0;
                    //long.TryParse(DistributorId, out DistId);
                    //var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == DistId && x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5).OrderByDescending(x => x.JOINING_DATE).ToList();
                    var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.ACTIVE_MEMBER == true && x.CREDIT_LIMIT > 0 && x.MEMBER_ROLE == 5 && (x.UName.StartsWith(SearchVal) || x.MEMBER_MOBILE.StartsWith(SearchVal) || x.MEMBER_NAME.StartsWith(SearchVal) || x.COMPANY.StartsWith(SearchVal) || x.COMPANY_GST_NO.StartsWith(SearchVal) || x.ADDRESS.StartsWith(SearchVal) || x.CITY.StartsWith(SearchVal) || x.PIN.StartsWith(SearchVal) || x.EMAIL_ID.StartsWith(SearchVal) || x.AADHAAR_NO.StartsWith(SearchVal) || x.PAN_NO.StartsWith(SearchVal) || x.RAIL_ID.StartsWith(SearchVal) || x.FACEBOOK_ID.StartsWith(SearchVal) || x.WEBSITE_NAME.StartsWith(SearchVal) || x.MEM_UNIQUE_ID.StartsWith(SearchVal))).ToList();
                    return PartialView("AllMerchantCreditCallingGrid", MerchantList);
                }
                else
                {
                    var MerchantList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER == true && x.CREDIT_LIMIT>0).OrderByDescending(x => x.JOINING_DATE).ToList();
                    return PartialView("AllMerchantCreditCallingGrid", MerchantList);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public FileResult ExportMerchantInfoIndex(string MEM_ID = "")
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
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.ACTIVE_MEMBER == true && x.CREDIT_LIMIT > 0 && x.MEMBER_ROLE == 5 &&(x.UName.StartsWith(MEM_ID) || x.MEMBER_MOBILE.StartsWith(MEM_ID) || x.MEMBER_NAME.StartsWith(MEM_ID) || x.COMPANY.StartsWith(MEM_ID) || x.COMPANY_GST_NO.StartsWith(MEM_ID) || x.ADDRESS.StartsWith(MEM_ID) || x.CITY.StartsWith(MEM_ID) || x.PIN.StartsWith(MEM_ID) || x.EMAIL_ID.StartsWith(MEM_ID) || x.AADHAAR_NO.StartsWith(MEM_ID) || x.PAN_NO.StartsWith(MEM_ID) || x.RAIL_ID.StartsWith(MEM_ID) || x.FACEBOOK_ID.StartsWith(MEM_ID) || x.WEBSITE_NAME.StartsWith(MEM_ID) || x.MEM_UNIQUE_ID.StartsWith(MEM_ID))).ToList();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == Mem_ID).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Mem ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RAIL_ID).Titled("Rail ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);                
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Amount").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BLOCKED_BALANCE).Titled("Block Balance").Filterable(true).Sortable(true);
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
            {
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5 && x.ACTIVE_MEMBER == true && x.CREDIT_LIMIT > 0).OrderByDescending(x => x.JOINING_DATE).ToList();
                IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.MEM_UNIQUE_ID).Titled("Mem ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RAIL_ID).Titled("Rail ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_NAME).Titled("Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile No").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CREDIT_LIMIT).Titled("Credit Amount").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BLOCKED_BALANCE).Titled("Block Balance").Filterable(true).Sortable(true);
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

    }
}
using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class MemberRDSReportsController : AdminBaseController
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
        // GET: Admin/MemberRDSReports
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
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
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
        public PartialViewResult IndexGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                //if (Merchant != "")
                if (MerchantUser != "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.BOOKING_AMT.ToString().StartsWith(MerchantUser)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME=y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                              
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                              
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
                {

                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);                    
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser)|| x.BOOKING_AMT.ToString().Contains(MerchantUser))
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where y.MEMBER_ROLE == 5
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_CANCELLATION> CreateExportableGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            if (MerchantUser != "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser)) && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                      select new
                                      {
                                          SLN = x.SLN,
                                          //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRN_ID,
                                          PNR = x.PNR_NO,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.REFUND_AMT,
                                          //PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRN_DATE,
                                          TDR_CAN = x.TDR_CAN,
                                          CANCELLATION_ID = x.CANCELLATION_ID,
                                          CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                          CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                          SYSTEM_DATE = x.SYSTEM_DATE,
                                          WT_AUTO_CAN = x.WT_AUTO_CAN,
                                          //APP_CODE = x.APP_CODE,
                                          //PAYMODE = x.PAYMODE,
                                          //SECURITY_ID = x.SECURITY_ID,
                                          //RU = x.RU,
                                          //PAY_REQ = x.PAY_REQ,
                                          //RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                      {
                                          SLN = z.SLN,
                                          //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRN_ID = z.TRAN_ID,
                                          PNR_NO = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          REFUND_AMT = z.BOOKING_AMT,
                                          //PG_CHARGE = z.PG_CHARGE,
                                          TRN_DATE = z.TRAN_DATE,
                                          TDR_CAN = z.TDR_CAN,
                                          CANCELLATION_ID = z.CANCELLATION_ID,
                                          CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                          SYSTEM_DATE = z.SYSTEM_DATE,
                                          WT_AUTO_CAN = z.WT_AUTO_CAN,
                                          //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          //APP_CODE = z.APP_CODE,
                                          //PAYMODE = z.PAYMODE,
                                          //SECURITY_ID = z.SECURITY_ID,
                                          //RU = z.RU,
                                          //PAY_REQ = z.PAY_REQ,
                                          //RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_GST_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                      select new
                                      {
                                          SLN = x.SLN,
                                          //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRN_ID,
                                          PNR = x.PNR_NO,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.REFUND_AMT,
                                          //PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRN_DATE,
                                          TDR_CAN = x.TDR_CAN,
                                          CANCELLATION_ID = x.CANCELLATION_ID,
                                          CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                          CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                          SYSTEM_DATE = x.SYSTEM_DATE,
                                          WT_AUTO_CAN = x.WT_AUTO_CAN,
                                          //APP_CODE = x.APP_CODE,
                                          //PAYMODE = x.PAYMODE,
                                          //SECURITY_ID = x.SECURITY_ID,
                                          //RU = x.RU,
                                          //PAY_REQ = x.PAY_REQ,
                                          //RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                      {
                                          SLN = z.SLN,
                                          //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRN_ID = z.TRAN_ID,
                                          PNR_NO = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          REFUND_AMT = z.BOOKING_AMT,
                                          //PG_CHARGE = z.PG_CHARGE,
                                          TRN_DATE = z.TRAN_DATE,
                                          TDR_CAN = z.TDR_CAN,
                                          CANCELLATION_ID = z.CANCELLATION_ID,
                                          CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                          SYSTEM_DATE = z.SYSTEM_DATE,
                                          WT_AUTO_CAN = z.WT_AUTO_CAN,
                                          //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          //APP_CODE = z.APP_CODE,
                                          //PAYMODE = z.PAYMODE,
                                          //SECURITY_ID = z.SECURITY_ID,
                                          //RU = z.RU,
                                          //PAY_REQ = z.PAY_REQ,
                                          //RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_GST_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser))
                                      select new
                                      {
                                          SLN = x.SLN,
                                          //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRN_ID,
                                          PNR = x.PNR_NO,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.REFUND_AMT,
                                          //PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRN_DATE,
                                          TDR_CAN = x.TDR_CAN,
                                          CANCELLATION_ID = x.CANCELLATION_ID,
                                          CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                          CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                          SYSTEM_DATE = x.SYSTEM_DATE,
                                          WT_AUTO_CAN = x.WT_AUTO_CAN,
                                          //APP_CODE = x.APP_CODE,
                                          //PAYMODE = x.PAYMODE,
                                          //SECURITY_ID = x.SECURITY_ID,
                                          //RU = x.RU,
                                          //PAY_REQ = x.PAY_REQ,
                                          //RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                      {
                                          SLN = z.SLN,
                                          //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRN_ID = z.TRAN_ID,
                                          PNR_NO = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          REFUND_AMT = z.BOOKING_AMT,
                                          //PG_CHARGE = z.PG_CHARGE,
                                          TRN_DATE = z.TRAN_DATE,
                                          TDR_CAN = z.TDR_CAN,
                                          CANCELLATION_ID = z.CANCELLATION_ID,
                                          CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                          SYSTEM_DATE = z.SYSTEM_DATE,
                                          WT_AUTO_CAN = z.WT_AUTO_CAN,
                                          //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          //APP_CODE = z.APP_CODE,
                                          //PAYMODE = z.PAYMODE,
                                          //SECURITY_ID = z.SECURITY_ID,
                                          //RU = z.RU,
                                          //PAY_REQ = z.PAY_REQ,
                                          //RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_GST_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where y.MEMBER_ROLE == 5
                                      select new
                                      {
                                          SLN = x.SLN,
                                          //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRN_ID,
                                          PNR = x.PNR_NO,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.REFUND_AMT,
                                          //PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRN_DATE,
                                          TDR_CAN = x.TDR_CAN,
                                          CANCELLATION_ID = x.CANCELLATION_ID,
                                          CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                          CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                          SYSTEM_DATE = x.SYSTEM_DATE,
                                          WT_AUTO_CAN = x.WT_AUTO_CAN,
                                          //APP_CODE = x.APP_CODE,
                                          //PAYMODE = x.PAYMODE,
                                          //SECURITY_ID = x.SECURITY_ID,
                                          //RU = x.RU,
                                          //PAY_REQ = x.PAY_REQ,
                                          //RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                                      {
                                          SLN = z.SLN,
                                          //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRN_ID = z.TRAN_ID,
                                          PNR_NO = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          REFUND_AMT = z.BOOKING_AMT,
                                          //PG_CHARGE = z.PG_CHARGE,
                                          TRN_DATE = z.TRAN_DATE,
                                          TDR_CAN = z.TDR_CAN,
                                          CANCELLATION_ID = z.CANCELLATION_ID,
                                          CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                          SYSTEM_DATE = z.SYSTEM_DATE,
                                          WT_AUTO_CAN = z.WT_AUTO_CAN,
                                          //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          //APP_CODE = z.APP_CODE,
                                          //PAYMODE = z.PAYMODE,
                                          //SECURITY_ID = z.SECURITY_ID,
                                          //RU = z.RU,
                                          //PAY_REQ = z.PAY_REQ,
                                          //RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_GST_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            //    var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                  select new
            //                  {
            //                      SLN = x.SLN,
            //                      BOOKING_GATEWAY = x.BOOKING_GATEWAY,
            //                      TRAN_ID = x.TRAN_ID,
            //                      PNR = x.PNR,
            //                      OPR_ID = x.OPR_ID,
            //                      BOOKING_AMT = x.BOOKING_AMT,
            //                      PG_CHARGE = x.PG_CHARGE,
            //                      TRAN_DATE = x.TRAN_DATE,
            //                      TRAN_STATUS = x.TRAN_STATUS,
            //                      BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
            //                      BOOKING_TIME = x.BOOKING_TIME,
            //                      CURRENCY_TYPE = x.CURRENCY_TYPE,
            //                      APP_CODE = x.APP_CODE,
            //                      PAYMODE = x.PAYMODE,
            //                      SECURITY_ID = x.SECURITY_ID,
            //                      RU = x.RU,
            //                      PAY_REQ = x.PAY_REQ,
            //                      RET_RES = x.RET_RES,
            //                      MER_RAIL_ID = y.RAIL_ID,
            //                      PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
            //                      PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
            //                      PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
            //                      PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
            //                      PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
            //                      PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
            //                      ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
            //                      ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
            //                      ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
            //                      ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,

            //                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
            //                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
            //                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
            //                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
            //                      CORRELATION_ID = x.CORRELATION_ID,
            //                      GST_RATE = x.GST_RATE,
            //                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
            //                  }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
            //                  {
            //                      SLN = z.SLN,
            //                      BOOKING_GATEWAY = z.BOOKING_GATEWAY,
            //                      TRAN_ID = z.TRAN_ID,
            //                      PNR = z.PNR,
            //                      OPR_ID = z.OPR_ID,
            //                      BOOKING_AMT = z.BOOKING_AMT,
            //                      PG_CHARGE = z.PG_CHARGE,
            //                      TRAN_DATE = z.TRAN_DATE,
            //                      TRAN_STATUS = z.TRAN_STATUS,
            //                      BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
            //                      BOOKING_TIME = z.BOOKING_TIME,
            //                      CURRENCY_TYPE = z.CURRENCY_TYPE,
            //                      APP_CODE = z.APP_CODE,
            //                      PAYMODE = z.PAYMODE,
            //                      SECURITY_ID = z.SECURITY_ID,
            //                      RU = z.RU,
            //                      PAY_REQ = z.PAY_REQ,
            //                      RET_RES = z.RET_RES,
            //                      MER_RAIL_ID = z.MER_RAIL_ID,
            //                      PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
            //                      PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
            //                      PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
            //                      PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
            //                      PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
            //                      PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
            //                      ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
            //                      ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
            //                      ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
            //                      ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,

            //                      ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
            //                      ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
            //                      TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                      TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
            //                      TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
            //                      CORRELATION_ID = z.CORRELATION_ID,
            //                      GST_RATE = z.GST_RATE,
            //                      MERCHANT_NAME = z.MERCHANT_NAME,
            //                  }).ToList();


            ////var memberinfo = dbcontext.TBL_FINAL_RDS_BOOKING.Where(x => x.WLP_ID == CurrentUser.USER_ID ).ToList().OrderByDescending(x => x.TRAN_DATE);
            //IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(memberinfo);
            //grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //grid.Query = Request.QueryString;
            //grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_STATUS).Titled("Booking Status ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BOOKING_TRAN_STATUS).Titled("Booking Trn Status").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
            //grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
            //grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 9999999;


            ////foreach (IGridColumn column in grid.Columns)
            ////{
            ////    column.Filter.IsEnabled = true;
            ////    column.Sort.IsEnabled = true;

            ////}
            ////foreach (IGridColumn row in grid.Rows)
            ////{
            ////    row.CssClasses = "red";

            ////}

            //return grid;

        }
        [HttpGet]
        public FileResult ExportIndex(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportBookingGrid(MerchantUser, DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_RDS_BOOKING> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "AdminRDSReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_FINAL_RDS_BOOKING> CreateExportBookingGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            //if (Merchant != "")
            if (MerchantUser != "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
            {

                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val 
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser))
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where y.MEMBER_ROLE == 5
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
        }
        [HttpPost]
        public JsonResult GetDistributorList(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
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
        public JsonResult GetMerchantList(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
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

        public ActionResult RDSCancellationReport()
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
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
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

        //public PartialViewResult IndexCancellationGrid(string Whitelevel = "", string Distributor = "", string Merchant = "", string DateFrom = "", string Date_To = "")

        public PartialViewResult IndexCancellationGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MerchantUser != "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.REFUND_AMT.ToString().Contains(MerchantUser)) && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                          select new
                                          {
                                              SLN = x.SLN,
                                              //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRN_ID,
                                              PNR = x.PNR_NO,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.REFUND_AMT,
                                              //PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRN_DATE,
                                              TDR_CAN = x.TDR_CAN,
                                              CANCELLATION_ID = x.CANCELLATION_ID,
                                              CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                              CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                              SYSTEM_DATE = x.SYSTEM_DATE,
                                              WT_AUTO_CAN = x.WT_AUTO_CAN,
                                              //APP_CODE = x.APP_CODE,
                                              //PAYMODE = x.PAYMODE,
                                              //SECURITY_ID = x.SECURITY_ID,
                                              //RU = x.RU,
                                              //PAY_REQ = x.PAY_REQ,
                                              //RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_CANCELLATION
                                          {
                                              SerialNo = index + 1,
                                              SLN = z.SLN,
                                              //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRN_ID = z.TRAN_ID,
                                              PNR_NO = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              REFUND_AMT = z.BOOKING_AMT,
                                              //PG_CHARGE = z.PG_CHARGE,
                                              TRN_DATE = z.TRAN_DATE,
                                              TDR_CAN = z.TDR_CAN,
                                              CANCELLATION_ID = z.CANCELLATION_ID,
                                              CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                              SYSTEM_DATE = z.SYSTEM_DATE,
                                              WT_AUTO_CAN = z.WT_AUTO_CAN,
                                              //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              //APP_CODE = z.APP_CODE,
                                              //PAYMODE = z.PAYMODE,
                                              //SECURITY_ID = z.SECURITY_ID,
                                              //RU = z.RU,
                                              //PAY_REQ = z.PAY_REQ,
                                              //RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);                    
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                          select new
                                          {
                                              SLN = x.SLN,
                                              //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRN_ID,
                                              PNR = x.PNR_NO,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.REFUND_AMT,
                                              //PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRN_DATE,
                                              TDR_CAN = x.TDR_CAN,
                                              CANCELLATION_ID = x.CANCELLATION_ID,
                                              CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                              CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                              SYSTEM_DATE = x.SYSTEM_DATE,
                                              WT_AUTO_CAN = x.WT_AUTO_CAN,
                                              //APP_CODE = x.APP_CODE,
                                              //PAYMODE = x.PAYMODE,
                                              //SECURITY_ID = x.SECURITY_ID,
                                              //RU = x.RU,
                                              //PAY_REQ = x.PAY_REQ,
                                              //RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_CANCELLATION
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRN_ID = z.TRAN_ID,
                                              PNR_NO = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              REFUND_AMT = z.BOOKING_AMT,
                                              //PG_CHARGE = z.PG_CHARGE,
                                              TRN_DATE = z.TRAN_DATE,
                                              TDR_CAN = z.TDR_CAN,
                                              CANCELLATION_ID = z.CANCELLATION_ID,
                                              CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                              SYSTEM_DATE = z.SYSTEM_DATE,
                                              WT_AUTO_CAN = z.WT_AUTO_CAN,
                                              //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              //APP_CODE = z.APP_CODE,
                                              //PAYMODE = z.PAYMODE,
                                              //SECURITY_ID = z.SECURITY_ID,
                                              //RU = z.RU,
                                              //PAY_REQ = z.PAY_REQ,
                                              //RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.REFUND_AMT.ToString().Contains(MerchantUser))
                                          select new
                                          {
                                              SLN = x.SLN,
                                              //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRN_ID,
                                              PNR = x.PNR_NO,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.REFUND_AMT,
                                              //PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRN_DATE,
                                              TDR_CAN = x.TDR_CAN,
                                              CANCELLATION_ID = x.CANCELLATION_ID,
                                              CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                              CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                              SYSTEM_DATE = x.SYSTEM_DATE,
                                              WT_AUTO_CAN = x.WT_AUTO_CAN,
                                              //APP_CODE = x.APP_CODE,
                                              //PAYMODE = x.PAYMODE,
                                              //SECURITY_ID = x.SECURITY_ID,
                                              //RU = x.RU,
                                              //PAY_REQ = x.PAY_REQ,
                                              //RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_CANCELLATION
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRN_ID = z.TRAN_ID,
                                              PNR_NO = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              REFUND_AMT = z.BOOKING_AMT,
                                              //PG_CHARGE = z.PG_CHARGE,
                                              TRN_DATE = z.TRAN_DATE,
                                              TDR_CAN = z.TDR_CAN,
                                              CANCELLATION_ID = z.CANCELLATION_ID,
                                              CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                              SYSTEM_DATE = z.SYSTEM_DATE,
                                              WT_AUTO_CAN = z.WT_AUTO_CAN,
                                              //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              //APP_CODE = z.APP_CODE,
                                              //PAYMODE = z.PAYMODE,
                                              //SECURITY_ID = z.SECURITY_ID,
                                              //RU = z.RU,
                                              //PAY_REQ = z.PAY_REQ,
                                              //RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where y.MEMBER_ROLE == 5
                                          select new
                                          {
                                              SLN = x.SLN,
                                              //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRN_ID,
                                              PNR = x.PNR_NO,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.REFUND_AMT,
                                              //PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRN_DATE,
                                              TDR_CAN = x.TDR_CAN,
                                              CANCELLATION_ID = x.CANCELLATION_ID,
                                              CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                              CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                              SYSTEM_DATE = x.SYSTEM_DATE,
                                              WT_AUTO_CAN = x.WT_AUTO_CAN,
                                              //APP_CODE = x.APP_CODE,
                                              //PAYMODE = x.PAYMODE,
                                              //SECURITY_ID = x.SECURITY_ID,
                                              //RU = x.RU,
                                              //PAY_REQ = x.PAY_REQ,
                                              //RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_CANCELLATION
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRN_ID = z.TRAN_ID,
                                              PNR_NO = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              REFUND_AMT = z.BOOKING_AMT,
                                              //PG_CHARGE = z.PG_CHARGE,
                                              TRN_DATE = z.TRAN_DATE,
                                              TDR_CAN = z.TDR_CAN,
                                              CANCELLATION_ID = z.CANCELLATION_ID,
                                              CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                              SYSTEM_DATE = z.SYSTEM_DATE,
                                              WT_AUTO_CAN = z.WT_AUTO_CAN,
                                              //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              //APP_CODE = z.APP_CODE,
                                              //PAYMODE = z.PAYMODE,
                                              //SECURITY_ID = z.SECURITY_ID,
                                              //RU = z.RU,
                                              //PAY_REQ = z.PAY_REQ,
                                              //RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_FINAL_CANCELLATION> CreateExportCancellationableGrid()
        {
            var dbcontext = new DBContext();
            var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
                              join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                              select new
                              {
                                  SLN = x.SLN,
                                  //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                  TRAN_ID = x.TRN_ID,
                                  PNR = x.PNR_NO,
                                  OPR_ID = x.OPR_ID,
                                  BOOKING_AMT = x.REFUND_AMT,
                                  //PG_CHARGE = x.PG_CHARGE,
                                  TRAN_DATE = x.TRN_DATE,
                                  TDR_CAN = x.TDR_CAN,
                                  CANCELLATION_ID = x.CANCELLATION_ID,
                                  CANCELLATION_STATUS = x.CANCELLATION_TYPE,
                                  CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
                                  SYSTEM_DATE = x.SYSTEM_DATE,
                                  WT_AUTO_CAN = x.WT_AUTO_CAN,
                                  //APP_CODE = x.APP_CODE,
                                  //PAYMODE = x.PAYMODE,
                                  //SECURITY_ID = x.SECURITY_ID,
                                  //RU = x.RU,
                                  //PAY_REQ = x.PAY_REQ,
                                  //RET_RES = x.RET_RES,
                                  MER_RAIL_ID = y.RAIL_ID,
                                  PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                  PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                  PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                  PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                  PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                  PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                  ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                  ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                  ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                  ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                  REMARK = x.REMARKS,
                                  NOTES = x.NOTES,
                                  ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                  ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                  TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                  TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                  TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                  CORRELATION_ID = x.CORRELATION_ID,
                                  GST_RATE = x.GST_RATE,
                                  MERCHANT_NAME = y.MEM_UNIQUE_ID,
                              }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
                              {
                                  SLN = z.SLN,
                                  //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                  TRN_ID = z.TRAN_ID,
                                  PNR_NO = z.PNR,
                                  OPR_ID = z.OPR_ID,
                                  REFUND_AMT = z.BOOKING_AMT,
                                  //PG_CHARGE = z.PG_CHARGE,
                                  TRN_DATE = z.TRAN_DATE,
                                  TDR_CAN = z.TDR_CAN,
                                  CANCELLATION_ID = z.CANCELLATION_ID,
                                  CANCELLATION_TYPE = z.CANCELLATION_STATUS,
                                  SYSTEM_DATE = z.SYSTEM_DATE,
                                  WT_AUTO_CAN = z.WT_AUTO_CAN,
                                  //CURRENCY_TYPE = z.CURRENCY_TYPE,
                                  //APP_CODE = z.APP_CODE,
                                  //PAYMODE = z.PAYMODE,
                                  //SECURITY_ID = z.SECURITY_ID,
                                  //RU = z.RU,
                                  //PAY_REQ = z.PAY_REQ,
                                  //RET_RES = z.RET_RES,
                                  MER_RAIL_ID = z.MER_RAIL_ID,
                                  PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                  PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                  PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                  PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                  PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                  PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                  ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                  ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                  ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                  ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                  REMARKS = z.REMARK,
                                  NOTES = z.NOTES,
                                  ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                  ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                  TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                  TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                  TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                  CORRELATION_ID = z.CORRELATION_ID,
                                  GST_RATE = z.GST_RATE,
                                  MERCHANT_NAME = z.MERCHANT_NAME,
                              }).ToList();


            //var memberinfo = dbcontext.TBL_FINAL_RDS_BOOKING.Where(x => x.WLP_ID == CurrentUser.USER_ID ).ToList().OrderByDescending(x => x.TRAN_DATE);
            IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
            grid.Columns.Add(model => model.WT_AUTO_CAN).Titled("Wt Auto Can").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);

            grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 9999999;


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
        [HttpGet]
        public FileResult ExportCancellationIndex(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_CANCELLATION> grid = CreateExportableGrid(MerchantUser, DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_CANCELLATION> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "AdminRDSCancellationReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }


        #region Refund Report
        public ActionResult RDSRefundReport()
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
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
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
        //public PartialViewResult RDSRefundReportGrid(string Whitelevel = "", string Distributor = "", string Merchant = "", string DateFrom = "", string Date_To = "")
        public PartialViewResult RDSRefundReportGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                //if (Merchant != "")
                if (MerchantUser != "" && DateFrom != "" && Date_To != "")
                {

                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.BOOKING_AMT.ToString().StartsWith(MerchantUser)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSRefundReportGrid", RDSbookinglist);
                }
                else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
                {

                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);                    
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSRefundReportGrid", RDSbookinglist);
                }
                else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.BOOKING_AMT.ToString().StartsWith(MerchantUser)) && x.TRAN_STATUS == "Failed"
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSRefundReportGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where y.MEMBER_ROLE == 5 && x.TRAN_STATUS == "Failed"
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo=index+1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSRefundReportGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> RDSRefundReportExportableGrid(string MerchantUser, string DateFrom, string Date_To)
        {
            var dbcontext = new DBContext();
            //if (Merchant != "")
            if (MerchantUser != "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
            {

                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser)) && x.TRAN_STATUS == "Failed"
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where y.MEMBER_ROLE == 5 && x.TRAN_STATUS == "Failed"
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 9999999;
                return grid;
            }
        }
        [HttpGet]
        public FileResult ExportRDSRefundReport(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = RDSRefundReportExportableGrid(MerchantUser, DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_RDS_BOOKING> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "AdminRDSRefundReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        #endregion

        #region UnMatch Rds Report

        public ActionResult RDSMisMatchRDSIdReport()
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
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
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

                //Response.Redirect(Url.Action("Index", "Login"));
                //return View();
            }
        }
        public PartialViewResult RDSMisMatchRDSIDReportGrid(string MerchantUser = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                //if (Merchant != "")
                if (MerchantUser!="" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID && (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.BOOKING_AMT.ToString().Contains(MerchantUser))
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo = index + 1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSMisMatchRDSIDReportGrid", RDSbookinglist);
                }
                else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID 
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo = index + 1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSMisMatchRDSIDReportGrid", RDSbookinglist);
                }
                else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
                {

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID && (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser) || x.BOOKING_AMT.ToString().Contains(MerchantUser))
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo = index + 1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSMisMatchRDSIDReportGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID
                                          select new
                                          {
                                              SLN = x.SLN,
                                              BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                              TRAN_ID = x.TRAN_ID,
                                              PNR = x.PNR,
                                              OPR_ID = x.OPR_ID,
                                              BOOKING_AMT = x.BOOKING_AMT,
                                              PG_CHARGE = x.PG_CHARGE,
                                              TRAN_DATE = x.TRAN_DATE,
                                              TRAN_STATUS = x.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = x.BOOKING_TIME,
                                              CURRENCY_TYPE = x.CURRENCY_TYPE,
                                              APP_CODE = x.APP_CODE,
                                              PAYMODE = x.PAYMODE,
                                              SECURITY_ID = x.SECURITY_ID,
                                              RU = x.RU,
                                              PAY_REQ = x.PAY_REQ,
                                              RET_RES = x.RET_RES,
                                              MER_RAIL_ID = y.RAIL_ID,
                                              PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                              REMARK = x.REMARKS,
                                              NOTES = x.NOTES,
                                              ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = x.CORRELATION_ID,
                                              GST_RATE = x.GST_RATE,
                                              MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                              COMPANY_NAME = y.COMPANY,
                                              COMPANY_GST = y.COMPANY_GST_NO,
                                          }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                          {
                                              SerialNo = index + 1,
                                              SLN = z.SLN,
                                              BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                              TRAN_ID = z.TRAN_ID,
                                              PNR = z.PNR,
                                              OPR_ID = z.OPR_ID,
                                              BOOKING_AMT = z.BOOKING_AMT,
                                              PG_CHARGE = z.PG_CHARGE,
                                              TRAN_DATE = z.TRAN_DATE,
                                              TRAN_STATUS = z.TRAN_STATUS,
                                              BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                              BOOKING_TIME = z.BOOKING_TIME,
                                              CURRENCY_TYPE = z.CURRENCY_TYPE,
                                              APP_CODE = z.APP_CODE,
                                              PAYMODE = z.PAYMODE,
                                              SECURITY_ID = z.SECURITY_ID,
                                              RU = z.RU,
                                              PAY_REQ = z.PAY_REQ,
                                              RET_RES = z.RET_RES,
                                              MER_RAIL_ID = z.MER_RAIL_ID,
                                              PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                              PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                              PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                              PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                              PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                              PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                              ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                              ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                              ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                              ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                              REMARKS = z.REMARK,
                                              NOTES = z.NOTES,
                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                              Company_Name = z.COMPANY_NAME,
                                              Company_GST = z.COMPANY_GST,
                                          }).ToList();
                    return PartialView("RDSMisMatchRDSIDReportGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public FileResult ExportRDSMisMatchRDSIdReport(string MerchantUser, string DateFrom, string Date_To)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportdRDSMisMatchRDSIdReportGrid(MerchantUser, DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_FINAL_RDS_BOOKING> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/unknown", "AdminRDSMisMatchReport.xlsx");
                //return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ////return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        private IGrid<TBL_FINAL_RDS_BOOKING> CreateExportdRDSMisMatchRDSIdReportGrid(string MerchantUser, string DateFrom, string Date_To)
        {
            var dbcontext = new DBContext();
            //if (Merchant != "")
            if (MerchantUser!="" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID && (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser))
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SerialNo = index + 1,
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SerialNo).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_STATUS).Titled("Booking Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_TRAN_STATUS).Titled("Booking Trn Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CURRENCY_TYPE).Titled("Currency Type ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.APP_CODE).Titled("App Code ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PAYMODE).Titled("Pay Mode ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 2000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
            else if (MerchantUser == "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SerialNo = index + 1,
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SerialNo).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_STATUS).Titled("Booking Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_TRAN_STATUS).Titled("Booking Trn Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CURRENCY_TYPE).Titled("Currency Type ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.APP_CODE).Titled("App Code ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PAYMODE).Titled("Pay Mode ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 2000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
            else if (MerchantUser != "" && DateFrom == "" && Date_To == "")
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where  x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID && (y.UName.StartsWith(MerchantUser) || y.MEMBER_MOBILE.StartsWith(MerchantUser) || y.MEMBER_NAME.StartsWith(MerchantUser) || y.COMPANY.StartsWith(MerchantUser) || y.COMPANY_GST_NO.StartsWith(MerchantUser) || y.ADDRESS.StartsWith(MerchantUser) || y.CITY.StartsWith(MerchantUser) || y.PIN.StartsWith(MerchantUser) || y.EMAIL_ID.StartsWith(MerchantUser) || y.AADHAAR_NO.StartsWith(MerchantUser) || y.PAN_NO.StartsWith(MerchantUser) || y.RAIL_ID.StartsWith(MerchantUser) || y.MEM_UNIQUE_ID.StartsWith(MerchantUser))
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SerialNo = index + 1,
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SerialNo).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_STATUS).Titled("Booking Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_TRAN_STATUS).Titled("Booking Trn Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CURRENCY_TYPE).Titled("Currency Type ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.APP_CODE).Titled("App Code ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PAYMODE).Titled("Pay Mode ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 2000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.TRAN_STATUS == "Success" && x.MER_RAIL_ID != x.BOOKING_MER_RAIL_ID
                                      select new
                                      {
                                          SLN = x.SLN,
                                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
                                          TRAN_ID = x.TRAN_ID,
                                          PNR = x.PNR,
                                          OPR_ID = x.OPR_ID,
                                          BOOKING_AMT = x.BOOKING_AMT,
                                          PG_CHARGE = x.PG_CHARGE,
                                          TRAN_DATE = x.TRAN_DATE,
                                          TRAN_STATUS = x.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = x.BOOKING_TIME,
                                          CURRENCY_TYPE = x.CURRENCY_TYPE,
                                          APP_CODE = x.APP_CODE,
                                          PAYMODE = x.PAYMODE,
                                          SECURITY_ID = x.SECURITY_ID,
                                          RU = x.RU,
                                          PAY_REQ = x.PAY_REQ,
                                          RET_RES = x.RET_RES,
                                          MER_RAIL_ID = y.RAIL_ID,
                                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
                                          REMARK = x.REMARKS,
                                          NOTES = x.NOTES,
                                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = x.CORRELATION_ID,
                                          GST_RATE = x.GST_RATE,
                                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
                                      }).AsEnumerable().Select((z, index) => new TBL_FINAL_RDS_BOOKING
                                      {
                                          SerialNo = index + 1,
                                          SLN = z.SLN,
                                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
                                          TRAN_ID = z.TRAN_ID,
                                          PNR = z.PNR,
                                          OPR_ID = z.OPR_ID,
                                          BOOKING_AMT = z.BOOKING_AMT,
                                          PG_CHARGE = z.PG_CHARGE,
                                          TRAN_DATE = z.TRAN_DATE,
                                          TRAN_STATUS = z.TRAN_STATUS,
                                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
                                          BOOKING_TIME = z.BOOKING_TIME,
                                          CURRENCY_TYPE = z.CURRENCY_TYPE,
                                          APP_CODE = z.APP_CODE,
                                          PAYMODE = z.PAYMODE,
                                          SECURITY_ID = z.SECURITY_ID,
                                          RU = z.RU,
                                          PAY_REQ = z.PAY_REQ,
                                          RET_RES = z.RET_RES,
                                          MER_RAIL_ID = z.MER_RAIL_ID,
                                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
                                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
                                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
                                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
                                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
                                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
                                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
                                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
                                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
                                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,
                                          REMARKS = z.REMARK,
                                          NOTES = z.NOTES,
                                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                          CORRELATION_ID = z.CORRELATION_ID,
                                          GST_RATE = z.GST_RATE,
                                          MERCHANT_NAME = z.MERCHANT_NAME,
                                      }).ToList();
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SerialNo).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_STATUS).Titled("Booking Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BOOKING_TRAN_STATUS).Titled("Booking Trn Status ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CURRENCY_TYPE).Titled("Currency Type ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.APP_CODE).Titled("App Code ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PAYMODE).Titled("Pay Mode ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Formatted("{0:yyyy-MM-dd}").Sortable(true);

                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 2000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }


        }
        #endregion
    }
}
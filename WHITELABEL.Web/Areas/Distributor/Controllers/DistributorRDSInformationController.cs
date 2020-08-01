using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorRDSInformationController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Distributor/DistributorRDSInformation
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Distributor";
                if (Session["DistributorUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["DistributorUserId"] != null)
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
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MObileNo = x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName + "-" + z.MObileNo
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid(string search = "",string DateFrom="" ,string Date_To="")
        {
            try
            {
                var dbcontext = new DBContext();
                if (search != "" && DateFrom != "" && Date_To != "")
                {

                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else if (search == "" && DateFrom != "" && Date_To != "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else if (search != "" && DateFrom == "" && Date_To == "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    //FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    //DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    //string From_TO = string.Empty;
                    //TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    //DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.MER_ID == mem_id
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);
                }
                else
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID 
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("IndexGrid", RDSbookinglist);

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> CreateExportableGrid(string search = "", string DateFrom = "", string Date_To = "")
        {
            //var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                  where x.DIST_ID == MemberCurrentUser.MEM_ID
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


            //var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
            //                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                      where x.DIST_ID == MemberCurrentUser.MEM_ID
            //                      select new
            //                      {
            //                          SLN = x.SLN,
            //                          BOOKING_GATEWAY = x.BOOKING_GATEWAY,
            //                          TRAN_ID = x.TRAN_ID,
            //                          PNR = x.PNR,
            //                          OPR_ID = x.OPR_ID,
            //                          BOOKING_AMT = x.BOOKING_AMT,
            //                          PG_CHARGE = x.PG_CHARGE,
            //                          TRAN_DATE = x.TRAN_DATE,
            //                          TRAN_STATUS = x.TRAN_STATUS,
            //                          BOOKING_TRAN_STATUS = x.BOOKING_TRAN_STATUS,
            //                          BOOKING_TIME = x.BOOKING_TIME,
            //                          CURRENCY_TYPE = x.CURRENCY_TYPE,
            //                          APP_CODE = x.APP_CODE,
            //                          PAYMODE = x.PAYMODE,
            //                          SECURITY_ID = x.SECURITY_ID,
            //                          RU = x.RU,
            //                          PAY_REQ = x.PAY_REQ,
            //                          RET_RES = x.RET_RES,
            //                          MER_RAIL_ID = y.RAIL_ID,
            //                          PG_CHARGE_APPLY = x.PG_CHARGE_APPLY,
            //                          PG_CHARGE_MAX_VAL = x.PG_CHARGE_MAX_VAL,
            //                          PG_CHARGE_LESS_THAN_2000 = x.PG_CHARGE_LESS_THAN_2000,
            //                          PG_CHARGE_GREATER_THAN_2000 = x.PG_CHARGE_GREATER_THAN_2000,
            //                          PG_CHARGE_GST_APPLY = x.PG_CHARGE_GST_APPLY,
            //                          PG_CHARGE_GST_VAL = x.PG_CHARGE_GST_VAL,
            //                          ADDN_CHARGE_APPLY = x.ADDN_CHARGE_APPLY,
            //                          ADDN_CHARGE_MAX_VAL = x.ADDN_CHARGE_MAX_VAL,
            //                          ADDN_CHARGE_AC = x.ADDN_CHARGE_AC,
            //                          ADDN_CHARGE_NON_AC = x.ADDN_CHARGE_NON_AC,
            //                          REMARK = x.REMARKS,
            //                          NOTES = x.NOTES,
            //                          ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
            //                          ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
            //                          TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                          TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
            //                          TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
            //                          CORRELATION_ID = x.CORRELATION_ID,
            //                          GST_RATE = x.GST_RATE,
            //                          MERCHANT_NAME = y.MEM_UNIQUE_ID,
            //                      }).AsEnumerable().Select(z => new TBL_FINAL_RDS_BOOKING
            //                      {
            //                          SLN = z.SLN,
            //                          BOOKING_GATEWAY = z.BOOKING_GATEWAY,
            //                          TRAN_ID = z.TRAN_ID,
            //                          PNR = z.PNR,
            //                          OPR_ID = z.OPR_ID,
            //                          BOOKING_AMT = z.BOOKING_AMT,
            //                          PG_CHARGE = z.PG_CHARGE,
            //                          TRAN_DATE = z.TRAN_DATE,
            //                          TRAN_STATUS = z.TRAN_STATUS,
            //                          BOOKING_TRAN_STATUS = z.BOOKING_TRAN_STATUS,
            //                          BOOKING_TIME = z.BOOKING_TIME,
            //                          CURRENCY_TYPE = z.CURRENCY_TYPE,
            //                          APP_CODE = z.APP_CODE,
            //                          PAYMODE = z.PAYMODE,
            //                          SECURITY_ID = z.SECURITY_ID,
            //                          RU = z.RU,
            //                          PAY_REQ = z.PAY_REQ,
            //                          RET_RES = z.RET_RES,
            //                          MER_RAIL_ID = z.MER_RAIL_ID,
            //                          PG_CHARGE_APPLY = z.PG_CHARGE_APPLY,
            //                          PG_CHARGE_MAX_VAL = z.PG_CHARGE_MAX_VAL,
            //                          PG_CHARGE_LESS_THAN_2000 = z.PG_CHARGE_LESS_THAN_2000,
            //                          PG_CHARGE_GREATER_THAN_2000 = z.PG_CHARGE_GREATER_THAN_2000,
            //                          PG_CHARGE_GST_APPLY = z.PG_CHARGE_GST_APPLY,
            //                          PG_CHARGE_GST_VAL = z.PG_CHARGE_GST_VAL,
            //                          ADDN_CHARGE_APPLY = z.ADDN_CHARGE_APPLY,
            //                          ADDN_CHARGE_MAX_VAL = z.ADDN_CHARGE_MAX_VAL,
            //                          ADDN_CHARGE_AC = z.ADDN_CHARGE_AC,
            //                          ADDN_CHARGE_NON_AC = z.ADDN_CHARGE_NON_AC,

            //                          ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
            //                          ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
            //                          TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                          TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
            //                          TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
            //                          CORRELATION_ID = z.CORRELATION_ID,
            //                          GST_RATE = z.GST_RATE,
            //                          MERCHANT_NAME = z.MERCHANT_NAME,
            //                      }).ToList();


            ////var memberinfo = dbcontext.TBL_FINAL_RDS_BOOKING.Where(x => x.WLP_ID == CurrentUser.USER_ID ).ToList().OrderByDescending(x => x.TRAN_DATE);
            //IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
            //grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //grid.Query = Request.QueryString;
            //grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BOOKING_GATEWAY).Titled("RDS Booking Gateway").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PNR).Titled("Pnr").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);


            //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
            ////grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
            ////grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
            ////grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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







            var dbcontext = new DBContext();
            if (search != "" && DateFrom != "" && Date_To != "")
            {

                long Dist = MemberCurrentUser.MEM_ID;
                long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else if (search == "" && DateFrom != "" && Date_To != "")
            {
                long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else if (search != "" && DateFrom == "" && Date_To == "")
            {
                long Dist = MemberCurrentUser.MEM_ID;
                long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                //FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                //DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                //string From_TO = string.Empty;
                //TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                //DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                //var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                //                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                //                      where x.DIST_ID == MemberCurrentUser.MEM_ID
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.MER_ID == mem_id
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else
            {

                long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(search);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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





        }
        [HttpGet]
        public FileResult ExportIndex(string search = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportableGrid(search, DateFrom, Date_To);
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

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }


        public ActionResult MerchantCancellationReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MObileNo = x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName + "-" + z.MObileNo +"-"+z.MEM_ID
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult MerchantCancellationIndexGrid(string search = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (search != "" && DateFrom != "" && Date_To != "")
                {

                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == Dist && x.MER_ID == mem_id && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantCancellationIndexGrid", RDSbookinglist);
                }
                else if (search == "" && DateFrom != "" && Date_To != "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == Dist &&  x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantCancellationIndexGrid", RDSbookinglist);
                }
                else if (search != "" && DateFrom == "" && Date_To == "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == Dist && x.MER_ID == mem_id
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
                    return PartialView("MerchantCancellationIndexGrid", RDSbookinglist);
                }
                else
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == Dist 
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
                    return PartialView("MerchantCancellationIndexGrid", RDSbookinglist);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private IGrid<TBL_FINAL_CANCELLATION> CreateExportCancellationableGrid(string search = "", string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID where x.DIST_ID==MemberCurrentUser.MEM_ID
            //                  select new
            //                  {
            //                      SLN = x.SLN,
            //                      //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
            //                      TRAN_ID = x.TRN_ID,
            //                      PNR = x.PNR_NO,
            //                      OPR_ID = x.OPR_ID,
            //                      BOOKING_AMT = x.REFUND_AMT,
            //                      //PG_CHARGE = x.PG_CHARGE,
            //                      TRAN_DATE = x.TRN_DATE,
            //                      TDR_CAN = x.TDR_CAN,
            //                      CANCELLATION_ID = x.CANCELLATION_ID,
            //                      CANCELLATION_STATUS = x.CANCELLATION_TYPE,
            //                      CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
            //                      SYSTEM_DATE = x.SYSTEM_DATE,
            //                      WT_AUTO_CAN = x.WT_AUTO_CAN,
            //                      //APP_CODE = x.APP_CODE,
            //                      //PAYMODE = x.PAYMODE,
            //                      //SECURITY_ID = x.SECURITY_ID,
            //                      //RU = x.RU,
            //                      //PAY_REQ = x.PAY_REQ,
            //                      //RET_RES = x.RET_RES,
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
            //                      REMARK = x.REMARKS,
            //                      NOTES = x.NOTES,
            //                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
            //                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
            //                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
            //                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
            //                      CORRELATION_ID = x.CORRELATION_ID,
            //                      GST_RATE = x.GST_RATE,
            //                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
            //                  }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
            //                  {
            //                      SLN = z.SLN,
            //                      //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
            //                      TRN_ID = z.TRAN_ID,
            //                      PNR_NO = z.PNR,
            //                      OPR_ID = z.OPR_ID,
            //                      REFUND_AMT = z.BOOKING_AMT,
            //                      //PG_CHARGE = z.PG_CHARGE,
            //                      TRN_DATE = z.TRAN_DATE,
            //                      TDR_CAN = z.TDR_CAN,
            //                      CANCELLATION_ID = z.CANCELLATION_ID,
            //                      CANCELLATION_TYPE = z.CANCELLATION_STATUS,
            //                      SYSTEM_DATE = z.SYSTEM_DATE,
            //                      WT_AUTO_CAN = z.WT_AUTO_CAN,
            //                      //CURRENCY_TYPE = z.CURRENCY_TYPE,
            //                      //APP_CODE = z.APP_CODE,
            //                      //PAYMODE = z.PAYMODE,
            //                      //SECURITY_ID = z.SECURITY_ID,
            //                      //RU = z.RU,
            //                      //PAY_REQ = z.PAY_REQ,
            //                      //RET_RES = z.RET_RES,
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
            //                      REMARKS = z.REMARK,
            //                      NOTES = z.NOTES,
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
            //IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(memberinfo);
            //grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //grid.Query = Request.QueryString;
            //grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.WT_AUTO_CAN).Titled("Wt Auto Can").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);

            //grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
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

            if (search != "" && DateFrom != "" && Date_To != "")
            {

                long Dist = MemberCurrentUser.MEM_ID;
                long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == Dist && x.MER_ID == mem_id && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else if (search == "" && DateFrom != "" && Date_To != "")
            {
                long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == Dist && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else if (search != "" && DateFrom == "" && Date_To == "")
            {
                long Dist = MemberCurrentUser.MEM_ID;
                long mem_id = long.Parse(search);

                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == Dist && x.MER_ID == mem_id
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else
            {
                long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(search);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == Dist
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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

        }
        [HttpGet]
        public FileResult ExportCancellationIndex(string search = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_CANCELLATION> grid = CreateExportCancellationableGrid(search, DateFrom, Date_To);
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

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }


        #region Get Auto Suggestion for RDS Booking and Cancellation Report
        public ActionResult DistMerchantRdsBooking()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MobileNo = x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName + "-" + z.MobileNo + "-" + z.MEM_ID
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DistributorMerchantRdsBookingIndexGrid(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MerID != "" && DateFrom != "" && Date_To != "")
                {


                    //long mem_id = long.Parse(MerID);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                          //(y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorMerchantRdsBookingIndexGrid", RDSbookinglist);
                }
                else if (MerID == "" && DateFrom != "" && Date_To != "")
                {
                    //long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorMerchantRdsBookingIndexGrid", RDSbookinglist);
                }
                else if (MerID != "" && DateFrom == "" && Date_To == "")
                {
                    ////long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(MerID);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
                                          //where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorMerchantRdsBookingIndexGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorMerchantRdsBookingIndexGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> DistMerchantRdsBookingExportableGrid(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                  where x.DIST_ID == MemberCurrentUser.MEM_ID
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

            if (MerID != "" && DateFrom != "" && Date_To != "")
            {
                
                //long mem_id = long.Parse(MerID);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
                                      //(y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Formatted("{0:yyyy-MM-dd}").Titled("Tranx Date").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else if (MerID == "" && DateFrom != "" && Date_To != "")
            {
                //long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(search);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Formatted("{0:yyyy-MM-dd}").Titled("Tranx Date").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else if (MerID != "" && DateFrom == "" && Date_To == "")
            {
                ////long Dist = MemberCurrentUser.MEM_ID;
                //long mem_id = long.Parse(MerID);
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
                                      //where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Formatted("{0:yyyy-MM-dd}").Titled("Tranx Date").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.WLP_ID == MemberCurrentUser.MEM_ID
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
                IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
                grid.Columns.Add(model => model.TRAN_DATE).Formatted("{0:yyyy-MM-dd}").Titled("Tranx Date").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
                grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
        }
        [HttpGet]
        public FileResult DistMerchantRdsBookingExportIndex(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = DistMerchantRdsBookingExportableGrid(MerID, DateFrom, Date_To);
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

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetDistMerchantMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.INTRODUCER == MemberCurrentUser.MEM_ID
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
        #endregion

        #region Cancellation Report
        public ActionResult MerchantWiseRDSCancellationReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MobileNo = x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName + "-" + z.MobileNo + "-" + z.MEM_ID
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult MerchantWIseIndexCancellationGrid(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            try
            {

                var dbcontext = new DBContext();
                if (MerID != "" && DateFrom != "" && Date_To != "")
                {
                    long MerId = 0;
                    //long.TryParse(MerID, out MerId);

                    long DistId = 0;

                    long WLPId = 0;
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == MerId && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantWIseIndexCancellationGrid", RDSbookinglist);
                }
                else if (MerID == "" && DateFrom != "" && Date_To != "")
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
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantWIseIndexCancellationGrid", RDSbookinglist);
                }
                else if (MerID != "" && DateFrom == "" && Date_To == "")
                {
                    long MerId = 0;
                    //long.TryParse(MerID, out MerId);


                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == MerId
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
                                          //(y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
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
                    return PartialView("MerchantWIseIndexCancellationGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID
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
                    return PartialView("MerchantWIseIndexCancellationGrid", RDSbookinglist);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_CANCELLATION> CreateMerchantWiseCancellationableGrid(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                  where x.WLP_ID == MemberCurrentUser.MEM_ID
            //                  select new
            //                  {
            //                      SLN = x.SLN,
            //                      //BOOKING_GATEWAY = x.BOOKING_GATEWAY,
            //                      TRAN_ID = x.TRN_ID,
            //                      PNR = x.PNR_NO,
            //                      OPR_ID = x.OPR_ID,
            //                      BOOKING_AMT = x.REFUND_AMT,
            //                      //PG_CHARGE = x.PG_CHARGE,
            //                      TRAN_DATE = x.TRN_DATE,
            //                      TDR_CAN = x.TDR_CAN,
            //                      CANCELLATION_ID = x.CANCELLATION_ID,
            //                      CANCELLATION_STATUS = x.CANCELLATION_TYPE,
            //                      CANCELLATION_AGST_MER_RAIL_ID = x.CANCELLATION_AGST_MER_RAIL_ID,
            //                      SYSTEM_DATE = x.SYSTEM_DATE,
            //                      WT_AUTO_CAN = x.WT_AUTO_CAN,
            //                      //APP_CODE = x.APP_CODE,
            //                      //PAYMODE = x.PAYMODE,
            //                      //SECURITY_ID = x.SECURITY_ID,
            //                      //RU = x.RU,
            //                      //PAY_REQ = x.PAY_REQ,
            //                      //RET_RES = x.RET_RES,
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
            //                      REMARK = x.REMARKS,
            //                      NOTES = x.NOTES,
            //                      ADDN_CHARGE_GST_APPLY = x.ADDN_CHARGE_GST_APPLY,
            //                      ADDN_CHARGE_GST_VAL = x.ADDN_CHARGE_GST_VAL,
            //                      TOTAL_NET_PAYBLE_WITHOUT_GST = x.TOTAL_NET_PAYBLE_WITHOUT_GST,
            //                      TOTAL_NET_PAYBLE_GST = x.TOTAL_NET_PAYBLE_GST,
            //                      TOTAL_NET_PAYBLE = x.TOTAL_NET_PAYBLE,
            //                      CORRELATION_ID = x.CORRELATION_ID,
            //                      GST_RATE = x.GST_RATE,
            //                      MERCHANT_NAME = y.MEM_UNIQUE_ID,
            //                  }).AsEnumerable().Select(z => new TBL_FINAL_CANCELLATION
            //                  {
            //                      SLN = z.SLN,
            //                      //BOOKING_GATEWAY = z.BOOKING_GATEWAY,
            //                      TRN_ID = z.TRAN_ID,
            //                      PNR_NO = z.PNR,
            //                      OPR_ID = z.OPR_ID,
            //                      REFUND_AMT = z.BOOKING_AMT,
            //                      //PG_CHARGE = z.PG_CHARGE,
            //                      TRN_DATE = z.TRAN_DATE,
            //                      TDR_CAN = z.TDR_CAN,
            //                      CANCELLATION_ID = z.CANCELLATION_ID,
            //                      CANCELLATION_TYPE = z.CANCELLATION_STATUS,
            //                      SYSTEM_DATE = z.SYSTEM_DATE,
            //                      WT_AUTO_CAN = z.WT_AUTO_CAN,
            //                      //CURRENCY_TYPE = z.CURRENCY_TYPE,
            //                      //APP_CODE = z.APP_CODE,
            //                      //PAYMODE = z.PAYMODE,
            //                      //SECURITY_ID = z.SECURITY_ID,
            //                      //RU = z.RU,
            //                      //PAY_REQ = z.PAY_REQ,
            //                      //RET_RES = z.RET_RES,
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
            //                      REMARKS = z.REMARK,
            //                      NOTES = z.NOTES,
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
            //IGrid<TBL_FINAL_CANCELLATION> grid = new Grid<TBL_FINAL_CANCELLATION>(memberinfo);
            //grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //grid.Query = Request.QueryString;
            //grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.WT_AUTO_CAN).Titled("Wt Auto Can").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TDR_CAN).Titled("TDR Can").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.ADDN_CHARGE_MAX_VAL).Titled("Addn. Charges Apply").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_WITHOUT_GST).Titled("Total Netamt Without GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE_GST).Titled("Total Netamt With GST ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TOTAL_NET_PAYBLE).Titled("Total Netamt").Filterable(true).Sortable(true);

            //grid.Pager = new GridPager<TBL_FINAL_CANCELLATION>(grid);
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

            if (MerID != "" && DateFrom != "" && Date_To != "")
            {
                long MerId = 0;
                //long.TryParse(MerID, out MerId);

                long DistId = 0;

                long WLPId = 0;
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == MerId && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID)) && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else if (MerID == "" && DateFrom != "" && Date_To != "")
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
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else if (MerID != "" && DateFrom == "" && Date_To == "")
            {
                long MerId = 0;
                //long.TryParse(MerID, out MerId);


                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      //where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == MerId
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID && (y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
                                      //(y.UName.StartsWith(MerID) || y.MEMBER_MOBILE.StartsWith(MerID) || y.MEMBER_NAME.StartsWith(MerID) || y.COMPANY.StartsWith(MerID) || y.COMPANY_GST_NO.StartsWith(MerID) || y.ADDRESS.StartsWith(MerID) || y.CITY.StartsWith(MerID) || y.PIN.StartsWith(MerID) || y.EMAIL_ID.Contains(MerID) || y.AADHAAR_NO.StartsWith(MerID) || y.PAN_NO.StartsWith(MerID) || y.RAIL_ID.StartsWith(MerID) || y.FACEBOOK_ID.StartsWith(MerID) || y.WEBSITE_NAME.StartsWith(MerID) || y.MEM_UNIQUE_ID.StartsWith(MerID))
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
            else
            {
                var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                      join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                      where x.DIST_ID == MemberCurrentUser.MEM_ID
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
                grid.Columns.Add(model => model.CANCELLATION_ID).Titled("Cancellation Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.CANCELLATION_TYPE).Titled("Cancellation Type").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_ID).Titled("Txn Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.PNR_NO).Titled("Pnr").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("Opr Id").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REFUND_AMT).Titled("Refund Amt ").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
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
        }
        [HttpGet]
        public FileResult ExportMerchantWiseCancellationIndex(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_CANCELLATION> grid = CreateMerchantWiseCancellationableGrid(MerID, DateFrom, Date_To);
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

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        #endregion

        #region Distributor Refund Report
        public ActionResult DistributorRDSRefundReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MObileNo = x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName + "-" + z.MObileNo
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DistributorRDSRefundReportGrid(string search = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (search != "" && DateFrom != "" && Date_To != "")
                {

                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS== "Failed"
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorRDSRefundReportGrid", RDSbookinglist);
                }
                else if (search == "" && DateFrom != "" && Date_To != "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorRDSRefundReportGrid", RDSbookinglist);
                }
                else if (search != "" && DateFrom == "" && Date_To == "")
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(search);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.MER_ID == mem_id && x.TRAN_STATUS == "Failed"
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorRDSRefundReportGrid", RDSbookinglist);
                }
                else
                {
                    long Dist = MemberCurrentUser.MEM_ID;
                    //long mem_id = long.Parse(search);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.DIST_ID == MemberCurrentUser.MEM_ID && x.TRAN_STATUS == "Failed"
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

                                              ADDN_CHARGE_GST_APPLY = z.ADDN_CHARGE_GST_APPLY,
                                              ADDN_CHARGE_GST_VAL = z.ADDN_CHARGE_GST_VAL,
                                              TOTAL_NET_PAYBLE_WITHOUT_GST = z.TOTAL_NET_PAYBLE_WITHOUT_GST,
                                              TOTAL_NET_PAYBLE_GST = z.TOTAL_NET_PAYBLE_GST,
                                              TOTAL_NET_PAYBLE = z.TOTAL_NET_PAYBLE,
                                              CORRELATION_ID = z.CORRELATION_ID,
                                              GST_RATE = z.GST_RATE,
                                              MERCHANT_NAME = z.MERCHANT_NAME,
                                          }).ToList();
                    return PartialView("DistributorRDSRefundReportGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> DistributorRDSRefundReportExportableGrid()
        {
            var dbcontext = new DBContext();
            //var memberinfo = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
            //                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
            //                  where x.DIST_ID == MemberCurrentUser.MEM_ID
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
            var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                  join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                  where x.DIST_ID == MemberCurrentUser.MEM_ID
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
            IGrid<TBL_FINAL_RDS_BOOKING> grid = new Grid<TBL_FINAL_RDS_BOOKING>(RDSbookinglist);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SLN).Titled("Sln").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MERCHANT_NAME).Titled("User Name").Filterable(true).Sortable(true);
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
            grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Formatted("{0:yyyy-MM-dd}").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BOOKING_AMT).Titled("Booking Amt ").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.PG_CHARGE).Titled("PG Charges").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_FINAL_RDS_BOOKING>(grid);
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
        public FileResult ExportDistributorRDSRefundReport()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = DistributorRDSRefundReportExportableGrid();
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

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }
        #endregion
    }
}
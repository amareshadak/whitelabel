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
    public class MemberRDSInformationController : AdminBaseController
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

        // GET: Admin/MemberRDSInformation
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MobileNo=x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName+"-"+z.MobileNo + "-" + z.MEM_ID
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
        public PartialViewResult IndexGrid(string Distributor="", string search = "", string DateFrom = "", string Date_To = "")
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
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                                          where x.WLP_ID==MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                    return PartialView("IndexGrid", "");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> CreateExportableGrid()
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
            grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportableGrid();
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
        public JsonResult GetMerchantListInfromation(long Disid)
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
                                     Mobileno=x.MEMBER_MOBILE
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName+"-"+z.Mobileno+"-"+z.MEM_ID
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
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
             return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }

        public PartialViewResult IndexCancellationGrid(string Distributor = "", string search = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (search != "" && DateFrom != "" && Date_To != "")
                {
                    long MerId = 0;
                    long.TryParse(search, out MerId);

                    long DistId = 0;
                    long.TryParse(Distributor, out DistId);
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
                                          where x.DIST_ID == DistId && x.MER_ID == MerId && x.TRN_DATE>= Date_From_Val && x.TRN_DATE<= Date_To_Val
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
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else if (search == "" && DateFrom != "" && Date_To != "")
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
                                          where x.WLP_ID==MemberCurrentUser.MEM_ID && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else if (search != "" && DateFrom == "" && Date_To == "")
                {
                    long MerId = 0;
                    long.TryParse(search, out MerId);

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID== MerId
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
                    return PartialView("IndexCancellationGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID==MemberCurrentUser.MEM_ID
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
                              join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID where x.WLP_ID==MemberCurrentUser.MEM_ID
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
            grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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
        public FileResult ExportCancellationIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportableGrid();
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


        public ActionResult MerchantRdsBooking()
        {
            if (Session["WhiteLevelUserId"] != null)
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
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
             return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        public PartialViewResult MerchantRdsBookingIndexGrid( string MerID = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (MerID != "" && DateFrom != "" && Date_To != "")
                {

                    
                    long mem_id = long.Parse(MerID);
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                    return PartialView("MerchantRdsBookingIndexGrid", RDSbookinglist);
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
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val
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
                    return PartialView("MerchantRdsBookingIndexGrid", RDSbookinglist);
                }
                else if (MerID != "" && DateFrom == "" && Date_To == "")
                {
                    //long Dist = MemberCurrentUser.MEM_ID;
                    long mem_id = long.Parse(MerID);                  
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID== mem_id
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
                    return PartialView("MerchantRdsBookingIndexGrid", RDSbookinglist);
                }
                else
                {
                    return PartialView("MerchantRdsBookingIndexGrid", "");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> MerchantRdsBookingExportableGrid()
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
            grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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
        public FileResult MerchantRdsBookingExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = CreateExportableGrid();
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
        public async Task<JsonResult> GetMerchantMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID
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



        public ActionResult MerchantRDSCancellationReport()
        {
            if (Session["WhiteLevelUserId"] != null)
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
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
             return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }

        public PartialViewResult MerchantIndexCancellationGrid(string MerID = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
            
                var dbcontext = new DBContext();
                if (MerID != "" && DateFrom != "" && Date_To != "")
                {
                    long MerId = 0;
                    long.TryParse(MerID, out MerId);

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
                                          where x.WLP_ID== MemberCurrentUser.MEM_ID && x.MER_ID == MerId && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantIndexCancellationGrid", RDSbookinglist);
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
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.TRN_DATE >= Date_From_Val && x.TRN_DATE <= Date_To_Val
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
                    return PartialView("MerchantIndexCancellationGrid", RDSbookinglist);
                }
                else if (MerID != "" && DateFrom == "" && Date_To == "")
                {
                    long MerId = 0;
                    long.TryParse(MerID, out MerId);
                   

                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID== MerId
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
                    return PartialView("MerchantIndexCancellationGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_CANCELLATION
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID==MemberCurrentUser.MEM_ID
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
                    return PartialView("MerchantIndexCancellationGrid", RDSbookinglist);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_FINAL_CANCELLATION> CreateMerchantCancellationableGrid()
        {
            var dbcontext = new DBContext();
            var memberinfo = (from x in dbcontext.TBL_FINAL_CANCELLATION
                              join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                              where x.WLP_ID == MemberCurrentUser.MEM_ID
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
            grid.Columns.Add(model => model.TRN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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
        public FileResult ExportMerchantCancellationIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_CANCELLATION> grid = CreateMerchantCancellationableGrid();
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


        #region GEt Refund List
        public ActionResult MemberRDSRefundReport()
        {
            if (Session["WhiteLevelUserId"] != null)
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
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
             return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        public PartialViewResult MemberRDSRefundReportGrid(string Distributor = "", string search = "", string DateFrom = "", string Date_To = "")
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
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.MER_ID == mem_id && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
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
                    return PartialView("MemberRDSRefundReportGrid", RDSbookinglist);
                }
                else if (search == "" && DateFrom != "" && Date_To != "")
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
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID && x.TRAN_DATE >= Date_From_Val && x.TRAN_DATE <= Date_To_Val && x.TRAN_STATUS == "Failed"
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
                    return PartialView("MemberRDSRefundReportGrid", RDSbookinglist);
                }
                else
                {
                    var RDSbookinglist = (from x in dbcontext.TBL_FINAL_RDS_BOOKING
                                          join y in dbcontext.TBL_MASTER_MEMBER on x.MER_ID equals y.MEM_ID
                                          where x.WLP_ID == MemberCurrentUser.MEM_ID  && x.TRAN_STATUS == "Failed"
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
                    return PartialView("MemberRDSRefundReportGrid", RDSbookinglist);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private IGrid<TBL_FINAL_RDS_BOOKING> MemberRDSRefundReportExportableGrid()
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
            grid.Columns.Add(model => model.TRAN_DATE).Titled("Tranx Date").Filterable(true).Sortable(true);
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
        public FileResult ExportMemberRDSRefundReport()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_FINAL_RDS_BOOKING> grid = MemberRDSRefundReportExportableGrid();
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
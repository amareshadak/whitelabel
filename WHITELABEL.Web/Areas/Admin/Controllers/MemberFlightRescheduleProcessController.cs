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
    public class MemberFlightRescheduleProcessController : AdminBaseController
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
        // GET: Admin/MemberFlightRescheduleProcess
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
        public PartialViewResult IndexGrid(string SearchVal = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (SearchVal != "")
                {
                    var Member_Info = (from x in dbcontext.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                       where x.TICKET_NO.StartsWith(SearchVal) || x.REF_NO.StartsWith(SearchVal) ||
                                       x.FROM_AIRPORT.StartsWith(SearchVal) || x.TO_AIRPORT.StartsWith(SearchVal) 
                                       select new
                                       {
                                           TicketNo=x.TICKET_NO,
                                           PNR=x.REF_NO,
                                           BookingDate=x.BOOKING_DATE,
                                           Source = x.FROM_AIRPORT,
                                           Destination =x.TO_AIRPORT,
                                           FlightNo=x.FLIGHT_NO,
                                           No_Of_Pnsg=x.NO_OF_ADULT+"-"+x.NO_OF_CHILD+"-"+x.NO_OF_INFANT,
                                           Total_FlightAmt=x.TOTAL_FLIGHT_AMT,
                                           TravellingClass=(x.MAIN_CLASS=="Y"?"Economy": x.MAIN_CLASS == "C" ? "Business" : x.MAIN_CLASS == "W" ? "Premium Economy" : "First Class"),
                                           RescheduleMsg=x.RESCHEDULE_REPLY,
                                           RescheduleRemark = x.RESCHEDULE_REMARK,
                                           Reschedule_Date =x.RESCHEDULE_DATE,
                                           SLN=x.SLN,
                                           MEM_ID=x.MEM_ID,
                                           DIST_ID=x.DIST_ID,
                                           WLP_ID=x.WLP_ID,
                                           CORelationID=x.CORELATION_ID
                                       }).AsEnumerable().Select((z, index) => new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           DIST_ID=z.DIST_ID,
                                           WLP_ID=z.WLP_ID,
                                           FROM_AIRPORT=z.Source+"->"+z.Destination,
                                           TO_AIRPORT=z.No_Of_Pnsg,
                                           FLIGHT_NO=z.FlightNo,
                                           BOOKING_DATE=z.BookingDate,
                                           PNR=z.PNR,
                                           TICKET_NO=z.TicketNo,
                                           CORELATION_ID=z.CORelationID,
                                           TOTAL_FLIGHT_AMT=z.Total_FlightAmt,
                                           MAIN_CLASS=z.TravellingClass,                                           
                                           RESCHEDULE_DATE=z.Reschedule_Date,
                                           RESCHEDULE_REMARK=z.RescheduleRemark,
                                           RESCHEDULE_REPLY=z.RescheduleMsg
                                       }).ToList();
                    return PartialView("IndexGrid", Member_Info);
                }
                else
                {
                    var Member_Info = (from x in dbcontext.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                       select new
                                       {
                                           TicketNo = x.TICKET_NO,
                                           PNR = x.REF_NO,
                                           BookingDate = x.BOOKING_DATE,
                                           Source = x.FROM_AIRPORT,
                                           Destination = x.TO_AIRPORT,
                                           FlightNo = x.FLIGHT_NO,
                                           No_Of_Pnsg = x.NO_OF_ADULT + "-" + x.NO_OF_CHILD + "-" + x.NO_OF_INFANT,
                                           Total_FlightAmt = x.TOTAL_FLIGHT_AMT,
                                           TravellingClass = (x.MAIN_CLASS == "Y" ? "Economy" : x.MAIN_CLASS == "C" ? "Business" : x.MAIN_CLASS == "W" ? "Premium Economy" : "First Class"),
                                           RescheduleMsg = x.RESCHEDULE_REPLY,
                                           Reschedule_Date = x.RESCHEDULE_DATE,
                                           SLN = x.SLN,
                                           MEM_ID = x.MEM_ID,
                                           DIST_ID = x.DIST_ID,
                                           WLP_ID = x.WLP_ID,
                                           CORelationID = x.CORELATION_ID,
                                           RescheduleRemark = x.RESCHEDULE_REMARK,
                                       }).AsEnumerable().Select((z, index) => new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           DIST_ID = z.DIST_ID,
                                           WLP_ID = z.WLP_ID,
                                           FROM_AIRPORT = z.Source + "->" + z.Destination,
                                           TO_AIRPORT = z.No_Of_Pnsg,
                                           FLIGHT_NO = z.FlightNo,
                                           BOOKING_DATE = z.BookingDate,
                                           PNR = z.PNR,
                                           TICKET_NO = z.TicketNo,
                                           CORELATION_ID = z.CORelationID,
                                           TOTAL_FLIGHT_AMT = z.Total_FlightAmt,
                                           MAIN_CLASS = z.TravellingClass,
                                           RESCHEDULE_REPLY = z.RescheduleMsg,
                                           RESCHEDULE_REMARK = z.RescheduleRemark,
                                           RESCHEDULE_DATE = z.Reschedule_Date,                                           
                                       }).ToList();
                    return PartialView("IndexGrid", Member_Info);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult FetchFlightMarkUpByMember(string SlnValue = "")
        {
            try
            {
                if (SlnValue != "")
                {
                    var db = new DBContext();
                    long Sln = 0;
                    long.TryParse(SlnValue, out Sln);
                    //var GettxnInfo = db.TBL_FLIGHT_MARKUP.FirstOrDefault(x => x.SLN == Sln);
                    var Member_Info = (from x in db.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                      
                                       where x.SLN == Sln
                                       select new
                                       {
                                           SLN = x.SLN,
                                           MEM_ID = x.MEM_ID,
                                           RESCHEDULE_REPLYDATE=x.RESCHEDULE_DATE
                                       }).AsEnumerable().Select((z, index) => new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                       {
                                           Serial_No = index + 1,
                                           SLN = z.SLN,
                                           MEM_ID = z.MEM_ID,
                                           RESCHEDULE_DATE=z.RESCHEDULE_REPLYDATE
                                       }).FirstOrDefault();

                    return Json(new { Result = Member_Info, Status = "0" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { Result = "Please Contact to Administrator", Status = "1" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Please Contact to Administrator", Status = "1" }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        [HttpPost]
        public JsonResult POSTTAGFLIGHT_RESCHEDULEING(FlightRescheduleViewModel objReschedule)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var getMArkup = db.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS.FirstOrDefault(x => x.SLN == objReschedule.SLN);
                    if (getMArkup != null)
                    {
                        getMArkup.RESCHEDULE_REPLY_DATE = DateTime.Now;
                        getMArkup.RESCHEDULE_REPLY = objReschedule.RescheduleResplyMsg;                        
                        db.Entry(getMArkup).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Reschedule reply is done.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Flight Reschedule is Not Done. Please contact to your Administrator.", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json("Please try again later", JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
        }


    }
}
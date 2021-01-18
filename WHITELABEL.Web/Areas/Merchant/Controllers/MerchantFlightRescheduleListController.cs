using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using System.Data.Entity.Core;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;
using System.Threading;
using System.Globalization;
using easebuzz_.net;
using System.Text;
using System.Security.Cryptography;


namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantFlightRescheduleListController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
       
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant";

                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;
                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
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
        // GET: Merchant/MerchantFlightRescheduleList
        public ActionResult Index()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                
                    initpage();
                    return View();
                
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        //public PartialViewResult IndexGrid(string SearchVal = "")
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                var Member_Info = (from x in dbcontext.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                                   join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                   where x.MEM_ID==CurrentMerchant.MEM_ID
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
                                       Reschedule_Reply_Date = x.RESCHEDULE_REPLY_DATE,
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
                                       RESCHEDULE_REPLY_DATE=z.Reschedule_Reply_Date
                                   }).ToList();
                return PartialView("IndexGrid", Member_Info);
                //if (SearchVal != "")
                //{
                //    var Member_Info = (from x in dbcontext.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                //                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                //                       where x.TICKET_NO.StartsWith(SearchVal) || x.REF_NO.StartsWith(SearchVal) ||
                //                       x.FROM_AIRPORT.StartsWith(SearchVal) || x.TO_AIRPORT.StartsWith(SearchVal)
                //                       select new
                //                       {
                //                           TicketNo = x.TICKET_NO,
                //                           PNR = x.REF_NO,
                //                           BookingDate = x.BOOKING_DATE,
                //                           Source = x.FROM_AIRPORT,
                //                           Destination = x.TO_AIRPORT,
                //                           FlightNo = x.FLIGHT_NO,
                //                           No_Of_Pnsg = x.NO_OF_ADULT + "-" + x.NO_OF_CHILD + "-" + x.NO_OF_INFANT,
                //                           Total_FlightAmt = x.TOTAL_FLIGHT_AMT,
                //                           TravellingClass = (x.MAIN_CLASS == "Y" ? "Economy" : x.MAIN_CLASS == "C" ? "Business" : x.MAIN_CLASS == "W" ? "Premium Economy" : "First Class"),
                //                           RescheduleMsg = x.RESCHEDULE_REPLY,
                //                           RescheduleRemark = x.RESCHEDULE_REMARK,
                //                           Reschedule_Date = x.RESCHEDULE_DATE,
                //                           SLN = x.SLN,
                //                           MEM_ID = x.MEM_ID,
                //                           DIST_ID = x.DIST_ID,
                //                           WLP_ID = x.WLP_ID,
                //                           CORelationID = x.CORELATION_ID
                //                       }).AsEnumerable().Select((z, index) => new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                //                       {
                //                           Serial_No = index + 1,
                //                           SLN = z.SLN,
                //                           MEM_ID = z.MEM_ID,
                //                           DIST_ID = z.DIST_ID,
                //                           WLP_ID = z.WLP_ID,
                //                           FROM_AIRPORT = z.Source + "->" + z.Destination,
                //                           TO_AIRPORT = z.No_Of_Pnsg,
                //                           FLIGHT_NO = z.FlightNo,
                //                           BOOKING_DATE = z.BookingDate,
                //                           PNR = z.PNR,
                //                           TICKET_NO = z.TicketNo,
                //                           CORELATION_ID = z.CORelationID,
                //                           TOTAL_FLIGHT_AMT = z.Total_FlightAmt,
                //                           MAIN_CLASS = z.TravellingClass,
                //                           RESCHEDULE_DATE = z.Reschedule_Date,
                //                           RESCHEDULE_REMARK = z.RescheduleRemark,
                //                           RESCHEDULE_REPLY = z.RescheduleMsg
                //                       }).ToList();
                //    return PartialView("IndexGrid", Member_Info);
                //}
                //else
                //{
                //    var Member_Info = (from x in dbcontext.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                //                       join y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                //                       select new
                //                       {
                //                           TicketNo = x.TICKET_NO,
                //                           PNR = x.REF_NO,
                //                           BookingDate = x.BOOKING_DATE,
                //                           Source = x.FROM_AIRPORT,
                //                           Destination = x.TO_AIRPORT,
                //                           FlightNo = x.FLIGHT_NO,
                //                           No_Of_Pnsg = x.NO_OF_ADULT + "-" + x.NO_OF_CHILD + "-" + x.NO_OF_INFANT,
                //                           Total_FlightAmt = x.TOTAL_FLIGHT_AMT,
                //                           TravellingClass = (x.MAIN_CLASS == "Y" ? "Economy" : x.MAIN_CLASS == "C" ? "Business" : x.MAIN_CLASS == "W" ? "Premium Economy" : "First Class"),
                //                           RescheduleMsg = x.RESCHEDULE_REPLY,
                //                           Reschedule_Date = x.RESCHEDULE_DATE,
                //                           SLN = x.SLN,
                //                           MEM_ID = x.MEM_ID,
                //                           DIST_ID = x.DIST_ID,
                //                           WLP_ID = x.WLP_ID,
                //                           CORelationID = x.CORELATION_ID,
                //                           RescheduleRemark = x.RESCHEDULE_REMARK,
                //                       }).AsEnumerable().Select((z, index) => new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS
                //                       {
                //                           Serial_No = index + 1,
                //                           SLN = z.SLN,
                //                           MEM_ID = z.MEM_ID,
                //                           DIST_ID = z.DIST_ID,
                //                           WLP_ID = z.WLP_ID,
                //                           FROM_AIRPORT = z.Source + "->" + z.Destination,
                //                           TO_AIRPORT = z.No_Of_Pnsg,
                //                           FLIGHT_NO = z.FlightNo,
                //                           BOOKING_DATE = z.BookingDate,
                //                           PNR = z.PNR,
                //                           TICKET_NO = z.TicketNo,
                //                           CORELATION_ID = z.CORelationID,
                //                           TOTAL_FLIGHT_AMT = z.Total_FlightAmt,
                //                           MAIN_CLASS = z.TravellingClass,
                //                           RESCHEDULE_REPLY = z.RescheduleMsg,
                //                           RESCHEDULE_REMARK = z.RescheduleRemark,
                //                           RESCHEDULE_DATE = z.Reschedule_Date,
                //                       }).ToList();
                //    return PartialView("IndexGrid", Member_Info);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
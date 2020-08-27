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
using static WHITELABEL.Web.Helper.Tek_TravelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using System.Globalization;
using WHITELABEL.Web.DTO.FlightApi;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantFlightDetailsController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        private DBContext _db;
        public MerchantFlightDetailsController()
        {
            _db = new DBContext();
        }


        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Merchant";
                if (Session["MerchantUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Merchant/MerchantFlightDetails
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public ActionResult GetAllAirportName()
        {
            try
            {
                var db = new DBContext();
                //var airportlist = db.TBL_AIRPORT_DETAILS.Where(x => x.CITYNAME.Contains(pretext)).ToList();
                var airportlist = db.TBL_AIRPORT_DETAILS.Select(z => new
                {
                    ID = z.ID,
                    CITYCODE = z.CITYCODE,
                    CITYNAME = z.CITYNAME + " " + z.CITYCODE,
                    COUNTRYCODE = z.COUNTRYCODE,
                    AIRPORT_TYPE = z.AIRPORT_TYPE,
                    ISACTIVE = z.ISACTIVE
                }).ToList();
                return new JsonResult { Data = airportlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //[Route("Merchant/MerchantFlightDetails/GetSearchResult/{Tripmode}/{FromCityCode}/{TOAirportCode}/{FromDate}/{ToDate}/{TravelType}/{Adult}/{Child}/{Infant}")]
        //[Route("Merchant/MerchantFlightDetails/GetSearchResult/{Tripmode}/{FromCityCode}/{TOAirportCode}/{FromDate}/{ToDate}/{TravelType}/{Adult}/{Child}/{Infant}")]
        public ActionResult GetSearchResult(string Tripmode, string FromCityCode, string TOAirportCode, string FromDate, string ToDate, string TravelType, string Adult, string Child, string Infant)
        {
            ViewBag.Tripmode = Tripmode;
            ViewBag.FromCityCode = FromCityCode;
            ViewBag.TOAirportCode = TOAirportCode;
            //DateTime FromDtaeCheckFormat= Convert.ToDateTime(FromDate);
            //string DateFromValue = FromDtaeCheckFormat.ToString("dd/MM/yyyy");
            //            ViewBag.FromDate = DateFromValue;
            ViewBag.FromDate = FromDate;
            //ViewBag.FromDate = "06/23/2020";
            ViewBag.ToDate = ToDate;
            ViewBag.TravelType = TravelType;
            ViewBag.Adult = Adult;
            ViewBag.Child = Child;
            ViewBag.Infant = Infant;
            return View("~/Areas/Merchant/Views/MerchantFlightDetails/GetSearchResult.cshtml");
        }

        [HttpPost]
        public JsonResult SerachFlight(FlightSearchParameter objserch)
        {
            try
            {
                string datefrm = null;
                string date_TO = null;
                string DateDeparting = "";
                string DD_Val = "";
                string DD_TO_Val = "";
                string dateTo = null;
                string Return_Origin = string.Empty;
                string Return_Destination = string.Empty;
                if (objserch.Tripmode == "1")
                {
                    DateTime datefrm11 = Convert.ToDateTime(objserch.FromDate);
                    datefrm = datefrm11.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DD_Val = datefrm11.ToString("dd/MM/yyyy").Replace('-', '/');
                    //DateDeparting =  datefrm+"----"+ DD_Val;
                    DateDeparting = datefrm;
                    //datefrm = DateTime.Parse(objserch.FromDate.ToString()).ToString("dd/MM/yyyy");

                    dateTo = "";
                    Return_Origin = null;
                    Return_Destination = null;
                }
                else if (objserch.Tripmode == "2")
                {
                    DateTime datefrm12 = Convert.ToDateTime(objserch.FromDate);
                    datefrm = datefrm12.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DD_Val = datefrm12.ToString("dd/MM/yyyy").Replace('-', '/');
                    //DateDeparting =  datefrm+"----"+ DD_Val;
                    DateDeparting = datefrm;

                    DateTime dateTo11 = Convert.ToDateTime(objserch.ToDate);
                    date_TO = dateTo11.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DD_TO_Val = dateTo11.ToString("dd/MM/yyyy").Replace('-', '/');
                    dateTo = date_TO;
                    //datefrm = DateTime.Parse(objserch.FromDate.ToString()).ToString("dd/MM/yyyy"); 
                    //DateTime date_to = Convert.ToDateTime(objserch.ToDate);
                    //dateTo = DateTime.Parse(objserch.ToDate.ToString()).ToString("dd/MM/yyyy");
                    Return_Origin = objserch.TOAirportCode;
                    Return_Destination = objserch.FromCityCode;
                }
                //var GetTokenValue = GetToken();

                FlightSearch objflightsearch = new FlightSearch();
                objflightsearch.TokenId = "AirticketOnlineWebSite";
                objflightsearch.AdultCount = objserch.Adult;
                if (objserch.Child == null)
                {
                    objflightsearch.ChildCount = "0";
                }
                else
                {
                    objflightsearch.ChildCount = objserch.Child;
                }
                if (objserch.Infant == null)
                {
                    objflightsearch.InfantCount = "0";
                }
                else
                {
                    objflightsearch.InfantCount = objserch.Infant;
                }
                //objflightsearch.InfantCount = objserch.Infant;
                objflightsearch.DirectFlight = "true";
                objflightsearch.OneStopFlight = "true";
                objflightsearch.JourneyType = objserch.Tripmode;
                objflightsearch.PreferredAirlines = "";
                objflightsearch.Origin = objserch.FromCityCode;
                objflightsearch.Destination = objserch.TOAirportCode;
                objflightsearch.FlightCabinClass = objserch.TravelType;
                objflightsearch.PreferredDepartureTime = DateDeparting;
                objflightsearch.PreferredArrivalTime = dateTo;
                //objflightsearch.ReturnOrigin = Return_Origin;
                //objflightsearch.ReturnDestination = Return_Destination;
                objflightsearch.Sources = "6E";
                dynamic searchflight = MultiLinkAirAPI.SerachFlight(objflightsearch);
                var data = JsonConvert.SerializeObject(searchflight);
                return Json(data, JsonRequestBehavior.AllowGet);
                //return Json(DateDeparting, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult FlightSearch()
        {
            return View();
        }

        public ActionResult TestPage()
        {
            return View();
        }

        public ActionResult ReturnFlightlist(string Tripmode, string FromCityCode, string TOAirportCode, string FromDate, string ToDate, string TravelType, string Adult, string Child, string Infant)
        {
            ViewBag.Tripmode = Tripmode;
            ViewBag.FromCityCode = FromCityCode;
            ViewBag.TOAirportCode = TOAirportCode;
            //DateTime FromDtaeCheckFormat= Convert.ToDateTime(FromDate);
            //string DateFromValue = FromDtaeCheckFormat.ToString("dd/MM/yyyy");
            //            ViewBag.FromDate = DateFromValue;
            ViewBag.FromDate = FromDate;
            //ViewBag.FromDate = "06/23/2020";
            ViewBag.ToDate = ToDate;
            ViewBag.TravelType = TravelType;
            ViewBag.Adult = Adult;
            ViewBag.Child = Child;
            ViewBag.Infant = Infant;
            return View("~/Areas/Merchant/Views/MerchantFlightDetails/ReturnFlightlist.cshtml");
        }
        public ActionResult FlightBookingDetails(string TrackNo = "", string PsgnAdult = "", string PsgnChildren = "", string PsgnInfant = "", string TripMode = "")
        {
            try
            {
                //var db = new DBContext();
                ViewBag.TrackNo = TrackNo;
                ViewBag.TripMode = TripMode;
                string PassAdult = PsgnAdult;
                string PassChild = PsgnChildren;
                string PassInfant = PsgnInfant;
                ViewBag.AdultCount = PassAdult;
                ViewBag.ChildCount = PassChild;
                ViewBag.InfantCount = PassInfant;

                if (TripMode == "1") // One Way Trip
                {
                    return View("~/Areas/Merchant/Views/MerchantFlightDetails/FlightBookingDetails.cshtml");
                }
                else if (TripMode == "2") // Round Trip
                {
                    return View("~/Areas/Merchant/Views/MerchantFlightDetails/RoundTripDetails.cshtml");
                }
                else if (TripMode == "3") // Multicity
                {
                    return View("~/Areas/Merchant/Views/MerchantFlightDetails/FlightBookingDetails.cshtml");
                }

                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult TestReturnPage()
        {
            return View();
        }
        public ActionResult Returnpage()
        {
            return View();
        }
        public ActionResult OLdReturnpage()
        {
            return View();
        }

        public ActionResult FlightDetails(string TrackNo = "", string PsgnAdult = "", string PsgnChildren = "", string PsgnInfant = "", string TripMode = "")
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #region Flight details verification page
        [HttpPost]
        public JsonResult GetFlightVerificationDetails(string TrackNo, string TripMode)
        {
            try
            {

                dynamic VerifyFlight = MultiLinkAirAPI.VerifyFlightDetails(TrackNo, TripMode);
                var data = JsonConvert.SerializeObject(VerifyFlight);
                return Json(data, JsonRequestBehavior.AllowGet);
                //var fetchToken = db.TBL_API_TOKEN.FirstOrDefault();
                //if (BookingValue != null && ReturnResultIndex != "0")
                //{
                //    var GetFareQuoteOneWay = Tek_TravelAPI.Fare_Quote(fetchToken.TOKEN, token, BookingValue);
                //    var data = JsonConvert.SerializeObject(GetFareQuoteOneWay);
                //    var GetFareQuoteReturn = Tek_TravelAPI.Fare_Quote(fetchToken.TOKEN, token, ReturnResultIndex);
                //    var ReturnDatadata = JsonConvert.SerializeObject(GetFareQuoteReturn);
                //    return Json(new { data, ReturnDatadata }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var GetFareQuoteOneWay = Tek_TravelAPI.Fare_Quote(fetchToken.TOKEN, token, BookingValue);
                //    var data = JsonConvert.SerializeObject(GetFareQuoteOneWay);
                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region Flight Additional Services
        [HttpPost]
        public JsonResult GetFlightAdditionalDetails(string req)
        {
            try
            {
                dynamic VerifyFlight = MultiLinkAirAPI.GetAdditionalFlightDetails(req);
                var data = JsonConvert.SerializeObject(VerifyFlight);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Flight Booking Request
        [HttpPost]
        public JsonResult FlightBookingRequest(string req)
        {
            try
            {
                #region variable Declieare                
                string BookingDate = string.Empty;
                string Ref_no = string.Empty;
                string PNR = string.Empty;
                string adult = string.Empty;
                int AdultVal = 0;
                int childVal = 0;
                int InfantVal = 0;
                string child = string.Empty;
                string infant = string.Empty;
                bool IsDomestic = false;
                string TicketType = string.Empty;
                string Airlinecode = string.Empty;
                string FlightNo = string.Empty;
                string FromAirport = string.Empty;
                string ToAirport = string.Empty;
                string DeptDate = string.Empty;
                string Depttime = string.Empty;
                string arivDate = string.Empty;
                string arivtime = string.Empty;
                string TotalFlightBaseFare = string.Empty;
                decimal TotalBaseFare = 0;
                string TotalFlightTax = string.Empty;
                decimal TotalTaxFare = 0;
                string TotalFlightPassengerTax = string.Empty;
                decimal TotalPngsTaxFare = 0;
                string TotalFlightAdditionalCharges = string.Empty;
                decimal TotalAdditionalFare = 0;
                string TotalFlightCuteFee = string.Empty;
                decimal TotalCuteFare = 0;
                string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
                decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
                string TotalFlightAmount = string.Empty;
                decimal TotalAmt = 0;
                string TotalCommissionAMtCharge = string.Empty;
                decimal TotalCommAmt = 0;
                string TotalServiceCharge = string.Empty;
                decimal TotalServiceAmt = 0;
                string TDSAmount = string.Empty;
                decimal TotalTDSAmountAmt = 0;
                string ServiceTax = string.Empty;
                decimal TotalServiceTaxAmt = 0;
                string AdultCheckedIn = string.Empty;
                string MainClass = string.Empty;
                string BookingClass = string.Empty;
                string BookingStatus = string.Empty;
                #endregion
                #region Booking Api call
                string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                dynamic VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
                string check = "";
                string ResValue = Convert.ToString(VerifyFlight);
                BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(ResValue);
                int count = BookingResponse.BookTicketResponses.BookTicketResponse.Count;
                var data = JsonConvert.SerializeObject(VerifyFlight);
                string APIRes = Convert.ToString(data);
                var TicketInfo = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();
                var Ticketcount = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.Count;
                var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
                var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
                var PassengerDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails.ToList();
                var lstpnsg = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails;
                var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
                string pnsgRes = Convert.ToString(val_Pnsg);
                BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
                BookingStatus = TicketInfo[Ticketcount - 1].Status;
                Ref_no = TicketInfo[Ticketcount-1].RefNo;
                PNR = FareDetails[FareDetailsCount-1].AirlinePNRNumber;
                TicketType= TicketInfo[Ticketcount - 1].TicketType;
                IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
                Airlinecode= FareDetails[0].AirlineCode;
                FlightNo = FareDetails[0].FlightNo;
                MainClass = FareDetails[0].MainClass;
                BookingClass = FareDetails[0].BookingClass;
                FromAirport = FareDetails[0].FromAirportCode;
                ToAirport = FareDetails[FareDetailsCount-1].ToAirportCode;
                DeptDate = FareDetails[0].DepartureDate;
                DeptDate = FareDetails[0].DepartureTime;
                arivDate = FareDetails[FareDetailsCount-1].ArrivalDate;
                arivtime = FareDetails[FareDetailsCount-1].ArriveTime;
                adult =  TicketInfo[Ticketcount - 1].Adult;
                AdultVal = int.Parse(adult);
                child = TicketInfo[Ticketcount - 1].Child;
                childVal = int.Parse(child);
                infant = TicketInfo[Ticketcount - 1].Infant;
                InfantVal = int.Parse(infant);

                TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
                TotalFlightTax = FareDetails[0].TotalFlightTax;
                TotalTaxFare = decimal.Parse(TotalFlightTax);
                TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
                TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
                TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
                TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
                TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
                TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
                TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
                TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
                TotalFlightAmount = FareDetails[0].TotalFlightAmount;
                TotalAmt = decimal.Parse(TotalFlightAmount);
                TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                TDSAmount = FareDetails[0].TDSAmount;
                TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                ServiceTax = FareDetails[0].ServiceTax;
                TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                AdultCheckedIn = FareDetails[0].AdultCheckedIn;

                #endregion

                var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                {
                    MEM_ID = CurrentMerchant.MEM_ID,
                    DIST_ID = getmemberinfo.INTRODUCER,
                    WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                    CORELATION_ID = COrelationID,
                    PNR = PNR,
                    REF_NO = Ref_no,
                    TRACK_NO = "",
                    TRIP_MODE = "1",
                    TICKET_NO = Ref_no,
                    TICKET_TYPE = TicketType,
                    IS_DOMESTIC = IsDomestic,
                    AIRLINE_CODE = Airlinecode,
                    FLIGHT_NO = FlightNo,
                    FROM_AIRPORT = FromAirport,
                    TO_AIRPORT = ToAirport,
                    BOOKING_DATE = DateTime.Now,
                    DEPT_DATE = DeptDate,
                    DEPT_TIME = Depttime,
                    ARRIVE_DATE = arivDate,
                    ARRIVE_TIME = arivtime,
                    NO_OF_ADULT = AdultVal,
                    NO_OF_CHILD = childVal,
                    NO_OF_INFANT = InfantVal,
                    TOTAL_FLIGHT_BASE_FARE = TotalBaseFare,
                    TOTAL_FLIGHT_TAX = TotalTaxFare,
                    TOTAL_PASSANGER_TAX = TotalPngsTaxFare,
                    TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                    TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare,
                    TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare,
                    TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare,
                    TOTAL_AIRPORT_FEE = 0,
                    TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                    TOTAL_FLIGHT_AMT = TotalAmt,
                    TOTAL_COMMISSION_AMT = TotalCommAmt,
                    TOTAL_TDS_AMT = TotalTDSAmountAmt,
                    TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                    TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                    STATUS = true,
                    IS_CANCELLATION = false,
                    FLIGHT_CANCELLATION_ID = "",
                    IS_HOLD = false,
                    API_RESPONSE = APIRes,
                    FLIGHT_BOOKING_DATE = BookingDate,
                    MAIN_CLASS= MainClass,
                    BOOKING_CLASS= BookingClass,
                    BOOKING_STATUS= BookingStatus
                };
                _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                _db.SaveChanges();
                foreach (var pnsgitem in PassengerDetails)
                {
                    var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                    string Pngdetails = Convert.ToString(val_Pnsg_Details);
                    string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                    string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                    TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        REF_NO = Ref_no,
                        PNR = PNR,
                        TITLE = pnsgitem.Title,
                        FIRST_NAME = pnsgitem.FirstName,
                        LAST_NAME = pnsgitem.LastName,
                        PASSENGER_TYPE = pnsgitem.PassengerType,
                        GENDER = pnsgitem.Gender,
                        BIRTH_DATE = PngDOB,
                        DETAILS = Pngdetails,
                        PASSENGER_RESP = pnsgRes,
                        CREATE_DATE = DateTime.Now,
                        PNSG_SEQ_NO= PnsgSeq_No,
                        CORELATION_ID = COrelationID,
                        PASSENGER_STATUS= BookingStatus,
                        CancelReqNo=""
                    };
                    _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                    _db.SaveChanges();
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Flight holding Request
        [HttpPost]
        public JsonResult FlightHoldingRequest(string req)
        {
            try
            {
                #region variable Declieare                
                string BookingDate = string.Empty;
                string Ref_no = string.Empty;
                string PNR = string.Empty;
                string adult = string.Empty;
                int AdultVal = 0;
                int childVal = 0;
                int InfantVal = 0;
                string child = string.Empty;
                string infant = string.Empty;
                bool IsDomestic = false;
                string TicketType = string.Empty;
                string Airlinecode = string.Empty;
                string FlightNo = string.Empty;
                string FromAirport = string.Empty;
                string ToAirport = string.Empty;
                string DeptDate = string.Empty;
                string Depttime = string.Empty;
                string arivDate = string.Empty;
                string arivtime = string.Empty;
                string TotalFlightBaseFare = string.Empty;
                decimal TotalBaseFare = 0;
                string TotalFlightTax = string.Empty;
                decimal TotalTaxFare = 0;
                string TotalFlightPassengerTax = string.Empty;
                decimal TotalPngsTaxFare = 0;
                string TotalFlightAdditionalCharges = string.Empty;
                decimal TotalAdditionalFare = 0;
                string TotalFlightCuteFee = string.Empty;
                decimal TotalCuteFare = 0;
                string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
                decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
                string TotalFlightAmount = string.Empty;
                decimal TotalAmt = 0;
                string TotalCommissionAMtCharge = string.Empty;
                decimal TotalCommAmt = 0;
                string TotalServiceCharge = string.Empty;
                decimal TotalServiceAmt = 0;
                string TDSAmount = string.Empty;
                decimal TotalTDSAmountAmt = 0;
                string ServiceTax = string.Empty;
                decimal TotalServiceTaxAmt = 0;
                string AdultCheckedIn = string.Empty;
                string MainClass = string.Empty;
                string BookingClass = string.Empty;
                string BookingStatus = string.Empty;
                #endregion
                #region Booking Api call
                string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                dynamic VerifyFlight = MultiLinkAirAPI.HoldingFlightTicket(req, COrelationID);
                string check = "";
                string ResValue = Convert.ToString(VerifyFlight);
                BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(ResValue);
                int count = BookingResponse.BookTicketResponses.BookTicketResponse.Count;
                var data = JsonConvert.SerializeObject(VerifyFlight);
                string APIRes = Convert.ToString(data);
                var TicketInfo = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();
                var Ticketcount = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.Count;
                var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
                var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
                var PassengerDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails.ToList();
              
                var lstpnsg = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails;
                var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
                string pnsgRes = Convert.ToString(val_Pnsg);
                BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
                BookingStatus = TicketInfo[Ticketcount - 1].Status;
                Ref_no = TicketInfo[Ticketcount - 1].RefNo;
                PNR = FareDetails[FareDetailsCount - 1].AirlinePNRNumber;
                TicketType = TicketInfo[Ticketcount - 1].TicketType;
                IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
                Airlinecode = FareDetails[0].AirlineCode;
                FlightNo = FareDetails[0].FlightNo;
                MainClass = FareDetails[0].MainClass;
                BookingClass = FareDetails[0].BookingClass;
                FromAirport = FareDetails[0].FromAirportCode;
                ToAirport = FareDetails[FareDetailsCount - 1].ToAirportCode;
                DeptDate = FareDetails[0].DepartureDate;
                DeptDate = FareDetails[0].DepartureTime;
                arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                adult = TicketInfo[Ticketcount - 1].Adult;
                AdultVal = int.Parse(adult);
                child = TicketInfo[Ticketcount - 1].Child;
                childVal = int.Parse(child);
                infant = TicketInfo[Ticketcount - 1].Infant;
                InfantVal = int.Parse(infant);
                TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
                TotalFlightTax = FareDetails[0].TotalFlightTax;
                TotalTaxFare = decimal.Parse(TotalFlightTax);
                TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
                TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
                TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
                TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
                TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
                TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
                TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
                TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
                TotalFlightAmount = FareDetails[0].TotalFlightAmount;
                TotalAmt = decimal.Parse(TotalFlightAmount);
                TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                TDSAmount = FareDetails[0].TDSAmount;
                TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                ServiceTax = FareDetails[0].ServiceTax;
                TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                AdultCheckedIn = FareDetails[0].AdultCheckedIn;

                #endregion

                var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                {
                    MEM_ID = CurrentMerchant.MEM_ID,
                    DIST_ID = getmemberinfo.INTRODUCER,
                    WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                    CORELATION_ID = COrelationID,
                    PNR = PNR,
                    REF_NO = Ref_no,
                    TRACK_NO = "",
                    TRIP_MODE = "1",
                    TICKET_NO = Ref_no,
                    TICKET_TYPE = TicketType,
                    IS_DOMESTIC = IsDomestic,
                    AIRLINE_CODE = Airlinecode,
                    FLIGHT_NO = FlightNo,
                    FROM_AIRPORT = FromAirport,
                    TO_AIRPORT = ToAirport,
                    BOOKING_DATE = DateTime.Now,
                    DEPT_DATE = DeptDate,
                    DEPT_TIME = Depttime,
                    ARRIVE_DATE = arivDate,
                    ARRIVE_TIME = arivtime,
                    NO_OF_ADULT = AdultVal,
                    NO_OF_CHILD = childVal,
                    NO_OF_INFANT = InfantVal,
                    TOTAL_FLIGHT_BASE_FARE = TotalBaseFare,
                    TOTAL_FLIGHT_TAX = TotalTaxFare,
                    TOTAL_PASSANGER_TAX = TotalPngsTaxFare,
                    TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                    TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare,
                    TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare,
                    TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare,
                    TOTAL_AIRPORT_FEE = 0,
                    TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                    TOTAL_FLIGHT_AMT = TotalAmt,
                    TOTAL_COMMISSION_AMT = TotalCommAmt,
                    TOTAL_TDS_AMT = TotalTDSAmountAmt,
                    TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                    TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                    STATUS = true,
                    IS_CANCELLATION = false,
                    FLIGHT_CANCELLATION_ID = "",
                    IS_HOLD = true,
                    BOOKING_HOLD_ID = Ref_no,
                    API_RESPONSE = APIRes,
                    FLIGHT_BOOKING_DATE = BookingDate,
                    MAIN_CLASS = MainClass,
                    BOOKING_CLASS = BookingClass,
                    BOOKING_STATUS = BookingStatus
                };
                _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                _db.SaveChanges();
                foreach (var pnsgitem in PassengerDetails)
                {
                    var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);                    
                    string Pngdetails = Convert.ToString(val_Pnsg_Details);
                    string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                    string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                    TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                    {
                        MEM_ID=CurrentMerchant.MEM_ID,
                        REF_NO= Ref_no,
                        PNR= PNR,
                        TITLE=pnsgitem.Title,
                        FIRST_NAME=pnsgitem.FirstName,
                        LAST_NAME=pnsgitem.LastName,
                        PASSENGER_TYPE=pnsgitem.PassengerType,
                        GENDER=pnsgitem.Gender,
                        BIRTH_DATE= PngDOB,
                        DETAILS= Pngdetails,
                        PASSENGER_RESP= pnsgRes,
                        CREATE_DATE=DateTime.Now,
                        PNSG_SEQ_NO= PnsgSeq_No,
                        CORELATION_ID = COrelationID,
                        PASSENGER_STATUS = BookingStatus,
                        CancelReqNo=""
                    };
                    _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                    _db.SaveChanges();
                }
                    
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        public ActionResult BookedFlightInformaiton()
        {
            return View();
        }
        public PartialViewResult BookedFlightgridList()
        {
            try
            {
                var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.ToList();
                return PartialView("BookedFlightgridList", GetBookedFlightList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult FlightBookingInvoice()
        {
            initpage();
            try
            {
                var db = new DBContext();
                var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x=>x.MEM_ID==CurrentMerchant.MEM_ID && x.BOOKING_STATUS!= "Cancelled").OrderByDescending(z=>z.BOOKING_DATE).ToList();

                return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PrintFlightInvoice(string refId,string PNR)
        {
            try
            {
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(refId,"", PNR, "","","");
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ConfirmHoldTicket(string refId,string corelation)
        {
            try
            {
                #region variable Declieare                
                string BookingDate = string.Empty;
                string Ref_no = string.Empty;
                string PNR = string.Empty;
                string adult = string.Empty;
                int AdultVal = 0;
                int childVal = 0;
                int InfantVal = 0;
                string child = string.Empty;
                string infant = string.Empty;
                bool IsDomestic = false;
                string TicketType = string.Empty;
                string Airlinecode = string.Empty;
                string FlightNo = string.Empty;
                string FromAirport = string.Empty;
                string ToAirport = string.Empty;
                string DeptDate = string.Empty;
                string Depttime = string.Empty;
                string arivDate = string.Empty;
                string arivtime = string.Empty;
                string TotalFlightBaseFare = string.Empty;
                decimal TotalBaseFare = 0;
                string TotalFlightTax = string.Empty;
                decimal TotalTaxFare = 0;
                string TotalFlightPassengerTax = string.Empty;
                decimal TotalPngsTaxFare = 0;
                string TotalFlightAdditionalCharges = string.Empty;
                decimal TotalAdditionalFare = 0;
                string TotalFlightCuteFee = string.Empty;
                decimal TotalCuteFare = 0;
                string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
                decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
                string TotalFlightAmount = string.Empty;
                decimal TotalAmt = 0;
                string TotalCommissionAMtCharge = string.Empty;
                decimal TotalCommAmt = 0;
                string TotalServiceCharge = string.Empty;
                decimal TotalServiceAmt = 0;
                string TDSAmount = string.Empty;
                decimal TotalTDSAmountAmt = 0;
                string ServiceTax = string.Empty;
                decimal TotalServiceTaxAmt = 0;
                string AdultCheckedIn = string.Empty;
                string MainClass = string.Empty;
                string BookingClass = string.Empty;
                string BookingStatus = string.Empty;
                #endregion
                dynamic HoldBookingConfirm = MultiLinkAirAPI.HoldTicketConfrim(refId, "");
                var data = JsonConvert.SerializeObject(HoldBookingConfirm);

                string check = "";
                string ResValue = Convert.ToString(HoldBookingConfirm);
                HoldBookingConfirmResponsesDTO BookingResponse = JsonConvert.DeserializeObject<HoldBookingConfirmResponsesDTO>(ResValue);
                int count = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse.Count;
                
                string APIRes = Convert.ToString(data);
                var TicketInfo = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].TicketDetails.ToList();
                var Ticketcount = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].TicketDetails.Count;
                var FareDetails = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].FlightFareDetails.ToList();
                var FareDetailsCount = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].FlightFareDetails.Count;
                var PassengerDetails = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].PassengerDetails.ToList();

                var lstpnsg = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].PassengerDetails;
                var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
                string pnsgRes = Convert.ToString(val_Pnsg);
                BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
                BookingStatus = TicketInfo[Ticketcount - 1].Status;
                Ref_no = TicketInfo[Ticketcount - 1].RefNo;
                PNR = FareDetails[FareDetailsCount - 1].AirlinePNRNumber;
                TicketType = TicketInfo[Ticketcount - 1].TicketType;
                IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
                Airlinecode = FareDetails[0].AirlineCode;
                FlightNo = FareDetails[0].FlightNo;
                MainClass = FareDetails[0].MainClass;
                BookingClass = FareDetails[0].BookingClass;
                FromAirport = FareDetails[0].FromAirportCode;
                ToAirport = FareDetails[FareDetailsCount - 1].ToAirportCode;
                DeptDate = FareDetails[0].DepartureDate;
                DeptDate = FareDetails[0].DepartureTime;
                arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                adult = TicketInfo[Ticketcount - 1].Adult;
                AdultVal = int.Parse(adult);
                child = TicketInfo[Ticketcount - 1].Child;
                childVal = int.Parse(child);
                infant = TicketInfo[Ticketcount - 1].Infant;
                InfantVal = int.Parse(infant);
                TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
                TotalFlightTax = FareDetails[0].TotalFlightTax;
                TotalTaxFare = decimal.Parse(TotalFlightTax);
                TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
                TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
                TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
                TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
                TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
                TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
                TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
                TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
                TotalFlightAmount = FareDetails[0].TotalFlightAmount;
                TotalAmt = decimal.Parse(TotalFlightAmount);
                TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                TDSAmount = FareDetails[0].TDSAmount;
                TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                ServiceTax = FareDetails[0].ServiceTax;
                TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                AdultCheckedIn = FareDetails[0].AdultCheckedIn;

                var GetFlightBookedInfor = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == Ref_no && x.CORELATION_ID == corelation && x.BOOKING_STATUS == "On Hold" && x.IS_HOLD==true);
                if (GetFlightBookedInfor != null)
                {
                    GetFlightBookedInfor.BOOKING_STATUS = "On Hold Confirm";
                    GetFlightBookedInfor.IS_HOLD = false;
                    GetFlightBookedInfor.BOOKING_HOLD_ID = "";
                }
                _db.Entry(GetFlightBookedInfor).State = System.Data.Entity.EntityState.Modified;
                 _db.SaveChanges();
                var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                {
                    MEM_ID = CurrentMerchant.MEM_ID,
                    DIST_ID = getmemberinfo.INTRODUCER,
                    WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                    CORELATION_ID = corelation,
                    PNR = PNR,
                    REF_NO = Ref_no,
                    TRACK_NO = "",
                    TRIP_MODE = "1",
                    TICKET_NO = Ref_no,
                    TICKET_TYPE = TicketType,
                    IS_DOMESTIC = IsDomestic,
                    AIRLINE_CODE = Airlinecode,
                    FLIGHT_NO = FlightNo,
                    FROM_AIRPORT = FromAirport,
                    TO_AIRPORT = ToAirport,
                    BOOKING_DATE = DateTime.Now,
                    DEPT_DATE = DeptDate,
                    DEPT_TIME = Depttime,
                    ARRIVE_DATE = arivDate,
                    ARRIVE_TIME = arivtime,
                    NO_OF_ADULT = AdultVal,
                    NO_OF_CHILD = childVal,
                    NO_OF_INFANT = InfantVal,
                    TOTAL_FLIGHT_BASE_FARE = TotalBaseFare,
                    TOTAL_FLIGHT_TAX = TotalTaxFare,
                    TOTAL_PASSANGER_TAX = TotalPngsTaxFare,
                    TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                    TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare,
                    TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare,
                    TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare,
                    TOTAL_AIRPORT_FEE = 0,
                    TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                    TOTAL_FLIGHT_AMT = TotalAmt,
                    TOTAL_COMMISSION_AMT = TotalCommAmt,
                    TOTAL_TDS_AMT = TotalTDSAmountAmt,
                    TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                    TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                    STATUS = true,
                    IS_CANCELLATION = false,
                    FLIGHT_CANCELLATION_ID = "",
                    IS_HOLD = false,
                    BOOKING_HOLD_ID = "",
                    API_RESPONSE = APIRes,
                    FLIGHT_BOOKING_DATE = BookingDate,
                    MAIN_CLASS = MainClass,
                    BOOKING_CLASS = BookingClass,
                    BOOKING_STATUS = BookingStatus
                };
                _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                _db.SaveChanges();
                //foreach (var pnsgitem in PassengerDetails)
                //{
                //    var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                //    string Pngdetails = Convert.ToString(val_Pnsg_Details);
                //    string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                //    TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                //    {
                //        MEM_ID = CurrentMerchant.MEM_ID,
                //        REF_NO = Ref_no,
                //        PNR = PNR,
                //        TITLE = pnsgitem.Title,
                //        FIRST_NAME = pnsgitem.FirstName,
                //        LAST_NAME = pnsgitem.LastName,
                //        PASSENGER_TYPE = pnsgitem.PassengerType,
                //        GENDER = pnsgitem.Gender,
                //        BIRTH_DATE = PngDOB,
                //        DETAILS = Pngdetails,
                //        PASSENGER_RESP = pnsgRes,
                //        CREATE_DATE = DateTime.Now
                //    };
                //    _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                //    _db.SaveChanges();
                //}

                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult BookedTicketPassangerList(string refId, string corelation)
        {
            try
            {

                var GetPassangerDetails = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == refId && x.CORELATION_ID== corelation && x.PASSENGER_STATUS== "Acknowledged").ToList();
                return Json(GetPassangerDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelFlightTicket(string[] PngsVal, string refId)
        {
            try
            {
                var db = new DBContext();
                 dynamic FlightCancellation = MultiLinkAirAPI.FlightCancellation(PngsVal, refId);
                string val= "";
                var getticketinfo = FlightCancellation.TicketCancelDetails.TicketCancelResponse[0];
                string resmas = Convert.ToString(getticketinfo);
                var getticketCancelRefifinfo = FlightCancellation.TicketCancelDetails.CancelReqNo[0];
                string valsdsf = "";
                string valsds345345f = "";
                var passangerId = PngsVal;
                var pangCount = PngsVal.Length;
                long Pnsgid = 0;
                foreach (string pnsgid in passangerId)
                {
                    long.TryParse(pnsgid,out Pnsgid);
                    var getpassagerlist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x=>x.SLN== Pnsgid);
                    if (getpassagerlist != null)
                    {
                        getpassagerlist.PASSENGER_STATUS = "Cancelled";
                        getpassagerlist.CancelReqNo = getticketCancelRefifinfo;
                        db.Entry(getpassagerlist).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (pangCount == 1)
                {
                    var cancelledFlightInfo = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                    if (cancelledFlightInfo != null) {
                        cancelledFlightInfo.BOOKING_STATUS = "Cancelled";
                        cancelledFlightInfo.CANCELLATION_DATE =DateTime.Now;
                        cancelledFlightInfo.IS_CANCELLATION = true;
                        cancelledFlightInfo.FLIGHT_CANCELLATION_ID = getticketCancelRefifinfo;
                        db.Entry(cancelledFlightInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }                
                return Json(resmas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }   
}
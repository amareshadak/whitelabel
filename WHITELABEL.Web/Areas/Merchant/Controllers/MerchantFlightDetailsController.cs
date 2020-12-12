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
                objflightsearch.TokenId = "BOMAK";
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
        //public ActionResult FlightBookingDetails(string TrackNo = "", string PsgnAdult = "", string PsgnChildren = "", string PsgnInfant = "", string TripMode = "")
        public ActionResult FlightBookingDetails(string TrackNo = "", string PsgnAdult = "", string PsgnChildren = "", string PsgnInfant = "", string TripMode = "",string OriginCode = "",string DestinationCode = "")
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
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
                    ViewBag.OriginCode = OriginCode;
                    ViewBag.DestinationCode = DestinationCode;
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
        public JsonResult GetFlightVerificationDetails(string TrackNo, string TripMode,string OriginCode,string DestinationCode)
        {
            try
            {
                string FlightType = string.Empty;
                decimal AdditionalAmount = 0;
                var db = new DBContext();
                var GetOriginAirportInfo = db.TBL_AIRPORT_DETAILS.FirstOrDefault(x => x.CITYCODE == OriginCode);
                var GetDestinationAirportInfo = db.TBL_AIRPORT_DETAILS.FirstOrDefault(x => x.CITYCODE == DestinationCode);
                var GetAdditionalAmt = db.TBL_FLIGHT_MARKUP.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                if (GetAdditionalAmt != null)
                {
                    if (GetOriginAirportInfo.AIRPORT_TYPE == "domestic" && GetDestinationAirportInfo.AIRPORT_TYPE == "domestic")
                    {
                        AdditionalAmount = GetAdditionalAmt.DOMESTIC_MARKUP;
                        FlightType = "Domestic";
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "domestic" && GetDestinationAirportInfo.AIRPORT_TYPE == "International")
                    {
                        FlightType = "International";
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "International" && GetDestinationAirportInfo.AIRPORT_TYPE == "domestic")
                    {
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                        FlightType = "International";
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "International" && GetDestinationAirportInfo.AIRPORT_TYPE == "International")
                    {
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                        FlightType = "International";
                    }
                    else
                    {
                        AdditionalAmount = 0;
                        FlightType = "Domestic";
                    }
                }
                else
                {
                    AdditionalAmount = 0;
                }
                
                dynamic VerifyFlight = MultiLinkAirAPI.VerifyFlightDetails(TrackNo, TripMode);
                var data = JsonConvert.SerializeObject(VerifyFlight);
                //return Json(data, JsonRequestBehavior.AllowGet);

                return Json(new { data = data, AdditionalAmount = AdditionalAmount,ISFlightType= FlightType }, JsonRequestBehavior.AllowGet);

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

        #region Round Trip Flight Details Verification
        [HttpPost]
        //public JsonResult GetRoundTripFlightVerificationDetails(string outBoundTrackNo, string inBoundTrackNo, string TripMode)
        public JsonResult GetRoundTripFlightVerificationDetails(string outBoundTrackNo, string inBoundTrackNo, string TripMode, string OriginCode, string DestinationCode)
        {
            try
            {
                string FlightType = string.Empty;
                decimal AdditionalAmount = 0;
                var db = new DBContext();
                var GetOriginAirportInfo = db.TBL_AIRPORT_DETAILS.FirstOrDefault(x => x.CITYCODE == OriginCode);
                var GetDestinationAirportInfo = db.TBL_AIRPORT_DETAILS.FirstOrDefault(x => x.CITYCODE == DestinationCode);
                var GetAdditionalAmt = db.TBL_FLIGHT_MARKUP.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                if (GetAdditionalAmt != null)
                {
                    if (GetOriginAirportInfo.AIRPORT_TYPE == "domestic" && GetDestinationAirportInfo.AIRPORT_TYPE == "domestic")
                    {
                        AdditionalAmount = GetAdditionalAmt.DOMESTIC_MARKUP;
                        FlightType = "Domestic";
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "domestic" && GetDestinationAirportInfo.AIRPORT_TYPE == "International")
                    {
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                        FlightType = "International";
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "International" && GetDestinationAirportInfo.AIRPORT_TYPE == "domestic")
                    {
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                    }
                    else if (GetOriginAirportInfo.AIRPORT_TYPE == "International" && GetDestinationAirportInfo.AIRPORT_TYPE == "International")
                    {
                        AdditionalAmount = GetAdditionalAmt.INTERNATIONAL_MARKUP;
                        FlightType = "International";
                    }
                    else
                    {
                        AdditionalAmount = 0;
                        FlightType = "Domestic";
                    }
                }
                else
                {
                    AdditionalAmount = 0;
                    FlightType = "Domestic";
                }
                dynamic outBoundResponce = MultiLinkAirAPI.VerifyFlightDetails(outBoundTrackNo, TripMode);
                dynamic inBoundResponce = MultiLinkAirAPI.VerifyFlightDetails(inBoundTrackNo, TripMode);
                var outBoundData = JsonConvert.SerializeObject(outBoundResponce);
                var inBoundData = JsonConvert.SerializeObject(inBoundResponce);
                //return Json(new { outBoundData, inBoundData }, JsonRequestBehavior.AllowGet);   
                return Json(new { outBoundData, inBoundData, AdditionalAmount = AdditionalAmount, ISFlightType = FlightType }, JsonRequestBehavior.AllowGet);
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

        #region Flight Fare Rules Details
        [HttpPost]
        public JsonResult GetFareDetails(string TrackNo)
        {
            try
            {
                dynamic VerifyFlight = MultiLinkAirAPI.GetFareDetails(TrackNo);
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
        public string RefundAmount(string corelationId, string TripMode)
        {
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    decimal BookingAmt = 0;
                    decimal UserMarkUp = 0;
                    decimal AdditionalAmt = 0;
                    decimal ClosingAmt = 0;
                    decimal AddClosingAmt = 0;
                    decimal TotalBookingAmt = 0;
                    var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    var flightBookingAmount = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.CORELATION_ID == corelationId);
                    BookingAmt = (decimal)flightBookingAmount.TOTAL_FLIGHT_AMT;
                    UserMarkUp = (decimal)flightBookingAmount.USER_MARKUP;
                    AdditionalAmt = (decimal)flightBookingAmount.ADMIN_MARKUP;
                    TotalBookingAmt = BookingAmt + UserMarkUp + AdditionalAmt;
                    var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                    if (MemberAcntLog != null)
                    {
                        ClosingAmt = MemberAcntLog.CLOSING;
                        AddClosingAmt = ClosingAmt + TotalBookingAmt;
                    }
                    else {
                        ClosingAmt = 0;
                        AddClosingAmt = ClosingAmt + TotalBookingAmt;
                    }
                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = CurrentMerchant.MEM_ID,
                        MEMBER_TYPE = "RETAILER",
                        TRANSACTION_TYPE = "FLIGHT BOOKING REFUND",
                        TRANSACTION_DATE = System.DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "CR",
                        //AMOUNT = TotalAmt,
                        AMOUNT = TotalBookingAmt,
                        NARRATION = "Debit amount for flight booking",
                        OPENING = ClosingAmt,
                        CLOSING = AddClosingAmt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = 0,
                        TDS = 0,
                        IPAddress = "",
                        TDS_PERCENTAGE = 0,
                        //GST_PERCENTAGE = AdminGST,
                        GST_PERCENTAGE = 0,
                        WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                        //SUPER_ID = (long)SUP_MEM_ID,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                        SERVICE_ID = 0,
                        CORELATIONID = corelationId,
                        REC_COMM_TYPE = "",
                        COMM_VALUE = 0,
                        NET_COMM_AMT = 0,
                        TDS_DR_COMM_AMT = 0,
                        CGST_COMM_AMT_INPUT = 0,
                        CGST_COMM_AMT_OUTPUT = 0,
                        SGST_COMM_AMT_INPUT = 0,
                        SGST_COMM_AMT_OUTPUT = 0,
                        IGST_COMM_AMT_INPUT = 0,
                        IGST_COMM_AMT_OUTPUT = 0,
                        TOTAL_GST_COMM_AMT_INPUT = 0,
                        TOTAL_GST_COMM_AMT_OUTPUT = 0,
                        TDS_RATE = 0,
                        CGST_RATE = 0,
                        SGST_RATE = 0,
                        IGST_RATE = 0,
                        TOTAL_GST_RATE = 0,
                        COMM_SLAB_ID = 0,
                        STATE_ID = getmemberinfo.STATE_ID,
                        FLAG1 = 0,
                        FLAG2 = 0,
                        FLAG3 = 0,
                        FLAG4 = 0,
                        FLAG5 = 0,
                        FLAG6 = 0,
                        FLAG7 = 0,
                        FLAG8 = 0,
                        FLAG9 = 0,
                        FLAG10 = 0,
                        VENDOR_ID = 0
                    };
                    _db.TBL_ACCOUNTS.Add(objCommPer);
                    _db.SaveChanges();
                    var BookingDetails = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == corelationId).ToList();
                    if (BookingDetails != null)
                    {
                        foreach (var BookingItem in BookingDetails)
                        {
                            var UpdateFlightStatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == BookingItem.SLN);
                            UpdateFlightStatus.BOOKING_STATUS = "REFUND";
                            UpdateFlightStatus.Cancellation_status = "REFUND";
                            UpdateFlightStatus.OP_MODE = "REFUND";
                            _db.Entry(UpdateFlightStatus).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    var PasangerDetails = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.CORELATION_ID == corelationId).ToList();
                    if (PasangerDetails != null)
                    {
                        foreach (var PnsgItem in PasangerDetails)
                        {
                            var UpdatePassangerStatus = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == PnsgItem.SLN);
                            UpdatePassangerStatus.PASSENGER_STATUS = "REFUND";
                            UpdatePassangerStatus.CANCEL_STATUS = "REFUND";
                            _db.Entry(UpdatePassangerStatus).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    ContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
            return "success";
        }
        [HttpPost]
        //public JsonResult FlightBookingRequest(string req, string userMarkup,string TotalAmount)
        public JsonResult FlightBookingRequest(string req, string userMarkup, string FlightAmt, string TripMode,string NetAmount,string ISFlightType, string INTPancard, string deptSegment = "", string returnSegment = "")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            int deptcnt = 0;
            int retncnt = 0;
            decimal BlockBal = 0;
            decimal AddMainBal = 0;
            decimal MainBALL = 0;
            decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            decimal.TryParse(getmemberinfo.BLOCKED_BALANCE.ToString(), out BlockBal);
            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out MainBALL);
            AddMainBal = MainBALL - BlockBal;
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            if (AddMainBal > TotalBookAmt)
            {
                var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
                List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
                if (deptureSegment != null)
                { deptcnt = deptureSegment.Count(); }
                if (retSegment != null)
                {
                    retncnt = retSegment.Count();
                }

                FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
                var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
                Deptstopage = FlightInfo.Segment.Count();
                var sengList = FlightInfo.Segment.ToList();
                string Dept_FlightNo = string.Empty;
                string Dept_airlineCode = string.Empty;
                string retn_FlightNo = string.Empty;
                string retn_airlineCode = string.Empty;
                string GSTCompanyName = string.Empty;
                string GSTCompanyEmail = string.Empty;
                string GSTNO = string.Empty;
                string GSTMOBILE_NO = string.Empty;
                string GSTADDRESS = string.Empty;
                GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
                GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
                GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
                GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
                GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
                int cntDept = 0;
                int cntretn = 0;
                string SeqNoDept = "";
                string SeqNoRtn = "";
                DateTime R_DeptDate = new DateTime();
                DateTime R_RetnDate = new DateTime();
                if (TripMode == "R")
                {
                    var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
                    var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
                    Dept_FlightNo = SegInfodept[0].FlightNo;
                    Dept_airlineCode = SegInfodept[0].AirlineCode;
                    retn_FlightNo = SegInforeturn[0].FlightNo;
                    retn_airlineCode = SegInforeturn[0].AirlineCode;
                    cntDept = SegInfodept.Count();
                    cntretn = SegInforeturn.Count();
                    SeqNoDept = SegInfodept[0].SegmentSeqNo;
                    SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
                    R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
                    R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
                }
                var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
                var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
                var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
                int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
                #region variable Declieare     
                decimal AirAdditionalCharge = 0;
                decimal GSTAmount = 0;
                decimal Holding_Amount = 0;
                decimal UserMarkUp_Value = 0;
                decimal.TryParse(userMarkup, out UserMarkUp_Value);
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
                string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
                decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
                decimal.TryParse(GSTValue, out GSTAmount);
                decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
                decimal AdminGST = 0;
                decimal Publish_Fare = 0;
                decimal NET_FARE = 0;
                decimal NET_TOTAL_FARE = 0;
                decimal RESCHEDULE_FARE = 0;
                string ResValue = string.Empty;
                dynamic VerifyFlight = null;
                decimal TotalNetAmount=0;
                decimal FlightNetAmount = 0;
                decimal AgentCommAmt = 0;
                decimal AgentCommTDS = 0;
                decimal TCSAmt = 0;
                
                using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                       
                        AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
                        decimal UserMarkupGST = 0;
                        UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);

                        decimal.TryParse(NetAmount, out FlightNetAmount);
                        AgentCommAmt = FlightNetAmount - TotalBookAmt;
                        AgentCommTDS = ((AgentCommAmt * 5) / 100);
                        TotalNetAmount = FlightNetAmount+ AgentCommTDS;
                        if (ISFlightType == "International")
                        {
                            if (INTPancard != "")
                            {
                                TCSAmt = ((TotalBookAmt * 5) / 100);
                            }
                            else
                            {
                                TCSAmt = ((TotalBookAmt * 10) / 100);
                            }
                        }
                        else
                        {
                            TCSAmt = 0;
                        }

                        if (TripMode == "O")
                        {
                            TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = TripMode,
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = TripMode,
                                IS_DOMESTIC = false,
                                AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
                                TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = false,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "BOOKED",
                                HOLD_CHARGE = 0,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
                                API_REQUEST = req,
                                STOPAGE = (Deptstopage - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE= TotalNetAmount,
                                FARE_COMMISSION= AgentCommAmt,
                                FARE_COMMISSION_TDS= AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT= TCSAmt,
                                INT_FLIGHT_PANCARD= INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                            //_db.SaveChanges();
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "O",
                                    FLIGHT_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
                                    FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
                                    TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                        }
                        else
                        {
                            TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = TripMode,
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = TripMode,
                                IS_DOMESTIC = false,
                                //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                AIRLINE_CODE = Dept_airlineCode,
                                FLIGHT_NO = Dept_FlightNo,
                                //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
                                //FROM_AIRPORT = RetdeptFromAir,
                                //TO_AIRPORT = RetdeptToAir,
                                FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                                TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
                                //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = false,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "BOOKED",
                                HOLD_CHARGE = 0,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = SeqNoDept,
                                API_REQUEST = req,
                                STOPAGE = (cntDept - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE = TotalNetAmount,
                                FARE_COMMISSION = AgentCommAmt,
                                FARE_COMMISSION_TDS = AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                                INT_FLIGHT_PANCARD = INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
                            TBL_FLIGHT_BOOKING_DETAILS objflightreturn = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = "",
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = "",
                                IS_DOMESTIC = false,
                                //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                AIRLINE_CODE = retn_airlineCode,
                                FLIGHT_NO = retn_FlightNo,
                                //FROM_AIRPORT = RettripFromAir,
                                //TO_AIRPORT = RettripToAir,
                                FROM_AIRPORT = retSegment[0].FromAirportCode,
                                TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
                                //FROM_AIRPORT = retSegment.Segment[0].FromAirportCode,
                                //TO_AIRPORT = retSegment.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = false,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "BOOKED",
                                HOLD_CHARGE = 0,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = SeqNoRtn,
                                API_REQUEST = req,
                                STOPAGE = (cntretn - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE = TotalNetAmount,
                                FARE_COMMISSION = AgentCommAmt,
                                FARE_COMMISSION_TDS = AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                                INT_FLIGHT_PANCARD = INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightreturn);
                            //_db.SaveChanges();
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "O",
                                    DOJ = R_DeptDate,
                                    FLIGHT_SEGMENT = SeqNoDept,
                                    FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                                    TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "R",
                                    FLIGHT_SEGMENT = SeqNoRtn,
                                    DOJ = R_RetnDate,
                                    FROM_AIRPORT = retSegment[0].FromAirportCode,
                                    TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                        }
                        decimal mmainBlc = 0;
                        decimal SubMainBlc = 0;
                        decimal Closing = 0;
                        decimal mainClosing = 0;
                        decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
                        //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                        if (getmemberinfo.BALANCE != null)
                        {
                            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                            //SubMainBlc = mmainBlc - TotalAmt;  //TotalBookAmt
                            SubMainBlc = mmainBlc - T_Amount;
                            getmemberinfo.BALANCE = SubMainBlc;
                        }
                        else
                        {
                            //SubMainBlc = mmainBlc - TotalAmt;
                            SubMainBlc = mmainBlc - T_Amount;
                            getmemberinfo.BALANCE = SubMainBlc;
                        }

                        getmemberinfo.BALANCE = SubMainBlc;
                        _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                        //_db.SaveChanges();
                        //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                        if (MemberAcntLog != null)
                        {
                            Closing = MemberAcntLog.CLOSING;
                            //mainClosing = Closing - TotalAmt;
                            mainClosing = Closing - T_Amount;
                        }
                        else
                        {
                            Closing = MemberAcntLog.CLOSING;
                            //mainClosing = Closing - TotalAmt;
                            mainClosing = Closing - T_Amount;
                        }
                        TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = CurrentMerchant.MEM_ID,
                            MEMBER_TYPE = "RETAILER",
                            TRANSACTION_TYPE = "FLIGHT BOOKING",
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            //AMOUNT = TotalAmt,
                            AMOUNT = T_Amount,
                            NARRATION = "Debit amount for flight booking",
                            OPENING = Closing,
                            CLOSING = mainClosing,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = (float)GSTAmount,
                            TDS = 0,
                            IPAddress = "",
                            TDS_PERCENTAGE = 0,
                            //GST_PERCENTAGE = AdminGST,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                            //SUPER_ID = (long)SUP_MEM_ID,
                            SUPER_ID = 0,
                            DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE = 0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = getmemberinfo.STATE_ID,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0,
                            VENDOR_ID = 0
                        };
                        _db.TBL_ACCOUNTS.Add(objCommPer);
                        _db.SaveChanges();
                        #endregion
                        #region Booking Api call
                        //dynamic VerifyFlight = null;
                        VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
                        ResValue = Convert.ToString(VerifyFlight);
                        //var APIStatus = VerifyFlight.BookTicketResponse.Error;
                        var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
                        if (getStatus == "Acknowledged" || getStatus == "Completed")
                        {
                            ContextTransaction.Commit();
                        }
                        else
                        {
                            ContextTransaction.Rollback();
                            return Json("Please try again later", JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        ContextTransaction.Rollback();
                        //string check = RefundAmount(COrelationID, TripMode);
                        return Json(false, JsonRequestBehavior.AllowGet);
                        throw;
                    }
                }
                using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
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
                        string DATEofJourney = DeptDate;
                        DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
                        //DateTime dateofJourey =Convert.ToDateTime(DATEofJourney);
                        Depttime = FareDetails[0].DepartureTime;
                        arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                        arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                        adult = TicketInfo[Ticketcount - 1].Adult;
                        AdultVal = int.Parse(adult);
                        child = TicketInfo[Ticketcount - 1].Child;
                        childVal = int.Parse(child);
                        infant = TicketInfo[Ticketcount - 1].Infant;
                        InfantVal = int.Parse(infant);
                        //string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                        string AIRADDITIONALAMOUNT_Value = Session["AIRADDITIONALAMOUNT"].ToString();
                        decimal Additional_AMt = 0;
                        decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
                        TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                        Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;

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
                        NET_FARE = TotalAmt;
                        NET_TOTAL_FARE = NET_FARE + Additional_AMt;
                        TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                        TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                        TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                        TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                        TDSAmount = FareDetails[0].TDSAmount;
                        TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                        ServiceTax = FareDetails[0].ServiceTax;
                        TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                        AdultCheckedIn = FareDetails[0].AdultCheckedIn;
                        string jhjdfd = "";
                        var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
                        if (GetFligtInfo != null)
                        {
                            foreach (var ticketInfo in GetFligtInfo)
                            {
                                var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
                                flightTicketInfo.PNR = PNR;
                                flightTicketInfo.REF_NO = Ref_no;
                                flightTicketInfo.TRACK_NO = "";
                                flightTicketInfo.TRIP_MODE = "1";
                                flightTicketInfo.TICKET_TYPE = TicketType;
                                flightTicketInfo.TICKET_NO = Ref_no;
                                flightTicketInfo.IS_DOMESTIC = IsDomestic;
                                //flightTicketInfo.AIRLINE_CODE = Airlinecode;
                                //flightTicketInfo.FLIGHT_NO = FlightNo;
                                //flightTicketInfo.FROM_AIRPORT = FromAirport;
                                //flightTicketInfo.TO_AIRPORT = ToAirport;
                                flightTicketInfo.BOOKING_DATE = DateTime.Now;
                                flightTicketInfo.DEPT_DATE = DeptDate;
                                flightTicketInfo.DEPT_TIME = Depttime;
                                flightTicketInfo.ARRIVE_DATE = arivDate;
                                flightTicketInfo.ARRIVE_TIME = arivtime;
                                flightTicketInfo.NO_OF_ADULT = AdultVal;
                                flightTicketInfo.NO_OF_CHILD = childVal;
                                flightTicketInfo.NO_OF_INFANT = InfantVal;
                                flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
                                flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
                                flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
                                flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
                                flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
                                flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
                                flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
                                flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
                                flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
                                flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
                                flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
                                flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
                                flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
                                flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
                                flightTicketInfo.STATUS = true;
                                flightTicketInfo.IS_CANCELLATION = false;
                                flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
                                flightTicketInfo.IS_HOLD = false;
                                flightTicketInfo.API_RESPONSE = APIRes;
                                flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
                                flightTicketInfo.BOOKING_STATUS = BookingStatus;
                                flightTicketInfo.MAIN_CLASS = MainClass;
                                flightTicketInfo.BOOKING_CLASS = BookingClass;
                                flightTicketInfo.PUBLISH_FARE = Publish_Fare;
                                flightTicketInfo.NET_FARE = NET_FARE;
                                flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
                                flightTicketInfo.CANCELLATION_REMARK = "";
                                flightTicketInfo.RESCHEDULE_FARE = false;
                                flightTicketInfo.RESCHEDULE_REMARK = "";
                                _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
                                //_db.SaveChanges();
                                //ContextTransaction.Commit();
                            }

                        }
                        if (TripMode == "O")
                        {
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName);
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.DOJ = dateofJourey;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                    //_db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                    //_db.SaveChanges();
                                }
                            }
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "R");
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                    //_db.SaveChanges();
                                }
                            }
                        }
                        _db.SaveChanges();
                        ContextTransaction.Commit();
                        #endregion
                        TempData["IsShowPrintTicket"] = "Show";
                        TempData["IsShowRef_NoTicket"] = Ref_no;
                        TempData["IsShowPNRTicket"] = PNR;
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        ContextTransaction.Rollback();
                        //string check = RefundAmount(COrelationID, TripMode);
                        return Json(false, JsonRequestBehavior.AllowGet);

                        throw;
                    }
                }
            }
            else
            {
                return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Old Booking Code
        //public JsonResult FlightBookingRequest(string req, string userMarkup, string FlightAmt, string TripMode, string deptSegment = "", string returnSegment = "")
        //{
        //    string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    int Deptstopage = 0;
        //    decimal TotalBookAmt = 0;
        //    int deptcnt = 0;
        //    int retncnt = 0;
        //    decimal.TryParse(FlightAmt, out TotalBookAmt);
        //    var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //    if (getmemberinfo.BALANCE > TotalBookAmt)
        //    {
        //        var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
        //        List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
        //        List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
        //        if (deptureSegment != null)
        //        { deptcnt = deptureSegment.Count(); }
        //        if (retSegment != null)
        //        {
        //            retncnt = retSegment.Count();
        //        }

        //        FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
        //        var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
        //        Deptstopage = FlightInfo.Segment.Count();
        //        var sengList = FlightInfo.Segment.ToList();
        //        string Dept_FlightNo = string.Empty;
        //        string Dept_airlineCode = string.Empty;
        //        string retn_FlightNo = string.Empty;
        //        string retn_airlineCode = string.Empty;
        //        string GSTCompanyName = string.Empty;
        //        string GSTCompanyEmail = string.Empty;
        //        string GSTNO = string.Empty;
        //        string GSTMOBILE_NO = string.Empty;
        //        string GSTADDRESS = string.Empty;
        //        GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
        //        GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
        //        GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
        //        GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
        //        GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
        //        int cntDept = 0;
        //        int cntretn = 0;
        //        string SeqNoDept = "";
        //        string SeqNoRtn = "";
        //        DateTime R_DeptDate = new DateTime();
        //        DateTime R_RetnDate = new DateTime();
        //        if (TripMode == "R")
        //        {
        //            var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
        //            var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
        //            Dept_FlightNo = SegInfodept[0].FlightNo;
        //            Dept_airlineCode = SegInfodept[0].AirlineCode;
        //            retn_FlightNo = SegInforeturn[0].FlightNo;
        //            retn_airlineCode = SegInforeturn[0].AirlineCode;
        //            cntDept = SegInfodept.Count();
        //            cntretn = SegInforeturn.Count();
        //            SeqNoDept = SegInfodept[0].SegmentSeqNo;
        //            SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
        //            R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
        //            R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
        //        }
        //        var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
        //        var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
        //        var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
        //        int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
        //        #region variable Declieare     
        //        decimal AirAdditionalCharge = 0;
        //        decimal GSTAmount = 0;
        //        decimal Holding_Amount = 0;
        //        decimal UserMarkUp_Value = 0;
        //        decimal.TryParse(userMarkup, out UserMarkUp_Value);
        //        string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
        //        string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
        //        string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
        //        decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
        //        decimal.TryParse(GSTValue, out GSTAmount);
        //        decimal.TryParse(HOLDCHARGES, out Holding_Amount);
        //        string BookingDate = string.Empty;
        //        string Ref_no = string.Empty;
        //        string PNR = string.Empty;
        //        string adult = string.Empty;
        //        int AdultVal = 0;
        //        int childVal = 0;
        //        int InfantVal = 0;
        //        string child = string.Empty;
        //        string infant = string.Empty;
        //        bool IsDomestic = false;
        //        string TicketType = string.Empty;
        //        string Airlinecode = string.Empty;
        //        string FlightNo = string.Empty;
        //        string FromAirport = string.Empty;
        //        string ToAirport = string.Empty;
        //        string DeptDate = string.Empty;
        //        string Depttime = string.Empty;
        //        string arivDate = string.Empty;
        //        string arivtime = string.Empty;
        //        string TotalFlightBaseFare = string.Empty;
        //        decimal TotalBaseFare = 0;
        //        string TotalFlightTax = string.Empty;
        //        decimal TotalTaxFare = 0;
        //        string TotalFlightPassengerTax = string.Empty;
        //        decimal TotalPngsTaxFare = 0;
        //        string TotalFlightAdditionalCharges = string.Empty;
        //        decimal TotalAdditionalFare = 0;
        //        string TotalFlightCuteFee = string.Empty;
        //        decimal TotalCuteFare = 0;
        //        string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
        //        decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
        //        string TotalFlightAmount = string.Empty;
        //        decimal TotalAmt = 0;
        //        string TotalCommissionAMtCharge = string.Empty;
        //        decimal TotalCommAmt = 0;
        //        string TotalServiceCharge = string.Empty;
        //        decimal TotalServiceAmt = 0;
        //        string TDSAmount = string.Empty;
        //        decimal TotalTDSAmountAmt = 0;
        //        string ServiceTax = string.Empty;
        //        decimal TotalServiceTaxAmt = 0;
        //        string AdultCheckedIn = string.Empty;
        //        string MainClass = string.Empty;
        //        string BookingClass = string.Empty;
        //        string BookingStatus = string.Empty;
        //        decimal AdminGST = 0;
        //        decimal Publish_Fare = 0;
        //        decimal NET_FARE = 0;
        //        decimal NET_TOTAL_FARE = 0;
        //        decimal RESCHEDULE_FARE = 0;
        //        string ResValue = string.Empty;
        //        dynamic VerifyFlight = null;
        //        using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
        //        {
        //            try
        //            {

        //                AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
        //                decimal UserMarkupGST = 0;
        //                UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);
        //                if (TripMode == "O")
        //                {
        //                    TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
        //                    {
        //                        MEM_ID = CurrentMerchant.MEM_ID,
        //                        DIST_ID = getmemberinfo.INTRODUCER,
        //                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //                        CORELATION_ID = COrelationID,
        //                        PNR = PNR,
        //                        REF_NO = Ref_no,
        //                        TRACK_NO = "",
        //                        TRIP_MODE = TripMode,
        //                        TICKET_NO = Ref_no,
        //                        TICKET_TYPE = TripMode,
        //                        IS_DOMESTIC = false,
        //                        AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
        //                        FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
        //                        FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
        //                        TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
        //                        BOOKING_DATE = DateTime.Now,
        //                        DEPT_DATE = DeptDate,
        //                        DEPT_TIME = Depttime,
        //                        ARRIVE_DATE = arivDate,
        //                        ARRIVE_TIME = arivtime,
        //                        NO_OF_ADULT = AdultVal,
        //                        NO_OF_CHILD = childVal,
        //                        NO_OF_INFANT = InfantVal,
        //                        TOTAL_FLIGHT_BASE_FARE = 0,
        //                        TOTAL_FLIGHT_TAX = 0,
        //                        TOTAL_PASSANGER_TAX = 0,
        //                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
        //                        TOTAL_FLIGHT_CUTE_FEE = 0,
        //                        TOTAL_FLIGHT_MEAL_FEE = 0,
        //                        TOTAL_AIRPORT_FEE = 0,
        //                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
        //                        TOTAL_COMMISSION_AMT = TotalCommAmt,
        //                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //                        STATUS = true,
        //                        IS_CANCELLATION = false,
        //                        FLIGHT_CANCELLATION_ID = "",
        //                        IS_HOLD = false,
        //                        API_RESPONSE = "",
        //                        FLIGHT_BOOKING_DATE = BookingDate,
        //                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
        //                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
        //                        BOOKING_STATUS = "PENDING",
        //                        USER_MARKUP = UserMarkUp_Value,
        //                        ADMIN_MARKUP = AirAdditionalCharge,
        //                        COMM_SLAP = 0,
        //                        ADMIN_GST = AdminGST,
        //                        ADMIN_cGST = 0,
        //                        ADMIN_sGST = 0,
        //                        ADMIN_iGST = 0,
        //                        USER_MARKUP_GST = UserMarkupGST,
        //                        USER_MARKUP_cGST = 0,
        //                        USER_MARKUP_sGST = 0,
        //                        USER_MARKUP_iGST = 0,
        //                        OP_MODE = "BOOKED",
        //                        HOLD_CHARGE = 0,
        //                        HOLD_CGST = 0,
        //                        HOLD_IGST = 0,
        //                        HOLD_SGST = 0,
        //                        PASSAGER_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
        //                        API_REQUEST = req,
        //                        STOPAGE = (Deptstopage - 1),
        //                        COMPANY_NAME = GSTCompanyName,
        //                        COMPANY_EMAIL_ID = GSTCompanyEmail,
        //                        COMPANY_GST_NO = GSTNO,
        //                        COMPANY_MOBILE = GSTMOBILE_NO,
        //                        COMPANY_GST_ADDRESS = GSTADDRESS,
        //                    };
        //                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
        //                    //_db.SaveChanges();
        //                    foreach (var pnsgitem in PassngFlight.Passenger)
        //                    {
        //                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        var val_Pnsg_Details = "";
        //                        string Pngdetails = "";
        //                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
        //                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
        //                        {
        //                            MEM_ID = CurrentMerchant.MEM_ID,
        //                            REF_NO = Ref_no,
        //                            PNR = PNR,
        //                            TITLE = pnsgitem.Title,
        //                            FIRST_NAME = pnsgitem.FirstName,
        //                            LAST_NAME = pnsgitem.LastName,
        //                            PASSENGER_TYPE = pnsgitem.PassengerType,
        //                            GENDER = "",
        //                            BIRTH_DATE = PngDOB,
        //                            DETAILS = Pngdetails,
        //                            PASSENGER_RESP = "",
        //                            CREATE_DATE = DateTime.Now,
        //                            PNSG_SEQ_NO = PnsgSeq_No,
        //                            CORELATION_ID = COrelationID,
        //                            PASSENGER_STATUS = "PENDING",
        //                            CancelReqNo = "",
        //                            TRIP_TYPE = "O",
        //                            FLIGHT_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
        //                            FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
        //                            TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
        //                        };
        //                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
        //                        //_db.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
        //                    {
        //                        MEM_ID = CurrentMerchant.MEM_ID,
        //                        DIST_ID = getmemberinfo.INTRODUCER,
        //                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //                        CORELATION_ID = COrelationID,
        //                        PNR = PNR,
        //                        REF_NO = Ref_no,
        //                        TRACK_NO = "",
        //                        TRIP_MODE = TripMode,
        //                        TICKET_NO = Ref_no,
        //                        TICKET_TYPE = TripMode,
        //                        IS_DOMESTIC = false,
        //                        //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
        //                        //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
        //                        AIRLINE_CODE = Dept_airlineCode,
        //                        FLIGHT_NO = Dept_FlightNo,
        //                        //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
        //                        //FROM_AIRPORT = RetdeptFromAir,
        //                        //TO_AIRPORT = RetdeptToAir,
        //                        FROM_AIRPORT = deptureSegment[0].FromAirportCode,
        //                        TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
        //                        //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
        //                        BOOKING_DATE = DateTime.Now,
        //                        DEPT_DATE = DeptDate,
        //                        DEPT_TIME = Depttime,
        //                        ARRIVE_DATE = arivDate,
        //                        ARRIVE_TIME = arivtime,
        //                        NO_OF_ADULT = AdultVal,
        //                        NO_OF_CHILD = childVal,
        //                        NO_OF_INFANT = InfantVal,
        //                        TOTAL_FLIGHT_BASE_FARE = 0,
        //                        TOTAL_FLIGHT_TAX = 0,
        //                        TOTAL_PASSANGER_TAX = 0,
        //                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
        //                        TOTAL_FLIGHT_CUTE_FEE = 0,
        //                        TOTAL_FLIGHT_MEAL_FEE = 0,
        //                        TOTAL_AIRPORT_FEE = 0,
        //                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
        //                        TOTAL_COMMISSION_AMT = TotalCommAmt,
        //                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //                        STATUS = true,
        //                        IS_CANCELLATION = false,
        //                        FLIGHT_CANCELLATION_ID = "",
        //                        IS_HOLD = false,
        //                        API_RESPONSE = "",
        //                        FLIGHT_BOOKING_DATE = BookingDate,
        //                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
        //                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
        //                        BOOKING_STATUS = "PENDING",
        //                        USER_MARKUP = UserMarkUp_Value,
        //                        ADMIN_MARKUP = AirAdditionalCharge,
        //                        COMM_SLAP = 0,
        //                        ADMIN_GST = AdminGST,
        //                        ADMIN_cGST = 0,
        //                        ADMIN_sGST = 0,
        //                        ADMIN_iGST = 0,
        //                        USER_MARKUP_GST = UserMarkupGST,
        //                        USER_MARKUP_cGST = 0,
        //                        USER_MARKUP_sGST = 0,
        //                        USER_MARKUP_iGST = 0,
        //                        OP_MODE = "BOOKED",
        //                        HOLD_CHARGE = 0,
        //                        HOLD_CGST = 0,
        //                        HOLD_IGST = 0,
        //                        HOLD_SGST = 0,
        //                        PASSAGER_SEGMENT = SeqNoDept,
        //                        API_REQUEST = req,
        //                        STOPAGE = (cntDept - 1),
        //                        COMPANY_NAME = GSTCompanyName,
        //                        COMPANY_EMAIL_ID = GSTCompanyEmail,
        //                        COMPANY_GST_NO = GSTNO,
        //                        COMPANY_MOBILE = GSTMOBILE_NO,
        //                        COMPANY_GST_ADDRESS = GSTADDRESS,
        //                    };
        //                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
        //                    TBL_FLIGHT_BOOKING_DETAILS objflightreturn = new TBL_FLIGHT_BOOKING_DETAILS()
        //                    {
        //                        MEM_ID = CurrentMerchant.MEM_ID,
        //                        DIST_ID = getmemberinfo.INTRODUCER,
        //                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //                        CORELATION_ID = COrelationID,
        //                        PNR = PNR,
        //                        REF_NO = Ref_no,
        //                        TRACK_NO = "",
        //                        TRIP_MODE = "",
        //                        TICKET_NO = Ref_no,
        //                        TICKET_TYPE = "",
        //                        IS_DOMESTIC = false,
        //                        //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
        //                        //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
        //                        AIRLINE_CODE = retn_airlineCode,
        //                        FLIGHT_NO = retn_FlightNo,
        //                        //FROM_AIRPORT = RettripFromAir,
        //                        //TO_AIRPORT = RettripToAir,
        //                        FROM_AIRPORT = retSegment[0].FromAirportCode,
        //                        TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
        //                        //FROM_AIRPORT = retSegment.Segment[0].FromAirportCode,
        //                        //TO_AIRPORT = retSegment.Segment[Flightcount - 1].ToAirportCode,
        //                        BOOKING_DATE = DateTime.Now,
        //                        DEPT_DATE = DeptDate,
        //                        DEPT_TIME = Depttime,
        //                        ARRIVE_DATE = arivDate,
        //                        ARRIVE_TIME = arivtime,
        //                        NO_OF_ADULT = AdultVal,
        //                        NO_OF_CHILD = childVal,
        //                        NO_OF_INFANT = InfantVal,
        //                        TOTAL_FLIGHT_BASE_FARE = 0,
        //                        TOTAL_FLIGHT_TAX = 0,
        //                        TOTAL_PASSANGER_TAX = 0,
        //                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
        //                        TOTAL_FLIGHT_CUTE_FEE = 0,
        //                        TOTAL_FLIGHT_MEAL_FEE = 0,
        //                        TOTAL_AIRPORT_FEE = 0,
        //                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
        //                        TOTAL_COMMISSION_AMT = TotalCommAmt,
        //                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //                        STATUS = true,
        //                        IS_CANCELLATION = false,
        //                        FLIGHT_CANCELLATION_ID = "",
        //                        IS_HOLD = false,
        //                        API_RESPONSE = "",
        //                        FLIGHT_BOOKING_DATE = BookingDate,
        //                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
        //                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
        //                        BOOKING_STATUS = "PENDING",
        //                        USER_MARKUP = UserMarkUp_Value,
        //                        ADMIN_MARKUP = AirAdditionalCharge,
        //                        COMM_SLAP = 0,
        //                        ADMIN_GST = AdminGST,
        //                        ADMIN_cGST = 0,
        //                        ADMIN_sGST = 0,
        //                        ADMIN_iGST = 0,
        //                        USER_MARKUP_GST = UserMarkupGST,
        //                        USER_MARKUP_cGST = 0,
        //                        USER_MARKUP_sGST = 0,
        //                        USER_MARKUP_iGST = 0,
        //                        OP_MODE = "BOOKED",
        //                        HOLD_CHARGE = 0,
        //                        HOLD_CGST = 0,
        //                        HOLD_IGST = 0,
        //                        HOLD_SGST = 0,
        //                        PASSAGER_SEGMENT = SeqNoRtn,
        //                        API_REQUEST = req,
        //                        STOPAGE = (cntretn - 1),
        //                        COMPANY_NAME = GSTCompanyName,
        //                        COMPANY_EMAIL_ID = GSTCompanyEmail,
        //                        COMPANY_GST_NO = GSTNO,
        //                        COMPANY_MOBILE = GSTMOBILE_NO,
        //                        COMPANY_GST_ADDRESS = GSTADDRESS,
        //                    };
        //                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightreturn);
        //                    //_db.SaveChanges();
        //                    foreach (var pnsgitem in PassngFlight.Passenger)
        //                    {
        //                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        var val_Pnsg_Details = "";
        //                        string Pngdetails = "";
        //                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
        //                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
        //                        {
        //                            MEM_ID = CurrentMerchant.MEM_ID,
        //                            REF_NO = Ref_no,
        //                            PNR = PNR,
        //                            TITLE = pnsgitem.Title,
        //                            FIRST_NAME = pnsgitem.FirstName,
        //                            LAST_NAME = pnsgitem.LastName,
        //                            PASSENGER_TYPE = pnsgitem.PassengerType,
        //                            GENDER = "",
        //                            BIRTH_DATE = PngDOB,
        //                            DETAILS = Pngdetails,
        //                            PASSENGER_RESP = "",
        //                            CREATE_DATE = DateTime.Now,
        //                            PNSG_SEQ_NO = PnsgSeq_No,
        //                            CORELATION_ID = COrelationID,
        //                            PASSENGER_STATUS = "PENDING",
        //                            CancelReqNo = "",
        //                            TRIP_TYPE = "O",
        //                            DOJ = R_DeptDate,
        //                            FLIGHT_SEGMENT = SeqNoDept,
        //                            FROM_AIRPORT = deptureSegment[0].FromAirportCode,
        //                            TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
        //                        };
        //                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
        //                        //_db.SaveChanges();
        //                    }
        //                    foreach (var pnsgitem in PassngFlight.Passenger)
        //                    {
        //                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        var val_Pnsg_Details = "";
        //                        string Pngdetails = "";
        //                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
        //                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
        //                        {
        //                            MEM_ID = CurrentMerchant.MEM_ID,
        //                            REF_NO = Ref_no,
        //                            PNR = PNR,
        //                            TITLE = pnsgitem.Title,
        //                            FIRST_NAME = pnsgitem.FirstName,
        //                            LAST_NAME = pnsgitem.LastName,
        //                            PASSENGER_TYPE = pnsgitem.PassengerType,
        //                            GENDER = "",
        //                            BIRTH_DATE = PngDOB,
        //                            DETAILS = Pngdetails,
        //                            PASSENGER_RESP = "",
        //                            CREATE_DATE = DateTime.Now,
        //                            PNSG_SEQ_NO = PnsgSeq_No,
        //                            CORELATION_ID = COrelationID,
        //                            PASSENGER_STATUS = "PENDING",
        //                            CancelReqNo = "",
        //                            TRIP_TYPE = "R",
        //                            FLIGHT_SEGMENT = SeqNoRtn,
        //                            DOJ = R_RetnDate,
        //                            FROM_AIRPORT = retSegment[0].FromAirportCode,
        //                            TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
        //                        };
        //                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
        //                        //_db.SaveChanges();
        //                    }
        //                }
        //                decimal mmainBlc = 0;
        //                decimal SubMainBlc = 0;
        //                decimal Closing = 0;
        //                decimal mainClosing = 0;
        //                decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
        //                //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //                if (getmemberinfo.BALANCE != null)
        //                {
        //                    decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
        //                    //SubMainBlc = mmainBlc - TotalAmt;  //TotalBookAmt
        //                    SubMainBlc = mmainBlc - T_Amount;
        //                    getmemberinfo.BALANCE = SubMainBlc;
        //                }
        //                else
        //                {
        //                    //SubMainBlc = mmainBlc - TotalAmt;
        //                    SubMainBlc = mmainBlc - T_Amount;
        //                    getmemberinfo.BALANCE = SubMainBlc;
        //                }

        //                getmemberinfo.BALANCE = SubMainBlc;
        //                _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
        //                //_db.SaveChanges();
        //                //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
        //                if (MemberAcntLog != null)
        //                {
        //                    Closing = MemberAcntLog.CLOSING;
        //                    //mainClosing = Closing - TotalAmt;
        //                    mainClosing = Closing - T_Amount;
        //                }
        //                else
        //                {
        //                    Closing = MemberAcntLog.CLOSING;
        //                    //mainClosing = Closing - TotalAmt;
        //                    mainClosing = Closing - T_Amount;
        //                }
        //                TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                {
        //                    API_ID = 0,
        //                    MEM_ID = CurrentMerchant.MEM_ID,
        //                    MEMBER_TYPE = "RETAILER",
        //                    TRANSACTION_TYPE = "FLIGHT BOOKING",
        //                    TRANSACTION_DATE = System.DateTime.Now,
        //                    TRANSACTION_TIME = DateTime.Now,
        //                    DR_CR = "DR",
        //                    //AMOUNT = TotalAmt,
        //                    AMOUNT = T_Amount,
        //                    NARRATION = "Debit amount for flight booking",
        //                    OPENING = Closing,
        //                    CLOSING = mainClosing,
        //                    REC_NO = 0,
        //                    COMM_AMT = 0,
        //                    GST = (float)GSTAmount,
        //                    TDS = 0,
        //                    IPAddress = "",
        //                    TDS_PERCENTAGE = 0,
        //                    //GST_PERCENTAGE = AdminGST,
        //                    GST_PERCENTAGE = 0,
        //                    WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
        //                    //SUPER_ID = (long)SUP_MEM_ID,
        //                    SUPER_ID = 0,
        //                    DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
        //                    SERVICE_ID = 0,
        //                    CORELATIONID = COrelationID,
        //                    REC_COMM_TYPE = "",
        //                    COMM_VALUE = 0,
        //                    NET_COMM_AMT = 0,
        //                    TDS_DR_COMM_AMT = 0,
        //                    CGST_COMM_AMT_INPUT = 0,
        //                    CGST_COMM_AMT_OUTPUT = 0,
        //                    SGST_COMM_AMT_INPUT = 0,
        //                    SGST_COMM_AMT_OUTPUT = 0,
        //                    IGST_COMM_AMT_INPUT = 0,
        //                    IGST_COMM_AMT_OUTPUT = 0,
        //                    TOTAL_GST_COMM_AMT_INPUT = 0,
        //                    TOTAL_GST_COMM_AMT_OUTPUT = 0,
        //                    TDS_RATE = 0,
        //                    CGST_RATE = 0,
        //                    SGST_RATE = 0,
        //                    IGST_RATE = 0,
        //                    TOTAL_GST_RATE = 0,
        //                    COMM_SLAB_ID = 0,
        //                    STATE_ID = getmemberinfo.STATE_ID,
        //                    FLAG1 = 0,
        //                    FLAG2 = 0,
        //                    FLAG3 = 0,
        //                    FLAG4 = 0,
        //                    FLAG5 = 0,
        //                    FLAG6 = 0,
        //                    FLAG7 = 0,
        //                    FLAG8 = 0,
        //                    FLAG9 = 0,
        //                    FLAG10 = 0,
        //                    VENDOR_ID = 0
        //                };
        //                _db.TBL_ACCOUNTS.Add(objCommPer);
        //                _db.SaveChanges();
        //                #endregion
        //                #region Booking Api call
        //                //dynamic VerifyFlight = null;
        //                VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
        //                ResValue = Convert.ToString(VerifyFlight);
        //                //var APIStatus = VerifyFlight.BookTicketResponse.Error;
        //                var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
        //                if (getStatus == "Acknowledged" || getStatus == "Completed")
        //                {
        //                    ContextTransaction.Commit();
        //                }
        //                else
        //                {
        //                    ContextTransaction.Rollback();
        //                    return Json("Please try again later", JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ContextTransaction.Rollback();
        //                //string check = RefundAmount(COrelationID, TripMode);
        //                return Json(false, JsonRequestBehavior.AllowGet);
        //                throw;
        //            }
        //        }
        //        using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(ResValue);
        //                int count = BookingResponse.BookTicketResponses.BookTicketResponse.Count;
        //                var data = JsonConvert.SerializeObject(VerifyFlight);
        //                string APIRes = Convert.ToString(data);
        //                var TicketInfo = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();
        //                var Ticketcount = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.Count;
        //                var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
        //                var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
        //                var PassengerDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails.ToList();
        //                var lstpnsg = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails;
        //                var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
        //                string pnsgRes = Convert.ToString(val_Pnsg);
        //                BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
        //                BookingStatus = TicketInfo[Ticketcount - 1].Status;
        //                Ref_no = TicketInfo[Ticketcount - 1].RefNo;
        //                PNR = FareDetails[FareDetailsCount - 1].AirlinePNRNumber;
        //                TicketType = TicketInfo[Ticketcount - 1].TicketType;
        //                IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
        //                Airlinecode = FareDetails[0].AirlineCode;
        //                FlightNo = FareDetails[0].FlightNo;
        //                MainClass = FareDetails[0].MainClass;
        //                BookingClass = FareDetails[0].BookingClass;
        //                FromAirport = FareDetails[0].FromAirportCode;
        //                ToAirport = FareDetails[FareDetailsCount - 1].ToAirportCode;
        //                DeptDate = FareDetails[0].DepartureDate;
        //                string DATEofJourney = DeptDate;
        //                DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
        //                //DateTime dateofJourey =Convert.ToDateTime(DATEofJourney);
        //                DeptDate = FareDetails[0].DepartureTime;
        //                arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
        //                arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
        //                adult = TicketInfo[Ticketcount - 1].Adult;
        //                AdultVal = int.Parse(adult);
        //                child = TicketInfo[Ticketcount - 1].Child;
        //                childVal = int.Parse(child);
        //                infant = TicketInfo[Ticketcount - 1].Infant;
        //                InfantVal = int.Parse(infant);
        //                string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
        //                decimal Additional_AMt = 0;
        //                decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
        //                TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
        //                Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;

        //                TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
        //                TotalFlightTax = FareDetails[0].TotalFlightTax;
        //                TotalTaxFare = decimal.Parse(TotalFlightTax);
        //                TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
        //                TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
        //                TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
        //                TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
        //                TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
        //                TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
        //                TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
        //                TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
        //                TotalFlightAmount = FareDetails[0].TotalFlightAmount;
        //                TotalAmt = decimal.Parse(TotalFlightAmount);
        //                NET_FARE = TotalAmt;
        //                NET_TOTAL_FARE = NET_FARE + Additional_AMt;
        //                TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
        //                TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
        //                TotalServiceCharge = FareDetails[0].TotalServiceCharge;
        //                TotalServiceAmt = decimal.Parse(TotalServiceCharge);
        //                TDSAmount = FareDetails[0].TDSAmount;
        //                TotalTDSAmountAmt = decimal.Parse(TDSAmount);
        //                ServiceTax = FareDetails[0].ServiceTax;
        //                TotalServiceTaxAmt = decimal.Parse(ServiceTax);
        //                AdultCheckedIn = FareDetails[0].AdultCheckedIn;
        //                string jhjdfd = "";
        //                var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
        //                if (GetFligtInfo != null)
        //                {
        //                    foreach (var ticketInfo in GetFligtInfo)
        //                    {
        //                        var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
        //                        flightTicketInfo.PNR = PNR;
        //                        flightTicketInfo.REF_NO = Ref_no;
        //                        flightTicketInfo.TRACK_NO = "";
        //                        flightTicketInfo.TRIP_MODE = "1";
        //                        flightTicketInfo.TICKET_TYPE = TicketType;
        //                        flightTicketInfo.TICKET_NO = Ref_no;
        //                        flightTicketInfo.IS_DOMESTIC = IsDomestic;
        //                        //flightTicketInfo.AIRLINE_CODE = Airlinecode;
        //                        //flightTicketInfo.FLIGHT_NO = FlightNo;
        //                        //flightTicketInfo.FROM_AIRPORT = FromAirport;
        //                        //flightTicketInfo.TO_AIRPORT = ToAirport;
        //                        flightTicketInfo.BOOKING_DATE = DateTime.Now;
        //                        flightTicketInfo.DEPT_DATE = DeptDate;
        //                        flightTicketInfo.DEPT_TIME = Depttime;
        //                        flightTicketInfo.ARRIVE_DATE = arivDate;
        //                        flightTicketInfo.ARRIVE_TIME = arivtime;
        //                        flightTicketInfo.NO_OF_ADULT = AdultVal;
        //                        flightTicketInfo.NO_OF_CHILD = childVal;
        //                        flightTicketInfo.NO_OF_INFANT = InfantVal;
        //                        flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
        //                        flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
        //                        flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
        //                        flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
        //                        flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
        //                        flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
        //                        flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
        //                        flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
        //                        flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
        //                        flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
        //                        flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
        //                        flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
        //                        flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
        //                        flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
        //                        flightTicketInfo.STATUS = true;
        //                        flightTicketInfo.IS_CANCELLATION = false;
        //                        flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
        //                        flightTicketInfo.IS_HOLD = false;
        //                        flightTicketInfo.API_RESPONSE = APIRes;
        //                        flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
        //                        flightTicketInfo.BOOKING_STATUS = BookingStatus;
        //                        flightTicketInfo.MAIN_CLASS = MainClass;
        //                        flightTicketInfo.BOOKING_CLASS = BookingClass;
        //                        flightTicketInfo.PUBLISH_FARE = Publish_Fare;
        //                        flightTicketInfo.NET_FARE = NET_FARE;
        //                        flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
        //                        flightTicketInfo.CANCELLATION_REMARK = "";
        //                        flightTicketInfo.RESCHEDULE_FARE = false;
        //                        flightTicketInfo.RESCHEDULE_REMARK = "";
        //                        _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
        //                        //_db.SaveChanges();
        //                        //ContextTransaction.Commit();
        //                    }

        //                }
        //                if (TripMode == "O")
        //                {
        //                    foreach (var pnsgitem in PassengerDetails)
        //                    {
        //                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
        //                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
        //                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName);
        //                        if (PnsgList != null)
        //                        {
        //                            PnsgList.PNR = PNR;
        //                            PnsgList.REF_NO = Ref_no;
        //                            PnsgList.DETAILS = Pngdetails;
        //                            PnsgList.GENDER = pnsgitem.Gender;
        //                            PnsgList.PASSENGER_RESP = pnsgRes;
        //                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
        //                            PnsgList.PASSENGER_STATUS = BookingStatus;
        //                            PnsgList.DOJ = dateofJourey;
        //                            PnsgList.CANCELLATION_REMARTK = "";
        //                            PnsgList.PASSPORT = "";
        //                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
        //                            //_db.SaveChanges();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var pnsgitem in PassengerDetails)
        //                    {
        //                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
        //                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
        //                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
        //                        if (PnsgList != null)
        //                        {
        //                            PnsgList.PNR = PNR;
        //                            PnsgList.REF_NO = Ref_no;
        //                            PnsgList.DETAILS = Pngdetails;
        //                            PnsgList.GENDER = pnsgitem.Gender;
        //                            PnsgList.PASSENGER_RESP = pnsgRes;
        //                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
        //                            PnsgList.PASSENGER_STATUS = BookingStatus;
        //                            PnsgList.CANCELLATION_REMARTK = "";
        //                            PnsgList.PASSPORT = "";
        //                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
        //                            //_db.SaveChanges();
        //                        }
        //                    }
        //                    foreach (var pnsgitem in PassengerDetails)
        //                    {
        //                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
        //                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
        //                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "R");
        //                        if (PnsgList != null)
        //                        {
        //                            PnsgList.PNR = PNR;
        //                            PnsgList.REF_NO = Ref_no;
        //                            PnsgList.DETAILS = Pngdetails;
        //                            PnsgList.GENDER = pnsgitem.Gender;
        //                            PnsgList.PASSENGER_RESP = pnsgRes;
        //                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
        //                            PnsgList.PASSENGER_STATUS = BookingStatus;
        //                            PnsgList.CANCELLATION_REMARTK = "";
        //                            PnsgList.PASSPORT = "";
        //                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
        //                            //_db.SaveChanges();
        //                        }
        //                    }
        //                }
        //                _db.SaveChanges();
        //                ContextTransaction.Commit();
        //                #endregion
        //                TempData["IsShowPrintTicket"] = "Show";
        //                return Json(data, JsonRequestBehavior.AllowGet);
        //            }
        //            catch (Exception ex)
        //            {
        //                ContextTransaction.Rollback();
        //                //string check = RefundAmount(COrelationID, TripMode);
        //                return Json(false, JsonRequestBehavior.AllowGet);

        //                throw;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        #region Flight holding Request
        [HttpPost]
        public JsonResult FlightHoldingRequest(string req, string userMarkup, string FlightAmt, string TripMode, string NetAmount, string ISFlightType, string INTPancard, string deptSegment = "", string returnSegment = "")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());

            int Deptstopage = 0;
            int deptcnt = 0;
            int retncnt = 0;
            decimal TotalBookAmt = 0;
            decimal Publish_Fare = 0;
            decimal NET_FARE = 0;
            decimal NET_TOTAL_FARE = 0;
            decimal RESCHEDULE_FARE = 0;
            decimal.TryParse(FlightAmt, out TotalBookAmt);

            string HLD_VAlue = "";

            decimal Hold_AMountValue = 0;
            //FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
            FlightHoldingReqDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightHoldingReqDTO>(req);
            var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
            Deptstopage = FlightInfo.Segment.Count();
            var sengList = FlightInfo.Segment.ToList();
            string Dept_FlightNo = string.Empty;
            string Dept_airlineCode = string.Empty;
            string retn_FlightNo = string.Empty;
            string retn_airlineCode = string.Empty;
            HLD_VAlue = beforeApiExecute.RequestXml.BookTicketRequest.HoldCharge;
            decimal.TryParse(HLD_VAlue,out Hold_AMountValue);
            int cntDept = 0;
            int cntretn = 0;
            string SeqNoDept = "";
            string SeqNoRtn = "";
            DateTime R_DeptDate = new DateTime();
            DateTime R_RetnDate = new DateTime();
            if (TripMode == "R")
            {
                var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
                var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
                Dept_FlightNo = SegInfodept[0].FlightNo;
                Dept_airlineCode = SegInfodept[0].AirlineCode;
                retn_FlightNo = SegInforeturn[0].FlightNo;
                retn_airlineCode = SegInforeturn[0].AirlineCode;
                cntDept = SegInfodept.Count();
                cntretn = SegInforeturn.Count();
                SeqNoDept = SegInfodept[0].SegmentSeqNo;
                SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
                R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
                R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
            }
            var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
            var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
            var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
            int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
            string GSTCompanyName = string.Empty;
            string GSTCompanyEmail = string.Empty;
            string GSTNO = string.Empty;
            string GSTMOBILE_NO = string.Empty;
            string GSTADDRESS = string.Empty;
            //GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
            //GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
            //GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
            //GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
            //GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
            decimal AirAdditionalCharge = 0;
            decimal GSTAmount = 0;
            decimal Holding_Amount = 0;
            decimal UserMarkUp_Value = 0;
            decimal.TryParse(userMarkup, out UserMarkUp_Value);
            //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
            string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
            string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
            string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
            decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
            decimal.TryParse(GSTValue, out GSTAmount);
            decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
            decimal AdminGST = 0;
            dynamic VerifyFlight = null;
            string ResValue = string.Empty;

            decimal TotalNetAmount = 0;
            decimal FlightNetAmount = 0;
            decimal AgentCommAmt = 0;
            decimal AgentCommTDS = 0;
            decimal TCSAmt = 0;

            decimal BlockBal = 0;
            decimal AddMainBal = 0;
            decimal MainBALL = 0;
            List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
            List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
            if (deptureSegment != null)
            { deptcnt = deptureSegment.Count(); }
            if (retSegment != null)
            {
                retncnt = retSegment.Count();
            }
            var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            decimal.TryParse(getmemberinfo.BLOCKED_BALANCE.ToString(), out BlockBal);
            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out MainBALL);
            AddMainBal = MainBALL - BlockBal;
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            if (AddMainBal > TotalBookAmt)
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            {
                using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        #region variable Declieare   
                        AdminGST = ((Holding_Amount * GSTAmount) / 118);
                        decimal UserMarkupGST = 0;
                        UserMarkupGST = ((Holding_Amount * GSTAmount) / 118);

                        decimal.TryParse(NetAmount, out FlightNetAmount);
                        AgentCommAmt = FlightNetAmount - TotalBookAmt;
                        AgentCommTDS = ((AgentCommAmt * 5) / 100);
                        TotalNetAmount = FlightNetAmount + AgentCommTDS;
                        if (ISFlightType == "International")
                        {
                            if (INTPancard != "")
                            {
                                TCSAmt = ((TotalBookAmt * 5) / 100);
                            }
                            else
                            {
                                TCSAmt = ((TotalBookAmt * 10) / 100);
                            }
                        }
                        else
                        {
                            TCSAmt = 0;
                        }

                        if (TripMode == "O")
                        {
                            TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = "",
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = "",
                                IS_DOMESTIC = false,
                                AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
                                TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = true,
                                BOOKING_HOLD_ID = Ref_no,
                                HOLD_DATE = DateTime.Now,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "HOLD",
                                HOLD_CHARGE = Hold_AMountValue,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
                                API_REQUEST = req,
                                STOPAGE = (Deptstopage - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE = TotalNetAmount,
                                FARE_COMMISSION = AgentCommAmt,
                                FARE_COMMISSION_TDS = AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                                INT_FLIGHT_PANCARD = INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                            //_db.SaveChanges();
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "O",
                                    FLIGHT_SEGMENT = FlightInfo.Segment[0].SegmentSeqNo,
                                    FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
                                    TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                        }
                        else
                        {
                            TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = "",
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = "",
                                IS_DOMESTIC = false,
                                //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                AIRLINE_CODE = Dept_airlineCode,
                                FLIGHT_NO = Dept_FlightNo,
                                //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
                                //FROM_AIRPORT = RetdeptFromAir,
                                //TO_AIRPORT = RetdeptToAir,
                                FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                                TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
                                //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = true,
                                BOOKING_HOLD_ID = Ref_no,
                                HOLD_DATE = DateTime.Now,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "HOLD",
                                HOLD_CHARGE = 0,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = SeqNoDept,
                                API_REQUEST = req,
                                STOPAGE = (cntDept - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE = TotalNetAmount,
                                FARE_COMMISSION = AgentCommAmt,
                                FARE_COMMISSION_TDS = AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                                INT_FLIGHT_PANCARD = INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
                            TBL_FLIGHT_BOOKING_DETAILS objflightreturn = new TBL_FLIGHT_BOOKING_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                DIST_ID = getmemberinfo.INTRODUCER,
                                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                                CORELATION_ID = COrelationID,
                                PNR = PNR,
                                REF_NO = Ref_no,
                                TRACK_NO = "",
                                TRIP_MODE = "",
                                TICKET_NO = Ref_no,
                                TICKET_TYPE = "",
                                IS_DOMESTIC = false,
                                //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                                //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                                AIRLINE_CODE = retn_airlineCode,
                                FLIGHT_NO = retn_FlightNo,
                                //FROM_AIRPORT = RettripFromAir,
                                //TO_AIRPORT = RettripToAir,
                                FROM_AIRPORT = retSegment[0].FromAirportCode,
                                TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
                                //FROM_AIRPORT = retSegment.Segment[0].FromAirportCode,
                                //TO_AIRPORT = retSegment.Segment[Flightcount - 1].ToAirportCode,
                                BOOKING_DATE = DateTime.Now,
                                DEPT_DATE = DeptDate,
                                DEPT_TIME = Depttime,
                                ARRIVE_DATE = arivDate,
                                ARRIVE_TIME = arivtime,
                                NO_OF_ADULT = AdultVal,
                                NO_OF_CHILD = childVal,
                                NO_OF_INFANT = InfantVal,
                                TOTAL_FLIGHT_BASE_FARE = 0,
                                TOTAL_FLIGHT_TAX = 0,
                                TOTAL_PASSANGER_TAX = 0,
                                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                                TOTAL_FLIGHT_CUTE_FEE = 0,
                                TOTAL_FLIGHT_MEAL_FEE = 0,
                                TOTAL_AIRPORT_FEE = 0,
                                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                                TOTAL_COMMISSION_AMT = TotalCommAmt,
                                TOTAL_TDS_AMT = TotalTDSAmountAmt,
                                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                                STATUS = true,
                                IS_CANCELLATION = false,
                                FLIGHT_CANCELLATION_ID = "",
                                IS_HOLD = true,
                                BOOKING_HOLD_ID = Ref_no,
                                HOLD_DATE = DateTime.Now,
                                API_RESPONSE = "",
                                FLIGHT_BOOKING_DATE = BookingDate,
                                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                                BOOKING_STATUS = "PENDING",
                                USER_MARKUP = UserMarkUp_Value,
                                ADMIN_MARKUP = AirAdditionalCharge,
                                COMM_SLAP = 0,
                                ADMIN_GST = AdminGST,
                                ADMIN_cGST = 0,
                                ADMIN_sGST = 0,
                                ADMIN_iGST = 0,
                                USER_MARKUP_GST = UserMarkupGST,
                                USER_MARKUP_cGST = 0,
                                USER_MARKUP_sGST = 0,
                                USER_MARKUP_iGST = 0,
                                OP_MODE = "HOLD",
                                HOLD_CHARGE = 0,
                                HOLD_CGST = 0,
                                HOLD_IGST = 0,
                                HOLD_SGST = 0,
                                PASSAGER_SEGMENT = SeqNoRtn,
                                API_REQUEST = req,
                                STOPAGE = (cntretn - 1),
                                COMPANY_NAME = GSTCompanyName,
                                COMPANY_EMAIL_ID = GSTCompanyEmail,
                                COMPANY_GST_NO = GSTNO,
                                COMPANY_MOBILE = GSTMOBILE_NO,
                                COMPANY_GST_ADDRESS = GSTADDRESS,
                                NET_COMM_FARE = TotalNetAmount,
                                FARE_COMMISSION = AgentCommAmt,
                                FARE_COMMISSION_TDS = AgentCommTDS,
                                TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                                INT_FLIGHT_PANCARD = INTPancard
                            };
                            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightreturn);
                           // _db.SaveChanges();
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "O",
                                    DOJ = R_DeptDate,
                                    FLIGHT_SEGMENT = SeqNoDept,
                                    FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                                    TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                            foreach (var pnsgitem in PassngFlight.Passenger)
                            {
                                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                var val_Pnsg_Details = "";
                                string Pngdetails = "";
                                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                                {
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    REF_NO = Ref_no,
                                    PNR = PNR,
                                    TITLE = pnsgitem.Title,
                                    FIRST_NAME = pnsgitem.FirstName,
                                    LAST_NAME = pnsgitem.LastName,
                                    PASSENGER_TYPE = pnsgitem.PassengerType,
                                    GENDER = "",
                                    BIRTH_DATE = PngDOB,
                                    DETAILS = Pngdetails,
                                    PASSENGER_RESP = "",
                                    CREATE_DATE = DateTime.Now,
                                    PNSG_SEQ_NO = PnsgSeq_No,
                                    CORELATION_ID = COrelationID,
                                    PASSENGER_STATUS = "PENDING",
                                    CancelReqNo = "",
                                    TRIP_TYPE = "R",
                                    FLIGHT_SEGMENT = SeqNoRtn,
                                    DOJ = R_RetnDate,
                                    FROM_AIRPORT = retSegment[0].FromAirportCode,
                                    TO_AIRPORT = retSegment[retncnt - 1].ToAirportCode,
                                };
                                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                                //_db.SaveChanges();
                            }
                        }
                        decimal mmainBlc = 0;
                        decimal SubMainBlc = 0;
                        decimal Closing = 0;
                        decimal mainClosing = 0;
                        //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                        if (getmemberinfo.BALANCE != null)
                        {
                            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                            SubMainBlc = mmainBlc - Holding_Amount;
                            getmemberinfo.BALANCE = SubMainBlc;
                        }
                        else
                        {
                            SubMainBlc = mmainBlc - Holding_Amount;
                            getmemberinfo.BALANCE = SubMainBlc;
                        }

                        getmemberinfo.BALANCE = SubMainBlc;
                        _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                       // _db.SaveChanges();
                        //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                        if (MemberAcntLog != null)
                        {
                            Closing = MemberAcntLog.CLOSING;
                            mainClosing = Closing - Holding_Amount;
                        }
                        else
                        {
                            Closing = MemberAcntLog.CLOSING;
                            mainClosing = Closing - Holding_Amount;
                        }
                        TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = CurrentMerchant.MEM_ID,
                            MEMBER_TYPE = "RETAILER",
                            TRANSACTION_TYPE = "FLIGHT BOOKING",
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            AMOUNT = Holding_Amount,
                            NARRATION = "Debit amount for flight Hold",
                            OPENING = Closing,
                            CLOSING = mainClosing,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = (float)GSTAmount,
                            TDS = 0,
                            IPAddress = "",
                            TDS_PERCENTAGE = 0,
                            //GST_PERCENTAGE = AdminGST,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                            //SUPER_ID = (long)SUP_MEM_ID,
                            SUPER_ID = 0,
                            DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE = 0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = getmemberinfo.STATE_ID,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0,
                            VENDOR_ID = 0
                        };
                        _db.TBL_ACCOUNTS.Add(objCommPer);
                        _db.SaveChanges();
                        #endregion
                        //string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        VerifyFlight = MultiLinkAirAPI.HoldingFlightTicket(req, COrelationID);
                        ResValue = Convert.ToString(VerifyFlight);
                        var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
                        if (getStatus == "On Hold")
                        {
                            ContextTransaction.Commit();
                        }
                        else
                        {
                            ContextTransaction.Rollback();
                            return Json("Please try again later", JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception)
                    {
                        ContextTransaction.Rollback();
                        //string check = RefundAmount(COrelationID, TripMode);
                        return Json(false, JsonRequestBehavior.AllowGet);
                        throw;
                    }
                }
                using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
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
                        string DATEofJourney = DeptDate;
                        //DateTime dateofJourey = Convert.ToDateTime(DeptDate);
                        DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
                        Depttime = FareDetails[0].DepartureTime;
                        arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                        arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                        adult = TicketInfo[Ticketcount - 1].Adult;
                        AdultVal = int.Parse(adult);
                        child = TicketInfo[Ticketcount - 1].Child;
                        childVal = int.Parse(child);
                        infant = TicketInfo[Ticketcount - 1].Infant;
                        InfantVal = int.Parse(infant);
                        //string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                        string AIRADDITIONALAMOUNT_Value = Session["AIRADDITIONALAMOUNT"].ToString();
                        decimal Additional_AMt = 0;
                        decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
                        TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                        Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;

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
                        NET_FARE = TotalAmt;
                        NET_TOTAL_FARE = NET_FARE + Additional_AMt;
                        TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                        TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                        TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                        TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                        TDSAmount = FareDetails[0].TDSAmount;
                        TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                        ServiceTax = FareDetails[0].ServiceTax;
                        TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                        AdultCheckedIn = FareDetails[0].AdultCheckedIn;
                        var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
                        if (GetFligtInfo != null)
                        {
                            foreach (var ticketInfo in GetFligtInfo)
                            {
                                var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
                                flightTicketInfo.PNR = PNR;
                                flightTicketInfo.REF_NO = Ref_no;
                                flightTicketInfo.TRACK_NO = "";
                                flightTicketInfo.TRIP_MODE = "1";
                                flightTicketInfo.TICKET_TYPE = TicketType;
                                flightTicketInfo.TICKET_NO = Ref_no;
                                flightTicketInfo.IS_DOMESTIC = IsDomestic;
                                flightTicketInfo.AIRLINE_CODE = Airlinecode;
                                //GetFligtInfo.FLIGHT_NO = FlightNo;
                                //GetFligtInfo.FROM_AIRPORT = FromAirport;
                                //GetFligtInfo.TO_AIRPORT = ToAirport;
                                flightTicketInfo.BOOKING_DATE = DateTime.Now;
                                flightTicketInfo.DEPT_DATE = DeptDate;
                                flightTicketInfo.DEPT_TIME = Depttime;
                                flightTicketInfo.ARRIVE_DATE = arivDate;
                                flightTicketInfo.ARRIVE_TIME = arivtime;
                                flightTicketInfo.NO_OF_ADULT = AdultVal;
                                flightTicketInfo.NO_OF_CHILD = childVal;
                                flightTicketInfo.NO_OF_INFANT = InfantVal;
                                flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
                                flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
                                flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
                                flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
                                flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
                                flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
                                flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
                                flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
                                flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
                                flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
                                flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
                                flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
                                flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
                                flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
                                flightTicketInfo.STATUS = true;
                                flightTicketInfo.IS_CANCELLATION = false;
                                flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
                                flightTicketInfo.API_RESPONSE = APIRes;
                                flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
                                flightTicketInfo.MAIN_CLASS = MainClass;
                                flightTicketInfo.BOOKING_CLASS = BookingClass;
                                flightTicketInfo.BOOKING_STATUS = BookingStatus;
                                flightTicketInfo.PUBLISH_FARE = Publish_Fare;
                                flightTicketInfo.NET_FARE = NET_FARE;
                                flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
                                flightTicketInfo.CANCELLATION_REMARK = "";
                                flightTicketInfo.RESCHEDULE_FARE = false;
                                flightTicketInfo.RESCHEDULE_REMARK = "";
                                _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
                                //_db.SaveChanges();
                            }
                        }
                        if (TripMode == "O")
                        {
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName);
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.DOJ = dateofJourey;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                   // _db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                    //_db.SaveChanges();
                                }
                            }
                            foreach (var pnsgitem in PassengerDetails)
                            {
                                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                                string Pngdetails = Convert.ToString(val_Pnsg_Details);
                                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                                string Png_LastName = Convert.ToString(pnsgitem.LastName);
                                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "R");
                                if (PnsgList != null)
                                {
                                    PnsgList.PNR = PNR;
                                    PnsgList.REF_NO = Ref_no;
                                    PnsgList.DETAILS = Pngdetails;
                                    PnsgList.GENDER = pnsgitem.Gender;
                                    PnsgList.PASSENGER_RESP = pnsgRes;
                                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                                    PnsgList.PASSENGER_STATUS = BookingStatus;
                                    PnsgList.CANCELLATION_REMARTK = "";
                                    PnsgList.PASSPORT = "";
                                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                                    //_db.SaveChanges();
                                }
                            }
                        }
                        _db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        ContextTransaction.Rollback();
                        //string check = RefundAmount(COrelationID, TripMode);
                        return Json(false, JsonRequestBehavior.AllowGet);
                        throw;
                    }
                }
            }
            else
            {
                return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult FlightHoldingRequest(string req, string userMarkup, string FlightAmt, string TripMode)
        //{
        //    try
        //    {
        //        decimal TotalBookAmt = 0;
        //        decimal.TryParse(FlightAmt, out TotalBookAmt);
        //        var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //        if (getmemberinfo.BALANCE > TotalBookAmt)
        //        {
        //            #region variable Declieare                
        //            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //            //FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
        //            FlightHoldingReqDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightHoldingReqDTO>(req);
        //            var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
        //            var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
        //            var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
        //            var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
        //            int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
        //            decimal AirAdditionalCharge = 0;
        //            decimal GSTAmount = 0;
        //            decimal Holding_Amount = 0;
        //            decimal UserMarkUp_Value = 0;
        //            decimal.TryParse(userMarkup, out UserMarkUp_Value);
        //            string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
        //            string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
        //            string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
        //            decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
        //            decimal.TryParse(GSTValue, out GSTAmount);
        //            decimal.TryParse(HOLDCHARGES, out Holding_Amount);
        //            string BookingDate = string.Empty;
        //            string Ref_no = string.Empty;
        //            string PNR = string.Empty;
        //            string adult = string.Empty;
        //            int AdultVal = 0;
        //            int childVal = 0;
        //            int InfantVal = 0;
        //            string child = string.Empty;
        //            string infant = string.Empty;
        //            bool IsDomestic = false;
        //            string TicketType = string.Empty;
        //            string Airlinecode = string.Empty;
        //            string FlightNo = string.Empty;
        //            string FromAirport = string.Empty;
        //            string ToAirport = string.Empty;
        //            string DeptDate = string.Empty;
        //            string Depttime = string.Empty;
        //            string arivDate = string.Empty;
        //            string arivtime = string.Empty;
        //            string TotalFlightBaseFare = string.Empty;
        //            decimal TotalBaseFare = 0;
        //            string TotalFlightTax = string.Empty;
        //            decimal TotalTaxFare = 0;
        //            string TotalFlightPassengerTax = string.Empty;
        //            decimal TotalPngsTaxFare = 0;
        //            string TotalFlightAdditionalCharges = string.Empty;
        //            decimal TotalAdditionalFare = 0;
        //            string TotalFlightCuteFee = string.Empty;
        //            decimal TotalCuteFare = 0;
        //            string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
        //            decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
        //            string TotalFlightAmount = string.Empty;
        //            decimal TotalAmt = 0;
        //            string TotalCommissionAMtCharge = string.Empty;
        //            decimal TotalCommAmt = 0;
        //            string TotalServiceCharge = string.Empty;
        //            decimal TotalServiceAmt = 0;
        //            string TDSAmount = string.Empty;
        //            decimal TotalTDSAmountAmt = 0;
        //            string ServiceTax = string.Empty;
        //            decimal TotalServiceTaxAmt = 0;
        //            string AdultCheckedIn = string.Empty;
        //            string MainClass = string.Empty;
        //            string BookingClass = string.Empty;
        //            string BookingStatus = string.Empty;
        //            decimal AdminGST = 0;
        //            AdminGST = ((Holding_Amount * GSTAmount) / 118);
        //            decimal UserMarkupGST = 0;
        //            UserMarkupGST = ((Holding_Amount * GSTAmount) / 118);

        //            TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
        //            {
        //                MEM_ID = CurrentMerchant.MEM_ID,
        //                DIST_ID = getmemberinfo.INTRODUCER,
        //                WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //                CORELATION_ID = COrelationID,
        //                PNR = PNR,
        //                REF_NO = Ref_no,
        //                TRACK_NO = "",
        //                TRIP_MODE = "",
        //                TICKET_NO = Ref_no,
        //                TICKET_TYPE = "",
        //                IS_DOMESTIC = false,
        //                AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
        //                FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
        //                FROM_AIRPORT = FlightInfo.Segment[0].FromAirportCode,
        //                TO_AIRPORT = FlightInfo.Segment[Flightcount - 1].ToAirportCode,
        //                BOOKING_DATE = DateTime.Now,
        //                DEPT_DATE = DeptDate,
        //                DEPT_TIME = Depttime,
        //                ARRIVE_DATE = arivDate,
        //                ARRIVE_TIME = arivtime,
        //                NO_OF_ADULT = AdultVal,
        //                NO_OF_CHILD = childVal,
        //                NO_OF_INFANT = InfantVal,
        //                TOTAL_FLIGHT_BASE_FARE = 0,
        //                TOTAL_FLIGHT_TAX = 0,
        //                TOTAL_PASSANGER_TAX = 0,
        //                TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //                TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
        //                TOTAL_FLIGHT_CUTE_FEE = 0,
        //                TOTAL_FLIGHT_MEAL_FEE = 0,
        //                TOTAL_AIRPORT_FEE = 0,
        //                TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //                TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
        //                TOTAL_COMMISSION_AMT = TotalCommAmt,
        //                TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //                TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //                TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //                STATUS = true,
        //                IS_CANCELLATION = false,
        //                FLIGHT_CANCELLATION_ID = "",
        //                IS_HOLD = false,
        //                API_RESPONSE = "",
        //                FLIGHT_BOOKING_DATE = BookingDate,
        //                MAIN_CLASS = FlightInfo.Segment[0].MainClass,
        //                BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
        //                BOOKING_STATUS = "PENDING",
        //                USER_MARKUP = UserMarkUp_Value,
        //                ADMIN_MARKUP = AirAdditionalCharge,
        //                COMM_SLAP = 0,
        //                ADMIN_GST = AdminGST,
        //                ADMIN_cGST = 0,
        //                ADMIN_sGST = 0,
        //                ADMIN_iGST = 0,
        //                USER_MARKUP_GST = UserMarkupGST,
        //                USER_MARKUP_cGST = 0,
        //                USER_MARKUP_sGST = 0,
        //                USER_MARKUP_iGST = 0,
        //                OP_MODE = "PENDING",
        //                HOLD_CHARGE = 0,
        //                HOLD_CGST = 0,
        //                HOLD_IGST = 0,
        //                HOLD_SGST = 0
        //            };
        //            _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
        //            _db.SaveChanges();
        //            foreach (var pnsgitem in PassngFlight.Passenger)
        //            {
        //                //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                //string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                var val_Pnsg_Details = "";
        //                string Pngdetails = "";
        //                string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
        //                string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
        //                TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
        //                {
        //                    MEM_ID = CurrentMerchant.MEM_ID,
        //                    REF_NO = Ref_no,
        //                    PNR = PNR,
        //                    TITLE = pnsgitem.Title,
        //                    FIRST_NAME = pnsgitem.FirstName,
        //                    LAST_NAME = pnsgitem.LastName,
        //                    PASSENGER_TYPE = pnsgitem.PassengerType,
        //                    GENDER = "",
        //                    BIRTH_DATE = PngDOB,
        //                    DETAILS = Pngdetails,
        //                    PASSENGER_RESP = "",
        //                    CREATE_DATE = DateTime.Now,
        //                    PNSG_SEQ_NO = PnsgSeq_No,
        //                    CORELATION_ID = COrelationID,
        //                    PASSENGER_STATUS = "PENDING",
        //                    CancelReqNo = ""
        //                };
        //                _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
        //                _db.SaveChanges();
        //            }
        //            decimal mmainBlc = 0;
        //            decimal SubMainBlc = 0;
        //            decimal Closing = 0;
        //            decimal mainClosing = 0;
        //            //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //            if (getmemberinfo.BALANCE != null)
        //            {
        //                decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
        //                SubMainBlc = mmainBlc - Holding_Amount;
        //                getmemberinfo.BALANCE = SubMainBlc;
        //            }
        //            else
        //            {
        //                SubMainBlc = mmainBlc - Holding_Amount;
        //                getmemberinfo.BALANCE = SubMainBlc;
        //            }

        //            getmemberinfo.BALANCE = SubMainBlc;
        //            _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
        //            _db.SaveChanges();
        //            var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
        //            if (MemberAcntLog != null)
        //            {
        //                Closing = MemberAcntLog.CLOSING;
        //                mainClosing = Closing - Holding_Amount;
        //            }
        //            else
        //            {
        //                Closing = MemberAcntLog.CLOSING;
        //                mainClosing = Closing - Holding_Amount;
        //            }
        //            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //            {
        //                API_ID = 0,
        //                MEM_ID = CurrentMerchant.MEM_ID,
        //                MEMBER_TYPE = "RETAILER",
        //                TRANSACTION_TYPE = "FLIGHT BOOKING",
        //                TRANSACTION_DATE = System.DateTime.Now,
        //                TRANSACTION_TIME = DateTime.Now,
        //                DR_CR = "DR",
        //                AMOUNT = Holding_Amount,
        //                NARRATION = "Debit amount for flight Hold",
        //                OPENING = Closing,
        //                CLOSING = mainClosing,
        //                REC_NO = 0,
        //                COMM_AMT = 0,
        //                GST = (float)GSTAmount,
        //                TDS = 0,
        //                IPAddress = "",
        //                TDS_PERCENTAGE = 0,
        //                //GST_PERCENTAGE = AdminGST,
        //                GST_PERCENTAGE = 0,
        //                WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
        //                //SUPER_ID = (long)SUP_MEM_ID,
        //                SUPER_ID = 0,
        //                DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
        //                SERVICE_ID = 0,
        //                CORELATIONID = COrelationID,
        //                REC_COMM_TYPE = "",
        //                COMM_VALUE = 0,
        //                NET_COMM_AMT = 0,
        //                TDS_DR_COMM_AMT = 0,
        //                CGST_COMM_AMT_INPUT = 0,
        //                CGST_COMM_AMT_OUTPUT = 0,
        //                SGST_COMM_AMT_INPUT = 0,
        //                SGST_COMM_AMT_OUTPUT = 0,
        //                IGST_COMM_AMT_INPUT = 0,
        //                IGST_COMM_AMT_OUTPUT = 0,
        //                TOTAL_GST_COMM_AMT_INPUT = 0,
        //                TOTAL_GST_COMM_AMT_OUTPUT = 0,
        //                TDS_RATE = 0,
        //                CGST_RATE = 0,
        //                SGST_RATE = 0,
        //                IGST_RATE = 0,
        //                TOTAL_GST_RATE = 0,
        //                COMM_SLAB_ID = 0,
        //                STATE_ID = getmemberinfo.STATE_ID,
        //                FLAG1 = 0,
        //                FLAG2 = 0,
        //                FLAG3 = 0,
        //                FLAG4 = 0,
        //                FLAG5 = 0,
        //                FLAG6 = 0,
        //                FLAG7 = 0,
        //                FLAG8 = 0,
        //                FLAG9 = 0,
        //                FLAG10 = 0,
        //                VENDOR_ID = 0
        //            };
        //            _db.TBL_ACCOUNTS.Add(objCommPer);
        //            _db.SaveChanges();
        //            #endregion
        //            #region Booking Api call
        //            //string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //            dynamic VerifyFlight = MultiLinkAirAPI.HoldingFlightTicket(req, COrelationID);
        //            string check = "";
        //            string ResValue = Convert.ToString(VerifyFlight);
        //            BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(ResValue);
        //            int count = BookingResponse.BookTicketResponses.BookTicketResponse.Count;
        //            var data = JsonConvert.SerializeObject(VerifyFlight);
        //            string APIRes = Convert.ToString(data);
        //            var TicketInfo = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();
        //            var Ticketcount = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.Count;
        //            var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
        //            var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
        //            var PassengerDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails.ToList();

        //            var lstpnsg = BookingResponse.BookTicketResponses.BookTicketResponse[0].PassengerDetails;
        //            var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
        //            string pnsgRes = Convert.ToString(val_Pnsg);
        //            BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
        //            BookingStatus = TicketInfo[Ticketcount - 1].Status;
        //            Ref_no = TicketInfo[Ticketcount - 1].RefNo;
        //            PNR = FareDetails[FareDetailsCount - 1].AirlinePNRNumber;
        //            TicketType = TicketInfo[Ticketcount - 1].TicketType;
        //            IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
        //            Airlinecode = FareDetails[0].AirlineCode;
        //            FlightNo = FareDetails[0].FlightNo;
        //            MainClass = FareDetails[0].MainClass;
        //            BookingClass = FareDetails[0].BookingClass;
        //            FromAirport = FareDetails[0].FromAirportCode;
        //            ToAirport = FareDetails[FareDetailsCount - 1].ToAirportCode;
        //            DeptDate = FareDetails[0].DepartureDate;
        //            DeptDate = FareDetails[0].DepartureTime;
        //            arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
        //            arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
        //            adult = TicketInfo[Ticketcount - 1].Adult;
        //            AdultVal = int.Parse(adult);
        //            child = TicketInfo[Ticketcount - 1].Child;
        //            childVal = int.Parse(child);
        //            infant = TicketInfo[Ticketcount - 1].Infant;
        //            InfantVal = int.Parse(infant);
        //            TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
        //            TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
        //            TotalFlightTax = FareDetails[0].TotalFlightTax;
        //            TotalTaxFare = decimal.Parse(TotalFlightTax);
        //            TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
        //            TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
        //            TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
        //            TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
        //            TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
        //            TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
        //            TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
        //            TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
        //            TotalFlightAmount = FareDetails[0].TotalFlightAmount;
        //            TotalAmt = decimal.Parse(TotalFlightAmount);
        //            TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
        //            TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
        //            TotalServiceCharge = FareDetails[0].TotalServiceCharge;
        //            TotalServiceAmt = decimal.Parse(TotalServiceCharge);
        //            TDSAmount = FareDetails[0].TDSAmount;
        //            TotalTDSAmountAmt = decimal.Parse(TDSAmount);
        //            ServiceTax = FareDetails[0].ServiceTax;
        //            TotalServiceTaxAmt = decimal.Parse(ServiceTax);
        //            AdultCheckedIn = FareDetails[0].AdultCheckedIn;
        //            var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.CORELATION_ID == COrelationID);
        //            if (GetFligtInfo != null)
        //            {
        //                GetFligtInfo.PNR = PNR;
        //                GetFligtInfo.REF_NO = Ref_no;
        //                GetFligtInfo.TRACK_NO = "";
        //                GetFligtInfo.TRIP_MODE = "1";
        //                GetFligtInfo.TICKET_TYPE = TicketType;
        //                GetFligtInfo.TICKET_NO = Ref_no;
        //                GetFligtInfo.IS_DOMESTIC = IsDomestic;
        //                GetFligtInfo.AIRLINE_CODE = Airlinecode;
        //                GetFligtInfo.FLIGHT_NO = FlightNo;
        //                GetFligtInfo.FROM_AIRPORT = FromAirport;
        //                GetFligtInfo.TO_AIRPORT = ToAirport;
        //                GetFligtInfo.BOOKING_DATE = DateTime.Now;
        //                GetFligtInfo.DEPT_DATE = DeptDate;
        //                GetFligtInfo.DEPT_TIME = Depttime;
        //                GetFligtInfo.ARRIVE_DATE = arivDate;
        //                GetFligtInfo.ARRIVE_TIME = arivtime;
        //                GetFligtInfo.NO_OF_ADULT = AdultVal;
        //                GetFligtInfo.NO_OF_CHILD = childVal;
        //                GetFligtInfo.NO_OF_INFANT = InfantVal;
        //                GetFligtInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
        //                GetFligtInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
        //                GetFligtInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
        //                GetFligtInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
        //                GetFligtInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
        //                GetFligtInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
        //                GetFligtInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
        //                GetFligtInfo.TOTAL_AIRPORT_FEE = 0;
        //                GetFligtInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
        //                GetFligtInfo.TOTAL_FLIGHT_AMT = TotalAmt;
        //                GetFligtInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
        //                GetFligtInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
        //                GetFligtInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
        //                GetFligtInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
        //                GetFligtInfo.STATUS = true;
        //                GetFligtInfo.IS_CANCELLATION = false;
        //                GetFligtInfo.FLIGHT_CANCELLATION_ID = "";
        //                GetFligtInfo.IS_HOLD = false;
        //                GetFligtInfo.API_RESPONSE = APIRes;
        //                GetFligtInfo.FLIGHT_BOOKING_DATE = BookingDate;
        //                GetFligtInfo.MAIN_CLASS = MainClass;
        //                GetFligtInfo.BOOKING_CLASS = BookingClass;
        //                GetFligtInfo.BOOKING_STATUS = BookingStatus;
        //                _db.Entry(GetFligtInfo).State = System.Data.Entity.EntityState.Modified;
        //                _db.SaveChanges();
        //            }
        //            //_db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.RemoveRange(_db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(u => u.CORELATION_ID == COrelationID));
        //            //_db.SaveChanges();

        //            //var DeletePnsg = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.CORELATION_ID == COrelationID).ToList();
        //            //_db.Entry(DeletePnsg).State = System.Data.Entity.EntityState.Deleted;
        //            //_db.SaveChanges();
        //            foreach (var pnsgitem in PassengerDetails)
        //            {
        //                var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //                string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //                string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //                string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //                string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
        //                string Png_LastName = Convert.ToString(pnsgitem.LastName);
        //                var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName);
        //                if (PnsgList != null)
        //                {
        //                    PnsgList.PNR = PNR;
        //                    PnsgList.REF_NO = Ref_no;
        //                    PnsgList.DETAILS = Pngdetails;
        //                    PnsgList.GENDER = pnsgitem.Gender;
        //                    PnsgList.PASSENGER_RESP = pnsgRes;
        //                    PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
        //                    PnsgList.PASSENGER_STATUS = BookingStatus;
        //                    _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
        //                    _db.SaveChanges();
        //                }
        //            }
        //            #endregion
        //            //TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
        //            //{
        //            //    MEM_ID = CurrentMerchant.MEM_ID,
        //            //    DIST_ID = getmemberinfo.INTRODUCER,
        //            //    WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //            //    CORELATION_ID = COrelationID,
        //            //    PNR = PNR,
        //            //    REF_NO = Ref_no,
        //            //    TRACK_NO = "",
        //            //    TRIP_MODE = "1",
        //            //    TICKET_NO = Ref_no,
        //            //    TICKET_TYPE = TicketType,
        //            //    IS_DOMESTIC = IsDomestic,
        //            //    AIRLINE_CODE = Airlinecode,
        //            //    FLIGHT_NO = FlightNo,
        //            //    FROM_AIRPORT = FromAirport,
        //            //    TO_AIRPORT = ToAirport,
        //            //    BOOKING_DATE = DateTime.Now,
        //            //    DEPT_DATE = DeptDate,
        //            //    DEPT_TIME = Depttime,
        //            //    ARRIVE_DATE = arivDate,
        //            //    ARRIVE_TIME = arivtime,
        //            //    NO_OF_ADULT = AdultVal,
        //            //    NO_OF_CHILD = childVal,
        //            //    NO_OF_INFANT = InfantVal,
        //            //    TOTAL_FLIGHT_BASE_FARE = TotalBaseFare,
        //            //    TOTAL_FLIGHT_TAX = TotalTaxFare,
        //            //    TOTAL_PASSANGER_TAX = TotalPngsTaxFare,
        //            //    TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //            //    TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare,
        //            //    TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare,
        //            //    TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare,
        //            //    TOTAL_AIRPORT_FEE = 0,
        //            //    TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //            //    TOTAL_FLIGHT_AMT = TotalAmt,
        //            //    TOTAL_COMMISSION_AMT = TotalCommAmt,
        //            //    TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //            //    TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //            //    TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //            //    STATUS = true,
        //            //    IS_CANCELLATION = false,
        //            //    FLIGHT_CANCELLATION_ID = "",
        //            //    IS_HOLD = true,
        //            //    BOOKING_HOLD_ID = Ref_no,
        //            //    API_RESPONSE = APIRes,
        //            //    FLIGHT_BOOKING_DATE = BookingDate,
        //            //    MAIN_CLASS = MainClass,
        //            //    BOOKING_CLASS = BookingClass,
        //            //    BOOKING_STATUS = BookingStatus,
        //            //    USER_MARKUP = UserMarkUp_Value,
        //            //    ADMIN_MARKUP = AirAdditionalCharge,
        //            //    COMM_SLAP = 0,
        //            //    ADMIN_GST = AdminGST,
        //            //    ADMIN_cGST = 0,
        //            //    ADMIN_sGST = 0,
        //            //    ADMIN_iGST = 0,
        //            //    USER_MARKUP_GST = UserMarkupGST,
        //            //    USER_MARKUP_cGST = 0,
        //            //    USER_MARKUP_sGST = 0,
        //            //    USER_MARKUP_iGST = 0,
        //            //    OP_MODE = "HOLD",
        //            //    HOLD_CHARGE = 0,
        //            //    HOLD_CGST = 0,
        //            //    HOLD_IGST = 0,
        //            //    HOLD_SGST = 0
        //            //};
        //            //_db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
        //            //_db.SaveChanges();
        //            //foreach (var pnsgitem in PassengerDetails)
        //            //{
        //            //    var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
        //            //    string Pngdetails = Convert.ToString(val_Pnsg_Details);
        //            //    string PngDOB = Convert.ToString(pnsgitem.BirthDate);
        //            //    string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
        //            //    TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
        //            //    {
        //            //        MEM_ID = CurrentMerchant.MEM_ID,
        //            //        REF_NO = Ref_no,
        //            //        PNR = PNR,
        //            //        TITLE = pnsgitem.Title,
        //            //        FIRST_NAME = pnsgitem.FirstName,
        //            //        LAST_NAME = pnsgitem.LastName,
        //            //        PASSENGER_TYPE = pnsgitem.PassengerType,
        //            //        GENDER = pnsgitem.Gender,
        //            //        BIRTH_DATE = PngDOB,
        //            //        DETAILS = Pngdetails,
        //            //        PASSENGER_RESP = pnsgRes,
        //            //        CREATE_DATE = DateTime.Now,
        //            //        PNSG_SEQ_NO = PnsgSeq_No,
        //            //        CORELATION_ID = COrelationID,
        //            //        PASSENGER_STATUS = BookingStatus,
        //            //        CancelReqNo = ""
        //            //    };
        //            //    _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
        //            //    _db.SaveChanges();
        //            //}
        //            //decimal mmainBlc = 0;
        //            //decimal SubMainBlc = 0;
        //            //decimal Closing = 0;
        //            //decimal mainClosing = 0;
        //            ////var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //            //if (getmemberinfo.BALANCE != null)
        //            //{
        //            //    decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
        //            //    SubMainBlc = mmainBlc - Holding_Amount;
        //            //    getmemberinfo.BALANCE = SubMainBlc;
        //            //}
        //            //else
        //            //{
        //            //    SubMainBlc = mmainBlc - Holding_Amount;
        //            //    getmemberinfo.BALANCE = SubMainBlc;
        //            //}

        //            //getmemberinfo.BALANCE = SubMainBlc;
        //            //_db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
        //            //_db.SaveChanges();
        //            //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
        //            //if (MemberAcntLog != null)
        //            //{
        //            //    Closing = MemberAcntLog.CLOSING;
        //            //    mainClosing = Closing - Holding_Amount;
        //            //}
        //            //else
        //            //{
        //            //    Closing = MemberAcntLog.CLOSING;
        //            //    mainClosing = Closing - Holding_Amount;
        //            //}
        //            //TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //            //{
        //            //    API_ID = 0,
        //            //    MEM_ID = CurrentMerchant.MEM_ID,
        //            //    MEMBER_TYPE = "RETAILER",
        //            //    TRANSACTION_TYPE = "FLIGHT HOLD",
        //            //    TRANSACTION_DATE = System.DateTime.Now,
        //            //    TRANSACTION_TIME = DateTime.Now,
        //            //    DR_CR = "DR",
        //            //    AMOUNT = Holding_Amount,
        //            //    NARRATION = "Debit amount for flight on hold",
        //            //    OPENING = Closing,
        //            //    CLOSING = mainClosing,
        //            //    REC_NO = 0,
        //            //    COMM_AMT = 0,
        //            //    GST = (float)GSTAmount,
        //            //    TDS = 0,
        //            //    IPAddress = "",
        //            //    TDS_PERCENTAGE = 0,
        //            //    GST_PERCENTAGE = AdminGST,
        //            //    WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
        //            //    //SUPER_ID = (long)SUP_MEM_ID,
        //            //    SUPER_ID = 0,
        //            //    DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
        //            //    SERVICE_ID = 0,
        //            //    CORELATIONID = COrelationID,
        //            //    REC_COMM_TYPE = "",
        //            //    COMM_VALUE = 0,
        //            //    NET_COMM_AMT = 0,
        //            //    TDS_DR_COMM_AMT = 0,
        //            //    CGST_COMM_AMT_INPUT = 0,
        //            //    CGST_COMM_AMT_OUTPUT = 0,
        //            //    SGST_COMM_AMT_INPUT = 0,
        //            //    SGST_COMM_AMT_OUTPUT = 0,
        //            //    IGST_COMM_AMT_INPUT = 0,
        //            //    IGST_COMM_AMT_OUTPUT = 0,
        //            //    TOTAL_GST_COMM_AMT_INPUT = 0,
        //            //    TOTAL_GST_COMM_AMT_OUTPUT = 0,
        //            //    TDS_RATE = 0,
        //            //    CGST_RATE = 0,
        //            //    SGST_RATE = 0,
        //            //    IGST_RATE = 0,
        //            //    TOTAL_GST_RATE = 0,
        //            //    COMM_SLAB_ID = 0,
        //            //    STATE_ID = getmemberinfo.STATE_ID,
        //            //    FLAG1 = 0,
        //            //    FLAG2 = 0,
        //            //    FLAG3 = 0,
        //            //    FLAG4 = 0,
        //            //    FLAG5 = 0,
        //            //    FLAG6 = 0,
        //            //    FLAG7 = 0,
        //            //    FLAG8 = 0,
        //            //    FLAG9 = 0,
        //            //    FLAG10 = 0,
        //            //    VENDOR_ID = 0
        //            //};
        //            //_db.TBL_ACCOUNTS.Add(objCommPer);
        //            //_db.SaveChanges();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }

        //}

        #endregion

        #region ReturnBookingprocess
        [HttpPost]
        public JsonResult FlightReturnBookingRequest(string Deptreq, string Retntreq, string userMarkup, string FlightAmt, string ReturnFlightAmt, string TripMode,string DEPTNetAmt,string RetnNetAmt, string ISFlightType, string INTPancard, string deptSegment = "", string returnSegment = "")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            decimal OnewayAmount = 0;
            decimal.TryParse(FlightAmt, out OnewayAmount);
            decimal ReturnAmount = 0;
            decimal.TryParse(ReturnFlightAmt, out ReturnAmount);
            TotalBookAmt = OnewayAmount + ReturnAmount;
            int deptcnt = 0;
            int retncnt = 0;
            string ReturnDeptResponse = "";
            string ReturnResponse = "";
            string Returnway = "";
            decimal BlockBal = 0;
            decimal AddMainBal = 0;
            decimal MainBALL = 0;
            //decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            decimal.TryParse(getmemberinfo.BLOCKED_BALANCE.ToString(), out BlockBal);
            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out MainBALL);
            AddMainBal = MainBALL - BlockBal;
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            if (AddMainBal > TotalBookAmt)
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            {
                string ReturnDepature = ReturnOnewayBooking(Deptreq, userMarkup, FlightAmt, ReturnFlightAmt, TripMode, DEPTNetAmt, deptSegment, returnSegment,"O", ISFlightType, INTPancard);
                
                //string Returnway = ReturnOnewayBooking(Retntreq, userMarkup, ReturnFlightAmt, ReturnFlightAmt, TripMode, RetnNetAmt, deptSegment, returnSegment,"R", ISFlightType, INTPancard);
                if (ReturnDepature == "Return Booking is Success")
                {
                    Returnway = ReturnOnewayBooking(Retntreq, userMarkup, ReturnFlightAmt, ReturnFlightAmt, TripMode, RetnNetAmt, deptSegment, returnSegment, "R", ISFlightType, INTPancard);

                    ReturnDeptResponse = "Round Trip Depature Booking is done";
                    TempData["IsShowPrintTicket"] = "Show";
                }
                else
                {
                    ReturnDeptResponse = "Round Trip Depature Booking is not done";
                }
                if (Returnway == "Return Booking is Success")
                {
                    ReturnResponse = "Round Trip Return Booking is done";
                    TempData["IsShowPrintTicket"] = "Show";
                }
                else
                { ReturnResponse = "Round Trip Return Booking is not done"; }
                return Json(new { result = ReturnDeptResponse,ReturnRes= ReturnResponse });
            }
            else
            {
                return Json(new { result = "", ReturnRes = "Your wallet balance is insufficient to book a ticket." }, JsonRequestBehavior.AllowGet);
                //return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult FlightReturnHoldBookingRequest(string Deptreq, string Retntreq, string userMarkup, string FlightAmt, string ReturnFlightAmt, string TripMode, string DEPTNetAmt, string RetnNetAmt, string ISFlightType, string INTPancard, string deptSegment = "", string returnSegment = "")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            decimal OnewayAmount = 0;
            decimal.TryParse(FlightAmt,out OnewayAmount);
            decimal ReturnAmount = 0;
            decimal.TryParse(ReturnFlightAmt, out ReturnAmount);
            TotalBookAmt = OnewayAmount + ReturnAmount;
            int deptcnt = 0;
            int retncnt = 0;
            string ReturnDeptResponse = "";
            string ReturnResponse = "";
            decimal BlockBal = 0;
            decimal AddMainBal = 0;
            decimal MainBALL = 0;
            //decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            decimal.TryParse(getmemberinfo.BLOCKED_BALANCE.ToString(), out BlockBal);
            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out MainBALL);
            AddMainBal = MainBALL - BlockBal;
            //if (getmemberinfo.BALANCE > TotalBookAmt)
            if (AddMainBal > TotalBookAmt)
            {
                string ReturnDepature = ReturnHoldBooking(Deptreq, userMarkup, FlightAmt, ReturnFlightAmt, TripMode, DEPTNetAmt, deptSegment, returnSegment,"O", ISFlightType, INTPancard);
                string Returnway = ReturnHoldBooking(Retntreq, userMarkup, ReturnFlightAmt, ReturnFlightAmt, TripMode, RetnNetAmt, deptSegment, returnSegment,"R", ISFlightType, INTPancard);
                //string Returnway = ReturnHoldBooking(Retntreq, userMarkup, FlightAmt, ReturnFlightAmt, TripMode, RetnNetAmt, deptSegment, returnSegment);
                if (ReturnDepature == "Return Booking is Success")
                { ReturnDeptResponse = "Round Trip Depature Booking is done"; }
                else
                { ReturnDeptResponse = "Round Trip Depature Booking is not done"; }
                if (Returnway == "Return Booking is Success")
                { ReturnResponse = "Round Trip Return Booking is done"; }
                else
                { ReturnResponse = "Round Trip Return Booking is not done"; }
                return Json(new { result = ReturnDeptResponse, ReturnRes = ReturnResponse });
            }
            else
            {
                return Json(new { result = "", ReturnRes = "Your wallet balance is insufficient to book a ticket." }, JsonRequestBehavior.AllowGet);
                //return Json("Your wallet balance is insufficient to book a ticket.", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Return Depeture from
        public string ReturnOnewayBooking(string req,  string userMarkup, string FlightAmt, string ReturnFlightAmt, string TripMode, string NetAmount,string deptSegment = "", string returnSegment = "",string TType="", string ISFlightType="", string INTPancard="")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            int deptcnt = 0;
            int retncnt = 0;
            decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
            List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
            List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
            if (deptureSegment != null)
            { deptcnt = deptureSegment.Count(); }
            if (retSegment != null)
            {
                retncnt = retSegment.Count();
            }
            FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
            var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
            Deptstopage = FlightInfo.Segment.Count();
            var sengList = FlightInfo.Segment.ToList();
            string Dept_FlightNo = string.Empty;
            string Dept_airlineCode = string.Empty;
            string retn_FlightNo = string.Empty;
            string retn_airlineCode = string.Empty;
            string GSTCompanyName = string.Empty;
            string GSTCompanyEmail = string.Empty;
            string GSTNO = string.Empty;
            string GSTMOBILE_NO = string.Empty;
            string GSTADDRESS = string.Empty;
            GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
            GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
            GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
            GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
            GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
            int cntDept = 0;
            int cntretn = 0;
            string SeqNoDept = "";
            string SeqNoRtn = "";
            DateTime R_DeptDate = new DateTime();
            DateTime R_RetnDate = new DateTime();
            
            if (TripMode == "R")
            {
                var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
                var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
                Dept_FlightNo = SegInfodept[0].FlightNo;
                Dept_airlineCode = SegInfodept[0].AirlineCode;
                //retn_FlightNo = SegInforeturn[0].FlightNo;
                //retn_airlineCode = SegInforeturn[0].AirlineCode;
                cntDept = SegInfodept.Count();
                //cntretn = SegInforeturn.Count();
                SeqNoDept = SegInfodept[0].SegmentSeqNo;
                //SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
                //R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
                R_DeptDate = DateTime.ParseExact(SegInfodept[0].DepDate, "dd/MM/yyyy", null); 
                //R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
            }
            var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
            var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
            var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
            int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
            #region variable Declieare     
            decimal AirAdditionalCharge = 0;
            decimal GSTAmount = 0;
            decimal Holding_Amount = 0;
            decimal UserMarkUp_Value = 0;
            decimal.TryParse(userMarkup, out UserMarkUp_Value);
            //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
            string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
            string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
            string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
            decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
            decimal.TryParse(GSTValue, out GSTAmount);
            decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
            decimal AdminGST = 0;
            decimal Publish_Fare = 0;
            decimal NET_FARE = 0;
            decimal NET_TOTAL_FARE = 0;
            decimal RESCHEDULE_FARE = 0;
            string ResValue = string.Empty;
            dynamic VerifyFlight = null;
            decimal TotalNetAmount = 0;
            decimal FlightNetAmount = 0;
            decimal AgentCommAmt = 0;
            decimal AgentCommTDS = 0;
            decimal TCSAmt = 0;
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
                    decimal UserMarkupGST = 0;
                    UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);

                    decimal.TryParse(NetAmount, out FlightNetAmount);
                    AgentCommAmt = FlightNetAmount - TotalBookAmt;
                    AgentCommTDS = ((AgentCommAmt * 5) / 100);
                    TotalNetAmount = FlightNetAmount + AgentCommTDS;
                    if (ISFlightType == "International")
                    {
                        if (INTPancard != "")
                        {
                            TCSAmt = ((TotalBookAmt * 5) / 100);
                        }
                        else
                        {
                            TCSAmt = ((TotalBookAmt * 10) / 100);
                        }
                    }
                    else
                    {
                        TCSAmt = 0;
                    }


                    TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        DIST_ID = getmemberinfo.INTRODUCER,
                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                        CORELATION_ID = COrelationID,
                        PNR = PNR,
                        REF_NO = Ref_no,
                        TRACK_NO = "",
                        TRIP_MODE = TripMode,
                        TICKET_NO = Ref_no,
                        TICKET_TYPE = TType,
                        IS_DOMESTIC = false,
                        //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                        //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                        AIRLINE_CODE = Dept_airlineCode,
                        FLIGHT_NO = Dept_FlightNo,
                        //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
                        //FROM_AIRPORT = RetdeptFromAir,
                        //TO_AIRPORT = RetdeptToAir,
                        FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                        TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
                        //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
                        BOOKING_DATE = DateTime.Now,
                        DEPT_DATE = DeptDate,
                        DEPT_TIME = Depttime,
                        ARRIVE_DATE = arivDate,
                        ARRIVE_TIME = arivtime,
                        NO_OF_ADULT = AdultVal,
                        NO_OF_CHILD = childVal,
                        NO_OF_INFANT = InfantVal,
                        TOTAL_FLIGHT_BASE_FARE = 0,
                        TOTAL_FLIGHT_TAX = 0,
                        TOTAL_PASSANGER_TAX = 0,
                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                        TOTAL_FLIGHT_CUTE_FEE = 0,
                        TOTAL_FLIGHT_MEAL_FEE = 0,
                        TOTAL_AIRPORT_FEE = 0,
                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                        TOTAL_COMMISSION_AMT = TotalCommAmt,
                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                        STATUS = true,
                        IS_CANCELLATION = false,
                        FLIGHT_CANCELLATION_ID = "",
                        IS_HOLD = false,
                        API_RESPONSE = "",
                        FLIGHT_BOOKING_DATE = BookingDate,
                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                        BOOKING_STATUS = "PENDING",
                        USER_MARKUP = UserMarkUp_Value,
                        ADMIN_MARKUP = AirAdditionalCharge,
                        COMM_SLAP = 0,
                        ADMIN_GST = AdminGST,
                        ADMIN_cGST = 0,
                        ADMIN_sGST = 0,
                        ADMIN_iGST = 0,
                        USER_MARKUP_GST = UserMarkupGST,
                        USER_MARKUP_cGST = 0,
                        USER_MARKUP_sGST = 0,
                        USER_MARKUP_iGST = 0,
                        OP_MODE = "BOOKED",
                        HOLD_CHARGE = 0,
                        HOLD_CGST = 0,
                        HOLD_IGST = 0,
                        HOLD_SGST = 0,
                        PASSAGER_SEGMENT = SeqNoDept,
                        API_REQUEST = req,
                        STOPAGE = (cntDept - 1),
                        COMPANY_NAME = GSTCompanyName,
                        COMPANY_EMAIL_ID = GSTCompanyEmail,
                        COMPANY_GST_NO = GSTNO,
                        COMPANY_MOBILE = GSTMOBILE_NO,
                        COMPANY_GST_ADDRESS = GSTADDRESS,
                        NET_COMM_FARE = TotalNetAmount,
                        FARE_COMMISSION = AgentCommAmt,
                        FARE_COMMISSION_TDS = AgentCommTDS,
                        TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                        INT_FLIGHT_PANCARD = INTPancard
                    };
                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
                    //_db.SaveChanges();
                    foreach (var pnsgitem in PassngFlight.Passenger)
                    {
                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        var val_Pnsg_Details = "";
                        string Pngdetails = "";
                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            REF_NO = Ref_no,
                            PNR = PNR,
                            TITLE = pnsgitem.Title,
                            FIRST_NAME = pnsgitem.FirstName,
                            LAST_NAME = pnsgitem.LastName,
                            PASSENGER_TYPE = pnsgitem.PassengerType,
                            GENDER = "",
                            BIRTH_DATE = PngDOB,
                            DETAILS = Pngdetails,
                            PASSENGER_RESP = "",
                            CREATE_DATE = DateTime.Now,
                            PNSG_SEQ_NO = PnsgSeq_No,
                            CORELATION_ID = COrelationID,
                            PASSENGER_STATUS = "PENDING",
                            CancelReqNo = "",
                            TRIP_TYPE = "O",
                            DOJ = R_DeptDate,
                            FLIGHT_SEGMENT = SeqNoDept,
                            FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                            TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
                        };
                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                        //_db.SaveChanges();
                    }
                    decimal mmainBlc = 0;
                    decimal SubMainBlc = 0;
                    decimal Closing = 0;
                    decimal mainClosing = 0;
                    decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
                    //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (getmemberinfo.BALANCE != null)
                    {
                        decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                        //SubMainBlc = mmainBlc - TotalAmt;  //TotalBookAmt
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    else
                    {
                        //SubMainBlc = mmainBlc - TotalAmt;
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    getmemberinfo.BALANCE = SubMainBlc;
                    _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                    //_db.SaveChanges();
                    //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                    if (MemberAcntLog != null)
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    else
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = CurrentMerchant.MEM_ID,
                        MEMBER_TYPE = "RETAILER",
                        TRANSACTION_TYPE = "FLIGHT BOOKING",
                        TRANSACTION_DATE = System.DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "DR",
                        //AMOUNT = TotalAmt,
                        AMOUNT = T_Amount,
                        NARRATION = "Debit amount for flight booking",
                        OPENING = Closing,
                        CLOSING = mainClosing,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (float)GSTAmount,
                        TDS = 0,
                        IPAddress = "",
                        TDS_PERCENTAGE = 0,
                        //GST_PERCENTAGE = AdminGST,
                        GST_PERCENTAGE = 0,
                        WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                        //SUPER_ID = (long)SUP_MEM_ID,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID,
                        REC_COMM_TYPE = "",
                        COMM_VALUE = 0,
                        NET_COMM_AMT = 0,
                        TDS_DR_COMM_AMT = 0,
                        CGST_COMM_AMT_INPUT = 0,
                        CGST_COMM_AMT_OUTPUT = 0,
                        SGST_COMM_AMT_INPUT = 0,
                        SGST_COMM_AMT_OUTPUT = 0,
                        IGST_COMM_AMT_INPUT = 0,
                        IGST_COMM_AMT_OUTPUT = 0,
                        TOTAL_GST_COMM_AMT_INPUT = 0,
                        TOTAL_GST_COMM_AMT_OUTPUT = 0,
                        TDS_RATE = 0,
                        CGST_RATE = 0,
                        SGST_RATE = 0,
                        IGST_RATE = 0,
                        TOTAL_GST_RATE = 0,
                        COMM_SLAB_ID = 0,
                        STATE_ID = getmemberinfo.STATE_ID,
                        FLAG1 = 0,
                        FLAG2 = 0,
                        FLAG3 = 0,
                        FLAG4 = 0,
                        FLAG5 = 0,
                        FLAG6 = 0,
                        FLAG7 = 0,
                        FLAG8 = 0,
                        FLAG9 = 0,
                        FLAG10 = 0,
                        VENDOR_ID = 0
                    };
                    _db.TBL_ACCOUNTS.Add(objCommPer);
                    _db.SaveChanges();
                    #endregion
                    #region Booking Api call
                    //dynamic VerifyFlight = null;
                    VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
                    ResValue = Convert.ToString(VerifyFlight);
                    //var APIStatus = VerifyFlight.BookTicketResponse.Error;
                    var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
                    if (getStatus == "Acknowledged" || getStatus == "Completed")
                    {
                        ContextTransaction.Commit();
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        //return Json("Please try again later", JsonRequestBehavior.AllowGet);
                        return "Please try again later";
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return "false";
                    //return Json(false, JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
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
                    string DATEofJourney = DeptDate;
                    DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
                    //DateTime dateofJourey =Convert.ToDateTime(DATEofJourney);
                    Depttime = FareDetails[0].DepartureTime;
                    arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                    arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                    adult = TicketInfo[Ticketcount - 1].Adult;
                    AdultVal = int.Parse(adult);
                    child = TicketInfo[Ticketcount - 1].Child;
                    childVal = int.Parse(child);
                    infant = TicketInfo[Ticketcount - 1].Infant;
                    InfantVal = int.Parse(infant);
                    //string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                    string AIRADDITIONALAMOUNT_Value = Session["AIRADDITIONALAMOUNT"].ToString();
                    decimal Additional_AMt = 0;
                    decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
                    TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                    Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;
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
                    NET_FARE = TotalAmt;
                    NET_TOTAL_FARE = NET_FARE + Additional_AMt;
                    TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                    TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                    TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                    TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                    TDSAmount = FareDetails[0].TDSAmount;
                    TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                    ServiceTax = FareDetails[0].ServiceTax;
                    TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                    AdultCheckedIn = FareDetails[0].AdultCheckedIn;
                    string jhjdfd = "";
                    var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
                    if (GetFligtInfo != null)
                    {
                        foreach (var ticketInfo in GetFligtInfo)
                        {
                            var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
                            flightTicketInfo.PNR = PNR;
                            flightTicketInfo.REF_NO = Ref_no;
                            flightTicketInfo.TRACK_NO = "";
                            flightTicketInfo.TRIP_MODE = "1";
                            //flightTicketInfo.TICKET_TYPE = TicketType;
                            flightTicketInfo.TICKET_NO = Ref_no;
                            flightTicketInfo.IS_DOMESTIC = IsDomestic;
                            flightTicketInfo.AIRLINE_CODE = Airlinecode;
                            flightTicketInfo.FLIGHT_NO = FlightNo;
                            flightTicketInfo.FROM_AIRPORT = FromAirport;
                            flightTicketInfo.TO_AIRPORT = ToAirport;
                            flightTicketInfo.BOOKING_DATE = DateTime.Now;
                            flightTicketInfo.DEPT_DATE = DeptDate;
                            flightTicketInfo.DEPT_TIME = Depttime;
                            flightTicketInfo.ARRIVE_DATE = arivDate;
                            flightTicketInfo.ARRIVE_TIME = arivtime;
                            flightTicketInfo.NO_OF_ADULT = AdultVal;
                            flightTicketInfo.NO_OF_CHILD = childVal;
                            flightTicketInfo.NO_OF_INFANT = InfantVal;
                            flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
                            flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
                            flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
                            flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
                            flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
                            flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
                            flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
                            flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
                            flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
                            flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
                            flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
                            flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
                            flightTicketInfo.STATUS = true;
                            flightTicketInfo.IS_CANCELLATION = false;
                            flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
                            flightTicketInfo.IS_HOLD = false;
                            flightTicketInfo.API_RESPONSE = APIRes;
                            flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
                            flightTicketInfo.BOOKING_STATUS = BookingStatus;
                            flightTicketInfo.MAIN_CLASS = MainClass;
                            flightTicketInfo.BOOKING_CLASS = BookingClass;
                            flightTicketInfo.PUBLISH_FARE = Publish_Fare;
                            flightTicketInfo.NET_FARE = NET_FARE;
                            flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
                            flightTicketInfo.CANCELLATION_REMARK = "";
                            flightTicketInfo.RESCHEDULE_FARE = false;
                            flightTicketInfo.RESCHEDULE_REMARK = "";
                            _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                            //ContextTransaction.Commit();
                        }
                    }
                    foreach (var pnsgitem in PassengerDetails)
                    {
                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
                        if (PnsgList != null)
                        {
                            PnsgList.PNR = PNR;
                            PnsgList.REF_NO = Ref_no;
                            PnsgList.DETAILS = Pngdetails;
                            PnsgList.GENDER = pnsgitem.Gender;
                            PnsgList.PASSENGER_RESP = pnsgRes;
                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                            PnsgList.PASSENGER_STATUS = BookingStatus;
                            PnsgList.CANCELLATION_REMARTK = "";
                            PnsgList.PASSPORT = "";
                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                        }
                    }
                    _db.SaveChanges();
                    ContextTransaction.Commit();
                    #endregion
                    TempData["IsShowRef_NoTicket"] = Ref_no;
                    TempData["IsShowPNRTicket"] = PNR;
                    return "Return Booking is Success";
                    //TempData["IsShowPrintTicket"] = "Show";
                    //return Json(data, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    //string check = RefundAmount(COrelationID, TripMode);
                    return "Return Dept Fail";
                    throw;
                }
            }
        }
        public string ReturnHoldBooking(string req, string userMarkup, string FlightAmt, string ReturnFlightAmt, string TripMode, string NetAmount, string deptSegment = "", string returnSegment = "", string TType = "", string ISFlightType="", string INTPancard="")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            int deptcnt = 0;
            int retncnt = 0;
            string HLD_VAlue = "";
            decimal Hold_AMountValue = 0;
            decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
            List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
            List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
            if (deptureSegment != null)
            { deptcnt = deptureSegment.Count(); }
            if (retSegment != null)
            {
                retncnt = retSegment.Count();
            }
            //FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
            FlightHoldingReqDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightHoldingReqDTO>(req);            
            var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
            Deptstopage = FlightInfo.Segment.Count();
            var sengList = FlightInfo.Segment.ToList();

            HLD_VAlue = beforeApiExecute.RequestXml.BookTicketRequest.HoldCharge;
            decimal.TryParse(HLD_VAlue, out Hold_AMountValue);

            string Dept_FlightNo = string.Empty;
            string Dept_airlineCode = string.Empty;
            string retn_FlightNo = string.Empty;
            string retn_airlineCode = string.Empty;
            string GSTCompanyName = string.Empty;
            string GSTCompanyEmail = string.Empty;
            string GSTNO = string.Empty;
            string GSTMOBILE_NO = string.Empty;
            string GSTADDRESS = string.Empty;
            //GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
            //GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
            //GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
            //GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
            //GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
            int cntDept = 0;
            int cntretn = 0;
            string SeqNoDept = "";
            string SeqNoRtn = "";
            DateTime R_DeptDate = new DateTime();
            DateTime R_RetnDate = new DateTime();

            if (TripMode == "R")
            {
                var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
                var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
                Dept_FlightNo = SegInfodept[0].FlightNo;
                Dept_airlineCode = SegInfodept[0].AirlineCode;
                //retn_FlightNo = SegInforeturn[0].FlightNo;
                //retn_airlineCode = SegInforeturn[0].AirlineCode;
                cntDept = SegInfodept.Count();
                //cntretn = SegInforeturn.Count();
                SeqNoDept = SegInfodept[0].SegmentSeqNo;
                //SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
                //R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
                R_DeptDate = DateTime.ParseExact(SegInfodept[0].DepDate, "dd/MM/yyyy", null);
                //R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
            }
            var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
            var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
            var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
            int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
            #region variable Declieare     
            decimal AirAdditionalCharge = 0;
            decimal GSTAmount = 0;
            decimal Holding_Amount = 0;
            decimal UserMarkUp_Value = 0;
            decimal.TryParse(userMarkup, out UserMarkUp_Value);
            //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
            string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
            string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
            string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
            decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
            decimal.TryParse(GSTValue, out GSTAmount);
            decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
            decimal AdminGST = 0;
            decimal Publish_Fare = 0;
            decimal NET_FARE = 0;
            decimal NET_TOTAL_FARE = 0;
            decimal RESCHEDULE_FARE = 0;
            string ResValue = string.Empty;
            dynamic VerifyFlight = null;
            decimal TotalNetAmount = 0;
            decimal FlightNetAmount = 0;
            decimal AgentCommAmt = 0;
            decimal AgentCommTDS = 0;
            decimal TCSAmt = 0;
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
                    decimal UserMarkupGST = 0;
                    UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);
                    decimal.TryParse(NetAmount, out FlightNetAmount);
                    AgentCommAmt = FlightNetAmount - TotalBookAmt;
                    AgentCommTDS = ((AgentCommAmt * 5) / 100);
                    TotalNetAmount = FlightNetAmount + AgentCommTDS;
                    if (ISFlightType == "International")
                    {
                        if (INTPancard != "")
                        {
                            TCSAmt = ((TotalBookAmt * 5) / 100);
                        }
                        else
                        {
                            TCSAmt = ((TotalBookAmt * 10) / 100);
                        }
                    }
                    else
                    {
                        TCSAmt = 0;
                    }

                    TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        DIST_ID = getmemberinfo.INTRODUCER,
                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                        CORELATION_ID = COrelationID,
                        PNR = PNR,
                        REF_NO = Ref_no,
                        TRACK_NO = "",
                        TRIP_MODE = TripMode,
                        TICKET_NO = Ref_no,
                        TICKET_TYPE = TType,
                        IS_DOMESTIC = false,
                        //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                        //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                        AIRLINE_CODE = Dept_airlineCode,
                        FLIGHT_NO = Dept_FlightNo,
                        //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
                        //FROM_AIRPORT = RetdeptFromAir,
                        //TO_AIRPORT = RetdeptToAir,
                        FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                        TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
                        //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
                        BOOKING_DATE = DateTime.Now,
                        DEPT_DATE = DeptDate,
                        DEPT_TIME = Depttime,
                        ARRIVE_DATE = arivDate,
                        ARRIVE_TIME = arivtime,
                        NO_OF_ADULT = AdultVal,
                        NO_OF_CHILD = childVal,
                        NO_OF_INFANT = InfantVal,
                        TOTAL_FLIGHT_BASE_FARE = 0,
                        TOTAL_FLIGHT_TAX = 0,
                        TOTAL_PASSANGER_TAX = 0,
                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                        TOTAL_FLIGHT_CUTE_FEE = 0,
                        TOTAL_FLIGHT_MEAL_FEE = 0,
                        TOTAL_AIRPORT_FEE = 0,
                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                        TOTAL_COMMISSION_AMT = TotalCommAmt,
                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                        STATUS = true,
                        IS_CANCELLATION = false,
                        FLIGHT_CANCELLATION_ID = "",
                        IS_HOLD = true,
                        BOOKING_HOLD_ID = Ref_no,
                        HOLD_DATE = DateTime.Now,
                        API_RESPONSE = "",
                        FLIGHT_BOOKING_DATE = BookingDate,
                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                        BOOKING_STATUS = "PENDING",
                        USER_MARKUP = UserMarkUp_Value,
                        ADMIN_MARKUP = AirAdditionalCharge,
                        COMM_SLAP = 0,
                        ADMIN_GST = AdminGST,
                        ADMIN_cGST = 0,
                        ADMIN_sGST = 0,
                        ADMIN_iGST = 0,
                        USER_MARKUP_GST = UserMarkupGST,
                        USER_MARKUP_cGST = 0,
                        USER_MARKUP_sGST = 0,
                        USER_MARKUP_iGST = 0,
                        OP_MODE = "HOLD",
                        HOLD_CHARGE = Hold_AMountValue,
                        HOLD_CGST = 0,
                        HOLD_IGST = 0,
                        HOLD_SGST = 0,
                        PASSAGER_SEGMENT = SeqNoDept,
                        API_REQUEST = req,
                        STOPAGE = (cntDept - 1),
                        COMPANY_NAME = GSTCompanyName,
                        COMPANY_EMAIL_ID = GSTCompanyEmail,
                        COMPANY_GST_NO = GSTNO,
                        COMPANY_MOBILE = GSTMOBILE_NO,
                        COMPANY_GST_ADDRESS = GSTADDRESS,
                        NET_COMM_FARE = TotalNetAmount,
                        FARE_COMMISSION = AgentCommAmt,
                        FARE_COMMISSION_TDS = AgentCommTDS,
                        TCS_AMOUNTON_INT_FLIGHT = TCSAmt,
                        INT_FLIGHT_PANCARD = INTPancard
                    };
                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
                    //_db.SaveChanges();
                    foreach (var pnsgitem in PassngFlight.Passenger)
                    {
                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        var val_Pnsg_Details = "";
                        string Pngdetails = "";
                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            REF_NO = Ref_no,
                            PNR = PNR,
                            TITLE = pnsgitem.Title,
                            FIRST_NAME = pnsgitem.FirstName,
                            LAST_NAME = pnsgitem.LastName,
                            PASSENGER_TYPE = pnsgitem.PassengerType,
                            GENDER = "",
                            BIRTH_DATE = PngDOB,
                            DETAILS = Pngdetails,
                            PASSENGER_RESP = "",
                            CREATE_DATE = DateTime.Now,
                            PNSG_SEQ_NO = PnsgSeq_No,
                            CORELATION_ID = COrelationID,
                            PASSENGER_STATUS = "PENDING",
                            CancelReqNo = "",
                            TRIP_TYPE = "O",
                            DOJ = R_DeptDate,
                            FLIGHT_SEGMENT = SeqNoDept,
                            FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                            TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
                        };
                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                        //_db.SaveChanges();
                    }
                    decimal mmainBlc = 0;
                    decimal SubMainBlc = 0;
                    decimal Closing = 0;
                    decimal mainClosing = 0;
                    decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
                    //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (getmemberinfo.BALANCE != null)
                    {
                        decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                        //SubMainBlc = mmainBlc - TotalAmt;  //TotalBookAmt
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    else
                    {
                        //SubMainBlc = mmainBlc - TotalAmt;
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    getmemberinfo.BALANCE = SubMainBlc;
                    _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                    //_db.SaveChanges();
                    //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                    if (MemberAcntLog != null)
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    else
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = CurrentMerchant.MEM_ID,
                        MEMBER_TYPE = "RETAILER",
                        TRANSACTION_TYPE = "FLIGHT BOOKING",
                        TRANSACTION_DATE = System.DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "DR",
                        //AMOUNT = TotalAmt,
                        AMOUNT = T_Amount,
                        NARRATION = "Debit amount for flight booking",
                        OPENING = Closing,
                        CLOSING = mainClosing,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (float)GSTAmount,
                        TDS = 0,
                        IPAddress = "",
                        TDS_PERCENTAGE = 0,
                        //GST_PERCENTAGE = AdminGST,
                        GST_PERCENTAGE = 0,
                        WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                        //SUPER_ID = (long)SUP_MEM_ID,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID,
                        REC_COMM_TYPE = "",
                        COMM_VALUE = 0,
                        NET_COMM_AMT = 0,
                        TDS_DR_COMM_AMT = 0,
                        CGST_COMM_AMT_INPUT = 0,
                        CGST_COMM_AMT_OUTPUT = 0,
                        SGST_COMM_AMT_INPUT = 0,
                        SGST_COMM_AMT_OUTPUT = 0,
                        IGST_COMM_AMT_INPUT = 0,
                        IGST_COMM_AMT_OUTPUT = 0,
                        TOTAL_GST_COMM_AMT_INPUT = 0,
                        TOTAL_GST_COMM_AMT_OUTPUT = 0,
                        TDS_RATE = 0,
                        CGST_RATE = 0,
                        SGST_RATE = 0,
                        IGST_RATE = 0,
                        TOTAL_GST_RATE = 0,
                        COMM_SLAB_ID = 0,
                        STATE_ID = getmemberinfo.STATE_ID,
                        FLAG1 = 0,
                        FLAG2 = 0,
                        FLAG3 = 0,
                        FLAG4 = 0,
                        FLAG5 = 0,
                        FLAG6 = 0,
                        FLAG7 = 0,
                        FLAG8 = 0,
                        FLAG9 = 0,
                        FLAG10 = 0,
                        VENDOR_ID = 0
                    };
                    _db.TBL_ACCOUNTS.Add(objCommPer);
                    _db.SaveChanges();
                    #endregion
                    #region Booking Api call
                    //dynamic VerifyFlight = null;
                    VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
                    ResValue = Convert.ToString(VerifyFlight);
                    //var APIStatus = VerifyFlight.BookTicketResponse.Error;
                    var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
                    //if (getStatus == "On Hold" || getStatus == "Completed")
                    if (getStatus == "Acknowledged")
                    {
                        ContextTransaction.Commit();
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        //return Json("Please try again later", JsonRequestBehavior.AllowGet);
                        return "Please try again later";
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return "false";
                    //return Json(false, JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
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
                    string DATEofJourney = DeptDate;
                    DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
                    //DateTime dateofJourey =Convert.ToDateTime(DATEofJourney);
                    Depttime = FareDetails[0].DepartureTime;
                    arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                    arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                    adult = TicketInfo[Ticketcount - 1].Adult;
                    AdultVal = int.Parse(adult);
                    child = TicketInfo[Ticketcount - 1].Child;
                    childVal = int.Parse(child);
                    infant = TicketInfo[Ticketcount - 1].Infant;
                    InfantVal = int.Parse(infant);
                    //string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                    string AIRADDITIONALAMOUNT_Value = Session["AIRADDITIONALAMOUNT"].ToString();
                    decimal Additional_AMt = 0;
                    decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
                    TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                    Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;
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
                    NET_FARE = TotalAmt;
                    NET_TOTAL_FARE = NET_FARE + Additional_AMt;
                    TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                    TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                    TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                    TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                    TDSAmount = FareDetails[0].TDSAmount;
                    TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                    ServiceTax = FareDetails[0].ServiceTax;
                    TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                    AdultCheckedIn = FareDetails[0].AdultCheckedIn;
                    string jhjdfd = "";
                    var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
                    if (GetFligtInfo != null)
                    {
                        foreach (var ticketInfo in GetFligtInfo)
                        {
                            var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
                            flightTicketInfo.PNR = PNR;
                            flightTicketInfo.REF_NO = Ref_no;
                            flightTicketInfo.TRACK_NO = "";
                            flightTicketInfo.TRIP_MODE = "1";
                           // flightTicketInfo.TICKET_TYPE = TicketType;
                            flightTicketInfo.TICKET_NO = Ref_no;
                            flightTicketInfo.IS_DOMESTIC = IsDomestic;
                            flightTicketInfo.AIRLINE_CODE = Airlinecode;
                            flightTicketInfo.FLIGHT_NO = FlightNo;
                            flightTicketInfo.FROM_AIRPORT = FromAirport;
                            flightTicketInfo.TO_AIRPORT = ToAirport;
                            flightTicketInfo.BOOKING_DATE = DateTime.Now;
                            flightTicketInfo.DEPT_DATE = DeptDate;
                            flightTicketInfo.DEPT_TIME = Depttime;
                            flightTicketInfo.ARRIVE_DATE = arivDate;
                            flightTicketInfo.ARRIVE_TIME = arivtime;
                            flightTicketInfo.NO_OF_ADULT = AdultVal;
                            flightTicketInfo.NO_OF_CHILD = childVal;
                            flightTicketInfo.NO_OF_INFANT = InfantVal;
                            flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
                            flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
                            flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
                            flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
                            flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
                            flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
                            flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
                            flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
                            flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
                            flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
                            flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
                            flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
                            flightTicketInfo.STATUS = true;
                            flightTicketInfo.IS_CANCELLATION = false;
                            flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
                            flightTicketInfo.IS_HOLD = false;
                            flightTicketInfo.API_RESPONSE = APIRes;
                            flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
                            flightTicketInfo.BOOKING_STATUS = BookingStatus;
                            flightTicketInfo.MAIN_CLASS = MainClass;
                            flightTicketInfo.BOOKING_CLASS = BookingClass;
                            flightTicketInfo.PUBLISH_FARE = Publish_Fare;
                            flightTicketInfo.NET_FARE = NET_FARE;
                            flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
                            flightTicketInfo.CANCELLATION_REMARK = "";
                            flightTicketInfo.RESCHEDULE_FARE = false;
                            flightTicketInfo.RESCHEDULE_REMARK = "";
                            _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                            //ContextTransaction.Commit();
                        }
                    }
                    foreach (var pnsgitem in PassengerDetails)
                    {
                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
                        if (PnsgList != null)
                        {
                            PnsgList.PNR = PNR;
                            PnsgList.REF_NO = Ref_no;
                            PnsgList.DETAILS = Pngdetails;
                            PnsgList.GENDER = pnsgitem.Gender;
                            PnsgList.PASSENGER_RESP = pnsgRes;
                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                            PnsgList.PASSENGER_STATUS = BookingStatus;
                            PnsgList.CANCELLATION_REMARTK = "";
                            PnsgList.PASSPORT = "";
                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                        }
                    }
                    _db.SaveChanges();
                    ContextTransaction.Commit();
                    #endregion
                    return "Return Booking is Success";
                    //TempData["IsShowPrintTicket"] = "Show";
                    //return Json(data, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    //string check = RefundAmount(COrelationID, TripMode);
                    return "Return Dept Fail";
                    throw;
                }
            }
        }
        #endregion

        #region Return Return from
        public string ReturnReturnwayBooking(string req, string Retntreq, string userMarkup, string FlightAmt, string ReturnFlightAmt, string TripMode, string deptSegment = "", string returnSegment = "")
        {
            string COrelationID = Settings.GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            int Deptstopage = 0;
            decimal TotalBookAmt = 0;
            int deptcnt = 0;
            int retncnt = 0;
            decimal.TryParse(FlightAmt, out TotalBookAmt);
            var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
            var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
            List<ReturnFlightSegments> deptureSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(deptSegment);
            List<ReturnFlightSegments> retSegment = JsonConvert.DeserializeObject<List<ReturnFlightSegments>>(returnSegment);
            if (deptureSegment != null)
            { deptcnt = deptureSegment.Count(); }
            if (retSegment != null)
            {
                retncnt = retSegment.Count();
            }
            FlightBookingDTO beforeApiExecute = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
            var FlightInfo = beforeApiExecute.RequestXml.BookTicketRequest.Segments;
            Deptstopage = FlightInfo.Segment.Count();
            var sengList = FlightInfo.Segment.ToList();
            string Dept_FlightNo = string.Empty;
            string Dept_airlineCode = string.Empty;
            string retn_FlightNo = string.Empty;
            string retn_airlineCode = string.Empty;
            string GSTCompanyName = string.Empty;
            string GSTCompanyEmail = string.Empty;
            string GSTNO = string.Empty;
            string GSTMOBILE_NO = string.Empty;
            string GSTADDRESS = string.Empty;
            GSTCompanyName = beforeApiExecute.RequestXml.BookTicketRequest.GSTCompanyName;
            GSTCompanyEmail = beforeApiExecute.RequestXml.BookTicketRequest.GSTEmailID;
            GSTNO = beforeApiExecute.RequestXml.BookTicketRequest.GSTNo;
            GSTMOBILE_NO = beforeApiExecute.RequestXml.BookTicketRequest.GSTMobileNo;
            GSTADDRESS = beforeApiExecute.RequestXml.BookTicketRequest.GSTAddress;
            int cntDept = 0;
            int cntretn = 0;
            string SeqNoDept = "";
            string SeqNoRtn = "";
            DateTime R_DeptDate = new DateTime();
            DateTime R_RetnDate = new DateTime();
            if (TripMode == "R")
            {
                var SegInfodept = sengList.Where(x => x.TrackNo.Contains("O")).ToList();
                var SegInforeturn = sengList.Where(x => x.TrackNo.Contains("R")).ToList();
                Dept_FlightNo = SegInfodept[0].FlightNo;
                Dept_airlineCode = SegInfodept[0].AirlineCode;
                //retn_FlightNo = SegInforeturn[0].FlightNo;
                //retn_airlineCode = SegInforeturn[0].AirlineCode;
                cntDept = SegInfodept.Count();
                cntretn = SegInforeturn.Count();
                SeqNoDept = SegInfodept[0].SegmentSeqNo;
                SeqNoRtn = SegInforeturn[0].SegmentSeqNo;
                R_DeptDate = Convert.ToDateTime(SegInfodept[0].DepDate);
                R_RetnDate = Convert.ToDateTime(SegInforeturn[0].DepDate);
            }
            var PassngFlight = beforeApiExecute.RequestXml.BookTicketRequest.Passengers;
            var PassngFlightCount = beforeApiExecute.RequestXml.BookTicketRequest.Passengers.Passenger.Count;
            var FltAmt = beforeApiExecute.RequestXml.BookTicketRequest.TotalAmount;
            int Flightcount = beforeApiExecute.RequestXml.BookTicketRequest.Segments.Segment.Count;
            #region variable Declieare     
            decimal AirAdditionalCharge = 0;
            decimal GSTAmount = 0;
            decimal Holding_Amount = 0;
            decimal UserMarkUp_Value = 0;
            decimal.TryParse(userMarkup, out UserMarkUp_Value);
            //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
            string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
            string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
            string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
            decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
            decimal.TryParse(GSTValue, out GSTAmount);
            decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
            decimal AdminGST = 0;
            decimal Publish_Fare = 0;
            decimal NET_FARE = 0;
            decimal NET_TOTAL_FARE = 0;
            decimal RESCHEDULE_FARE = 0;
            string ResValue = string.Empty;
            dynamic VerifyFlight = null;
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
                    decimal UserMarkupGST = 0;
                    UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);
                    TBL_FLIGHT_BOOKING_DETAILS objflightOne = new TBL_FLIGHT_BOOKING_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        DIST_ID = getmemberinfo.INTRODUCER,
                        WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                        CORELATION_ID = COrelationID,
                        PNR = PNR,
                        REF_NO = Ref_no,
                        TRACK_NO = "",
                        TRIP_MODE = TripMode,
                        TICKET_NO = Ref_no,
                        TICKET_TYPE = TripMode,
                        IS_DOMESTIC = false,
                        //AIRLINE_CODE = FlightInfo.Segment[0].AirlineCode,
                        //FLIGHT_NO = FlightInfo.Segment[0].FlightNo,
                        AIRLINE_CODE = Dept_airlineCode,
                        FLIGHT_NO = Dept_FlightNo,
                        //FROM_AIRPORT = deptureSegment.Segment[0].FromAirportCode,
                        //FROM_AIRPORT = RetdeptFromAir,
                        //TO_AIRPORT = RetdeptToAir,
                        FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                        TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode,
                        //TO_AIRPORT = deptureSegment.Segment[Flightcount - 1].ToAirportCode,
                        BOOKING_DATE = DateTime.Now,
                        DEPT_DATE = DeptDate,
                        DEPT_TIME = Depttime,
                        ARRIVE_DATE = arivDate,
                        ARRIVE_TIME = arivtime,
                        NO_OF_ADULT = AdultVal,
                        NO_OF_CHILD = childVal,
                        NO_OF_INFANT = InfantVal,
                        TOTAL_FLIGHT_BASE_FARE = 0,
                        TOTAL_FLIGHT_TAX = 0,
                        TOTAL_PASSANGER_TAX = 0,
                        TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = 0,
                        TOTAL_FLIGHT_CUTE_FEE = 0,
                        TOTAL_FLIGHT_MEAL_FEE = 0,
                        TOTAL_AIRPORT_FEE = 0,
                        TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                        TOTAL_FLIGHT_AMT = Convert.ToDecimal(FltAmt),
                        TOTAL_COMMISSION_AMT = TotalCommAmt,
                        TOTAL_TDS_AMT = TotalTDSAmountAmt,
                        TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
                        TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
                        STATUS = true,
                        IS_CANCELLATION = false,
                        FLIGHT_CANCELLATION_ID = "",
                        IS_HOLD = false,
                        API_RESPONSE = "",
                        FLIGHT_BOOKING_DATE = BookingDate,
                        MAIN_CLASS = FlightInfo.Segment[0].MainClass,
                        BOOKING_CLASS = FlightInfo.Segment[0].FlightClass,
                        BOOKING_STATUS = "PENDING",
                        USER_MARKUP = UserMarkUp_Value,
                        ADMIN_MARKUP = AirAdditionalCharge,
                        COMM_SLAP = 0,
                        ADMIN_GST = AdminGST,
                        ADMIN_cGST = 0,
                        ADMIN_sGST = 0,
                        ADMIN_iGST = 0,
                        USER_MARKUP_GST = UserMarkupGST,
                        USER_MARKUP_cGST = 0,
                        USER_MARKUP_sGST = 0,
                        USER_MARKUP_iGST = 0,
                        OP_MODE = "BOOKED",
                        HOLD_CHARGE = 0,
                        HOLD_CGST = 0,
                        HOLD_IGST = 0,
                        HOLD_SGST = 0,
                        PASSAGER_SEGMENT = SeqNoDept,
                        API_REQUEST = req,
                        STOPAGE = (cntDept - 1),
                        COMPANY_NAME = GSTCompanyName,
                        COMPANY_EMAIL_ID = GSTCompanyEmail,
                        COMPANY_GST_NO = GSTNO,
                        COMPANY_MOBILE = GSTMOBILE_NO,
                        COMPANY_GST_ADDRESS = GSTADDRESS,
                    };
                    _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflightOne);
                    //_db.SaveChanges();
                    foreach (var pnsgitem in PassngFlight.Passenger)
                    {
                        //var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        //string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        //string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        //string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        var val_Pnsg_Details = "";
                        string Pngdetails = "";
                        string PngDOB = Convert.ToString(pnsgitem.DateOfBirth);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.PaxSeqNo);
                        TBL_FLIGHT_BOOKING_PASSENGER_LIST objpnsg = new TBL_FLIGHT_BOOKING_PASSENGER_LIST()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            REF_NO = Ref_no,
                            PNR = PNR,
                            TITLE = pnsgitem.Title,
                            FIRST_NAME = pnsgitem.FirstName,
                            LAST_NAME = pnsgitem.LastName,
                            PASSENGER_TYPE = pnsgitem.PassengerType,
                            GENDER = "",
                            BIRTH_DATE = PngDOB,
                            DETAILS = Pngdetails,
                            PASSENGER_RESP = "",
                            CREATE_DATE = DateTime.Now,
                            PNSG_SEQ_NO = PnsgSeq_No,
                            CORELATION_ID = COrelationID,
                            PASSENGER_STATUS = "PENDING",
                            CancelReqNo = "",
                            TRIP_TYPE = "O",
                            DOJ = R_DeptDate,
                            FLIGHT_SEGMENT = SeqNoDept,
                            FROM_AIRPORT = deptureSegment[0].FromAirportCode,
                            TO_AIRPORT = deptureSegment[deptcnt - 1].ToAirportCode
                        };
                        _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Add(objpnsg);
                        //_db.SaveChanges();
                    }
                    decimal mmainBlc = 0;
                    decimal SubMainBlc = 0;
                    decimal Closing = 0;
                    decimal mainClosing = 0;
                    decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
                    //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (getmemberinfo.BALANCE != null)
                    {
                        decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                        //SubMainBlc = mmainBlc - TotalAmt;  //TotalBookAmt
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    else
                    {
                        //SubMainBlc = mmainBlc - TotalAmt;
                        SubMainBlc = mmainBlc - T_Amount;
                        getmemberinfo.BALANCE = SubMainBlc;
                    }
                    getmemberinfo.BALANCE = SubMainBlc;
                    _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                    //_db.SaveChanges();
                    //var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                    if (MemberAcntLog != null)
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    else
                    {
                        Closing = MemberAcntLog.CLOSING;
                        //mainClosing = Closing - TotalAmt;
                        mainClosing = Closing - T_Amount;
                    }
                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = CurrentMerchant.MEM_ID,
                        MEMBER_TYPE = "RETAILER",
                        TRANSACTION_TYPE = "FLIGHT BOOKING",
                        TRANSACTION_DATE = System.DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "DR",
                        //AMOUNT = TotalAmt,
                        AMOUNT = T_Amount,
                        NARRATION = "Debit amount for flight booking",
                        OPENING = Closing,
                        CLOSING = mainClosing,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (float)GSTAmount,
                        TDS = 0,
                        IPAddress = "",
                        TDS_PERCENTAGE = 0,
                        //GST_PERCENTAGE = AdminGST,
                        GST_PERCENTAGE = 0,
                        WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                        //SUPER_ID = (long)SUP_MEM_ID,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID,
                        REC_COMM_TYPE = "",
                        COMM_VALUE = 0,
                        NET_COMM_AMT = 0,
                        TDS_DR_COMM_AMT = 0,
                        CGST_COMM_AMT_INPUT = 0,
                        CGST_COMM_AMT_OUTPUT = 0,
                        SGST_COMM_AMT_INPUT = 0,
                        SGST_COMM_AMT_OUTPUT = 0,
                        IGST_COMM_AMT_INPUT = 0,
                        IGST_COMM_AMT_OUTPUT = 0,
                        TOTAL_GST_COMM_AMT_INPUT = 0,
                        TOTAL_GST_COMM_AMT_OUTPUT = 0,
                        TDS_RATE = 0,
                        CGST_RATE = 0,
                        SGST_RATE = 0,
                        IGST_RATE = 0,
                        TOTAL_GST_RATE = 0,
                        COMM_SLAB_ID = 0,
                        STATE_ID = getmemberinfo.STATE_ID,
                        FLAG1 = 0,
                        FLAG2 = 0,
                        FLAG3 = 0,
                        FLAG4 = 0,
                        FLAG5 = 0,
                        FLAG6 = 0,
                        FLAG7 = 0,
                        FLAG8 = 0,
                        FLAG9 = 0,
                        FLAG10 = 0,
                        VENDOR_ID = 0
                    };
                    _db.TBL_ACCOUNTS.Add(objCommPer);
                    _db.SaveChanges();
                    #endregion
                    #region Booking Api call
                    //dynamic VerifyFlight = null;
                    VerifyFlight = MultiLinkAirAPI.BookedFlightTicket(req, COrelationID);
                    ResValue = Convert.ToString(VerifyFlight);
                    //var APIStatus = VerifyFlight.BookTicketResponse.Error;
                    var getStatus = VerifyFlight.BookTicketResponses.BookTicketResponse[0].TicketDetails[0].Status.Value;
                    if (getStatus == "Acknowledged" || getStatus == "Completed")
                    {
                        ContextTransaction.Commit();
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        //return Json("Please try again later", JsonRequestBehavior.AllowGet);
                        return "Please try again later";
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return "false";
                    //return Json(false, JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
            using (System.Data.Entity.DbContextTransaction ContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
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
                    string DATEofJourney = DeptDate;
                    DateTime dateofJourey = DateTime.Parse(DATEofJourney, new System.Globalization.CultureInfo("pt-BR"));
                    //DateTime dateofJourey =Convert.ToDateTime(DATEofJourney);
                    DeptDate = FareDetails[0].DepartureTime;
                    arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
                    arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
                    adult = TicketInfo[Ticketcount - 1].Adult;
                    AdultVal = int.Parse(adult);
                    child = TicketInfo[Ticketcount - 1].Child;
                    childVal = int.Parse(child);
                    infant = TicketInfo[Ticketcount - 1].Infant;
                    InfantVal = int.Parse(infant);
                    //string AIRADDITIONALAMOUNT_Value = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                    string AIRADDITIONALAMOUNT_Value = Session["AIRADDITIONALAMOUNT"].ToString();
                    decimal Additional_AMt = 0;
                    decimal.TryParse(AIRADDITIONALAMOUNT_Value, out Additional_AMt);
                    TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
                    Publish_Fare = decimal.Parse(TotalFlightBaseFare) + Additional_AMt;
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
                    NET_FARE = TotalAmt;
                    NET_TOTAL_FARE = NET_FARE + Additional_AMt;
                    TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
                    TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
                    TotalServiceCharge = FareDetails[0].TotalServiceCharge;
                    TotalServiceAmt = decimal.Parse(TotalServiceCharge);
                    TDSAmount = FareDetails[0].TDSAmount;
                    TotalTDSAmountAmt = decimal.Parse(TDSAmount);
                    ServiceTax = FareDetails[0].ServiceTax;
                    TotalServiceTaxAmt = decimal.Parse(ServiceTax);
                    AdultCheckedIn = FareDetails[0].AdultCheckedIn;
                    string jhjdfd = "";
                    var GetFligtInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.CORELATION_ID == COrelationID).ToList();
                    if (GetFligtInfo != null)
                    {
                        foreach (var ticketInfo in GetFligtInfo)
                        {
                            var flightTicketInfo = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == ticketInfo.SLN);
                            flightTicketInfo.PNR = PNR;
                            flightTicketInfo.REF_NO = Ref_no;
                            flightTicketInfo.TRACK_NO = "";
                            flightTicketInfo.TRIP_MODE = "1";
                            flightTicketInfo.TICKET_TYPE = TicketType;
                            flightTicketInfo.TICKET_NO = Ref_no;
                            flightTicketInfo.IS_DOMESTIC = IsDomestic;
                            //flightTicketInfo.AIRLINE_CODE = Airlinecode;
                            //flightTicketInfo.FLIGHT_NO = FlightNo;
                            //flightTicketInfo.FROM_AIRPORT = FromAirport;
                            //flightTicketInfo.TO_AIRPORT = ToAirport;
                            flightTicketInfo.BOOKING_DATE = DateTime.Now;
                            flightTicketInfo.DEPT_DATE = DeptDate;
                            flightTicketInfo.DEPT_TIME = Depttime;
                            flightTicketInfo.ARRIVE_DATE = arivDate;
                            flightTicketInfo.ARRIVE_TIME = arivtime;
                            flightTicketInfo.NO_OF_ADULT = AdultVal;
                            flightTicketInfo.NO_OF_CHILD = childVal;
                            flightTicketInfo.NO_OF_INFANT = InfantVal;
                            flightTicketInfo.TOTAL_FLIGHT_BASE_FARE = TotalBaseFare;
                            flightTicketInfo.TOTAL_FLIGHT_TAX = TotalTaxFare;
                            flightTicketInfo.TOTAL_PASSANGER_TAX = TotalPngsTaxFare;
                            flightTicketInfo.TOTAL_FLIGHT_SERVICE_CHARGES = 0;
                            flightTicketInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare;
                            flightTicketInfo.TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare;
                            flightTicketInfo.TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare;
                            flightTicketInfo.TOTAL_AIRPORT_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_CONVENIENCE_FEE = 0;
                            flightTicketInfo.TOTAL_FLIGHT_AMT = TotalAmt;
                            flightTicketInfo.TOTAL_COMMISSION_AMT = TotalCommAmt;
                            flightTicketInfo.TOTAL_TDS_AMT = TotalTDSAmountAmt;
                            flightTicketInfo.TOTAL_SERVICES_TAX = TotalServiceTaxAmt;
                            flightTicketInfo.TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn;
                            flightTicketInfo.STATUS = true;
                            flightTicketInfo.IS_CANCELLATION = false;
                            flightTicketInfo.FLIGHT_CANCELLATION_ID = "";
                            flightTicketInfo.IS_HOLD = false;
                            flightTicketInfo.API_RESPONSE = APIRes;
                            flightTicketInfo.FLIGHT_BOOKING_DATE = BookingDate;
                            flightTicketInfo.BOOKING_STATUS = BookingStatus;
                            flightTicketInfo.MAIN_CLASS = MainClass;
                            flightTicketInfo.BOOKING_CLASS = BookingClass;
                            flightTicketInfo.PUBLISH_FARE = Publish_Fare;
                            flightTicketInfo.NET_FARE = NET_FARE;
                            flightTicketInfo.NET_TOTAL_FARE = NET_TOTAL_FARE;
                            flightTicketInfo.CANCELLATION_REMARK = "";
                            flightTicketInfo.RESCHEDULE_FARE = false;
                            flightTicketInfo.RESCHEDULE_REMARK = "";
                            _db.Entry(flightTicketInfo).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                            //ContextTransaction.Commit();
                        }
                    }
                    foreach (var pnsgitem in PassengerDetails)
                    {
                        var val_Pnsg_Details = JsonConvert.SerializeObject(pnsgitem.Details);
                        string Pngdetails = Convert.ToString(val_Pnsg_Details);
                        string PngDOB = Convert.ToString(pnsgitem.BirthDate);
                        string PnsgSeq_No = Convert.ToString(pnsgitem.SeqNo);
                        string Png_FirstName = Convert.ToString(pnsgitem.FirstName);
                        string Png_LastName = Convert.ToString(pnsgitem.LastName);
                        var PnsgList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.CORELATION_ID == COrelationID && x.FIRST_NAME == Png_FirstName && x.LAST_NAME == Png_LastName && x.TRIP_TYPE == "O");
                        if (PnsgList != null)
                        {
                            PnsgList.PNR = PNR;
                            PnsgList.REF_NO = Ref_no;
                            PnsgList.DETAILS = Pngdetails;
                            PnsgList.GENDER = pnsgitem.Gender;
                            PnsgList.PASSENGER_RESP = pnsgRes;
                            PnsgList.PNSG_SEQ_NO = PnsgSeq_No;
                            PnsgList.PASSENGER_STATUS = BookingStatus;
                            PnsgList.CANCELLATION_REMARTK = "";
                            PnsgList.PASSPORT = "";
                            _db.Entry(PnsgList).State = System.Data.Entity.EntityState.Modified;
                            //_db.SaveChanges();
                        }
                    }
                    _db.SaveChanges();
                    ContextTransaction.Commit();
                    #endregion
                    //TempData["IsShowPrintTicket"] = "Show";
                    //return Json(data, JsonRequestBehavior.AllowGet);
                    return "Return Retn Success";
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    //string check = RefundAmount(COrelationID, TripMode);
                    return "Return Retn Fail";
                    throw;
                }
            }
        }
        #endregion

        #region Flight Price Calender
        public ActionResult GetFlightPriceCalender(string DepartureDate, string ArrivalDate, int TripType, string FromSourceCode, string ToDestinationCode)
        {
            try
            {
                dynamic fareCalender = MultiLinkAirAPI.FareCalender(
                    DepartureDate,
                    ArrivalDate,
                    TripType,
                    FromSourceCode,
                    ToDestinationCode);

                var data = JsonConvert.SerializeObject(fareCalender);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public ActionResult BookedFlightInformaiton()
        {
            if ((TempData["IsShowPrintTicket"] as string) == "Show")
            {
                string Ref_No = TempData["IsShowRef_NoTicket"].ToString();
                string PNR = TempData["IsShowPNRTicket"].ToString();
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(Ref_No, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed")
                {
                    ViewBag.IsShowPrintTicket = "Show";
                }
                else
                { ViewBag.IsShowPrintTicket = ""; } 
            }
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
        public JsonResult FlightBookingInvoice(DateTime? fromDate, DateTime? toDate,string BookedStatus,string PNRNo="")
        {
            initpage();
            try
            {
                var db = new DBContext();
                if (fromDate != null && toDate != null && BookedStatus!="--Select--")
                {
                    DateTime valueFrom = Convert.ToDateTime(toDate);
                    DateTime ToDateVal = valueFrom.AddDays(1);
                    //var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.BOOKING_STATUS != "Cancelled" && x.BOOKING_DATE >= fromDate && x.BOOKING_DATE<= ToDateVal).OrderByDescending(z => z.BOOKING_DATE).ToList();
                    var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.BOOKING_DATE >= fromDate && x.BOOKING_DATE <= ToDateVal && x.BOOKING_STATUS == BookedStatus).OrderByDescending(z => z.BOOKING_DATE).ToList();
                    return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);
                }
                else if (PNRNo != "" && BookedStatus!= "--Select--")
                {
                    var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.PNR== PNRNo && x.BOOKING_STATUS == BookedStatus).OrderByDescending(z => z.BOOKING_DATE).ToList();

                    return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);
                }
                else if (PNRNo == "" && BookedStatus != "--Select--")
                {
                    var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.BOOKING_STATUS == BookedStatus).OrderByDescending(z => z.BOOKING_DATE).ToList();

                    return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);
                }
                else if (PNRNo != "" && BookedStatus == "--Select--")
                {
                    var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.PNR == PNRNo).OrderByDescending(z => z.BOOKING_DATE).ToList();

                    return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var GetBookedFlightList = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.BOOKING_DATE).ToList();

                    return Json(GetBookedFlightList, JsonRequestBehavior.AllowGet);
                }
                

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
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                string COMPANY_GST_NO = GetMemberInfo.COMPANY_GST_NO;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                //ViewBag.Additionalcharge = AIRADDITIONALAMOUNT;
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                //ViewBag.processingCharge = Updatestatus.USER_MARKUP;
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(refId,"", PNR, "","","");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {
                    //var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION = true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                //return Json(data, JsonRequestBehavior.AllowGet);
                //return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP });
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP, Address = CompAddress, CompName = Companyname, CompEmail = CompanyEmail, CompContact = CompanyMobile, CompGSTNo = COMPANY_GST_NO });

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PrintWithOutFareInvoice(string refId, string PNR)
        {
            try
            {
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                string COMPANY_GST_NO = GetMemberInfo.COMPANY_GST_NO;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                //ViewBag.Additionalcharge = AIRADDITIONALAMOUNT;
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                //ViewBag.processingCharge = Updatestatus.USER_MARKUP;
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(refId, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {
                    //var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION = true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                //return Json(data, JsonRequestBehavior.AllowGet);
                //return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP });
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP, Address = CompAddress, CompName = Companyname, CompEmail = CompanyEmail, CompContact = CompanyMobile, CompGSTNo = COMPANY_GST_NO });

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PrintPublishFareInvoice(string refId, string PNR)
        {
            try
            {
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                string COMPANY_GST_NO = GetMemberInfo.COMPANY_GST_NO;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                //ViewBag.Additionalcharge = AIRADDITIONALAMOUNT;
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                //ViewBag.processingCharge = Updatestatus.USER_MARKUP;
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(refId, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {
                    //var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION = true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                //return Json(data, JsonRequestBehavior.AllowGet);
                //return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP });
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP, Address = CompAddress, CompName = Companyname, CompEmail = CompanyEmail, CompContact = CompanyMobile, CompGSTNo = COMPANY_GST_NO,FlightDetails= Updatestatus });

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PrintNetFareInvoice(string refId, string PNR)
        {
            try
            {
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                string COMPANY_GST_NO = GetMemberInfo.COMPANY_GST_NO;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                //ViewBag.Additionalcharge = AIRADDITIONALAMOUNT;
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                //ViewBag.processingCharge = Updatestatus.USER_MARKUP;
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.printBookTicket(refId, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {
                    //var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION = true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                //return Json(data, JsonRequestBehavior.AllowGet);
                //return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP });
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP, Address = CompAddress, CompName = Companyname, CompEmail = CompanyEmail, CompContact = CompanyMobile, CompGSTNo = COMPANY_GST_NO });

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TicketInformationGet(string refId, string PNR)
        {
            try
            {
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                string COMPANY_GST_NO = GetMemberInfo.COMPANY_GST_NO;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                //ViewBag.Additionalcharge = AIRADDITIONALAMOUNT;
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                //ViewBag.processingCharge = Updatestatus.USER_MARKUP;
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.GetTicketInformation(refId, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {
                    //var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x=>x.REF_NO==refId);
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION=true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
               
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP,Address= CompAddress,CompName= Companyname,CompEmail= CompanyEmail,CompContact= CompanyMobile, CompGSTNo = COMPANY_GST_NO });
                //return Json(new { result = data,AdditionalCharge= AIRADDITIONALAMOUNT ,ProcessingCharge= Updatestatus.USER_MARKUP });

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TIcketCancellationStatusCheck(string refId, string PNR)
        {
            try
            {
                var db = new DBContext();
                string StateName = string.Empty;
                var GetMemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                var getState = db.TBL_STATES.FirstOrDefault(x => x.STATEID == GetMemberInfo.STATE_ID);
                if (getState != null)
                { StateName = getState.STATENAME; }
                else
                { StateName = ""; }
                string CompAddress = GetMemberInfo.ADDRESS + "," + GetMemberInfo.CITY + "," + StateName + "," + GetMemberInfo.PIN;
                string Companyname = GetMemberInfo.COMPANY;
                string CompanyEmail = GetMemberInfo.EMAIL_ID;
                string CompanyMobile = GetMemberInfo.MEMBER_MOBILE;
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];                
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                var Updatestatus = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);                
                dynamic PrintFlghtInvoice = MultiLinkAirAPI.GetTicketInformation(refId, "", PNR, "", "", "");
                var TicketStatus = PrintFlghtInvoice.PNRDetailsResponse.TicketDetails[0].Status.Value;
                if (TicketStatus == "Completed" || TicketStatus == "Fully Cancelled" || TicketStatus == "Partially Cancelled")
                {                    
                    Updatestatus.BOOKING_STATUS = TicketStatus;
                    if (TicketStatus == "Fully Cancelled")
                    {
                        Updatestatus.IS_CANCELLATION = true;
                        Updatestatus.CANCELLATION_DATE = DateTime.Now;
                    }
                    _db.Entry(Updatestatus).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
                var data = JsonConvert.SerializeObject(PrintFlghtInvoice);
                //return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP });
                return Json(new { result = data, AdditionalCharge = AIRADDITIONALAMOUNT, ProcessingCharge = Updatestatus.USER_MARKUP, Address = CompAddress, CompName = Companyname, CompEmail = CompanyEmail, CompContact = CompanyMobile });
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost] 
        public JsonResult ConfirmHoldTicket(string refId, string corelation)
        {
            try
            {
                var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);

                #region variable Declieare            
                string uMark = "";
                decimal TotalBookAmt = 0;
                var UserMarkupdata = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId && x.CORELATION_ID == corelation && x.IS_HOLD == true);
                if (UserMarkupdata.USER_MARKUP != null)
                {
                    uMark = UserMarkupdata.USER_MARKUP.ToString();
                }
                else
                {
                    uMark = "0";
                }
                decimal.TryParse(UserMarkupdata.TOTAL_FLIGHT_AMT.ToString(),out TotalBookAmt);
                decimal AirAdditionalCharge = 0;
                decimal GSTAmount = 0;
                decimal Holding_Amount = 0;
                decimal UserMarkUp_Value = 0;
                decimal.TryParse(uMark, out UserMarkUp_Value);
                //string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
                string AIRADDITIONALAMOUNT = Session["AIRADDITIONALAMOUNT"].ToString();
                string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
                string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
                decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
                decimal.TryParse(GSTValue, out GSTAmount);
                decimal.TryParse(HOLDCHARGES, out Holding_Amount);
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
                decimal AdminGST = 0;
                AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
                decimal UserMarkupGST = 0;
                UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);

                decimal mmainBlc = 0;
                decimal SubMainBlc = 0;
                decimal Closing = 0;
                decimal mainClosing = 0;
                decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
                //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                if (getmemberinfo.BALANCE != null)
                {
                    decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
                    //SubMainBlc = mmainBlc - TotalAmt;
                    SubMainBlc = mmainBlc - T_Amount;
                    getmemberinfo.BALANCE = SubMainBlc;
                }
                else
                {
                    //SubMainBlc = mmainBlc - TotalAmt;
                    SubMainBlc = mmainBlc - T_Amount;
                    getmemberinfo.BALANCE = SubMainBlc;
                }
                getmemberinfo.BALANCE = SubMainBlc;
                _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                if (MemberAcntLog != null)
                {
                    Closing = MemberAcntLog.CLOSING;
                    //mainClosing = Closing - TotalAmt;
                    mainClosing = Closing - T_Amount;
                }
                else
                {
                    Closing = MemberAcntLog.CLOSING;
                    //mainClosing = Closing - TotalAmt;
                    mainClosing = Closing - T_Amount;
                }
                TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                {
                    API_ID = 0,
                    MEM_ID = CurrentMerchant.MEM_ID,
                    MEMBER_TYPE = "RETAILER",
                    TRANSACTION_TYPE = "FLIGHT BOOKING",
                    TRANSACTION_DATE = System.DateTime.Now,
                    TRANSACTION_TIME = DateTime.Now,
                    DR_CR = "DR",
                    //AMOUNT = TotalAmt,
                    AMOUNT= T_Amount,
                    NARRATION = "Debit amount for flight booking",
                    OPENING = Closing,
                    CLOSING = mainClosing,
                    REC_NO = 0,
                    COMM_AMT = 0,
                    GST = (float)GSTAmount,
                    TDS = 0,
                    IPAddress = "",
                    TDS_PERCENTAGE = 0,
                    GST_PERCENTAGE = AdminGST,
                    WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
                    //SUPER_ID = (long)SUP_MEM_ID,
                    SUPER_ID = 0,
                    DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
                    SERVICE_ID = 0,
                    CORELATIONID = corelation,
                    REC_COMM_TYPE = "",
                    COMM_VALUE = 0,
                    NET_COMM_AMT = 0,
                    TDS_DR_COMM_AMT = 0,
                    CGST_COMM_AMT_INPUT = 0,
                    CGST_COMM_AMT_OUTPUT = 0,
                    SGST_COMM_AMT_INPUT = 0,
                    SGST_COMM_AMT_OUTPUT = 0,
                    IGST_COMM_AMT_INPUT = 0,
                    IGST_COMM_AMT_OUTPUT = 0,
                    TOTAL_GST_COMM_AMT_INPUT = 0,
                    TOTAL_GST_COMM_AMT_OUTPUT = 0,
                    TDS_RATE = 0,
                    CGST_RATE = 0,
                    SGST_RATE = 0,
                    IGST_RATE = 0,
                    TOTAL_GST_RATE = 0,
                    COMM_SLAB_ID = 0,
                    STATE_ID = getmemberinfo.STATE_ID,
                    FLAG1 = 0,
                    FLAG2 = 0,
                    FLAG3 = 0,
                    FLAG4 = 0,
                    FLAG5 = 0,
                    FLAG6 = 0,
                    FLAG7 = 0,
                    FLAG8 = 0,
                    FLAG9 = 0,
                    FLAG10 = 0,
                    VENDOR_ID = 0
                };
                _db.TBL_ACCOUNTS.Add(objCommPer);
                _db.SaveChanges();
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
                var GetFlightBookedInfor = _db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.REF_NO == Ref_no && x.CORELATION_ID == corelation && x.BOOKING_STATUS == "On Hold" && x.IS_HOLD == true).ToList();
                if (GetFlightBookedInfor != null)
                {
                    foreach (var Gethold in GetFlightBookedInfor)
                    {
                        var updateOldHoldDetails = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == Gethold.SLN);
                        updateOldHoldDetails.BOOKING_STATUS = "On Hold Confirm";
                        updateOldHoldDetails.IS_HOLD = false;
                        updateOldHoldDetails.BOOKING_HOLD_ID = "";
                        _db.Entry(updateOldHoldDetails).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            DIST_ID = getmemberinfo.INTRODUCER,
                            WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
                            CORELATION_ID = corelation,
                            PNR = updateOldHoldDetails.PNR,
                            REF_NO = updateOldHoldDetails.REF_NO,
                            TRACK_NO = "",
                            TRIP_MODE = "1",
                            TICKET_NO = updateOldHoldDetails.REF_NO,
                            TICKET_TYPE = updateOldHoldDetails.TICKET_TYPE,
                            IS_DOMESTIC = updateOldHoldDetails.IS_DOMESTIC,
                            AIRLINE_CODE = updateOldHoldDetails.AIRLINE_CODE,
                            FLIGHT_NO = updateOldHoldDetails.FLIGHT_NO,
                            FROM_AIRPORT = updateOldHoldDetails.FROM_AIRPORT,
                            TO_AIRPORT = updateOldHoldDetails.TO_AIRPORT,
                            BOOKING_DATE = DateTime.Now,
                            DEPT_DATE = updateOldHoldDetails.DEPT_DATE,
                            DEPT_TIME = updateOldHoldDetails.DEPT_TIME,
                            ARRIVE_DATE = updateOldHoldDetails.ARRIVE_DATE,
                            ARRIVE_TIME = updateOldHoldDetails.ARRIVE_TIME,
                            NO_OF_ADULT = updateOldHoldDetails.NO_OF_ADULT,
                            NO_OF_CHILD = updateOldHoldDetails.NO_OF_CHILD,
                            NO_OF_INFANT = updateOldHoldDetails.NO_OF_INFANT,
                            TOTAL_FLIGHT_BASE_FARE = updateOldHoldDetails.TOTAL_FLIGHT_BASE_FARE,
                            TOTAL_FLIGHT_TAX = updateOldHoldDetails.TOTAL_FLIGHT_TAX,
                            TOTAL_PASSANGER_TAX = updateOldHoldDetails.TOTAL_PASSANGER_TAX,
                            TOTAL_FLIGHT_SERVICE_CHARGES = 0,
                            TOTAL_FLIGHT_ADDITIONAL_CHARGE = updateOldHoldDetails.TOTAL_FLIGHT_ADDITIONAL_CHARGE,
                            TOTAL_FLIGHT_CUTE_FEE = updateOldHoldDetails.TOTAL_FLIGHT_CUTE_FEE,
                            TOTAL_FLIGHT_MEAL_FEE = updateOldHoldDetails.TOTAL_FLIGHT_MEAL_FEE,
                            TOTAL_AIRPORT_FEE = 0,
                            TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
                            TOTAL_FLIGHT_AMT = TotalAmt,
                            TOTAL_COMMISSION_AMT = updateOldHoldDetails.TOTAL_COMMISSION_AMT,
                            TOTAL_TDS_AMT = updateOldHoldDetails.TOTAL_TDS_AMT,
                            TOTAL_SERVICES_TAX = updateOldHoldDetails.TOTAL_SERVICES_TAX,
                            TOTAL_BAGGAGE_ALLOWES = updateOldHoldDetails.TOTAL_BAGGAGE_ALLOWES,
                            STATUS = true,
                            IS_CANCELLATION = false,
                            FLIGHT_CANCELLATION_ID = "",
                            IS_HOLD = false,
                            BOOKING_HOLD_ID = "",
                            API_RESPONSE = APIRes,
                            FLIGHT_BOOKING_DATE = updateOldHoldDetails.FLIGHT_BOOKING_DATE,
                            MAIN_CLASS = updateOldHoldDetails.MAIN_CLASS,
                            BOOKING_CLASS = updateOldHoldDetails.BOOKING_CLASS,
                            BOOKING_STATUS = BookingStatus,
                            USER_MARKUP = updateOldHoldDetails.USER_MARKUP,
                            ADMIN_MARKUP = updateOldHoldDetails.ADMIN_MARKUP,
                            COMM_SLAP = 0,
                            ADMIN_GST = updateOldHoldDetails.ADMIN_GST,
                            ADMIN_cGST = 0,
                            ADMIN_sGST = 0,
                            ADMIN_iGST = 0,
                            USER_MARKUP_GST = updateOldHoldDetails.USER_MARKUP_GST,
                            USER_MARKUP_cGST = 0,
                            USER_MARKUP_sGST = 0,
                            USER_MARKUP_iGST = 0,
                            OP_MODE = "BOOKED",
                            HOLD_CHARGE = 0,
                            HOLD_CGST = 0,
                            HOLD_IGST = 0,
                            HOLD_SGST = 0,
                            PASSAGER_SEGMENT = updateOldHoldDetails.PASSAGER_SEGMENT,
                            API_REQUEST = updateOldHoldDetails.API_REQUEST,
                            STOPAGE = updateOldHoldDetails.STOPAGE
                        };
                        _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
                        _db.SaveChanges();

                    }
                    
                    
                }

                      
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult ConfirmHoldTicket(string refId, string corelation)
        //{
        //    try
        //    {
        //        var getmemberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);

        //        #region variable Declieare            
        //        string uMark = "";
        //        decimal TotalBookAmt = 0;
        //        var UserMarkupdata = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId && x.CORELATION_ID == corelation && x.IS_HOLD == true);
        //        if (UserMarkupdata.USER_MARKUP != null)
        //        {
        //            uMark = UserMarkupdata.USER_MARKUP.ToString();
        //        }
        //        else
        //        {
        //            uMark = "0";
        //        }
        //        decimal.TryParse(UserMarkupdata.TOTAL_FLIGHT_AMT.ToString(), out TotalBookAmt);
        //        decimal AirAdditionalCharge = 0;
        //        decimal GSTAmount = 0;
        //        decimal Holding_Amount = 0;
        //        decimal UserMarkUp_Value = 0;
        //        decimal.TryParse(uMark, out UserMarkUp_Value);
        //        string AIRADDITIONALAMOUNT = System.Configuration.ConfigurationManager.AppSettings["AIRADDITIONALAMOUNT"];
        //        string GSTValue = System.Configuration.ConfigurationManager.AppSettings["GSTVALUE"];
        //        string HOLDCHARGES = System.Configuration.ConfigurationManager.AppSettings["HOLDCHARGES"];
        //        decimal.TryParse(AIRADDITIONALAMOUNT, out AirAdditionalCharge);
        //        decimal.TryParse(GSTValue, out GSTAmount);
        //        decimal.TryParse(HOLDCHARGES, out Holding_Amount);
        //        string BookingDate = string.Empty;
        //        string Ref_no = string.Empty;
        //        string PNR = string.Empty;
        //        string adult = string.Empty;
        //        int AdultVal = 0;
        //        int childVal = 0;
        //        int InfantVal = 0;
        //        string child = string.Empty;
        //        string infant = string.Empty;
        //        bool IsDomestic = false;
        //        string TicketType = string.Empty;
        //        string Airlinecode = string.Empty;
        //        string FlightNo = string.Empty;
        //        string FromAirport = string.Empty;
        //        string ToAirport = string.Empty;
        //        string DeptDate = string.Empty;
        //        string Depttime = string.Empty;
        //        string arivDate = string.Empty;
        //        string arivtime = string.Empty;
        //        string TotalFlightBaseFare = string.Empty;
        //        decimal TotalBaseFare = 0;
        //        string TotalFlightTax = string.Empty;
        //        decimal TotalTaxFare = 0;
        //        string TotalFlightPassengerTax = string.Empty;
        //        decimal TotalPngsTaxFare = 0;
        //        string TotalFlightAdditionalCharges = string.Empty;
        //        decimal TotalAdditionalFare = 0;
        //        string TotalFlightCuteFee = string.Empty;
        //        decimal TotalCuteFare = 0;
        //        string TOTAL_FLIGHT_MEAL_FEE = string.Empty;
        //        decimal TotalTOTAL_FLIGHT_MEAL_FEEare = 0;
        //        string TotalFlightAmount = string.Empty;
        //        decimal TotalAmt = 0;
        //        string TotalCommissionAMtCharge = string.Empty;
        //        decimal TotalCommAmt = 0;
        //        string TotalServiceCharge = string.Empty;
        //        decimal TotalServiceAmt = 0;
        //        string TDSAmount = string.Empty;
        //        decimal TotalTDSAmountAmt = 0;
        //        string ServiceTax = string.Empty;
        //        decimal TotalServiceTaxAmt = 0;
        //        string AdultCheckedIn = string.Empty;
        //        string MainClass = string.Empty;
        //        string BookingClass = string.Empty;
        //        string BookingStatus = string.Empty;
        //        decimal AdminGST = 0;
        //        AdminGST = ((AirAdditionalCharge * GSTAmount) / 118);
        //        decimal UserMarkupGST = 0;
        //        UserMarkupGST = ((UserMarkUp_Value * GSTAmount) / 118);

        //        decimal mmainBlc = 0;
        //        decimal SubMainBlc = 0;
        //        decimal Closing = 0;
        //        decimal mainClosing = 0;
        //        decimal T_Amount = TotalBookAmt + UserMarkUp_Value + AirAdditionalCharge;
        //        //var Member_MainBlc = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
        //        if (getmemberinfo.BALANCE != null)
        //        {
        //            decimal.TryParse(getmemberinfo.BALANCE.ToString(), out mmainBlc);
        //            //SubMainBlc = mmainBlc - TotalAmt;
        //            SubMainBlc = mmainBlc - T_Amount;
        //            getmemberinfo.BALANCE = SubMainBlc;
        //        }
        //        else
        //        {
        //            //SubMainBlc = mmainBlc - TotalAmt;
        //            SubMainBlc = mmainBlc - T_Amount;
        //            getmemberinfo.BALANCE = SubMainBlc;
        //        }
        //        getmemberinfo.BALANCE = SubMainBlc;
        //        _db.Entry(getmemberinfo).State = System.Data.Entity.EntityState.Modified;
        //        _db.SaveChanges();
        //        var MemberAcntLog = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
        //        if (MemberAcntLog != null)
        //        {
        //            Closing = MemberAcntLog.CLOSING;
        //            //mainClosing = Closing - TotalAmt;
        //            mainClosing = Closing - T_Amount;
        //        }
        //        else
        //        {
        //            Closing = MemberAcntLog.CLOSING;
        //            //mainClosing = Closing - TotalAmt;
        //            mainClosing = Closing - T_Amount;
        //        }
        //        TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //        {
        //            API_ID = 0,
        //            MEM_ID = CurrentMerchant.MEM_ID,
        //            MEMBER_TYPE = "RETAILER",
        //            TRANSACTION_TYPE = "FLIGHT BOOKING",
        //            TRANSACTION_DATE = System.DateTime.Now,
        //            TRANSACTION_TIME = DateTime.Now,
        //            DR_CR = "DR",
        //            //AMOUNT = TotalAmt,
        //            AMOUNT = T_Amount,
        //            NARRATION = "Debit amount for flight booking",
        //            OPENING = Closing,
        //            CLOSING = mainClosing,
        //            REC_NO = 0,
        //            COMM_AMT = 0,
        //            GST = (float)GSTAmount,
        //            TDS = 0,
        //            IPAddress = "",
        //            TDS_PERCENTAGE = 0,
        //            GST_PERCENTAGE = AdminGST,
        //            WHITELEVEL_ID = (long)getmemberinfo.UNDER_WHITE_LEVEL,
        //            //SUPER_ID = (long)SUP_MEM_ID,
        //            SUPER_ID = 0,
        //            DISTRIBUTOR_ID = (long)getmemberinfo.INTRODUCER,
        //            SERVICE_ID = 0,
        //            CORELATIONID = corelation,
        //            REC_COMM_TYPE = "",
        //            COMM_VALUE = 0,
        //            NET_COMM_AMT = 0,
        //            TDS_DR_COMM_AMT = 0,
        //            CGST_COMM_AMT_INPUT = 0,
        //            CGST_COMM_AMT_OUTPUT = 0,
        //            SGST_COMM_AMT_INPUT = 0,
        //            SGST_COMM_AMT_OUTPUT = 0,
        //            IGST_COMM_AMT_INPUT = 0,
        //            IGST_COMM_AMT_OUTPUT = 0,
        //            TOTAL_GST_COMM_AMT_INPUT = 0,
        //            TOTAL_GST_COMM_AMT_OUTPUT = 0,
        //            TDS_RATE = 0,
        //            CGST_RATE = 0,
        //            SGST_RATE = 0,
        //            IGST_RATE = 0,
        //            TOTAL_GST_RATE = 0,
        //            COMM_SLAB_ID = 0,
        //            STATE_ID = getmemberinfo.STATE_ID,
        //            FLAG1 = 0,
        //            FLAG2 = 0,
        //            FLAG3 = 0,
        //            FLAG4 = 0,
        //            FLAG5 = 0,
        //            FLAG6 = 0,
        //            FLAG7 = 0,
        //            FLAG8 = 0,
        //            FLAG9 = 0,
        //            FLAG10 = 0,
        //            VENDOR_ID = 0
        //        };
        //        _db.TBL_ACCOUNTS.Add(objCommPer);
        //        _db.SaveChanges();
        //        #endregion
        //        dynamic HoldBookingConfirm = MultiLinkAirAPI.HoldTicketConfrim(refId, "");
        //        var data = JsonConvert.SerializeObject(HoldBookingConfirm);
        //        string check = "";
        //        string ResValue = Convert.ToString(HoldBookingConfirm);
        //        HoldBookingConfirmResponsesDTO BookingResponse = JsonConvert.DeserializeObject<HoldBookingConfirmResponsesDTO>(ResValue);
        //        int count = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse.Count;
        //        string APIRes = Convert.ToString(data);
        //        var TicketInfo = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].TicketDetails.ToList();
        //        var Ticketcount = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].TicketDetails.Count;
        //        var FareDetails = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].FlightFareDetails.ToList();
        //        var FareDetailsCount = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].FlightFareDetails.Count;
        //        var PassengerDetails = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].PassengerDetails.ToList();
        //        var lstpnsg = BookingResponse.HoldBookingConfirmResponses.HoldBookingConfirmResponse[0].PassengerDetails;
        //        var val_Pnsg = JsonConvert.SerializeObject(lstpnsg);
        //        string pnsgRes = Convert.ToString(val_Pnsg);
        //        BookingDate = TicketInfo[Ticketcount - 1].BookingDateTime;
        //        BookingStatus = TicketInfo[Ticketcount - 1].Status;
        //        Ref_no = TicketInfo[Ticketcount - 1].RefNo;
        //        PNR = FareDetails[FareDetailsCount - 1].AirlinePNRNumber;
        //        TicketType = TicketInfo[Ticketcount - 1].TicketType;
        //        IsDomestic = (TicketInfo[Ticketcount - 1].IsDomestic == "Yes" ? true : false);
        //        Airlinecode = FareDetails[0].AirlineCode;
        //        FlightNo = FareDetails[0].FlightNo;
        //        MainClass = FareDetails[0].MainClass;
        //        BookingClass = FareDetails[0].BookingClass;
        //        FromAirport = FareDetails[0].FromAirportCode;
        //        ToAirport = FareDetails[FareDetailsCount - 1].ToAirportCode;
        //        DeptDate = FareDetails[0].DepartureDate;
        //        DeptDate = FareDetails[0].DepartureTime;
        //        arivDate = FareDetails[FareDetailsCount - 1].ArrivalDate;
        //        arivtime = FareDetails[FareDetailsCount - 1].ArriveTime;
        //        adult = TicketInfo[Ticketcount - 1].Adult;
        //        AdultVal = int.Parse(adult);
        //        child = TicketInfo[Ticketcount - 1].Child;
        //        childVal = int.Parse(child);
        //        infant = TicketInfo[Ticketcount - 1].Infant;
        //        InfantVal = int.Parse(infant);
        //        TotalFlightBaseFare = FareDetails[0].TotalFlightBaseFare;
        //        TotalBaseFare = decimal.Parse(TotalFlightBaseFare);
        //        TotalFlightTax = FareDetails[0].TotalFlightTax;
        //        TotalTaxFare = decimal.Parse(TotalFlightTax);
        //        TotalFlightPassengerTax = FareDetails[0].TotalFlightPassengerTax;
        //        TotalPngsTaxFare = decimal.Parse(TotalFlightPassengerTax);
        //        TotalFlightAdditionalCharges = FareDetails[0].TotalFlightAdditionalCharges;
        //        TotalAdditionalFare = decimal.Parse(TotalFlightAdditionalCharges);
        //        TotalFlightCuteFee = FareDetails[0].TotalFlightCuteFee;
        //        TotalCuteFare = decimal.Parse(TotalFlightCuteFee);
        //        TOTAL_FLIGHT_MEAL_FEE = FareDetails[0].TotalFlightSkyCafeMealFee;
        //        TotalTOTAL_FLIGHT_MEAL_FEEare = decimal.Parse(TOTAL_FLIGHT_MEAL_FEE);
        //        TotalFlightAmount = FareDetails[0].TotalFlightAmount;
        //        TotalAmt = decimal.Parse(TotalFlightAmount);
        //        TotalCommissionAMtCharge = FareDetails[0].TotalFlightCommissionAmount;
        //        TotalCommAmt = decimal.Parse(TotalCommissionAMtCharge);
        //        TotalServiceCharge = FareDetails[0].TotalServiceCharge;
        //        TotalServiceAmt = decimal.Parse(TotalServiceCharge);
        //        TDSAmount = FareDetails[0].TDSAmount;
        //        TotalTDSAmountAmt = decimal.Parse(TDSAmount);
        //        ServiceTax = FareDetails[0].ServiceTax;
        //        TotalServiceTaxAmt = decimal.Parse(ServiceTax);
        //        AdultCheckedIn = FareDetails[0].AdultCheckedIn;
        //        var GetFlightBookedInfor = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == Ref_no && x.CORELATION_ID == corelation && x.BOOKING_STATUS == "On Hold" && x.IS_HOLD == true);
        //        if (GetFlightBookedInfor != null)
        //        {
        //            GetFlightBookedInfor.BOOKING_STATUS = "On Hold Confirm";
        //            GetFlightBookedInfor.IS_HOLD = false;
        //            GetFlightBookedInfor.BOOKING_HOLD_ID = "";
        //        }
        //        _db.Entry(GetFlightBookedInfor).State = System.Data.Entity.EntityState.Modified;
        //        _db.SaveChanges();
        //        TBL_FLIGHT_BOOKING_DETAILS objflight = new TBL_FLIGHT_BOOKING_DETAILS()
        //        {
        //            MEM_ID = CurrentMerchant.MEM_ID,
        //            DIST_ID = getmemberinfo.INTRODUCER,
        //            WLP_ID = getmemberinfo.UNDER_WHITE_LEVEL,
        //            CORELATION_ID = corelation,
        //            PNR = PNR,
        //            REF_NO = Ref_no,
        //            TRACK_NO = "",
        //            TRIP_MODE = "1",
        //            TICKET_NO = Ref_no,
        //            TICKET_TYPE = TicketType,
        //            IS_DOMESTIC = IsDomestic,
        //            AIRLINE_CODE = Airlinecode,
        //            FLIGHT_NO = FlightNo,
        //            FROM_AIRPORT = FromAirport,
        //            TO_AIRPORT = ToAirport,
        //            BOOKING_DATE = DateTime.Now,
        //            DEPT_DATE = DeptDate,
        //            DEPT_TIME = Depttime,
        //            ARRIVE_DATE = arivDate,
        //            ARRIVE_TIME = arivtime,
        //            NO_OF_ADULT = AdultVal,
        //            NO_OF_CHILD = childVal,
        //            NO_OF_INFANT = InfantVal,
        //            TOTAL_FLIGHT_BASE_FARE = TotalBaseFare,
        //            TOTAL_FLIGHT_TAX = TotalTaxFare,
        //            TOTAL_PASSANGER_TAX = TotalPngsTaxFare,
        //            TOTAL_FLIGHT_SERVICE_CHARGES = 0,
        //            TOTAL_FLIGHT_ADDITIONAL_CHARGE = TotalAdditionalFare,
        //            TOTAL_FLIGHT_CUTE_FEE = TotalCuteFare,
        //            TOTAL_FLIGHT_MEAL_FEE = TotalTOTAL_FLIGHT_MEAL_FEEare,
        //            TOTAL_AIRPORT_FEE = 0,
        //            TOTAL_FLIGHT_CONVENIENCE_FEE = 0,
        //            TOTAL_FLIGHT_AMT = TotalAmt,
        //            TOTAL_COMMISSION_AMT = TotalCommAmt,
        //            TOTAL_TDS_AMT = TotalTDSAmountAmt,
        //            TOTAL_SERVICES_TAX = TotalServiceTaxAmt,
        //            TOTAL_BAGGAGE_ALLOWES = AdultCheckedIn,
        //            STATUS = true,
        //            IS_CANCELLATION = false,
        //            FLIGHT_CANCELLATION_ID = "",
        //            IS_HOLD = false,
        //            BOOKING_HOLD_ID = "",
        //            API_RESPONSE = APIRes,
        //            FLIGHT_BOOKING_DATE = BookingDate,
        //            MAIN_CLASS = MainClass,
        //            BOOKING_CLASS = BookingClass,
        //            BOOKING_STATUS = BookingStatus,
        //            USER_MARKUP = UserMarkUp_Value,
        //            ADMIN_MARKUP = AirAdditionalCharge,
        //            COMM_SLAP = 0,
        //            ADMIN_GST = AdminGST,
        //            ADMIN_cGST = 0,
        //            ADMIN_sGST = 0,
        //            ADMIN_iGST = 0,
        //            USER_MARKUP_GST = UserMarkupGST,
        //            USER_MARKUP_cGST = 0,
        //            USER_MARKUP_sGST = 0,
        //            USER_MARKUP_iGST = 0,
        //            OP_MODE = "BOOKED",
        //            HOLD_CHARGE = 0,
        //            HOLD_CGST = 0,
        //            HOLD_IGST = 0,
        //            HOLD_SGST = 0
        //        };
        //        _db.TBL_FLIGHT_BOOKING_DETAILS.Add(objflight);
        //        _db.SaveChanges();
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public JsonResult BookedTicketPassangerList(string refId, string corelation,string TicketType,string FromAirport,string ToAirport)
        {
            try
            {
                //if (TicketType == "O")
                //{
                //    var GetPassangerDetails = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == refId && x.CORELATION_ID == corelation).ToList();
                //    return Json(GetPassangerDetails, JsonRequestBehavior.AllowGet);
                //}
                //else {
                    var GetPassangerDetails = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == refId && x.CORELATION_ID == corelation).ToList();
                    return Json(GetPassangerDetails, JsonRequestBehavior.AllowGet);
                //}
                
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult returnFullTicketPassangerList(string refId, string corelation, string TicketType, string FromAirport, string ToAirport)
        {
            try
            {
                var GetPassangerDetails = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == refId && x.CORELATION_ID == corelation && x.FROM_AIRPORT == FromAirport && x.TO_AIRPORT == ToAirport).ToList();
                return Json(GetPassangerDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CancelFlightTicket(string[] PngsVal, string refId,string Cancellation_Type)
        {
            try
            {
                var db = new DBContext();
                var GetPassangerList = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == refId && x.PASSENGER_STATUS == "Completed").Count();
                if (GetPassangerList > 0)
                {
                    dynamic FlightCancellation = MultiLinkAirAPI.FlightCancellation(PngsVal, refId, Cancellation_Type);
                    string APiRes = Convert.ToString(FlightCancellation);
                    var getticketinfo = FlightCancellation.TicketCancelDetails.TicketCancelResponse[0];
                    string resmas = Convert.ToString(getticketinfo);
                    var getticketCancelRefifinfo = FlightCancellation.TicketCancelDetails.CancelReqNo[0];
                    string CancellationResId = getticketCancelRefifinfo.Value;
                    string valsds345345f = "";
                    var passangerId = PngsVal;
                    var pangCount = PngsVal.Length;
                    long Pnsgid = 0;
                    decimal CancellationTotalAmount = 0;
                    decimal TotalCancelAmt = 0;
                    decimal refundAmount = 0;
                    decimal TotalRefundAmt = 0;
                    string CoRelationId = "";
                    foreach (string pnsgid in passangerId)
                    {
                        long.TryParse(pnsgid, out Pnsgid);


                        var getpassagerlist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid);
                        if (getpassagerlist != null)
                        {
                            CoRelationId = getpassagerlist.CORELATION_ID;
                            getpassagerlist.PASSENGER_STATUS = "Cancelled";
                            getpassagerlist.CANCEL_STATUS = "Partially Cancelled";
                            getpassagerlist.CancelReqNo = getticketCancelRefifinfo;
                            getpassagerlist.API_CANCELLATION_RESP = APiRes;
                            var cancelledFlightInfo = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == refId);
                            cancelledFlightInfo.Cancellation_status = "Partially Cancelled";
                            db.Entry(cancelledFlightInfo).State = System.Data.Entity.EntityState.Modified;
                            dynamic cancelHistory = MultiLinkAirAPI.FlightCancellationHistory(CancellationResId, refId, "");
                            string cancellationHisRes = Convert.ToString(cancelHistory);
                            FlightCancellationHistoryResponseDTO dtoCancellationrea = JsonConvert.DeserializeObject<FlightCancellationHistoryResponseDTO>(cancellationHisRes);
                            var CancelAmt = dtoCancellationrea.CancelTicketHistoryResponses.CancelTicketHistoryResponse[0].TicketDetails[0].TotalAmount;
                            decimal.TryParse(CancelAmt.ToString(), out CancellationTotalAmount);
                            var refundAmt = dtoCancellationrea.CancelTicketHistoryResponses.CancelTicketHistoryResponse[0].TicketDetails[0].RefundAmount;
                            decimal.TryParse(refundAmt.ToString(), out refundAmount);
                            TotalCancelAmt = CancellationTotalAmount + TotalCancelAmt;
                            TotalRefundAmt = refundAmount + TotalRefundAmt;
                            getpassagerlist.CANCELLATIONHISTORY_RESP = cancellationHisRes;
                            db.Entry(getpassagerlist).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //if (GetPassangerList == 1)
                    if (GetPassangerList == pangCount)
                    {
                        var cancelledFlightInfo = db.TBL_FLIGHT_BOOKING_DETAILS.Where(x => x.REF_NO == refId).ToList();
                        if (cancelledFlightInfo != null)
                        {
                            foreach (var itemcancel in cancelledFlightInfo)
                            {
                                var getCanceltkt = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == itemcancel.SLN);
                                CoRelationId = getCanceltkt.CORELATION_ID;
                                getCanceltkt.BOOKING_STATUS = "Cancelled";
                                getCanceltkt.Cancellation_status = "Fully Cancelled";
                                getCanceltkt.CANCELLATION_DATE = DateTime.Now;
                                getCanceltkt.IS_CANCELLATION = true;
                                getCanceltkt.FLIGHT_CANCELLATION_ID = getticketCancelRefifinfo;
                                getCanceltkt.API_CANCELLATION_RESPONSE = APiRes;
                                db.Entry(getCanceltkt).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            var pnsh_list = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.REF_NO == refId);
                            pnsh_list.CANCEL_STATUS = "Full Cancelled";
                            pnsh_list.PASSENGER_STATUS = "Cancelled";
                            db.Entry(pnsh_list).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            //dynamic cancelHistory = MultiLinkAirAPI.FlightCancellationHistory(CancellationResId, refId, "");
                            //string cancellationHisRes = Convert.ToString(cancelHistory);
                            //FlightCancellationHistoryResponseDTO dtoCancellationrea = JsonConvert.DeserializeObject<FlightCancellationHistoryResponseDTO>(cancellationHisRes);
                            //var CancelAmt = dtoCancellationrea.CancelTicketHistoryResponses.CancelTicketHistoryResponse[0].TicketDetails[0].TotalAmount;
                            //decimal.TryParse(CancelAmt.ToString(), out CancellationTotalAmount);
                            //var refundAmt = dtoCancellationrea.CancelTicketHistoryResponses.CancelTicketHistoryResponse[0].TicketDetails[0].RefundAmount;
                            //decimal.TryParse(refundAmt.ToString(), out refundAmount);
                            //TotalCancelAmt = CancellationTotalAmount;
                            //TotalRefundAmt = refundAmount;
                            //foreach (string pnsgid in passangerId)
                            //{
                            //    var getpassagerlist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid);
                            //    if (getpassagerlist != null)
                            //    {
                            //        getpassagerlist.PASSENGER_STATUS = "Cancelled";
                            //        getpassagerlist.CancelReqNo = getticketCancelRefifinfo;
                            //        getpassagerlist.API_CANCELLATION_RESP = APiRes;
                            //    }
                            //    getpassagerlist.CANCELLATIONHISTORY_RESP = cancellationHisRes;
                            //    db.Entry(getpassagerlist).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }
                    }
                    decimal mainBlc = 0;
                    decimal AddMainBlc = 0;
                    var memberinfo = _db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (memberinfo.BALANCE != null)
                    {
                        decimal.TryParse(memberinfo.BALANCE.ToString(), out mainBlc);
                        AddMainBlc = mainBlc + TotalRefundAmt;
                    }
                    else
                    {
                        AddMainBlc = mainBlc + TotalRefundAmt;
                    }
                    memberinfo.BALANCE = AddMainBlc;
                    _db.Entry(memberinfo).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    decimal Opening = 0;
                    decimal AddClosing = 0;
                    var MemberAcnt = _db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(c => c.ACC_NO).FirstOrDefault();
                    if (MemberAcnt != null)
                    {
                        Opening = MemberAcnt.CLOSING;
                        AddClosing = Opening + TotalRefundAmt;
                    }
                    else
                    {
                        Opening = MemberAcnt.CLOSING;
                        AddClosing = Opening + TotalRefundAmt;
                    }
                    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = CurrentMerchant.MEM_ID,
                        MEMBER_TYPE = "RETAILER",
                        TRANSACTION_TYPE = "REFUND TICKET BOOKING AMOUNT",
                        TRANSACTION_DATE = System.DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "CR",
                        //AMOUNT = TotalAmt,
                        AMOUNT = TotalRefundAmt,
                        NARRATION = "Credit cancelled ticket refund amount",
                        OPENING = Opening,
                        CLOSING = AddClosing,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = 0,
                        TDS = 0,
                        IPAddress = "",
                        TDS_PERCENTAGE = 0,
                        //GST_PERCENTAGE = AdminGST,
                        GST_PERCENTAGE = 0,
                        WHITELEVEL_ID = (long)memberinfo.UNDER_WHITE_LEVEL,
                        //SUPER_ID = (long)SUP_MEM_ID,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = (long)memberinfo.INTRODUCER,
                        SERVICE_ID = 0,
                        CORELATIONID = CoRelationId,
                        REC_COMM_TYPE = "",
                        COMM_VALUE = 0,
                        NET_COMM_AMT = 0,
                        TDS_DR_COMM_AMT = 0,
                        CGST_COMM_AMT_INPUT = 0,
                        CGST_COMM_AMT_OUTPUT = 0,
                        SGST_COMM_AMT_INPUT = 0,
                        SGST_COMM_AMT_OUTPUT = 0,
                        IGST_COMM_AMT_INPUT = 0,
                        IGST_COMM_AMT_OUTPUT = 0,
                        TOTAL_GST_COMM_AMT_INPUT = 0,
                        TOTAL_GST_COMM_AMT_OUTPUT = 0,
                        TDS_RATE = 0,
                        CGST_RATE = 0,
                        SGST_RATE = 0,
                        IGST_RATE = 0,
                        TOTAL_GST_RATE = 0,
                        COMM_SLAB_ID = 0,
                        STATE_ID = memberinfo.STATE_ID,
                        FLAG1 = 0,
                        FLAG2 = 0,
                        FLAG3 = 0,
                        FLAG4 = 0,
                        FLAG5 = 0,
                        FLAG6 = 0,
                        FLAG7 = 0,
                        FLAG8 = 0,
                        FLAG9 = 0,
                        FLAG10 = 0,
                        VENDOR_ID = 0
                    };
                    _db.TBL_ACCOUNTS.Add(objacnt);
                    _db.SaveChanges();
                    return Json(resmas, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Ticket Status is not Completed", JsonRequestBehavior.AllowGet);
                } 
                 
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ResheduleBookedTicket(string refId, string corelation, string BookingId, string RescheduleingText)
        {
            try
            {
                var db = new DBContext();
                long SlNo = 0;
                long.TryParse(BookingId,out SlNo);
                var FlightBookingInfo = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.SLN == SlNo);
                var GetRechedultInfo=db.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS.FirstOrDefault(x => x.REF_NO == refId && x.CORELATION_ID== corelation);
                if (GetRechedultInfo != null)
                {
                    GetRechedultInfo.RESCHEDULE_REMARK = RescheduleingText;
                    GetRechedultInfo.RESCHEDULE_DATE = DateTime.Now;
                    GetRechedultInfo.RESCHEDULE_AMOUNT = 0;
                    GetRechedultInfo.RESCHEDULE_STATUS = "Pending";
                    db.Entry(GetRechedultInfo).State = System.Data.Entity.EntityState.Modified;
                    FlightBookingInfo.RESCHEDULE_REMARK = "RECHEDULE UNDER PROCESS";
                    db.Entry(FlightBookingInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Flight rechedule process is under process", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TBL_RESCHEDULE_BOOKED_TICKET_DETAILS objrechedule = new TBL_RESCHEDULE_BOOKED_TICKET_DETAILS()
                    {
                        MEM_ID= FlightBookingInfo.MEM_ID,
                        DIST_ID= FlightBookingInfo.DIST_ID,
                        WLP_ID = FlightBookingInfo.WLP_ID,
                        CORELATION_ID = FlightBookingInfo.CORELATION_ID,
                        PNR = FlightBookingInfo.PNR,
                        REF_NO = FlightBookingInfo.REF_NO,
                        TRACK_NO = FlightBookingInfo.TRACK_NO,
                        TRIP_MODE = FlightBookingInfo.TRIP_MODE,
                        TICKET_NO = FlightBookingInfo.TICKET_NO,
                        TICKET_TYPE = FlightBookingInfo.TICKET_TYPE,
                        IS_DOMESTIC = FlightBookingInfo.IS_DOMESTIC,
                        AIRLINE_CODE = FlightBookingInfo.AIRLINE_CODE,
                        FLIGHT_NO = FlightBookingInfo.FLIGHT_NO,
                        FROM_AIRPORT = FlightBookingInfo.FROM_AIRPORT,
                        TO_AIRPORT = FlightBookingInfo.TO_AIRPORT,
                        BOOKING_DATE = FlightBookingInfo.BOOKING_DATE,
                        DEPT_DATE = FlightBookingInfo.DEPT_DATE,
                        DEPT_TIME = FlightBookingInfo.DEPT_TIME,
                        ARRIVE_DATE = FlightBookingInfo.ARRIVE_DATE,
                        ARRIVE_TIME = FlightBookingInfo.ARRIVE_TIME,
                        NO_OF_ADULT = FlightBookingInfo.NO_OF_ADULT,
                        NO_OF_CHILD = FlightBookingInfo.NO_OF_CHILD,
                        NO_OF_INFANT = FlightBookingInfo.NO_OF_INFANT,
                        TOTAL_FLIGHT_BASE_FARE = FlightBookingInfo.TOTAL_FLIGHT_BASE_FARE,
                        TOTAL_FLIGHT_TAX = FlightBookingInfo.TOTAL_FLIGHT_TAX,
                        TOTAL_PASSANGER_TAX = FlightBookingInfo.TOTAL_PASSANGER_TAX,
                        TOTAL_FLIGHT_SERVICE_CHARGES = FlightBookingInfo.TOTAL_FLIGHT_SERVICE_CHARGES,
                        TOTAL_FLIGHT_ADDITIONAL_CHARGE = FlightBookingInfo.TOTAL_FLIGHT_ADDITIONAL_CHARGE,
                        TOTAL_FLIGHT_CUTE_FEE = FlightBookingInfo.TOTAL_FLIGHT_CUTE_FEE,
                        TOTAL_FLIGHT_MEAL_FEE = FlightBookingInfo.TOTAL_FLIGHT_MEAL_FEE,
                        TOTAL_AIRPORT_FEE = FlightBookingInfo.TOTAL_AIRPORT_FEE,
                        TOTAL_FLIGHT_CONVENIENCE_FEE = FlightBookingInfo.TOTAL_FLIGHT_CONVENIENCE_FEE,
                        TOTAL_FLIGHT_AMT = FlightBookingInfo.TOTAL_FLIGHT_AMT,
                        TOTAL_COMMISSION_AMT = FlightBookingInfo.TOTAL_COMMISSION_AMT,
                        TOTAL_TDS_AMT = FlightBookingInfo.TOTAL_TDS_AMT,
                        TOTAL_SERVICES_TAX = FlightBookingInfo.TOTAL_SERVICES_TAX,
                        TOTAL_BAGGAGE_ALLOWES = FlightBookingInfo.TOTAL_BAGGAGE_ALLOWES,
                        STATUS = FlightBookingInfo.STATUS,
                        IS_CANCELLATION = FlightBookingInfo.IS_CANCELLATION,
                        FLIGHT_CANCELLATION_ID = FlightBookingInfo.FLIGHT_CANCELLATION_ID,
                        CANCELLATION_DATE = FlightBookingInfo.CANCELLATION_DATE,
                        IS_HOLD = FlightBookingInfo.IS_HOLD,
                        BOOKING_HOLD_ID = FlightBookingInfo.BOOKING_HOLD_ID,
                        HOLD_DATE = FlightBookingInfo.HOLD_DATE,
                        API_RESPONSE = FlightBookingInfo.API_RESPONSE,
                        USER_MARKUP = FlightBookingInfo.USER_MARKUP,
                        ADMIN_MARKUP = FlightBookingInfo.ADMIN_MARKUP,
                        COMM_SLAP = FlightBookingInfo.COMM_SLAP,
                        ADMIN_GST = FlightBookingInfo.ADMIN_GST,
                        ADMIN_sGST = FlightBookingInfo.ADMIN_sGST,
                        ADMIN_cGST = FlightBookingInfo.ADMIN_cGST,
                        ADMIN_iGST = FlightBookingInfo.ADMIN_iGST,
                        FLIGHT_BOOKING_DATE = FlightBookingInfo.FLIGHT_BOOKING_DATE,
                        BOOKING_STATUS = FlightBookingInfo.BOOKING_STATUS,
                        MAIN_CLASS = FlightBookingInfo.MAIN_CLASS,
                        BOOKING_CLASS = FlightBookingInfo.BOOKING_CLASS,
                        USER_MARKUP_GST = FlightBookingInfo.USER_MARKUP_GST,
                        USER_MARKUP_cGST = FlightBookingInfo.USER_MARKUP_cGST,
                        USER_MARKUP_sGST = FlightBookingInfo.USER_MARKUP_sGST,
                        USER_MARKUP_iGST = FlightBookingInfo.USER_MARKUP_iGST,
                        OP_MODE = FlightBookingInfo.OP_MODE,
                        HOLD_CHARGE = FlightBookingInfo.HOLD_CHARGE,
                        HOLD_CGST = FlightBookingInfo.HOLD_CGST,
                        HOLD_SGST = FlightBookingInfo.HOLD_SGST,
                        HOLD_IGST = FlightBookingInfo.HOLD_IGST,
                        API_CANCELLATION_RESPONSE = FlightBookingInfo.API_CANCELLATION_RESPONSE,
                        PASSAGER_SEGMENT = FlightBookingInfo.PASSAGER_SEGMENT,
                        API_REQUEST = FlightBookingInfo.API_REQUEST,
                        Cancellation_status = FlightBookingInfo.Cancellation_status,
                        STOPAGE = FlightBookingInfo.STOPAGE,
                        COMPANY_GST_NO = FlightBookingInfo.COMPANY_GST_NO,
                        COMPANY_NAME = FlightBookingInfo.COMPANY_NAME,
                        COMPANY_EMAIL_ID = FlightBookingInfo.COMPANY_EMAIL_ID,
                        COMPANY_MOBILE = FlightBookingInfo.COMPANY_MOBILE,
                        COMPANY_GST_ADDRESS = FlightBookingInfo.COMPANY_GST_ADDRESS,

                        PUBLISH_FARE = FlightBookingInfo.PUBLISH_FARE,
                        NET_FARE = FlightBookingInfo.NET_FARE,
                        NET_TOTAL_FARE = FlightBookingInfo.NET_TOTAL_FARE,
                        CANCELLATION_REMARK = FlightBookingInfo.CANCELLATION_REMARK,
                        RESCHEDULE_FARE = FlightBookingInfo.RESCHEDULE_FARE,
                        RESCHEDULE_REMARK = RescheduleingText,
                        RESCHEDULE_DATE =DateTime.Now,
                        RESCHEDULE_AMOUNT = 0,
                        RESCHEDULE_STATUS = "RECHEDULE UNDER PROCESS"                        
                    };
                    db.TBL_RESCHEDULE_BOOKED_TICKET_DETAILS.Add(objrechedule);
                    FlightBookingInfo.RESCHEDULE_REMARK = "RECHEDULE UNDER PROCESS";
                    db.Entry(FlightBookingInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Flight rechedule process is under process",JsonRequestBehavior.AllowGet);
                }

                
            }
            catch (Exception ex)
            {
                return Json("");
                throw;
            }
            

        }
        [HttpPost]
        public JsonResult BookedTicketDeatils(string BookingId="")
        {
            try
            {
                long BookingTabId = 0;
                long.TryParse(BookingId, out BookingTabId);
                var GetFlightInformation = _db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x=>x.SLN==BookingTabId && x.MEM_ID==CurrentMerchant.MEM_ID);
                long MEM_ID = GetFlightInformation.MEM_ID; 
                string Ref_NO= GetFlightInformation.REF_NO;
                string PNR_NO = GetFlightInformation.PNR;
                var GetFlightPassenger = _db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.Where(x => x.REF_NO == Ref_NO && x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
                return Json(new { FlightDetails = GetFlightInformation, PassangerList = GetFlightPassenger},JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { FlightDetails = "1", PassangerList = "1" }, JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }
    }   
}
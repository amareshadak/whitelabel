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

        public ActionResult FlightDetails(string TrackNo="", string PsgnAdult="",string PsgnChildren="",string PsgnInfant="",string TripMode="")
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
    }
}
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WHITELABEL.Data;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.DTO.FlightApi;


namespace WHITELABEL.Web.Helper
{
    public static class MultiLinkAirAPI
    {
        //private static string token = "BOMAK";
        //public static string root = "http://mwapiv2.multilinkworld.com/";

        //public static string ApiIntegrationNew = "BOMAK";
        //public static string AgentCode = "MLA0007925";
        //public static string AgentPass = "abcd@123";


        //private static string token = "AirticketOnlineWebSite";
        //public static string root = "http://stagingv2.multilinkworld.com/";

        //private static string InterfaceCode = "1";
        //public static string ApiIntegrationNew = "AirticketOnlineWebSite";
        //public static string AgentCode = "MLD0000001";
        //public static string AgentPass = "TEST1_";
        private static string token = System.Configuration.ConfigurationManager.AppSettings["token"];
        private static string root = System.Configuration.ConfigurationManager.AppSettings["rootURL"];
        private static string InterfaceCode = System.Configuration.ConfigurationManager.AppSettings["InterfaceCode"];
        private static string ApiIntegrationNew = System.Configuration.ConfigurationManager.AppSettings["ApiIntegrationNew"];
        private static string AgentCode = System.Configuration.ConfigurationManager.AppSettings["AgentCode"];
        private static string AgentPass = System.Configuration.ConfigurationManager.AppSettings["AgentPass"];
        private static string MerchantCode = System.Configuration.ConfigurationManager.AppSettings["MerchantCode"];
        private static string MerchantKey = System.Configuration.ConfigurationManager.AppSettings["MerchantKey"];
        private static string SaltKey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];

        public static dynamic SerachFlight(FlightSearch objsearch)
        {
            try
            {
                string url = root + "API/FlightAvailibility";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode =InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic GetFlightAvailibilityRequest_Val = new JObject();
                GetFlightAvailibilityRequest_Val.NoofAdult = objsearch.AdultCount;
                //GetFlightAvailibilityRequest_Val.NoofChild = objsearch.ChildCount;
                //GetFlightAvailibilityRequest_Val.NoofInfant = objsearch.InfantCount;
                GetFlightAvailibilityRequest_Val.NoofChild = objsearch.ChildCount;
                GetFlightAvailibilityRequest_Val.NoofInfant = objsearch.InfantCount;
                GetFlightAvailibilityRequest_Val.FromAirportCode = objsearch.Origin;
                GetFlightAvailibilityRequest_Val.ToAirportCode = objsearch.Destination;
                GetFlightAvailibilityRequest_Val.DepartureDate = objsearch.PreferredDepartureTime;
                //GetFlightAvailibilityRequest_Val.ReturnDate = objsearch.PreferredArrivalTime;
                GetFlightAvailibilityRequest_Val.ReturnDate = objsearch.PreferredArrivalTime;
                GetFlightAvailibilityRequest_Val.TripType = objsearch.JourneyType;
                GetFlightAvailibilityRequest_Val.FlightClass = objsearch.FlightCabinClass;
                if (objsearch.JourneyType == "2")
                {
                    GetFlightAvailibilityRequest_Val.SpecialFare = "1";
                }
                else
                {
                    GetFlightAvailibilityRequest_Val.SpecialFare = "0";
                }

                GetFlightAvailibilityRequest_Val.PreferedAirlines = objsearch.PreferredAirlines;
                GetFlightAvailibilityRequest_Val.AirlineType = "A";
                RequestXml_Val.GetFlightAvailibilityRequest = new JObject(GetFlightAvailibilityRequest_Val);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
                string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(SearchparamValue, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
        }
        public static dynamic VerifyFlightDetails(string TrackNo, string TripMode)
        {
            string url = root + "API/VerifyFlightDetail";
            dynamic RequestXml_Val = new JObject();
            dynamic RequestXmlObj = new JObject();
            dynamic Authenticate_Val = new JObject();
            Authenticate_Val.InterfaceCode =InterfaceCode;
            Authenticate_Val.InterfaceAuthKey = token;
            Authenticate_Val.AgentCode = AgentCode;
            Authenticate_Val.Password = AgentPass;
            RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
            dynamic VerifyFlightDetailRequest_Val = new JObject();
            VerifyFlightDetailRequest_Val.TrackNo = TrackNo;
            RequestXml_Val.VerifyFlightDetailRequest = new JObject(VerifyFlightDetailRequest_Val);
            RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
            string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
            var res = GetResponse(SearchparamValue, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }
        #region Flight Price Calender
        public static dynamic FareCalender(string DepartureDate, string ArrivalDate, int TripType, string FromSourceCode, string ToDestinationCode)
        {
            string url = root + "API/FareCalender";
            dynamic RequestXml_Val = new JObject();
            dynamic RequestXmlObj = new JObject();
            dynamic Authenticate_Val = new JObject();
            Authenticate_Val.InterfaceCode = InterfaceCode;
            Authenticate_Val.InterfaceAuthKey = token;
            Authenticate_Val.AgentCode = AgentCode;
            Authenticate_Val.Password = AgentPass;
            RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
            dynamic FlightPriceDetailRequest_Val = new JObject();

            FlightPriceDetailRequest_Val.DepartureDate = DepartureDate;
            FlightPriceDetailRequest_Val.ArrivalDate = ArrivalDate;
            FlightPriceDetailRequest_Val.TripType = TripType;
            FlightPriceDetailRequest_Val.FromSourceCode = FromSourceCode;
            FlightPriceDetailRequest_Val.ToDestinationCode = ToDestinationCode;

            RequestXml_Val.GetFareCalendarRequest = new JObject(FlightPriceDetailRequest_Val);
            RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
            string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
            var res = GetResponse(SearchparamValue, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }
        #endregion

        public static dynamic GetAdditionalFlightDetails(string req)
        {
            string url = $"{root}API/AdditionalServices";
            AdditionalDetailsDTO AdditionalDetails = JsonConvert.DeserializeObject<AdditionalDetailsDTO>(req);
            AdditionalDetails.RequestXml.Authenticate.InterfaceCode =InterfaceCode;
            AdditionalDetails.RequestXml.Authenticate.InterfaceAuthKey = token;
            AdditionalDetails.RequestXml.Authenticate.AgentCode = AgentCode;
            AdditionalDetails.RequestXml.Authenticate.Password = AgentPass;

            string requestObject = JsonConvert.SerializeObject(AdditionalDetails);

            var res = GetResponse(requestObject, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }
        public static dynamic BookedFlightTicket(string req,string Mem_id)
        //public static dynamic BookedFlightTicket(string req)
        {           

            var ClientRequestID = Mem_id;
            string url = $"{root}API/BookTicket";
            FlightBookingDTO AdditionalDetails = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
            AdditionalDetails.RequestXml.Authenticate.InterfaceCode =InterfaceCode;
            AdditionalDetails.RequestXml.Authenticate.InterfaceAuthKey = token;
            AdditionalDetails.RequestXml.Authenticate.AgentCode = AgentCode;
            AdditionalDetails.RequestXml.Authenticate.Password = AgentPass;
            AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = ClientRequestID.ToString();
            //AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = "";

            string requestObject = JsonConvert.SerializeObject(AdditionalDetails);

            var temp = JObject.Parse(requestObject);
            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Value.ToString() == "")
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

            requestObject = temp.ToString();

           

            var res = GetResponse(requestObject, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }
        public static dynamic HoldingFlightTicket(string req, string Mem_id)
        //public static dynamic BookedFlightTicket(string req)
        {

            var ClientRequestID = Mem_id;
            string url = $"{root}API/BookTicket";
            FlightHoldingReqDTO Request = JsonConvert.DeserializeObject<FlightHoldingReqDTO>(req);
            Request.RequestXml.Authenticate.InterfaceCode =InterfaceCode;
            Request.RequestXml.Authenticate.InterfaceAuthKey = token;
            Request.RequestXml.Authenticate.AgentCode = AgentCode;
            Request.RequestXml.Authenticate.Password = AgentPass;
            Request.RequestXml.BookTicketRequest.ClientRequestID = ClientRequestID.ToString();
            //AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = "";

            string requestObject = JsonConvert.SerializeObject(Request);

            var temp = JObject.Parse(requestObject);
            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Value.ToString() == "")
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

            requestObject = temp.ToString();

            var res = GetResponse(requestObject, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }
        public static dynamic GetTicketInformation(string refId, string GDSPNR, string AirlinePNR, string ClientRequestID, string BookingFromDate, string BookingToDate)
        {
            try
            {
                string url = root + "/API/PNRDetailsVer2";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode =InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic GetRefNoValue = new JObject();
                //GetRefNoValue.RefNo = refId;
                GetRefNoValue.RefNo = "";
                GetRefNoValue.GDSPNR = "";
                //GetRefNoValue.AirlinePNR = AirlinePNR;
                GetRefNoValue.AirlinePNR = "";
                //GetRefNoValue.ClientRequestID = "";
                GetRefNoValue.ClientRequestID = AirlinePNR;
                GetRefNoValue.BookingFromDate = "";
                GetRefNoValue.BookingToDate = "";
                RequestXml_Val.PNRDetailsRequest = new JObject(GetRefNoValue);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
                string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(SearchparamValue, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {//get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }

        }
        public static dynamic printBookTicket(string refId,string GDSPNR,string AirlinePNR,string ClientRequestID,string BookingFromDate,string BookingToDate)
        {
            try
            {
                string url = root + "/API/PNRDetailsVer2";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode =InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic GetRefNoValue = new JObject();
                GetRefNoValue.RefNo = refId;
                //GetRefNoValue.RefNo = "";
                GetRefNoValue.GDSPNR = "";
                GetRefNoValue.AirlinePNR = AirlinePNR;
                //GetRefNoValue.AirlinePNR = "";
                GetRefNoValue.ClientRequestID = "";
                //GetRefNoValue.ClientRequestID = AirlinePNR;
                GetRefNoValue.BookingFromDate = "";
                GetRefNoValue.BookingToDate = "";
                RequestXml_Val.PNRDetailsRequest = new JObject(GetRefNoValue);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
                string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(SearchparamValue, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {//get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
            
        }
        public static dynamic HoldTicketConfrim(string refId, string  ClientRequestID)
        {
            try
            {
                string url = root + "/API/HoldBookingConfirm";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode =InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic HoldBookingConfirmRequest_val = new JObject();
                HoldBookingConfirmRequest_val.RefNo = refId;
                HoldBookingConfirmRequest_val.ClientRequestID = ClientRequestID;
                HoldBookingConfirmRequest_val.MerchantCode = MerchantCode;
                HoldBookingConfirmRequest_val.MerchantKey = MerchantKey;
                HoldBookingConfirmRequest_val.SaltKey = SaltKey;
                //HoldBookingConfirmRequest_val.MerchantCode = "PAY9zJhspxq7m";
                //HoldBookingConfirmRequest_val.MerchantKey = "eSpbcYMkPoZYFPcE8FnZ";
                //HoldBookingConfirmRequest_val.SaltKey = "WHJIIcNjVXaZj03TnDme";                
                RequestXml_Val.HoldBookingConfirmRequest = new JObject(HoldBookingConfirmRequest_val);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
                string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(SearchparamValue, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
            
        }
        #region Get Flight Fare Rules
        public static dynamic GetFareDetails(string TrackNo)
        {
            try
            {
                var db = new DBContext();
                string url = root + "/API/FareRule";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode = InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);

                dynamic GetFareRuleRequest = new JObject();
                GetFareRuleRequest.TrackNo = TrackNo;
                RequestXml_Val.GetFareRuleRequest = new JObject(GetFareRuleRequest);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);

                string requestObject = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(requestObject, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }

        }
        #endregion
        public static dynamic FlightCancellation(string[] pnsgDetails, string RefNo, string Cancellation_Type)
        {
            try
            {
                var db = new DBContext();
                string url = root + "API/TicketCancel";
                //var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': 'MOS0000001','Password': 'KGBW5P'},'TicketCancelRequest': {'RefNo': '6923576722','Passengers': {'Passenger': [{'PaxSeqNo': '1','FirstName': 'Vikas','LastName': 'Gore'},{'PaxSeqNo': '2','FirstName': 'Avneet','LastName': 'Gore'}]},'Segments': {'Segment': [{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '1'},{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '2'}]},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";

                var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '"+ InterfaceCode +"','InterfaceAuthKey': '"+ token + "','AgentCode': '" + AgentCode + "','Password': '" + AgentPass + "'},'TicketCancelRequest': {'RefNo': '" + RefNo + "','Passengers': {'Passenger': []},'Segments': {'Segment': []},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': ''}}}";
                //var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': '" + AgentCode + "','Password': '" + AgentPass + "'},'TicketCancelRequest': {'RefNo': '" + RefNo + "','Passengers': {'Passenger': []},'Segments': {'Segment': []},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";
                BookedTicketCancellationDTO ObjcancelTicket = JsonConvert.DeserializeObject<BookedTicketCancellationDTO>(test);
                var pangCount = pnsgDetails.Length;
                var FlightBookingDetails = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == RefNo);
                string BookingRes = Convert.ToString(FlightBookingDetails.API_RESPONSE);
                string BookRequest = Convert.ToString(FlightBookingDetails.API_REQUEST);
                FlightBookingDTO ReuestBook = JsonConvert.DeserializeObject<FlightBookingDTO>(BookRequest);
                
                
                BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(BookingRes);
                var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
                var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
                var TicketDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();

                string pnnn = string.Empty;
                string Sseg = string.Empty;

                var pnsg = ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger;
                long Pnsgid_Val = 0;

                if (Cancellation_Type == "Full")
                {
                    if (FlightBookingDetails.TICKET_TYPE == "R")
                    {
                        var segment_List = ReuestBook.RequestXml.BookTicketRequest.Segments.Segment;
                        var SegInfodept = segment_List.Where(x => x.TrackNo.Contains("O")).ToList();
                        int segDeptCnt = SegInfodept.Count();
                        var SegInforeturn = segment_List.Where(x => x.TrackNo.Contains("R")).ToList();
                        int segretCnt = SegInforeturn.Count();
                        foreach (string PnsgId in pnsgDetails)
                        {
                            long.TryParse(PnsgId, out Pnsgid_Val);
                            var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val && x.TRIP_TYPE == "O");
                            if (getpnsglist != null)
                            {
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
                                {
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
                                    FirstName = getpnsglist.FIRST_NAME,
                                    LastName = getpnsglist.LAST_NAME,
                                });
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                {
                                    SegmentSeqNo = SegInfodept[0].SegmentSeqNo,
                                    AirlineCode = SegInfodept[0].AirlineCode,
                                    FlightNo = SegInfodept[0].FlightNo,
                                    FromAirportCode = SegInfodept[0].FromAirportCode,
                                    ToAirportCode = SegInfodept[segDeptCnt - 1].ToAirportCode,
                                    FlightClass = SegInfodept[0].MainClass,
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                });
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                {
                                    SegmentSeqNo = SegInforeturn[0].SegmentSeqNo,
                                    AirlineCode = SegInforeturn[0].AirlineCode,
                                    FlightNo = SegInforeturn[0].FlightNo,
                                    FromAirportCode = SegInforeturn[0].FromAirportCode,
                                    ToAirportCode = SegInforeturn[segretCnt - 1].ToAirportCode,
                                    FlightClass = SegInforeturn[0].MainClass,
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                });

                            }
                        }
                    }
                    else
                    {
                        var segment_List = ReuestBook.RequestXml.BookTicketRequest.Segments.Segment;
                        var SegInfodept = segment_List.Where(x => x.TrackNo.Contains("O")).ToList();
                        int segDeptCnt = SegInfodept.Count();
                        //var SegInforeturn = segment_List.Where(x => x.TrackNo.Contains("R")).ToList();
                        //int segretCnt = SegInforeturn.Count();
                        foreach (string PnsgId in pnsgDetails)
                        {
                            long.TryParse(PnsgId, out Pnsgid_Val);
                            var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val && x.TRIP_TYPE == "O");
                            if (getpnsglist != null)
                            {
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
                                {
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
                                    FirstName = getpnsglist.FIRST_NAME,
                                    LastName = getpnsglist.LAST_NAME,
                                });
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                {
                                    SegmentSeqNo = SegInfodept[0].SegmentSeqNo,
                                    AirlineCode = SegInfodept[0].AirlineCode,
                                    FlightNo = SegInfodept[0].FlightNo,
                                    FromAirportCode = SegInfodept[0].FromAirportCode,
                                    ToAirportCode = SegInfodept[segDeptCnt - 1].ToAirportCode,
                                    FlightClass = SegInfodept[0].MainClass,
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                });
                                //ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                //{
                                //    SegmentSeqNo = SegInforeturn[0].SegmentSeqNo,
                                //    AirlineCode = SegInforeturn[0].AirlineCode,
                                //    FlightNo = SegInforeturn[0].FlightNo,
                                //    FromAirportCode = SegInforeturn[0].FromAirportCode,
                                //    ToAirportCode = SegInforeturn[segretCnt - 1].ToAirportCode,
                                //    FlightClass = SegInforeturn[0].MainClass,
                                //    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                //});

                            }
                        }
                    }

                    
                    ObjcancelTicket.RequestXml.TicketCancelRequest.CancellationType = "Fully Cancelled";
                }
                else
                {
                    if (TicketDetails[0].TicketType == "O")
                    {
                        foreach (string PnsgId in pnsgDetails)
                        {
                            long.TryParse(PnsgId, out Pnsgid_Val);
                            var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
                            ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
                            {
                                PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
                                FirstName = getpnsglist.FIRST_NAME,
                                LastName = getpnsglist.LAST_NAME,
                            });
                            ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                            {
                                SegmentSeqNo = FareDetails[0].SeqNo,
                                AirlineCode = FareDetails[0].AirlineCode,
                                FlightNo = FareDetails[0].FlightNo,
                                FromAirportCode = FareDetails[0].FromAirportCode,
                                ToAirportCode = FareDetails[FareDetailsCount - 1].ToAirportCode,
                                FlightClass = FareDetails[0].MainClass,
                                PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                            });
                        }
                        ObjcancelTicket.RequestXml.TicketCancelRequest.CancellationType = "Partially Cancelled";
                    }
                    else if (TicketDetails[0].TicketType == "R")
                    {
                        var segment_List = ReuestBook.RequestXml.BookTicketRequest.Segments.Segment;
                        var SegInfodept = segment_List.Where(x => x.TrackNo.Contains("O")).ToList();
                        int segDeptCnt = SegInfodept.Count();
                        var SegInforeturn = segment_List.Where(x => x.TrackNo.Contains("R")).ToList();
                        int segretCnt = SegInforeturn.Count();
                        foreach (string PnsgId in pnsgDetails)
                        {
                            long.TryParse(PnsgId, out Pnsgid_Val);
                            var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
                            ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
                            {
                                PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
                                FirstName = getpnsglist.FIRST_NAME,
                                LastName = getpnsglist.LAST_NAME,
                            });
                            if (getpnsglist.TRIP_TYPE == "R")
                            {
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                {
                                    SegmentSeqNo = SegInforeturn[0].SegmentSeqNo,
                                    AirlineCode = SegInforeturn[0].AirlineCode,
                                    FlightNo = SegInforeturn[0].FlightNo,
                                    FromAirportCode = SegInforeturn[0].FromAirportCode,
                                    ToAirportCode = SegInforeturn[segretCnt - 1].ToAirportCode,
                                    FlightClass = SegInforeturn[0].MainClass,
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                });
                            }
                            else
                            {
                                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                                {
                                    SegmentSeqNo = SegInfodept[0].SegmentSeqNo,
                                    AirlineCode = SegInfodept[0].AirlineCode,
                                    FlightNo = SegInfodept[0].FlightNo,
                                    FromAirportCode = SegInfodept[0].FromAirportCode,
                                    ToAirportCode = SegInfodept[segDeptCnt - 1].ToAirportCode,
                                    FlightClass = SegInfodept[0].MainClass,
                                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                                });
                            }

                            //ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                            //{
                            //    SegmentSeqNo = FareDetails[0].SeqNo,
                            //    AirlineCode = FareDetails[0].AirlineCode,
                            //    FlightNo = FareDetails[0].FlightNo,
                            //    FromAirportCode = FareDetails[0].FromAirportCode,
                            //    ToAirportCode = FareDetails[FareDetailsCount - 1].ToAirportCode,
                            //    FlightClass = FareDetails[0].MainClass,
                            //    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                            //});
                        }
                        ObjcancelTicket.RequestXml.TicketCancelRequest.CancellationType = "Partially Cancelled";
                    }
                    else
                    {
                        foreach (string PnsgId in pnsgDetails)
                        {
                            long.TryParse(PnsgId, out Pnsgid_Val);
                            var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
                            ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
                            {
                                PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
                                FirstName = getpnsglist.FIRST_NAME,
                                LastName = getpnsglist.LAST_NAME,
                            });
                            ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
                            {
                                SegmentSeqNo = FareDetails[0].SeqNo,
                                AirlineCode = FareDetails[0].AirlineCode,
                                FlightNo = FareDetails[0].FlightNo,
                                FromAirportCode = FareDetails[0].FromAirportCode,
                                ToAirportCode = FareDetails[FareDetailsCount - 1].ToAirportCode,
                                FlightClass = FareDetails[0].MainClass,
                                PaxSeqNo = getpnsglist.PNSG_SEQ_NO
                            });
                        }
                        ObjcancelTicket.RequestXml.TicketCancelRequest.CancellationType = "Partially Cancelled";
                    }
                }
                

                string requestObject = JsonConvert.SerializeObject(ObjcancelTicket);
                var res = GetResponse(requestObject, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }

        }
        //public static dynamic FlightCancellation(string[] pnsgDetails, string RefNo,string Cancellation_Type)
        //{
        //    try
        //    {
        //        var db = new DBContext();
        //        string url = root + "API/TicketCancel";
        //        //var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': 'MOS0000001','Password': 'KGBW5P'},'TicketCancelRequest': {'RefNo': '6923576722','Passengers': {'Passenger': [{'PaxSeqNo': '1','FirstName': 'Vikas','LastName': 'Gore'},{'PaxSeqNo': '2','FirstName': 'Avneet','LastName': 'Gore'}]},'Segments': {'Segment': [{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '1'},{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '2'}]},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";


        //        var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': '" + AgentCode + "','Password': '"+ AgentPass + "'},'TicketCancelRequest': {'RefNo': '"+ RefNo + "','Passengers': {'Passenger': []},'Segments': {'Segment': []},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";                
        //        BookedTicketCancellationDTO ObjcancelTicket = JsonConvert.DeserializeObject<BookedTicketCancellationDTO>(test);                
        //        var pangCount = pnsgDetails.Length;
        //        var FlightBookingDetails = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == RefNo);
        //        string BookingRes = Convert.ToString(FlightBookingDetails.API_RESPONSE);
        //        string BookRequest = Convert.ToString(FlightBookingDetails.API_REQUEST);
        //        FlightBookingDTO AdditionalDetails = JsonConvert.DeserializeObject<FlightBookingDTO>(BookRequest);
        //        BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(BookingRes);
        //        var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
        //        var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
        //        var TicketDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].TicketDetails.ToList();

        //        string pnnn = string.Empty;
        //        string Sseg = string.Empty;

        //        var pnsg = ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger;
        //        long Pnsgid_Val = 0;
        //        if (TicketDetails[0].TicketType == "O")
        //        {
        //            foreach (string PnsgId in pnsgDetails)
        //            {
        //                long.TryParse(PnsgId, out Pnsgid_Val);
        //                var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
        //                ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
        //                {
        //                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
        //                    FirstName = getpnsglist.FIRST_NAME,
        //                    LastName = getpnsglist.LAST_NAME,
        //                });
        //                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
        //                {
        //                    SegmentSeqNo = FareDetails[0].SeqNo,
        //                    AirlineCode = FareDetails[0].AirlineCode,
        //                    FlightNo = FareDetails[0].FlightNo,
        //                    FromAirportCode = FareDetails[0].FromAirportCode,
        //                    ToAirportCode = FareDetails[FareDetailsCount - 1].ToAirportCode,
        //                    FlightClass = FareDetails[0].MainClass,
        //                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
        //                });
        //            }
        //        }
        //        else if (TicketDetails[0].TicketType == "R")
        //        {
        //        }
        //        else
        //        {
        //            foreach (string PnsgId in pnsgDetails)
        //            {
        //                long.TryParse(PnsgId, out Pnsgid_Val);
        //                var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
        //                ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger
        //                {
        //                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO,
        //                    FirstName = getpnsglist.FIRST_NAME,
        //                    LastName = getpnsglist.LAST_NAME,
        //                });
        //                ObjcancelTicket.RequestXml.TicketCancelRequest.Segments.Segment.Add(new CancelSegment
        //                {
        //                    SegmentSeqNo = FareDetails[0].SeqNo,
        //                    AirlineCode = FareDetails[0].AirlineCode,
        //                    FlightNo = FareDetails[0].FlightNo,
        //                    FromAirportCode = FareDetails[0].FromAirportCode,
        //                    ToAirportCode = FareDetails[FareDetailsCount - 1].ToAirportCode,
        //                    FlightClass = FareDetails[0].MainClass,
        //                    PaxSeqNo = getpnsglist.PNSG_SEQ_NO
        //                });
        //            }
        //        }           

        //        string requestObject = JsonConvert.SerializeObject(ObjcancelTicket);                
        //        var res = GetResponse(requestObject, url);
        //        if (res != null)
        //        {
        //            return res;
        //        }
        //        else
        //        {
        //            return res;
        //        }
        //    }
        //    catch (WebException webEx)
        //    {
        //        //get the response stream
        //        WebResponse response = webEx.Response;
        //        Stream stream = response.GetResponseStream();
        //        String responseMessage = new StreamReader(stream).ReadToEnd();
        //        return responseMessage;
        //    }

        //}
        public static dynamic FlightCancellationHistory(string CancelReqNo, string RefNo,string AirPNR)
        {
            try
            {
                var db = new DBContext();
                string url = root + "API/CancelTicketHistory";
                //var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': 'MLD0000001','Password': 'TEST1_'},'CancelTicketHistoryRequest': {'RefNo': '"+ RefNo + "','CancelReqNo': '"+ CancelReqNo + "','AirlinePNR': '','BookingFromDate': '','BookingToDate': '','DepartureFromDate': '','DepartureToDate': '','ArrivalFromDate': '','ArrivalToDate': '','CancelFromDate': '','CancelToDate': ''}}}";


                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode =InterfaceCode;
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic GetRefNoValue = new JObject();                
                GetRefNoValue.RefNo = RefNo;
                GetRefNoValue.CancelReqNo = CancelReqNo;                
                GetRefNoValue.AirlinePNR = "";                
                GetRefNoValue.BookingFromDate = "";
                GetRefNoValue.BookingToDate = "";
                GetRefNoValue.DepartureFromDate = "";
                GetRefNoValue.DepartureToDate = "";
                GetRefNoValue.ArrivalFromDate = "";
                GetRefNoValue.ArrivalToDate = "";
                GetRefNoValue.CancelFromDate = "";
                GetRefNoValue.CancelToDate = "";
                RequestXml_Val.CancelTicketHistoryRequest = new JObject(GetRefNoValue);
                RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
                string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
                var res = GetResponse(SearchparamValue, url);
                if (res != null)
                {
                    return res;
                }
                else
                {
                    return res;
                }

                //string requestObject = JsonConvert.SerializeObject(test);
                //var res = GetResponse(requestObject, url);
                //if (res != null)
                //{
                //    return res;
                //}
                //else
                //{
                //    return res;
                //}
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }

        }


        public static dynamic GetMultilinkBalance()
        {
            string url = root + "API/GetBalance";
            dynamic RequestXml_Val = new JObject();
            dynamic RequestXmlObj = new JObject();
            dynamic Authenticate_Val = new JObject();
            Authenticate_Val.InterfaceCode = InterfaceCode;
            Authenticate_Val.InterfaceAuthKey = token;
            Authenticate_Val.AgentCode = AgentCode;
            Authenticate_Val.Password = AgentPass;

            RequestXml_Val.Authenticate = new JObject(Authenticate_Val);

            dynamic GetBalanceRequest_Val = new JObject();
            GetBalanceRequest_Val.MerchantCode = MerchantCode;
            GetBalanceRequest_Val.Key = MerchantKey;
            GetBalanceRequest_Val.SaltKey = SaltKey;

            RequestXml_Val.GetBalanceRequest = new JObject(GetBalanceRequest_Val);

            RequestXmlObj.RequestXml = new JObject(RequestXml_Val);
            string SearchparamValue = JsonConvert.SerializeObject(RequestXmlObj);
            var res = GetResponse(SearchparamValue, url);
            if (res != null)
            {
                return res;
            }
            else
            {
                return res;
            }
        }

        public static dynamic GetResponse(string requestData, string url)
        {
            string responseXML = string.Empty;
            dynamic responsesult = null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    //throw exception
                }
                using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                {
                    responsesult = JsonConvert.DeserializeObject(readStream.ReadToEnd());
                    //responseXML = JsonConvert.DeserializeXmlNode(readStream.ReadToEnd()).InnerXml;
                }
                return responsesult;
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
        }
    }
}
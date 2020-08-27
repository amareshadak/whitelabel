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
        private static string token = "AirticketOnlineWebSite";
        public static string root = "http://stagingv2.multilinkworld.com/";

        public static string ApiIntegrationNew = "ApiIntegrationNew";
        public static string AgentCode = "MLD0000001";
        public static string AgentPass = "TEST1_";

        public static dynamic SerachFlight(FlightSearch objsearch)
        {
            string url = root + "API/FlightAvailibility";
            dynamic RequestXml_Val = new JObject();
            dynamic RequestXmlObj = new JObject();
            dynamic Authenticate_Val = new JObject();
            Authenticate_Val.InterfaceCode = "1";
            Authenticate_Val.InterfaceAuthKey = objsearch.TokenId;
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

        public static dynamic VerifyFlightDetails(string TrackNo, string TripMode)
        {
            string url = root + "API/VerifyFlightDetail";
            dynamic RequestXml_Val = new JObject();
            dynamic RequestXmlObj = new JObject();
            dynamic Authenticate_Val = new JObject();
            Authenticate_Val.InterfaceCode = "1";
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



        public static dynamic GetAdditionalFlightDetails(string req)
        {
            string url = $"{root}API/AdditionalServices";
            AdditionalDetailsDTO AdditionalDetails = JsonConvert.DeserializeObject<AdditionalDetailsDTO>(req);
            AdditionalDetails.RequestXml.Authenticate.InterfaceCode = "1";
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
            AdditionalDetails.RequestXml.Authenticate.InterfaceCode = "1";
            AdditionalDetails.RequestXml.Authenticate.InterfaceAuthKey = token;
            AdditionalDetails.RequestXml.Authenticate.AgentCode = AgentCode;
            AdditionalDetails.RequestXml.Authenticate.Password = AgentPass;
            AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = ClientRequestID.ToString();
            //AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = "";

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

        public static dynamic HoldingFlightTicket(string req, string Mem_id)
        //public static dynamic BookedFlightTicket(string req)
        {

            var ClientRequestID = Mem_id;
            string url = $"{root}API/BookTicket";
            FlightHoldingReqDTO Request = JsonConvert.DeserializeObject<FlightHoldingReqDTO>(req);
            Request.RequestXml.Authenticate.InterfaceCode = "1";
            Request.RequestXml.Authenticate.InterfaceAuthKey = token;
            Request.RequestXml.Authenticate.AgentCode = AgentCode;
            Request.RequestXml.Authenticate.Password = AgentPass;
            Request.RequestXml.BookTicketRequest.ClientRequestID = ClientRequestID.ToString();
            //AdditionalDetails.RequestXml.BookTicketRequest.ClientRequestID = "";

            string requestObject = JsonConvert.SerializeObject(Request);

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

        public static dynamic printBookTicket(string refId,string GDSPNR,string AirlinePNR,string ClientRequestID,string BookingFromDate,string BookingToDate)
        {
            try
            {
                string url = root + "/API/PNRDetailsVer2";
                dynamic RequestXml_Val = new JObject();
                dynamic RequestXmlObj = new JObject();
                dynamic Authenticate_Val = new JObject();
                Authenticate_Val.InterfaceCode = "1";
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic GetRefNoValue = new JObject();
                GetRefNoValue.RefNo = refId;
                GetRefNoValue.GDSPNR = "";
                GetRefNoValue.AirlinePNR = AirlinePNR;
                GetRefNoValue.ClientRequestID = "";
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
                Authenticate_Val.InterfaceCode = "1";
                Authenticate_Val.InterfaceAuthKey = token;
                Authenticate_Val.AgentCode = AgentCode;
                Authenticate_Val.Password = AgentPass;
                RequestXml_Val.Authenticate = new JObject(Authenticate_Val);
                dynamic HoldBookingConfirmRequest_val = new JObject();
                HoldBookingConfirmRequest_val.RefNo = refId;
                HoldBookingConfirmRequest_val.ClientRequestID = ClientRequestID;
                HoldBookingConfirmRequest_val.MerchantCode = "PAY9zJhspxq7m";
                HoldBookingConfirmRequest_val.MerchantKey = "eSpbcYMkPoZYFPcE8FnZ";
                HoldBookingConfirmRequest_val.SaltKey = "WHJIIcNjVXaZj03TnDme";                
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


        //public static dynamic FlightCancellation(string refId, string fName, string lName,string AirlineCode, string FlightNo,
        //    string FromAirportCode, string ToAirportCode, string FlightClass)
        //{
        //    try
        //    {
        //        string url = root + "/API/TicketCancel";
        //        var test = @"{'RequestXml':{'Authenticate':{'InterfaceCode':'1','InterfaceAuthKey':'"+ token + "','AgentCode':'"+ AgentCode + "','Password':'"+ AgentPass + "'},'TicketCancelRequest':{'RefNo':'"+ refId + "','Passengers':{'Passenger':{'PaxSeqNo':'1','FirstName':'"+ fName + "','LastName':'"+ lName + "'}},'Segments':{'Segment':{'SegmentSeqNo':'1','AirlineCode':'"+ AirlineCode + "','FlightNo':'"+ FlightNo + "','FromAirportCode':'"+ FromAirportCode + "','ToAirportCode':'"+ ToAirportCode + "','FlightClass':'"+ FlightClass + "','PaxSeqNo':'1'}},'IsNoShow':'False','CancelRemark':'Test','CancellationType':'Full Cancel'}}}";
        //        //TicketCancellationDTO objticketcancel = new TicketCancellationDTO();
        //        TicketCancellationDTO objticketcancel = JsonConvert.DeserializeObject<TicketCancellationDTO>(test);
        //        //objticketcancel.RequestXml.Authenticate.InterfaceCode = "1";
        //        //objticketcancel.RequestXml.Authenticate.InterfaceAuthKey = token;
        //        //objticketcancel.RequestXml.Authenticate.AgentCode = AgentCode;
        //        //objticketcancel.RequestXml.Authenticate.Password = AgentPass;
        //        //objticketcancel.RequestXml.TicketCancelRequest.RefNo = refId;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Passengers.Passenger.PaxSeqNo = "1";
        //        //objticketcancel.RequestXml.TicketCancelRequest.Passengers.Passenger.FirstName = fName;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Passengers.Passenger.LastName = lName;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.SegmentSeqNo = "1";
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.AirlineCode = AirlineCode;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.FlightNo = FlightNo;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.FromAirportCode = FromAirportCode;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.ToAirportCode = ToAirportCode;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.FlightClass = FlightClass;
        //        //objticketcancel.RequestXml.TicketCancelRequest.Segments.Segment.PaxSeqNo = "1";

        //        string requestObject = JsonConvert.SerializeObject(objticketcancel);

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

        public static dynamic FlightCancellation(string[] pnsgDetails, string RefNo)
        {
            try
            {
                var db = new DBContext();
                string url = root + "API/TicketCancel";
                //var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': 'MOS0000001','Password': 'KGBW5P'},'TicketCancelRequest': {'RefNo': '6923576722','Passengers': {'Passenger': [{'PaxSeqNo': '1','FirstName': 'Vikas','LastName': 'Gore'},{'PaxSeqNo': '2','FirstName': 'Avneet','LastName': 'Gore'}]},'Segments': {'Segment': [{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '1'},{'SegmentSeqNo': '1','AirlineCode': 'SG','FlightNo': '158','FromAirportCode': 'BOM','ToAirportCode': 'DEL','FlightClass': 'Y','PaxSeqNo': '2'}]},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";


                var test = @"{'RequestXml': {'Authenticate': {'InterfaceCode': '1','InterfaceAuthKey': 'AirticketOnlineWebSite','AgentCode': '" + AgentCode + "','Password': '"+ AgentPass + "'},'TicketCancelRequest': {'RefNo': '"+ RefNo + "','Passengers': {'Passenger': []},'Segments': {'Segment': []},'IsNoShow': 'False','CancelRemark': 'Malay Team Test','CancellationType': 'Full Cancel'}}}";                
                BookedTicketCancellationDTO ObjcancelTicket = JsonConvert.DeserializeObject<BookedTicketCancellationDTO>(test);                
                var pangCount = pnsgDetails.Length;
                var FlightBookingDetails = db.TBL_FLIGHT_BOOKING_DETAILS.FirstOrDefault(x => x.REF_NO == RefNo);
                string BookingRes = Convert.ToString(FlightBookingDetails.API_RESPONSE);
                BookTicketResponsesDTO BookingResponse = JsonConvert.DeserializeObject<BookTicketResponsesDTO>(BookingRes);
                var FareDetails = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.ToList();
                var FareDetailsCount = BookingResponse.BookTicketResponses.BookTicketResponse[0].FlightFareDetails.Count;
                string pnnn = string.Empty;
                string Sseg = string.Empty;
                
                var pnsg = ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger;
                long Pnsgid_Val = 0;                
                foreach (string PnsgId in pnsgDetails)
                {
                    long.TryParse(PnsgId, out Pnsgid_Val);
                    var getpnsglist = db.TBL_FLIGHT_BOOKING_PASSENGER_LIST.FirstOrDefault(x => x.SLN == Pnsgid_Val);
                    ObjcancelTicket.RequestXml.TicketCancelRequest.Passengers.Passenger.Add(new CancelPassenger {
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
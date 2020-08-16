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

        public static dynamic BookedFlightTicket(string req)
        {
            string url = $"{root}API/BookTicket";
            FlightBookingDTO AdditionalDetails = JsonConvert.DeserializeObject<FlightBookingDTO>(req);
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
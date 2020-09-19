using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
    public class FlightHoldingReqDTO
    {
        public HoldingFlightBookingRequestXml RequestXml { get; set; }
    }


    public class HoldingFlightBookingAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }

    public class HoldingPassenger
    {
        public string PaxSeqNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DateOfBirth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PassportNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PassportExpDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PassportIssuingCountry { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NationalityCountry { get; set; }
        //public string PassportNo { get; set; }
        //public string PassportExpDate { get; set; }
        //public string PassportIssuingCountry { get; set; }
        //public string NationalityCountry { get; set; }
    }

    public class HoldingPassengers
    {
        public List<HoldingPassenger> Passenger { get; set; }
    }

    public class HoldingSegment
    {
        public string TrackNo { get; set; }
        public string SegmentSeqNo { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string DepDate { get; set; }
        public string DepTime { get; set; }
        public string ArrDate { get; set; }
        public string ArrTime { get; set; }
        public string FlightClass { get; set; }
        public string MainClass { get; set; }
    }

    public class HoldingSegments
    {
        public List<HoldingSegment> Segment { get; set; }
    }

    public class HoldingAdditionalService
    {
        public string PaxSeqNo { get; set; }
        public string FromStationCode { get; set; }
        public string ToStationCode { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceFlightKey { get; set; }
    }

    public class HoldingAdditionalServices
    {
        public List<HoldingAdditionalService> AdditionalService { get; set; }
    }

    public class HoldingBookTicketRequest
    {
        public string TrackNo { get; set; }
        public string MobileNo { get; set; }
        public string AltMobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ClientRequestID { get; set; }
        public HoldingPassengers Passengers { get; set; }
        public HoldingSegments Segments { get; set; }
        public HoldingAdditionalServices AdditionalServices { get; set; }
        public string TotalAmount { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantKey { get; set; }
        public string SaltKey { get; set; }
        public string IsTicketing { get; set; }
        public string HoldAllowed { get; set; }
        public string HoldCharge { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string GSTNo { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string GSTEmailID { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string GSTCompanyName { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string GSTMobileNo { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string GSTAddress { get; set; }

    }

    public class HoldingFlightBookingRequestXml
    {
        public HoldingFlightBookingAuthenticate Authenticate { get; set; }
        public HoldingBookTicketRequest BookTicketRequest { get; set; }
    }
}
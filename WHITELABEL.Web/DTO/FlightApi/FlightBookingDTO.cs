using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
    public class FlightBookingDTO
    {
        public FlightBookingRequestXml RequestXml { get; set; }
    }

    public class FlightBookingAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }

    public class Passenger
    {
        public string PaxSeqNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        public string DateOfBirth { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpDate { get; set; }
        public string PassportIssuingCountry { get; set; }
        public string NationalityCountry { get; set; }
    }

    public class Passengers
    {
        public List<Passenger> Passenger { get; set; }
    }

    public class Segment
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

    public class Segments
    {
        public List<Segment> Segment { get; set; }
    }

    public class AdditionalService
    {
        public string PaxSeqNo { get; set; }
        public string FromStationCode { get; set; }
        public string ToStationCode { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceFlightKey { get; set; }
    }

    public class AdditionalServices
    {
        public List<AdditionalService> AdditionalService { get; set; }
    }

    public class BookTicketRequest
    {
        public string TrackNo { get; set; }
        public string MobileNo { get; set; }
        public string AltMobileNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ClientRequestID { get; set; }
        public Passengers Passengers { get; set; }
        public Segments Segments { get; set; }
        public AdditionalServices AdditionalServices { get; set; }
        public string TotalAmount { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantKey { get; set; }
        public string SaltKey { get; set; }
        public string IsTicketing { get; set; }
    }

    public class FlightBookingRequestXml
    {
        public FlightBookingAuthenticate Authenticate { get; set; }
        public BookTicketRequest BookTicketRequest { get; set; }
    }

}
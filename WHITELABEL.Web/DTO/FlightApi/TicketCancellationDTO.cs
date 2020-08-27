using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{   
    public class TicketCancellationDTO
    {
        public TicketCancelRequestXml RequestXml { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TicketCancelAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }
    public class TicketCancelPassenger
    {
        public string PaxSeqNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TicketCancelPassengers
    {
        public TicketCancelPassenger Passenger { get; set; }
    }

    public class TicketCancelSegment
    {
        public string SegmentSeqNo { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string FlightClass { get; set; }
        public string PaxSeqNo { get; set; }
    }

    public class TicketCancelSegments
    {
        public TicketCancelSegment Segment { get; set; }
    }

    public class TicketCancelTicketCancelRequest
    {
        public string RefNo { get; set; }
        public TicketCancelPassengers Passengers { get; set; }
        public TicketCancelSegments Segments { get; set; }
        public string IsNoShow { get; set; }
        public string CancelRemark { get; set; }
        public string CancellationType { get; set; }
    }

    public class TicketCancelRequestXml
    {
        public TicketCancelAuthenticate Authenticate { get; set; }
        public TicketCancelTicketCancelRequest TicketCancelRequest { get; set; }
    }

   


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
 
    public class BookedTicketCancellationDTO
    {
        public CancelRequestXml RequestXml { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CancelAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }

    public class CancelPassenger
    {
        public string PaxSeqNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CancelPassengers
    {
        public List<CancelPassenger> Passenger { get; set; }
    }

    public class CancelSegment
    {
        public string SegmentSeqNo { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string FlightClass { get; set; }
        public string PaxSeqNo { get; set; }
    }

    public class CancelSegments
    {
        public List<CancelSegment> Segment { get; set; }
    }

    public class CancelTicketCancelRequest
    {
        public string RefNo { get; set; }
        public CancelPassengers Passengers { get; set; }
        public CancelSegments Segments { get; set; }
        public string IsNoShow { get; set; }
        public string CancelRemark { get; set; }
        public string CancellationType { get; set; }
    }

    public class CancelRequestXml
    {
        public CancelAuthenticate Authenticate { get; set; }
        public CancelTicketCancelRequest TicketCancelRequest { get; set; }
    }

   



    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    //public class CancelPassenger
    //{
    //    public string PaxSeqNo { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //}

    //public class CancelPassengers
    //{
    //    public List<CancelPassenger> Passenger { get; set; }
    //}

    //public class CancelSegment
    //{
    //    public string SegmentSeqNo { get; set; }
    //    public string AirlineCode { get; set; }
    //    public string FlightNo { get; set; }
    //    public string FromAirportCode { get; set; }
    //    public string ToAirportCode { get; set; }
    //    public string FlightClass { get; set; }
    //    public string PaxSeqNo { get; set; }
    //}

    //public class CancelSegments
    //{
    //    public List<CancelSegment> Segment { get; set; }
    //}

    //public class CancelTicketCancelRequest
    //{
    //    public string RefNo { get; set; }
    //    public CancelPassengers Passengers { get; set; }
    //    public CancelSegments Segments { get; set; }
    //    public string IsNoShow { get; set; }
    //    public string CancelRemark { get; set; }
    //}






}
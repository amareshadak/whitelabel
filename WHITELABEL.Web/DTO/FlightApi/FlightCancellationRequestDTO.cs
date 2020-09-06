using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
    public class FlightCancellationRequestDTO
    {
        public CancellationRequestXml RequestXml { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CancellationAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }

    public class CancellationCancelTicketHistoryRequest
    {
        public string RefNo { get; set; }
        public string CancelReqNo { get; set; }
        public string AirlinePNR { get; set; }
        public string BookingFromDate { get; set; }
        public string BookingToDate { get; set; }
        public string DepartureFromDate { get; set; }
        public string DepartureToDate { get; set; }
        public string ArrivalFromDate { get; set; }
        public string ArrivalToDate { get; set; }
        public string CancelFromDate { get; set; }
        public string CancelToDate { get; set; }
    }

    public class CancellationRequestXml
    {
        public CancellationAuthenticate Authenticate { get; set; }
        public CancellationCancelTicketHistoryRequest CancelTicketHistoryRequest { get; set; }
    }
}
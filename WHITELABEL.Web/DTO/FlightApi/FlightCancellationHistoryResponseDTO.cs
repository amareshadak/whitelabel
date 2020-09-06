using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{    
    public class FlightCancellationHistoryResponseDTO
    {
        public CancelTicketHistoryResponses CancelTicketHistoryResponses { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CancellationHistoryTicketDetail
    {
        public List<string> CancelReqNo { get; set; }
        public string CancelRequestDateTime { get; set; }
        public string TotalCancellationCharges { get; set; }
        public string RefundAmount { get; set; }
        public string TotalAmount { get; set; }
        public string TicketStatus { get; set; }
        public string CancelStatus { get; set; }
        public string CompanyCancelRemark { get; set; }
    }

    public class CancellationHistoryPassengerDetail
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        public string Gender { get; set; }
        public object BirthDate { get; set; }
        public object CreditedDate { get; set; }
    }

    public class CancellationHistoryFlightDetail
    {
        public string SeqNo { get; set; }
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string MainClass { get; set; }
        public string BookingClass { get; set; }
        public string FromAirportName { get; set; }
        public string ToAirportName { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureTime { get; set; }
        public string APIPNRNumber { get; set; }
        public string AirlinePNRNumber { get; set; }
        public List<CancellationHistoryPassengerDetail> PassengerDetails { get; set; }
    }

    public class CancellationHistoryCancelTicketHistoryResponse
    {
        public List<CancellationHistoryTicketDetail> TicketDetails { get; set; }
        public List<CancellationHistoryFlightDetail> FlightDetails { get; set; }
    }

    public class CancelTicketHistoryResponses
    {
        public List<CancellationHistoryCancelTicketHistoryResponse> CancelTicketHistoryResponse { get; set; }
    }

    


}
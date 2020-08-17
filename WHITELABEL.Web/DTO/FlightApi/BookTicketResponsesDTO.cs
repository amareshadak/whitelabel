using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{

    public class BookTicketResponsesDTO
    {
        public BookTicketResponses BookTicketResponses { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TicketDetail
    {
        public string RefNo { get; set; }
        public string BookingDateTime { get; set; }
        public string TicketType { get; set; }
        public string IsDomestic { get; set; }
        public string Adult { get; set; }
        public string Child { get; set; }
        public string Infant { get; set; }
        public string Status { get; set; }
        public string TotalBaseFare { get; set; }
        public string TotalTax { get; set; }
        public string TotalPassengerTax { get; set; }
        public string TotalPassengerServiceFee { get; set; }
        public string TotalPassengerTranFare { get; set; }
        public string TotalFuelFee { get; set; }
        public string TotalAirPortFee { get; set; }
        public string TotalAdditionalCharges { get; set; }
        public string TotalAirportDevelopmentFee { get; set; }
        public string TotalCuteFee { get; set; }
        public string TotalConvenienceFee { get; set; }
        public string TotalSkyCafeMealFee { get; set; }
        public string TotalTicketServiceCharge { get; set; }
        public string TotalAmount { get; set; }
        public string TotalTicketCommissionAmount { get; set; }
        public string TotalTDSAmount { get; set; }
        public string TotalServiceTax { get; set; }
        public string ServiceTaxOnComm { get; set; }
        public string PGTranCharges { get; set; }
        public string TotalSuppCGST { get; set; }
        public string TotalSuppSGST { get; set; }
        public string TotalSuppIGST { get; set; }
    }

    public class FlightFareDetail
    {
        public string SeqNo { get; set; }
        public string IsTripMainRecord { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string AirlineName { get; set; }
        public string MainClass { get; set; }
        public string BookingClass { get; set; }
        public string ValidatingCarrier { get; set; }
        public string OperatedBy { get; set; }
        public string FromAirportName { get; set; }
        public string ToAirportName { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string AdultFareBasis { get; set; }
        public string DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalDate { get; set; }
        public string ArriveTime { get; set; }
        public string APIPNRNumber { get; set; }
        public string AirlinePNRNumber { get; set; }
        public string TotalFlightBaseFare { get; set; }
        public string TotalFlightTax { get; set; }
        public string TotalFlightPassengerTax { get; set; }
        public string TotalFlightPassengerServiceFee { get; set; }
        public string TotalFlightPassengerTranFare { get; set; }
        public string TotalFlightFuelFee { get; set; }
        public string TotalFlightAirPortFee { get; set; }
        public string TotalFlightAdditionalCharges { get; set; }
        public string TotalFlightAirportDevelopmentFee { get; set; }
        public string TotalFlightCuteFee { get; set; }
        public string TotalFlightConvenienceFee { get; set; }
        public string TotalFlightSkyCafeMealFee { get; set; }
        public string TotalServiceCharge { get; set; }
        public string TotalFlightAmount { get; set; }
        public string TotalFlightCommissionAmount { get; set; }
        public string TDSAmount { get; set; }
        public string ServiceTax { get; set; }
        public string ServiceTaxOnComm { get; set; }
        public string TotalFlightSuppSGST { get; set; }
        public string TotalFlightSuppIGST { get; set; }
        public string TotalFlightSuppCGST { get; set; }
        public string AdultCheckedIn { get; set; }
        public string ChildCheckedIn { get; set; }
        public string InfantCheckedIn { get; set; }
        public string AdultCabin { get; set; }
        public string ChildCabin { get; set; }
        public string InfantCabin { get; set; }
    }

    public class Detail2
    {
        public string SeqNo { get; set; }
        public string FreqFlyerNo { get; set; }
        public string TicketNumber { get; set; }
        public string IsPaxCancel { get; set; }
    }

    public class Details
    {
        public List<Detail2> Detail { get; set; }
    }

    public class PassengerDetail
    {
        public string SeqNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        public string Gender { get; set; }
        public object BirthDate { get; set; }
        public List<Details> Details { get; set; }
    }

    public class BookTicketResponse
    {
        public List<TicketDetail> TicketDetails { get; set; }
        public List<FlightFareDetail> FlightFareDetails { get; set; }
        public List<PassengerDetail> PassengerDetails { get; set; }
    }

    public class BookTicketResponses
    {
        public List<BookTicketResponse> BookTicketResponse { get; set; }
    }

   


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{

    //public class FlightSegmentDTO
    //{
    //    public List<ReturnFlightSegments> Segment { get; set; }
    //}
    public class FlightSegmentDTO
    {
        public List<ReturnFlightSegments> Segment { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ReturnFlightSegments
    {
        public string SrNo { get; set; }
        public string AirlineName { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public string FromAirportCode { get; set; }
        public string ToAirportCode { get; set; }
        public string FromAirportName { get; set; }
        public string ToAirportName { get; set; }
        public string DepDate { get; set; }
        public string DepTime { get; set; }
        public string ArrDate { get; set; }
        public string ArrTime { get; set; }
        public string FlightClass { get; set; }
        public string FlightTime { get; set; }
        public string TotalAmount { get; set; }
        public string TaxAmount { get; set; }
        public string Stops { get; set; }
        public string ValCarrier { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string MainClass { get; set; }
        public string FareBasis { get; set; }
        public string AgencyCharge { get; set; }
        public string FareType { get; set; }
        public string AvailSeats { get; set; }
        public object FlightRemarks { get; set; }
        public string TrackNo { get; set; }
        public string IsDOBMandatory { get; set; }
        public string IsPassportNoMandatory { get; set; }
        public string IsPassportExpDateMandatory { get; set; }
        public string IsNationalityMandatory { get; set; }
        public string IsPICountryMandatory { get; set; }
        public string HoldAllowed { get; set; }
        public string HoldCharges { get; set; }
        public string HoldDuration { get; set; }
        public string IsGSTMandatory { get; set; }
        //public string hashKey { get; set; }
}





    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    //public class ReturnFlightSegments
    //{
    //    public string SrNo { get; set; }
    //    public string AirlineName { get; set; }
    //    public string AirlineCode { get; set; }
    //    public string FlightNo { get; set; }
    //    public string FromAirportCode { get; set; }
    //    public string ToAirportCode { get; set; }
    //    public string FromAirportName { get; set; }
    //    public string ToAirportName { get; set; }
    //    public string DepDate { get; set; }
    //    public string DepTime { get; set; }
    //    public string ArrDate { get; set; }
    //    public string ArrTime { get; set; }
    //    public string FlightClass { get; set; }
    //    public string FlightTime { get; set; }
    //    public string TotalAmount { get; set; }
    //    public string TaxAmount { get; set; }
    //    public string Stops { get; set; }
    //    public string ValCarrier { get; set; }
    //    public string FromTerminal { get; set; }
    //    public string ToTerminal { get; set; }
    //    public string MainClass { get; set; }
    //    public string FareBasis { get; set; }
    //    public string AgencyCharge { get; set; }
    //    public string FareType { get; set; }
    //    public string AvailSeats { get; set; }
    //    public string FlightRemarks { get; set; }
    //    public string TrackNo { get; set; }
    //    public string IsDOBMandatory { get; set; }
    //    public string IsPassportNoMandatory { get; set; }
    //    public string IsPassportExpDateMandatory { get; set; }
    //    public string IsNationalityMandatory { get; set; }
    //    public string IsPICountryMandatory { get; set; }
    //    public string HoldAllowed { get; set; }
    //    public string HoldCharges { get; set; }
    //    public string HoldDuration { get; set; }
    //}

    


}
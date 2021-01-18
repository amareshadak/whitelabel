using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
    public class ErrorOnBookingTimeDTO
    {
        public BookTicketResponseError BookTicketResponse { get; set; }
    }
    public class Error
    {
        [JsonProperty("@errorDescription")]
        public string ErrorDescription { get; set; }
    }

    public class BookTicketResponseError
    {
        public Error Error { get; set; }
    }
    
}
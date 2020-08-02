using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{

    // AdditionalDetailsDTO myDeserializedClass = JsonConvert.DeserializeObject<AdditionalDetailsDTO>(myJsonResponse); 
    public class AdditionalDetailsAuthenticate
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }

    public class GetAdditionalServicesRequest
    {
        public string TrackNo { get; set; }
    }

    public class AdditionalDetailsRequestXml
    {
        public AdditionalDetailsAuthenticate Authenticate { get; set; }
        public GetAdditionalServicesRequest GetAdditionalServicesRequest { get; set; }
    }

    public class AdditionalDetailsDTO
    {
        public AdditionalDetailsRequestXml RequestXml { get; set; }
    }


}
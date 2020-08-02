using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.DTO.FlightApi
{
    public class AuthenticateDTO
    {
        public string InterfaceCode { get; set; }
        public string InterfaceAuthKey { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
    }
}
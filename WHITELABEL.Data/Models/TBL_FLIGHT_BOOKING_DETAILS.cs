﻿namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("FLIGHT_BOOKING_DETAILS")]
    public class TBL_FLIGHT_BOOKING_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public long? DIST_ID { get; set; }
        public long? WLP_ID { get; set; }
        public string CORELATION_ID { get; set; }
        public string PNR { get; set; }
        public string REF_NO { get; set; }
        public string TRACK_NO { get; set; }
        public string TRIP_MODE { get; set; }
        public string TICKET_NO { get; set; }
        public string TICKET_TYPE { get; set; }
        public bool? IS_DOMESTIC { get; set; }
        public string AIRLINE_CODE { get; set; }
        public string FLIGHT_NO { get; set; }
        public string FROM_AIRPORT { get; set; }
        public string TO_AIRPORT { get; set; }
        public DateTime? BOOKING_DATE { get; set; }
        public string DEPT_DATE { get; set; }
        public string DEPT_TIME { get; set; }
        public string ARRIVE_DATE { get; set; }
        public string ARRIVE_TIME { get; set; }
        public int? NO_OF_ADULT { get; set; }
        public int? NO_OF_CHILD { get; set; }
        public int? NO_OF_INFANT { get; set; }
        public decimal? TOTAL_FLIGHT_BASE_FARE { get; set; }
        public decimal? TOTAL_FLIGHT_TAX { get; set; }
        public decimal? TOTAL_PASSANGER_TAX { get; set; }
        public decimal? TOTAL_FLIGHT_SERVICE_CHARGES { get; set; }
        public decimal? TOTAL_FLIGHT_ADDITIONAL_CHARGE { get; set; }
        public decimal? TOTAL_FLIGHT_CUTE_FEE { get; set; }
        public decimal? TOTAL_FLIGHT_MEAL_FEE { get; set; }
        public decimal? TOTAL_AIRPORT_FEE { get; set; }
        public decimal? TOTAL_FLIGHT_CONVENIENCE_FEE { get; set; }
        public decimal? TOTAL_FLIGHT_AMT { get; set; }
        public decimal? TOTAL_COMMISSION_AMT { get; set; }
        public decimal? TOTAL_TDS_AMT { get; set; }
        public decimal? TOTAL_SERVICES_TAX { get; set; }
        public string TOTAL_BAGGAGE_ALLOWES { get; set; }
        public bool? STATUS { get; set; }
        public bool? IS_CANCELLATION { get; set; }
        public string FLIGHT_CANCELLATION_ID { get; set; }
        public DateTime? CANCELLATION_DATE { get; set; }
        public bool? IS_HOLD { get; set; }
        public string BOOKING_HOLD_ID { get; set; }
        public DateTime? HOLD_DATE { get; set; }
        public string API_RESPONSE { get; set; }
        public decimal? USER_MARKUP { get; set; }
        public decimal? ADMIN_MARKUP { get; set; }
        public int? COMM_SLAP { get; set; }
        public decimal? ADMIN_GST { get; set; }
        public decimal? ADMIN_sGST { get; set; }
        public decimal? ADMIN_cGST { get; set; }
        public decimal? ADMIN_iGST { get; set; }
        public string FLIGHT_BOOKING_DATE { get; set; }
    }
}
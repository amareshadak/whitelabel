namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("FLIGHT_BOOKING_PASSENGER_LIST")]
    public class TBL_FLIGHT_BOOKING_PASSENGER_LIST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public string REF_NO { get; set; }
        public string PNR { get; set; }
        public string TITLE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PASSENGER_TYPE { get; set; }

        public string GENDER { get; set; }
        public string BIRTH_DATE { get; set; }
        public string DETAILS { get; set; }
        public string PASSENGER_RESP { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string PNSG_SEQ_NO { get; set; }
        public string CORELATION_ID { get; set; }
        public string PASSENGER_STATUS { get; set; }
        public string CancelReqNo { get; set; }
        public string API_CANCELLATION_RESP { get; set; }
        public string CANCELLATIONHISTORY_RESP { get; set; }
        public string TRIP_TYPE { get; set; }
        public string FLIGHT_SEGMENT { get; set; }
        public string CANCEL_RESPONSE { get; set; }
        public DateTime? DOJ { get; set; }
        public string FROM_AIRPORT { get; set; }
        public string TO_AIRPORT { get; set; }
        public string CANCEL_STATUS { get; set; }

    }
}

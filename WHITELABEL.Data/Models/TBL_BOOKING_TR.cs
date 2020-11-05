namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("BOOKING_TR")]
    public class TBL_BOOKING_TR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string sln { get; set; }
        public string tran_id { get; set; }
        public string pnr_id { get; set; }
        public string opr_id { get; set; }
        public string pnr_class { get; set; }
        public decimal bk_amt { get; set; }
        public DateTime? tran_date { get; set; }
        public string user_id { get; set; }
        public string reason { get; set; }
    }
}

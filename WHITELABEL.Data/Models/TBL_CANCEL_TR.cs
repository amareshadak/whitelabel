namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("CANCEL_TR")]
    public class TBL_CANCEL_TR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sln { get; set; }
        public string id { get; set; }
        public string tran_id { get; set; }
        public string pnr_no { get; set; }
        public string opr_id { get; set; }
        public string pnr_class { get; set; }
        public decimal ref_amt { get; set; }
        public string wt_at_can { get; set; }
        public DateTime? tran_date { get; set; }
        public string tdr_can { get; set; }
        public string user_id { get; set; }
        public string cancelID { get; set; }
        public string reason { get; set; }
    }
}

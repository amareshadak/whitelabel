﻿namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CANCELLATION_FILE_PATH")]
    public class TBL_CANCELLATION_FILE_PATH
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
    }
}

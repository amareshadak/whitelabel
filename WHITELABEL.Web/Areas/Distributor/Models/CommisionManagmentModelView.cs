﻿namespace WHITELABEL.Web.Areas.Distributor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    public class CommisionManagmentModelView
    {
        public long SLN { get; set; }
        public string SLAB_NAME { get; set; }
        public string SLAB_DETAILS { get; set; }
        public long SLAB_TYPE { get; set; }
        public bool SLAB_STATUS { get; set; }
        public DateTime DATE_CREATED { get; set; }
        public long MEM_ID { get; set; }
        public string Slab_TypeName { get; set; }
    }
    public class CommissionListView
    {
        //public long ID { get; set; }
        //public string SERVICE_NAME { get; set; }
        //public string TYPE { get; set; }
        //public string SERVICE_KEY { get; set; }
        //public string COMMERTIALS { get; set; }
        //public string BILLING_MODEL { get; set; }
        //public string HSN_SAC { get; set; }
        //public string TDS { get; set; }
        //[NotMapped]
        //public decimal DMRFrom { get; set; }
        //[NotMapped]
        //public decimal DMRTo { get; set; }
        //[NotMapped]
        //public string COMM_TYPE { get; set; }
        //public string DMT_TYPE { get; set; }
        //public string CommissionPercentage { get; set; }
        //public string OldCommissionPercentage { get { return this.CommissionPercentage; } }
        public long ID { get; set; }
        public string SERVICE_NAME { get; set; }
        public string TYPE { get; set; }
        public string SERVICE_KEY { get; set; }
        public string COMMERTIALS { get; set; }
        public string BILLING_MODEL { get; set; }
        public string HSN_SAC { get; set; }
        public string TDS { get; set; }
        public decimal DMRFrom { get; set; }
        public decimal DMRTo { get; set; }
        public string COMM_TYPE { get; set; }
        public string DMT_TYPE { get; set; }
        public string CommissionType { get; set; }
        public string CommissionPercentage { get; set; }
        public string SuperDisPercentage { get; set; }
        public string DistributorComm_Type { get; set; }
        public string DistriCommissionPer { get; set; }
        public string MerchantComm_Type { get; set; }
        public string RetailerCommissionPer { get; set; }
        public string OldCommissionPercentage { get { return this.CommissionPercentage; } }
    }
    public class CommissoinManagmentmodel
    {
        //public long SLN { get; set; }
        //public string SLAB_NAME { get; set; }
        //public string SLAB_DETAILS { get; set; }
        //public long SLAB_TYPE { get; set; }
        //public string Slab_TypeName { get; set; }
        //public bool SLAB_STATUS { get; set; }
        //[NotMapped]
        //public DateTime DATE_CREATED { get; set; }
        //[NotMapped]
        //public long MEM_ID { get; set; }

        //public List<CommissionListView> OperatorDetails { get; set; }
        //public List<CommissionListView> ServiceInformationDMR { get; set; }
        public long SLN { get; set; }
        public string SLAB_NAME { get; set; }
        public string SLAB_DETAILS { get; set; }
        public long SLAB_TYPE { get; set; }
        public string Slab_TypeName { get; set; }
        public bool SLAB_STATUS { get; set; }
        public string SLAB_TDS { get; set; }
        public DateTime DATE_CREATED { get; set; }
        public long MEM_ID { get; set; }
        public string ASSIGNED_SLAB { get; set; }

        //public long ASSIGNED_SLAB { get; set; }
        public List<CommissionListView> OperatorDetails { get; set; }
        public List<CommissionListView> ServiceInformationDMR { get; set; }

    }
}
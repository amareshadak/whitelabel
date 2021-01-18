namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using WHITELABEL.Data;
    using WHITELABEL.Data.Models;
    using WHITELABEL.Web.Models;
    using WHITELABEL.Web.Helper;
    using System.Data.Entity.Core;
    using WHITELABEL.Web.Areas.Merchant.Models;
    using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
    using static WHITELABEL.Web.Helper.InstantPayApi;
    using NonFactors.Mvc.Grid;
    using OfficeOpenXml;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using log4net;
    public class MerchantRequisitionReportViewModel
    {
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetMerchantAllRequisitionReport(string Mem_id, string status, string DateFrom , string Date_To)
        {
            var db = new DBContext();
            if (status != "" && DateFrom == "" && Date_To == "")
            {
                long MemberId = long.Parse(Mem_id.ToString());
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.STATUS == status
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                //APPROVAL_DATE = x.APPROVAL_DATE,
                                                SLN = x.SLN,
                                                PAYMENT_MODE = x.PAYMENT_METHOD,
                                                APPROVAL_DATE = (status=="Pending"?null:x.APPROVAL_DATE)
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD = z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status == "" && DateFrom != "" && Date_To != "")
            {
                long MemberId = long.Parse(Mem_id.ToString());                
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                APPROVAL_DATE = x.APPROVAL_DATE,
                                                SLN = x.SLN,
                                                PAYMENT_MODE = x.PAYMENT_METHOD
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD = z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status != "Pending" && DateFrom != "" && Date_To != "")
            {
                long MemberId = long.Parse(Mem_id.ToString());
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                APPROVAL_DATE = x.APPROVAL_DATE,
                                                SLN = x.SLN,
                                                PAYMENT_MODE = x.PAYMENT_METHOD
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD = z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status != "Approve" && DateFrom != "" && Date_To != "")
            {
                long MemberId = long.Parse(Mem_id.ToString());
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                APPROVAL_DATE = (status == "Pending" ? null : x.APPROVAL_DATE),
                                                SLN = x.SLN,
                                                PAYMENT_MODE = x.PAYMENT_METHOD
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD = z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status != "Decline" && DateFrom != "" && Date_To != "")
            {
                long MemberId = long.Parse(Mem_id.ToString());
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                APPROVAL_DATE = (status == "Pending" ? null : x.APPROVAL_DATE),
                                                SLN = x.SLN,
                                                PAYMENT_MODE = x.PAYMENT_METHOD
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD = z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }            
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                long MemberId = long.Parse(Mem_id.ToString());
                var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                            where x.FROM_MEMBER == MemberId && x.REQUEST_DATE == Todaydate
                                            select new
                                            {
                                                Touser = "Merchant",
                                                transid = x.TransactionID,
                                                FromUser = y.UName,
                                                REQUEST_DATE = x.REQUEST_DATE,
                                                REQUEST_TIME = x.REQUEST_TIME,
                                                AMOUNT = x.AMOUNT,
                                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                                STATUS = x.STATUS,
                                                APPROVED_BY = x.APPROVED_BY,
                                                APPROVAL_DATE = (status == "Pending" ? null : x.APPROVAL_DATE),
                                                SLN = x.SLN,
                                                PAYMENT_MODE=x.PAYMENT_METHOD
                                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                            {
                                                Serial_No = index + 1,
                                                ToUser = z.Touser,
                                                TransactionID = z.transid,
                                                FromUser = z.FromUser,
                                                AMOUNT = z.AMOUNT,
                                                REQUEST_DATE = z.REQUEST_DATE,
                                                REQUEST_TIME = z.REQUEST_TIME,
                                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                                STATUS = z.STATUS,
                                                APPROVED_BY = z.APPROVED_BY,
                                                APPROVAL_DATE = z.APPROVAL_DATE,
                                                SLN = z.SLN,
                                                PAYMENT_METHOD=z.PAYMENT_MODE
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }
    }
}
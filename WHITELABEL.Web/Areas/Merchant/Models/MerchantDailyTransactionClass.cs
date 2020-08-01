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
    public class MerchantDailyTransactionClass
    {
        //public static List<TBL_ACCOUNTS> GetTransactionReport(string search,long MemberId)
        public static List<TBL_ACCOUNTS> GetTransactionReport(string search, long MemberId,string DateFrom,string DateTo)
        {
            var db = new DBContext();
            if (search == "Requisition")
            {
                if (DateFrom != "" && DateTo != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(DateTo.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.MEM_ID == MemberId && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_DATE>= Date_From_Val && x.TRANSACTION_DATE<= Date_To_Val
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderBy(m => m.SerialNo).ThenByDescending(a => a.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }
                else
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.MEM_ID == MemberId && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge"
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderByDescending(c => c.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }

                
            }
            else if (search == "Mobile Recharge" || search == "DMR")
            {
                if (DateFrom != "" && DateTo != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(DateTo.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where x.MEM_ID == MemberId && x.TRANSACTION_TYPE == search && x.TRANSACTION_DATE >= Date_From_Val && x.TRANSACTION_DATE <= Date_To_Val
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderByDescending(c => c.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }
                else
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where x.MEM_ID == MemberId && x.TRANSACTION_TYPE == search
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderBy(m => m.SerialNo).ThenByDescending(a => a.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }

                
            }
            else
            {
                if (DateFrom != "" && DateTo != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(DateTo.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.MEM_ID == MemberId && x.TRANSACTION_DATE >= Date_From_Val && x.TRANSACTION_DATE <= Date_To_Val
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderBy(m => m.SerialNo).ThenByDescending(a => a.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.MEM_ID == MemberId && x.TRANSACTION_DATE == Todaydate
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CR_Col = (z.DR_CR == "CR" ? z.Amount.ToString() : "0"),
                                                    DR_Col = (z.DR_CR == "DR" ? z.Amount.ToString() : "0"),
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).OrderBy(m => m.SerialNo).ThenByDescending(a => a.TRANSACTION_DATE).ToList();
                    return transactionlistvalue;
                }
                
            }
            return null;
        }
    }
}
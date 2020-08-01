﻿namespace WHITELABEL.Web.Areas.PowerAdmin.Models
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
    using WHITELABEL.Web.Areas.PowerAdmin.Models;
    using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
    using static WHITELABEL.Web.Helper.InstantPayApi;
    using NonFactors.Mvc.Grid;
    using OfficeOpenXml;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using log4net;
    public class PowerAdminRquisitionReportModel
    {
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAdminRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (status != "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME=z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME=z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllAdminRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (status != "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == UserID && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME=z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == UserID
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetSuperRequisitionReport(string WhiteLevel,  string MemberId, string status)
        {
            var db = new DBContext();

            if (status != "" && MemberId != "" && WhiteLevel!="")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "" && MemberId != "" && WhiteLevel != "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID 
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "" && MemberId == "" && WhiteLevel != "")
            {
                long UserID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (status != "" && MemberId == "" && WhiteLevel != "")
            {
                long UserID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString()) && x.STATUS==status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            

            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllSuperRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (MemberId != "" && status != "")
            {
                long UserID = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where SuperMemberIdTest.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status == "")
            {
                long UserID = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where SuperMemberIdTest.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorRequisitionReport(string WhiteLevel, string SuperId, string MemberId, string status)
        {
            var db = new DBContext();
            if (WhiteLevel != "" && SuperId != "" && MemberId != "" && status != "")
            {
                long Userid = long.Parse(MemberId);
                long WhiteLevelID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME=z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId == "" && MemberId == "" && status == "")
            {
                //long Userid = long.Parse(MemberId);
                long WhiteLevelID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId == "" && status == "")
            {
                //long Userid = long.Parse(MemberId);
                long WhiteLevelID = long.Parse(SuperId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString()) 
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId != "" && status == "")
            {
                long Userid = long.Parse(MemberId);
                long WhiteLevelID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER== Userid
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status == "")
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }


        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllDistributorRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();

            long UserID = long.Parse(MemberId);
            string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
            //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            if (MemberId != "" && status != "")
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status == "")
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }


            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllMerchantRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (status != "")
            {
                long mem_id = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                long mem_id = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();                
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where MerchantuserId.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetMerchantRequisitionReport(string WhiteLevel, string SuperId, string Distributor, string MemberId, string status)
        {
            var db = new DBContext();
            if (WhiteLevel != "" && SuperId != "" && MemberId != "" && status != "" && Distributor!="")
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId != "" && status == "" && Distributor != "")
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid 
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId == "" && status == "" && Distributor != "")
            {
                //long Userid = long.Parse(MemberId);
                
                long WhiteLevelID = long.Parse(Distributor);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();                
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString()) 
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId == "" && status != "" && Distributor != "")
            {
                //long Userid = long.Parse(MemberId);

                long WhiteLevelID = long.Parse(Distributor);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString()) && x.STATUS== status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId == "" && status == "" && Distributor == "")
            {
                //long Userid = long.Parse(MemberId);

                long WhiteLevelID = long.Parse(SuperId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where SuperMemberIdTest.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId != "" && MemberId == "" && status != "" && Distributor == "")
            {
                //long Userid = long.Parse(MemberId);

                long WhiteLevelID = long.Parse(SuperId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where SuperMemberIdTest.Contains(x.FROM_MEMBER.ToString()) && x.STATUS==status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId == "" && MemberId == "" && status == "" && Distributor == "")
            {
                //long Userid = long.Parse(MemberId);

                long WhiteLevelID = long.Parse(WhiteLevel);                
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (WhiteLevel != "" && SuperId == "" && MemberId == "" && status != "" && Distributor == "")
            {
                //long Userid = long.Parse(MemberId);

                long WhiteLevelID = long.Parse(WhiteLevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == WhiteLevelID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS==status
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (!string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid
                                       select new
                                       {
                                           Touser = "White Level",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }
    }
}
using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    public class MemberRDSFileProcessingController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["WhiteLevelUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Admin/MemberRDSFileProcessing
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> UploadRDSFile(HttpPostedFileBase file)
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                TempData["msgVal"] = null;
                var db = new DBContext();
                var db_Val = new DBContext();
                //using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                //{
                try
                {
                    if (file.ContentLength > 0)
                    {
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE BOOKING_FILE_PATH");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE BOOKING_TEMP");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE BOOKING_TR");

                        string File_NameValue = Path.GetFileName(file.FileName);
                        string file_path_Value = Server.MapPath("~/UPLOADS/BOOKING_UPLOAD") + "//" + file.FileName;
                        if (System.IO.File.Exists(file_path_Value))
                        {
                            System.IO.File.Delete(file_path_Value);
                        }
                        TBL_BOOKING_FILE_PATH ObjBookingFile = new TBL_BOOKING_FILE_PATH()
                        {
                            FILE_NAME = File_NameValue,
                            FILE_PATH = file_path_Value
                        };
                        db.TBL_BOOKING_FILE_PATH.Add(ObjBookingFile);
                        await db.SaveChangesAsync();
                        file.SaveAs(file_path_Value);
                        /* -- Import CSV Code Start -- */
                        long Id_Val = 0;
                        int count = 0;
                        string BookingfileName = Server.MapPath("~/UPLOADS/BOOKING_UPLOAD") + "//" + file.FileName;
                        DataTable dtCSV = new DataTable();
                        //string SourceConstr = @"Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + fileName + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'";
                        string SourceConstr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + System.IO.Path.GetDirectoryName(BookingfileName) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"";

                        OleDbConnection con = new OleDbConnection(SourceConstr);
                        string query = "SELECT * FROM [" + System.IO.Path.GetFileName(BookingfileName) + "]";
                        OleDbDataAdapter data = new OleDbDataAdapter(query, con);
                        data.SelectCommand.CommandTimeout = 2000;
                        data.Fill(dtCSV);
                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                        {
                            string sln = dtCSV.Rows[i][0].ToString();
                            long.TryParse(sln, out Id_Val);
                            string tran_id = dtCSV.Rows[i][1].ToString();
                            string pnr_no = dtCSV.Rows[i][2].ToString();
                            string ClientRefId = dtCSV.Rows[i][3].ToString();
                            string opr_id = "";
                            string bk_amt = dtCSV.Rows[i][4].ToString();
                            string bk_class = dtCSV.Rows[i][6].ToString();
                            DateTime tran_date = Convert.ToDateTime(dtCSV.Rows[i][7]);
                            string f_tran_date = tran_date.ToString("yyyy-MM-dd");
                            string user_id = dtCSV.Rows[i][8].ToString();

                            if (bk_class != "1A" || bk_class != "2A" || bk_class != "3A" || bk_class != "CC" || bk_class != "3E" || bk_class != "EC" || bk_class != "EA" || bk_class != "SL" || bk_class != "2S")
                            {
                                bk_class = string.Empty;
                            }

                            if (dtCSV.Rows[i][5].ToString() == "RDS" || dtCSV.Rows[i][5].ToString() == "VAPRON")
                            {
                                TBL_BOOKING_TEMP obkBookingTmp = new TBL_BOOKING_TEMP()
                                {

                                    TRN_ID = tran_id,
                                    PNR_NO = pnr_no,
                                    CLIENT_TXN_ID = ClientRefId,
                                    BOOKING_AMT = Convert.ToDecimal(bk_amt),
                                    PG_NAME = f_tran_date,
                                    PNR_CLASS = bk_class,
                                    TRN_DATE = Convert.ToDateTime(f_tran_date),
                                    USER_ID = user_id,
                                    STATUS = true,
                                    SYSTEM_UPDATE_TIME = DateTime.Now

                                };
                                db.TBL_BOOKING_TEMP.Add(obkBookingTmp);
                                await db.SaveChangesAsync();
                                //cmd2 = new SqlCommand("insert into booking_temp values('" + sln + "','" + tran_id + "','" + pnr_no + "','" + opr_id + "','" + Convert.ToDecimal(bk_amt) + "','" + f_tran_date + "','" + user_id + "','" + bk_class + "')", conLinq);
                                //cmd2.CommandTimeout = 2000;
                                //cmd2.ExecuteNonQuery();
                            }
                            //await db.SaveChangesAsync();
                            //ContextTransaction.Commit();
                            count++;
                        }
                        if (count == dtCSV.Rows.Count)
                        {
                            TempData["msgVal"] = "Booking File Import Successfully";
                            return RedirectToAction("GetAllRDSBookingList", "MemberRDSFileProcessing", new { area = "Admin" });
                        }
                        else
                        {
                            TempData["msgVal"] = "Booking File Not Import Successfully";
                            return View("Index");
                        }


                    }
                    else
                    {
                        TempData["msgVal"] = "Please Upload files!!";
                        return View("Index");
                    }
                }
                catch (Exception ex)
                {
                    //ContextTransaction.Rollback();
                    TempData["msgVal"] = "File upload failed!!";
                    //ViewBag.Message = "File upload failed!!";
                    //Session["msgVal"] =  "File upload failed!!";
                    return View("Index");
                }
                //}
            }
            else
            {

                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpGet]
        public ActionResult RDSCancellationFileUpload()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                TempData["msgVal"] = null;
                return View();
            }
            else
            {

                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> RDSCancellationFileUpload(HttpPostedFileBase file)
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                TempData["msgVal"] = null;
                var db = new DBContext();
                var db_Val = new DBContext();
                //using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                //{
                try
                {
                    if (file.ContentLength > 0)
                    {
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCELLATION_FILE_PATH");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCELLATION_TEMP");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCEL_TR");
                        string File_NameValue = Path.GetFileName(file.FileName);
                        string file_path_Value = Server.MapPath("~/UPLOADS/CANCEL_UPLOAD") + "//" + file.FileName;
                        if (System.IO.File.Exists(file_path_Value))
                        {
                            System.IO.File.Delete(file_path_Value);
                        }
                        TBL_CANCELLATION_FILE_PATH ObjcANCELFile = new TBL_CANCELLATION_FILE_PATH()
                        {
                            FILE_NAME = File_NameValue,
                            FILE_PATH = file_path_Value
                        };
                        db.TBL_CANCELLATION_FILE_PATH.Add(ObjcANCELFile);
                        await db.SaveChangesAsync();
                        file.SaveAs(file_path_Value);
                        /* -- Import CSV Code Start -- */
                        long Id_Val = 0;
                        int count = 0;
                        string BookingfileName = Server.MapPath("~/UPLOADS/CANCEL_UPLOAD") + "//" + file.FileName;
                        DataTable dtCSV = new DataTable();
                        //string SourceConstr = @"Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + fileName + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'";
                        string SourceConstr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + System.IO.Path.GetDirectoryName(BookingfileName) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"";

                        OleDbConnection con = new OleDbConnection(SourceConstr);
                        string query = "SELECT * FROM [" + System.IO.Path.GetFileName(BookingfileName) + "]";
                        OleDbDataAdapter data = new OleDbDataAdapter(query, con);
                        data.SelectCommand.CommandTimeout = 2000;
                        data.Fill(dtCSV);
                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                        {
                            string sln = dtCSV.Rows[i][0].ToString();
                            long.TryParse(sln, out Id_Val);
                            string tran_id = dtCSV.Rows[i][1].ToString();
                            string pnr_no = dtCSV.Rows[i][2].ToString();
                            //string ClientRefId = dtCSV.Rows[i][3].ToString();
                            string opr_id = dtCSV.Rows[i][3].ToString();
                            string Tdr_Can = dtCSV.Rows[i][10].ToString();
                            string bk_amt = dtCSV.Rows[i][6].ToString();
                            string bk_class = dtCSV.Rows[i][5].ToString();
                            string waitingAutocancelled = dtCSV.Rows[i][7].ToString();
                            DateTime tran_date = Convert.ToDateTime(dtCSV.Rows[i][8]);
                            string f_tran_date = tran_date.ToString("yyyy-MM-dd");
                            DateTime ActualRef_date = Convert.ToDateTime(dtCSV.Rows[i][9]);
                            string ActRef_Date = ActualRef_date.ToString("yyyy-MM-dd");
                            string user_id = dtCSV.Rows[i][11].ToString();
                            string CancellationId = dtCSV.Rows[i][13].ToString();

                            if (bk_class != "1A" || bk_class != "2A" || bk_class != "3A" || bk_class != "CC" || bk_class != "3E" || bk_class != "EC" || bk_class != "EA" || bk_class != "SL" || bk_class != "2S")
                            {
                                bk_class = string.Empty;
                            }

                            if (dtCSV.Rows[i][4].ToString() == "RDS" || dtCSV.Rows[i][4].ToString() == "VAPRON")
                            {
                                TBL_CANCELLATION_TEMP obkCANCELTmp = new TBL_CANCELLATION_TEMP()
                                {

                                    TRN_ID = tran_id,
                                    PNR_NO = pnr_no,
                                    CANCELLATION_ID = CancellationId,
                                    OPER_ID = opr_id,
                                    WAITING_AUTO_CANCELLED = waitingAutocancelled,
                                    REFUND_AMOUNT = Convert.ToDecimal(bk_amt),
                                    PNR_CLASS = bk_class,
                                    TRN_DATE = Convert.ToDateTime(f_tran_date),
                                    TDR_CAN = Tdr_Can,
                                    USER_ID = user_id,
                                    ACTUAL_REFUND_DATE = Convert.ToDateTime(ActRef_Date),
                                    SYSTEM_UPDATED_DATE = DateTime.Now

                                };
                                db.TBL_CANCELLATION_TEMP.Add(obkCANCELTmp);
                                await db.SaveChangesAsync();

                            }
                            //await db.SaveChangesAsync();
                            //ContextTransaction.Commit();
                            count++;
                        }
                        if (count == dtCSV.Rows.Count)
                        {
                            TempData["msgVal"] = "Cancellation File Import Successfully";
                            return RedirectToAction("GetAllRDSCancellationList", "MemberRDSFileProcessing", new { area = "Admin" });
                        }
                        else
                        {
                            TempData["msgVal"] = "Cancellation File Not Import Successfully";
                            return View("RDSCancellationFileUpload");
                        }


                    }
                    else
                    {
                        TempData["msgVal"] = "Please Upload files!!";
                        return View("RDSCancellationFileUpload");
                    }
                }
                catch (Exception ex)
                {
                    //ContextTransaction.Rollback();
                    TempData["msgVal"] = "File upload failed!!";
                    //ViewBag.Message = "File upload failed!!";
                    //Session["msgVal"] =  "File upload failed!!";
                    return View("RDSCancellationFileUpload");
                }
                //}
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });

            }

        }

        [HttpGet]
        public ActionResult RDSTDRFileUpload()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                TempData["msgVal"] = null;
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult RDSTDRFileUpload(HttpPostedFileBase file)
        {
            TempData["msgVal"] = null;
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/WhiteLabelLogo"), _FileName);
                    file.SaveAs(_path);
                }
                TempData["msgVal"] = "File Uploaded Successfully!!";
                //ViewBag.Message = "File Uploaded Successfully!!";
                //Session["msgVal"]= "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                TempData["msgVal"] = "File upload failed!!";
                //ViewBag.Message = "File upload failed!!";
                //Session["msgVal"] =  "File upload failed!!";
                return View();
            }
        }

        public ActionResult GetAllRDSBookingList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var BookingInfo = (from x in db.TBL_BOOKING_TEMP
                                   select new
                                   {
                                       bookingAmt = x.BOOKING_AMT
                                   }).Sum(z => z.bookingAmt);

                ViewBag.BookingAmt = BookingInfo;
                //var BookingPnr = db.TBL_BOOKING_TEMP.Count();
                var BookingPnr = db.TBL_BOOKING_TEMP.Select(x => x.PNR_NO).Count();

                ViewBag.BookingPnrNo = BookingPnr;

                var AgentPnrcount = db.TBL_BOOKING_TEMP.Select(x => x.USER_ID).Distinct().Count();


                ViewBag.BookingAgentPNR = AgentPnrcount;

                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult RdsBookingLilst()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_BOOKING_TEMP.ToList();
                return PartialView("RdsBookingLilst", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        //public async Task<ActionResult> TriggerBookingRDSFiles()
        public async Task<JsonResult> TriggerBookingRDSFiles()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                var db = new DBContext();
                var db_Val = new DBContext();
                TempData["msgVal"] = null;
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE BOOKING_TR");
                        var Bookingtemp = await db.TBL_BOOKING_TEMP.ToListAsync();
                        string TRAN_DATE = string.Empty;

                        foreach (var lst in Bookingtemp)
                        {
                            string SLN = lst.SLN.ToString();
                            string TRAN_ID = lst.TRN_ID.ToString();
                            string PNR_NO = lst.PNR_NO.ToString();
                            string OPR_ID = "";
                            string BK_AMT = lst.BOOKING_AMT.ToString();
                            TRAN_DATE = DateTime.Parse(lst.TRN_DATE.ToString()).ToString("yyyy-MM-dd");
                            string USER_ID = lst.USER_ID.ToString();
                            string BK_CLASS = lst.PNR_CLASS.ToString();

                            decimal bookigamt = 0;

                            decimal.TryParse(BK_AMT, out bookigamt);
                            DateTime TrnDateVal_1 = Convert.ToDateTime(TRAN_DATE);
                            DateTime dtFrom = DateTime.ParseExact(TRAN_DATE, "yyyy-mm-dd", CultureInfo.InvariantCulture);
                            var cmd_final_booking_pnr = await db.TBL_FINAL_RDS_BOOKING.Where(x => x.TRAN_DATE == TrnDateVal_1 && x.PNR == PNR_NO).FirstOrDefaultAsync();
                            //var cmd_final_booking_pnr = await db.TBL_FINAL_RDS_BOOKING.Where(x => x.PNR == PNR_NO && x.TRAN_DATE == Convert.ToDateTime(TRAN_DATE)).FirstOrDefaultAsync();
                            if (cmd_final_booking_pnr == null)
                            {
                                var GetMerchantInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.RAIL_ID == USER_ID);
                                var GetFinalBooking = await db.TBL_FINAL_RDS_BOOKING.Where(x => x.TRAN_ID == TRAN_ID && x.TRAN_DATE == TrnDateVal_1).FirstOrDefaultAsync();
                                if (GetFinalBooking != null)
                                {
                                    GetFinalBooking.PNR = PNR_NO;
                                    GetFinalBooking.OPR_ID = OPR_ID;
                                    GetFinalBooking.BOOKING_TRAN_STATUS = "Success";
                                    GetFinalBooking.TRAN_STATUS = "Success";
                                    GetFinalBooking.NOTES = BK_CLASS;
                                    GetFinalBooking.BOOKING_MER_RAIL_ID = USER_ID.ToString();
                                    db.Entry(GetFinalBooking).State = System.Data.Entity.EntityState.Modified;
                                    //await db.SaveChangesAsync();

                                    TBL_BOOKING_TR objBkTR = new TBL_BOOKING_TR()
                                    {
                                        sln = SLN,
                                        tran_id = TRAN_ID,
                                        pnr_id = PNR_NO,
                                        opr_id = OPR_ID,
                                        pnr_class = BK_CLASS,
                                        bk_amt = bookigamt,
                                        tran_date = Convert.ToDateTime(TRAN_DATE),
                                        user_id = USER_ID,
                                        reason = "SUCCESSFULLY UPDATED",

                                    };
                                    db.TBL_BOOKING_TR.Add(objBkTR);
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    TBL_BOOKING_TR objBkTR = new TBL_BOOKING_TR()
                                    {
                                        sln = SLN,
                                        tran_id = TRAN_ID,
                                        pnr_id = PNR_NO,
                                        opr_id = OPR_ID,
                                        pnr_class = BK_CLASS,
                                        bk_amt = bookigamt,
                                        tran_date = Convert.ToDateTime(TRAN_DATE),
                                        user_id = USER_ID,
                                        reason = "TRANSACTION ID IS NOT EXISTS ON DATED" + dtFrom,

                                    };
                                    db.TBL_BOOKING_TR.Add(objBkTR);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                #region PNR ALREADY EXIST RECORD ENTRY
                                string REASONS = "PNR IS ALREADY EXIST DATED " + TRAN_DATE;
                                TBL_BOOKING_TR objBkTR = new TBL_BOOKING_TR()
                                {
                                    sln = SLN,
                                    tran_id = TRAN_ID,
                                    pnr_id = PNR_NO,
                                    opr_id = OPR_ID,
                                    pnr_class = BK_CLASS,
                                    bk_amt = bookigamt,
                                    tran_date = Convert.ToDateTime(TRAN_DATE),
                                    user_id = USER_ID,
                                    reason = REASONS,

                                };
                                db.TBL_BOOKING_TR.Add(objBkTR);
                                await db.SaveChangesAsync();
                                #endregion
                            }
                        }

                        #region REFUND TRANSACTION PROCESS
                        DateTime TrnDateVal = Convert.ToDateTime(TRAN_DATE);
                        DateTime dtFromVal = DateTime.ParseExact(TRAN_DATE, "yyyy-mm-dd", CultureInfo.InvariantCulture);
                        var refund_booking_pnr = await db.TBL_FINAL_RDS_BOOKING.Where(x => x.PNR == "" && x.TRAN_DATE == TrnDateVal && x.TRAN_STATUS != "Failed").ToListAsync();

                        if (refund_booking_pnr.Count > 0)
                        {
                            foreach (var Blst in refund_booking_pnr)
                            {
                                long WLP_ID = Blst.WLP_ID;
                                long FR_USER_ID = Blst.MER_ID;
                                string TRANID = Blst.TRAN_ID;
                                string CORELATIONID = Blst.CORRELATION_ID;
                                var MerInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == FR_USER_ID);
                                decimal RefundAmt = Convert.ToDecimal(Blst.TOTAL_NET_PAYBLE);
                                //decimal PNR_COMM = Convert.ToDecimal(Blst.PG_CHARGE);
                                decimal CR_AMT = RefundAmt;
                                decimal FR_CURR_BAL = Convert.ToDecimal(MerInfo.BALANCE);
                                decimal UPDATED_CURR_BAL = FR_CURR_BAL + CR_AMT;
                                var FinalRefundRDS = await db.TBL_FINAL_RDS_BOOKING.FirstOrDefaultAsync(x => x.TRAN_ID == TRANID);
                                FinalRefundRDS.TRAN_STATUS = "Failed";
                                db.Entry(FinalRefundRDS).State = System.Data.Entity.EntityState.Modified;
                                MerInfo.BALANCE = UPDATED_CURR_BAL;
                                db.Entry(MerInfo).State = System.Data.Entity.EntityState.Modified;
                                TBL_ACCOUNTS objAcnt = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = FR_USER_ID,
                                    MEMBER_TYPE = "RETAILER",
                                    TRANSACTION_TYPE = "RDS REFUND",
                                    TRANSACTION_DATE = Convert.ToDateTime(TRAN_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "CR",
                                    AMOUNT = RefundAmt,
                                    NARRATION = "RDS REFUND",
                                    OPENING = FR_CURR_BAL,
                                    CLOSING = UPDATED_CURR_BAL,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    CORELATIONID = CORELATIONID,
                                    SERVICE_ID = 7,
                                    WHITELEVEL_ID = Blst.WLP_ID,
                                    SUPER_ID = 0,
                                    DISTRIBUTOR_ID = Blst.DIST_ID,
                                    STATE_ID = MerInfo.STATE_ID
                                };
                                db.TBL_ACCOUNTS.Add(objAcnt);

                                decimal Wlp_MainBal = 0;
                                decimal Wlp_AddMainBal = 0;
                                var WLPInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WLP_ID);
                                decimal.TryParse(WLPInfo.BALANCE.ToString(), out Wlp_MainBal);
                                Wlp_AddMainBal = Wlp_MainBal + RefundAmt;
                                WLPInfo.BALANCE = Wlp_AddMainBal;
                                db.Entry(WLPInfo).State = System.Data.Entity.EntityState.Modified;
                                TBL_ACCOUNTS objWLPAcnt = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = WLP_ID,
                                    MEMBER_TYPE = "WHITELABEL",
                                    TRANSACTION_TYPE = "RDS REFUND",
                                    TRANSACTION_DATE = Convert.ToDateTime(TRAN_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "CR",
                                    AMOUNT = RefundAmt,
                                    NARRATION = "RDS REFUND",
                                    OPENING = Wlp_MainBal,
                                    CLOSING = Wlp_AddMainBal,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    CORELATIONID = CORELATIONID,
                                    SERVICE_ID = 7,
                                    WHITELEVEL_ID = Blst.WLP_ID,
                                    SUPER_ID = 0,
                                    DISTRIBUTOR_ID = Blst.DIST_ID,
                                    STATE_ID = 9
                                };
                                db.TBL_ACCOUNTS.Add(objWLPAcnt);
                                await db.SaveChangesAsync();
                            }
                        }

                        //TempData["msgVal"] = "RDS Booking File Triggered Successfully ";
                        #endregion
                        ContextTransaction.Commit();
                        return Json("Triggered RDS Booking File is done successfully ", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        //TempData["msgVal"] = "RDS Booking File Triggered Failed ";
                        ContextTransaction.Rollback();
                        throw ex;
                        return Json("Please contact to Administrator", JsonRequestBehavior.AllowGet);
                    }
                }

                //return View("GetAllRDSBookingList");
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                Response.Redirect(Url.Action("Index", "Login"));
                return Json("", JsonRequestBehavior.AllowGet);
            }




        }

        public ActionResult GetAllRDSCancellationList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var BookingInfo = (from x in db.TBL_CANCELLATION_TEMP
                                   select new
                                   {
                                       bookingAmt = x.REFUND_AMOUNT
                                   }).Sum(z => z.bookingAmt);

                ViewBag.BookingAmt = BookingInfo;
                //var BookingPnr = db.TBL_BOOKING_TEMP.Count();
                var BookingPnr = db.TBL_CANCELLATION_TEMP.Select(x => x.PNR_NO).Count();

                ViewBag.BookingPnrNo = BookingPnr;

                var AgentPnrcount = db.TBL_CANCELLATION_TEMP.Select(x => x.USER_ID).Distinct().Count();


                ViewBag.BookingAgentPNR = AgentPnrcount;

                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult RdsCancellationLilst()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_CANCELLATION_TEMP.ToList();
                return PartialView("RdsCancellationLilst", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [HttpPost]
        //public async Task<ActionResult> TriggerBookingRDSFiles()
        public async Task<JsonResult> TriggerCancellationRDSFiles()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                var db = new DBContext();
                var db_Val = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCEL_TR");
                        var CancellationTemplist = await db.TBL_CANCELLATION_TEMP.ToListAsync();
                        //var FinalCancellationList = await db.TBL_FINAL_CANCELLATION.ToListAsync();
                        var MEMBER_LIST = await db.TBL_MASTER_MEMBER.ToListAsync();
                        string TRAN_DATE = string.Empty;
                        foreach (var Cancellst in CancellationTemplist)
                        {
                            int count = 0;

                            //string ID = Cancellst.id;
                            string TRAN_ID = Cancellst.TRN_ID;
                            string PNR_NO = Cancellst.PNR_NO;
                            string OPR_ID = Cancellst.OPER_ID;
                            string TR_CLASS = Cancellst.PNR_CLASS;
                            string REF_AMT = Cancellst.REFUND_AMOUNT.ToString();
                            string WT_AT_CAN = Cancellst.WAITING_AUTO_CANCELLED;

                            TRAN_DATE = DateTime.Parse(Cancellst.ACTUAL_REFUND_DATE.ToString()).ToString("yyyy-MM-dd");
                            DateTime TrnDateVal_1 = Convert.ToDateTime(TRAN_DATE);
                            string TDR_CAN = Cancellst.TDR_CAN;
                            string USER_ID = Cancellst.USER_ID;
                            string CANCELLATION_ID = Cancellst.CANCELLATION_ID;
                            //foreach (var MERClst in MEMBER_LIST)
                            //{
                            var CheckCalcellationMem_Id = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.RAIL_ID == USER_ID);
                            //if (USER_ID == MERClst.RAIL_ID)
                            if (USER_ID == CheckCalcellationMem_Id.RAIL_ID)
                            {
                                count++;
                                var FinalCancellationlist = await db.TBL_FINAL_CANCELLATION.Where(x => x.TRN_DATE == TrnDateVal_1).ToListAsync();
                                int Flag = 0;
                                foreach (var FinalCanlst in FinalCancellationlist)
                                {
                                    if (CANCELLATION_ID == FinalCanlst.CANCELLATION_ID)
                                    {
                                        Flag++;
                                    }
                                }
                                if (Flag == 0)
                                {
                                    long WLP_ID = (long)CheckCalcellationMem_Id.UNDER_WHITE_LEVEL;
                                    string CorelationId = string.Empty;
                                    CorelationId = GetUniqueKey(USER_ID);
                                    string CancelMerid = string.Empty;

                                    bool PG_CHARGE_APPLY = false;
                                    decimal PG_CHARGE_MAX_VAL = 0;
                                    decimal PG_CHARGE_LESS_THAN_2000 = 0;
                                    decimal PG_CHARGE_GREATER_THAN_2000 = 0;
                                    bool PG_CHARGE_GST_APPLY = false;
                                    decimal PG_CHARGE_GST_VAL = 0;
                                    bool ADDN_CHARGE_APPLY = false;
                                    decimal ADDN_CHARGE_MAX_VAL = 0;
                                    decimal ADDN_CHARGE_AC = 0;
                                    decimal ADDN_CHARGE_NON_AC = 0;
                                    bool ADDN_CHARGE_GST_APPLY = false;
                                    decimal ADDN_CHARGE_GST_VAL = 0;
                                    decimal TOTAL_NET_PAYBLE_WITHOUT_GST = 0;
                                    decimal TOTAL_NET_PAYBLE_GST = 0;
                                    decimal TOTAL_NET_PAYBLE = 0;
                                    string CORRELATION_ID = string.Empty;
                                    decimal GST_RATE = 0;
                                    string Remark = string.Empty;
                                    string Notes = string.Empty;
                                    string IPAddressA = string.Empty;
                                    string CORE_ID = string.Empty;
                                    var GetRailAgentCom = await db.TBL_RAIL_AGENTS_COMMISSION.FirstOrDefaultAsync(x => x.RAIL_AGENT_ID == USER_ID);
                                    if (GetRailAgentCom != null)
                                    {
                                        PG_CHARGE_APPLY = GetRailAgentCom.PG_CHARGES_APPLY;
                                        PG_CHARGE_MAX_VAL = GetRailAgentCom.PG_MAX_VALUE;
                                        PG_CHARGE_LESS_THAN_2000 = GetRailAgentCom.PG_EQUAL_LESS_2000;
                                        PG_CHARGE_GREATER_THAN_2000 = GetRailAgentCom.PG_EQUAL_GREATER_2000;
                                        PG_CHARGE_GST_APPLY = GetRailAgentCom.PG_CHARGES_APPLY;
                                        PG_CHARGE_GST_VAL = 0;
                                        ADDN_CHARGE_APPLY = GetRailAgentCom.PG_CHARGES_APPLY;
                                        ADDN_CHARGE_MAX_VAL = GetRailAgentCom.ADDITIONAL_CHARGE_MAX_VAL;
                                        ADDN_CHARGE_AC = GetRailAgentCom.ADDITIONAL_CHARGE_AC;
                                        ADDN_CHARGE_NON_AC = GetRailAgentCom.ADDITIONAL_CHARGE_NON_AC;
                                        ADDN_CHARGE_GST_APPLY = (GetRailAgentCom.ADDITIONAL_GST_STATUS == "Yes" ? true : false); ;
                                        ADDN_CHARGE_GST_VAL = 0;
                                        TOTAL_NET_PAYBLE_WITHOUT_GST = Convert.ToDecimal(REF_AMT);
                                        TOTAL_NET_PAYBLE_GST = 18M;
                                        TOTAL_NET_PAYBLE = Convert.ToDecimal(REF_AMT);
                                        GST_RATE = 18M;
                                    }

                                    var GetFinalBookingRds = await db.TBL_FINAL_RDS_BOOKING.FirstOrDefaultAsync(x => x.TRAN_ID == TRAN_ID);
                                    if (GetFinalBookingRds != null)
                                    {
                                        PG_CHARGE_GST_VAL = GetFinalBookingRds.PG_CHARGE_GST_VAL;
                                        ADDN_CHARGE_GST_VAL = GetFinalBookingRds.ADDN_CHARGE_GST_VAL;
                                        TOTAL_NET_PAYBLE_WITHOUT_GST = GetFinalBookingRds.TOTAL_NET_PAYBLE_WITHOUT_GST;
                                        TOTAL_NET_PAYBLE_GST = GetFinalBookingRds.TOTAL_NET_PAYBLE_GST;
                                        TOTAL_NET_PAYBLE = GetFinalBookingRds.TOTAL_NET_PAYBLE;
                                        GST_RATE = GetFinalBookingRds.GST_RATE;
                                        Remark = GetFinalBookingRds.REMARKS;
                                        Notes = GetFinalBookingRds.NOTES;
                                        IPAddressA = GetFinalBookingRds.IP_ADDRESS;
                                        CORE_ID = GetFinalBookingRds.CORRELATION_ID;
                                        CancelMerid = GetFinalBookingRds.MER_RAIL_ID;
                                    }
                                    else
                                    {
                                        CORE_ID = CorelationId;
                                        CancelMerid = USER_ID;
                                    }
                                    TBL_FINAL_CANCELLATION ObjFinalCanc = new TBL_FINAL_CANCELLATION()
                                    {
                                        TRN_ID = TRAN_ID,
                                        PNR_NO = PNR_NO,
                                        OPR_ID = OPR_ID,
                                        PNR_CLASS = TR_CLASS,
                                        REFUND_AMT = Convert.ToDecimal(REF_AMT),
                                        WT_AUTO_CAN = WT_AT_CAN,
                                        TRN_DATE = Convert.ToDateTime(TRAN_DATE),
                                        SYSTEM_DATE = DateTime.Now,
                                        TDR_CAN = TDR_CAN,
                                        CANCELLATION_TYPE = "",
                                        CANCELLATION_ID = CANCELLATION_ID,
                                        MER_RAIL_ID = USER_ID,
                                        CANCELLATION_AGST_MER_RAIL_ID = CancelMerid,
                                        MER_ID = CheckCalcellationMem_Id.MEM_ID,
                                        DIST_ID = (long)CheckCalcellationMem_Id.INTRODUCER,
                                        SUP_ID = 0,
                                        WLP_ID = (long)CheckCalcellationMem_Id.UNDER_WHITE_LEVEL,
                                        PG_CHARGE_APPLY = PG_CHARGE_APPLY,
                                        PG_CHARGE_MAX_VAL = PG_CHARGE_MAX_VAL,
                                        PG_CHARGE_LESS_THAN_2000 = PG_CHARGE_LESS_THAN_2000,
                                        PG_CHARGE_GREATER_THAN_2000 = PG_CHARGE_GREATER_THAN_2000,
                                        PG_CHARGE_GST_APPLY = PG_CHARGE_GST_APPLY,
                                        PG_CHARGE_GST_VAL = PG_CHARGE_GST_VAL,
                                        ADDN_CHARGE_APPLY = ADDN_CHARGE_APPLY,
                                        ADDN_CHARGE_MAX_VAL = ADDN_CHARGE_MAX_VAL,
                                        ADDN_CHARGE_AC = ADDN_CHARGE_AC,
                                        ADDN_CHARGE_NON_AC = ADDN_CHARGE_NON_AC,
                                        ADDN_CHARGE_GST_APPLY = ADDN_CHARGE_GST_APPLY,
                                        ADDN_CHARGE_GST_VAL = ADDN_CHARGE_GST_VAL,
                                        TOTAL_NET_PAYBLE_WITHOUT_GST = TOTAL_NET_PAYBLE_WITHOUT_GST,
                                        TOTAL_NET_PAYBLE_GST = TOTAL_NET_PAYBLE_GST,
                                        TOTAL_NET_PAYBLE = TOTAL_NET_PAYBLE,
                                        CORRELATION_ID = CorelationId,
                                        GST_RATE = GST_RATE,
                                        REMARKS = Remark,
                                        NOTES = Notes,
                                        IP_ADDRESS = IPAddressA
                                    };
                                    db.TBL_FINAL_CANCELLATION.Add(ObjFinalCanc);

                                    decimal REFUND_AMT = Convert.ToDecimal(REF_AMT);
                                    var MEr_ID_VAl = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.RAIL_ID == CancelMerid);
                                    decimal Mer_CURR_Bal = 0;
                                    decimal.TryParse(MEr_ID_VAl.BALANCE.ToString(), out Mer_CURR_Bal);
                                    decimal UPDATED_CURR_BAL = Mer_CURR_Bal + REFUND_AMT;
                                    MEr_ID_VAl.BALANCE = UPDATED_CURR_BAL;
                                    db.Entry(MEr_ID_VAl).State = System.Data.Entity.EntityState.Modified;

                                    string UNUSED_VAL = "NOT APPLICABLE";
                                    string TRAN_TYPE = "CANCELLATION";
                                    string DR_CR = "CR";
                                    //string TR_THROUGH = "CURRENT BALANCE";
                                    decimal TR_CHARGE = 0;

                                    string SYS_UPDATE_DATE = DateTime.Now.ToShortDateString();
                                    DateTime.Parse(SYS_UPDATE_DATE).ToString("yyyy-MM-dd");

                                    string SYS_UP_TIME = DateTime.Now.ToString("yyyy-MM-dd H:m:s");
                                    TBL_ACCOUNTS ObjAcnt = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = MEr_ID_VAl.MEM_ID,
                                        MEMBER_TYPE = "RETAILER",
                                        TRANSACTION_TYPE = "RDS CANCELLATION REFUND",
                                        TRANSACTION_DATE = Convert.ToDateTime(TRAN_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = DR_CR,
                                        AMOUNT = REFUND_AMT,
                                        NARRATION = "RDS CANCELLATION REFUND",
                                        OPENING = Mer_CURR_Bal,
                                        CLOSING = UPDATED_CURR_BAL,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        CORELATIONID = CorelationId,
                                        SERVICE_ID = 7,
                                        WHITELEVEL_ID = (long)MEr_ID_VAl.UNDER_WHITE_LEVEL,
                                        SUPER_ID = 0,
                                        DISTRIBUTOR_ID = (long)MEr_ID_VAl.INTRODUCER,
                                        STATE_ID = (long)MEr_ID_VAl.STATE_ID
                                    };
                                    db.TBL_ACCOUNTS.Add(ObjAcnt);

                                    string REASONS = "SUCCESSFULLY UPDATED";
                                    TBL_CANCEL_TR objCanTR = new TBL_CANCEL_TR()
                                    {
                                        id = TRAN_ID,
                                        tran_id = TRAN_ID,
                                        pnr_no = PNR_NO,
                                        opr_id = OPR_ID,
                                        pnr_class = TR_CLASS,
                                        ref_amt = Convert.ToDecimal(REF_AMT),
                                        wt_at_can = WT_AT_CAN,
                                        tran_date = Convert.ToDateTime(TRAN_DATE),
                                        tdr_can = TDR_CAN,
                                        user_id = USER_ID,
                                        cancelID = CANCELLATION_ID,
                                        reason = REASONS
                                    };
                                    db.TBL_CANCEL_TR.Add(objCanTR);


                                    decimal Wlp_MainBal = 0;
                                    decimal Wlp_AddMainBal = 0;
                                    var WLPInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WLP_ID);
                                    decimal.TryParse(WLPInfo.BALANCE.ToString(), out Wlp_MainBal);
                                    Wlp_AddMainBal = Wlp_MainBal + REFUND_AMT;
                                    WLPInfo.BALANCE = Wlp_AddMainBal;
                                    db.Entry(WLPInfo).State = System.Data.Entity.EntityState.Modified;
                                    TBL_ACCOUNTS objWLPAcnt = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = WLP_ID,
                                        MEMBER_TYPE = "WHITELABEL",
                                        TRANSACTION_TYPE = "RDS REFUND",
                                        TRANSACTION_DATE = Convert.ToDateTime(TRAN_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = "CR",
                                        AMOUNT = REFUND_AMT,
                                        NARRATION = "RDS REFUND",
                                        OPENING = Wlp_MainBal,
                                        CLOSING = Wlp_AddMainBal,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        CORELATIONID = CorelationId,
                                        SERVICE_ID = 7,
                                        WHITELEVEL_ID = WLP_ID,
                                        SUPER_ID = 0,
                                        DISTRIBUTOR_ID = (long)MEr_ID_VAl.INTRODUCER,
                                        STATE_ID = 9
                                    };
                                    db.TBL_ACCOUNTS.Add(objWLPAcnt);
                                    await db.SaveChangesAsync();
                                    //ContextTransaction.Commit();
                                    //return Json("Triggered RDS Booking File is done successfully ", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    string REASONS = "SAME CANCELLATION ID IS ALREADY EXIST DATED " + TRAN_DATE;
                                    TBL_CANCEL_TR objCanTR = new TBL_CANCEL_TR()
                                    {
                                        id = TRAN_ID,
                                        tran_id = TRAN_ID,
                                        pnr_no = PNR_NO,
                                        opr_id = OPR_ID,
                                        pnr_class = TR_CLASS,
                                        ref_amt = Convert.ToDecimal(REF_AMT),
                                        wt_at_can = WT_AT_CAN,
                                        tran_date = Convert.ToDateTime(TRAN_DATE),
                                        tdr_can = TDR_CAN,
                                        user_id = USER_ID,
                                        cancelID = CANCELLATION_ID,
                                        reason = REASONS
                                    };
                                    db.TBL_CANCEL_TR.Add(objCanTR);
                                    await db.SaveChangesAsync();
                                    //ContextTransaction.Commit();
                                    //return Json(REASONS, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                string REASON = "USER ID NOT REGISTERED IN SYSTEM";
                                TBL_CANCEL_TR objCanTR = new TBL_CANCEL_TR()
                                {
                                    id = TRAN_ID,
                                    tran_id = TRAN_ID,
                                    pnr_no = PNR_NO,
                                    opr_id = OPR_ID,
                                    pnr_class = TR_CLASS,
                                    ref_amt = Convert.ToDecimal(REF_AMT),
                                    wt_at_can = WT_AT_CAN,
                                    tran_date = Convert.ToDateTime(TRAN_DATE),
                                    tdr_can = TDR_CAN,
                                    user_id = USER_ID,
                                    cancelID = CANCELLATION_ID,
                                    reason = REASON
                                };
                                db.TBL_CANCEL_TR.Add(objCanTR);
                                await db.SaveChangesAsync();
                                //ContextTransaction.Commit();
                                //return Json(REASON, JsonRequestBehavior.AllowGet);
                            }
                            //}
                            if (count == 0)
                            {
                                string REASON = "USER ID NOT REGISTERED IN SYSTEM";
                                TBL_CANCEL_TR objCanTR = new TBL_CANCEL_TR()
                                {
                                    id = TRAN_ID,
                                    tran_id = TRAN_ID,
                                    pnr_no = PNR_NO,
                                    opr_id = OPR_ID,
                                    pnr_class = TR_CLASS,
                                    ref_amt = Convert.ToDecimal(REF_AMT),
                                    wt_at_can = WT_AT_CAN,
                                    tran_date = Convert.ToDateTime(TRAN_DATE),
                                    tdr_can = TDR_CAN,
                                    user_id = USER_ID,
                                    cancelID = CANCELLATION_ID,
                                    reason = REASON
                                };
                                db.TBL_CANCEL_TR.Add(objCanTR);
                                await db.SaveChangesAsync();
                                //ContextTransaction.Commit();
                                //return Json(REASON, JsonRequestBehavior.AllowGet);
                            }
                        }
                        ContextTransaction.Commit();
                        return Json("Triggered RDS Booking File is done successfully ", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        //TempData["msgVal"] = "RDS Booking File Triggered Failed ";
                        ContextTransaction.Rollback();
                        throw ex;
                        return Json("Please contact to Administrator", JsonRequestBehavior.AllowGet);
                    }
                }

                //return View("GetAllRDSBookingList");
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                
                Response.Redirect(Url.Action("Index", "Login"));
                return Json("", JsonRequestBehavior.AllowGet);
            }




        }
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }

        public ActionResult AllRDSBookingList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var dbcontext = new DBContext();
                var NONbooked = dbcontext.TBL_BOOKING_TR.Where(x => x.reason != "SUCCESSFULLY UPDATED").ToList();
                ViewBag.NonBookedPNR = NONbooked.Count();
                var booked = dbcontext.TBL_BOOKING_TR.Count(x => x.reason == "SUCCESSFULLY UPDATED");
                ViewBag.BookedPNR = booked;
                //var BookingInfo = (from x in dbcontext.TBL_BOOKING_TR where x.reason == "SUCCESSFULLY UPDATED"
                //                   select new
                //                   {
                //                       bookingAmt = x.bk_amt
                //                   }).Sum(z => z.bookingAmt);
                //ViewBag.BookingBookedPNR = BookingInfo;
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }


        public ActionResult AllRDSCancellationTRList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var dbcontext = new DBContext();
                var NONbooked = dbcontext.TBL_CANCEL_TR.Where(x => x.reason != "SUCCESSFULLY UPDATED").ToList();
                ViewBag.NonBookedPNR = NONbooked.Count();
                var booked = dbcontext.TBL_CANCEL_TR.Count(x => x.reason == "SUCCESSFULLY UPDATED");
                ViewBag.BookedPNR = booked;
                //var BookingInfo = (from x in dbcontext.TBL_BOOKING_TR where x.reason == "SUCCESSFULLY UPDATED"
                //                   select new
                //                   {
                //                       bookingAmt = x.bk_amt
                //                   }).Sum(z => z.bookingAmt);
                //ViewBag.BookingBookedPNR = BookingInfo;
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });

            }
        }

        public PartialViewResult RdsBookNONBookingLilst()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_BOOKING_TR.Where(x => x.reason != "SUCCESSFULLY UPDATED").ToList();
                return PartialView("RdsBookNONBookingLilst", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PartialViewResult RdsBookBookingDetailsInfo()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_BOOKING_TR.Where(x => x.reason == "SUCCESSFULLY UPDATED").ToList();
                return PartialView("RdsBookBookingDetailsInfo", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PartialViewResult RdsCancellationDetails()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_CANCEL_TR.Where(x => x.reason != "SUCCESSFULLY UPDATED").ToList();
                return PartialView("RdsCancellationDetails", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PartialViewResult RdsCancellationDetailsInfo()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_CANCEL_TR.Where(x => x.reason == "SUCCESSFULLY UPDATED").ToList();
                return PartialView("RdsCancellationDetailsInfo", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult CancellationPNRList()
        {
            return View();
        }

        public ActionResult TDRFILEUplaod()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                TempData["msgVal"] = null;
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });

            }

        }
        [HttpPost]
        public async Task<ActionResult> UploadTDRFile(HttpPostedFileBase file)
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                TempData["msgVal"] = null;
                var db = new DBContext();
                var db_Val = new DBContext();
                //using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                //{
                try
                {
                    if (file.ContentLength > 0)
                    {
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCELLATION_FILE_PATH");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCELLATION_TEMP");
                        db_Val.Database.ExecuteSqlCommand("TRUNCATE TABLE CANCEL_TR");
                        string File_NameValue = Path.GetFileName(file.FileName);
                        string file_path_Value = Server.MapPath("~/UPLOADS/CANCEL_UPLOAD") + "//" + file.FileName;
                        if (System.IO.File.Exists(file_path_Value))
                        {
                            System.IO.File.Delete(file_path_Value);
                        }
                        TBL_CANCELLATION_FILE_PATH ObjcANCELFile = new TBL_CANCELLATION_FILE_PATH()
                        {
                            FILE_NAME = File_NameValue,
                            FILE_PATH = file_path_Value
                        };
                        db.TBL_CANCELLATION_FILE_PATH.Add(ObjcANCELFile);
                        await db.SaveChangesAsync();
                        file.SaveAs(file_path_Value);
                        /* -- Import CSV Code Start -- */
                        long Id_Val = 0;
                        int count = 0;
                        string BookingfileName = Server.MapPath("~/UPLOADS/CANCEL_UPLOAD") + "//" + file.FileName;
                        DataTable dtCSV = new DataTable();
                        //string SourceConstr = @"Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + fileName + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'";
                        string SourceConstr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + System.IO.Path.GetDirectoryName(BookingfileName) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"";

                        OleDbConnection con = new OleDbConnection(SourceConstr);
                        string query = "SELECT * FROM [" + System.IO.Path.GetFileName(BookingfileName) + "]";
                        OleDbDataAdapter data = new OleDbDataAdapter(query, con);
                        data.SelectCommand.CommandTimeout = 2000;
                        data.Fill(dtCSV);
                        for (int i = 0; i < dtCSV.Rows.Count; i++)
                        {
                            string sln = dtCSV.Rows[i][0].ToString();
                            long.TryParse(sln, out Id_Val);
                            string tran_id = dtCSV.Rows[i][2].ToString();
                            string pnr_no = dtCSV.Rows[i][3].ToString();
                            //string ClientRefId = dtCSV.Rows[i][3].ToString();
                            string opr_id = dtCSV.Rows[i][3].ToString();
                            string Tdr_Can = "";
                            string Ref_Status = dtCSV.Rows[i][12].ToString();
                            string ref_Amt = dtCSV.Rows[i][8].ToString();
                            string Ticket_Amt = dtCSV.Rows[i][9].ToString();
                            string PG_NAme = dtCSV.Rows[i][7].ToString();
                            DateTime tran_date = Convert.ToDateTime(dtCSV.Rows[i][13]);
                            string f_tran_date = tran_date.ToString("yyyy-MM-dd");

                            DateTime Txn_dateVal = Convert.ToDateTime(dtCSV.Rows[i][10]);
                            string txn_tran_date = Txn_dateVal.ToString("yyyy-MM-dd");
                            //DateTime ActualRef_date = Convert.ToDateTime(dtCSV.Rows[i][9]);
                            //string ActRef_Date = ActualRef_date.ToString("yyyy-MM-dd");
                            string user_id = dtCSV.Rows[i][4].ToString();
                            string CancellationId = dtCSV.Rows[i][6].ToString();


                            if (dtCSV.Rows[i][7].ToString() == "RDS" || dtCSV.Rows[i][7].ToString() == "VAPRON")
                            {
                                TBL_CANCELLATION_TEMP obkCANCELTmp = new TBL_CANCELLATION_TEMP()
                                {

                                    TRN_ID = tran_id,
                                    PNR_NO = pnr_no,
                                    CANCELLATION_ID = CancellationId,
                                    OPER_ID = opr_id,
                                    WAITING_AUTO_CANCELLED = "",
                                    REFUND_AMOUNT = Convert.ToDecimal(ref_Amt),
                                    PNR_CLASS = "",
                                    TRN_DATE = Convert.ToDateTime(f_tran_date),
                                    TDR_CAN = Tdr_Can,
                                    USER_ID = user_id,
                                    ACTUAL_REFUND_DATE = Convert.ToDateTime(txn_tran_date),
                                    SYSTEM_UPDATED_DATE = DateTime.Now

                                };
                                db.TBL_CANCELLATION_TEMP.Add(obkCANCELTmp);
                                await db.SaveChangesAsync();
                                //cmd2 = new SqlCommand("insert into booking_temp values('" + sln + "','" + tran_id + "','" + pnr_no + "','" + opr_id + "','" + Convert.ToDecimal(bk_amt) + "','" + f_tran_date + "','" + user_id + "','" + bk_class + "')", conLinq);
                                //cmd2.CommandTimeout = 2000;
                                //cmd2.ExecuteNonQuery();
                            }
                            //await db.SaveChangesAsync();
                            //ContextTransaction.Commit();
                            count++;
                        }
                        TempData["msgVal"] = "TDR File upload Successfully!!";
                    }
                    else
                    {
                        TempData["msgVal"] = "Please Upload TDR files!!";
                        return View("TDRFILEUplaod");
                    }
                    //return View("GetAllRDSCancellationList");
                    return RedirectToAction("GetAllRDSCancellationList", "MemberRDSFileProcessing", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    //ContextTransaction.Rollback();
                    TempData["msgVal"] = "TDR File upload failed!!";
                    //ViewBag.Message = "File upload failed!!";
                    //Session["msgVal"] =  "File upload failed!!";
                    return View("TDRFILEUplaod");
                }
                //}
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
    }
}
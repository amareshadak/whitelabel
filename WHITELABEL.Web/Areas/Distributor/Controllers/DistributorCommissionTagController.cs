using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorCommissionTagController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Distributor Dashboard";
             
                if (Session["DistributorUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;
                if (Session["DistributorUserId"] != null)
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

        // GET: Distributor/DistributorCommissionTag
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete()
        {
            try
            {
                long currentuser = MemberCurrentUser.MEM_ID;
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == currentuser).Select(x => new { x.MEM_ID, x.UName }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- AutoComplete (POST) Line No:- 103", ex);                
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> MobileRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 1).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.RECHARGE_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1 && whitelebelslab.SLAB_STATUS==true
                                     select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- MobileRechargeSlab (POST) Line No:- 138", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> UtilityRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    ////var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 2).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.BILLPAYMENT_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2 && whitelebelslab.SLAB_STATUS == true
                                     select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- UtilityRechargeSlab (POST) Line No:- 174", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> DMRRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    ////var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 3).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.DMR_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3 && whitelebelslab.SLAB_STATUS == true
                                     select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- DMRRechargeSlab (POST) Line No:- 210", ex);
                throw ex;
            }

        }
        [HttpPost]
        public async Task<JsonResult> AirSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 4).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.AIR_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 4 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 4
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- AirSlabDetails (POST) Line No:- 238", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> BusSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 5).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.BUS_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 5 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 5
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- BusSlabDetails (POST) Line No:- 265", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> HotelSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 6).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.HOTEL_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 6 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 6
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- HotelSlabDetails (POST) Line No:- 292", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> CashCardSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 7).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.CASHCARD_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 7 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 7
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- CashCardSlabDetails (POST) Line No:- 319", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveCommissionSlab(DistributorCommissionSlabTaggingModelView objval)
        {
            try
            {
                var db = new DBContext();
                var GEtInforComm = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == objval.WHITE_LEVEL_ID).FirstOrDefault();
                if (GEtInforComm != null)
                {
                    var checkValUpdate = await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.SL_NO == GEtInforComm.SL_NO).FirstOrDefaultAsync();
                    //var checkVal = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.WHITE_LEVEL_ID == objval.WHITE_LEVEL_ID && x.RECHARGE_SLAB==objval.RECHARGE_SLAB && x.BILLPAYMENT_SLAB==objval.BILLPAYMENT_SLAB && x.DMR_SLAB==objval.DMR_SLAB ).FirstOrDefault();
                    if (checkValUpdate != null)
                    {
                        checkValUpdate.RECHARGE_SLAB = objval.RECHARGE_SLAB;
                        checkValUpdate.BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB;
                        checkValUpdate.DMR_SLAB = objval.DMR_SLAB;
                        checkValUpdate.AIR_SLAB = objval.AIR_SLAB;
                        checkValUpdate.BUS_SLAB = objval.BUS_SLAB;
                        checkValUpdate.HOTEL_SLAB = objval.HOTEL_SLAB;
                        checkValUpdate.CASHCARD_SLAB = objval.CASHCARD_SLAB;
                        db.Entry(checkValUpdate).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Updated" });
                    }
                    else
                    {
                        TBL_DETAILS_MEMBER_COMMISSION_SLAB objmodel = new TBL_DETAILS_MEMBER_COMMISSION_SLAB()
                        {
                            WHITE_LEVEL_ID = MemberCurrentUser.MEM_ID,
                            INTRODUCER_ID = objval.WHITE_LEVEL_ID,
                            INTRODUCE_TO_ID = objval.INTRODUCE_TO_ID,
                            RECHARGE_SLAB = objval.RECHARGE_SLAB,
                            BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB,
                            DMR_SLAB = objval.DMR_SLAB,
                            AIR_SLAB = objval.AIR_SLAB,
                            BUS_SLAB = objval.BUS_SLAB,
                            HOTEL_SLAB = objval.HOTEL_SLAB,
                            CASHCARD_SLAB = objval.CASHCARD_SLAB,
                            STATUS = true,
                            CREATED_DATE = DateTime.Now
                        };
                        db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Add(objmodel);
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Success" });
                    }
                }
                else
                {
                    var checkVal = await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.SL_NO == objval.SL_NO).FirstOrDefaultAsync();
                    //var checkVal = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.WHITE_LEVEL_ID == objval.WHITE_LEVEL_ID && x.RECHARGE_SLAB==objval.RECHARGE_SLAB && x.BILLPAYMENT_SLAB==objval.BILLPAYMENT_SLAB && x.DMR_SLAB==objval.DMR_SLAB ).FirstOrDefault();
                    if (checkVal != null)
                    {
                        checkVal.RECHARGE_SLAB = objval.RECHARGE_SLAB;
                        checkVal.BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB;
                        checkVal.DMR_SLAB = objval.DMR_SLAB;
                        checkVal.AIR_SLAB = objval.AIR_SLAB;
                        checkVal.BUS_SLAB = objval.BUS_SLAB;
                        checkVal.HOTEL_SLAB = objval.HOTEL_SLAB;
                        checkVal.CASHCARD_SLAB = objval.CASHCARD_SLAB;
                        db.Entry(checkVal).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Updated" });
                    }
                    else
                    {
                        TBL_DETAILS_MEMBER_COMMISSION_SLAB objmodel = new TBL_DETAILS_MEMBER_COMMISSION_SLAB()
                        {
                            WHITE_LEVEL_ID = MemberCurrentUser.MEM_ID,
                            INTRODUCER_ID = objval.WHITE_LEVEL_ID,
                            INTRODUCE_TO_ID = objval.INTRODUCE_TO_ID,
                            RECHARGE_SLAB = objval.RECHARGE_SLAB,
                            BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB,
                            DMR_SLAB = objval.DMR_SLAB,
                            AIR_SLAB = objval.AIR_SLAB,
                            BUS_SLAB = objval.BUS_SLAB,
                            HOTEL_SLAB = objval.HOTEL_SLAB,
                            CASHCARD_SLAB = objval.CASHCARD_SLAB,
                            STATUS = true,
                            CREATED_DATE = DateTime.Now
                        };
                        db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Add(objmodel);
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Success" });
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- SaveCommissionSlab (POST) Line No:- 368", ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetListInformation()
        {
            try
            {
                var db = new DBContext();
                var list = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                            where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID != 0
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                MEM_UNIQUE_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEM_UNIQUE_ID).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                            {
                                SL_NO = s.ID,
                                WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                MEM_Unique_ID=s.MEM_UNIQUE_ID,
                                INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                INTRODUCER_ID = s.INTRODUCER_ID,
                                RechargeName = s.RechargeName,
                                BillName = s.BillName,
                                DMR_SLAB_Name = s.DMR_SLAB_Name,
                                AIR_SLAB_Name = s.AIR_SLAB_Name,
                                BUS_SLAB_Name = s.BUS_SLAB_Name,
                                HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                            }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- GetListInformation (POST) Line No:- 414", ex);
                throw;
            }
           
        }

        [HttpPost]
        public JsonResult GetMemberListInformation(string Mem_Id)
        {
            try
            {
                long mem_idval = long.Parse(Mem_Id);
                var db = new DBContext();
                var list = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                            where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID == mem_idval
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                            {
                                SL_NO = s.ID,
                                WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                INTRODUCER_ID = s.INTRODUCER_ID,
                                RechargeName = s.RechargeName,
                                BillName = s.BillName,
                                DMR_SLAB_Name = s.DMR_SLAB_Name,
                                AIR_SLAB_Name = s.AIR_SLAB_Name,
                                BUS_SLAB_Name = s.BUS_SLAB_Name,
                                HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                            }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- GetMemberListInformation (POST) Line No:- 462", ex);
                throw;
            }

        }
        [HttpPost]
        public JsonResult fetchMemCommInfo(string Mem_Id, string WhitelevelId)
        {
            try
            {
                long mem_idval = long.Parse(Mem_Id);
                long WhiteLevelID = long.Parse(WhitelevelId);
                var db = new DBContext();
                var listinfo = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID == WhiteLevelID && valid.SL_NO == mem_idval
                                select new
                                {
                                    ID = valid.SL_NO,
                                    WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                    WHITELEVELNAME1 = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.DOMAIN).FirstOrDefault(),
                                    INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                    INTRODUCER_ID = valid.INTRODUCER_ID,
                                    RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    MobileRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    UtilityRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    DMRRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    AIRSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    BusSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    HotelSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    CashCardSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    MEM_NAME = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => (z.MEM_UNIQUE_ID + "-" + z.COMPANY)).FirstOrDefault(),

                                }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                                {
                                    SL_NO = s.ID,
                                    WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                    WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                    INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                    INTRODUCER_ID = s.INTRODUCER_ID,
                                    MobileRechargeSlabdetails = s.MobileRechargeSlabdetails,
                                    UtilityRechargeSlabdetails = s.UtilityRechargeSlabdetails,
                                    DMRRechargeSlabdetails = s.DMRRechargeSlabdetails,
                                    AIRSlabdetailsList = s.AIRSlabdetailsList,
                                    BusSlabdetailsList = s.BusSlabdetailsList,
                                    HotelSlabdetailsList = s.HotelSlabdetailsList,
                                    CashCardSlabdetailsList = s.CashCardSlabdetailsList,
                                    RechargeName = s.RechargeName,
                                    BillName = s.BillName,
                                    DMR_SLAB_Name = s.DMR_SLAB_Name,
                                    AIR_SLAB_Name = s.AIR_SLAB_Name,
                                    BUS_SLAB_Name = s.BUS_SLAB_Name,
                                    HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                    CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name,
                                    MEM_NAme= s.MEM_NAME
                                }).FirstOrDefault();

                return Json(listinfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- fetchMemCommInfo (POST) Line No:- 526", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> BindCommissionSlabTagg()
        {
            try
            {
                var db = new DBContext();
                var Commlist =await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.ToListAsync();
                return Json(Commlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- BindCommissionSlabTagg (POST) Line No:- 544", ex);
                throw ex;
            }
        }


        public ActionResult BulkCommissionTagging()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        public JsonResult GetAllMerchantListInformation()
        {
            try
            {
                var db = new DBContext();

                var list = (from validComm in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB join MEm_Obj in db.TBL_MASTER_MEMBER
                            on validComm.INTRODUCER_ID equals MEm_Obj.MEM_ID into eGroup
                            from d in eGroup.DefaultIfEmpty()
                            select new
                            {
                                
                                DepartmentName = (d == null ? 0 : d.MEM_ID),
                            }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                            {
                                WHITE_LEVEL_ID=s.DepartmentName
                            }).ToList();

                var list_2 = (from MEm_Obj in db.TBL_MASTER_MEMBER
                              join validComm in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                            on MEm_Obj.MEM_ID  equals validComm.INTRODUCER_ID into eGroup
                            from d in eGroup.DefaultIfEmpty()
                            where MEm_Obj.INTRODUCER==MemberCurrentUser.MEM_ID && MEm_Obj.MEMBER_ROLE==5 && d.INTRODUCER_ID==null
                              select new
                            {
                                  CheckId=false,
                                  ID = (d.SL_NO == null ? 0 : d.SL_NO),
                                  WHITE_LEVEL_ID =(d.INTRODUCER_ID == null ? 0 : d.INTRODUCER_ID), 
                                  MEM_ID= MEm_Obj.MEM_ID,
                                  WHITELEVELNAME1 = MEm_Obj.MEMBER_NAME,
                                  INTRODUCE_TO_ID = (d.INTRODUCE_TO_ID == null ? 0 : d.INTRODUCE_TO_ID),                                  
                                  INTRODUCER_ID = (d.INTRODUCER_ID == null ? 0 : d.INTRODUCER_ID),                                  
                                  RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                  CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == d.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),

                                  //DepartmentName = (d == null ? 0 : d.SL_NO),
                                  //MEM_ID= MEm_Obj.MEM_ID,
                                  //MEM_NAME=MEm_Obj.MEMBER_NAME
                              }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                            {
                                  CheckId=s.CheckId,
                                  SL_NO = s.ID,
                                  WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                  MEM_ID=s.MEM_ID,
                                  WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                  INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                  INTRODUCER_ID = s.INTRODUCER_ID,
                                  RechargeName = s.RechargeName,
                                  BillName = s.BillName,
                                  DMR_SLAB_Name = s.DMR_SLAB_Name,
                                  AIR_SLAB_Name = s.AIR_SLAB_Name,
                                  BUS_SLAB_Name = s.BUS_SLAB_Name,
                                  HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                  CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                                  //  MEM_ID=s.MEM_ID,
                                  //  WHITELEVELNAME1=s.MEM_NAME,
                                  //WHITE_LEVEL_ID = s.DepartmentName
                              }).ToList();

                //var list = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                //            where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID != 0
                //            select new
                //            {
                //                ID = valid.SL_NO,
                //                WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                //                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                //                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                //                INTRODUCER_ID = valid.INTRODUCER_ID,
                //                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                //            }).AsEnumerable().Select(s => new DistributorCommissionSlabTaggingModelView
                //            {
                //                SL_NO = s.ID,
                //                WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                //                WHITELEVELNAME1 = s.WHITELEVELNAME1,
                //                INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                //                INTRODUCER_ID = s.INTRODUCER_ID,
                //                RechargeName = s.RechargeName,
                //                BillName = s.BillName,
                //                DMR_SLAB_Name = s.DMR_SLAB_Name,
                //                AIR_SLAB_Name = s.AIR_SLAB_Name,
                //                BUS_SLAB_Name = s.BUS_SLAB_Name,
                //                HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                //                CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                //            }).ToList();
                return Json(list_2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorCommissionTag(Distributor), method:- GetListInformation (POST) Line No:- 414", ex);
                throw;
            }

        }

        public ActionResult BulkMerchantCommissionTagging()
        {
            if (Session["DistributorUserId"] != null)
            {
                var db = new DBContext();             

                var memberService = (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                     where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1 && whitelebelslab.SLAB_STATUS == true
                                     select new
                                     {
                                         SLN = whitelebelslab.SLN,
                                         SLAB_NAME = whitelebelslab.SLAB_NAME
                                     }).ToList().Distinct();
                ViewBag.MobileSlab = new SelectList(memberService, "SLN", "SLAB_NAME");

                var UtilitySlab = (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                     where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2 && whitelebelslab.SLAB_STATUS == true
                                     select new
                                     {
                                         SLN = whitelebelslab.SLN,
                                         SLAB_NAME = whitelebelslab.SLAB_NAME
                                     }).ToList().Distinct();
                ViewBag.UtilitySlab = new SelectList(UtilitySlab, "SLN", "SLAB_NAME");

                var DMRSLAB = (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                   where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3 && whitelebelslab.SLAB_STATUS == true
                                   select new
                                   {
                                       SLN = whitelebelslab.SLN,
                                       SLAB_NAME = whitelebelslab.SLAB_NAME
                                   }).ToList().Distinct();
                ViewBag.DMRSLAB = new SelectList(DMRSLAB, "SLN", "SLAB_NAME");

                //var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "RETAILER").ToListAsync();
                //ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> BulkMerchantCommissionTagging(HttpPostedFileBase file, DistributorCommissionSlabTaggingModelView objmodel)
        {
            if (Session["DistributorUserId"] != null)
            {
                TempData["msgVal"] = null;
                var db = new DBContext();
                var db_Val = new DBContext();              
                try
                {
                    if (file.ContentLength > 0)
                    {
                        string fileName = file.FileName;
                        string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                        if (FileExtension == "csv")
                        {
                            string File_NameValue = Path.GetFileName(file.FileName);
                            string file_path_Value = Server.MapPath("~/WhiteLabelLogo") + "//" + file.FileName;
                            if (System.IO.File.Exists(file_path_Value))
                            {
                                System.IO.File.Delete(file_path_Value);
                            }
                            file.SaveAs(file_path_Value);

                            /* -- Import CSV Code Start -- */
                            long Id_Val = 0;
                            int count = 0;
                            string BookingfileName = Server.MapPath("~/WhiteLabelLogo") + "//" + file.FileName;
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
                                string Unique_Id = dtCSV.Rows[i][0].ToString();
                                var Get_MemberID = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_UNIQUE_ID == Unique_Id).MEM_ID;
                                if (Get_MemberID != null)
                                {
                                    var checkValComm = await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == Get_MemberID).FirstOrDefaultAsync();
                                    if (checkValComm != null)
                                    {
                                        checkValComm.RECHARGE_SLAB = objmodel.RECHARGE_SLAB;
                                        checkValComm.BILLPAYMENT_SLAB = objmodel.BILLPAYMENT_SLAB;
                                        checkValComm.DMR_SLAB = 0;
                                        checkValComm.AIR_SLAB = 0;
                                        checkValComm.BUS_SLAB = 0;
                                        checkValComm.HOTEL_SLAB = 0;
                                        checkValComm.CASHCARD_SLAB = 0;
                                        db.Entry(checkValComm).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        TBL_DETAILS_MEMBER_COMMISSION_SLAB objmodel_val = new TBL_DETAILS_MEMBER_COMMISSION_SLAB()
                                        {
                                            WHITE_LEVEL_ID = MemberCurrentUser.MEM_ID,
                                            INTRODUCER_ID = Get_MemberID,
                                            INTRODUCE_TO_ID = 0,
                                            RECHARGE_SLAB = objmodel.RECHARGE_SLAB,
                                            BILLPAYMENT_SLAB = objmodel.BILLPAYMENT_SLAB,
                                            DMR_SLAB = 0,
                                            AIR_SLAB = 0,
                                            BUS_SLAB = 0,
                                            HOTEL_SLAB = 0,
                                            CASHCARD_SLAB = 0,
                                            STATUS = true,
                                            CREATED_DATE = DateTime.Now
                                        };
                                        db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Add(objmodel_val);
                                        db.SaveChanges();
                                    }
                                }
                                count++;
                            }
                            TempData["msgVal"] = "Commission Tagged Successfully!!";
                        }
                        else
                        {
                            TempData["msgVal"] = "Please Upload Only CSV File !!";
                            //return View("BulkMerchantCommissionTagging");
                            return RedirectToAction("BulkMerchantCommissionTagging", "DistributorCommissionTag");
                        }
                    }
                    else
                    {
                        TempData["msgVal"] = "Please Upload Member Id files!!";
                        return View("BulkMerchantCommissionTagging");
                    }
                    //return View("GetAllRDSCancellationList");
                    return RedirectToAction("BulkMerchantCommissionTagging", "DistributorCommissionTag");
                }
                catch (Exception ex)
                {
                    //ContextTransaction.Rollback();
                    TempData["msgVal"] = "Member Id files upload failed!!";
                    //ViewBag.Message = "File upload failed!!";
                    //Session["msgVal"] =  "File upload failed!!";
                    return View("BulkMerchantCommissionTagging");
                }              
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }


        public FileResult ExportCSVFile()
        {
            // here I open a file, write the info in, then clsoe the file; and this works, becaise I do have the correct file save on local drive.
            string listFile = Server.MapPath("~/WhiteLabelLogo/CommissionTagg.csv");
            string contentType = "text/CSV"; // I tried "application/text" as well, same result.
            return new FilePathResult(listFile, contentType);
        }
        public FileResult downloadfiles(string type, string memid)
        {
            try
            {
                var db = new DBContext();
               
                string filepath = string.Empty;
                string fileNameinfo = string.Empty;
                

                //string contentType = "application/pdf";
                string contentType = string.Empty;
                string listFile = Server.MapPath("~/WhiteLabelLogo/CommissionTagg.csv");
                filepath = listFile;
                string path = listFile;
                string fileName = path.Substring(path.LastIndexOf(((char)92)) + 1);
                int index = fileName.LastIndexOf('.');
                string onyName = fileName.Substring(0, index);
                string fileExtension = fileName.Substring(index + 1);
                if (fileExtension == "csv")
                {
                    //fileNameinfo = fileNameinfo + "." + fileExtension;
                    fileNameinfo = fileName;
                    contentType = "Images/png";
                }
                return File(filepath, contentType, fileNameinfo);
                //string filename = (from f in files
                //                   where f.FileId == fid
                //                   select f.FilePath).First();
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  DistributorKYC(Distributor), method:- downloadfiles (File) Line No:- 158", ex);
                throw;
            }
        }


        [HttpPost]
        public JsonResult getAllMembersList()
        {
            using (var db = new DBContext())
            {
                var countrylist = db.TBL_MASTER_MEMBER.Select(z=>new
                {
                    MEM_ID = z.MEM_ID,
                    COMPANY = z.MEM_UNIQUE_ID+"-"+z.COMPANY
                }).ToList();
                return new JsonResult
                {
                    Data = countrylist,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

    }
}
using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberRailIdApplicationController : AdminBaseController
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
                    Response.Redirect(Url.Action("Logout", "AdminLogin", new { area = "Admin" }));
                    //Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/MemberRailIdApplicationm
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var MemberRailIDQty = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).RAIL_ID_QUANTITY;
                ViewBag.RailIDQty = MemberRailIDQty;
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
                return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;

                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.MEMBER_ROLE == 4 && oper.INTRODUCER == MemberCurrentUser.MEM_ID
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.MEM_ID,
                                               val = oper.MEM_ID
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> POSTSellRailId(TBL_RAIL_ID_ALLOCATION objRail)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal MainBalance = 0;
                    int RailQty = 0;
                    decimal RailSellRate = 0;
                    decimal TotalSellingAmt = 0;
                    decimal GST_AMount = 0;
                    decimal GrossSellingAMT = 0;
                    decimal DeductMainBalance = 0;
                    decimal SGST = 0;
                    decimal CGST = 0;
                    decimal Total_GST = 0;
                    decimal IGST = 0;
                    int WLPRailIdQTYUpdate = 0;
                    int WLPRailIdQTY = 0;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    int.TryParse(objRail.RAIL_ID_QUANTITY.ToString(), out RailQty);

                    // Add RailIdQty From Distributor
                    var WhitelabelInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                    if (WhitelabelInfo.RAIL_ID_QUANTITY >= RailQty)
                    {
                        var WLP_Info = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == objRail.BUYER_ID);
                        if (WLP_Info.BALANCE >= objRail.GROSS_AMOUNT)
                        {
                            var GSTVAl = await db.TBL_STATES.FirstOrDefaultAsync(x => x.STATEID == WLP_Info.STATE_ID);
                            if (GSTVAl != null)
                            {
                                decimal.TryParse(GSTVAl.SGST.ToString(), out SGST);
                                decimal.TryParse(GSTVAl.CGST.ToString(), out CGST);
                                decimal.TryParse(GSTVAl.IGST.ToString(), out IGST);
                                Total_GST = SGST + CGST + IGST;
                            }
                            decimal.TryParse(WLP_Info.BALANCE.ToString(), out MainBalance);
                            int.TryParse(WLP_Info.RAIL_ID_QUANTITY.ToString(), out WLPRailIdQTY);

                            decimal.TryParse(objRail.RAIL_ID_SELLING_RATE.ToString(), out RailSellRate);
                            // WLP Rail Qty Update and Main Balance Update
                            WLPRailIdQTYUpdate = WLPRailIdQTY + RailQty;
                            TotalSellingAmt = RailSellRate * RailQty;
                            if (objRail.GSTApplied == "Yes")
                            {
                                GST_AMount = ((TotalSellingAmt * Total_GST) / 100);
                            }
                            else
                            {
                                GST_AMount = 0;
                            }
                            GrossSellingAMT = TotalSellingAmt + GST_AMount;
                            DeductMainBalance = MainBalance - GrossSellingAMT;
                            WLP_Info.BALANCE = DeductMainBalance;
                            WLP_Info.RAIL_ID_QUANTITY = WLPRailIdQTYUpdate;
                            db.Entry(WLP_Info).State = System.Data.Entity.EntityState.Modified;
                           // await db.SaveChangesAsync();

                            TBL_RAIL_ID_ALLOCATION sellRail = new TBL_RAIL_ID_ALLOCATION()
                            {
                                SELLER_ID = MemberCurrentUser.MEM_ID,
                                BUYER_ID = objRail.BUYER_ID,
                                RAIL_ID_QUANTITY = RailQty,
                                RAIL_ID_SELLING_RATE = RailSellRate,
                                GROSS_AMOUNT = GrossSellingAMT,
                                GST_RATE = Total_GST,
                                GST_AMOUNT = GST_AMount,
                                SALE_DATE = DateTime.Now,
                                STATUS = true,
                                CORELATIONID = COrelationID,
                                SYSTEM_DATE = DateTime.Now
                            };
                            db.TBL_RAIL_ID_ALLOCATION.Add(sellRail);
                            //await db.SaveChangesAsync();
                            decimal Opening_amt = 0;
                            decimal Closing_amt = 0;
                            decimal Trans_amt = 0;

                            decimal AddAmount = 0;
                            decimal AddOpening_amt = 0;
                            decimal AddClosing_amt = 0;
                            var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == objRail.BUYER_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                            if (amtobj != null)
                            {
                                Opening_amt = amtobj.OPENING;
                                Closing_amt = amtobj.CLOSING;
                                AddAmount = Closing_amt - GrossSellingAMT;

                            }
                            else
                            {
                                AddAmount = DeductMainBalance;
                            }
                            TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = objRail.BUYER_ID,
                                MEMBER_TYPE = "DISTRIBUTOR",
                                TRANSACTION_TYPE = "RAILID SELLING",
                                TRANSACTION_DATE = Convert.ToDateTime(DateTime.Now),
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = GrossSellingAMT,
                                NARRATION = "RAILID SELLING",
                                OPENING = Closing_amt,
                                CLOSING = AddAmount,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST_PERCENTAGE = Total_GST,
                                GST= (double)GST_AMount,
                                TDS = 0,
                                IPAddress = "",
                                SERVICE_ID = 0,
                                CORELATIONID = COrelationID
                            };
                            db.TBL_ACCOUNTS.Add(objacnt);
                            //await db.SaveChangesAsync();
                            //// Subustract RailIdQty From White Label
                            int WhitelabelRailTqy = 0;
                            int WhitelabelSubRailTqy = 0;
                            decimal WhitelabelBalance = 0;
                            decimal WhitelabelAddBalance = 0;
                            //var WhitelabelInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                            int.TryParse(WhitelabelInfo.RAIL_ID_QUANTITY.ToString(), out WhitelabelRailTqy);
                            decimal.TryParse(WhitelabelInfo.BALANCE.ToString(), out WhitelabelBalance);
                            WhitelabelSubRailTqy = WhitelabelRailTqy - RailQty;
                            WhitelabelAddBalance = WhitelabelBalance + GrossSellingAMT;
                            WhitelabelInfo.BALANCE = WhitelabelAddBalance;
                            WhitelabelInfo.RAIL_ID_QUANTITY = WhitelabelSubRailTqy;
                            db.Entry(WhitelabelInfo).State = System.Data.Entity.EntityState.Modified;
                            //await db.SaveChangesAsync();

                            decimal WLPP_ClosingAmount = 0;
                            decimal WLPP_AddClosingClosingAmount = 0;
                            var WLLPAccnt = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                            if (WLLPAccnt != null)
                            {
                                decimal.TryParse(WLLPAccnt.CLOSING.ToString(), out WLPP_ClosingAmount);
                                WLPP_AddClosingClosingAmount = WLPP_ClosingAmount + GrossSellingAMT;
                            }
                            else
                            {
                                WLPP_AddClosingClosingAmount = GrossSellingAMT;
                            }

                            TBL_ACCOUNTS WLPobjacnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = MemberCurrentUser.MEM_ID,
                                MEMBER_TYPE = "WHITELABEL",
                                TRANSACTION_TYPE = "RAILID SELLING",
                                TRANSACTION_DATE = Convert.ToDateTime(DateTime.Now),
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = GrossSellingAMT,
                                NARRATION = "RAILID SELLING",
                                OPENING = WLPP_ClosingAmount,
                                CLOSING = WLPP_AddClosingClosingAmount,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = "",
                                SERVICE_ID = 0,
                                CORELATIONID = COrelationID
                            };
                            db.TBL_ACCOUNTS.Add(WLPobjacnt);
                            await db.SaveChangesAsync();
                            ContextTransaction.Commit();
                            return Json("Rail Id Sell to Distributor", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("There is no suffcient Balance in Distributor for selling of rail Id. ", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("There is no suffcient Rail Id for sell.Please Purchase Rail Id From Power Admin ", JsonRequestBehavior.AllowGet);
                    }
                    


                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberChannelRegistration(Admin), method:- ADDSUPERDISTRIBUTOR (POST) Line No:- 230", ex);
                    throw ex;
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }

        public ActionResult DistributorRailIDAllocationList()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();

                var db = new DBContext();
                var GetRailIdQty = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).RAIL_ID_QUANTITY;
                ViewBag.RailQTY = GetRailIdQty;
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
                return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                //var memberinfo = db.TBL_RAIL_ID_ALLOCATION.Where(x => x.SELLER_ID == 0).ToList();

                var transactionlist = (from x in db.TBL_RAIL_ID_ALLOCATION
                                       join y in db.TBL_MASTER_MEMBER on x.BUYER_ID equals y.MEM_ID
                                      
                                       where x.SELLER_ID == MemberCurrentUser.MEM_ID
                                       select new
                                       {
                                           Touser = "White Label",
                                           FromUser = y.UName + " - " + y.MEMBER_MOBILE,
                                           REQUEST_DATE = x.SALE_DATE,
                                           RailQTY = x.RAIL_ID_QUANTITY,
                                           SellingAmt = x.RAIL_ID_SELLING_RATE,
                                           GrossSellAmt = x.GROSS_AMOUNT,
                                           GSTRate = x.GST_RATE,
                                           SLN = x.ID,
                                           GST_Amount = x.GROSS_AMOUNT
                                       }).AsEnumerable().Select(z => new TBL_RAIL_ID_ALLOCATION
                                       {

                                           WLPUserName = z.FromUser,
                                           RAIL_ID_SELLING_RATE = z.SellingAmt,
                                           RAIL_ID_QUANTITY = z.RailQTY,
                                           GROSS_AMOUNT = z.GrossSellAmt,
                                           GST_RATE = z.GSTRate,
                                           GST_AMOUNT = z.GST_Amount,
                                           SALE_DATE = z.REQUEST_DATE,
                                           ID = z.SLN
                                       }).ToList();
                return PartialView("IndexGrid", transactionlist);
                //return PartialView("IndexGrid", memberinfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
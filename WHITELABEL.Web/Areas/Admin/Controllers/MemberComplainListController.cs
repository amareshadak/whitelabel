using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
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
    [Authorize]
    public class MemberComplainListController : AdminBaseController
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
                    Response.Redirect(Url.Action("Logout", "AdminLogin", new { area = "Admin" }));
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

        // GET: Admin/MemberComplainList
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
                return RedirectToAction("AdminLogin", "Login", new { area = "" });
            }
        }
        [HttpGet]
        public PartialViewResult IndexGrid(string MemberDetails = "" ,string DateFrom = "", string Date_To = "")
        {
            try
            {
                // Only grid query values will be available here.
                var db = new DBContext();
                if (MemberDetails != "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime valueFrom = Convert.ToDateTime(Date_To_Val);
                    DateTime ToDateVal = valueFrom.AddDays(1);
                    var ComplainList = (from x in db.TBL_MASTER_MEMBER
                                        join y in db.TBL_COMPLAIN_DETAILS on x.MEM_ID equals y.MEM_ID
                                        where y.COMPLAIN_DATE >= Date_From_Val && y.COMPLAIN_DATE <= ToDateVal && (x.MEMBER_NAME.StartsWith(MemberDetails) || x.MEM_UNIQUE_ID.StartsWith(MemberDetails) || x.MEMBER_MOBILE.StartsWith(MemberDetails) ||x.EMAIL_ID.StartsWith(MemberDetails) || x.COMPANY.StartsWith(MemberDetails) || x.COMPANY_GST_NO.StartsWith(MemberDetails) || y.COMPLAIN_TYPE.StartsWith(MemberDetails) || y.COMPLAIN_DETAILS.StartsWith(MemberDetails))
                                        select new
                                        {
                                            SLN = y.SLN,
                                            MEM_NAME = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                            MEM_Comp = x.COMPANY,
                                            MEM_COMP_GST = x.COMPANY_GST_NO,
                                            COMPLAIN_TYPE = y.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = y.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = y.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = y.COMPLAIN_STATUS,
                                            REPLY_DETAILS = y.REPLY_DETAILS,
                                            REPLY_DATE = y.REPLY_DATE,
                                        }).AsEnumerable().Select((z, index) => new TBL_COMPLAIN_DETAILS
                                        {
                                            SLN = z.SLN,
                                            Serial_No = index + 1,
                                            Member_Company = z.MEM_Comp,
                                            Member_Name = z.MEM_NAME,
                                            Member_Company_GST = z.MEM_COMP_GST,
                                            COMPLAIN_TYPE = z.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = z.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = z.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = z.COMPLAIN_STATUS,
                                            REPLY_DATE = z.REPLY_DATE,
                                            REPLY_DETAILS = z.REPLY_DETAILS
                                        }).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }
                else if (MemberDetails != "" && DateFrom == "" && Date_To == "")
                {
                    var ComplainList = (from x in db.TBL_MASTER_MEMBER
                                        join y in db.TBL_COMPLAIN_DETAILS on x.MEM_ID equals y.MEM_ID where x.MEMBER_NAME.StartsWith(MemberDetails) || x.MEM_UNIQUE_ID.StartsWith(MemberDetails) || x.MEMBER_MOBILE.StartsWith(MemberDetails) || x.EMAIL_ID.StartsWith(MemberDetails) || x.COMPANY.StartsWith(MemberDetails) || x.COMPANY_GST_NO.StartsWith(MemberDetails) || y.COMPLAIN_TYPE.StartsWith(MemberDetails) || y.COMPLAIN_DETAILS.StartsWith(MemberDetails)
                                        select new
                                        {
                                            SLN = y.SLN,
                                            MEM_NAME = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                            MEM_Comp = x.COMPANY,
                                            MEM_COMP_GST = x.COMPANY_GST_NO,
                                            COMPLAIN_TYPE = y.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = y.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = y.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = y.COMPLAIN_STATUS,
                                            REPLY_DETAILS = y.REPLY_DETAILS,
                                            REPLY_DATE = y.REPLY_DATE,
                                        }).AsEnumerable().Select((z, index) => new TBL_COMPLAIN_DETAILS
                                        {
                                            SLN = z.SLN,
                                            Serial_No = index + 1,
                                            Member_Company = z.MEM_Comp,
                                            Member_Name = z.MEM_NAME,
                                            Member_Company_GST = z.MEM_COMP_GST,
                                            COMPLAIN_TYPE = z.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = z.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = z.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = z.COMPLAIN_STATUS,
                                            REPLY_DATE = z.REPLY_DATE,
                                            REPLY_DETAILS = z.REPLY_DETAILS
                                        }).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }
                else if (MemberDetails == "" && DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime valueFrom = Convert.ToDateTime(Date_To_Val);
                    DateTime ToDateVal = valueFrom.AddDays(1);
                    var ComplainList = (from x in db.TBL_MASTER_MEMBER
                                        join y in db.TBL_COMPLAIN_DETAILS on x.MEM_ID equals y.MEM_ID where y.COMPLAIN_DATE >= Date_From_Val && y.COMPLAIN_DATE <= ToDateVal
                                        select new
                                        {
                                            SLN = y.SLN,
                                            MEM_NAME = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                            MEM_Comp = x.COMPANY,
                                            MEM_COMP_GST = x.COMPANY_GST_NO,
                                            COMPLAIN_TYPE = y.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = y.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = y.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = y.COMPLAIN_STATUS,
                                            REPLY_DETAILS = y.REPLY_DETAILS,
                                            REPLY_DATE = y.REPLY_DATE,
                                        }).AsEnumerable().Select((z, index) => new TBL_COMPLAIN_DETAILS
                                        {
                                            SLN = z.SLN,
                                            Serial_No = index + 1,
                                            Member_Company = z.MEM_Comp,
                                            Member_Name = z.MEM_NAME,
                                            Member_Company_GST = z.MEM_COMP_GST,
                                            COMPLAIN_TYPE = z.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = z.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = z.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = z.COMPLAIN_STATUS,
                                            REPLY_DATE = z.REPLY_DATE,
                                            REPLY_DETAILS = z.REPLY_DETAILS
                                        }).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }
                else
                {
                    var ComplainList = (from x in db.TBL_MASTER_MEMBER
                                        join y in db.TBL_COMPLAIN_DETAILS on x.MEM_ID equals y.MEM_ID
                                        select new
                                        {
                                            SLN = y.SLN,
                                            MEM_NAME = x.MEM_UNIQUE_ID + "-" + x.MEMBER_NAME,
                                            MEM_Comp = x.COMPANY,
                                            MEM_COMP_GST = x.COMPANY_GST_NO,
                                            COMPLAIN_TYPE = y.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = y.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = y.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = y.COMPLAIN_STATUS,
                                            REPLY_DETAILS = y.REPLY_DETAILS,
                                            REPLY_DATE = y.REPLY_DATE,
                                        }).AsEnumerable().Select((z, index) => new TBL_COMPLAIN_DETAILS
                                        {
                                            SLN = z.SLN,
                                            Serial_No = index + 1,
                                            Member_Company = z.MEM_Comp,
                                            Member_Name = z.MEM_NAME,
                                            Member_Company_GST = z.MEM_COMP_GST,
                                            COMPLAIN_TYPE = z.COMPLAIN_TYPE,
                                            COMPLAIN_DETAILS = z.COMPLAIN_DETAILS,
                                            COMPLAIN_DATE = z.COMPLAIN_DATE,
                                            COMPLAIN_STATUS = z.COMPLAIN_STATUS,
                                            REPLY_DATE = z.REPLY_DATE,
                                            REPLY_DETAILS = z.REPLY_DETAILS
                                        }).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetComplainDetails(string id)
        {
            initpage();////
            try
            {
                EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long SlnId = long.Parse(id);
                var meminfo = db.TBL_COMPLAIN_DETAILS.FirstOrDefault(x => x.SLN == SlnId);
                if (meminfo != null)
                {
                    return Json(meminfo, JsonRequestBehavior.AllowGet);
                }
                else
                { return Json("Complain is not registered", JsonRequestBehavior.AllowGet); }
                
            }
            catch (Exception ex)
            {
                return Json("Please Try Again Later", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PostReplyComplainRegister(string ComplainId, string ReplyDetails)
        {
            try
            {
                var db = new DBContext();
                long complain_id = 0;
                long MEM_ReplyId = MemberCurrentUser.MEM_ID;
                long.TryParse(ComplainId,out complain_id);
                var getcomplain = db.TBL_COMPLAIN_DETAILS.FirstOrDefault(x => x.SLN == complain_id);
                if (getcomplain != null)
                {
                    getcomplain.REPLY_DETAILS = ReplyDetails;
                    getcomplain.REPLY_DATE=DateTime.Now;
                    getcomplain.REPLY_ID = MEM_ReplyId;
                    getcomplain.COMPLAIN_STATUS = false;
                    db.Entry(getcomplain).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Reply is given to this complain", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Complain is not registered", JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
                throw;
            }
        }
    }
}
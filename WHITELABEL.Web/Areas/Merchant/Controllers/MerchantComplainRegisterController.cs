using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using System.Web.Security;
using easebuzz_.net;
using System.Threading.Tasks;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantComplainRegisterController : MerchantBaseController
    {
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Merchant";
                if (Session["MerchantUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Merchant/MerchantComplainRegister
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();                
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpGet]
        public PartialViewResult IndexGrid(string DateFrom="",string Date_To="")
        {
            try
            {
                // Only grid query values will be available here.
                var db = new DBContext();
                if (DateFrom != "" && Date_To != "")
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
                    var ComplainList = db.TBL_COMPLAIN_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.COMPLAIN_DATE >= Date_From_Val && x.COMPLAIN_DATE <= ToDateVal).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }
                else
                {
                    var ComplainList = db.TBL_COMPLAIN_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
                    return PartialView("IndexGrid", ComplainList);
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public JsonResult PostComplainRegister(string ComplainType,string ComplainDetails, string CompId)
        {
            try
            {
                var db = new DBContext();
                long Comp_Id = 0;
                long.TryParse(CompId, out Comp_Id);
                var checkCompliant = db.TBL_COMPLAIN_DETAILS.FirstOrDefault(x => x.SLN == Comp_Id);
                if (checkCompliant != null)
                {
                    checkCompliant.COMPLAIN_TYPE = ComplainType;
                    checkCompliant.COMPLAIN_DETAILS = ComplainDetails;
                    checkCompliant.COMPLAIN_DATE = DateTime.Now;
                    db.Entry(checkCompliant).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    TBL_COMPLAIN_DETAILS objcomp = new TBL_COMPLAIN_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        DIST_ID = (long)CurrentMerchant.INTRODUCER,
                        WLP_ID = (long)CurrentMerchant.UNDER_WHITE_LEVEL,
                        COMPLAIN_TYPE = ComplainType,
                        COMPLAIN_DETAILS = ComplainDetails,
                        COMPLAIN_DATE = DateTime.Now,
                        COMPLAIN_STATUS = true
                    };
                    db.TBL_COMPLAIN_DETAILS.Add(objcomp);
                    db.SaveChanges();
                }
                
                return Json("Complain send to admin", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        [HttpPost]        
        public async Task<JsonResult> GetAdminReply(string id)
        {
            initpage();////
            try
            {
                EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long SlnId = long.Parse(id);
                var meminfo = db.TBL_COMPLAIN_DETAILS.FirstOrDefault(x => x.SLN == SlnId && x.MEM_ID == CurrentMerchant.MEM_ID);
                if (meminfo != null)
                {
                    //string decriptpass = Decrypt.DecryptMe(meminfo.User_pwd);
                    string ReplyMsg = "Reply:= " + meminfo.REPLY_DETAILS + ".\n" + Environment.NewLine + "<br />\n Reply Date:- " + meminfo.REPLY_DATE;
                    return Json(ReplyMsg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string ReplyMsg = "Admin can not reply till now";
                    return Json(ReplyMsg, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json("Please Try Again Later",JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetEditonComplein(string id)
        {
            initpage();////
            try
            {                
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
    }
}
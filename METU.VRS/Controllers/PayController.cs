using METU.VRS.Models;
using METU.VRS.Models.CT;
using METU.VRS.Services;
using METU.VRS.UI;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace METU.VRS.Controllers
{
    public class PayController : Base.ControllerBase
    {
        public PayController() : base()
        {
        }

        public ActionResult Index()
        {
            Trace.WriteLine("GET /Pay/Index");
            return RedirectToAction("Index", "Sticker");
        }

        [HttpGet]
        public ActionResult Index(int? Id)
        {
            Trace.WriteLine("GET /Pay/Index/" + Id.ToString());
            if (null == Id)
            {
                return RedirectToAction("Index", "Sticker");
            }

            using (DatabaseContext db = GetNewDBContext())
            {
                var application = db.StickerApplications
                    .Include(a => a.Owner)
                    .Include(a => a.Payment)
                    .Include(a => a.Quota.Term)
                    .Include(a => a.Quota.Type)
                    .Include(a => a.Sticker)
                    .Include(a => a.User.Division)
                    .Include(a => a.User.Category)
                    .Include(a => a.Vehicle)
                    .Where(a => a.ID == Id)
                    .FirstOrDefault();

                if (application == null)
                {
                    return new HttpNotFoundResult();
                }

                if (application.User.ID != ((METUPrincipal)User).User.ID)
                {
                    return new HttpUnauthorizedResult();
                }

                return View(new PaymentRequest(application));
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Ok(PaymentResponseSuccess resp)
        {
            Trace.WriteLine("POST /Pay/Ok");
            using (DatabaseContext db = GetNewDBContext())
            {
                int id = int.Parse(resp.ApplicationId);

                if (id == 0)
                {
                    return new HttpUnauthorizedResult();
                }

                var application = db.StickerApplications
                    .Include(a => a.Owner)
                    .Include(a => a.Payment)
                    .Include(a => a.Quota.Term)
                    .Include(a => a.Quota.Type)
                    .Include(a => a.Sticker)
                    .Include(a => a.User.Division)
                    .Include(a => a.User.Category)
                    .Include(a => a.Vehicle)
                    .Where(a => a.ID == id)
                    .FirstOrDefault();

                if (application == null)
                {
                    return new HttpNotFoundResult();
                }

                var p = new Payment()
                {
                    Amount = resp.amount,
                    Application = application,
                    TransactionDate = DateTime.Now,
                    TransactionNumber = resp.TransId
                };

                db.Payments.Add(p);
                if (db.SaveChanges() == 0)
                {
                    throw new InvalidOperationException("Payment could not be saved!!");
                }

                application.Status = Models.StickerApplicationStatus.WaitingForDelivery;
                application.LastModified = DateTime.Now;
                if (db.SaveChanges() == 0)
                {
                    throw new InvalidOperationException("Application could not be updated!!");
                }
                return RedirectToAction("Index", "Sticker", new { paymentok = "1" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Fail(PaymentResponseFail resp)
        {
            Trace.WriteLine("POST /Pay/Fail");
            if (resp != null && resp.ProcReturnCode == "51")
            {
                return RedirectToAction("Index", "Pay", new { Id = resp.ApplicationId, insufficientFunds = "1" });
            }
            else if (resp != null)
            {
                return RedirectToAction("Index", "Pay", new { Id = resp.ApplicationId, hasError = "1" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
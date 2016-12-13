using Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Shift;
using Shift.Entities;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MyApp.Client.Controllers
{
    public class RTProgressController : Controller
    {
        private static JobClient jobClient;

        public RTProgressController(HttpContextBase httpContext)
        {
            if (jobClient == null)
            {
                jobClient = httpContext.Application["Shift.JobClient"] as JobClient;
            }
        }

        // GET: RTProgress
        public ActionResult Index(int jobID)
        {
            ViewBag.JobID = jobID;
            return View();
        }

        public ActionResult GetProgress(int jobID)
        {
            var jsProgress = jobClient.GetProgress(jobID);

            return Json(jsProgress, JsonRequestBehavior.AllowGet);
        }
    }
}
using System.Web;
using System.Web.Mvc;

using Shift;

namespace MyApp.Client.Controllers
{
    public class MethodController : Controller
    {
        private static JobClient jobClient;

        public MethodController(HttpContextBase httpContext)
        {
            if (jobClient == null)
            {
                jobClient = httpContext.Application["Shift.JobClient"] as JobClient;
            }
        }

        public ActionResult Index(int jobID)
        {
            ViewBag.JobView = jobClient.GetJobView(jobID);

            return View();
        }
    }
}
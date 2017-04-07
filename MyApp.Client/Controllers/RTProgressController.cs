using Shift;
using System.Threading.Tasks;
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
        public ActionResult Index(string jobID)
        {
            ViewBag.JobID = jobID;
            return View();
        }

        public async Task<ActionResult> GetProgress(string jobID)
        {
            var jsProgress = await jobClient.GetProgressAsync(jobID).ConfigureAwait(false);

            return Json(jsProgress, JsonRequestBehavior.AllowGet);
        }
    }
}
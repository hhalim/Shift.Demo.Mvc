using System.Web;
using System.Web.Mvc;

using Shift;
using System.Threading.Tasks;

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

        public async Task<ActionResult> Index(string jobID)
        {
            ViewBag.JobView = await jobClient.GetJobViewAsync(jobID);

            return View();
        }
    }
}
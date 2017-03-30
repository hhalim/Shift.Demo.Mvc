using System.Web;
using System.Web.Mvc;

using Shift;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MyApp.Client.Controllers
{
    public class StatusController : Controller
    {
        private static JobClient jobClient;

        public StatusController(HttpContextBase httpContext)
        {
            if (jobClient == null)
            {
                jobClient = httpContext.Application["Shift.JobClient"] as JobClient;
            }

        }

        public async Task<ActionResult> StatusCount(string appID, string userID)
        {
            var output = await jobClient.GetJobStatusCountAsync(appID, userID);
            ViewBag.Result = JsonConvert.SerializeObject(output);
            ViewBag.ApplicationID = ConfigurationManager.AppSettings["ApplicationID"];
            return View("Index");
        }

        public ActionResult Index()
        {
            ViewBag.ApplicationID = ConfigurationManager.AppSettings["ApplicationID"];

            return View();
        }
    }
}
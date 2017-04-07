using Shift;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyApp.Client.Controllers
{
    public class ProgressController : Controller
    {
        private static JobClient jobClient;

        public ProgressController(HttpContextBase httpContext)
        {
            if (jobClient == null)
            {
                jobClient = httpContext.Application["Shift.JobClient"] as JobClient;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ReadData(int? pageIndex, int? pageSize)
        {
            var jobViewList = await jobClient.GetJobViewsAsync(pageIndex, pageSize).ConfigureAwait(false);
            var output = new Dictionary<string, object>();
            output.Add("data", jobViewList.Items);
            output.Add("itemsCount", jobViewList.Total);

            return Json(output, JsonRequestBehavior.AllowGet);
        }


    }
}
using Shift;
using System.Collections.Generic;
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

        public ActionResult ReadData(int? pageIndex, int? pageSize)
        {
            var jobViewList = jobClient.GetJobViews(pageIndex, pageSize);
            var output = new Dictionary<string, object>();
            output.Add("data", jobViewList.Items);
            output.Add("itemsCount", jobViewList.Total);

            return Json(output, JsonRequestBehavior.AllowGet);
        }


    }
}
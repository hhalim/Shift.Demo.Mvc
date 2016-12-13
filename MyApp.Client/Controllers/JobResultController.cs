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
    public class JobResultController : Controller
    {
        private static JobClient jobClient;

        public JobResultController(HttpContextBase httpContext)
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

        public ActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = null;
            using (var db = new BGProcess(DBConstant.ConnectionName))
            {
                IQueryable<JobResult> query = from p in db.JobResult
                                              select p;
                result = query.ToDataSourceResult(request);
                foreach(JobResult row in result.Data)
                {
                    row.BinaryContent = null; //don't want to send this to browser
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContent(string externalID)
        {
            var jobResult = jobClient.GetJobResult(externalID);
            if(jobResult != null)
            {
                return File(jobResult.BinaryContent, jobResult.ContentType, jobResult.Name);
            }

            return null;
        }
    }
}
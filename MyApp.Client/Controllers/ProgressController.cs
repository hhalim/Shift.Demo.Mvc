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

        public ActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {

            DataSourceResult result = null;
            using (var db = new BGProcess(DBConstant.ConnectionName))
            {
                IQueryable<JobView> query = from p in db.JobView
                                        select p;
                result = query.ToDataSourceResult(request);
            }

            //Merge the Cached progress with the data in DB
            foreach (JobView row in result.Data)
            {
                if (row.Status == JobStatus.Running)
                {
                    var cached = jobClient.GetCachedProgress(row.JobID);
                    if (cached != null)
                    {
                        row.Status = cached.Status;
                        row.Percent = cached.Percent;
                        row.Note = cached.Note;
                        row.Data = cached.Data;
                        row.Error = cached.Error;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
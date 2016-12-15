using Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Shift;
using Shift.Entities;
using System.Collections.Generic;
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

        public ActionResult ReadData(int? pageIndex, int? pageSize)
        {
            var result = new List<JobResult>();
            var totalCount = 0;
            using (var db = new BGProcess(DBConstant.ConnectionName))
            {
                totalCount = (from p in db.JobResult
                              select p.JobResultID).Count();
                IQueryable<JobResult> query = (from p in db.JobResult
                                               orderby p.JobResultID ascending
                                               select p)
                                                .Skip((pageIndex.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault())
                                                .Take(pageSize.GetValueOrDefault());
                result = query.ToList();
                foreach (JobResult row in result)
                {
                    row.BinaryContent = null; //don't want to send this to browser
                }
            }

            var output = new Dictionary<string, object>();
            output.Add("data", result);
            output.Add("itemsCount", totalCount);
            return Json(output, JsonRequestBehavior.AllowGet);
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
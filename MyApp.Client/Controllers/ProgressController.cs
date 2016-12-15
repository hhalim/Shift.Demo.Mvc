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
            IQueryable<JobView> query;
            var result = new List<JobView>();
            var totalCount = 0;
            using (var db = new BGProcess(DBConstant.ConnectionName))
            {
                totalCount = (from p in db.JobView
                              orderby p.AppID descending
                              select p.JobID).Count();
                query = (from p in db.JobView
                         orderby p.AppID descending
                         select p)
                        .Skip((pageIndex.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault())
                        .Take(pageSize.GetValueOrDefault())
                        ;

                result = query.ToList();
            }

            //Merge the Cached progress with the data in DB
            foreach (JobView row in result)
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

            var output = new Dictionary<string, object>();
            output.Add("data", result);
            output.Add("itemsCount", totalCount);

            return Json(output, JsonRequestBehavior.AllowGet);
        }


    }
}
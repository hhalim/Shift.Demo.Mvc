using Global;
using Shift;
using Shift.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

using Dapper;

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
            var result = new List<JobView>();
            var totalCount = 0;

            using (var connection = new SqlConnection(DBConstant.ConnectionString))
            {
                connection.Open();
                var offset = (pageIndex.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault();
                var sqlQuery = @"SELECT COUNT(JobID) FROM JobView;
                                SELECT * 
                                FROM JobView jv 
                                ORDER BY jv.Created, jv.JobID
                                OFFSET " + offset + " ROWS FETCH NEXT " + pageSize.GetValueOrDefault() + " ROWS ONLY;";
                using (var multiResult = connection.QueryMultiple(sqlQuery))
                {
                    totalCount = multiResult.Read<int>().Single();
                    result = multiResult.Read<JobView>().ToList();
                }
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
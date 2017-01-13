using Shift;
using Shift.Entities;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Dapper;
using Global;
using System.Linq;

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

            using (var connection = new SqlConnection(DBConstant.ConnectionString))
            {
                connection.Open();
                var offset = (pageIndex.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault();
                var sqlQuery = @"SELECT COUNT(JobResultID) FROM JobResult;
                                SELECT JobResultID, JobID, ExternalID, Name, ContentType FROM JobResult ORDER BY JobResultID ASC
                                OFFSET " + offset + " ROWS FETCH NEXT " + pageSize.GetValueOrDefault() + " ROWS ONLY;";
                using (var multiResult = connection.QueryMultiple(sqlQuery))
                {
                    totalCount = multiResult.Read<int>().Single();
                    result = multiResult.Read<JobResult>().ToList();
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
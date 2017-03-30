using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Shift;
using System.Threading.Tasks;

namespace MyApp.Client.Controllers
{
    public class DashboardController : Controller
    {
        private static JobClient jobClient;
        private static JobServer jobServer;

        public DashboardController (HttpContextBase httpContext)
        {
            if (jobClient == null )
            {
                jobClient = httpContext.Application["Shift.JobClient"] as JobClient;
            }
            if(jobServer == null)
            {
                jobServer = httpContext.Application["Shift.JobServer"] as JobServer;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ReadData(int? pageIndex, int? pageSize)
        {
            var jobViewList = await jobClient.GetJobViewsAsync(pageIndex, pageSize);
            var output = new Dictionary<string, object>();
            output.Add("data", jobViewList.Items);
            output.Add("itemsCount", jobViewList.Total);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Delete jobs that's not running
            await jobClient.DeleteJobsAsync(ids);

            return Json(true);
        }

        public async Task<ActionResult> Reset(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Reset jobs that's not running
            await jobClient.ResetJobsAsync(ids);

            return Json(true);
        }

        #region Shift Actions
        public async Task<ActionResult> Stop(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            await jobClient.SetCommandStopAsync(ids);

            return Json(true);
        }

        public async Task<ActionResult> RunNow(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to 'run-now', wait for RunServer to pickup and run it
            await jobClient.SetCommandRunNowAsync(ids);

            return Json(true);
        }

        public ActionResult RunSelected(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            jobServer.RunJobs(ids);

            return Json(true);
        }

        //public ActionResult RunServer()
        //{
        //    //Jobs running through this function will be running under the IIS Process!
        //    jobServer.RunServer(); //Run jobs server, use the MaxRunableJobs setting
        //    return Json(true);
        //}

        public async Task<ActionResult> RunServer()
        {
            //Jobs running through this function will be running under the IIS Process!
            await jobServer.RunServerAsync(); //Run jobs server, use the MaxRunableJobs setting
            return Json(true);
        }

        public async Task<ActionResult> StopServer()
        {
            await jobServer.StopServerAsync(); 

            return Json(true);
        }

        public async Task<ActionResult> CleanUp()
        {
            await jobServer.CleanUpAsync(); 

            return Json(true);
        }

        #endregion
    }
}
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
            var jobViewList = await jobClient.GetJobViewsAsync(pageIndex, pageSize).ConfigureAwait(false);
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
            await jobClient.DeleteJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<ActionResult> Reset(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Reset jobs that's not running
            await jobClient.ResetJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        #region Shift Actions
        public async Task<ActionResult> Stop(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            await jobClient.SetCommandStopAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<ActionResult> RunNow(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to 'run-now', wait for RunServer to pickup and run it
            await jobClient.SetCommandRunNowAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<ActionResult> RunSelected(List<string> ids)
        {
            if (ids == null)
                return Json(false);

            await jobServer.RunJobsAsync(ids).ConfigureAwait(false);

            return Json(true);
        }

        public async Task<ActionResult> RunServer()
        {
            //Jobs running through this function will be running under the IIS Process!
            await jobServer.RunServerAsync().ConfigureAwait(false); //Run jobs server, use the MaxRunableJobs setting
            return Json(true);
        }

        public async Task<ActionResult> StopServer()
        {
            await jobServer.StopServerAsync().ConfigureAwait(false);

            return Json(true);
        }

        public async Task<ActionResult> CleanUp()
        {
            await jobServer.CleanUpAsync().ConfigureAwait(false);

            return Json(true);
        }
        #endregion
    }
}
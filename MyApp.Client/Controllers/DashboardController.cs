using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

using Shift;
using Global;
using Shift.Entities;
using Dapper;
using System;

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

        public ActionResult ReadData(int? pageIndex, int? pageSize)
        {
            var jobViewList = jobClient.GetJobViews(pageIndex, pageSize);
            var output = new Dictionary<string, object>();
            output.Add("data", jobViewList.Items);
            output.Add("itemsCount", jobViewList.Total);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(List<int> ids)
        {
            if (ids == null)
                return Json(false);

            //Delete jobs that's not running
            jobClient.DeleteJobs(ids);

            return Json(true);
        }

        public ActionResult Reset(List<int> ids)
        {
            if (ids == null)
                return Json(false);

            //Reset jobs that's not running
            jobClient.ResetJobs(ids);

            return Json(true);
        }

        #region Shift Actions
        public ActionResult Stop(List<int> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to stop
            jobClient.SetCommandStop(ids);

            return Json(true);
        }

        public ActionResult RunNow(List<int> ids)
        {
            if (ids == null)
                return Json(false);

            //Set command to 'run-now', wait for RunServer to pickup and run it
            jobClient.SetCommandRunNow(ids);

            return Json(true);
        }

        public ActionResult RunServer()
        {
            //Jobs run through this function will be running under the IIS Process!
            jobServer.RunServer(); //Run jobs server, use the MaxRunableJobs setting
            return Json(true);
        }

        public ActionResult StopServer()
        {
            jobServer.StopServer(); 

            return Json(true);
        }

        public ActionResult CleanUp()
        {
            jobServer.CleanUp(); 

            return Json(true);
        }

        #endregion
    }
}
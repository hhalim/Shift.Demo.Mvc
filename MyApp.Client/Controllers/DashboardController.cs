﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MyApp.DataLayer;
using Shift;
using System.Configuration;
using Global;
using Shift.Entities;

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
                if(row.Status == JobStatus.Running)
                {
                    var cached = jobClient.GetCachedProgress(row.JobID);
                    if(cached != null)
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

            //Server actually runs the Cancel jobs that's running or not started
            jobServer.StopJobs();

            return Json(true);
        }

        public ActionResult Start(List<int> ids)
        {
            if (ids == null)
                return Json(false);

            //Jobs run through this function will be running under the IIS Process!
            jobServer.StartJobs(ids); //Try to start the selected jobs, ignoring MaxRunableJobs 

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
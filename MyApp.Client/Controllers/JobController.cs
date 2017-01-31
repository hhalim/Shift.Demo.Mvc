using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyApp.DataLayer;
using MyApp.BusinessLayer;
using Global;
using Shift.Entities;
using Shift;
using System.Resources;
using System.Reflection;
using System.Configuration;

namespace MyApp.Client.Controllers
{
    public class JobController : Controller
    {
        private static JobClient jobClient;

        public JobController(HttpContextBase httpContext)
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

        public ActionResult Add(int? p0Count, int? p1Count, int? p2Count, int? p3Count, int? p4Count, int? p5Count)
        {
            var appID = ConfigurationManager.AppSettings["ApplicationID"];
            if (string.IsNullOrWhiteSpace(appID))
                appID = "/Software/BGTest";

            var message = "";
            if (p1Count > 0)
            {
                for (var i = 0; i < p1Count; i++)
                {
                    var job1 = new Job1();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    jobClient.Add(appID, () => job1.Start("Hello World!", progress));
                }

                message += p1Count + " - Job1 added to background server list. <br/>";
            }

            if (p2Count > 0)
            {
                for (var i = 0; i < p2Count; i++)
                {
                    var job2 = new Job2();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    jobClient.Add(appID, () => job2.Start(progress, 1));
                }

                message += p2Count + " - Job2 added to background server list. <br/>";
            }

            if (p3Count > 0)
            {
                var simpleList = new List<int>();
                for (var i = 1; i <= 100; i++)
                {
                    simpleList.Add(i);
                }

                for (var i = 0; i < p3Count; i++)
                {
                    var job3 = new Job3();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    jobClient.Add(appID, () => job3.Start(progress, simpleList));
                }

                message += p3Count + " - Job3 added to background server list. <br/>";
            }

            if (p4Count > 0)
            {
                var complexList = new List<TestData>();
                for (var i = 1; i <= 100; i++)
                {
                    var newData = new TestData();
                    newData.MyNumber = i;
                    newData.MyString = "mystring " + i;
                    complexList.Add(newData);
                }

                for (var i = 0; i < p4Count; i++)
                {
                    var job4 = new Job4();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    jobClient.Add(appID, () => job4.Start(progress, complexList));
                }

                message += p4Count + " - Job4 added to background server list. <br/>";
            }

            if (p5Count > 0)
            {
                var myList = new List<int>();
                for (var i = 1; i <= 100; i++)
                {
                    myList.Add(i);
                }

                var testData2 = new TestData2();
                testData2.MyAppStatus = MyAppStatus.Suspend;
                testData2.MyList = myList;

                var complexList = new List<TestData>();
                for (var i = 1; i <= 100; i++)
                {
                    var newData = new TestData();
                    newData.MyNumber = i;
                    newData.MyString = "mystring " + i;
                    complexList.Add(newData);
                }
                testData2.TestDataList = complexList;

                for (var i = 0; i < p5Count; i++)
                {
                    var job5 = new Job5();
                    var progress = new SynchronousProgress<ProgressInfo>();
                    jobClient.Add(appID, () => job5.Start(progress, testData2));
                }

                message += p5Count + " - Job5 added to background server list. <br/>";
            }

            ViewBag.Message = message;

            return View("Index");
        }


    }
}
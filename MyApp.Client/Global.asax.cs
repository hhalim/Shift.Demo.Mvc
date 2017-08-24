using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Shift;
using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using System.Collections.Generic;

namespace MyApp.Client
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register); //WebAPI

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutofacConfiguration();

            //Shift Client
            var clientConfig = new Shift.ClientConfig();
            clientConfig.DBConnectionString = ConfigurationManager.ConnectionStrings["ShiftDBConnection"].ConnectionString;
            clientConfig.DBAuthKey = ConfigurationManager.AppSettings["DocumentDBAuthKey"];
            clientConfig.EncryptionKey = ConfigurationManager.AppSettings["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if exists
            clientConfig.StorageMode = ConfigurationManager.AppSettings["StorageMode"];
            Application["Shift.JobClient"] = new JobClient(clientConfig); //only the DBConnectionString and CacheConfigurationString are required for Client's background job

            //Shift Server
            var serverConfig = new Shift.ServerConfig();
            serverConfig.DBConnectionString = ConfigurationManager.ConnectionStrings["ShiftDBConnection"].ConnectionString;
            serverConfig.DBAuthKey = ConfigurationManager.AppSettings["DocumentDBAuthKey"];
            serverConfig.EncryptionKey = ConfigurationManager.AppSettings["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if exists
            serverConfig.MaxRunnableJobs = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRunnableJobs"]);
            serverConfig.ProcessID = ConfigurationManager.AppSettings["ShiftPID"];
            serverConfig.Workers = Convert.ToInt32(ConfigurationManager.AppSettings["ShiftWorkers"]);

            serverConfig.StorageMode = ConfigurationManager.AppSettings["StorageMode"];
            var progressDBInterval = ConfigurationManager.AppSettings["ProgressDBInterval"];

            if (!string.IsNullOrWhiteSpace(progressDBInterval))
                serverConfig.ProgressDBInterval = TimeSpan.Parse(progressDBInterval); //Interval when progress is updated in main DB

            var autoDeletePeriod = ConfigurationManager.AppSettings["AutoDeletePeriod"];
            serverConfig.AutoDeletePeriod = string.IsNullOrWhiteSpace(autoDeletePeriod) ? null : (int?)Convert.ToInt32(autoDeletePeriod);
            serverConfig.AutoDeleteStatus = new List<JobStatus?> { JobStatus.Completed }; //Auto delete only the jobs that had been Completed

            serverConfig.ForceStopServer = Convert.ToBoolean(ConfigurationManager.AppSettings["ForceStopServer"]); //Set to true to allow windows service to shut down after a set delay in StopServerDelay
            serverConfig.StopServerDelay = Convert.ToInt32(ConfigurationManager.AppSettings["StopServerDelay"]);

            //serverConfig.ServerTimerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ServerTimerInterval"]); //optional: default every 5 sec for server running jobs
            //serverConfig.ServerTimerInterval2 = Convert.ToInt32(ConfigurationManager.AppSettings["ServerTimerInterval2"]); //optional: default every 10 sec for server CleanUp()
            //serverConfig.AssemblyFolder = ConfigurationManager.AppSettings["AssemblyFolder"];
            //serverConfig.AssemblyListPath = ConfigurationManager.AppSettings["AssemblyListPath"]; 

            serverConfig.PollingOnce = Convert.ToBoolean(ConfigurationManager.AppSettings["PollingOnce"]);

            //For this demo, we're running the background process server in the same process as the web client
            //It's recommended to run the server in a separate process, such as windows service or Azure WebJob or another app.
            var jobServer = new Shift.JobServer(serverConfig);
            Application["Shift.JobServer"] = jobServer;
        }

        //Inject httpContextHttpContextBase into MVC constructor
        protected void AutofacConfiguration()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            //builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            //builder.RegisterFilterProvider();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
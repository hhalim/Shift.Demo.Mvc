using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Shift;
using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;

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

            //Shift
            var options = new Shift.Options();
            options.DBConnectionString = ConfigurationManager.ConnectionStrings["ShiftDBConnection"].ConnectionString;

            options.UseCache = true;
            options.CacheConfigurationString = ConfigurationManager.AppSettings["RedisConfiguration"];

            options.AssemblyListPath = ConfigurationManager.AppSettings["AssemblyListPath"]; //Shift.Server
            options.MaxRunnableJobs = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRunableJobs"]); //Shift.Server
            options.ProcessID = Convert.ToInt32(ConfigurationManager.AppSettings["ShiftPID"]); //Shift.Server

            options.EncryptionKey = ConfigurationManager.AppSettings["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if exists

            Application["Shift.JobClient"] = new JobClient(options); //only the DBConnectionString and CacheConfigurationString are required for Client's background job

            //For this demo, we're running the background process server in the same process as the web client
            //It's better to run the server portion in a separate windows service or Azure WebJob or another app.
            var jobServer = new Shift.JobServer(options);
            jobServer.Start();
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

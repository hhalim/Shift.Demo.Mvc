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
            options.AssemblyListPath = ConfigurationManager.AppSettings["AssemblyListPath"];
            options.MaxRunnableJobs = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRunableJobs"]);
            options.ProcessID = Convert.ToInt32(ConfigurationManager.AppSettings["ShiftPID"]);
            options.DBConnectionString = ConfigurationManager.ConnectionStrings["ShiftDBConnection"].ConnectionString;
            options.CacheConfigurationString = ConfigurationManager.AppSettings["RedisConfiguration"];
            //options.EncryptionKey = ConfigurationManager.AppSettings["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if filled

            Application["Shift.JobClient"] = new JobClient(options); //only the DBConnectionString and RedisConnectionString are required for Client's background job

            //Running the server in the same process as the client
            //It's better to run the server portion in a separate windows service or Azure WebJobs
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

using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;

namespace MyApp.Client
{
    public class RequiresSSL : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase req = filterContext.HttpContext.Request;
            HttpResponseBase res = filterContext.HttpContext.Response;

            //Check if we're secure or not and if we're on the local box
            var forceSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["ForceSSL"]);
            if (forceSSL && !req.IsSecureConnection)
            {
                var builder = new UriBuilder(req.Url)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = 443
                };
                filterContext.Result = new RedirectResult(builder.Uri.ToString());
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }

    public abstract class CustomController : Controller
    {
        protected override JsonResult Json(object data, string contentType,
            Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }

    public class JsonNetActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result.GetType() == typeof(JsonResult))
            {
                // Get the standard result object with unserialized data
                JsonResult result = filterContext.Result as JsonResult;

                // Replace it with our new result object and transfer settings
                filterContext.Result = new JsonNetResult
                {
                    ContentEncoding = result.ContentEncoding,
                    ContentType = result.ContentType,
                    Data = result.Data,
                    JsonRequestBehavior = result.JsonRequestBehavior
                };

                // Later on when ExecuteResult will be called it will be the
                // function in JsonNetResult instead of in JsonResult
            }
            base.OnActionExecuted(filterContext);
        }
    }

    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Error,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    Formatting = Formatting.Indented
                };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet 
                && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed. To allow GET requests, set JsonRequestBehavior to AllowGet.");

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrWhiteSpace(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data == null)
                return;

            var scriptSerializer = JsonSerializer.Create(Settings);

            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, Data);
                response.Write(sw.ToString());
            }
        }
    }

    public class SimpleResult
    {
        public SimpleResult()
        {
            Title = "";
            Status = "";
            Message = "";
            Data = null;
        }

        public string Title { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }
    }

    ///<summary>
    /// Prevent the auth cookie from being reset for this action, allows you to
    /// have requests that do not reset the login timeout.
    /// </summary>
    public class DoNotResetAuthCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Cookies.Remove(FormsAuthentication.FormsCookieName);
        }
    }
}
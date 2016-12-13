using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using Shift.Entities;
using Global;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace MyApp.Client.Controllers
{
    public class DashboardApiController : ApiController
    {
        //Test moving DataSourceRequest to Web API
        public DataSourceResult Get([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))]DataSourceRequest request)
        {
            DataSourceResult result = null;
            using (var db = new BGProcess(DBConstant.ConnectionName))
            {
                IQueryable<JobView> query = from p in db.JobView
                                            select p;
                result = query.ToDataSourceResult(request);
            }


            return result;
        }

    }
}

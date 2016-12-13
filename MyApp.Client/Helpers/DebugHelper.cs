using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyApp.Client.Helpers
{
    public static class DebugHelper
    {
        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

    }
}
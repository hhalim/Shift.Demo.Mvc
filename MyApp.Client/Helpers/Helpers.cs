using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApp.Client.Helpers
{
    public class Helpers
    {

        public static string GetPath(HttpContext context, string path)
        {

            if (System.IO.Path.IsPathRooted(path))
            {
                //it is a full path, don't do anything
                return path;
            }

            return context.Server.MapPath(path); //transform relative path to full path
        }

    }
}
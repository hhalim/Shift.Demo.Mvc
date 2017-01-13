using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Global
{
    public static class DBConstant
    {
        public static string ConnectionName = "name=ShiftDBConnection";
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ShiftDBConnection"].ConnectionString;
    }

}
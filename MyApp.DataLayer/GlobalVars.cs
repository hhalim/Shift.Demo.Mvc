using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataLayer
{
    public enum MyAppStatus
    {
        Start = 1,
        Stop = 2,
        Suspend = 3,
        Continue = 4,
        Cancelled = -1,
        Error = -99
    }
}

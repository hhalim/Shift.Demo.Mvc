using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shift.Entities;

namespace MyApp.BusinessLayer
{
    public class Job3
    {
        public void Start(IProgress<ProgressInfo> progress, List<int> simpleList)
        {
            var total = simpleList.Count;
            var counter = 0;
            var pInfo = new ProgressInfo();

            foreach (var number in simpleList)
            {
                //Report progress
                counter++;
                pInfo.Percent = (int)Math.Round((counter / (double)total) * 100.00, MidpointRounding.AwayFromZero);
                if (progress != null)
                    progress.Report(pInfo);

                //Do main job
                var newnumber = number + 1;
                Thread.Sleep(750);
            }
        }
    }
}

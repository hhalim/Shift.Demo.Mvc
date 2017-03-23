using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MyApp.DataLayer;
using Shift.Entities;

namespace MyApp.BusinessLayer
{
    public class Job4
    {

        public void Start(IProgress<ProgressInfo> progress, List<TestData> complexList, CancellationToken token)
        {
            var total = complexList.Count;
            var counter = 0;
            var pInfo = new ProgressInfo();

            foreach (var row in complexList)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested(); //throw OperationCanceledException
                }

                //Report progress
                counter++;
                pInfo.Percent = (int)Math.Round((counter / (double)total) * 100.00, MidpointRounding.AwayFromZero);
                if (progress != null)
                    progress.Report(pInfo);

                //Do main job
                var myString = row.MyString;
                var myNumber = row.MyNumber;
                Thread.Sleep(2000);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shift.Entities;

namespace MyApp.BusinessLayer
{
    public class Job5
    {
        public async Task StartAsync(IProgress<ProgressInfo> progress, List<int> simpleList, CancellationToken cancelToken, PauseToken pauseToken)
        {
            var total = simpleList.Count;
            var counter = 0;
            var pInfo = new ProgressInfo();

            foreach (var number in simpleList)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    cancelToken.ThrowIfCancellationRequested(); //throw OperationCanceledException
                }

                await pauseToken.WaitWhilePausedAsync(); //pause if IsPaused = true
                
                //Report progress
                counter++;
                pInfo.Percent = (int)Math.Round((counter / (double)total) * 100.00, MidpointRounding.AwayFromZero);
                if (progress != null)
                    progress.Report(pInfo);

                //Do main job
                var newnumber = number + 1;
                Thread.Sleep(2000);
            }
        }
    }
}

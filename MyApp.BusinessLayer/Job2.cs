using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shift.Entities;
using Newtonsoft.Json;

namespace MyApp.BusinessLayer
{
    public class Job2
    {

        public void Start(IProgress<ProgressInfo> progress, int x, CancellationToken cancelToken, PauseToken pauseToken)
        {
            var total = 100; //get total for progress
            var counter = 0; //progress counter
            var pInfo = new ProgressInfo();

            for (var i = 0; i < total; i++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    cancelToken.ThrowIfCancellationRequested(); //throw OperationCanceledException
                }

                pauseToken.WaitWhilePausedAsync().GetAwaiter().GetResult();

                //Report progress
                counter++;
                var percent = (int)Math.Round((counter / (double)total) * 100.00, MidpointRounding.AwayFromZero);
                pInfo.Percent = percent;
                switch(percent)
                {
                    case 25:
                        pInfo.Note += "Quarter of the way. <br/>";
                        break;
                    case 50:
                        pInfo.Note += "Half way there. <br/>";
                        break;
                    case 75:
                        pInfo.Note += "Three fourth of the way. <br/>";
                        break;
                    case 100:
                        pInfo.Note += "DONE! <br/>Filename: <a href='/downloadfile/internal/234234234.pdf'>tesfile.data</a><br/>";
                        break;
                }
                var tmpDict = new Dictionary<string, string>();
                tmpDict.Add("Percent", pInfo.Percent.ToString("##.##"));
                tmpDict.Add("Note", pInfo.Note);
                pInfo.Data = JsonConvert.SerializeObject(tmpDict);
                if (progress != null)
                    progress.Report(pInfo);

                //Do Job
                x += i;
                Thread.Sleep(2500);
            }

        }
    }
}

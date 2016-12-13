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
    public class Job5
    {

        public void Start(IProgress<ProgressInfo> progress, TestData2 complexData)
        {
            var total = complexData.TestDataList.Count;

            var counter = 0;
            var status = complexData.MyAppStatus;
            var myList = complexData.MyList;
            var pInfo = new ProgressInfo();

            ////Looping to simulate processing
            foreach (var row in complexData.TestDataList)
            {
                //Report progress
                counter++;
                pInfo.Percent = (int)Math.Round((counter / (double)total) * 100.00, MidpointRounding.AwayFromZero);

                if (progress != null)
                    progress.Report(pInfo);

                //Do main job
                var myString = row.MyString;
                var myNumber = row.MyNumber;
                Thread.Sleep(200);
            }

            //Insert files in the end report
            var path = AppDomain.CurrentDomain.BaseDirectory + "/SampleFiles/";
            var fileInfoList = new List<FileInfo> {
                new FileInfo { ExternalID = Guid.NewGuid().ToString("N"), ContentType="plain/text", FileName="testfile1.txt", FullPath= path + "testfile1.txt", DeleteAfterUpload=false },
                new FileInfo { ExternalID = Guid.NewGuid().ToString("N"), ContentType="plain/text", FileName="testfile2.txt", FullPath= path + "testfile2.txt", DeleteAfterUpload=false }
            };
            pInfo.FileInfoList = fileInfoList;
            progress.Report(pInfo);
            pInfo.FileInfoList.Clear(); //This only works at the end, if cleared then do another .Report call, the file saving doesn't work

            pInfo.Note = "All files saved into DB table JobResult.";
            progress.Report(pInfo); //Test next progress report

        }
    }
}

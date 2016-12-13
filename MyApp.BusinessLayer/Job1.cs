﻿using Shift.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.BusinessLayer
{
    public class Job1  
    {
        public void Start(string value, IProgress<ProgressInfo> progress)
        {
            var total = 10;

            var note = "";
            for (var i = 0; i < total; i++)
            {
                note += i + " - " + value + "<br/> \n";

                var pInfo = new ProgressInfo();
                pInfo.Percent = (int)Math.Round( ( (i+1)/(double)total ) * 100.00, MidpointRounding.AwayFromZero); ;
                pInfo.Note = note;
                if (progress != null)
                    progress.Report(pInfo);
                 
                Thread.Sleep(1000);
            }

            return;
        }
    }
}

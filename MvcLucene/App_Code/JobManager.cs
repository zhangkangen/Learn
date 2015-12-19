using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using Quartz;

namespace MvcLucene.App_Code
{
    public class JobManager : IJob
    {
        private ILog _logger = LogManager.GetLogger(typeof (JobManager));

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("test"+DateTime.Now.ToString());
        }
    }
}
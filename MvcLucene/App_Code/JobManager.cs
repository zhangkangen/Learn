using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;

namespace MvcLucene.App_Code
{
    public class JobManager : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
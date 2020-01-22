using Automate.Expense.Tracking.Sample.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace Automate.Expense.Tracking.Sample.Helper
{
    public class HandleInvoke
    {
        public static TaskInfo GetTaskInfo(string actionInfo)
        {
            TaskInfo taskInfo = new TaskInfo();
            switch (actionInfo)
            {
                case "createexp":
                    taskInfo.Url = taskInfo.FallbackUrl = ConfigurationManager.AppSettings["BaseUri"] + "createexp";
                    SetTaskInfo(taskInfo, TaskModelUIConstant.CreateExp);
                    break;
                default:
                    break;
            }
            return taskInfo;
        }

        public static void SetTaskInfo(TaskInfo taskInfo, UIConstants uIConstants)
        {
            taskInfo.Height = uIConstants.Height;
            taskInfo.Width = uIConstants.Width;
        }
    }
}
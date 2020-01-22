using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automate.Expense.Tracking.Sample.Models
{
    public class BotFrameworkCardValue<T>
    {
        [JsonProperty("type")]
        public object Type { get; set; } = "task/fetch";

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class AdaptiveCardValue<T>
    {
        [JsonProperty("msteams")]
        public object Type { get; set; } = JsonConvert.DeserializeObject("{\"type\": \"task/fetch\" }");

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class TaskModuleActionData<T>
    {
        [JsonProperty("data")]
        public BotFrameworkCardValue<T> Data { get; set; }
    }

    public class ActionDetails<T>
    {
        [JsonProperty("action")]
        public object Action { get; set; }

        [JsonProperty("TicketNo")]
        public string TicketNo { get; set; }

    }

    public class TaskModuleSubmitData<T>
    {
        [JsonProperty("commandId")]
        public T commandId { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class activityJson {

        public string type { get; set; }
        public string reportId { get; set; }
        public string managerId { get; set; }
        public string employeeId { get; set; }
    }
}
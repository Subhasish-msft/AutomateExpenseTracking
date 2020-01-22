using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automate.Expense.Tracking.Sample.Model
{
    public class DatabaseItem
    {
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
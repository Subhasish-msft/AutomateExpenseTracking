using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automate.Expense.Tracking.Sample.Model
{
    public class Employee : DatabaseItem
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(Employee);
        [JsonIgnore]
        public const string TYPE = "Employee";

        [JsonProperty(PropertyName = "id")]
        public string EmailId { get; set; }

        [JsonProperty(PropertyName = "azureADId")]
        public string AzureADId { get; set; }

        [JsonProperty(PropertyName = "userUniqueId")]
        public string UserUniqueId { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "photoPath")]
        public string PhotoPath { get; set; }

        [JsonProperty(PropertyName = "ManagerEmailId")]
        public string ManagerEmailId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string DisplayName { get { return (Name ?? string.Empty).Split(' ').First(); } }

        [JsonIgnore]
        public bool IsManager { get; set; }

        [JsonProperty(PropertyName = "jobTitle")]
        public string JobTitle { get; set; }

        public List<string> ExpenseReportList { get; set; }

    }
}
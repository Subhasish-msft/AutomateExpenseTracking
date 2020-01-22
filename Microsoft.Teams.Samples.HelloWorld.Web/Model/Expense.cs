using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Expense.Tracking.Sample.Enums;

namespace Automate.Expense.Tracking.Sample.Model
{
    public class Expense : DatabaseItem
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(Expense);
        public string ExpenseId { get; set; }
        public string ReportName { get; set; }
        public double SerialNumber { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public string DocumentId { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}
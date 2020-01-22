using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Expense.Tracking.Sample.Enums;

namespace Automate.Expense.Tracking.Sample.Model
{
    public class ExpenseReport : DatabaseItem
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(ExpenseReport);
        public DateTime DateCreated { get; set; }
        public ApprovalStatus Status { get; set; }
        public string ApprovedBy { get; set; }
        public List<Expense> ExpenseItems { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automate.Expense.Tracking.Sample
{
    public static class TaskModelUIConstant
    {
        public static UIConstants PurchaseOrder { get; set; } =
              new UIConstants(950, 650, "Purchase Order", TaskModuleIds.PurchaseOrder, "Purchase Order");
        public static UIConstants NewsCard { get; set; } =
            new UIConstants(750, 650, "News Card", TaskModuleIds.Newscard, "News Card");
        public static UIConstants CreateTicket { get; set; } =
        new UIConstants(610, 500, "Create Ticket", TaskModuleIds.CreateTicket, "Create Ticket");

        public static UIConstants TicketComplete { get; set; } =
        new UIConstants(350, 420, "Ticket Complete", TaskModuleIds.TicketComplete, "Ticket Complete");

        public static UIConstants VisitorRegistration { get; set; } =
    new UIConstants(500, 550, "Visitor Registration", TaskModuleIds.VisitorRegistration, "Visitor Registration");
        public static UIConstants SendRequest { get; set; } =
        new UIConstants(400, 459, "Send Request", TaskModuleIds.SendRequest, "Send Request");
        public static UIConstants PoDecline { get; set; } =
        new UIConstants(400, 367, "PO Decline", TaskModuleIds.PODecline, "PO Decline");
        public static UIConstants Declined { get; set; } =
        new UIConstants(400, 170, "Declined", TaskModuleIds.Declined, "Declined");

        public static UIConstants ETCard { get; set; } =
        new UIConstants(750, 600, "ET Card: Inputs", TaskModuleIds.ETcard, "ET Card");

        public static UIConstants POCard { get; set; } =
        new UIConstants(750, 600, "PO Card", TaskModuleIds.ETcard, "PO Card");

        public static UIConstants CreateExp { get; set; } =
       new UIConstants(510, 450, "Create Expense", "createexp", "Create Expense");

        public static UIConstants CustomForm { get; set; } =
            new UIConstants(510, 450, "Custom Form", TaskModuleIds.CustomForm, "Custom Form");
    }
    public class UIConstants
    {
        public UIConstants(int width, int height, string title, string id, string buttonTitle)
        {
            Width = width;
            Height = height;
            Title = title;
            Id = id;
            ButtonTitle = buttonTitle;
        }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Title { get; set; }
        public string ButtonTitle { get; set; }
        public string Id { get; set; }
    }

    public class TaskModuleIds
    {
        public const string PurchaseOrder = "purchaseorder";
        public const string CreateTicket = "createticket";
        public const string TicketComplete = "ticketcomplete";
        public const string VisitorRegistration = "visitorregistration";
        public const string PODecline = "podecline";
        public const string Declined = "declined";
        public const string Newscard = "newscard";
        public const string ETcard = "ETcard";
        public const string SendRequest = "sendrequest";
        public const string EventCard = "eventcard";
        public const string POCard = "pocard";
        public const string CustomForm = "customform";
        public const string pendingDates = "pendingdates";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Automate.Expense.Tracking.Sample.Models;
using System.Configuration;

namespace Automate.Expense.Tracking.Sample.Helper
{
    public class CardHelper
    {

        public static Attachment ExpenseCard()
        {
            var card = new ListCard();
            card.content = new Content();
            var list = new List<Item>();
            var buttonsList = new List<ListButton>();
            card.content.title = "Welcome to Automate Expense Tracking App";
            Item item = new Item();
            item = new Item();
            item.title = "Create Expenses";
            item.subtitle = "Create your expense sheet for manager review";
            item.type = "resultItem";
            //item.icon = ConfigurationManager.AppSettings["BaseUri"] + "/Images/purpleImage.JPG";
            item.icon= System.Web.Hosting.HostingEnvironment.MapPath(@"~\Images\purpleImage.JPG");
            var url = "createexp";
            item.tap = new Tap()
            {
                type = "invoke",
                title = item.id,
                value = "{ \"type\": \"task/fetch\", \"data\": \"" + url + "\"}"
            };
            list.Add(item);
            item = new Item();
            item.title = "Edit Expenses";
            item.subtitle = "Edit your existing sheet and review ";
            item.type = "resultItem";
            //item.icon = ConfigurationManager.AppSettings["BaseUri"] + "/Images/purpleImage.JPG";
            item.icon = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Images\purpleImage.JPG");
            var editUrl = "customform";
            item.tap = new Tap()
            {
                type = "invoke",
                title = item.id,
                value = "{ \"type\": \"task/fetch\", \"data\": \"" + editUrl + "\"}"
            };
            list.Add(item);
            item = new Item();
            item = new Item();
            item.title = "Upload Report";
            item.subtitle = "Upload existing csv file";
            item.type = "resultItem";
            //item.icon = ConfigurationManager.AppSettings["BaseUri"] + "/Images/purpleImage.JPG";
            //item.icon = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Images\purpleImage.JPG");
            var UploadUrl = "UploadExp";
            item.tap = new Tap()
            {
                type = "invoke",
                title = item.id,
                value = "{ \"type\": \"task/fetch\", \"data\": \"" + UploadUrl + "\"}"
            };
            list.Add(item);
            card.content.items = list.ToArray();
            Attachment attachment = new Attachment();
            attachment.ContentType = card.contentType;
            attachment.Content = card.content;
            return attachment;
        }
    }
}
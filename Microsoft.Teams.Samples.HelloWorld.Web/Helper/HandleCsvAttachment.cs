using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Automate.Expense.Tracking.Sample.Model;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Automate.Expense.Tracking.Sample.Enums;

namespace Automate.Expense.Tracking.Sample.Helper
{
    public static class HandleCsvAttachment
    {

        public static List<Model.Expense> ReadCsv(Attachment attachment)
        {
            var expenseList = new List<Model.Expense>();
            if (attachment.ContentType == FileDownloadInfo.ContentType)
            {
                FileDownloadInfo downloadInfo = (attachment.Content as JObject).ToObject<FileDownloadInfo>();
                var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/");

                filePath += attachment.Name + DateTime.Now.Millisecond; // just to avoid name collision with other users
                if (downloadInfo != null)
                {
                    using (WebClient myWebClient = new WebClient())
                    {
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(downloadInfo.DownloadUrl, filePath);
                    }
                    if (File.Exists(filePath))
                    {
                        using (TextFieldParser parser = new TextFieldParser(filePath))
                        {
                            bool isFirstRow = true;
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");
                            while (!parser.EndOfData)
                            {
                                string[] fields = parser.ReadFields();

                                if (isFirstRow)
                                {
                                    isFirstRow = false;
                                    continue;
                                }
                                try
                                {
                                    var expense = new Model.Expense()
                                    {
                                        SerialNumber = Convert.ToDouble(fields[0]),
                                        ReportName = fields[1],
                                        Date = DateTime.Parse(fields[2]),
                                        Description = fields[3],
                                        Currency = Currency.Rupee,
                                        TotalAmount = Convert.ToDecimal(fields[4])
                                    };
                                    expenseList.Add(expense);

                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }
                        }
                    }
                }
            }
            return expenseList;
        }
    }
}
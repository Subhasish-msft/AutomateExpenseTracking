using AdaptiveCards;
using Automate.Expense.Tracking.Sample.Model;
using Automate.Expense.Tracking.Sample.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Automate.Expense.Tracking.Sample
{
    public static class EchoBot
    {
        //public static async Task EchoMessage(ConnectorClient connector, Activity activity)
        //{
        //    var reply = activity.CreateReply("You said: " + activity.GetTextWithoutMentions());
        //    await connector.Conversations.ReplyToActivityWithRetriesAsync(reply);
        //}

        public static Attachment WelcomeCard(string userEmailId, string userName)
        {
            var WelcomeCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>()
            {
            new AdaptiveContainer
            {
            Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveImage()
                        {
                            Url = new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/WelcomeBanner.png"),
                        },
                        new AdaptiveTextBlock()
                        {
                            Text=$"{userName}! Here is what I can do for you.",
                            Size=AdaptiveTextSize.Large
                        },
                        new AdaptiveColumnSet()
                        {
                            Columns=new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage(){Url=new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/Create.png"),Size=AdaptiveImageSize.Small,Style=AdaptiveImageStyle.Default, SelectAction=new AdaptiveSubmitAction(){ DataJson=@"{'Type':'" + Constants.CreateExpense+"'}", Title="Leave Request"},HorizontalAlignment=AdaptiveHorizontalAlignment.Center, Spacing=AdaptiveSpacing.None }
                                        }
                                },

                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Create Expense",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.None, HorizontalAlignment=AdaptiveHorizontalAlignment.Center}
                                        },
                                        SelectAction = new AdaptiveSubmitAction()
                                        {
                                            DataJson=@"{'Type':'" + Constants.CreateExpense+"'}", Title="Create Expense"
                                        }
                                }
                            }
                        },
                        new AdaptiveColumnSet()
                        {
                            Columns=new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage(){Url=new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/Edit.png"),Size=AdaptiveImageSize.Small,Style=AdaptiveImageStyle.Default, SelectAction=new AdaptiveSubmitAction(){ DataJson= @"{'Type':'" + Constants.EditExpense +"'}",   Title= "Leave Balance"},HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None}
                                        }
                                },

                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Edit Expense",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium,HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                        },
                                        SelectAction = new AdaptiveSubmitAction()
                                        {
                                            DataJson=@"{'Type':'" + Constants.EditExpense+"'}", Title="Edit Expense"
                                        }
                                }
                            }
                        },
                        new AdaptiveColumnSet()
                        {
                            Columns=new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage(){Url=new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/Upload.png"),Size=AdaptiveImageSize.Small,Style=AdaptiveImageStyle.Default, SelectAction=new AdaptiveSubmitAction(){ DataJson= @"{'Type':'" + Constants.UploadReport+"'}",   Title= "Leave Balance"},HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None}
                                        }
                                },

                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Upload Report",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium,HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                        },
                                        SelectAction = new AdaptiveSubmitAction()
                                        {
                                            DataJson=@"{'Type':'" + Constants.UploadReport+"'}", Title="Upload Report"
                                        }
                                }
                            }
                        }
                    }

            } }

            };

            var isManager = IsEmployeeManger(userEmailId);

            if (isManager)
            {
                (WelcomeCard.Body[0] as AdaptiveContainer).Items.Insert(4,
                    new AdaptiveColumnSet()
                    {
                        Columns = new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage(){Url=new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/Pending.png"),Size=AdaptiveImageSize.Small,Style=AdaptiveImageStyle.Default, SelectAction=new AdaptiveSubmitAction(){ DataJson=@"{'Type':'" + Constants.PendingApproval+"'}", Title="Leave Request"},HorizontalAlignment=AdaptiveHorizontalAlignment.Center, Spacing=AdaptiveSpacing.None }
                                        }
                                },

                                new AdaptiveColumn()
                                {
                                        Width=AdaptiveColumnWidth.Auto,
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Pending Approval",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.None, HorizontalAlignment=AdaptiveHorizontalAlignment.Center}
                                        },
                                        SelectAction = new AdaptiveSubmitAction()
                                        {
                                            DataJson=@"{'Type':'" + Constants.PendingApproval+"'}", Title="Pending Approval"
                                        }
                                }
                            }
                    }
                    );
            }

            var card = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = WelcomeCard
            };
            return card;
        }

        private static bool IsEmployeeManger(string emailId)
        {
            var isManager = false;
            var managerIds = ConfigurationManager.AppSettings["ManagerIds"].ToString().Split(',');
            if (managerIds.Length > 0)
            {
                foreach (var managerId in managerIds)
                {
                    if (managerId.ToLower() == emailId.ToLower())
                    {
                        isManager = true;
                        break;
                    }
                }
            }
            return isManager;
        }

        public static Attachment CreateExpense()
        {
            List<AdaptiveChoice> currency = GetCurrencyData();

            var expense = new AdaptiveCard("1.0")
            {

                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                         new AdaptiveTextBlock(){Text="Create Expense", Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Medium,Wrap=true},
                                        }
                                    }
                                 }
                             }
                        }
                    },
                     new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Name", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveTextInput(){Id="ReportName",Placeholder="Expense Name" }

                                        }
                                    }
                                 }
                             }
                        }
                    },
                    new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Date", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveDateInput(){Id="Date",Placeholder="Date", Value = DateTime.Today.ToUniversalTime().ToString("u") }


                                        }
                                    }
                                 }
                             }
                        }
                    },
                     new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Currency", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveChoiceSetInput()
                                                {
                                                    Id = "FromDuration",
                                                    Choices = new List<AdaptiveChoice>(currency),
                                                    IsMultiSelect = false,
                                                    Style = AdaptiveChoiceInputStyle.Compact,
                                                    Value = ""
                                                }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Total Amount", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },

                                            new AdaptiveNumberInput(){Id="TotalAmount",Placeholder="0.0" }

                                        }
                                    }

                                 }
                             }
                        }
                    },
                      new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Description", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveTextInput(){Id="Description",Placeholder="Write description here" }


                                        }
                                    }
                                 }
                             }
                        }
                    }

                },
                Actions = new List<AdaptiveAction>() {
                    new AdaptiveSubmitAction(){
                         DataJson=@"{'Type':'Submit Expense'}", Title="Create"
                    }
                }

            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = expense
            };
        }

        private static List<AdaptiveChoice> GetCurrencyData()
        {
            var currency = new List<AdaptiveChoice>();
            currency.Add(new AdaptiveChoice() { Title = "Rupees(₹)", Value = "Rupees" });
            currency.Add(new AdaptiveChoice() { Title = "Dollar($)", Value = "Dollar" });
            currency.Add(new AdaptiveChoice() { Title = "Euro(€)", Value = "Euro" });
            return currency;
        }

        public static Attachment EditExpense()
        {
            List<AdaptiveChoice> currency = GetCurrencyData();

            var expense = new AdaptiveCard("1.0")
            {

                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                         new AdaptiveTextBlock(){Text="Edit Expense", Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Medium,Wrap=true},
                                        }
                                    }
                                 }
                             }
                        }
                    },
                     new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Name", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveTextInput(){Id="Name",Placeholder="Expense Name" , Value="Subhasish Pani"}

                                        }
                                    }
                                 }
                             }
                        }
                    },
                    new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Date", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveDateInput(){Id="Date",Placeholder="Date", Value = DateTime.Today.ToUniversalTime().ToString("u") }


                                        }
                                    }
                                 }
                             }
                        }
                    },
                     new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Currency", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveChoiceSetInput()
                                                {
                                                    Id = "FromDuration",
                                                    Choices = new List<AdaptiveChoice>(currency),
                                                    IsMultiSelect = false,
                                                    Style = AdaptiveChoiceInputStyle.Compact,
                                                    Value = "Rupees"
                                                }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Width="50",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Total Amount", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveNumberInput(){Id="Total Amount",Placeholder="0.0" , Value=1200}


                                        }
                                    }
                                 }
                             }
                        }
                    },
                      new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Description", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true },
                                            new AdaptiveTextInput(){Id="Description",Placeholder="Write description here" , Value="Employee expense report for August."}


                                        }
                                    }
                                 }
                             }
                        }
                    }

                },
                Actions = new List<AdaptiveAction>() {
                    new AdaptiveSubmitAction(){
                         DataJson=@"{'Type':Edit Expense'}", Title="Save"
                    }
                }

            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = expense
            };
        }

        public static Attachment UploadExpense()
        {
            var expense = new AdaptiveCard("1.0")
            {

                Body = new List<AdaptiveElement>()
                {
                     new AdaptiveContainer
                    {

                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                             {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width="100",
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock(){Text="Steps to Upload a file:", Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Medium,Wrap=true},
                                            new AdaptiveTextBlock(){Text="1. Drag and drop the file in the chart box below", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true,IsSubtle=true  },
                                            new AdaptiveTextBlock(){Text="2. Click on the attachment icon below. Then browes and upload the file", Weight=AdaptiveTextWeight.Lighter,Size=AdaptiveTextSize.Medium,Wrap=true,IsSubtle=true  }

                                        }
                                    }
                                 }
                             }
                        }
                    }

                }


            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = expense
            };
        }

        public static Attachment PendingApproval()
        {

            var pendingReports = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveContainer
                    {
                        Items=new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                    {
                        Columns=new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width=AdaptiveColumnWidth.Auto,
                                Items=new List<AdaptiveElement>()
                                {

                                    new AdaptiveImage(){Size=AdaptiveImageSize.Large,Url=new Uri(ConfigurationManager.AppSettings["BaseUri"] +"Images/peson.png"),
                                        Style =AdaptiveImageStyle.Person}
                                }

                            },
                            new AdaptiveColumn()
                            {
                                Spacing=AdaptiveSpacing.Large,
                                Width=AdaptiveColumnWidth.Stretch,
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(){
                                    Text ="Submitted by: Subhasish Pani",
                                    Size=AdaptiveTextSize.Medium,Wrap=true},

                                     new AdaptiveTextBlock(){Text=$"Report Name: Subhasish-25/09", Size=AdaptiveTextSize.Default,Wrap=true},
                                     new AdaptiveTextBlock(){Text=$"Applied Date: Sept 25, 2019", Size=AdaptiveTextSize.Default,Wrap=true},
                                     new AdaptiveTextBlock(){Text=$"Total Amount: Rs. 1250.00",Weight=AdaptiveTextWeight.Default,Size=AdaptiveTextSize.Medium,Wrap=true},
                                    new AdaptiveTextBlock(){Text="Description: Pleese review employee expense report from August.",HorizontalAlignment=AdaptiveHorizontalAlignment.Left,Wrap=true }
                                }

                            }
                        }

                    }
                        }
                    }

                },
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveShowCardAction()
                    {
                        Title="Approve",

                         Card=new AdaptiveCard("1.0")
                       {
                          Body=new List<AdaptiveElement>()
                          {
                              new AdaptiveTextInput(){Id="ManagerComment", IsMultiline=true,MaxLength=300, IsRequired=true, Placeholder="Comments (Optional)"}
                          },
                          Actions=new List<AdaptiveAction>()
                          {
                              new AdaptiveSubmitAction()
                              {
                                  Title="Approve",
                                  DataJson= @"{'Type':'" + Constants.ApproveReport+"', 'ReportId':'" + "subhasish-25/09"+"'}"

                              }
                          }
                       }
                    },
                    new AdaptiveShowCardAction()
                    {
                        Title="Reject",

                         Card=new AdaptiveCard("1.0")
                       {
                          Body=new List<AdaptiveElement>()
                          {
                              new AdaptiveTextInput(){Id="ManagerComment", IsMultiline=true,MaxLength=300, IsRequired=true, Placeholder="Write a reason (Optional)"}
                          },
                          Actions=new List<AdaptiveAction>()
                          {
                              new AdaptiveSubmitAction()
                              {
                                  Title="Reject",
                                  DataJson= @"{'Type':'" + Constants.RejectReport+"', 'ReportId':'" + "subhasish-25/09" +"'}"
                              }
                          }
                       }
                    }
                },
            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = pendingReports
            };
        }

        public static Attachment CraeteManagerCard()
        {
            var Card = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>()
                          {
                              new AdaptiveTextBlock(){Text="Please Enter Manager Email Id:"},
                              new AdaptiveTextInput(){Id="managerId", IsMultiline=false, Style = AdaptiveTextInputStyle.Email, IsRequired=true, Placeholder="Manager Email Id"}
                          },
                Actions = new List<AdaptiveAction>()
                          {
                              new AdaptiveSubmitAction()
                              {
                                  Title="Set Manager",
                                  DataJson= @"{'Type':'" + Constants.SetManager+"'}"
                              }
                          }
            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
        }

        public static Attachment PrepareCardWithAttachment(ExpenseReport expenseReport, Employee employee, bool isManagerCard = false, bool isAdminCard = false)
        {
            var card = new ListCard();
            card.content = new Content();
            var list = new List<Item>();
            card.content.title = "Uploaded Expenses";
            foreach (var exp in expenseReport.ExpenseItems)
            {
                Item item = new Item();
                item.title = exp.ReportName;
                item.subtitle = "Total Amount: Rs." + exp.TotalAmount;
                item.type = "resultItem";
                item.icon = ConfigurationManager.AppSettings["BaseUri"] + "/Images/Expense.PNG";
                var url = "pendingdates";
                //item.tap = new Tap()
                //{
                //    type = "invoke",
                //    title = item.id,
                //    value = "{ \"type\": \"task/fetch\", \"data\": \"" + url + "\"}"
                //};
                list.Add(item);
            }
            if (isManagerCard)
            {
                var approve = new ListButton() { title = "Approve", type = "messageBack", value = "{ \"type\": \"ApproveReport\", \"reportId\": \"" + expenseReport.Id + "\", \"employeeId\": \"" + employee.EmailId + "\"}" };
                var reject = new ListButton() { title = "Reject", type = "messageBack", value = "{ \"type\": \"RejectReport\", \"reportId\": \"" + expenseReport.Id + "\", \"employeeId\": \"" + employee.EmailId + "\"}" };
                var buttonLists = new List<ListButton>() { approve, reject };
                card.content.buttons = buttonLists.ToArray();
                card.content.title = employee.DisplayName + " has uploaded " + expenseReport.ExpenseItems.Count() + " bills";
            }
            if (isAdminCard)
            {
                card.content.title = employee.DisplayName + " has uploaded " + expenseReport.ExpenseItems.Count() + " bills. Approved by manager";
            }

            card.content.items = list.ToArray();
            Attachment attachment = new Attachment();
            attachment.ContentType = card.contentType;
            attachment.Content = card.content;
            return attachment;

        }

    }
    public class Constants
    {
        public const string ShowPendingApprovals = "ShowPendingApprovals";

        public const string ApproveReport = "ApproveReport";
        public const string RejectReport = "RejectReport";
        public const string SetManager = "SetManager";

        public const string CreateExpense = "Create Expense";
        public const string SubmitExpense = "Submit Expense";
        public const string EditExpense = "Edit Expense";
        public const string UploadReport = "Upload Report";
        public const string PendingApproval = "Pending Approval";
        public const string ExpenseReport = "ExpenseReport";
    }

}

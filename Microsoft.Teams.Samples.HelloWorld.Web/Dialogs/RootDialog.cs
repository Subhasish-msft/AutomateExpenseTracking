using Automate.Expense.Tracking.Sample.Helper;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Automate.Expense.Tracking.Sample.Model;
using Automate.Expense.Tracking.Sample.Repository;
using Automate.Expense.Tracking.Sample.Enums;
using System.Collections.Generic;
using Microsoft.Bot.Connector.Teams.Models;

namespace Automate.Expense.Tracking.Sample.Dialog
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string EmailKey = "emailId";
        private const string ManagerEmailKey = "managerEmailId";
        private const string ProfileKey = "profile";

        /// <summary>
        /// Called when the dialog is started.
        /// </summary>
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// Called when a message is received by the dialog
        /// </summary>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            string userEmailId = string.Empty;
            string userName = string.Empty;

            string managerEmailKey = activity.From.Id + ManagerEmailKey;
            userEmailId = await GetUserMailId(context, activity);
            userName = await getUserName(activity);

            string profileKey = GetProfileKey(activity);
            Employee employee = new Employee();



            if (activity.Value != null)
            {
                var activityValue = JsonConvert.DeserializeObject<Models.activityJson>(activity.Value.ToString());
                if (activityValue.type == Constants.SetManager)
                {
                    if (ValidateManagerId(activityValue.managerId))
                    {
                        await SetManagerId(context, managerEmailKey, profileKey, activityValue.managerId);
                        reply.Text = "Manager Email Id is updated as : " + activityValue.managerId;
                    }
                    else
                    {
                        reply.Text = "Invalid Email Id, Please enter a valid manager email id. ";
                    }
                }
            }

            if (context.ConversationData.ContainsKey(profileKey))
            {
                employee = context.ConversationData.GetValue<Employee>(profileKey);
            }
            else
            {
                // TODO: Check if employee exists in DB before creating new record.

                employee = await DocumentDBRepository.GetItemAsync<Employee>(userEmailId, nameof(Employee));
                if (employee != null)
                {
                    context.ConversationData.SetValue<Employee>(profileKey, employee);
                }
                else
                {
                    employee = await SaveEmployeeToDbAsync(context, userEmailId, profileKey, managerEmailKey);
                    string emailKey = activity.From.Id + EmailKey;
                    context.ConversationData.SetValue(emailKey, userEmailId);
                    if (!string.IsNullOrEmpty(employee.ManagerEmailId))
                        context.ConversationData.SetValue(managerEmailKey, employee.ManagerEmailId);
                }

            }

            if (!(await IsManagerMailIdPresent(context, activity, managerEmailKey, employee)))
            {
                reply.Attachments.Add(EchoBot.CraeteManagerCard());
                await context.PostAsync(reply);
                return;
            }

            if (activity.Value != null)
            {
                var activityValue = JsonConvert.DeserializeObject<Models.activityJson>(activity.Value.ToString());
                switch (activityValue.type)
                {
                    case Constants.CreateExpense:
                        reply.Attachments.Add(EchoBot.CreateExpense());
                        break;
                    case Constants.EditExpense:
                        reply.Attachments.Add(EchoBot.EditExpense());
                        break;
                    case Constants.UploadReport:
                        reply.Attachments.Add(EchoBot.UploadExpense());
                        break;
                    case Constants.PendingApproval:
                        reply.Attachments.Add(EchoBot.PendingApproval());
                        break;
                    case Constants.ApproveReport:
                        await HandleManagerAction(context, activity, profileKey, activityValue.reportId, activityValue.employeeId, true);
                        return;
                    case Constants.RejectReport:
                        await HandleManagerAction(context, activity, profileKey, activityValue.reportId, activityValue.employeeId);
                        return;
                    case Constants.SubmitExpense:
                        reply.Text = await ValidateAndSubmitExpense(context, activity, profileKey);
                        break;
                }
            }
            else if (activity.Attachments.Count > 0)
            {
                //Read attachement and save data to database
                try
                {
                    var attachmentList = HandleCsvAttachment.ReadCsv(activity.Attachments.First());
                    reply.Text = await ValidateAndSubmitExpense(context, activity, profileKey, attachmentList);
                    PrepareExpenseList(attachmentList);
                    var expenseReport = CreateExpenseReport(attachmentList);
                    reply.Attachments.Add(EchoBot.PrepareCardWithAttachment(expenseReport, null));
                    //reply.Text = "Uploaded sucessfully. ";
                }
                catch (Exception ex)
                {
                    reply.Text = "Unable to process the uploaded file, Please verify the file format. ";
                }
            }
            else
            {
                var typingReply = activity.CreateReply();
                typingReply.Text = null;
                typingReply.Type = ActivityTypes.Typing;
                await context.PostAsync(typingReply);

                reply.Attachments.Add(EchoBot.WelcomeCard(userEmailId, userName));
            }
            await context.PostAsync(reply);
        }

        private async Task HandleManagerAction(IDialogContext context, Activity activity, string profileKey, string reportId, string employeeId, bool isApproved = false)
        {
            try
            {
                var expenseReport = await DocumentDBRepository.GetItemAsync<ExpenseReport>(reportId, nameof(ExpenseReport));
                expenseReport.Status = isApproved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
                await DocumentDBRepository.UpdateItemAsync<ExpenseReport>(reportId, expenseReport);
                var displayText = isApproved ? "Your Expense Request is Approved by manager" : "Your Expense Request is Rejected by manager";
                await UpdateBackEmployee(context, reportId, employeeId, displayText);
                if (isApproved)
                {
                    var cardAttachment = await PrepareCardForFinanceTeam(context, expenseReport, profileKey, employeeId);
                    await NotifyAccountChannel(context, activity, cardAttachment);

                }
            }
            catch (Exception ex)
            {
                var msg = context.MakeMessage();
                msg.Text = "Couldnot update in admin channel";
                await context.PostAsync(msg);
            }
        }

        private async Task<Attachment> PrepareCardForFinanceTeam(IDialogContext context, ExpenseReport expenseReport, string profileKey, string employeeId)
        {
            Employee employee;
            if (context.ConversationData.ContainsKey(profileKey))
            {
                employee = context.ConversationData.GetValue<Employee>(profileKey);
            }
            else
            {
                employee = await DocumentDBRepository.GetItemAsync<Employee>(employeeId, nameof(Employee));
            }
            var attachment = EchoBot.PrepareCardWithAttachment(expenseReport, employee, false, true);
            return attachment;
        }

        private static async Task NotifyAccountChannel(IDialogContext context, Activity activity, Attachment cardAttachment)
        {
            await SendMessageToAccountChannel(context, activity, cardAttachment);
        }

        private async Task UpdateBackEmployee(IDialogContext context, string reportId, string employeeId, string displayText)
        {
            var employee = await DocumentDBRepository.GetItemAsync<Employee>(employeeId, nameof(Employee));
            var conversationId = await SendNotification(context, employee.UserUniqueId, displayText, null, "", false);
        }

        private static async Task<Employee> SaveEmployeeToDbAsync(IDialogContext context, string userEmailId, string profileKey, string managerEmailKey)
        {
            Employee employee = new Employee();
            // Fill in the details
            try
            {
                var channelData = context.Activity.GetChannelData<TeamsChannelData>();

                employee.EmailId = userEmailId;
                employee.Id = Guid.NewGuid().ToString();
                employee.AzureADId = ""; // TBD
                employee.TenantId = channelData.Tenant.Id;
                employee.IsManager = false;
                employee.ManagerEmailId = context.ConversationData.ContainsKey(managerEmailKey) ? context.ConversationData.GetValue<string>(managerEmailKey) : "";
                employee.UserUniqueId = context.Activity.From.Id;
                employee.Name = context.Activity.From.Name;
                // Add new record in DB
                await DocumentDBRepository.CreateItemAsync<Employee>(employee);
                context.ConversationData.SetValue<Employee>(profileKey, employee);
            }
            catch (Exception ex)
            {

            }

            return employee;
        }

        private async Task<string> ValidateAndSubmitExpense(IDialogContext context, Activity activity, string profileKey)
        {
            var message = string.Empty;
            try
            {
                var expenseDetail = JsonConvert.DeserializeObject<Model.Expense>(activity.Value.ToString());
                expenseDetail.Type = nameof(Model.Expense);
                expenseDetail.CreatedTimeStamp = DateTime.Now;
                expenseDetail.ExpenseId = Guid.NewGuid().ToString();

                var expenseReport = CreateExpenseReport(new List<Model.Expense> { expenseDetail });

                await SaveExpanseAndUpdateEmployee(context, profileKey, expenseReport);

                // Current URL
                // Add Expense Id -- ExpenseId
                // Save

                // Get manager details
                // Send proactive Message
                // Get the Message Id and save it back to expense report - Add new field

                await SendNotificaiotnToManager(context, profileKey, expenseReport);

                message = "Expense created successfully and sent for approval to your Manager.";

            }
            catch (Exception ex)
            {
                message = "Something went wrong while creating the expense report.";
            }
            return message;
        }

        private static async Task SendNotificaiotnToManager(IDialogContext context, string profileKey, ExpenseReport expenseReport)
        {
            var employee = context.ConversationData.GetValue<Employee>(profileKey);
            var attachmentForManagerNotification = EchoBot.PrepareCardWithAttachment(expenseReport, employee, true);
            var conversationId = await SendNotification(context, employee.UserUniqueId, null, attachmentForManagerNotification, "", false);
        }

        private static async Task SaveExpanseAndUpdateEmployee(IDialogContext context, string profileKey, ExpenseReport expenseReport)
        {
            await DocumentDBRepository.CreateItemAsync<ExpenseReport>(expenseReport);
            Employee employee = context.ConversationData.GetValue<Employee>(profileKey);
            if (employee.ExpenseReportList == null)
            {
                employee.ExpenseReportList = new List<string>() { expenseReport.Id };
            }
            else
            {
                employee.ExpenseReportList.Add(expenseReport.Id);
            }

            await DocumentDBRepository.UpdateItemAsync<Employee>(employee.EmailId, employee);
            context.ConversationData.SetValue(profileKey, employee);

        }

        public static async Task<string> SendNotification(IDialogContext context, string userOrChannelId, string messageText, Microsoft.Bot.Connector.Attachment attachment, string updateMessageId, bool isChannelMessage)
        {
            var userId = userOrChannelId.Trim();
            var botId = context.Activity.Recipient.Id;
            var botName = context.Activity.Recipient.Name;

            var channelData = context.Activity.GetChannelData<TeamsChannelData>();
            var connectorClient = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            var parameters = new ConversationParameters
            {
                Bot = new ChannelAccount(botId, botName),
                Members = !isChannelMessage ? new ChannelAccount[] { new ChannelAccount(userId) } : null,
                ChannelData = new TeamsChannelData
                {
                    Tenant = channelData.Tenant,
                    Channel = isChannelMessage ? new ChannelInfo(userId) : null,
                    Notification = new NotificationInfo() { Alert = true }
                },
                IsGroup = isChannelMessage
            };

            try
            {
                var conversationResource = await connectorClient.Conversations.CreateConversationAsync(parameters);
                var replyMessage = Activity.CreateMessageActivity();
                replyMessage.From = new ChannelAccount(botId, botName);
                replyMessage.Conversation = new ConversationAccount(id: conversationResource.Id.ToString());
                replyMessage.ChannelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                replyMessage.Text = messageText;
                if (attachment != null)
                    replyMessage.Attachments.Add(attachment);//  EchoBot.ManagerViewCard(employee, leaveDetails));

                if (string.IsNullOrEmpty(updateMessageId))
                {
                    var resourceResponse = await connectorClient.Conversations.SendToConversationAsync(conversationResource.Id, (Activity)replyMessage);
                    return resourceResponse.Id;
                }
                else
                {
                    await connectorClient.Conversations.UpdateActivityAsync(conversationResource.Id, updateMessageId, (Activity)replyMessage);
                    return updateMessageId; // Just return the same Id.
                }
            }

            catch (Exception ex)

            {
                // Handle the error.
                var msg = context.MakeMessage();
                msg.Text = ex.Message;
                await context.PostAsync(msg);
                return null;
            }
        }

        private async Task<string> ValidateAndSubmitExpense(IDialogContext context, Activity activity, string profileKey, List<Model.Expense> expenseList)
        {
            var message = string.Empty;

            try
            {
                PrepareExpenseList(expenseList);
                var expenseReport = CreateExpenseReport(expenseList);
                await SaveExpanseAndUpdateEmployee(context, profileKey, expenseReport);
                await SendNotificaiotnToManager(context, profileKey, expenseReport);
                message = "Expense created successfully and sent for approval to your Manager.";
            }
            catch (Exception)
            {

                message = "Something went wrong while creating the expense report.";

            }
            return message;
        }

        private static void PrepareExpenseList(List<Model.Expense> expenseList)
        {
            foreach (var exp in expenseList)
            {
                exp.Type = nameof(Model.Expense);
                exp.CreatedTimeStamp = DateTime.Now;
                exp.ExpenseId = Guid.NewGuid().ToString();
            }
        }

        private static ExpenseReport CreateExpenseReport(List<Model.Expense> expenseDetail)
        {
            return new ExpenseReport()
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                Status = ApprovalStatus.Pending,
                ExpenseItems = expenseDetail
            };
        }

        private static string GetProfileKey(IActivity activity)
        {
            return activity.From.Id + ProfileKey;
        }

        private bool ValidateManagerId(string inputManagerId)
        {
            var isManagerValid = false;
            var managerIds = ConfigurationManager.AppSettings["ManagerIds"].ToString().Split(',');
            if (managerIds.Length > 0)
            {
                foreach (var managerId in managerIds)
                {
                    if (managerId.ToLower() == inputManagerId.ToLower())
                    {
                        isManagerValid = true;
                        break;
                    }
                }
            }
            return isManagerValid;
        }

        private async Task<string> getUserName(Activity activity)
        {
            return activity.From.Name.Split(' ')[0];
        }

        private async Task<string> GetUserMailId(IDialogContext context, Activity activity)
        {
            var userEmailId = string.Empty;
            string emailKey = activity.From.Id + EmailKey;
            if (context.ConversationData.ContainsKey(emailKey))
            {
                userEmailId = context.ConversationData.GetValue<string>(emailKey);
            }
            else
            {
                // Fetch from roaster
                userEmailId = await GetUserEmailId(activity);
                context.ConversationData.SetValue(emailKey, userEmailId);
            }

            return userEmailId;
        }

        private async Task<bool> IsManagerMailIdPresent(IDialogContext context, Activity activity, string managerEmailKey, Employee employee)
        {
            if (employee != null && !string.IsNullOrEmpty(employee.ManagerEmailId))
            {
                return true;
            }
            return context.ConversationData.ContainsKey(managerEmailKey);
        }

        private async Task SetManagerId(IDialogContext context, string managerEmailKey, string profileKey, string managerEmailId)
        {
            var employee = context.ConversationData.GetValue<Employee>(profileKey);
            employee.ManagerEmailId = managerEmailId;
            await UpdateEmployeeInDB(context, employee);
            context.ConversationData.SetValue(managerEmailKey, managerEmailId);
        }

        private async Task<string> GetUserEmailId(Activity activity)
        {
            // Fetch the members in the current conversation
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var members = await connector.Conversations.GetConversationMembersAsync(activity.Conversation.Id);
            return members.Where(m => m.Id == activity.From.Id).First().AsTeamsChannelAccount().UserPrincipalName.ToLower();
        }

        private static async Task UpdateEmployeeInDB(IDialogContext context, Employee employee)
        {
            await DocumentDBRepository.UpdateItemAsync(employee.EmailId, employee);
            var profileKey = GetProfileKey(context.Activity);
            context.ConversationData.SetValue(profileKey, employee);
        }

        private static async Task SendMessageToAccountChannel(IDialogContext context, Activity activity, Attachment cardAttachment)
        {
            var channelData = context.Activity.GetChannelData<TeamsChannelData>();
            channelData.Channel = new ChannelInfo(ConfigurationManager.AppSettings["adminChanelId"], ConfigurationManager.AppSettings["adminChanelName"]);
            
             var message = Activity.CreateMessageActivity();
            message.Attachments.Add(cardAttachment);
            var conversationParameter = new ConversationParameters
            {
                IsGroup = true,
                ChannelData = new TeamsChannelData
                {
                    Channel = new ChannelInfo(channelData.Channel.Id)
                },
                Activity = (Activity)message
            };
            MicrosoftAppCredentials.TrustServiceUrl(activity.ServiceUrl, DateTime.MaxValue);
            var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));
            var response = await connectorClient.Conversations.CreateConversationAsync(conversationParameter);

            context.Done<object>(null);
        }
    }

}

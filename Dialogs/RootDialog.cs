using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using BotTest.SitecoreAPI;

namespace BotTest.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            // int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            // await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            if (!string.IsNullOrEmpty(activity.Text) && activity.Text.ToLower().Contains("airport"))
            {
                SitecoreApiClient sitecoreAPI = new SitecoreApiClient();
                var sitecoreResults = sitecoreAPI.GetByQuery("Name eq 'Home' and TemplateName eq 'Sample Item'", null);
                var builder = new StringBuilder("List of terminal inside airport ...");
                if (sitecoreResults != null)
                {
                    foreach (var item in sitecoreResults)
                    {
                        builder.Append(item.Name);
                        builder.AppendLine();
                    }
                }
            }
            else
            {

                await context.PostAsync($"Hi there! My name is Sitecore ChatOps!  Why don't you ask me a question?");

                var builder = new StringBuilder("Try asking 'Show me '...");
                builder.AppendLine();

                var rand = new Random();
                foreach (var suggestion in ExampleCommands.OrderBy(k => rand.Next()).Take(3))
                {
                    builder.AppendLine($"* {suggestion}");
                }
                await context.PostAsync(builder.ToString());
            }
            context.Wait(MessageReceivedAsync);
        }

        private static readonly string[] ExampleCommands =
       {
            "airport details",
            "terminal details",
            "offer on lounges",
            "lounges details"
        };
    }
}
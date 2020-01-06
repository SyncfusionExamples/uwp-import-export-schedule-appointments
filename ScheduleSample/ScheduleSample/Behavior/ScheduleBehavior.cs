using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Resources;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using ScheduleSample;
using Xamarin.Forms;
using Syncfusion.SfSchedule.XForms;

namespace ScheduleSample
{
    class ScheduleBehavior : Behavior<ContentPage>
    {
        private GraphServiceClient Client = null;
        private AuthenticationResult tokenRequest;
        private SfSchedule schedule;
        private Button export;
        private Button import;
        private ViewModel viewModel;
        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            schedule = bindable.FindByName<SfSchedule>("schedule");
            export = bindable.FindByName<Button>("export");
            import = bindable.FindByName<Button>("import");
            import.Clicked += ImportToSchedule;
            export.Clicked += ExportToOutlook;
        }

        private async void ExportToOutlook(object sender, EventArgs e)
        {
            this.Authenticate();

            this.viewModel = (schedule.BindingContext as ViewModel);

            foreach (var appointment in viewModel.Meetings)
            {
                var calEvent = new Event
                {
                    Subject = appointment.EventName,

                    Start = new DateTimeTimeZone
                    {
                        DateTime = appointment.From.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = "GMT Standard Time"
                    },
                    End = new DateTimeTimeZone()
                    {
                        DateTime = appointment.To.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = "GMT Standard Time"
                    },
                };
                await Client.Me.Events.Request().AddAsync(calEvent);
            }
        }

        private async void ImportToSchedule(object sender, EventArgs e)
        {
            this.Authenticate();

            this.viewModel = (schedule.BindingContext as ViewModel);
            var events = await Client.Me.Events.Request().GetAsync();
            var appointmentList = events.ToList();

            foreach (var appointment in appointmentList)
            {
                viewModel.Meetings.Add(new Meeting()
                {
                    From = Convert.ToDateTime(appointment.Start.DateTime).ToLocalTime(),
                    To = Convert.ToDateTime(appointment.End.DateTime).ToLocalTime(),
                    EventName = appointment.Subject
                });
            }
        }

        private void Authenticate()
        {
            Client = new GraphServiceClient("https://graph.microsoft.com/v1.0",
                                new DelegateAuthenticationProvider(async (requestMessage) =>
                                {
                                    if (App.IdentityClientApp.Users == null || App.IdentityClientApp.Users.Count() == 0)
                                        tokenRequest = await App.IdentityClientApp.AcquireTokenAsync(App.Scopes, App.UiParent);
                                    else
                                        tokenRequest = await App.IdentityClientApp.AcquireTokenSilentAsync(App.Scopes, App.IdentityClientApp.Users.FirstOrDefault());
                                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", tokenRequest.AccessToken);
                                }));
        }
    }
}

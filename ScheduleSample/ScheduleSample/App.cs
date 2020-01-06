

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ScheduleSample
{
    public class App : Application
    {
        public static PublicClientApplication IdentityClientApp;
        public static UIParent UiParent;
        //You need to replace your Application ID
        public static string ClientID = "61d2216e-06e6-4940-8c20-c3a923a40620";
        public static string[] Scopes = { "User.Read", "Calendars.Read", "Calendars.ReadWrite" };
        public App()
        {
            IdentityClientApp = new PublicClientApplication(ClientID);
            MainPage = new NavigationPage(new ScheduleSample.ScheduleView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

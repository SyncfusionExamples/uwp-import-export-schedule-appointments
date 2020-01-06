# How to import and export schedule appointments with Outlook calendar?

In SfSchedule, you can import and export the appointments to Outlook using Microsoft Graph.

**Step 1:**
The first step in creating an application that has access to personal information is to register it with the required service. This can be accomplished by signing into the Application Registration portal, by clicking the “Add an app button” and completing the required workflow. Note that you must add delegated permissions for User.Read, Calendars.Read and Calendars.ReadWrite. Once application registration is complete, make a note of the Application ID and URLs. 

**Step 2:**
In portable project, replace your application ID inside App.cs. 

```csharp
//You need to replace your Application ID 
public static string ClientID = "61d2216e-06e6-4940-8c20-c3a923a40620"; 
public static string[] Scopes = { "User.Read", "Calendars.Read", "Calendars.ReadWrite" };
```

**Step 3:**
In Android project, open AndroidManifest.xml and replace your URL in data inside intent-filter. 

```AXML
 <category android:name="android.intent.category.BROWSABLE" /> 
  <!--You need to replace your URL--> 
  <data android:scheme="msal61d2216e-06e6-4940-8c20-c3a923a40620" android:host="auth"/> 
</intent-filter> 
```

You also need to replace your URL in App.IdentityClientApp.RedirectUri in OnCreate override method inside MainActivity.cs.

```csharp
//You need to replace your URL 
App.IdentityClientApp.RedirectUri = "msal61d2216e-06e6-4940-8c20-c3a923a40620://auth"; 
App.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity); 
```

**Step 4:**
In iOS project, you need to register your custom URL scheme in Info.plist. Kindly refer the below steps

* Click Info.plist.
* Select ‘Advanced’ tab.
* Now, click ‘Add URL Type’.
* Enter the text ‘MASL’ in Identifier text block.
* Enter the URL in ‘URL Schemes’ text block.

You also need to replace your URL in App.IdentityClientApp.RedirectUri in FinishedLaunching override method inside AppDelegate.cs 

```csharp
LoadApplication(new App()); 
//You need to replace your URL 
App.IdentityClientApp.RedirectUri = "msal61d2216e-06e6-4940-8c20-c3a923a40620://auth"; 
return base.FinishedLaunching(app, options);     
```
In Universal Windows Platform (UWP) project, the Microsoft Authentication Library doesn’t require any specific platform modifications to handle the login process.

**Step 5:**
In our sample authentication is done by using GraphServiceClient, which will be responsible for calling all future Graph APIs. 
```csharp
Client = new GraphServiceClient("https://graph.microsoft.com/v1.0", 
                                new DelegateAuthenticationProvider(async (requestMessage) => 
                                { 
                                    if (App.IdentityClientApp.Users == null || App.IdentityClientApp.Users.Count() == 0) 
                                        tokenRequest = awaitApp.IdentityClientApp.AcquireTokenAsync(App.Scopes, App.UiParent); 
                                    else 
                                        tokenRequest = awaitApp.IdentityClientApp.AcquireTokenSilentAsync(App.Scopes, App.IdentityClientApp.Users.FirstOrDefault()); 
                                    requestMessage.Headers.Authorization = newAuthenticationHeaderValue("bearer", tokenRequest.AccessToken); 
                                }));
```

**Step 6:**
You can bind appointments to the Schedule control form other sources using AppointmentMapping support. You will need to create custom appointment collection from other sources using custom data model and all the properties of the data model should be mapped to the Schedule mapping properties. Kindly refer our online User Guide Documentation by using the below links.
```csharp
/// <summary>   
/// Represents data model properties.   
/// </summary>  
public class Meeting 
{
    public string EventName { get; set; }
    public string Organizer { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public bool AllDay { get; set; }
    public Color Color { get; set; }
}
```
**Links:**
* https://help.syncfusion.com/xamarin/sfschedule/data-bindings#mapping
* https://help.syncfusion.com/xamarin/sfschedule/data-bindings#creating-custom-appointments

**Step 7:**
Now, you can populate the appointments to the Schedule from the registered Outlook account by converting the events to appointments using AppointmentMapping support.
```csharp
var events = await Client.Me.Events.Request().GetAsync();
var appointmentList = events.ToList();
  
this.CreateColorCollection();
var randomTime = new Random();
  
foreach (var appointment in appointmentList)
{
    viewModel.Meetings.Add(new Meeting()
    {
        From = Convert.ToDateTime(appointment.Start.DateTime).ToLocalTime(),
        To = Convert.ToDateTime(appointment.End.DateTime).ToLocalTime(),
        EventName = appointment.Subject,
    });
}
```
You can also export the Schedule appointments to Outlook by converting it as events and each event can be populated to the registered Outlook account.

```csharp
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
```

You can also refer the below blogs to connect the Xamarin Forms application with Outlook using Microsoft Graph.

**Links:**
* https://blog.xamarin.com/enterprise-apps-made-easy-updated-libraries-apis/
* https://blog.xamarin.com/lets-schedule-meeting/

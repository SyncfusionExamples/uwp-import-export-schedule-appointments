using System;

using Xamarin.Forms;

namespace ScheduleSample
{
	/// <summary>   
	/// Represents data model properties.   
	/// </summary>  
	public class Meeting : ContentPage
	{
		public string EventName { get; set; }
		public string Organizer { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
        public bool AllDay { get; set; }
        public Color Color { get; set; }
    }
}


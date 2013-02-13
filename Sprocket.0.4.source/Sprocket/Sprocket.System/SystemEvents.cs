using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket
{
	[ModuleDescription("Handles and responds to basic system events such as shutdown.")]
	[ModuleTitle("System Events Manager")]
	public class SystemEvents : ISprocketModule
	{
		/// <summary>
		/// This event should be handled when the user's session is about to end and clean
		/// up tasks need to be performed, such as closing database connections. This does
		/// not necessarily mean the application is shutting down, just
		/// that resources allocated for a specific user session can be released. For non-web
		/// applications, unless designed specifically to persist between multiple user requests,
		/// this will generally be called right before the application is closed. For web
		/// applications, this is called during the HttpApplication's EndRequest event.
		/// </summary>
		public event EmptyHandler OnSessionShutDown;

		/// <summary>
		/// This event is a general notifier to inform the system that an exception has been
		/// thrown somewhere in the application. Logging, email notification or whatever is
		/// relevant should be handled here. This event fires when the NotifyExceptionThrown
		/// method is called.
		/// </summary>
		public event NotificationEventHandler<Exception> OnExceptionThrown;

		/// <summary>
		/// Internal use only. This method should only be called when a user is exiting the
		/// application. This does not necessarily mean the application is shutting down, just
		/// that resources allocated for a specific user session can be released. For non-web
		/// applications, unless designed specifically to persist between multiple user requests,
		/// this will generally be called right before the application is closed. For web
		/// applications, this should be called during the HttpApplication's EndRequest event.
		/// </summary>
		public void NotifySessionEnding()
		{
			if (OnSessionShutDown != null)
				OnSessionShutDown();
		}

		/// <summary>
		/// This event should be called once only when an exception is thrown to allow modules
		/// to perform any kind of error logging that they wish to perform.
		/// </summary>
		/// <param name="ex">The exception that was thrown</param>
		public void NotifyExceptionThrown(Exception ex)
		{
			if (OnExceptionThrown != null)
				OnExceptionThrown(ex);
		}

		public static SystemEvents Instance
		{
			get { return (SystemEvents)Core.Instance[typeof(SystemEvents)].Module; }
		}

		#region ISprocketModule Members

		public void AttachEventHandlers(ModuleRegistry registry)
		{
		}

		#endregion
	}
}

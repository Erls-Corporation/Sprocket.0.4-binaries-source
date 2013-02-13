using System;
using System.Collections.Generic;
using System.Text;
using Sprocket;

namespace Sprocket
{
	/// <summary>
	/// Designed for events which require no data to be shared.
	/// </summary>
	public delegate void EmptyHandler();

	/// <summary>
	/// Designed for events that signify some interrupable process
	/// </summary>
	/// <typeparam name="T">The target of the current operation</typeparam>
	/// <typeparam name="Result">A result to be passed back indicating if things should go ahead normally</typeparam>
	/// <param name="?"></param>
	public delegate void InterruptableEventHandler<T>(T source, Result result);
	public delegate void InterruptableEventHandler(Result result);

	/// <summary>
	/// Designed for events that only require a reference to a specific
	/// Sprocket module.
	/// </summary>
	/// <param name="module">The module for which the event is occurring.</param>
	public delegate void ModuleEventHandler(ISprocketModule module);

	/// <summary>
	/// Generic notification delegate for event handlers that serve simply to notify
	/// that some action has occurred without requiring a variable response once
	/// the event has completed.
	/// </summary>
	/// <typeparam name="T">The type of object for which the event is occurring</typeparam>
	/// <param name="source">The source of the event</param>
	public delegate void NotificationEventHandler<T>(T source);

	/// <summary>
	/// Generic notification delegate for event handlers that serve simply to notify
	/// that some action has occurred without requiring a variable response once
	/// the event has completed. An optional HandleFlag is provided so that the source
	/// of the event can be notified that the event was responded to, in case the source
	/// wants to behave in an alternate way as a result.
	/// </summary>
	/// <typeparam name="T">The type of object for which the event is occurring</typeparam>
	/// <param name="source">The source of the event</param>
	/// <param name="handled">An object to indicate whether or not the event was responded to</param>
	public delegate void RespondableEventHandler<T>(T source, HandleFlag handled);

	/// <summary>
	/// Same as NotificationEventHandler except designed for notification that data has been
	/// added or updated.
	/// </summary>
	/// <typeparam name="T">The type of object for which the event is occurring</typeparam>
	/// <param name="source">The source of the event</param>
	/// <param name="isNew">Specifies whether the saved data was for a new object or if it was a pre-existing object being updated.</param>
	public delegate void SavedNotificationEventHandler<T>(T source, bool isNew);
}

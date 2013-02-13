using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket
{
	/// <summary>
	/// Used as a generic event handler for events that are designed to notify
	/// Sprocket modules of some occurrence and allow those modules to signal to
	/// the originating module that the event should be cancelled.
	/// </summary>
	public class StateChangingEventArgs : EventArgs
	{
		private bool cancelStateChange = false;
		/// <summary>
		/// Gets or sets whether or not the state change should proceed. If it is set to true,
		/// the property ReasonForCancellation should also be specified to assist with providing
		/// a meaningful response to the user.
		/// </summary>
		public bool CancelStateChange
		{
			get { return cancelStateChange; }
			set { cancelStateChange = value; }
		}

		private string reason = "";
		/// <summary>
		/// Specifies the reason for cancelling the state change, if applicable.
		/// </summary>
		public string ReasonForCancellation
		{
			set { reason = value; }
			get { return reason; }
		}
	}

	public class BooleanStateChangingEventArgs : StateChangingEventArgs
	{
		private bool newState;
		/// <summary>
		/// Constructs a new StateChangingEventArgs object.
		/// </summary>
		/// <param name="newState">The state that the source is about to enter.</param>
		public BooleanStateChangingEventArgs(bool newState)
		{
			this.newState = newState;
		}

		/// <summary>
		/// Gets the state that the source is about to enter.
		/// </summary>
		public bool NewState
		{
			get { return newState; }
		}
	}

	public delegate void StateChangingEventHandler<T>(T source, StateChangingEventArgs args);
	public delegate void BooleanStateChangingEventHandler<T>(T source, BooleanStateChangingEventArgs args);
}

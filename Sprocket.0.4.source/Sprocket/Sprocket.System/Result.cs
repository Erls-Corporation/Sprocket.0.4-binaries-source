using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web;

namespace Sprocket
{
	/// <summary>
	/// Used for returning an indicator of success from a method call where
	/// a message is required if the call fails.
	/// </summary>
	public class Result : IJSONEncoder
	{
		protected bool succeeded = true;
		protected string message = "";

		/// <summary>
		/// Constructs a successful result indicator.
		/// </summary>
		public Result()
		{
		}

		/// <summary>
		/// Constructs an unsuccessful result indicator with the specified message.
		/// </summary>
		/// <param name="message">The reason for the failed call</param>
		public Result(string message)
		{
			this.succeeded = false;
			this.message = message;
		}

		/// <summary>
		/// Specifies whether or not the method call succeeded.
		/// </summary>
		public bool Succeeded
		{
			get { return succeeded; }
		}

		/// <summary>
		/// The reason for the method call failure, if Succeeded == false, otherwise blank.
		/// </summary>
		public string Message
		{
			get { return message; }
		}

		/// <summary>
		/// Sets Succeeded to false and Message to the supplied message.
		/// </summary>
		/// <param name="message">The reason for the failure</param>
		public void SetFailed(string message)
		{
			succeeded = false;
			this.message = message;
		}

		public virtual void WriteJSON(System.IO.StringWriter writer)
		{
			JSON.EncodeCustomObject(writer,
				new KeyValuePair<string, object>("Succeeded", Succeeded),
				new KeyValuePair<string, object>("Message", Message)
			);
		}
	}
}

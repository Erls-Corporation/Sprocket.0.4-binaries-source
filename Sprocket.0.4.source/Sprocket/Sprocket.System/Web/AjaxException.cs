using System;

namespace Sprocket.Web
{
	/// <summary>
	/// The AjaxException class is a special exception that makes error displaying on the client
	/// clean and graceful. When you throw an AjaxException during a call to an Ajax Method, the client
	/// will display a message box displaying the error but without all of the line numbers, stack
	/// trace information, etc.
	/// </summary>
	public class AjaxException : SprocketException
	{
		public AjaxException()
		{
		}

		public AjaxException(string message) : base(message)
		{
			
		}

		public AjaxException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public override string ToString()
		{
			return Message;
		}
	}

	public class AjaxSessionExpiredException : SprocketException
	{
		public AjaxSessionExpiredException()
		{
		}

		public AjaxSessionExpiredException(string message) : base(message)
		{
			
		}

		public AjaxSessionExpiredException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public override string ToString()
		{
			return Message;
		}
	}
}

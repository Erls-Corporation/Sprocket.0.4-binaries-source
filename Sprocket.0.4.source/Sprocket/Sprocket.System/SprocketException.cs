using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket
{
	/// <summary>
	/// Generic exception class for special exceptions thrown by Sprocket and
	/// Sprocket modules. All custom exceptions used by Sprocket modules should
	/// inherit from this class. The Sprocket system traps Sprocket Exceptions
	/// but not standard exceptions.
	/// </summary>
	public class SprocketException : Exception
	{
		public SprocketException()
		{
		}

		public SprocketException(string message) : base(message)
		{
			
		}

		public SprocketException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public override string ToString()
		{
			return Message;
		}
	}
}

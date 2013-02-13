using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket
{
	public static class SprocketDate
	{
		public static DateTime Now
		{
			get { return DateTime.Now.ToUniversalTime(); }
		}
	}
}

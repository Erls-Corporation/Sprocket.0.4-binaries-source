using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket
{
	public class HandleFlag
	{
		private bool flag = false;
		public bool Handled
		{
			get { return flag; }
		}

		public void Set()
		{
			flag = true;
		}
	}
}

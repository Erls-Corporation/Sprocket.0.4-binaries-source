using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Web;

using Sprocket;
using Sprocket.Data;
using Sprocket.Utility;

namespace Sprocket.Web.Merchant.PayPal
{
	public partial class PayPalTransactionResponse : IDisposable
	{
		public void LoadFromResponseText(string text)
		{
			string[] values = text.Split('\n');
			foreach (string s in values)
			{
				if (!s.Contains("="))
					continue;
				string[] kv = s.Split('=');
				Populate(kv[0], HttpUtility.UrlDecode(kv[1]));
			}
		}

		public void Dispose()
		{
		}
	}
}

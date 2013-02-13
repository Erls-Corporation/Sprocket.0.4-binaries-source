using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Sprocket.Web.Merchant.PayPal
{
	public static class InstantPaymentNotification
	{
		public static PayPalTransactionResponse Authenticate()
		{
			WebRequest w = WebRequest.Create(PayPal.PayPalPostURL);
			w.ContentType = "application/x-www-form-urlencoded";
			w.Method = "POST";
			string data = ComposeFormPostData("cmd","_notify-validate");
			NameValueCollection form = HttpContext.Current.Request.Form;
			for (int i = 0; i < form.Count; i++)
				data += "&" + ComposeFormPostData(form.GetKey(i), form[i]);
			Stream s = w.GetRequestStream();
			byte[] buf = Encoding.ASCII.GetBytes(data);
			s.Write(buf, 0, buf.Length);
			s.Close();
			HttpWebResponse r = (HttpWebResponse)w.GetResponse();
			StreamReader sr = new StreamReader(r.GetResponseStream());
			string response = HttpUtility.UrlDecode(sr.ReadToEnd());
			sr.Close();
			r.Close();
			if (!response.StartsWith("VERIFIED"))
				return null;
			PayPalTransactionResponse resp = new PayPalTransactionResponse();
			resp.Populate(HttpContext.Current.Request.Form);
			string appstr = "PayPal_IPN_RecentTXNIDs" + resp.Txn_type;
			Queue<string> recentIDs = HttpContext.Current.Application[appstr] as Queue<string>;
			if (recentIDs == null)
			{
				recentIDs = new Queue<string>();
				HttpContext.Current.Application[appstr] = recentIDs;
			}
			if (recentIDs.Count > 500) recentIDs.Dequeue();
			if (recentIDs.Contains(resp.Txn_id))
				return null;
			recentIDs.Enqueue(resp.Txn_id);
			try { resp.Save(); }
			catch(Exception e) { } // thrown usually on duplicate id, which we try to avoid via the queue above (less hits to database)
			return resp;
		}

		private static string ComposeFormPostData(string key, string value)
		{
			return key + "=" + HttpUtility.UrlEncode(value);
		}
	}
}

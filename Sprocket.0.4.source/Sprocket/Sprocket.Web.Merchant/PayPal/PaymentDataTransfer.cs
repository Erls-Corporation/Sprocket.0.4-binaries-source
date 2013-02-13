using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Sprocket.Web.Merchant.PayPal
{
	public sealed class PaymentDataTransfer
	{
		private string transactionToken = null;
		private decimal? amount = null;
		private string countryCode = null;
		private string sig = null;

		/*
		private PayPalPaymentStatus? status = null;
		public PayPalPaymentStatus PaymentStatus
		{
			get { return status.Value; }
		}
		 * */

		public decimal Amount
		{
			get { return amount.Value; }
		}

		public string CountryCode
		{
			get { return countryCode; }
		}
		
		public void ReadInitialResponse()
		{
			HttpContext c = HttpContext.Current;
			sig = c.Request.QueryString["sig"];
			transactionToken = c.Request.QueryString["tx"];
			amount = decimal.Parse(c.Request.QueryString["amt"]);
			countryCode = c.Request.QueryString["cc"];
		}

		private string response = null;
		private PayPalTransactionResponse ppresponse = null;
		public bool AuthenticateResponse()
		{
			WebRequest w = WebRequest.Create(PayPal.PayPalPostURL);
			w.ContentType = "application/x-www-form-urlencoded";
			w.Method = "POST";
			string data
				= ComposeFormPostData("cmd", "_notify-synch") + "&"
				+ ComposeFormPostData("tx", transactionToken) + "&"
				+ ComposeFormPostData("at", PayPal.IdentityToken)
				;
			Stream s = w.GetRequestStream();
			byte[] buf = Encoding.ASCII.GetBytes(data);
			s.Write(buf, 0, buf.Length);
			s.Close();
			HttpWebResponse r = (HttpWebResponse)w.GetResponse();
			StreamReader sr = new StreamReader(r.GetResponseStream());
			response = HttpUtility.UrlDecode(sr.ReadToEnd());
			sr.Close();
			r.Close();
			return response.StartsWith("SUCCESS");
		}

		public PayPalTransactionResponse TransactionResponse
		{
			get
			{
				if (ppresponse == null)
				{
					ppresponse = new PayPalTransactionResponse();
					ppresponse.LoadFromResponseText(response);
				}
				return ppresponse;
			}
		}

		public PayPalTransactionResponse StoreResponseToDatabase()
		{
			TransactionResponse.Save();
			return ppresponse;
		}

		private string ComposeFormPostData(string key, string value)
		{
			return key + "=" + HttpUtility.UrlEncode(value);
		}
	}
}

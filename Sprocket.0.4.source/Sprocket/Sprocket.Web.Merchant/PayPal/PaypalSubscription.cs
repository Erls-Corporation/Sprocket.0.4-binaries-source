using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Sprocket.Web;

namespace Sprocket.Web.Merchant.PayPal
{
	public class PaypalSubscription
	{
		private string itemName = "";
		private string returnURL = WebUtility.BaseURL;
		private string cancelURL = WebUtility.BaseURL + "paypal-cancel/";
		private string notifyURL = PayPal.PayPalIpnUrl;
		private double trialPeriodPrice = 0;
		private int trialPeriodSize = 1;
		private PayPalSubscriptionPeriodUnit trialPeriodUnit = PayPalSubscriptionPeriodUnit.Month;
		private double subscriptionPrice = 1;
		private int subscriptionPeriodSize = 1;
		private PayPalSubscriptionPeriodUnit subscriptionPeriodUnit;
		private string customValue = "";
		private int itemNumber = 0;
		private string logoURL = "";
		private PayPalSubscriptionEditMode editMode = PayPalSubscriptionEditMode.CreateOrModify;

		public PayPalSubscriptionEditMode EditMode
		{
			get { return editMode; }
			set { editMode = value; }
		}

		/// <summary>
		/// The URL which will receive instant payment notifications automatically when aspects of the subscription change
		/// </summary>
		public string NotifyURL
		{
			get { return notifyURL; }
			set { notifyURL = value; }
		}

		/// <summary>
		/// The URL of the company logo to use
		/// </summary>
		public string LogoURL
		{
			get { return logoURL; }
			set { logoURL = value; }
		}

		/// <summary>
		/// The item reference number
		/// </summary>
		public int ItemNumber
		{
			get { return itemNumber; }
			set { itemNumber = value; }
		}

		/// <summary>
		/// Passed back when the payment is made. Max 255 characters.
		/// </summary>
		public string CustomValue
		{
			get { return customValue; }
			set { customValue = value; }
		}

		/// <summary>
		/// The subscription billing period time unit
		/// </summary>
		public PayPalSubscriptionPeriodUnit SubscriptionPeriodUnit
		{
			get { return subscriptionPeriodUnit; }
			set { subscriptionPeriodUnit = value; }
		}

		/// <summary>
		/// The number of time units that make up the billing period
		/// </summary>
		public int SubscriptionPeriodSize
		{
			get { return subscriptionPeriodSize; }
			set { subscriptionPeriodSize = value; }
		}

		/// <summary>
		/// The price of the subscription per time unit
		/// </summary>
		public double SubscriptionPrice
		{
			get { return subscriptionPrice; }
			set { subscriptionPrice = value; }
		}

		/// <summary>
		/// Size of the trial period time unit (see TrialPeriodSize)
		/// </summary>
		public PayPalSubscriptionPeriodUnit TrialPeriodUnit
		{
			get { return trialPeriodUnit; }
			set { trialPeriodUnit = value; }
		}

		/// <summary>
		/// Number of time units that the trial period should last for
		/// </summary>
		public int TrialPeriodSize
		{
			get { return trialPeriodSize; }
			set { trialPeriodSize = value; }
		}

		/// <summary>
		/// Cost of the trial period. Set to 0 to make it free.
		/// </summary>
		public double TrialPeriodPrice
		{
			get { return trialPeriodPrice; }
			set { trialPeriodPrice = value; }
		}

		/// <summary>
		/// A URL where the user is to be sent if the transaction is cancelled.
		/// </summary>
		public string CancelURL
		{
			get { return cancelURL; }
			set { cancelURL = value; }
		}

		/// <summary>
		/// The URL where the user is returned to after completing the transaction.
		/// Variables detailing the transaction are posted here.
		/// </summary>
		public string ReturnURL
		{
			get { return returnURL; }
			set { returnURL = value; }
		}

		/// <summary>
		/// A descriptive name for the type of subscription
		/// </summary>
		public string ItemName
		{
			get { return itemName; }
			set { itemName = value; }
		}

		private List<KeyValuePair<string, object>> GetPostVars()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			list.Add(new KeyValuePair<string, object>("cmd",			PayPalEnumToString.From(PayPalCommand.Subscription)));
			list.Add(new KeyValuePair<string, object>("business",		PayPal.AccountAddress));
			list.Add(new KeyValuePair<string, object>("item_name",		itemName));
			list.Add(new KeyValuePair<string, object>("item_number",	itemNumber));
			list.Add(new KeyValuePair<string, object>("return",			returnURL));
			list.Add(new KeyValuePair<string, object>("notify_url",		notifyURL));
			list.Add(new KeyValuePair<string, object>("cancel_return",	cancelURL));
			list.Add(new KeyValuePair<string, object>("image_url",		logoURL));
			list.Add(new KeyValuePair<string, object>("rm",				2));
			list.Add(new KeyValuePair<string, object>("no_shipping",	1));

			if (trialPeriodSize > 0)
			{
				list.Add(new KeyValuePair<string, object>("a1", trialPeriodPrice));
				list.Add(new KeyValuePair<string, object>("p1", trialPeriodSize));
				list.Add(new KeyValuePair<string, object>("t1", PayPalEnumToString.From(trialPeriodUnit)));
			}

			list.Add(new KeyValuePair<string, object>("a3",			subscriptionPrice));
			list.Add(new KeyValuePair<string, object>("p3",			subscriptionPeriodSize));
			list.Add(new KeyValuePair<string, object>("t3",			PayPalEnumToString.From(subscriptionPeriodUnit)));
			list.Add(new KeyValuePair<string, object>("src",		1)); // payment will recur at end of billing cycle (0 means at end of billing cycle, payment does not recur)
			list.Add(new KeyValuePair<string, object>("sra",		1)); // payments will retry twice more after failure
			list.Add(new KeyValuePair<string, object>("no_note",	1)); // required internall by paypal
			list.Add(new KeyValuePair<string, object>("custom",		customValue));
			list.Add(new KeyValuePair<string, object>("modify",		PayPalEnumToString.From(editMode)));

			return list;
		}

		public string GetFormFields()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, object> kvp in GetPostVars())
				sb.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", kvp.Key, HttpUtility.HtmlEncode(kvp.Value.ToString()));
			return sb.ToString();
		}

		public static string GetCancelLink(string paypalMerchantEmail)
		{
			return PayPal.PayPalPostURL + "?cmd=_subscr-find&alias=" + paypalMerchantEmail;
		}
	}
}

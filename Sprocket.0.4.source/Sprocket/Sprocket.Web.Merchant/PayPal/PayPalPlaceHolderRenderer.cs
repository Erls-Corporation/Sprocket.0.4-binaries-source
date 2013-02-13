using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Pages;
using Sprocket.Web.Merchant.PayPal;

namespace Sprocket.Web.Merchant.PayPal
{
	public class PayPalPlaceHolderRenderer : IPlaceHolderRenderer
	{
		public string Render(PlaceHolder placeHolder, PageEntry pageEntry, System.Xml.XmlDocument content, Stack<string> placeHolderStack, out bool containsCacheableContent)
		{
			containsCacheableContent = true;
			switch (placeHolder.Expression.ToLower())
			{
				case "action":
					return PayPal.PayPalPostURL;

				case "subscribetest":

					PaypalSubscription pps = new PaypalSubscription();
					pps.CustomValue = Guid.NewGuid().ToString();
					pps.ItemName = "Test Subscription";
					pps.ItemNumber = 400;
					pps.NotifyURL = "http://snowdevil78.dyndns.org/prospector/public/paypal-ipn-process/";
					pps.SubscriptionPeriodSize = 3;
					pps.SubscriptionPeriodUnit = PayPalSubscriptionPeriodUnit.Day;
					pps.SubscriptionPrice = 10;
					pps.TrialPeriodSize = 0;
					pps.EditMode = PayPalSubscriptionEditMode.ModifyOnly;
					return pps.GetFormFields();

				default:
					return "[PayPalPlaceHolderRenderer error: Expression \"" + placeHolder.Expression + "\" not recognised]";
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

using Sprocket.Web.CMS.Pages;
using Sprocket;
using Sprocket.Data;
using Sprocket;
using Sprocket.Utility;

namespace Sprocket.Web.Merchant.PayPal
{
	[ModuleDependency("WebEvents")]
	[ModuleDependency("SprocketSettings")]
	[ModuleDescription("Encapsulates PayPal features and merchant facilities")]
	public sealed class PayPal : ISprocketModule, IDataHandlerModule
	{
		public event NotificationEventHandler<PayPalTransactionResponse> OnTransactionResponse;
		public event NotificationEventHandler<PayPalTransactionResponse> OnInstantPaymentNotification;

		void WebEvents_OnLoadRequestedPath(System.Web.HttpApplication app, string sprocketPath, string[] pathSections, HandleFlag handled)
		{
			if (!IntegrationEnabled)
				return;

			switch (sprocketPath)
			{
				case "paypal-ipn-process":
					using (PayPalTransactionResponse resp = InstantPaymentNotification.Authenticate())
					{
						if (OnInstantPaymentNotification != null && resp != null)
							OnInstantPaymentNotification(resp);
					}

					break;

				case "paypal-trans-return":
					using (PayPalTransactionResponse resp = TransactionReturn())
					{
						if (OnTransactionResponse != null && resp != null)
							OnTransactionResponse(resp);
					}
					break;

				default:
					return;
			}
			handled.Set();
		}

		PayPalTransactionResponse TransactionReturn()
		{
			HttpContext c = HttpContext.Current;
			PaymentDataTransfer pdt = new PaymentDataTransfer();
			pdt.ReadInitialResponse();
			if (!pdt.AuthenticateResponse())
				return null;
			pdt.StoreResponseToDatabase();
			return pdt.TransactionResponse;
		}

		void PageRequestHandler_OnRegisteringPlaceHolderRenderers(Dictionary<string, IPlaceHolderRenderer> placeHolderRenderers)
		{
			placeHolderRenderers.Add("paypal", new PayPalPlaceHolderRenderer());
		}

		#region Settings Properties
		private static string _ppsetting(string suffix)
		{
			return SprocketSettings.GetValue((TestMode ? "PayPalTest" : "PayPal") + suffix);
		}

		public static bool TestMode
		{
			get { return SprocketSettings.GetBooleanValue("PayPalTestMode"); }
		}

		public static string PayPalIpnUrl
		{
			get { return SprocketSettings.GetValue("PayPal-IPN-URL"); }
		}

		public static bool IntegrationEnabled
		{
			get { return SprocketSettings.GetBooleanValue("PayPalIntegration"); }
		}

		public static string AccountAddress
		{
			get { return _ppsetting("AccountAddress"); }
		}

		public static string IdentityToken
		{
			get { return _ppsetting("IdentityToken"); }
		}

		public static string PayPalPostURL
		{
			get { return "https://www." + (TestMode ? "sandbox." : "") + "paypal.com/cgi-bin/webscr"; }
		}

		public static PayPal Instance
		{
			get { return (PayPal)SystemCore.Instance["PayPal"]; }
		}

		#endregion

		#region SprockSettings OnCheckSettings
		void Settings_OnCheckingSettings(SprocketSettings.SettingsErrors errors)
		{
			if (!IntegrationEnabled) 
				return;

			if (TestMode)
			{
				if (SprocketSettings.GetValue("PayPalTestIdentityToken") == null)
				{
					errors.Add("PayPal", "PayPalTestMode setting has been specified, thus a value is required for PayPalTestIdentityToken. This is the PayPal-supplied identity token for use with the PayPal Sandbox development environment. See developer.paypal.com for more info.");
					errors.SetCriticalError();
				}
				if (SprocketSettings.GetValue("PayPalTestAccountAddress") == null)
				{
					errors.Add("PayPal", "PayPalTestMode setting has been specified, thus a value is required for PayPalTestAccountAddress. This is a test PayPal account address for use with the PayPal Sandbox development environment. See developer.paypal.com for more info.");
					errors.SetCriticalError();
				}
			}
			else
			{
				if (SprocketSettings.GetValue("PayPalIdentityToken") == null)
				{
					errors.Add("PayPal", "The PayPalTestMode setting is disabled and the PayPalIntegration setting is enabled, thus a value is required for PayPalIdentityToken. This is the PayPal-supplied identity token for authenticating PayPal responses. See developer.paypal.com for more info.");
					errors.SetCriticalError();
				}
				if (SprocketSettings.GetValue("PayPalAccountAddress") == null)
				{
					errors.Add("PayPal", "The PayPalTestMode setting is disabled and the PayPalIntegration setting is enabled, thus a value is required for PayPalAccountAddress. This is the PayPal account address that is to receive transaction payments. See developer.paypal.com for more info.");
					errors.SetCriticalError();
				}
			}
		}
		#endregion

		#region ISprocketModule Members

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(WebEvents_OnLoadRequestedPath);
			SprocketSettings.Instance.OnCheckingSettings += new SprocketSettings.CheckSettingsHandler(Settings_OnCheckingSettings);
			if (registry.IsRegistered("PageRequestHandler"))
				PageRequestHandler.Instance.OnRegisteringPlaceHolderRenderers += new PageRequestHandler.RegisteringPlaceHolderRenderers(PageRequestHandler_OnRegisteringPlaceHolderRenderers);
		}

		public void Initialise(ModuleRegistry registry)
		{
		}

		public string RegistrationCode
		{
			get { return "PayPal"; }
		}

		public string Title
		{
			get { return "PayPal Module"; }
		}

		#endregion

		#region IDataHandlerModule Members

		public void ExecuteDataScripts()
		{
			((SqlDatabase)Database.Main).ExecuteScript(ResourceLoader.LoadTextResource("Sprocket.Web.Merchant.PayPal.Database.tables.sql"));
			((SqlDatabase)Database.Main).ExecuteScript(ResourceLoader.LoadTextResource("Sprocket.Web.Merchant.PayPal.Database.procs.sql"));
		}

		#endregion
	}
}

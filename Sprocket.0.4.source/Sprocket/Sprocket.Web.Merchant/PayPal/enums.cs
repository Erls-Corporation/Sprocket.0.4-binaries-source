using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.Merchant.PayPal
{
	public enum PayPalTransactionType
	{
		/// <summary>
		/// Transaction created by a customer either (a) via the PayPal Shopping Cart feature or
		/// (b) via Express Checkout when the cart contains multiple items.
		/// </summary>
		Cart,
		/// <summary>
		/// Transaction created by Express Checkout when the customer's cart contains a single item
		/// </summary>
		ExpressCheckout,
		/// <summary>
		/// Transaction created by customer from the Send Money tab on the PayPal website
		/// </summary>
		SendMoney,
		/// <summary>
		/// Transaction created with the Virtual Terminal
		/// </summary>
		VirtualTerminal,
		/// <summary>
		/// Transaction created by customer via Buy Now, Donation or Auction Smart Logos
		/// </summary>
		WebAccept,
		/// <summary>
		/// This payment was sent via Mass Payment
		/// </summary>
		MassPay,
		/// <summary>
		/// Subscription payment failure
		/// </summary>
		SubscriptionFailed,
		/// <summary>
		/// Subscription cancellation
		/// </summary>
		SubscriptionCancelled,
		/// <summary>
		/// Subscription payment successful
		/// </summary>
		SubscriptionPayment,
		/// <summary>
		/// Subscription's end of term has been reached
		/// </summary>
		SubscriptionEndOfTerm,
		/// <summary>
		/// The subscription has been modified
		/// </summary>
		SubscriptionModification,
		/// <summary>
		/// A new dispute case has been registered
		/// </summary>
		NewCase,
		/// <summary>
		/// Some transaction type that did not exist when this code was written
		/// </summary>
		Unknown
	}

	public enum PayPalAddressStatus
	{
		Confirmed,
		Unconfirmed
	}

	public enum PayPalPayerStatus
	{
		Verified,
		Unverified
	}

	public enum PayPalAuthorizationResponse
	{
		Success,
		Failed
	}

	public enum PayPalCommand
	{
		/// <summary>
		/// A Buy Now or Donations button
		/// </summary>
		BuyNowOrDonation,
		/// <summary>
		/// A shopping cart
		/// </summary>
		ShoppingCart,
		/// <summary>
		/// For prepopulating PayPal account signup. Requires use of the redirect_cmd hidden field.
		/// </summary>
		PrePopulatePaypalAccountSignup,
		/// <summary>
		/// Used for creating or modifying PayPal subscriptions
		/// </summary>
		Subscription
	}

	public enum PayPalPaymentStatus
	{
		/// <summary>
		/// A reversal has been canceled. For example, you won a dispute with the customer, and the
		/// funds for the transaction that was reversed have been returned to you.
		/// </summary>
		CanceledReversal,
		/// <summary>
		/// The payment has been completed, and the funds have been added successfully to your
		/// account balance.
		/// </summary>
		Completed,
		/// <summary>
		/// You denied the payment. This happens only if the payment was previously pending because
		/// of possible reasons described for the PendingReason element.
		/// </summary>
		Denied,
		/// <summary>
		/// This authorization has expired and cannot be captured.
		/// </summary>
		Expired,
		/// <summary>
		/// The payment has failed. This happens only if the payment was made from your customer's
		/// bank account.
		/// </summary>
		Failed,
		/// <summary>
		/// The transaction is in process of authorization and capture.
		/// </summary>
		InProgress,
		/// <summary>
		/// The transaction has been partially refunded.
		/// </summary>
		PartiallyRefunded,
		/// <summary>
		/// The payment is pending. See pending_ re for more information.
		/// </summary>
		Pending,
		/// <summary>
		/// A payment has been accepted.
		/// </summary>
		Processed,
		/// <summary>
		/// You refunded the payment.
		/// </summary>
		Refunded,
		/// <summary>
		/// A payment was reversed due to a chargeback or other type of reversal. The funds have been
		/// removed from your account balance and returned to the buyer. The reason for the reversal
		/// is specified in the ReasonCode element.
		/// </summary>
		Reversed,
		/// <summary>
		/// This authorization has been voided.
		/// </summary>
		Voided
	}

	public enum PayPalSubscriptionPeriodUnit
	{
		Day,
		Week,
		Month,
		Year
	}

	public enum PayPalSubscriptionEditMode
	{
		CreateOnly,
		CreateOrModify,
		ModifyOnly
	}

	public static class PayPalEnumToString
	{
		public static string From(PayPalCommand cmd)
		{
			switch (cmd)
			{
				case PayPalCommand.ShoppingCart:
					return "_cart";
				case PayPalCommand.BuyNowOrDonation:
					return "_xclick";
				case PayPalCommand.PrePopulatePaypalAccountSignup:
					return "_ext-enter";
				case PayPalCommand.Subscription:
					return "_xclick-subscriptions";
			}
			return null;
		}

		public static string From(PayPalSubscriptionPeriodUnit unit)
		{
			switch (unit)
			{
				case PayPalSubscriptionPeriodUnit.Day:
					return "D";
				case PayPalSubscriptionPeriodUnit.Week:
					return "W";
				case PayPalSubscriptionPeriodUnit.Month:
					return "M";
				case PayPalSubscriptionPeriodUnit.Year:
					return "Y";
			}
			return null;
		}

		public static string From(PayPalSubscriptionEditMode type)
		{
			switch (type)
			{
				case PayPalSubscriptionEditMode.CreateOnly:
					return "0";
				case PayPalSubscriptionEditMode.CreateOrModify:
					return "1";
				case PayPalSubscriptionEditMode.ModifyOnly:
					return "2";
			}
			return null;
		}
	}
}

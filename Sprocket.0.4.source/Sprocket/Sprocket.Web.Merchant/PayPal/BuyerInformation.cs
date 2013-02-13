using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Reflection;

namespace Sprocket.Web.Merchant.PayPal
{
	public sealed class BuyerInformation
	{
		private BuyerAddress address = new BuyerAddress();
		private string first_name = "";
		private string last_name = "";
		private string payer_business_name = "";
		private string payer_email = "";
		private string payer_id = "";
		private string payer_status = "";
		private string residence_country = "";

		#region Properties
		/// <summary>
		/// Address information for the customer
		/// </summary>
		public BuyerAddress Address
		{
			get { return address; }
		}

		/// <summary>
		/// The customer's first name
		/// </summary>
		public string FirstName
		{
			get { return first_name; }
			set { first_name = value; }
		}

		/// <summary>
		/// The customer's last name
		/// </summary>
		public string LastName
		{
			get { return last_name; }
			set { last_name = value; }
		}

		/// <summary>
		/// The customer's company name, if customer represents a business
		/// </summary>
		public string PayerBusinessName
		{
			get { return payer_business_name; }
			set { payer_business_name = value; }
		}

		/// <summary>
		/// Customer's primary email address. Use this email to provide any credits.
		/// </summary>
		public string PayerEmail
		{
			get { return payer_email; }
			set { payer_email = value; }
		}

		/// <summary>
		/// Unique customer ID
		/// </summary>
		public string PayerID
		{
			get { return payer_id; }
			set { payer_id = value; }
		}

		/// <summary>
		/// Whether or not the customer has a verified PayPal account
		/// </summary>
		public PayPalPayerStatus PayerStatus
		{
			get { return payer_status == "verified" ? PayPalPayerStatus.Verified : PayPalPayerStatus.Unverified; }
			set { payer_status = value == PayPalPayerStatus.Verified ? "verified" : "unverified"; }
		}

		/// <summary>
		/// Whether or not the customer has a verified PayPal account (valid values are "verified" or "unverified")
		/// </summary>
		public string PayerStatusCode
		{
			get { return payer_status; }
			set
			{
				if(!(payer_status == "verified" || payer_status == "unverified"))
					throw new Exception("Failed to set to paypal payer_status code. \"" + value + "\" is not a valid value.");
				payer_status = value;
			}
		}

		/// <summary>
		/// Two-character ISO 3166 country code
		/// </summary>
		public string ResidenceCountry
		{
			get { return residence_country; }
			set { residence_country = value; }
		}
		#endregion

		#region BuyerAddress class
		public sealed class BuyerAddress
		{
			private string address_city = "";
			private string address_country = "";
			private string address_country_code = "";
			private string address_name = "";
			private string address_state = "";
			private string address_status = "";
			private string address_street = "";
			private string address_zip = "";

			#region Properties
			/// <summary>
			/// Gets or sets the city of the customer's address
			/// </summary>
			public string City
			{
				get { return address_city; }
				set { address_city = value; }
			}

			/// <summary>
			/// Gets or sets the country of the customer's address
			/// </summary>
			public string Country
			{
				get { return address_country; }
				set { address_country = value; }
			}

			/// <summary>
			/// Gets or sets the two-character ISO 3166 country code of the customer's address
			/// </summary>
			public string CountryCode
			{
				get { return address_country_code; }
				set { address_country_code = value; }
			}

			/// <summary>
			/// The name used with the address (included when the customer provides a gift address)
			/// </summary>
			public string Name
			{
				get { return address_name; }
				set { address_name = value; }
			}

			/// <summary>
			/// The state of the customer's address
			/// </summary>
			public string State
			{
				get { return address_state; }
				set { address_state = value; }
			}

			/// <summary>
			/// The customer's street address
			/// </summary>
			public string Street
			{
				get { return address_street; }
				set { address_street = value; }
			}

			/// <summary>
			/// The customer's zip code
			/// </summary>
			public string Zip
			{
				get { return address_zip; }
				set { address_zip = value; }
			}

			/// <summary>
			/// Whether the customer provided a confirmed or unconfirmed address
			/// </summary>
			public PayPalAddressStatus Status
			{
				get { return address_status == "confirmed" ? PayPalAddressStatus.Confirmed : PayPalAddressStatus.Unconfirmed; }
				set { address_status = value == PayPalAddressStatus.Confirmed ? "confirmed" : "unconfirmed"; }
			}

			/// <summary>
			/// Whether the customer provided a confirmed or unconfirmed address (must be "confirmed" or "unconfirmed")
			/// </summary>
			public string StatusCode
			{
				get { return address_status; }
				set
				{
					if (!(value == "confirmed" || value == "unconfirmed"))
						throw new Exception("Failed to set to paypal address_status code. \"" + value + "\" is not a valid value.");
					address_status = value;
				}
			}
			#endregion
		}
		#endregion

		public void Populate(NameValueCollection nvc)
		{
			foreach (string key in nvc.AllKeys)
				Populate(key, nvc[key]);
		}

		public bool Populate(string key, string value)
		{
			switch (key)
			{
				case "first_name": first_name = value; break;
				case "last_name": last_name = value; break;
				case "payer_business_name": payer_business_name = value; break;
				case "payer_email": payer_email = value; break;
				case "payer_id": payer_id = value; break;
				case "payer_status": payer_status = value; break;
				case "residence_country": residence_country = value; break;
				case "address_city": Address.City = value; break;
				case "address_country": Address.Country = value; break;
				case "address_country_code": Address.CountryCode = value; break;
				case "address_name": Address.Name = value; break;
				case "address_state": Address.State = value; break;
				case "address_status": Address.StatusCode = value; break;
				case "address_street": Address.Street = value; break;
				case "address_zip": Address.Zip = value; break;
				default: return false;
			}
			return true;
		}
	}
}

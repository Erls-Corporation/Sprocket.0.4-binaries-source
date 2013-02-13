using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace Sprocket.Web.Controls
{
	public class AjaxFormField : IRankable
	{
		protected string fieldHTML = "";
		protected AjaxJavaScript runScript = null;
		protected string clientValidationFunctionReference = "";

		public AjaxFormField() { }
		public AjaxFormField(string fieldHTML, AjaxJavaScript runScript, string clientValidationFunctionName, int rank)
		{
			FieldHTML = fieldHTML;
			RunScript = runScript;
			ClientValidationFunctionReference = clientValidationFunctionName;
			this.rank = rank;
		}

		/// <summary>
		/// Gets or sets the name of the javascript function that will be called when
		/// the form is validating itself on the client side. The function will be
		/// expected to return true or false to indicate whether or not the field has
		/// a valid value. If blank, this field will not be validated on the client side.
		/// </summary>
		public string ClientValidationFunctionReference
		{
			get { return clientValidationFunctionReference == null ? "" : clientValidationFunctionReference; }
			set { clientValidationFunctionReference = value; }
		}
		
		/// <summary>
		/// Gets or sets the script that will be executed once the FieldHTML property
		/// has been written to the page.
		/// </summary>
		public AjaxJavaScript RunScript
		{
			get { return runScript; }
			set { runScript = value; }
		}

		/// <summary>
		/// Gets or sets the html that will be written to the form for this field.
		/// </summary>
		public string FieldHTML
		{
			get { return fieldHTML; }
			set { fieldHTML = value; }
		}

		/// <summary>
		/// Inherited classes should override this member and return the full html
		/// of the field to be written to the page.
		/// </summary>
		public virtual string FullFieldHTML
		{
			get { return fieldHTML; }
		}

		public virtual string FullFieldJavaScript
		{
			get { return runScript == null ? "" : runScript.ToString(); }
		}

		protected int rank = 0;
		public int Rank
		{
			set { rank = value; }
			get { return rank; }
		}
	}

	public class AjaxFormStandardField : AjaxFormField
	{
		protected string fieldName = "", fieldLabel = "", fieldError = "";
		protected bool validateOnServer = false;

		public AjaxFormStandardField() { }

		public AjaxFormStandardField(string fieldLabel, string fieldName, string fieldHTML, AjaxJavaScript runScript, string clientValidationFunctionReference, bool validateOnServer, int rank)
		{
			SetFields(fieldLabel, fieldName, fieldHTML, runScript, clientValidationFunctionReference, validateOnServer, rank);
		}

		protected void SetFields(string fieldLabel, string fieldName, string fieldHTML, AjaxJavaScript runScript, string clientValidationFunctionReference, bool validateOnServer, int rank)
		{
			this.fieldLabel = fieldLabel;
			this.fieldName = fieldName;
			this.fieldHTML = fieldHTML;
			this.validateOnServer = validateOnServer;
			this.runScript = runScript;
			this.rank = rank;
			this.clientValidationFunctionReference = clientValidationFunctionReference;
		}

		protected bool ValidateOnServer
		{
			get { return validateOnServer; }
			set { validateOnServer = value; }
		}

		public string FieldLabel
		{
			get { return fieldLabel; }
			set { fieldLabel = value; }
		}

		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}

		public string FieldError
		{
			get { return fieldError; }
			set { fieldError = value; }
		}

		public override string FullFieldHTML
		{
			get
			{
				return string.Format("<div class=\"ajaxform-row\" id=\"ajaxform-row-{0}\"> " +
					"<span class=\"label\" id=\"ajaxform-label-{0}\">{1}</span>" +
					"<span class=\"field\" id=\"ajaxform-field-{0}\">{2}</span>" +
					"<span class=\"error\" id=\"ajaxform-error-{0}\">{3}</span>" +
					"<span class=\"rowend\"></span>" +
					"</div>", fieldName, fieldLabel, fieldHTML, fieldError);
			}
		}

		public override string FullFieldJavaScript
		{
			get
			{
				if (ClientValidationFunctionReference.Length == 0 && !validateOnServer)
					return "";
				return string.Format("{0}\r\nAjaxFormValidator.AddFieldValidationCheck('{1}','ajaxform-error-{1}',{2},{3});\r\n",
					runScript,
					FieldName,
					validateOnServer.ToString().ToLower(),
					(ClientValidationFunctionReference.Length == 0 ? "null" : ClientValidationFunctionReference));
			}
		}
	}

	public class AjaxFormInputField : AjaxFormStandardField
	{
		private int? maxLength;
		private bool readOnly;
		private string fieldValue, className, style;

		public AjaxFormInputField(string fieldLabel, string fieldName, int? maxLength, bool readOnly, string className, string style, string fieldValue, AjaxJavaScript runScript, string clientValidationFunctionReference, bool validateOnServer, int rank)
		{
			this.maxLength = maxLength;
			this.readOnly = readOnly;
			this.fieldValue = fieldValue;
			this.className = className;
			this.style = style;
			this.fieldName = fieldName;
			this.fieldLabel = fieldLabel;
			this.fieldName = fieldName;
			this.validateOnServer = validateOnServer;
			this.runScript = runScript;
			this.rank = rank;
			this.clientValidationFunctionReference = clientValidationFunctionReference;
			this.fieldHTML = Generate();
		}

		private string Generate()
		{
			string fld = "<input type=\"text\" name=\"" + fieldName + "\" id=\"" + fieldName + "\"";
			if (maxLength != null) fld += " maxlength=\"" + maxLength + "\"";
			if (className != null) fld += " class=\"" + className + "\"";
			if (style != null) fld += " style=\"" + style + "\"";
			if (readOnly) fld += " disabled=\"true\"";
			if (fieldValue != null) fld += " value=\"" + HttpUtility.HtmlEncode(fieldValue) + "\"";
			fld += " />";
			return fld;
		}

		public void SetValue(string value)
		{
			fieldValue = value;
			this.fieldHTML = Generate();
		}

		public static string RequiredFieldValidationJS
		{
			get { return "function(value){return value==''?'This is a required field':'';}"; }
		}
	}

	public class AjaxFormTextAreaField : AjaxFormStandardField
	{
		private bool readOnly;
		private string fieldValue, className, style;

		public AjaxFormTextAreaField(string fieldLabel, string fieldName, bool readOnly, string className, string style, string fieldValue, AjaxJavaScript runScript, string clientValidationFunctionReference, bool validateOnServer, int rank)
		{
			this.readOnly = readOnly;
			this.fieldValue = fieldValue;
			this.className = className;
			this.style = style;
			this.fieldName = fieldName;
			this.fieldLabel = fieldLabel;
			this.fieldName = fieldName;
			this.validateOnServer = validateOnServer;
			this.runScript = runScript;
			this.rank = rank;
			this.clientValidationFunctionReference = clientValidationFunctionReference;
			this.fieldHTML = Generate();
		}

		private string Generate()
		{
			string fld = "<textarea name=\"" + fieldName + "\" id=\"" + fieldName + "\"";
			if (className != null) fld += " class=\"" + className + "\"";
			if (style != null) fld += " style=\"" + style + "\"";
			if (readOnly) fld += " disabled=\"true\"";
			fld += ">";
			if (fieldValue != null) fld += HttpUtility.HtmlEncode(fieldValue);
			fld += "</textarea>";
			return fld;
		}

		public void SetValue(string value)
		{
			fieldValue = value;
			this.fieldHTML = Generate();
		}
	}

	/// <summary>
	/// Creates a dual-entry password field. The first field is called "Password1" and the second field is called "Password2".
	/// </summary>
	public class AjaxFormPasswordField : AjaxFormStandardField
	{
		public AjaxFormPasswordField(string fieldLabel, int? maxLength, string className, string style, int rank, bool multilingual, bool requiredField, bool allowBlankPassword)
		{
			string errNoMatch = multilingual ? "{?form-error-different-passwords?}" : "The passwords entered must match";
			string errNoPassword = multilingual ? "{?form-error-password-required?}" : "Please enter a password";
			string pwvf = "function(value){" +
				"if($('Password2').value != value) " +
				"return '" + errNoMatch + "'; " +
				"else if(value.length == 0) return '" + errNoPassword + "'; " +
				" else return null;}"
				;
			string pwvf2 = "function(value){" +
				"if($('Password2').value != value) " +
				"return '" + errNoMatch + "'; " +
				"else return null;}"
				;
			string fld = "<input type=\"password\" name=\"Password2\" id=\"Password2\"";
			if (maxLength != null) fld += " maxlength=\"" + maxLength + "\"";
			if (className != null) fld += " class=\"" + className + "\"";
			if (style != null) fld += " style=\"" + style + "\"";
			fld += " />";
			fld += "<input type=\"password\" name=\"Password1\" id=\"Password1\"";
			if (maxLength != null) fld += " maxlength=\"" + maxLength + "\"";
			if (className != null) fld += " class=\"" + className + "\"";
			if (style != null) fld += " style=\"" + style + "\"";
			fld += " />";
			SetFields(fieldLabel, "Password1", fld, null, allowBlankPassword ? pwvf2 : pwvf, true, rank);
		}
	}

	public class AjaxFormCheckboxField : AjaxFormStandardField
	{
		protected bool isChecked = false, readOnly = false;
		public AjaxFormCheckboxField(string fieldLabel, string fieldName, bool isChecked, bool readOnly, AjaxJavaScript runScript, string clientValidationFunctionReference, bool validateOnServer, int rank)
		{
			this.isChecked = isChecked;
			this.readOnly = readOnly;
			SetFields(fieldLabel, fieldName, null, runScript, clientValidationFunctionReference, validateOnServer, rank);
		}

		public override string FullFieldHTML
		{
			get
			{
				return string.Format("<div class=\"ajaxform-row\" id=\"ajaxform-row-{0}\"> " +
					"<span class=\"checkbox-field\" id=\"ajaxform-checkbox-field-{0}\">" +
						"<input type=\"checkbox\" name=\"{0}\" id=\"{0}\" value=\"true\" {4}{5}/>" +
					"</span>" +
					"<span class=\"checkbox-label\" id=\"ajaxform-label-{0}\">{1}</span>" +
					"<span class=\"checkbox-error\" id=\"ajaxform-error-{0}\">{3}</span>" +
					"<span class=\"rowend\"></span>" +
					"</div>", fieldName, fieldLabel, fieldHTML, fieldError,
					isChecked ? "checked=\"true\" " : "",
					readOnly ? "disabled=\"true\" " : ""
					);
			}
		}
	}

	public class AjaxFormSelectField : AjaxFormStandardField
	{
		public AjaxFormSelectField(string fieldLabel, string fieldName, AjaxJavaScript runScript,
			string clientValidationFunctionReference, int rank, string onchangeScript,
			string nothingSelectedLabel, string selectedValue)
		{
			SetFields(fieldLabel, fieldName, "", runScript, clientValidationFunctionReference, validateOnServer, rank);
			OnchangeScript = onchangeScript;
			NothingSelectedLabel = nothingSelectedLabel;
			SelectedValue = selectedValue;
		}

		private string nothingSelectedLabel = null;
		public string NothingSelectedLabel
		{
			get { return nothingSelectedLabel; }
			set { nothingSelectedLabel = value; }
		}

		private string selectedValue = null;
		public string SelectedValue
		{
			get { return selectedValue; }
			set { selectedValue = value; }
		}

		private List<KeyValuePair<string, string>> options = new List<KeyValuePair<string, string>>();
		public void AddOption(object value, object name)
		{
			options.Add(new KeyValuePair<string, string>(value.ToString(), name.ToString()));
		}

		private string onchangeScript = null;
		public string OnchangeScript
		{
			get { return onchangeScript; }
			set { onchangeScript = value; }
		}

		public override string FullFieldHTML
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (nothingSelectedLabel != null)
					sb.AppendFormat("<option value=\"{0}\">{1}</option>", "", nothingSelectedLabel);
				foreach (KeyValuePair<string, string> option in options)
					sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option>",
						option.Key, option.Key == selectedValue ? " selected " : "", option.Value);

				return string.Format("<div class=\"ajaxform-row\" id=\"ajaxform-row-{0}\"> " +
					"<span class=\"label\" id=\"ajaxform-label-{0}\">{1}</span>" +
					"<span class=\"field\" id=\"ajaxform-field-{0}\">" +
						"<select name=\"{0}\" id=\"{0}\" onchange=\"{4}\">{5}</select>" +
					"</span>" +
					"<span class=\"error\" id=\"ajaxform-error-{0}\">{3}</span>" +
					"<span class=\"rowend\"></span>" +
					"</div>", fieldName, fieldLabel, fieldHTML, fieldError, onchangeScript, sb);
			}
		}

		public override string FullFieldJavaScript
		{
			get
			{
				if (ClientValidationFunctionReference.Length == 0 && !validateOnServer)
					return "";
				return string.Format("{0}\r\nAjaxFormValidator.AddFieldValidationCheck('{1}','ajaxform-error-{1}',{2},{3});\r\n",
					runScript,
					FieldName,
					validateOnServer.ToString().ToLower(),
					(ClientValidationFunctionReference.Length == 0 ? "null" : ClientValidationFunctionReference));
			}
		}
	}

	public class AjaxFormButtonGroup : AjaxFormField
	{
		protected class Button
		{
			public string id, label, onclick;
			public List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string,string>>();
		}
		protected List<Button> buttons = new List<Button>();

		public void AddButton(string id, string label, string onclick, params KeyValuePair<string, string>[] attributes)
		{
			Button b = new Button();
			b.id = id;
			b.label = label;
			b.onclick = onclick;
			foreach (KeyValuePair<string, string> attr in attributes)
				b.attributes.Add(attr);
			buttons.Add(b);
		}

		public void AddSubmitButton(string id, string label, string onSuccessfulValidationJSFuncRef, string onclick, params KeyValuePair<string,string>[] attributes)
		{
			Button b = new Button();
			b.id = id;
			b.label = label;
			string funcref = onSuccessfulValidationJSFuncRef != null ? ", " + onSuccessfulValidationJSFuncRef : "";
			b.onclick = "if(!AjaxFormValidator.ValidateForm(this" + funcref + ")) return false;"
				+ (onclick != null ? " " + onclick : "");
			b.attributes.Add(new KeyValuePair<string, string>("formpart", "submit"));
			foreach (KeyValuePair<string, string> attr in attributes)
				b.attributes.Add(attr);
			buttons.Add(b);
		}

		public override string FullFieldHTML
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("<div class=\"ajaxform-buttongroup\">");
				foreach (Button b in buttons)
				{
					sb.Append("<input type=\"submit\"");
					if (b.id != null) sb.AppendFormat(" id=\"{0}\"", b.id);
					if (b.label != null) sb.AppendFormat(" value=\"{0}\"", HttpUtility.HtmlEncode(b.label));
					if (b.onclick != null) sb.AppendFormat(" onclick=\"{0}\"", b.onclick);
					sb.Append(" />");
				}
				sb.Append("</div>");
				return sb.ToString();
			}
		}
	}

	public class AjaxFormFieldBlock : List<AjaxFormField>, IRankable
	{
		private string name = "";
		private string label = null;

		public string Label
		{
			get { return label; }
			set { label = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public AjaxFormFieldBlock(string name)
		{
			this.name = name;
		}

		public AjaxFormFieldBlock(string name, string label)
		{
			this.name = name;
			this.label = label;
		}

		protected int rank = 0;
		public int Rank
		{
			set { rank = value; }
			get { return rank; }
		}

		private string cssClass = "ajaxformblock";
		protected string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		private string cssHeadingClass = "ajaxformblock-heading";
		protected string CssHeadingClass
		{
			get { return cssHeadingClass; }
			set { cssHeadingClass = value; }
		}

		private string blockID = null;
		public string BlockID
		{
			get { return blockID == null ? "ajaxform-block-" + Name : blockID; }
			set { blockID = value; }
		}

		public new void Sort()
		{
			this.Sort(RankedObject.SortByRank);
		}

		private string runScript = null;
		protected string RunScript
		{
			get { return runScript; }
			set { runScript = value; }
		}

		public void BuildHTML(AjaxFormType formType, StringWriter htmlWriter, StringWriter jsWriter, bool isAltBlock)
		{
			jsWriter.WriteLine(RunScript);
			htmlWriter.Write("<div id=\"{0}\" class=\"{1} {2}\" name=\"{3}\" formpart=\"block\">",
				BlockID, CssClass, isAltBlock ? "alt2" : "alt1", Name);
			if (Label != null)
				htmlWriter.Write("<div class=\"{0}\">{1}</div>", CssHeadingClass, Label);
			if (formType == AjaxFormType.FormPost)
				htmlWriter.Write("<input type=\"hidden\" name=\"_BlockName\" value=\"{0}\" />", Name);
			string fieldNames = "";
			foreach (AjaxFormField field in this)
			{
				if (formType == AjaxFormType.FormPost && field is AjaxFormStandardField)
				{
					AjaxFormStandardField f = (AjaxFormStandardField)field;
					if (fieldNames.Length > 0)
						fieldNames += "|";
					fieldNames += HttpUtility.HtmlEncode(f.FieldName);
					if (f.FieldName == "Password1")
						fieldNames += "|Password2";
				}
				htmlWriter.Write(field.FullFieldHTML);
				jsWriter.WriteLine(field.FullFieldJavaScript);
			}
			if (formType == AjaxFormType.FormPost)
				htmlWriter.Write("<input type=\"hidden\" name=\"_FieldNames_{0}\" value=\"{1}\" />", Name, fieldNames);

			htmlWriter.Write("</div>");
		}
	}

	public class AjaxFormFieldBlockList : List<AjaxFormFieldBlock>
	{
		public new void Sort()
		{
			this.Sort(RankedObject.SortByRank);
		}

		public override string ToString()
		{
			Sort();
			StringBuilder sb = new StringBuilder();
			foreach (AjaxFormFieldBlock b in this)
				sb.Append(b);
			return sb.ToString();
		}

		public AjaxFormFieldBlock this[string blockName]
		{
			get { return this[BinarySearch(null, new BlockComparer())]; }
		}

		public class BlockComparer : IComparer<AjaxFormFieldBlock>
		{
			public int Compare(AjaxFormFieldBlock x, AjaxFormFieldBlock y)
			{
				return x.Name.CompareTo(y.Name);
			}
		}

		public void BuildHTML(AjaxFormType formType, StringWriter htmlWriter, StringWriter jsWriter)
		{
			bool isAltBlock = false;
			foreach (AjaxFormFieldBlock block in this)
			{
				block.BuildHTML(formType, htmlWriter, jsWriter, isAltBlock);
				isAltBlock = !isAltBlock;
			}
		}
	}
}

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using Sprocket;
using Sprocket.Data;
using Sprocket.Utility;
using Sprocket.Web;
using Sprocket.Web.Controls;
using System.IO;
using System.Web;
using System.Diagnostics;

namespace Sprocket.Web.Controls
{
	[AjaxMethodHandler("AjaxFormHandler")]
	[ModuleDescription("Provides a centralised facility for constructing, processing and validating html forms.")]
	[ModuleTitle("Ajax Form Handler")]
	public class AjaxFormHandler : ISprocketModule
	{
		public event AjaxFormFieldValidationHandler OnValidateField;
		public event AjaxFormFieldValidationHandler OnAfterValidateField;
		public event AjaxFormSubmissionHandler OnValidateForm;
		public event AjaxFormSubmissionHandler OnAfterValidateForm;
		public event AjaxFormSubmissionHandler OnSaveForm;
		public event AjaxFormSubmissionHandler OnBeforeSaveForm;
		public event AjaxFormSubmissionHandler OnAfterSaveForm;

		public static AjaxFormHandler Instance
		{
			get { return (AjaxFormHandler)Core.Instance[typeof(AjaxFormHandler)].Module; }
		}

		#region ISprocketModule members
		public void AttachEventHandlers(ModuleRegistry registry)
		{
		}

		#endregion

		[AjaxMethod()]
		public AjaxFormFieldValidationResponse ValidateField(string formName, string fieldName, string fieldValue, long? recordID)
		{
			AjaxFormFieldValidationResponse resp = new AjaxFormFieldValidationResponse(formName, fieldName, fieldValue, recordID);
			if (OnValidateField != null)
				OnValidateField(resp);
			if (OnAfterValidateField != null)
				OnAfterValidateField(resp);
			return resp;
		}

		[AjaxMethod()]
		public AjaxFormSubmittedValues Submit(AjaxFormSubmittedValues form)
		{
			if (OnValidateForm != null)
				OnValidateForm(form);

			if (OnAfterValidateForm != null)
				OnAfterValidateForm(form);

			if(form.ErrorCount > 0)
				return form; // return the validation results

			if (OnBeforeSaveForm != null)
				OnBeforeSaveForm(form);

			if (OnSaveForm != null)
				OnSaveForm(form);

			if (OnAfterSaveForm != null)
				OnAfterSaveForm(form);

			return form;
		}
	}

	public class AjaxFormFieldValidationResponse : IJSONEncoder
	{
		private string msg = "";
		private bool isValid = true;
		private string formName = "";
		private string fieldName = "";
		private string fieldValue = "";
		private long? recordID;

		public AjaxFormFieldValidationResponse(string formName, string fieldName, string fieldValue, long? recordID)
		{
			this.formName = formName;
			this.fieldName = fieldName;
			this.fieldValue = fieldValue;
			this.recordID = recordID;
		}

		public long? RecordID
		{
			get { return recordID; }
			set { recordID = value; }
		}

		public string FormName
		{
			get { return formName; }
		}

		public string FieldName
		{
			get { return fieldName; }
		}

		public string FieldValue
		{
			get { return fieldValue; }
		}

		public string ErrorMessage
		{
			get { return msg; }
			set { msg = value; }
		}
		public bool IsValid
		{
			get { return isValid; }
			set { isValid = value; }
		}

		public void WriteJSON(StringWriter writer)
		{
			JSON.EncodeCustomObject(writer,
				new KeyValuePair<string, object>("FieldName", fieldName),
				new KeyValuePair<string, object>("IsValid", isValid),
				new KeyValuePair<string, object>("Message", msg)
			);
		}
	}

	public class AjaxForm : IJSONEncoder
	{
		private AjaxFormFieldBlockList fieldBlocks = new AjaxFormFieldBlockList();
		public AjaxFormFieldBlockList FieldBlocks
		{
			get { return fieldBlocks; }
		}

		public AjaxForm()
		{
		}

		public AjaxForm(string formName)
		{
			this.formName = formName;
		}

		private string formName = "";
		public string FormName
		{
			get { return formName; }
			set { formName = value; }
		}

		private object recordID = null;
		public object RecordID
		{
			get { return recordID; }
			set { recordID = value; }
		}

		private AjaxFormType ajaxFormType = AjaxFormType.FullAjax;
		public AjaxFormType FormType
		{
			get { return ajaxFormType; }
			set { ajaxFormType = value; }
		}

		private AjaxFormMethod ajaxFormMethod = AjaxFormMethod.Post;
		public AjaxFormMethod FormMethod
		{
			get { return ajaxFormMethod; }
			set { ajaxFormMethod = value; }
		}

		private AjaxFormEncType ajaxFormEncType = AjaxFormEncType.None;
		public AjaxFormEncType FormEncType
		{
			get { return ajaxFormEncType; }
			set { ajaxFormEncType = value; }
		}

		private string formAction = "";
		public string FormAction
		{
			get { return formAction; }
			set { formAction = value; }
		}

		private string cssClass = "ajaxform";
		public string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		private string formID = null;
		public string FormID
		{
			get { return formID == null ? formName : formID; }
			set { formID = value; }
		}

		public void PopulateValuesAndErrors(AjaxFormSubmittedValues values)
		{
			foreach(AjaxFormFieldBlock block in FieldBlocks)
				if(values.Blocks.ContainsKey(block.Name))
					foreach(AjaxFormField fld in block)
						if(fld is AjaxFormStandardField)
							if (values.Blocks[block.Name].Fields.ContainsKey(((AjaxFormStandardField)fld).FieldName))
							{
								AjaxFormStandardField f = (AjaxFormStandardField)fld;
								AjaxFormSubmittedValues.Field sfld = values.Blocks[block.Name].Fields[f.FieldName];
								if (sfld.ErrorMessage != null)
									f.FieldError = sfld.ErrorMessage;
								if (fld is AjaxFormInputField)
									((AjaxFormInputField)fld).SetValue(sfld.Value);
							}
		}

		public void WriteJSON(StringWriter writer)
		{
			StringWriter html = new StringWriter();
			StringWriter js = new StringWriter();
			BuildForm(html, js);
			JSON.EncodeCustomObject(writer,
				new KeyValuePair<string, object>("HTML", html.ToString()),
				new KeyValuePair<string, object>("JavaScript", js.ToString())
			);
		}

		public void BuildForm(StringWriter htmlWriter, StringWriter jsWriter)
		{
			switch (FormType)
			{
				case AjaxFormType.FormPost:
					htmlWriter.Write(
						"<form method=\"{0}\" action=\"{1}\" id=\"{2}\" name=\"{3}\" enctype=\"{4}\" class=\"{5}\" " +
						"recordid=\"{6}\" formpart=\"form\"><input type=\"hidden\" name=\"RecordID\" value=\"{6}\" />" +
						"<input type=\"hidden\" name=\"FormName\" value=\"{3}\" />",
						FormMethod, FormAction, FormID, HttpUtility.HtmlEncode(FormName),
						FormEncType == AjaxFormEncType.None ? "application/x-www-form-urlencoded" : "multipart/form-data",
						CssClass, RecordID);
					FieldBlocks.Sort(RankedObject.SortByRank);
					fieldBlocks.BuildHTML(FormType, htmlWriter, jsWriter);
					htmlWriter.Write("</form>");
					break;

				case AjaxFormType.FullAjax:
					htmlWriter.Write("<div id=\"{0}\" class=\"{1}\" name=\"{2}\" recordid=\"{3}\" formpart=\"form\">",
						FormID, CssClass, FormName, RecordID);
					FieldBlocks.Sort(RankedObject.SortByRank);
					fieldBlocks.BuildHTML(FormType, htmlWriter, jsWriter);
					htmlWriter.Write("</div>");
					break;
			}
		}

		public string BuildForm()
		{
			StringWriter htmlWriter = new StringWriter();
			StringWriter jsWriter = new StringWriter();
			BuildForm(htmlWriter, jsWriter);
			return string.Format("{1}{0}{1}<script>{1}{2}{1}</script>{1}", htmlWriter, Environment.NewLine, jsWriter);
		}
	}

	public enum AjaxFormType
	{
		FullAjax,
		FormPost
	}

	public enum AjaxFormMethod
	{
		Post,
		Get
	}

	public enum AjaxFormEncType
	{
		None,
		MultiPartFormData
	}

	public class AjaxFormSubmittedValues : IJSONReader, IJSONEncoder
	{
		public int ErrorCount
		{
			get
			{
				int c = 0;
				foreach (Block block in blocks.Values)
					foreach (Field field in block.Fields.Values)
						if (field.ErrorMessage != null)
							c++;
				return c;
			}
		}

		private string formName = "";
		public string FormName
		{
			get { return formName; }
		}

		private long? recordID = null;
		public long? RecordID
		{
			get { return recordID; }
			set { recordID = value; }
		}

		private Dictionary<string, Block> blocks = new Dictionary<string, Block>();
		public Dictionary<string, Block> Blocks
		{
			get { return blocks; }
		}

		public class Block : IJSONEncoder
		{
			public Block(string name)
			{
				this.name = name;
			}

			private string name;
			public string Name
			{
				get { return name; }
			}

			private Dictionary<string, Field> fields = new Dictionary<string, Field>();
			public Dictionary<string, Field> Fields
			{
				get { return fields; }
			}

			public void WriteJSON(StringWriter writer)
			{
				writer.Write("{");
				JSON.EncodeNameValuePair(writer, "Name", Name);
				writer.Write(",\"Fields\":[");
				int c = 0;
				foreach (Field field in fields.Values)
				{
					if (c++ > 0) writer.Write(",");
					JSON.Encode(writer, field);
				}
				writer.Write("]}");
			}
		}

		public class Field : IJSONEncoder
		{
			private string name, value, error=null;

			public Field(string name, string value)
			{
				this.name = name;
				this.value = value;
			}

			public string Value
			{
				get { return this.value; }
			}

			public string Name
			{
				get { return name; }
			}

			public string ErrorMessage
			{
				get { return error; }
				set { error = value; }
			}

			public void WriteJSON(StringWriter writer)
			{
				JSON.EncodeCustomObject(writer,
					new KeyValuePair<string, object>("Name", Name),
					new KeyValuePair<string, object>("Value", Value),
					new KeyValuePair<string, object>("ErrorMessage", ErrorMessage)
				);
			}
		}

		public void LoadJSON(object json)
		{
			Dictionary<string, object> form = (Dictionary<string, object>)json;
			formName = form["Name"].ToString();
			if (form["RecordID"] != null)
				recordID = long.Parse(form["RecordID"].ToString());
			else
				recordID = null;

			foreach (KeyValuePair<string, object> b in (Dictionary<string, object>)form["FieldBlocks"])
			{
				Block block = new Block(b.Key);
				Dictionary<string, object> values = (Dictionary<string, object>)b.Value;
				foreach (KeyValuePair<string, object> fld in values)
					block.Fields.Add(fld.Key, new Field(fld.Key, fld.Value.ToString()));
				blocks.Add(block.Name, block);
			}
		}

		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "FormName", FormName);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "RecordID", recordID);
			writer.Write(",");
			JSON.EncodeString(writer, "Blocks");
			writer.Write(":");
			JSON.EncodeEnumerated(writer, blocks.Values);
			writer.Write("}");
		}

		public bool ReadPostedForm()
		{
			HttpRequest Request = HttpContext.Current.Request;
			if (Request.Form["RecordID"] == null)
				return false;
			formName = Request.Form["FormName"];
			recordID = Request.Form["RecordID"] == "" ? (long?)null : long.Parse(Request.Form["RecordID"]);
			foreach (string blockName in Request.Form.GetValues("_BlockName"))
			{
				Block block = new Block(blockName);
				foreach (string fieldName in Request.Form["_FieldNames_" + blockName].Split('|'))
					block.Fields.Add(fieldName, new Field(fieldName, Request.Form[fieldName]));
				if(block.Fields.Count > 0)
					blocks.Add(blockName, block);
			}
			return true;
		}
	}

	public delegate void AjaxFormSubmissionHandler(AjaxFormSubmittedValues form);
	public delegate void AjaxFormFieldValidationHandler(AjaxFormFieldValidationResponse formArgs);
}

using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Utility;

namespace Sprocket.Web.Controls
{
	public enum WebControlScript
	{
		AjaxForm,
		TabStrip,
		Fader,
		Splitter
	}

	public static class WebControls
	{
		public static string TabStrip
		{
			get { return ResourceLoader.LoadTextResource("Sprocket.Web.Controls.tabstrip.js"); }
		}

		public static string AjaxForm
		{
			get { return ResourceLoader.LoadTextResource("Sprocket.Web.Controls.AjaxForm.AjaxForm.js"); }
		}

		public static string Fader
		{
			get { return ResourceLoader.LoadTextResource("Sprocket.Web.Controls.opacity.js"); }
		}

		public static string Splitter
		{
			get { return ResourceLoader.LoadTextResource("Sprocket.Web.Controls.splitter.js"); }
		}

		public static string GetScript(WebControlScript script)
		{
			switch (script)
			{
				case WebControlScript.TabStrip: return TabStrip;
				case WebControlScript.AjaxForm: return AjaxForm;
				case WebControlScript.Fader: return Fader;
				case WebControlScript.Splitter: return Splitter;
				default: return "";
			}
		}

		public static string GetScriptTags(WebControlScript script)
		{
			return "<script>" + GetScript(script) + "</script>";
		}
	}
}

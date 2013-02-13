using System;
using System.Collections.Generic;
using System.Text;
using Sprocket;
using Sprocket.Web;
using Sprocket.Web.Controls;

namespace Sprocket.Web.CMS.Admin
{
	public class AdminMenuLink : IRankable
	{
		private string name, link;
		private int rank;

		public AdminMenuLink(string name, string link, int rank)
		{
			this.name = name;
			this.link = link;
			this.rank = rank;
		}

		public int Rank
		{
			set { rank = value; }
			get { return rank; }
		}

		public virtual string Render()
		{
			if(link == "" || link == null)
				return name;

			return string.Format("<a class=\"AdminMenuLink\" id=\"AdminMenuLink_{2}\" href=\"{0}\">{1}</a>",
				link, name, link.Trim('/').Replace(':','_').Replace('/','_').Replace('\\','_').Replace('.','_').Replace(' ','_'));
		}
	}

	public class RankedString : IRankable
	{
		private string text;
		private int rank;

		public RankedString(string text, int rank)
		{
			this.text = text;
			this.rank = rank;
		}

		public RankedString(object text, int rank)
		{
			this.text = text.ToString();
			this.rank = rank;
		}

		public int Rank
		{
			set { rank = value; }
			get { return rank; }
		}

		public virtual string Render()
		{
			return text;
		}
	}

	public class AdminInterface
	{
		private List<AdminMenuLink> menuLinks = new List<AdminMenuLink>();
		private List<AdminMenuLink> footerLinks = new List<AdminMenuLink>();
		private List<RankedString> contentSections = new List<RankedString>();
		private List<RankedString> headSections = new List<RankedString>();
		private List<RankedString> menuSections = new List<RankedString>();
		private List<RankedString> bodyOnLoadScripts = new List<RankedString>();
		private List<RankedString> interfaceScripts = new List<RankedString>();
		private string contentHeading = "";
		private string websiteName = "";

		public string WebsiteName
		{
			get { return websiteName; }
			set { websiteName = value; }
		}

		public string ContentHeading
		{
			get { return contentHeading; }
			set { contentHeading = value; }
		}

		protected Dictionary<WebControlScript, bool> loadedScripts = new Dictionary<WebControlScript, bool>();
		public void AddInterfaceScript(WebControlScript script)
		{
			if (loadedScripts.ContainsKey(script)) return;
			loadedScripts.Add(script, true);
			interfaceScripts.Add(new RankedString(WebControls.GetScript(script), -1));
		}

		public void AddInterfaceScript(RankedString script)
		{
			interfaceScripts.Add(script);
		}

		public void AddMainMenuLink(AdminMenuLink link)
		{
			menuLinks.Add(link);
		}

		public void AddFooterLink(AdminMenuLink link)
		{
			footerLinks.Add(link);
		}

		public void AddContentSection(RankedString section)
		{
			contentSections.Add(section);
		}

		public void AddHeadSection(RankedString section)
		{
			headSections.Add(section);
		}

		public void AddLeftColumnSection(RankedString section)
		{
			menuSections.Add(section);
		}

		public void AddBodyOnLoadScript(RankedString script)
		{
			bodyOnLoadScripts.Add(script);
		}

		public string Render()
		{
			interfaceScripts.Sort(RankedObject.SortByRank);
			JavaScriptCollection jsc = new JavaScriptCollection();
			int scrnum = 0;
			foreach (RankedString str in interfaceScripts)
				jsc.Add("sprocket-admin-script-" + scrnum++, str.Render());
			headSections.Add(new RankedString(jsc.CreateScriptTags(), 1000));

			menuLinks.Sort(RankedObject.SortByRank);
			footerLinks.Sort(RankedObject.SortByRank);
			contentSections.Sort(RankedObject.SortByRank);
			headSections.Sort(RankedObject.SortByRank);
			bodyOnLoadScripts.Sort(RankedObject.SortByRank);

			StringBuilder menu = new StringBuilder();
			foreach (AdminMenuLink link in menuLinks)
				menu.AppendFormat("<div id=\"main-menu\">{0}</div>", link.Render());

			menuSections.Add(new RankedString(menu.ToString(), 0));
			menuSections.Sort(RankedObject.SortByRank);

			StringBuilder footer = new StringBuilder();
			foreach (AdminMenuLink link in footerLinks)
			{
				if (footer.Length > 0)
					footer.Append(" | ");
				footer.Append(link.Render());
			}

			StringBuilder content = new StringBuilder();
			foreach (RankedString section in contentSections)
				content.Append(section.Render());

			StringBuilder head = new StringBuilder();
			foreach (RankedString section in headSections)
				head.Append(section.Render());

			StringBuilder left = new StringBuilder();
			foreach (RankedString section in menuSections)
				left.Append(section.Render());

			StringBuilder onLoad = new StringBuilder();
			foreach (RankedString script in bodyOnLoadScripts)
			{
				string scr = script.Render();
				if (!scr.Trim().EndsWith(";"))
					scr += ";";
				onLoad.Append(scr);
			}

			string html = WebUtility.CacheTextFile("resources/admin/admin.htm");
			html = html.Replace("{website-name}", websiteName);
			html = html.Replace("{head}", head.ToString());
			html = html.Replace("//{onload}", onLoad.ToString());
			html = html.Replace("{main-menu}", left.ToString());
			html = html.Replace("{main-content}", content.ToString());
			html = html.Replace("{section-heading}", contentHeading);
			html = html.Replace("{footer}", footer.ToString());
			html = html.Replace("{basepath}", WebUtility.BasePath);
			return html;
		}
	}
}

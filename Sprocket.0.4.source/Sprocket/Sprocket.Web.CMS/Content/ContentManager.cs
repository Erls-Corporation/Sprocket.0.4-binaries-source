using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using Sprocket.Web.CMS.Script;
using Sprocket.Utility;

namespace Sprocket.Web.CMS.Content
{
	[ModuleTitle("Content Manager")]
	[ModuleDescription("The content management engine that handles content, pages and the templates they use")]
	[ModuleDependency(typeof(WebEvents))]
	[AjaxMethodHandler("ContentManager")]
	public sealed class ContentManager : ISprocketModule
	{
		public static ContentManager Instance
		{
			get { return (ContentManager)Core.Instance[typeof(ContentManager)].Module; }
		}

		public delegate void BeforeRenderPage(PageEntry page);
		public event BeforeRenderPage OnBeforeRenderPage;

		private class StateValues
		{
			public string XmlSprocketPath = "resources/definitions.xml";
			public string XmlPath = null;
			public XmlDocument MainXml = null;
			public DateTime LastXmlFileUpdate = DateTime.MinValue;
			public ContentManager.TemplateRegistry Templates = null;
			public ContentManager.PageRegistry Pages = null;
			public Stack<PageEntry> PageStack = new Stack<PageEntry>();
		}
		private StateValues stateValues = new StateValues();
		private static StateValues Values
		{
			get { return Instance.stateValues; }
		}

		public static XmlDocument DefinitionsXml
		{
			get
			{
				lock (WebUtility.GetSyncObject("Sprocket.Web.CMS.Content.ContentManager.MainXml"))
				{
					if (Values.XmlPath == null)
						Values.XmlPath = WebUtility.MapPath(Values.XmlSprocketPath);
					if (!File.Exists(Values.XmlPath))
						using (StreamWriter sw = new StreamWriter(Values.XmlPath))
						{
							sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<Definitions>\r\n\t<Pages />\r\n\t<Templates />\r\n</Definitions>");
							sw.Flush();
							sw.Close();
						}
					if (IsDefinitionsXmlOutOfDate || Values.MainXml == null)
					{
						Values.Templates = null;
						Values.Pages = null;
						Values.MainXml = new XmlDocument();
						Values.MainXml.Load(Values.XmlPath);
						Values.LastXmlFileUpdate = new FileInfo(Values.XmlPath).LastWriteTime;
					}
				}
				return Values.MainXml;
			}
		}

		public static bool IsDefinitionsXmlOutOfDate
		{
			get
			{
				if (Values.XmlPath == null)
					return true;
				return new FileInfo(Values.XmlPath).LastWriteTime != Values.LastXmlFileUpdate;
			}
		}

		#region Template and Page Registries
		public static TemplateRegistry Templates
		{
			get
			{
				if (Values.Templates == null)
					Values.Templates = new TemplateRegistry();
				return Values.Templates;
			}
		}
		public static PageRegistry Pages
		{
			get
			{
				if (Values.Pages == null)
					Values.Pages = new PageRegistry();
				return Values.Pages;
			}
		}
		public class TemplateRegistry
		{
			Dictionary<string, XmlElement> templateXML = new Dictionary<string, XmlElement>();
			Dictionary<string, Template> templates = new Dictionary<string, Template>();

			public TemplateRegistry()
			{
				Init();
			}

			public void Init()
			{
				foreach (XmlElement xml in ContentManager.DefinitionsXml.SelectNodes("/Definitions/Templates/Template"))
				{
					if (!xml.HasAttribute("Name"))
						continue;
					templateXML.Add(xml.GetAttribute("Name"), xml);
				}
			}

			public Template this[string id]
			{
				get
				{
					if (templates.ContainsKey(id))
					{
						if (templates[id].IsOutOfDate || templates[id].Script.HasParseError)
							templates[id] = BuildTemplateScript(id);
					}
					else
						templates[id] = BuildTemplateScript(id);
					return templates[id];
				}
			}

			private Template BuildTemplateScript(string name)
			{
				Dictionary<string, DateTime> fileTimes = new Dictionary<string, DateTime>();
				SprocketScript script = BuildTemplateScript(name, fileTimes);
				return new Template(name, script, fileTimes);
			}

			private SprocketScript BuildTemplateScript(string name, Dictionary<string, DateTime> fileTimes)
			{
				return BuildTemplateScript(name, new Stack<string>(), fileTimes);
			}

			private SprocketScript BuildTemplateScript(string name, Stack<string> embedStack, Dictionary<string, DateTime> fileTimes)
			{
				if(embedStack.Contains(name))
					return new SprocketScript("[Circular dependency detected in template heirarchy at \"" + name + "\"]", "Template: " + name, "Template: " + name);
				embedStack.Push(name);

				if (!templateXML.ContainsKey(name))
					return new SprocketScript("[There was no template found named \"" + name + "\"]", "Template: " + name, "Template: " + name); ;
				XmlElement xml = templateXML[name];

				SprocketScript script = null;
				if (xml.HasAttribute("Master"))
				{
					script = BuildTemplateScript(xml.GetAttribute("Master"), embedStack, fileTimes);
					foreach (XmlElement replace in xml.SelectNodes("Replace[@Section]"))
					{
						string sectionName = replace.GetAttribute("Section");
						if (replace.HasAttribute("File"))
						{
							string sprocketPath = replace.GetAttribute("File");
							string path = WebUtility.MapPath(sprocketPath);
							if (File.Exists(path))
							{
								fileTimes[path] = new FileInfo(path).LastWriteTime;
								using (StreamReader reader = new StreamReader(path))
								{
									script.OverrideSection(sectionName, new SprocketScript(reader.ReadToEnd(), sprocketPath, sprocketPath));
									reader.Close();
								}
							}
							else
								script.OverrideSection(sectionName, new SprocketScript("[Unable to replace section \"" + sectionName + "\". The referenced file doesn't exist]", name, name));
						}
						else if(replace.HasAttribute("Template"))
							script.OverrideSection(sectionName, BuildTemplateScript(replace.GetAttribute("Template"), embedStack, fileTimes));
						else
						{
							if(replace.HasChildNodes)
								script.OverrideSection(sectionName, new SprocketScript(replace.FirstChild.Value, "Template Section " + sectionName, "Template Section " + sectionName));
						}
					}
				}
				else if (xml.HasAttribute("File"))
				{
					//	return new SprocketScript("[The template \"" + name + "\" is lacking a Master or File attribute]", "Template: " + name, "Template: " + name);
					string path = WebUtility.MapPath(xml.GetAttribute("File"));
					if (!File.Exists(path))
						return new SprocketScript("[The template \"" + name + "\" references a nonexistant file]", "Template: " + name, "Template: " + name);
					fileTimes[path] = new FileInfo(path).LastWriteTime;
					using (StreamReader reader = new StreamReader(path))
					{
						script = new SprocketScript(reader.ReadToEnd(), "Template: " + name, "Template: " + name);
						reader.Close();
					}
				}
				else
				{
					if(xml.ChildNodes.Count > 0)
						if(xml.FirstChild.NodeType == XmlNodeType.CDATA || xml.FirstChild.NodeType == XmlNodeType.Text)
							script = new SprocketScript(xml.FirstChild.Value, "Template: " + name, "Template: " + name);
					if(script == null)
						script = new SprocketScript(xml.InnerText, "Template: " + name, "Template: " + name);
				}

				embedStack.Pop();
				return script;
			}
		}
		public class PageRegistry
		{
			private List<PageEntry> pages = null;
			private Dictionary<string, PageEntry> requestPaths = null;
			private Dictionary<string, PageEntry> pageCodes = null;
			private Dictionary<string, PageEntry> contentFiles = null;
			private List<PageEntry> flexiblePaths = null;
			private DateTime fileDate = DateTime.MinValue;

			public PageRegistry()
			{
				Init();
			}

			private void Init()
			{
				pages = new List<PageEntry>();
				requestPaths = new Dictionary<string, PageEntry>();
				pageCodes = new Dictionary<string, PageEntry>();
				contentFiles = new Dictionary<string, PageEntry>();
				flexiblePaths = new List<PageEntry>();
				foreach (XmlNode node in ContentManager.DefinitionsXml.SelectNodes("/Definitions/Pages/*"))
					if (node is XmlElement)
						LoadEntry((XmlElement)node, null);
			}

			private PageEntry LoadEntry(XmlElement xml, PageEntry parent)
			{
				PageEntry pageEntry = new PageEntry();
				pageEntry.Parent = parent;
				pageEntry.TemplateName = xml.GetAttribute("Template");
				pageEntry.ContentFile = xml.GetAttribute("ContentFile");
				if (xml.HasAttribute("ContentType"))
					pageEntry.ContentType = xml.GetAttribute("ContentType");

				if (xml.HasAttribute("Path"))
				{
					pageEntry.Path = xml.GetAttribute("Path").ToLower();
					requestPaths[pageEntry.Path] = pageEntry;
					if (xml.HasAttribute("HandleSubPaths"))
					{
						pageEntry.HandleSubPaths = StringUtilities.BoolFromString(xml.GetAttribute("HandleSubPaths"));
						flexiblePaths.Add(pageEntry);
					}
				}
				if (xml.HasAttribute("Code"))
				{
					pageEntry.PageCode = xml.GetAttribute("Code");
					pageCodes[pageEntry.PageCode] = pageEntry;
				}
				if (xml.HasAttribute("ContentFile"))
				{
					pageEntry.ContentFile = xml.GetAttribute("ContentFile");
					contentFiles[pageEntry.ContentFile] = pageEntry;
				}

				foreach (XmlElement node in xml.ChildNodes)
					pageEntry.Pages.Add(LoadEntry(node, pageEntry));

				pages.Add(pageEntry);
				return pageEntry;
			}

			public PageEntry FromPath(string sprocketPath)
			{
				if (requestPaths.ContainsKey(sprocketPath))
					return requestPaths[sprocketPath];
				foreach (PageEntry page in flexiblePaths)
					if (SprocketPath.IsCurrentPathDescendentOf(page.Path))
						return page;
				return null;
			}

			public PageEntry FromPageCode(string pageCode)
			{
				if (pageCodes.ContainsKey(pageCode))
					return pageCodes[pageCode];
				return null;
			}

			public PageEntry FromContentFile(string contentFile)
			{
				if (contentFiles.ContainsKey(contentFile))
					return contentFiles[contentFile];
				return null;
			}

			public List<PageEntry> SelectPages(string xpath)
			{
				List<PageEntry> list = new List<PageEntry>();
				XmlNodeList nodes = ContentManager.DefinitionsXml.SelectSingleNode("/Definitions/Pages").SelectNodes(xpath);
				foreach (XmlNode node in nodes)
				{
					if (node.NodeType != XmlNodeType.Element)
						continue;

					XmlElement e = (XmlElement)node;
					PageEntry page = null;
					if (e.HasAttribute("PageCode"))
						page = FromPageCode(e.GetAttribute("PageCode"));
					else if (e.HasAttribute("Path"))
						page = FromPath(e.GetAttribute("Path"));
					else if (e.HasAttribute("ContentFile"))
						page = FromContentFile(e.GetAttribute("ContentFile"));

					if (page != null)
						list.Add(page);
				}
				return list;
			}

			public PageEntry SelectSinglePage(string xpath)
			{
				XmlNode node = ContentManager.DefinitionsXml.SelectSingleNode("/Definitions/Pages").SelectSingleNode(xpath);
				if (node == null)
					return null;
				if (!(node is XmlElement))
					return null;
				XmlElement e = (XmlElement)node;
				if (e.HasAttribute("PageCode"))
					return FromPageCode(e.GetAttribute("PageCode"));
				return FromPath(e.GetAttribute("Path"));
			}
		}
		#endregion

		public static Stack<PageEntry> PageStack
		{
			get { return Values.PageStack; }
		}

		private PageEntry requestedPage = null;
		public static PageEntry RequestedPage
		{
			get { return Instance.requestedPage; }
		}

		HttpRequest Request { get { return HttpContext.Current.Request; } }
		HttpResponse Response { get { return HttpContext.Current.Response; } }

		#region Module Event Handlers
		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnBeginHttpRequest += new WebEvents.HttpApplicationCancellableEventHandler(WebEvents_OnBeginHttpRequest);
			WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(WebEvents_OnLoadRequestedPath);
			WebEvents.Instance.OnPathNotFound += new WebEvents.RequestedPathEventHandler(WebEvents_OnPathNotFound);
			WebEvents.Instance.OnEndHttpRequest += new WebEvents.HttpApplicationEventHandler(WebEvents_OnEndHttpRequest);
		}

		void WebEvents_OnBeginHttpRequest(HandleFlag handled)
		{
			RequestSpeedExpression.Set();
			Values.PageStack.Clear();
			if (IsDefinitionsXmlOutOfDate)
			{
				Values.Templates = null;
				Values.Pages = null;
			}
		}

		void WebEvents_OnPathNotFound(HandleFlag handled)
		{
			#region Map missing referenced files (e.g. images and css) to the same location as the content file

			if (!SprocketPath.Value.Contains("."))
			{
				HttpContext.Current.Response.Write(ResourceLoader.LoadTextResource("Sprocket.Web.CMS.Content.404.htm"));
				handled.Set();
				return;
			}
			string urlpath;
			if (SprocketPath.Sections.Length == 1)
				urlpath = "";
			else
				urlpath = SprocketPath.Value.Substring(0, SprocketPath.Value.Length - SprocketPath.Sections[SprocketPath.Sections.Length - 1].Length - 1);

			PageEntry page = Pages.FromPath(urlpath);
			if (page == null) return;
			string newurl = page.ContentFile;
			newurl = WebUtility.BasePath + newurl.Substring(0, newurl.LastIndexOf('/') + 1) + SprocketPath.Sections[SprocketPath.Sections.Length - 1];
			if (!File.Exists(HttpContext.Current.Server.MapPath(newurl)))
				return;
			HttpContext.Current.Response.TransmitFile(HttpContext.Current.Server.MapPath(newurl));
			handled.Set();

			#endregion
		}

		void WebEvents_OnLoadRequestedPath(HandleFlag handled)
		{
			requestedPage = null;
			if (handled.Handled) return;
			PageEntry page = Pages.FromPath(SprocketPath.Value);
			if (page == null)
				return;
			requestedPage = page;
			if (OnBeforeRenderPage != null)
				OnBeforeRenderPage(page);
			string txt = page.Render();
			Response.ContentType = page.ContentType;
			Response.Write(txt);
			handled.Set();
		}

		void WebEvents_OnEndHttpRequest()
		{
			PageStack.Clear();
			requestedPage = null;
		}
		#endregion

		#region Ajax Methods

		[AjaxMethod(RequiresAuthentication = true)]
		public string ShowAdminInterface()
		{
			return "interface";
		}

		#endregion
	}
}

using System;
using System.Collections;
using Sprocket;

namespace Sprocket.Web.Controls
{
	/// <summary>
	/// This generates an html navigation trail for a set of pages, in the format:
	/// Previous | 1 | ... | 6 | 7 | 8 | 9 | 10 | ... | 27 | Next
	/// </summary>
	public class MultiPageNav
	{
		private string divider, between, url, nextword, prevword, nav;
		private int pages, currentpage, pageslots;
		private bool generate = true;
		private Hashtable urlOverrides = new Hashtable();
		public MultiPageNav(int pages, int currentpage, int pageslots)
		{
			divider = " | ";
			between = "...";
			url = "?page={0}";
			nextword = "Next";
			prevword = "Previous";
			nav = "";
			this.pages = pages;
			this.currentpage = currentpage;
			this.pageslots = pageslots;
		}

		public override string ToString()
		{
			if (generate)
			{
				generate = false;
				Generate();
			}
			return nav;
		}

		public void OverrideUrl(int pageIndex, string url)
		{
			if (urlOverrides.ContainsKey(pageIndex))
				urlOverrides[pageIndex] = url;
			else
				urlOverrides.Add(pageIndex, url);
		}

		private void Generate()
		{
			if (pages == 0)
			{
				nav = "";
				return;
			}

			//"previous" link
			if (currentpage == 1)
				nav = "<span class=\"disabled\">" + prevword + "</span>";
			else
				nav = "<a href=\"" + string.Format(url, currentpage - 1) + "\">" + prevword + "</a>";

			//page links
			int x1 = ((pageslots - 4) / 2) + 1; //position left of current to show ...
			int x2 = ((pageslots - 4) / 2) + ((pageslots - 4) % 2); //position right of current to show ...
			x2 += Math.Max((pageslots / 2) + 1 - currentpage, 0); //extra pages to be displayed on left
			x1 += Math.Max((x2 + 1) - (pages - currentpage), 0); //extra pages to be displayed on right

			for (int i = 1; i <= pages; i++)
			{
				nav += divider;

				if (i == currentpage)
					nav += i.ToString();
				else
				{
					//if(i == 1 || i == pages)
					if (pages > pageslots && i == 2 && currentpage - x1 > 2)
					{
						nav += between;
						i = currentpage - x1;
					}
					else if (pages > pageslots && i == currentpage + x2 && currentpage + x2 < pages)
					{
						nav += between;
						i = pages - 1;
					}
					else
						nav += string.Format("<a href=\"{0}\">{1}</a>", urlOverrides.ContainsKey(i) ? urlOverrides[i] : string.Format(url, i), i);
				}
			}

			//"next" link
			nav += divider;
			if (currentpage == pages)
				nav += "<span class=\"disabled\">" + nextword + "</span>";
			else
				nav += "<a href=\"" + string.Format(url, currentpage + 1) + "\">" + nextword + "</a>";
		}

		//Properties

		public string Divider
		{
			get { return divider; }
			set { divider = value; generate = true; }
		}

		public string Between
		{
			get { return between; }
			set { between = value; generate = true; }
		}

		public string Url
		{
			get { return url; }
			set
			{
				url = value;
				if (url.IndexOf("{0}") == -1)
					throw new SprocketException("MultiPageNav class requires a URL with at least one {0} parameter for the page number to be generated in.");
				generate = true;
			}
		}

		public string NextWord
		{
			get { return nextword; }
			set { nextword = value; generate = true; }
		}

		public string PrevWord
		{
			get { return prevword; }
			set { prevword = value; generate = true; }
		}
	}
}

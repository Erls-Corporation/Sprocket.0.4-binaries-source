using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Admin;

namespace Sprocket.Web.CMS.Content
{
	class PagesAdminMenuItem : IAdminMenuItem
	{
		public string MenuLinkText
		{
			get { return "pages and templates"; }
		}

		public string MenuLinkOnClick
		{
			get { return "InitPagesAndTemplates()"; }
		}

		public string HeadContent
		{
			get
			{
				return "<script type=\"text/javascript\" src=\"" + WebUtility.BasePath + "resources/admin/pages/pages.js\"></script>\r\n"
					+ "<link type=\"text/css\" rel=\"stylesheet\" href=\"" + WebUtility.BasePath + "resources/admin/pages/pages.css\" />";
			}
		}

		public AdminMenuPriority Priority
		{
			get { return AdminMenuPriority.Middle; }
		}

		#region IAdminMenuItem Members

		public string IconPath
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	}
}

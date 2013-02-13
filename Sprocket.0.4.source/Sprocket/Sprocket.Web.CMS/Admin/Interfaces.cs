using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.CMS.Admin
{
	public interface IAdminMenuItem
	{
		string MenuLinkText { get; }
		string MenuLinkOnClick { get; }
		string HeadContent { get; }
		string IconPath { get; }
		AdminMenuPriority Priority { get; }
	}

	public enum AdminMenuPriority
	{
		Start = 3,
		Middle = 2,
		End = 1
	}
}

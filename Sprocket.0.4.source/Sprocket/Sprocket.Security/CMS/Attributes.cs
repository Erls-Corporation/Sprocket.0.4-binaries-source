using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.CMS.Security
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class RequiresRoleAttribute : Attribute
	{
		private string roleCode;

		public string RoleCode
		{
			get { return roleCode; }
		}

		public RequiresRoleAttribute(string roleCode)
		{
			this.roleCode = roleCode;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class RequiresPermissionAttribute : Attribute
	{
		private string permissionTypeCode;

		public string PermissionTypeCode
		{
			get { return permissionTypeCode; }
		}

		public RequiresPermissionAttribute(string permissionTypeCode)
		{
			this.permissionTypeCode = permissionTypeCode;
		}
	}
}

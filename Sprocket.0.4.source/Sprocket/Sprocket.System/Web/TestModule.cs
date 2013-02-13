using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.SqlClient;

using Sprocket;
using Sprocket.Data;
using Sprocket.Utility;

namespace Sprocket.Web
{
	//[AjaxMethodHandler("TestModule")]
	//[ModuleDependency(typeof(WebEvents))]
	//[ModuleDescription("A module for writing test code.")]
	//[ModuleTitle("Testing Module")]
	//public class TestModule : ISprocketModule
	//{
	//    public void AttachEventHandlers(ModuleRegistry registry)
	//    {
	//        WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(OnLoadRequestedPath);
	//    }

	//    void OnLoadRequestedPath(HandleFlag handled)
	//    {
	//        switch (path)
	//        {
	//            default:
	//                return;
	//        }
	//        handled.Set();
	//    }
	//}
}
